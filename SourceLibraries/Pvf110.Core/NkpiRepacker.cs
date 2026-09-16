using System.IO.Compression;

namespace Pvf110.Core;

/// <summary>一个要追加到 NKPI/ProtectedNKPI 归档中的文件。</summary>
public sealed class NkpiNewFile
{
    public NkpiNewFile(string filePath, byte[] content, int dataType = 1)
    {
        FilePath = NormalizePath(filePath);
        Content = content ?? throw new ArgumentNullException(nameof(content));
        if (FilePath.Length == 0)
            throw new ArgumentException("file path cannot be empty", nameof(filePath));
        if (dataType is not (1 or 2 or 3))
            throw new ArgumentOutOfRangeException(nameof(dataType), "NKPI dataType must be 1, 2, or 3");
        DataType = dataType;
    }

    public string FilePath { get; }
    public byte[] Content { get; }
    public int DataType { get; }

    private static string NormalizePath(string value)
        => (value ?? string.Empty).Replace('\\', '/').Trim('/').ToLowerInvariant();
}

/// <summary>
/// NKPI / ProtectedNKPI 归档重建。
///
/// 除了修改已有文件外，新增文件会同步更新 file table、name table、hash table、
/// group table 和 body；因此 GUI/CLI 的“导入新文件/文件夹”可以真正落入归档，
/// 而不是只停留在内存 FileList。
/// </summary>
public static class NkpiRepacker
{
    /// <summary>重建归档中的已有文件；名称表保持原始字节，哈希表按现有文件重建。</summary>
    public static byte[] Rebuild(
        NkpiReader original,
        Func<int, byte[]> getContent,
        IProgress<double>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(getContent);

        int n = original.Entries.Count;
        var contents = new byte[n][];
        for (int i = 0; i < n; i++)
            contents[i] = getContent(i) ?? throw new InvalidOperationException($"entry {i} content is null");

        var entries = original.Entries
            .Select(e => new NkpiEntry
            {
                Index = e.Index,
                NameOffset = e.NameOffset,
                PathOffset = e.PathOffset,
                ChunkIndex = e.ChunkIndex,
                DataOffset = e.DataOffset,
                DataSize = e.DataSize,
                DataType = e.DataType,
            })
            .ToList();

        return RebuildCore(original, entries, contents, original.Header.GroupCount, original.Header.GroupCount,
            originalNameTable: true, resolveName: original.ResolveName, progress: progress);
    }

    /// <summary>
    /// 在重建已有文件的同时追加指定文件。新增文件默认放进一个新 body group，
    /// 并使用名称池构建器提供的新 magic offset 编写 file/hash/name 表。
    /// </summary>
    public static byte[] RebuildWithAdditions(
        NkpiReader original,
        Func<int, byte[]> getContent,
        IReadOnlyList<NkpiNewFile> additions,
        NkpiNamePoolBuilder? namePool = null,
        IProgress<double>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(getContent);
        ArgumentNullException.ThrowIfNull(additions);

        var existingPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (NkpiEntry entry in original.Entries)
            existingPaths.Add(original.FilePath(entry));

        var newPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        NkpiNamePoolBuilder pool = namePool ?? new NkpiNamePoolBuilder(original);
        foreach (NkpiNewFile addition in additions)
        {
            if (!newPaths.Add(addition.FilePath))
                throw new InvalidOperationException("duplicate new NKPI path: " + addition.FilePath);
            if (existingPaths.Contains(addition.FilePath))
                throw new InvalidOperationException("NKPI path already exists; use the existing-file update path: " + addition.FilePath);

            int slash = addition.FilePath.LastIndexOf('/');
            string folder = slash > 0 ? addition.FilePath[..slash] : string.Empty;
            string name = slash > 0 ? addition.FilePath[(slash + 1)..] : addition.FilePath;
            pool.GetOrAdd(folder);
            pool.GetOrAdd(name);
        }

        int existingCount = original.Entries.Count;
        var entries = original.Entries
            .Select(e => new NkpiEntry
            {
                Index = e.Index,
                NameOffset = e.NameOffset,
                PathOffset = e.PathOffset,
                ChunkIndex = e.ChunkIndex,
                DataOffset = e.DataOffset,
                DataSize = e.DataSize,
                DataType = e.DataType,
            })
            .ToList();

        var contents = new List<byte[]>(existingCount + additions.Count);
        for (int i = 0; i < existingCount; i++)
            contents.Add(getContent(i) ?? throw new InvalidOperationException($"entry {i} content is null"));

        int additionChunk = original.Header.GroupCount;
        for (int i = 0; i < additions.Count; i++)
        {
            NkpiNewFile addition = additions[i];
            int slash = addition.FilePath.LastIndexOf('/');
            string folder = slash > 0 ? addition.FilePath[..slash] : string.Empty;
            string name = slash > 0 ? addition.FilePath[(slash + 1)..] : addition.FilePath;
            entries.Add(new NkpiEntry
            {
                Index = existingCount + i,
                NameOffset = pool.GetOrAdd(name),
                PathOffset = slash > 0 ? pool.GetOrAdd(folder) : pool.GetOrAdd(string.Empty),
                ChunkIndex = additionChunk,
                DataOffset = 0,
                DataSize = addition.Content.Length,
                DataType = addition.DataType,
            });
            contents.Add(addition.Content);
        }

        int groupCount = original.Header.GroupCount + (additions.Count > 0 ? 1 : 0);
        return RebuildCore(original, entries, contents, original.Header.GroupCount, groupCount,
            originalNameTable: false, resolveName: pool.Resolve, namePool: pool, progress);
    }

