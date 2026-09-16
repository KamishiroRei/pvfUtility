namespace Pvf110.Core;

/// <summary>
/// Pvf110 HASH 段：与 ProtectedNKPI 共用同一条布局，只有段密钥不同。
///
/// 明文布局（与 NkpiHashTable 一致，逐字节自证由 CLI <c>hash-test</c> 在真实归档上执行）：
/// <code>
///   [pairCount][pairCount × (nameMagic, pathMagic)][uniqueCount][uniqueCount × magic]
/// </code>
/// 段加密：LCG，key = UTF-16 "HSrm" 派生 seed，inc = 0x00269EC3（与 header/group/body 同族）。
///
/// 与 Pvf110Crypto.KeyWords 的关系：本类只包一层语义命名，
/// 实际加解密仍由 <see cref="Pvf110Crypto.LcgDecryptKey"/> 完成（XOR 自反，加解密同函数）。
/// </summary>
public static class Pvf110HashTable
{
    /// <summary>无路径（根目录文件）的 sentinel，与 NkpiHashTable.NullPath 相同。</summary>
    public const uint NullPath = 0xFFFFFFFFu;

    /// <summary>Pvf110Crypto.KeyWords 中 HASH 段的词表键。</summary>
    public const string KeyLabel = "hash";

    public static byte[] Decrypt(byte[] encrypted) => Pvf110Crypto.LcgDecryptKey(encrypted, KeyLabel);

    public static byte[] Encrypt(byte[] plain) => Pvf110Crypto.LcgDecryptKey(plain, KeyLabel);

    /// <summary>读取 pairCount（明文前 4 字节）。越界抛出 <see cref="InvalidDataException"/>。</summary>
    public static int ReadPairCount(byte[] plain)
    {
        if (plain.Length < 8) throw new InvalidDataException("Pvf110 hash plaintext too small");
        int pairCount = BitConverter.ToInt32(plain, 0);
        if (pairCount < 0 || 4L + (long)pairCount * 8 + 4 > plain.Length)
            throw new InvalidDataException($"Pvf110 hash pairCount {pairCount} out of bounds");
        return pairCount;
    }

    /// <summary>读取 pairs（名称池 magic 对）。</summary>
    public static List<(uint name, uint path)> ReadPairs(byte[] plain)
    {
        int pairCount = ReadPairCount(plain);
        var pairs = new List<(uint name, uint path)>(pairCount);
        for (int i = 0; i < pairCount; i++)
        {
            uint name = BitConverter.ToUInt32(plain, 4 + i * 8);
            uint path = BitConverter.ToUInt32(plain, 4 + i * 8 + 4);
            pairs.Add((name, path));
        }
        return pairs;
    }

    /// <summary>读取尾部唯一 magic 计数。</summary>
    public static int ReadUniqueCount(byte[] plain)
    {
        int pairCount = ReadPairCount(plain);
        return BitConverter.ToInt32(plain, 4 + pairCount * 8);
    }

    /// <summary>
    /// 构建 HASH 明文。布局与排序规则复用 ProtectedNKPI 已逆向确认的实现，
    /// 保证同一 pairs 输入总是产生逐字节相同的输出。
    /// </summary>
    public static byte[] BuildPlainText(
        IReadOnlyList<(uint name, uint path)> pairs,
        byte[] utf8Pool,
        byte[] utf16Pool)
        => NkpiHashTable.BuildPlainText(pairs, utf8Pool, utf16Pool);

    /// <summary>
    /// 结构自证：解密明文 → 按其 pairs 重建 → 逐字节比较。
    /// mismatch=0 表示布局与排序算法完全复现真实归档。
    /// </summary>
    public static (int pairCount, int uniqueCount, long mismatchBytes, long firstMismatch) SelfTest(
        byte[] decryptedPlain,
        byte[] utf8Pool,
        byte[] utf16Pool)
        => NkpiHashTable.SelfTest(decryptedPlain, utf8Pool, utf16Pool);

