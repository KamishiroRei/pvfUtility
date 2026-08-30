using System.IO.Compression;

namespace Pvf110.Core;

/// <summary>
/// NKPI / ProtectedNKPI 归档重建（内容修改 → 新 Script.pvf）。
/// 保留 name/hash/entry 结构，重建 body group 表与数据。
/// 不增删文件、不改路径、不修改 name 池。
/// </summary>
public static class NkpiRepacker
{
    /// <summary>重建 NKPI/ProtectedNKPI 归档：修改内容 → 新 Script.pvf（无 sk.dat）。</summary>
    /// <param name="original">打开的 NkpiReader 实例。</param>
    /// <param name="getContent">回调：接收 entry 索引，返回修改后的内容字节。</param>
    /// <param name="progress">可选进度报告。</param>
    /// <returns>新 Script.pvf 字节数组。</returns>
    public static byte[] Rebuild(
        NkpiReader original,
        Func<int, byte[]> getContent,
        IProgress<double>? progress = null)
    {
        bool isProtected = original.Format == NkpiFormatKind.Protected;

        // 1. 收集每个 entry 的新内容
        int n = original.Entries.Count;
        var contents = new byte[n][];
        for (int i = 0; i < n; i++)
            contents[i] = getContent(i);

        // 2. 按 chunk 分组
        var byChunk = new Dictionary<int, List<(int index, int dataOff, int dataSize)>>();
        for (int i = 0; i < n; i++)
        {
            var e = original.Entries[i];
            if (!byChunk.TryGetValue(e.ChunkIndex, out var list))
                byChunk[e.ChunkIndex] = list = new();
            list.Add((i, e.DataOffset, e.DataSize));
        }

        // 3. 逐组重建 body
        var newBody = new List<byte>();
        var groupTable = new List<(int cumulative, int original)>();
        var dataOffsets = new int[n];
        var dataSizes = new int[n];
        int cumulative = 0;
        int processed = 0;

        for (int chunk = 0; chunk < original.Header.GroupCount; chunk++)
        {
            if (!byChunk.TryGetValue(chunk, out var flist)) continue;

            // 拼接组内内容
            using var plainMs = new MemoryStream();
            foreach (var (entryIndex, _, _) in flist)
            {
                var c = contents[entryIndex];
                dataOffsets[entryIndex] = (int)plainMs.Length;
                dataSizes[entryIndex] = c.Length;
                plainMs.Write(c, 0, c.Length);
            }
            byte[] plain = plainMs.ToArray();
            byte[] comp = Compress(plain);
            byte[] enc = LcgEncryptGroup(comp, "body", isProtected);
            groupTable.Add((cumulative + enc.Length, plain.Length));
            newBody.AddRange(enc);
            cumulative += enc.Length;
            processed++;
            if ((processed % 500) == 0)
                progress?.Report(processed * 100.0 / original.Header.GroupCount);
        }

        // 4. 重建 file table
        byte[] fileTable = new byte[n * NkpiReader.FileEntrySize];
        for (int i = 0; i < n; i++)
        {
            var e = original.Entries[i];
            int off = i * NkpiReader.FileEntrySize;
            BitConverter.GetBytes(e.NameOffset).CopyTo(fileTable, off);
            BitConverter.GetBytes(e.PathOffset).CopyTo(fileTable, off + 4);
            BitConverter.GetBytes(e.ChunkIndex).CopyTo(fileTable, off + 8);
            BitConverter.GetBytes(dataOffsets[i]).CopyTo(fileTable, off + 12);
            BitConverter.GetBytes(dataSizes[i]).CopyTo(fileTable, off + 16);
            BitConverter.GetBytes(e.DataType).CopyTo(fileTable, off + 20);
        }

        // 5. hash 表原始字节（保持不变）
        byte[] hashTable = new byte[original.Header.HashTableSize];
        Array.Copy(original.Stream, original.Layout.HashOffset, hashTable, 0, hashTable.Length);

        // 6. name 表原始字节（保持不变）
        byte[] nameTable = new byte[original.Header.NameTableSize];
        Array.Copy(original.Stream, original.Layout.NameOffset, nameTable, 0, nameTable.Length);

        // 7. 重建 group 表（grpi 段）
        byte[] grpiPlain = new byte[groupTable.Count * NkpiReader.GroupEntrySize];
        for (int i = 0; i < groupTable.Count; i++)
        {
            BitConverter.GetBytes(groupTable[i].cumulative).CopyTo(grpiPlain, i * 8);
            BitConverter.GetBytes(groupTable[i].original).CopyTo(grpiPlain, i * 8 + 4);
        }
        byte[] grpiEnc = LcgEncryptGroup(grpiPlain, "group", isProtected);

        // 8. 重建 header
        byte[] header = new byte[NkpiReader.HeaderSize];
        BitConverter.GetBytes(NkpiReader.Signature).CopyTo(header, 0);
        Array.Copy(original.Header.Guid, 0, header, 4, 0x14);
        BitConverter.GetBytes(original.Header.FileCount).CopyTo(header, 0x18);
        BitConverter.GetBytes(original.Header.Padding).CopyTo(header, 0x1C);
        BitConverter.GetBytes(cumulative).CopyTo(header, 0x20);
        BitConverter.GetBytes(original.Header.GroupCount).CopyTo(header, 0x24);
        BitConverter.GetBytes(original.Header.HashTableSize).CopyTo(header, 0x28);
        BitConverter.GetBytes(original.Header.NameTableSize).CopyTo(header, 0x2C);
        byte[] headerEnc = LcgEncryptGroup(header, "header", isProtected);

        // 9. 组装输出
        using var outMs = new MemoryStream();
        outMs.Write(headerEnc);
        outMs.Write(fileTable);
        outMs.Write(hashTable);
        outMs.Write(nameTable);
        outMs.Write(grpiEnc);
        outMs.Write(newBody.ToArray());

        progress?.Report(100.0);
        return outMs.ToArray();
    }

    // ─── 辅助函数 ───────────────────────────────────────────────────────

    private static byte[] Compress(byte[] data)
    {
        using MemoryStream ms = new();
        using (ZLibStream zs = new(ms, CompressionLevel.SmallestSize, leaveOpen: true))
            zs.Write(data, 0, data.Length);
        return ms.ToArray();
    }

    /// <summary>LCG 加密 body/group/header 段（XOR 自反 == 解密）。</summary>
    private static byte[] LcgEncryptGroup(byte[] data, string section, bool isProtected)
    {
        string label = section switch
        {
            "header" => isProtected ? "hEAd" : "HeaD",
            "hash" => "HASH",
            "group" => isProtected ? "grpi" : "GRPI",
            "body" => isProtected ? "bODy" : "BodY",
            _ => section,
        };

        uint seed = isProtected
            ? NkpiReader.DeriveSeedUtf16(label)
            : NkpiReader.DeriveSeedAscii(label);
        return NkpiReader.LcgDecrypt(data, seed, NkpiReader.SegInc);
    }
}