    /// <summary>
    /// 增量写回：只修改一个已有条目，只重压缩其所在 body 组。
    /// 文件表只更新受影响行；HASH 段、名称段和其他组的字节原样复制（不进入 CPU）；
    /// 组表从目标组起的 cumulative 按长度差平移。输出写为新文件，原始 PVF 不被修改。
    /// 这使“保留客户端 HASH 索引”天然成立，写回耗时与影响面从整包级降到单组级。
    /// </summary>
    public static void PatchEntry(NkpiReader original, int entryIndex, byte[] newContent, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(newContent);
        if (string.IsNullOrWhiteSpace(outputPath))
            throw new ArgumentException("output path required", nameof(outputPath));
        if (entryIndex < 0 || entryIndex >= original.Entries.Count)
            throw new ArgumentOutOfRangeException(nameof(entryIndex));

        NkpiEntry target = original.Entries[entryIndex];
        int chunk = target.ChunkIndex;
        if (chunk < 0 || chunk >= original.Groups.Count)
            throw new InvalidDataException($"entry {entryIndex} group {chunk} out of range");

        List<NkpiEntry> sameGroup = original.Entries.Where(e => e.ChunkIndex == chunk).ToList();
        byte[] groupPlain = original.GroupData(chunk);

        int check = 0;
        foreach (NkpiEntry e in sameGroup)
        {
            if (e.DataOffset != check)
                throw new InvalidDataException(
                    $"group {chunk} entries are not contiguous: entry {e.Index} offset {e.DataOffset} != {check}");
            if (e.DataSize < 0 || check + e.DataSize > groupPlain.Length)
                throw new InvalidDataException($"group {chunk} entry {e.Index} size out of bounds");
            check += e.DataSize;
        }
        if (check != groupPlain.Length)
            throw new InvalidDataException(
                $"group {chunk} entry sizes {check} != group plain size {groupPlain.Length}");

        int plainDelta = checked(newContent.Length - target.DataSize);

        // 新组明文：目标条目替换为 newContent，其余条目原样；记录受影响条目的新组内偏移
        byte[] newPlain = new byte[checked(groupPlain.Length + plainDelta)];
        var newOffsets = new Dictionary<int, int>(sameGroup.Count);
        int pos = 0;
        foreach (NkpiEntry e in sameGroup)
        {
            newOffsets[e.Index] = pos;
            if (e.Index == entryIndex)
            {
                Buffer.BlockCopy(newContent, 0, newPlain, pos, newContent.Length);
                pos += newContent.Length;
            }
            else
            {
                Buffer.BlockCopy(groupPlain, e.DataOffset, newPlain, pos, e.DataSize);
                pos += e.DataSize;
            }
        }

        byte[] compressed = Compress(newPlain);
        byte[] newGroupEnc = EncryptSegment(compressed, original, "body");

        int prevCum = chunk > 0 ? original.Groups[chunk - 1].cumulative : 0;
        int oldCum = original.Groups[chunk].cumulative;
        // body 布局差 = 新旧组密文长度差（即使明文等长，重压缩输出大小也可能不同）
        int encDelta = checked(newGroupEnc.Length - (oldCum - prevCum));
        int newBodySize = checked(original.Header.BodySize + encDelta);

        // 文件表：只改目标条目及同组后续条目的组内偏移（与目标条目同长时仅目标行变化）
        byte[] fileTable = ReadSection(original.Stream, NkpiReader.HeaderSize,
            original.Header.FileCount * NkpiReader.FileEntrySize);
        foreach (NkpiEntry e in sameGroup)
        {
            int row = e.Index * NkpiReader.FileEntrySize;
            BitConverter.GetBytes(newOffsets[e.Index]).CopyTo(fileTable, row + 12);
            if (e.Index == entryIndex)
                BitConverter.GetBytes(newContent.Length).CopyTo(fileTable, row + 16);
        }

        // 组表：LCG XOR 流自逆，解密→目标组起 cumulative 平移→再加密
        byte[] grpi = ReadSection(original.Stream, original.Layout.GroupOffset,
            original.Header.GroupCount * NkpiReader.GroupEntrySize);
        byte[] grpiPlain = EncryptSegment(grpi, original, "group");
        for (int g = chunk; g < original.Header.GroupCount; g++)
        {
            int c = checked(BitConverter.ToInt32(grpiPlain, g * 8) + encDelta);
            BitConverter.GetBytes(c).CopyTo(grpiPlain, g * 8);
        }
        byte[] grpiEnc = EncryptSegment(grpiPlain, original, "group");

        byte[] header = BuildHeader(original, original.Header.FileCount, newBodySize, original.Header.GroupCount,
            original.Header.HashTableSize, original.Header.NameTableSize);

        using (FileStream outFs = File.Create(outputPath))
        using (BufferedStream buf = new(outFs, 1 << 20))
        {
            buf.Write(header);
            buf.Write(fileTable);
            CopySectionBytes(original.Stream, original.Layout.HashOffset, original.Header.HashTableSize, buf);
            CopySectionBytes(original.Stream, original.Layout.NameOffset, original.Header.NameTableSize, buf);
            buf.Write(grpiEnc);

            const int copyChunk = 8 << 20;
            for (int off = 0; off < prevCum; off += copyChunk)
            {
                int len = Math.Min(copyChunk, prevCum - off);
                buf.Write(original.ReadRawBody(off, len));
            }
            buf.Write(newGroupEnc);
            for (int off = oldCum; off < original.Header.BodySize; off += copyChunk)
            {
                int len = Math.Min(copyChunk, original.Header.BodySize - off);
                buf.Write(original.ReadRawBody(off, len));
            }
        }
    }

