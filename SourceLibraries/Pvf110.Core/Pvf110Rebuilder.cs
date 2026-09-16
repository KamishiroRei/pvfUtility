using System.IO.Compression;

namespace Pvf110.Core;

/// <summary>Pvf110 新增条目（add 路径使用）。路径为归档内相对路径（'/' 分隔）。</summary>
public sealed record Pvf110Addition(string Path, byte[] Content, int DataType);

/// <summary>Pvf110 重建选项。</summary>
public sealed class Pvf110RebuildOptions
{
    /// <summary>
    /// 复用原归档 chunk key（默认 true）。原 sk.dat 的 key 槽位够用时，
    /// 输出 sk.dat 就是原 sk.dat 字节 —— 客户端 sk.dat 无需更换，
    /// 部署面只有 Script.pvf 一个文件。槽位不足时才生成新 key 并用
    /// <see cref="Pvf110Crypto.ActiveKeySet"/> 重建 sk.dat。
    /// </summary>
    public bool ReuseClientKeys { get; init; } = true;

    public static readonly Pvf110RebuildOptions Default = new();
}

/// <summary>
/// Pvf110 逻辑流重建（内容修改 → 新 Script.pvf + sk.dat）。
/// 保留 name 表与 entry 结构，重建 file table dataOffset/dataSize、group 表与 body。
/// 阶段约束：不删文件、不改既有路径；新增条目必须走 <c>additions</c> 参数
/// （名称池追加 + HASH 段增量），不得改动既有条目的路径 magic。
///
/// 增量优化：isEntryModified 给出每个 entry 的修改状态；
/// - 整组未修改：直接复用原始密文组字节（不解压、不重编译、不重压缩）；
/// - 组内有修改：仅修改 entry 调 getContent 重编译，其余 entry 取原始组明文，整组重压缩。
/// 未提供 isEntryModified 时视为全部修改（等价旧行为，供 CLI 校验链使用）。
/// </summary>
public static class Pvf110Rebuilder
{
    /// <summary>
    /// 流式重建（百万级归档的默认路径）：只重压缩/重加密**确有修改**的组，其余组与
    /// hash/name 段直接从原逻辑流搬运；file table 分块写出，header / group 表就地 LCG 加密，
    /// 最后就地为各 chunk 前缀做 AES。
    ///
    /// 峰值内存与归档大小无关（64 KB 级写出缓冲 + 被修改组 + 需要重建的段），
    /// 不再需要"整包明文逻辑流 + 整包密文输出"两份 760 MB 级缓冲。
    /// <paramref name="output"/> 必须可 seek（写出后要就地为各 chunk 前缀做 AES）。
    /// </summary>
    public static (long Length, byte[] SkDat) RebuildToStream(
        Pvf110Reader original,
        Stream output,
        Func<Pvf110Entry, byte[]> getContent,
        IProgress<double>? progress = null,
        Func<Pvf110Entry, bool>? isEntryModified = null,
        IReadOnlyList<Pvf110Addition>? additions = null,
        Pvf110RebuildOptions? options = null,
        Pvf110NamePool? namePool = null)
    {
        options ??= Pvf110RebuildOptions.Default;
        int n = original.Entries.Count;
        int added = additions?.Count ?? 0;
        int total = n + added;

        // 0. 名称池：确有新增条目，或调用方自己往池里追加过字符串（GUI 编辑引入池外新串）时才构建。
        //    既有条目一律复用原 magic，不移动任何既有字符串。
        Pvf110NamePool? pool = null;
        var additionNames = new int[added];
        var additionPaths = new int[added];
        if (added > 0)
        {
            pool = namePool ?? Pvf110NamePool.FromReader(original);
            for (int k = 0; k < added; k++)
            {
                Pvf110Addition a = additions![k];
                string norm = a.Path.Replace('\\', '/').Trim('/');
                int slash = norm.LastIndexOf('/');
                string folder = slash >= 0 ? norm[..slash] : string.Empty;
                string name = slash >= 0 ? norm[(slash + 1)..] : norm;
                additionNames[k] = pool.GetOrAdd(name);
                additionPaths[k] = folder.Length > 0 ? pool.GetOrAdd(folder) : unchecked((int)Pvf110HashTable.NullPath);
            }
        }

        // 1. 按 chunk 分组（保留 entry 顺序）
        var byChunk = new Dictionary<int, List<int>>();
        for (int i = 0; i < n; i++)
        {
            int chunk = original.Entries[i].ChunkIndex;
            if (chunk < 0 || chunk >= original.Header.GroupCount)
                throw new InvalidDataException($"entry {i} chunk index {chunk} out of group range");
            if (!byChunk.TryGetValue(chunk, out var list))
                byChunk[chunk] = list = new();
            list.Add(i);
        }

        bool IsModified(Pvf110Entry e) => isEntryModified == null || isEntryModified(e);

        // 2. 只重建有修改的组；未修改组复用原始密文
        var rebuiltEnc = new Dictionary<int, byte[]>();
        var newOffsets = new Dictionary<int, int>();
        var newSizes = new Dictionary<int, int>();
        int processed = 0;
        for (int chunk = 0; chunk < original.Header.GroupCount; chunk++)
        {
            if (!byChunk.TryGetValue(chunk, out var flist))
                continue; // 无 entry 的组：直接复用原始字节
            bool anyModified = false;
            foreach (int i in flist)
            {
                if (IsModified(original.Entries[i])) { anyModified = true; break; }
            }
            if (!anyModified)
            {
                processed++;
                continue;
            }

            byte[] originalPlain = original.GroupData(chunk);
            using var plainMs = new MemoryStream(originalPlain.Length);
            foreach (int i in flist)
            {
                Pvf110Entry e = original.Entries[i];
                byte[] content = IsModified(e)
                    ? getContent(e) ?? throw new InvalidOperationException($"entry {i} content is null")
                    : Slice(originalPlain, e.DataOffset, e.DataSize);
                newOffsets[i] = checked((int)plainMs.Length);
                newSizes[i] = content.Length;
                plainMs.Write(content, 0, content.Length);
            }
            byte[] plain = plainMs.ToArray();
            rebuiltEnc[chunk] = Pvf110Repack.LcgEncryptKey(Compress(plain), "body");
            processed++;
            if ((processed % 1000) == 0) progress?.Report(processed * 100.0 / (original.Header.GroupCount + 1));
        }

        // 2b. 新增条目：追加一个末尾组承载全部新内容
        int groupCount = original.Header.GroupCount;
        if (added > 0)
        {
            using var addMs = new MemoryStream();
            for (int k = 0; k < added; k++)
            {
                int i = n + k;
                newOffsets[i] = checked((int)addMs.Position);
                newSizes[i] = additions![k].Content.Length;
                addMs.Write(additions[k].Content, 0, additions[k].Content.Length);
            }
            byte[] addPlain = addMs.ToArray();
            rebuiltEnc[groupCount] = Pvf110Repack.LcgEncryptKey(Compress(addPlain), "body");
            groupCount++;
        }

        // 3. 组表 + body 总长（输出组顺序与原始一致）
        var groupTable = new List<(int cumulative, int original)>(groupCount);
        int cumulative = 0;
        for (int chunk = 0; chunk < groupCount; chunk++)
        {
            int originalLen;
            int encLen;
            if (rebuiltEnc.TryGetValue(chunk, out byte[]? enc))
            {
                originalLen = GetPlainLength(byChunk, original, chunk, newSizes, n, added);
                encLen = enc.Length;
            }
            else
            {
                int prev = chunk > 0 ? original.Groups[chunk - 1].cumulative : 0;
                encLen = original.Groups[chunk].cumulative - prev;
                originalLen = original.Groups[chunk].original;
            }
            cumulative = checked(cumulative + encLen);
            groupTable.Add((cumulative, originalLen));
        }

        // 4. HASH / name 段
        //    - 无新增：HASH 段密文原样保留（写入阶段直接从原逻辑流搬运，不再整段复制）。
        //    - 有新增：在 HASH 明文尾部追加新 pair（既有 pair 与既有尾部字节原样保留），再重新加密。
        byte[]? hashSegment = null;
        if (added > 0)
        {
            byte[] hashPlain = Pvf110HashTable.Decrypt(original.HashEncrypted);
            var newPairs = new List<(uint name, uint path)>(added);
            for (int k = 0; k < added; k++)
                newPairs.Add((unchecked((uint)additionNames[k]), unchecked((uint)additionPaths[k])));
            (byte[] utf8Pool, byte[] utf16Pool) = pool!.Pools;
            byte[] merged = Pvf110HashTable.AppendPairs(hashPlain, newPairs, utf8Pool, utf16Pool);
            string? hashError = Pvf110HashTable.VerifyAppend(hashPlain, merged, newPairs, total);
            if (hashError != null)
                throw new InvalidDataException("Pvf110 hash append verify failed: " + hashError);
            hashSegment = Pvf110HashTable.Encrypt(merged);
        }

        // 名称池是否增长必须在**重编译之后**判定：getContent 会在编辑内容引入池外新串时
        // 向池尾追加（AppendedCount>0），此刻才需要重建 name 表段；提前判定会漏掉这种增长，
        // 导致组内容引用了池外 magic 而盘上 name 表没有该字符串。
        bool namePoolGrew = added > 0 || (namePool is not null && namePool.AppendedCount > 0);
        if (namePoolGrew) pool ??= namePool!;
        byte[]? nameSegment = namePoolGrew ? pool!.BuildNameTableBytes() : null;
        if (namePoolGrew)
            Console.Error.WriteLine($"pvf110 rebuild: name pool {pool!.LastCompressionNote}; name table {original.Header.NameTableSize:N0}B -> {nameSegment!.Length:N0}B");

        // 5. 段尺寸与总长
        int fileTableLen = total * Pvf110Crypto.FileEntrySize;
        int hashLen = hashSegment?.Length ?? original.Header.HashTableSize;
        int nameLen = nameSegment?.Length ?? original.Header.NameTableSize;
        int groupTableLen = groupCount * Pvf110Crypto.GroupEntrySize;
        int bodyOffset = Pvf110Crypto.HeaderSize + fileTableLen + hashLen + nameLen + groupTableLen;
        long totalLength = bodyOffset + (long)cumulative;

        // 6. chunk key：优先复用原 sk.dat（槽位够用即原样回写），不足时按打开时生效的密钥集新建
        int nChunks = (int)((totalLength + Pvf110Crypto.ChunkStride - 1) / Pvf110Crypto.ChunkStride);
        byte[][] keys;
        byte[] sk;
        if (options.ReuseClientKeys && original.ChunkKeys.Length >= nChunks)
        {
            keys = new byte[nChunks][];
            Array.Copy(original.ChunkKeys, keys, nChunks);
            sk = original.SkDatBytes;
        }
        else
        {
            keys = new byte[nChunks][];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            for (int i = 0; i < nChunks; i++) { keys[i] = new byte[0x20]; rng.GetBytes(keys[i]); }
            sk = Pvf110Repack.BuildSkDat(keys, Pvf110Crypto.ActiveKeySet);
        }

        // 7. 顺序写出逻辑流（header 明文 → LCG；file table 分块；hash/name 搬运或重建；group 表 LCG；body 逐组）
        output.Position = 0;
        byte[] header = new byte[Pvf110Crypto.HeaderSize];
        BitConverter.GetBytes(0x69706B6Eu).CopyTo(header, 0);
        Array.Copy(original.Header.Guid, 0, header, 4, 0x14);
        BitConverter.GetBytes(total).CopyTo(header, 0x18);
        BitConverter.GetBytes(original.Header.Padding).CopyTo(header, 0x1C);
        BitConverter.GetBytes(cumulative).CopyTo(header, 0x20);
        BitConverter.GetBytes(groupCount).CopyTo(header, 0x24);
        BitConverter.GetBytes(hashLen).CopyTo(header, 0x28);
        BitConverter.GetBytes(nameLen).CopyTo(header, 0x2C);
        byte[] headerEnc = Pvf110Repack.LcgEncryptKey(header, "header");
        output.Write(headerEnc, 0, headerEnc.Length);

        byte[] rowStaging = new byte[Pvf110Crypto.FileEntrySize * 4096];
        int rowPos = 0;
        void FlushRows()
        {
            if (rowPos > 0)
            {
                output.Write(rowStaging, 0, rowPos);
                rowPos = 0;
            }
        }
        for (int i = 0; i < n; i++)
        {
            Pvf110Entry e = original.Entries[i];
            BitConverter.GetBytes(e.NameOffset).CopyTo(rowStaging, rowPos);
            BitConverter.GetBytes(e.PathOffset).CopyTo(rowStaging, rowPos + 4);
            BitConverter.GetBytes(e.ChunkIndex).CopyTo(rowStaging, rowPos + 8);
            if (newOffsets.TryGetValue(i, out int off))
            {
                BitConverter.GetBytes(off).CopyTo(rowStaging, rowPos + 12);
                BitConverter.GetBytes(newSizes[i]).CopyTo(rowStaging, rowPos + 16);
            }
            else
            {
                BitConverter.GetBytes(e.DataOffset).CopyTo(rowStaging, rowPos + 12);
                BitConverter.GetBytes(e.DataSize).CopyTo(rowStaging, rowPos + 16);
            }
            BitConverter.GetBytes(e.DataType).CopyTo(rowStaging, rowPos + 20);
            rowPos += Pvf110Crypto.FileEntrySize;
            if (rowPos == rowStaging.Length) FlushRows();
        }
        for (int k = 0; k < added; k++)
        {
            int i = n + k;
            BitConverter.GetBytes(additionNames[k]).CopyTo(rowStaging, rowPos);
            BitConverter.GetBytes(additionPaths[k]).CopyTo(rowStaging, rowPos + 4);
            BitConverter.GetBytes(groupCount - 1).CopyTo(rowStaging, rowPos + 8);
            BitConverter.GetBytes(newOffsets[i]).CopyTo(rowStaging, rowPos + 12);
            BitConverter.GetBytes(newSizes[i]).CopyTo(rowStaging, rowPos + 16);
            BitConverter.GetBytes(additions![k].DataType).CopyTo(rowStaging, rowPos + 20);
            rowPos += Pvf110Crypto.FileEntrySize;
            if (rowPos == rowStaging.Length) FlushRows();
        }
        FlushRows();

        if (hashSegment != null) output.Write(hashSegment, 0, hashSegment.Length);
        else WriteFromOriginal(output, original.Stream, original.Layout.HashOffset, original.Header.HashTableSize);

        if (nameSegment != null) output.Write(nameSegment, 0, nameSegment.Length);
        else WriteFromOriginal(output, original.Stream, original.Layout.NameOffset, original.Header.NameTableSize);

        byte[] groupBytes = new byte[groupTableLen];
        for (int i = 0; i < groupTable.Count; i++)
        {
            BitConverter.GetBytes(groupTable[i].cumulative).CopyTo(groupBytes, i * 8);
            BitConverter.GetBytes(groupTable[i].original).CopyTo(groupBytes, i * 8 + 4);
        }
        byte[] groupEnc = Pvf110Repack.LcgEncryptKey(groupBytes, "group");
        output.Write(groupEnc, 0, groupEnc.Length);

        for (int chunk = 0; chunk < original.Header.GroupCount; chunk++)
        {
            if (rebuiltEnc.TryGetValue(chunk, out byte[]? enc))
            {
                output.Write(enc, 0, enc.Length);
            }
            else
            {
                int prev = chunk > 0 ? original.Groups[chunk - 1].cumulative : 0;
                int len = original.Groups[chunk].cumulative - prev;
                WriteFromOriginal(output, original.Stream, original.Layout.BodyOffset + prev, len);
            }
        }
        if (rebuiltEnc.TryGetValue(original.Header.GroupCount, out byte[]? addEnc))
            output.Write(addEnc, 0, addEnc.Length);

        output.Flush();
        if (output.Position != totalLength)
            throw new InvalidDataException($"pvf110 rebuild: 写出长度 {output.Position} 与预期 {totalLength} 不一致");

        // 8. 各 chunk 前缀 AES（就地：读回 10 KB → CBC 加密 → 原位写回）
        for (int i = 0; i < nChunks; i++)
            EncryptChunkPrefixInPlace(output, i, keys[i]);

        progress?.Report(100.0);
        return (totalLength, sk);
    }

