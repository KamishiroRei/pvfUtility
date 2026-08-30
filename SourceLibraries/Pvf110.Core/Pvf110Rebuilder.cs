using System.IO.Compression;

namespace Pvf110.Core;

/// <summary>
/// Pvf110 逻辑流重建（内容修改 → 新 Script.pvf + sk.dat）。
/// 保留 name/hash 表与 entry 结构，重建 file table dataOffset/dataSize、group 表与 body。
/// 阶段约束：不增删文件、不改路径、type-1 内容字符串须在 name 池内（否则抛异常）。
/// </summary>
public static class Pvf110Rebuilder
{
    public static (byte[] scriptPvf, byte[] skdat) Rebuild(
        Pvf110Reader original,
        Func<Pvf110Entry, byte[]> getContent,
        IProgress<double>? progress = null)
    {
        // 1. 收集每个 entry 的新内容（getContent 已返回可直接进 body 的字节）
        int n = original.Entries.Count;
        var contents = new byte[n][];
        for (int i = 0; i < n; i++)
            contents[i] = getContent(original.Entries[i]);

        // 2. 按 chunk 分组（保留 entry 顺序）
        var byChunk = new Dictionary<int, List<(int entryIndex, Pvf110Entry e)>>();
        for (int i = 0; i < n; i++)
        {
            var e = original.Entries[i];
            if (!byChunk.TryGetValue(e.ChunkIndex, out var list))
                byChunk[e.ChunkIndex] = list = new();
            list.Add((i, e));
        }

        // 3. 逐组压缩，重建 group table + body + file table offset
        var newBody = new List<byte>();
        var groupTable = new List<(int cumulative, int original)>();
        var dataOffsets = new int[n];
        int cumulative = 0;
        int processed = 0;

        for (int chunk = 0; chunk < original.Header.GroupCount; chunk++)
        {
            if (!byChunk.TryGetValue(chunk, out var flist)) continue;
            // 拼接组内内容
            using var plainMs = new MemoryStream();
            foreach (var (entryIndex, _) in flist)
            {
                var c = contents[entryIndex];
                plainMs.Write(c, 0, c.Length);
                dataOffsets[entryIndex] = (int)plainMs.Length - c.Length;
            }
            byte[] plain = plainMs.ToArray();
            byte[] comp = Compress(plain);
            groupTable.Add((cumulative + comp.Length, plain.Length));
            newBody.AddRange(comp);
            cumulative += comp.Length;
            processed++;
            if ((processed % 1000) == 0) progress?.Report(processed * 100.0 / original.Header.GroupCount);
        }

        // 4. 重建 file table（name/path/chunk/type 保留，offset/size 更新）
        byte[] fileTable = new byte[n * Pvf110Crypto.FileEntrySize];
        for (int i = 0; i < n; i++)
        {
            var e = original.Entries[i];
            BitConverter.GetBytes(e.NameOffset).CopyTo(fileTable, i * 24);
            BitConverter.GetBytes(e.PathOffset).CopyTo(fileTable, i * 24 + 4);
            BitConverter.GetBytes(e.ChunkIndex).CopyTo(fileTable, i * 24 + 8);
            BitConverter.GetBytes(dataOffsets[i]).CopyTo(fileTable, i * 24 + 12);
            BitConverter.GetBytes(contents[i].Length).CopyTo(fileTable, i * 24 + 16);
            BitConverter.GetBytes(e.DataType).CopyTo(fileTable, i * 24 + 20);
        }

        // 5. header（bodySize 更新，其余保留）
        byte[] header = new byte[Pvf110Crypto.HeaderSize];
        BitConverter.GetBytes(0x69706B6Eu).CopyTo(header, 0);
        Array.Copy(original.Header.Guid, 0, header, 4, 0x14);
        BitConverter.GetBytes(original.Header.EntryCount).CopyTo(header, 0x18);
        BitConverter.GetBytes(original.Header.Padding).CopyTo(header, 0x1C);
        BitConverter.GetBytes(newBody.Count).CopyTo(header, 0x20);
        BitConverter.GetBytes(original.Header.GroupCount).CopyTo(header, 0x24);
        BitConverter.GetBytes(original.Header.HashTableSize).CopyTo(header, 0x28);
        BitConverter.GetBytes(original.Header.NameTableSize).CopyTo(header, 0x2C);

        // 6. 保留 name/hash 表原始字节
        byte[] hashTable = Slice(original.Stream, original.Layout.HashOffset,
            original.Layout.NameOffset - original.Layout.HashOffset);
        byte[] nameTable = Slice(original.Stream, original.Layout.NameOffset,
            original.Layout.GroupOffset - original.Layout.NameOffset);

        // 7. 组装明文逻辑流（header + filetable + hash + name + group 表 + body）
        using MemoryStream logicalMs = new();
        logicalMs.Write(header);
        logicalMs.Write(fileTable);
        logicalMs.Write(hashTable);
        logicalMs.Write(nameTable);
        foreach (var (cum, orig) in groupTable)
        {
            logicalMs.Write(BitConverter.GetBytes(cum));
            logicalMs.Write(BitConverter.GetBytes(orig));
        }
        logicalMs.Write(newBody.ToArray());
        byte[] plaintext = logicalMs.ToArray();

        // 8. 新 chunk keys + 加密
        int nChunks = (plaintext.Length + Pvf110Crypto.ChunkStride - 1) / Pvf110Crypto.ChunkStride;
        byte[][] keys = new byte[nChunks][];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        for (int i = 0; i < nChunks; i++) { keys[i] = new byte[0x20]; rng.GetBytes(keys[i]); }

        var h = new Pvf110Header
        {
            Magic = original.Header.Magic,
            Guid = original.Header.Guid,
            EntryCount = original.Header.EntryCount,
            Padding = original.Header.Padding,
            BodySize = newBody.Count,
            GroupCount = original.Header.GroupCount,
            HashTableSize = original.Header.HashTableSize,
            NameTableSize = original.Header.NameTableSize,
        };
        byte[] enc = Pvf110Repack.PackPvf(plaintext, h, keys, groupTable, hashPreserved: true);
        byte[] sk = Pvf110Repack.BuildSkDat(keys);
        progress?.Report(100.0);
        return (enc, sk);
    }

    private static byte[] Compress(byte[] data)
    {
        using MemoryStream ms = new();
        using (ZLibStream zs = new(ms, CompressionLevel.SmallestSize, leaveOpen: true))
            zs.Write(data, 0, data.Length);
        return ms.ToArray();
    }

    private static byte[] Slice(byte[] data, int offset, int length)
    {
        byte[] result = new byte[length];
        Array.Copy(data, offset, result, 0, length);
        return result;
    }
}