    /// <summary>
    /// 增量流式重建：未修改组直接复用原始密文（不解压、不重压缩、不重编译），
    /// 只有含修改条目的组按新内容重压缩。输出流式写入 outputPath（通常为临时文件，由调用方原子替换），
    /// body/结构区不整包驻留内存。适用于无新增文件的整包保存。
    /// 组表/文件表/头段落盘前统一重建；HASH 段与名称段原样复制原始密文字节。
    /// </summary>
    public static void RebuildIncrementalToFile(
        NkpiReader original,
        Func<int, bool> isEntryModified,
        Func<int, byte[]> getContent,
        string outputPath,
        IProgress<double>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(isEntryModified);
        ArgumentNullException.ThrowIfNull(getContent);
        if (string.IsNullOrWhiteSpace(outputPath))
            throw new ArgumentException("output path required", nameof(outputPath));

        int n = original.Entries.Count;
        var byChunk = new Dictionary<int, List<int>>();
        for (int i = 0; i < n; i++)
        {
            int chunk = original.Entries[i].ChunkIndex;
            if (chunk < 0 || chunk >= original.Header.GroupCount)
                throw new InvalidDataException($"entry {i} chunk index {chunk} out of group range");
            if (!byChunk.TryGetValue(chunk, out List<int>? list))
                byChunk[chunk] = list = new List<int>();
            list.Add(i);
        }

        var dataOffsets = new int[n];
        var dataSizes = new int[n];
        var groupPlainLengths = new int[original.Header.GroupCount];
        var groupEncLengths = new int[original.Header.GroupCount];
        int cumulative = 0;
        int processed = 0;

        using (FileStream outFs = File.Create(outputPath))
        using (BufferedStream buf = new(outFs, 1 << 20))
        {
            // 结构区布局：header 0x30 + file table 24n（占位）→ hash → name（原样复制）→ 组表（占位）→ body
            int hashOffset = NkpiReader.HeaderSize + n * NkpiReader.FileEntrySize;
            int nameOffset = hashOffset + original.Header.HashTableSize;
            int groupTableOffset = nameOffset + original.Header.NameTableSize;
            int bodyOffset = groupTableOffset + original.Header.GroupCount * NkpiReader.GroupEntrySize;

            WriteZeros(buf, hashOffset);

            // HASH 段与名称段：原样复制原始密文字节
            buf.Write(ReadSection(original.Stream, original.Layout.HashOffset, original.Header.HashTableSize));
            buf.Write(ReadSection(original.Stream, original.Layout.NameOffset, original.Header.NameTableSize));

            // 组表占位
            WriteZeros(buf, original.Header.GroupCount * NkpiReader.GroupEntrySize);

            // body：逐组复用或重建
            for (int chunk = 0; chunk < original.Header.GroupCount; chunk++)
            {
                int prev = chunk > 0 ? original.Groups[chunk - 1].cumulative : 0;
                int originalEncLen = original.Groups[chunk].cumulative - prev;

                if (!byChunk.TryGetValue(chunk, out List<int>? entries))
                {
                    // 无条目组：原样保留原始密文
                    CopyBodyBytes(original, prev, originalEncLen, buf);
                    groupEncLengths[chunk] = originalEncLen;
                    groupPlainLengths[chunk] = original.Groups[chunk].original;
                    cumulative += originalEncLen;
                    processed++;
                    continue;
                }

                bool anyModified = false;
                foreach (int i in entries)
                {
                    if (isEntryModified(i)) { anyModified = true; break; }
                }

                if (!anyModified)
                {
                    foreach (int i in entries)
                    {
                        dataOffsets[i] = original.Entries[i].DataOffset;
                        dataSizes[i] = original.Entries[i].DataSize;
                    }
                    CopyBodyBytes(original, prev, originalEncLen, buf);
                    groupEncLengths[chunk] = originalEncLen;
                    groupPlainLengths[chunk] = original.Groups[chunk].original;
                    cumulative += originalEncLen;
                }
                else
                {
                    byte[] groupPlain = original.GroupData(chunk);
                    using MemoryStream plainMs = new();
                    foreach (int i in entries)
                    {
                        NkpiEntry e = original.Entries[i];
                        byte[] content = isEntryModified(i)
                            ? getContent(i) ?? throw new InvalidOperationException($"entry {i} content is null")
                            : ReadSection(groupPlain, e.DataOffset, e.DataSize);
                        dataOffsets[i] = checked((int)plainMs.Length);
                        dataSizes[i] = content.Length;
                        plainMs.Write(content, 0, content.Length);
                    }
                    byte[] plain = plainMs.ToArray();
                    byte[] enc = EncryptSegment(Compress(plain), original, "body");
                    buf.Write(enc);
                    groupEncLengths[chunk] = enc.Length;
                    groupPlainLengths[chunk] = plain.Length;
                    cumulative = checked(cumulative + enc.Length);
                }

                processed++;
                if ((processed % 500) == 0)
                    progress?.Report(processed * 80.0 / Math.Max(1, original.Header.GroupCount));
            }

            if (buf.Position != bodyOffset + cumulative)
                throw new InvalidDataException($"incremental body size mismatch: {buf.Position - bodyOffset} != {cumulative}");

            // 文件表（明文）
            byte[] fileTable = new byte[n * NkpiReader.FileEntrySize];
            for (int i = 0; i < n; i++)
            {
                NkpiEntry e = original.Entries[i];
                int off = i * NkpiReader.FileEntrySize;
                BitConverter.GetBytes(e.NameOffset).CopyTo(fileTable, off);
                BitConverter.GetBytes(e.PathOffset).CopyTo(fileTable, off + 4);
                BitConverter.GetBytes(e.ChunkIndex).CopyTo(fileTable, off + 8);
                BitConverter.GetBytes(dataOffsets[i]).CopyTo(fileTable, off + 12);
                BitConverter.GetBytes(dataSizes[i]).CopyTo(fileTable, off + 16);
                BitConverter.GetBytes(e.DataType).CopyTo(fileTable, off + 20);
            }

            // 组表（明文 → LCG）
            byte[] groupTable = new byte[original.Header.GroupCount * NkpiReader.GroupEntrySize];
            int running = 0;
            for (int chunk = 0; chunk < original.Header.GroupCount; chunk++)
            {
                running = checked(running + groupEncLengths[chunk]);
                BitConverter.GetBytes(running).CopyTo(groupTable, chunk * 8);
                BitConverter.GetBytes(groupPlainLengths[chunk]).CopyTo(groupTable, chunk * 8 + 4);
            }

            // header（明文 → LCG）
            byte[] header = BuildHeader(original, n, cumulative, original.Header.GroupCount,
                original.Header.HashTableSize, original.Header.NameTableSize);

            buf.Flush();
            outFs.Seek(0, SeekOrigin.Begin);
            outFs.Write(header);
            outFs.Write(fileTable);
            outFs.Seek(groupTableOffset, SeekOrigin.Begin);
            outFs.Write(EncryptSegment(groupTable, original, "group"));
            outFs.Flush();
        }

        progress?.Report(100.0);
    }