    /// <summary>
    /// 兼容入口：整包驻留内存的重建（测试与校验链使用）。
    /// 生产路径（GUI 保存 / CLI 写回 / 新增）应使用 <see cref="RebuildToStream"/>，避免两份整包缓冲。
    /// </summary>
    public static (byte[] scriptPvf, byte[] skdat) Rebuild(
        Pvf110Reader original,
        Func<Pvf110Entry, byte[]> getContent,
        IProgress<double>? progress = null,
        Func<Pvf110Entry, bool>? isEntryModified = null,
        IReadOnlyList<Pvf110Addition>? additions = null,
        Pvf110RebuildOptions? options = null,
        Pvf110NamePool? namePool = null)
    {
        using var ms = new MemoryStream(original.Stream.Length + (1 << 20));
        (long _, byte[] sk) = RebuildToStream(original, ms, getContent, progress, isEntryModified, additions, options, namePool);
        return (ms.ToArray(), sk);
    }

    /// <summary>把原逻辑流的一段直接写入输出（不做任何拷贝分配）。</summary>
    private static void WriteFromOriginal(Stream output, byte[] source, int offset, int length)
    {
        const int Block = 1 << 20;
        int pos = offset;
        int end = offset + length;
        while (pos < end)
        {
            int len = Math.Min(Block, end - pos);
            output.Write(source, pos, len);
            pos += len;
        }
    }

