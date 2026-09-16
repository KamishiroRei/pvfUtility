using System.Text;

namespace Pvf110.Core;

/// <summary>
/// ProtectedNKPI HASH 段实现:布局与排序规则已通过真实文件黄金测试逆向确认。
///
/// 明文布局(大小与 90CN 生产 PVF 精确吻合):
/// <code>
///   [pairCount][pairCount × (nameMagic, pathMagic)][uniqueCount][uniqueCount × magic]
/// </code>
///
/// 语义(实测确认):
///   - pairs 为每条目的 (name, path);根目录文件 path = 0xFFFFFFFF;顺序为集合语义,
///     客户端按字符串定位而不是按位置。
///   - 尾部数组为 pairs 中全部有效 magic(排除 0xFFFFFFFF)的唯一集合,
///     按名称池字符串字节字典序升序排列;并列(同字符串多 magic)按 magic 升序。
///   - pairCount 与集合内容允许与文件表失配:实测生产 PVF 存在 ±19 条幽灵对、
///     23 个 magic 失配,客户端照常加载。加密正确性是硬要求,内容准确性是宽松要求。
///
/// 加密:key = 小写 "hash"(UTF-16 seed 派生)+ SegInc。
/// 此前工具使用大写 "HASH" 加密,客户端解密 HASH 段失败,是写回报错的直接原因。
/// </summary>
public static class NkpiHashTable
{
    public const uint NullPath = 0xFFFFFFFFu;

    /// <summary>名称池字节段比较:逐字节字典序,短段在前。</summary>
    public static int CompareSegment(byte[] a, byte[] b)
    {
        int n = Math.Min(a.Length, b.Length);
        for (int i = 0; i < n; i++)
        {
            int d = a[i] - b[i];
            if (d != 0) return d;
        }
        return a.Length - b.Length;
    }

    /// <summary>
    /// 取 magic 指向的池字符串字节段(不含终止符);偶数 magic 取 UTF-8 池,
    /// 奇数 magic 取 UTF-16 池(UTF-16LE 字节);0xFFFFFFFF 返回空段。
    /// </summary>
    public static byte[] PoolSegment(byte[] utf8Pool, byte[] utf16Pool, uint magic)
    {
        if (magic == NullPath) return Array.Empty<byte>();
        if ((magic & 1) == 0)
        {
            int pos = checked((int)(magic >> 1));
            if (pos < 0 || pos >= utf8Pool.Length) return Array.Empty<byte>();
            int end = Array.IndexOf(utf8Pool, (byte)0, pos);
            if (end < 0) end = utf8Pool.Length;
            byte[] seg = new byte[end - pos];
            Array.Copy(utf8Pool, pos, seg, 0, seg.Length);
            return seg;
        }
        int pos16 = checked((int)((magic >> 1) * 2));
        if (pos16 < 0 || pos16 >= utf16Pool.Length) return Array.Empty<byte>();
        int end16 = pos16;
        while (end16 + 1 < utf16Pool.Length && !(utf16Pool[end16] == 0 && utf16Pool[end16 + 1] == 0))
            end16 += 2;
        if (end16 > utf16Pool.Length) end16 = utf16Pool.Length;
        byte[] segW = new byte[end16 - pos16];
        Array.Copy(utf16Pool, pos16, segW, 0, segW.Length);
        return segW;
    }

    /// <summary>
    /// 按逆向确认的布局构建 HASH 明文。pairs 顺序按传入顺序写入(集合语义);
    /// 尾部唯一 magic 按池字节段升序排列,并列(同字符串多 magic)按 magic 升序,
    /// 因此同一输入总是产生逐字节相同的输出。
    /// </summary>
    public static byte[] BuildPlainText(IReadOnlyList<(uint name, uint path)> pairs, byte[] utf8Pool, byte[] utf16Pool)
    {
        var uniq = new HashSet<uint>();
        foreach ((uint name, uint path) pr in pairs)
        {
            if (pr.name != NullPath) uniq.Add(pr.name);
            if (pr.path != NullPath) uniq.Add(pr.path);
        }

        var entries = new List<(uint magic, byte[] seg)>(uniq.Count);
        foreach (uint m in uniq)
            entries.Add((m, PoolSegment(utf8Pool, utf16Pool, m)));
        entries.Sort(static (x, y) =>
        {
            int c = CompareSegment(x.seg, y.seg);
            return c != 0 ? c : x.magic.CompareTo(y.magic);
        });

        using MemoryStream ms = new();
        ms.Write(BitConverter.GetBytes(checked(pairs.Count)));
        foreach ((uint name, uint path) pr in pairs)
        {
            ms.Write(BitConverter.GetBytes(pr.name));
            ms.Write(BitConverter.GetBytes(pr.path));
        }
        ms.Write(BitConverter.GetBytes(entries.Count));
        foreach ((uint magic, _) in entries)
            ms.Write(BitConverter.GetBytes(magic));
        return ms.ToArray();
    }

    /// <summary>用逆向确认的密钥(小写 hash + UTF-16 seed + SegInc)解密 HASH 段密文。</summary>
    public static byte[] Decrypt(byte[] encrypted)
        => NkpiReader.LcgDecrypt(encrypted, NkpiReader.DeriveSeedUtf16("hash"), NkpiReader.SegInc);

    /// <summary>用逆向确认的密钥加密 HASH 明文。</summary>
    public static byte[] Encrypt(byte[] plain)
        => NkpiReader.LcgDecrypt(plain, NkpiReader.DeriveSeedUtf16("hash"), NkpiReader.SegInc);

    /// <summary>
    /// 结构自证:从解密明文中读出 pairs,重新构建明文并与原明文逐字节对比。
    /// 返回 (pairCount, uniqueCount, mismatchBytes, firstMismatchOffset)。
    /// mismatch=0 表示布局与排序算法完全复现真实文件。
    /// </summary>
    public static (int pairCount, int uniqueCount, long mismatchBytes, long firstMismatch) SelfTest(byte[] decryptedPlain, byte[] utf8Pool, byte[] utf16Pool)
    {
        int pairCount = BitConverter.ToInt32(decryptedPlain, 0);
        if (pairCount < 0 || 4L + (long)pairCount * 8 + 4 > decryptedPlain.Length)
            throw new InvalidDataException($"HASH pairCount {pairCount} out of bounds");
        var pairs = new List<(uint name, uint path)>(pairCount);
        for (int i = 0; i < pairCount; i++)
        {
            uint name = BitConverter.ToUInt32(decryptedPlain, 4 + i * 8);
            uint path = BitConverter.ToUInt32(decryptedPlain, 4 + i * 8 + 4);
            pairs.Add((name, path));
        }
        int uniqueCount = BitConverter.ToInt32(decryptedPlain, 4 + pairCount * 8);
        byte[] rebuilt = BuildPlainText(pairs, utf8Pool, utf16Pool);
        long mismatch = 0, first = -1;
        int n = Math.Min(rebuilt.Length, decryptedPlain.Length);
        for (int i = 0; i < n; i++)
        {
            if (rebuilt[i] != decryptedPlain[i])
            {
                mismatch++;
                if (first < 0) first = i;
            }
        }
        if (rebuilt.Length != decryptedPlain.Length) mismatch += Math.Abs(rebuilt.Length - decryptedPlain.Length);
        return (pairCount, uniqueCount, mismatch, first);
    }
}