    private static void CopyBodyBytes(NkpiReader original, int bodyOffset, int length, Stream output)
    {
        const int copyChunk = 8 << 20;
        for (int off = 0; off < length; off += copyChunk)
        {
            int len = Math.Min(copyChunk, length - off);
            output.Write(original.ReadRawBody(bodyOffset + off, len));
        }
    }

    private static void WriteZeros(Stream output, int count)
    {
        byte[] zeros = new byte[64 * 1024];
        int remaining = count;
        while (remaining > 0)
        {
            int len = Math.Min(zeros.Length, remaining);
            output.Write(zeros, 0, len);
            remaining -= len;
        }
    }

    private static void CopySectionBytes(byte[] source, int offset, int length, Stream output)
    {
        byte[] seg = ReadSection(source, offset, length);
        output.Write(seg, 0, seg.Length);
    }

    private static byte[] RebuildCore(
        NkpiReader original,
        IReadOnlyList<NkpiEntry> entries,
        IReadOnlyList<byte[]> contents,
        int originalGroupCount,
        int outputGroupCount,
        bool originalNameTable,
        Func<int, string> resolveName,
        NkpiNamePoolBuilder? namePool = null,
        IProgress<double>? progress = null)
    {
        int n = entries.Count;
        if (contents.Count != n)
            throw new ArgumentException("entry/content count mismatch", nameof(contents));

        var byChunk = new Dictionary<int, List<int>>();
        for (int i = 0; i < n; i++)
        {
            int chunk = entries[i].ChunkIndex;
            if (!byChunk.TryGetValue(chunk, out List<int>? list))
                byChunk[chunk] = list = new List<int>();
            list.Add(i);
        }

        var body = new List<byte>();
        var groupTable = new List<(int cumulative, int original)>();
        var dataOffsets = new int[n];
        var dataSizes = new int[n];
        int cumulative = 0;
        int processed = 0;

        for (int chunk = 0; chunk < originalGroupCount; chunk++)
        {
            if (!byChunk.TryGetValue(chunk, out List<int>? fileIndices))
                throw new InvalidDataException($"original NKPI group {chunk} has no file entries");

            using MemoryStream plainStream = new();
            foreach (int entryIndex in fileIndices)
            {
                byte[] content = contents[entryIndex];
                dataOffsets[entryIndex] = checked((int)plainStream.Length);
                dataSizes[entryIndex] = content.Length;
                plainStream.Write(content, 0, content.Length);
            }

            byte[] plain = plainStream.ToArray();
            byte[] compressed = Compress(plain);
            byte[] encrypted = EncryptSegment(compressed, original, "body");
            groupTable.Add((checked(cumulative + encrypted.Length), plain.Length));
            body.AddRange(encrypted);
            cumulative = checked(cumulative + encrypted.Length);
            processed++;
            if ((processed % 500) == 0)
                progress?.Report(processed * 100.0 / Math.Max(1, outputGroupCount));
        }

        if (outputGroupCount > originalGroupCount)
        {
            if (!byChunk.TryGetValue(originalGroupCount, out List<int>? additions) || additions.Count == 0)
                throw new InvalidDataException("new NKPI group has no file entries");
            using MemoryStream plainStream = new();
            foreach (int entryIndex in additions)
            {
                byte[] content = contents[entryIndex];
                dataOffsets[entryIndex] = checked((int)plainStream.Length);
                dataSizes[entryIndex] = content.Length;
                plainStream.Write(content, 0, content.Length);
            }
            byte[] plain = plainStream.ToArray();
            byte[] compressed = Compress(plain);
            byte[] encrypted = EncryptSegment(compressed, original, "body");
            groupTable.Add((checked(cumulative + encrypted.Length), plain.Length));
            body.AddRange(encrypted);
            cumulative = checked(cumulative + encrypted.Length);
        }

        byte[] fileTable = BuildFileTable(entries, dataOffsets, dataSizes);
        // 第一阶段修复(2026-08-31):仅更新已有条目时,名称表保持原始字节、
        // 所有 NameOffset/PathOffset 与原始一致,HASH 段引用的索引没有变化,
        // 因此原样复制原始 HASH 段密文字节,避免用未经验证的推测结构重写客户端索引。
        // 新增条目会改变名称池索引,HASH 必须重建;但真实 ProtectedNKPI HASH 结构
        // 尚未通过真实客户端验证,显式停止生成而不是写出可疑数据。
        byte[] hashTable;
        if (originalNameTable && n == original.Entries.Count)
        {
            // 已有条目更新路径:HASH 段原样保留原始密文字节(客户端兼容性已实测确认)。
            hashTable = ReadSection(original.Stream, original.Layout.HashOffset, original.Header.HashTableSize);
        }
        else
        {
            // 新增条目路径:用逆向确认的布局(NkpiHashTable)重建 HASH 段。
            // 名称池构建器是追加式的,既有 magic 保持不变,因此旧 pairs 的 magic 仍有效;
            // 新条目的 magic 来自池构建器,tail 覆盖新旧全部有效 magic。
            if (namePool == null)
                throw new InvalidOperationException("rebuild with additions requires a name pool builder");
            var pairList = new List<(uint name, uint path)>(n);
            for (int i = 0; i < n; i++)
            {
                uint name = unchecked((uint)entries[i].NameOffset);
                uint path = entries[i].PathOffset < 0
                    ? NkpiHashTable.NullPath
                    : unchecked((uint)entries[i].PathOffset);
                pairList.Add((name, path));
            }
            byte[] hashPlain = NkpiHashTable.BuildPlainText(pairList, namePool.Utf8Pool, namePool.Utf16Pool);
            hashTable = NkpiHashTable.Encrypt(hashPlain);
        }
        byte[] nameTable = originalNameTable
            ? ReadSection(original.Stream, original.Layout.NameOffset, original.Header.NameTableSize)
            : namePool!.BuildRawNameTable();
        byte[] groupTableBytes = BuildGroupTable(groupTable, original);
        byte[] header = BuildHeader(original, n, cumulative, groupTable.Count, hashTable.Length, nameTable.Length);

        using MemoryStream output = new();
        output.Write(header);
        output.Write(fileTable);
        output.Write(hashTable);
        output.Write(nameTable);
        output.Write(groupTableBytes);
        output.Write(body.ToArray());
        progress?.Report(100.0);
        return output.ToArray();
    }