    /// <summary>就地为单个 chunk 前缀做 AES-CBC（密钥来自 chunk key，零 IV，无填充）。</summary>
    private static void EncryptChunkPrefixInPlace(Stream stream, int index, byte[] key)
    {
        long off = (long)index * Pvf110Crypto.ChunkStride;
        if (off + Pvf110Crypto.ChunkPrefix > stream.Length) return;
        byte[] block = new byte[Pvf110Crypto.ChunkPrefix];
        stream.Position = off;
        stream.ReadExactly(block);
        using System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = new byte[16];
        aes.Mode = System.Security.Cryptography.CipherMode.CBC;
        aes.Padding = System.Security.Cryptography.PaddingMode.None;
        using System.Security.Cryptography.ICryptoTransform enc = aes.CreateEncryptor();
        enc.TransformBlock(block, 0, block.Length, block, 0);
        stream.Position = off;
        stream.Write(block, 0, block.Length);
    }

    private static int GetPlainLength(Dictionary<int, List<int>> byChunk, Pvf110Reader original,
        int chunk, Dictionary<int, int> newSizes, int entryCount, int added)
    {
        if (chunk >= original.Header.GroupCount)
        {
            long addTotal = 0;
            for (int i = entryCount; i < entryCount + added; i++)
                addTotal += newSizes[i];
            return checked((int)addTotal);
        }
        long total = 0;
        foreach (int i in byChunk[chunk])
            total += newSizes[i];
        return checked((int)total);
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
        if (offset < 0 || length < 0 || offset + length > data.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        byte[] result = new byte[length];
        Array.Copy(data, offset, result, 0, length);
        return result;
    }
}