    /// <summary>
    /// 增量构建：在既有 HASH 明文尾部追加新 pair，并把新出现的 magic 按同一排序规则
    /// 插入尾部唯一集合。<b>既有 pair 与其顺序、既有尾部字节一律原样保留</b>，
    /// 因此纯内容编辑（无新增路径）不会改动 HASH 段的任何既存字节。
    /// </summary>
    public static byte[] AppendPairs(
        byte[] originalPlain,
        IReadOnlyList<(uint name, uint path)> newPairs,
        byte[] utf8Pool,
        byte[] utf16Pool)
    {
        if (newPairs.Count == 0) return (byte[])originalPlain.Clone();

        List<(uint name, uint path)> pairs = ReadPairs(originalPlain);
        int uniqueCount = BitConverter.ToInt32(originalPlain, 4 + pairs.Count * 8);
        int tailStart = 4 + pairs.Count * 8 + 4;
        if (tailStart + uniqueCount * 4 > originalPlain.Length)
            throw new InvalidDataException("Pvf110 hash tail exceeds plaintext length");

        var tail = new List<uint>(uniqueCount + newPairs.Count * 2);
        var present = new HashSet<uint>(uniqueCount);
        for (int i = 0; i < uniqueCount; i++)
        {
            uint m = BitConverter.ToUInt32(originalPlain, tailStart + i * 4);
            tail.Add(m);
            present.Add(m);
        }

        pairs.AddRange(newPairs);
        foreach ((uint name, uint path) in newPairs)
        {
            foreach (uint m in new[] { name, path })
            {
                if (m == NullPath || !present.Add(m)) continue;
                InsertSorted(tail, m, utf8Pool, utf16Pool);
            }
        }

        using var ms = new MemoryStream(4 + pairs.Count * 8 + 4 + tail.Count * 4);
        Write(ms, pairs.Count);
        foreach ((uint name, uint path) in pairs)
        {
            Write(ms, unchecked((int)name));
            Write(ms, unchecked((int)path));
        }
        Write(ms, tail.Count);
        foreach (uint m in tail) Write(ms, unchecked((int)m));
        return ms.ToArray();
    }

    /// <summary>尾部唯一集合的插入位置：按名称池字节段升序，同段按 magic 升序。</summary>
    private static void InsertSorted(List<uint> tail, uint magic, byte[] utf8Pool, byte[] utf16Pool)
    {
        byte[] seg = NkpiHashTable.PoolSegment(utf8Pool, utf16Pool, magic);
        int lo = 0, hi = tail.Count;
        while (lo < hi)
        {
            int mid = (lo + hi) / 2;
            if (CompareEntry(tail[mid], magic, seg, utf8Pool, utf16Pool) <= 0) lo = mid + 1;
            else hi = mid;
        }
        tail.Insert(lo, magic);
    }

    /// <summary>比较既有条目与待插入条目：先比名称池字节段，同段按 magic 升序。</summary>
    private static int CompareEntry(uint existingMagic, uint insertedMagic, byte[] insertedSeg, byte[] utf8Pool, byte[] utf16Pool)
    {
        int c = NkpiHashTable.CompareSegment(NkpiHashTable.PoolSegment(utf8Pool, utf16Pool, existingMagic), insertedSeg);
        return c != 0 ? c : existingMagic.CompareTo(insertedMagic);
    }

    /// <summary>
    /// 追加后校验：既有 pair 全部保留且顺序不变，既有尾部 magic 全部保留，入口条目数一致。
    /// 返回 null 表示通过，否则返回失败原因。
    /// </summary>
    public static string? VerifyAppend(
        byte[] originalPlain,
        byte[] rebuiltPlain,
        IReadOnlyList<(uint name, uint path)> appendedPairs,
        int expectedPairCount)
    {
        int beforeCount = ReadPairCount(originalPlain);
        int afterCount = ReadPairCount(rebuiltPlain);
        if (afterCount != expectedPairCount)
            return $"pairCount {afterCount} != expected {expectedPairCount}";
        List<(uint name, uint path)> before = ReadPairs(originalPlain);
        List<(uint name, uint path)> after = ReadPairs(rebuiltPlain);
        for (int i = 0; i < beforeCount; i++)
        {
            if (before[i] != after[i])
                return $"existing pair {i} changed: ({before[i].name:X8},{before[i].path:X8}) -> ({after[i].name:X8},{after[i].path:X8})";
        }
        for (int i = 0; i < appendedPairs.Count; i++)
        {
            if (after[beforeCount + i] != appendedPairs[i])
                return $"appended pair {i} mismatch";
        }
        int beforeUnique = ReadUniqueCount(originalPlain);
        int afterUnique = ReadUniqueCount(rebuiltPlain);
        int tailStartBefore = 4 + beforeCount * 8 + 4;
        int tailStartAfter = 4 + afterCount * 8 + 4;
        var afterSet = new HashSet<uint>();
        for (int i = 0; i < afterUnique; i++)
            afterSet.Add(BitConverter.ToUInt32(rebuiltPlain, tailStartAfter + i * 4));
        for (int i = 0; i < beforeUnique; i++)
        {
            uint m = BitConverter.ToUInt32(originalPlain, tailStartBefore + i * 4);
            if (!afterSet.Contains(m)) return $"original tail magic {m:X8} lost after append";
        }
        return null;
    }

    private static void Write(Stream s, int v)
    {
        byte[] b = BitConverter.GetBytes(v);
        s.Write(b, 0, b.Length);
    }
}