    private static byte[] BuildFileTable(IReadOnlyList<NkpiEntry> entries, IReadOnlyList<int> dataOffsets, IReadOnlyList<int> dataSizes)
    {
        byte[] table = new byte[checked(entries.Count * NkpiReader.FileEntrySize)];
        for (int i = 0; i < entries.Count; i++)
        {
            NkpiEntry e = entries[i];
            int off = i * NkpiReader.FileEntrySize;
            BitConverter.GetBytes(e.NameOffset).CopyTo(table, off);
            BitConverter.GetBytes(e.PathOffset).CopyTo(table, off + 4);
            BitConverter.GetBytes(e.ChunkIndex).CopyTo(table, off + 8);
            BitConverter.GetBytes(dataOffsets[i]).CopyTo(table, off + 12);
            BitConverter.GetBytes(dataSizes[i]).CopyTo(table, off + 16);
            BitConverter.GetBytes(e.DataType).CopyTo(table, off + 20);
        }
        return table;
    }

    // 旧 BuildHashTable 推测实现已删除:其明文布局正确,但加密密钥错误(大写 HASH)
    // 且从未通过真实文件验证;正式实现见 NkpiHashTable(逆向确认布局+排序+小写 hash 密钥)。

    private static byte[] BuildGroupTable(IReadOnlyList<(int cumulative, int original)> groups, NkpiReader original)
    {
        byte[] plain = new byte[checked(groups.Count * NkpiReader.GroupEntrySize)];
        for (int i = 0; i < groups.Count; i++)
        {
            BitConverter.GetBytes(groups[i].cumulative).CopyTo(plain, i * 8);
            BitConverter.GetBytes(groups[i].original).CopyTo(plain, i * 8 + 4);
        }
        return EncryptSegment(plain, original, "group");
    }

    private static byte[] BuildHeader(NkpiReader original, int fileCount, int bodySize, int groupCount,
        int hashTableSize, int nameTableSize)
    {
        byte[] plain = new byte[NkpiReader.HeaderSize];
        BitConverter.GetBytes(NkpiReader.Signature).CopyTo(plain, 0);
        Array.Copy(original.Header.Guid, 0, plain, 4, Math.Min(20, original.Header.Guid.Length));
        BitConverter.GetBytes(fileCount).CopyTo(plain, 0x18);
        BitConverter.GetBytes(original.Header.Padding).CopyTo(plain, 0x1C);
        BitConverter.GetBytes(bodySize).CopyTo(plain, 0x20);
        BitConverter.GetBytes(groupCount).CopyTo(plain, 0x24);
        BitConverter.GetBytes(hashTableSize).CopyTo(plain, 0x28);
        BitConverter.GetBytes(nameTableSize).CopyTo(plain, 0x2C);
        return EncryptSegment(plain, original, "header");
    }

    internal static byte[] EncryptName(byte[] data, NkpiReader original, string pool)
        => EncryptSegment(data, original, pool);

    private static byte[] EncryptSegment(byte[] data, NkpiReader original, string section)
    {
        string key = section switch
        {
            "header" => original.Format == NkpiFormatKind.Protected ? "hEAd" : "HeaD",
            // ProtectedNKPI 的 hash 段实测为小写 "hash"(黄金测试确认);Standard 保持历史推测。
            "hash" => original.Format == NkpiFormatKind.Protected ? "hash" : "HASH",
            "group" => original.Format == NkpiFormatKind.Protected ? "grpi" : "GRPI",
            "body" => original.Format == NkpiFormatKind.Protected ? "bODy" : "BodY",
            "utf8" => original.Format == NkpiFormatKind.Protected ? "StRa" : "sTrA",
            "utf16" => original.Format == NkpiFormatKind.Protected ? "StRw" : "sTrW",
            _ => throw new ArgumentOutOfRangeException(nameof(section), section, "unknown NKPI section"),
        };
        uint increment = section is "utf8" or "utf16" ? NkpiReader.NameInc : NkpiReader.SegInc;
        uint seed = original.Format == NkpiFormatKind.Protected
            ? NkpiReader.DeriveSeedUtf16(key)
            : NkpiReader.DeriveSeedAscii(key);
        return NkpiReader.LcgDecrypt(data, seed, increment);
    }

    private static byte[] ReadSection(byte[] source, int offset, int length)
    {
        byte[] result = new byte[length];
        Array.Copy(source, offset, result, 0, length);
        return result;
    }

    private static byte[] Compress(byte[] data)
    {
        using MemoryStream output = new();
        using (ZLibStream zlib = new(output, CompressionLevel.SmallestSize, leaveOpen: true))
            zlib.Write(data, 0, data.Length);
        return output.ToArray();
    }
}
