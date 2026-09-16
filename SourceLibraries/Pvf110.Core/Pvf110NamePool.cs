using System.IO.Compression;
using System.Text;

namespace Pvf110.Core;

/// <summary>
/// Pvf110 名称池写入器：在既有 utf8/utf16 池尾部追加新字符串，并重建 name 表段。
///
/// name 表段布局（与 <see cref="Pvf110Reader.ParseNameTable"/> 互为逆运算）：
/// <code>
///   [8B 保留头][u32 (NameUtf8Xor ^ cs1)][u32 (utf8PlainLen ^ cs1)][cs1 字节]
///              [u32 (NameCountA  ^ cs2)][u32 (utf16PlainLen ^ cs2)][cs2 字节]
/// </code>
/// 其中 csN = LcgDecryptName(zlib(pool), utf8|utf16).Length（名称段 LCG，inc = 0x00269EC9）。
///
/// magic 编码（与读取侧一致）：
///   - 偶数 → UTF-8 池字节偏移的 2 倍；
///   - 奇数 → UTF-16 池 **字符索引** 的 2 倍加 1（即 2 字节对齐偏移 + 1）。
/// 既有字符串一律复用原 magic，只有真正新增的字符串才写入池尾，因此不移动任何既有条目。
/// </summary>
public sealed class Pvf110NamePool
{
    private const uint NameUtf8Xor = 0xE7ADF7EA;
    private const uint NameCountA = 0xB8DEA7AC;

    private readonly byte[] _head;
    private readonly List<byte> _utf8;
    private readonly List<byte> _utf16;

    /// <summary>
    /// 既有字符串 → 池偏移 的反向索引（**惰性构建**，且只存 64 位哈希 + 偏移，逐字节复核段内容）。
    ///
    /// 为什么不用 <c>Dictionary&lt;string,int&gt;</c>：真实 115 归档的 utf16 池近 500 MB / 210 万条字符串，
    /// 直接建字符串字典会额外吃掉约 1 GB 常驻内存，并使每次保存都先做一次全池索引；
    /// 哈希索引把这一开销压到约 1/20（每条约 8+4 字节），且只在首次真正需要"判重"时才构建。
    /// 哈希冲突由 <see cref="SegmentEquals"/> 按池内真实字节复核，不会误判为已存在。
    /// </summary>
    private Dictionary<ulong, List<int>>? _lookupIndex;
    private readonly bool _enableLookupIndex;

    public int Utf8Length => _utf8.Count;
    public int Utf16Length => _utf16.Count;

    /// <summary>最后一次 <see cref="BuildNameTableBytes"/> 使用的压缩级别说明（诊断用）。</summary>
    public string LastCompressionNote { get; private set; } = "";

    /// <summary>本次会话内池尾追加的字符串数量。>0 表示 name 表段必须重建。</summary>
    public int AppendedCount { get; private set; }

    /// <summary>反向索引是否已构建（诊断用）。</summary>
    public bool LookupIndexBuilt => _lookupIndex != null;

    /// <summary>索引条目数（诊断用）。</summary>
    public int LookupIndexCount => _lookupIndex?.Count ?? 0;

    /// <param name="enableLookupIndex">
    /// 为 true 时按需构建"判重"反向索引（<see cref="GetOrAdd"/> 会复用池内既有字符串）。
    /// 为 false 时 <see cref="GetOrAdd"/> 只做追加（不做判重）——用于"确定不会有池内既有字符串"的调用方。
    /// </param>
    public Pvf110NamePool(byte[] head, byte[] utf8Pool, byte[] utf16Pool, bool enableLookupIndex = true)
    {
        _head = head;
        _utf8 = new List<byte>(utf8Pool);
        _utf16 = new List<byte>(utf16Pool);
        _enableLookupIndex = enableLookupIndex;
    }

    public static Pvf110NamePool FromReader(Pvf110Reader reader, bool enableLookupIndex = true)
        => new(reader.NameTableHead, reader.Utf8Pool, reader.Utf16Pool, enableLookupIndex);

    /// <summary>按需构建反向索引（首次判重时执行一次）。</summary>
    private Dictionary<ulong, List<int>> GetOrBuildLookupIndex()
    {
        if (_lookupIndex != null) return _lookupIndex;
        var index = new Dictionary<ulong, List<int>>();
        IndexPool(index, _utf8, even: true);
        IndexPool(index, _utf16, even: false);
        _lookupIndex = index;
        return index;
    }

    private static void IndexPool(Dictionary<ulong, List<int>> index, List<byte> pool, bool even)
    {
        int i = 0;
        while (i < pool.Count)
        {
            int start = i;
            int end = start;
            if (even)
            {
                while (end < pool.Count && pool[end] != 0) end++;
            }
            else
            {
                while (end + 1 < pool.Count && !(pool[end] == 0 && pool[end + 1] == 0)) end += 2;
            }
            ulong h = HashSegment(pool, start, end - start);
            if (!index.TryGetValue(h, out List<int>? bucket))
            {
                bucket = new List<int>(1);
                index[h] = bucket;
            }
            if (!bucket.Contains(start)) bucket.Add(start);
            i = even ? end + 1 : end + 2;
        }
    }

    /// <summary>段内容哈希（FNV-1a 64）。</summary>
    private static ulong HashSegment(List<byte> pool, int offset, int length)
    {
        ulong h = 14695981039346656037UL;
        for (int i = 0; i < length; i++)
        {
            h ^= pool[offset + i];
            h *= 1099511628211UL;
        }
        return h;
    }

    /// <summary>池内真实字节复核：长度与内容逐字节比较。</summary>
    private static bool SegmentEquals(List<byte> pool, int offset, byte[] bytes)
    {
        if (offset < 0 || offset + bytes.Length > pool.Count) return false;
        for (int i = 0; i < bytes.Length; i++)
        {
            if (pool[offset + i] != bytes[i]) return false;
        }
        return true;
    }

    /// <summary>按既有池查找字符串的 magic；未命中返回 false。优先 UTF-16 池（真机主要池）。</summary>
    public bool TryGetMagic(string value, out int magic)
    {
        magic = -1;
        if (!_enableLookupIndex) return false;
        byte[] utf16Bytes = Encoding.Unicode.GetBytes(value);
        if (TryMatch(_utf16, utf16Bytes, even: false, out int offset16))
        {
            magic = offset16 | 1;
            return true;
        }
        if (_utf8.Count > 0)
        {
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(value);
            if (TryMatch(_utf8, utf8Bytes, even: true, out int offset8))
            {
                magic = offset8 << 1;
                return true;
            }
        }
        return false;
    }

    private bool TryMatch(List<byte> pool, byte[] encoded, bool even, out int offset)
    {
        offset = -1;
        var index = GetOrBuildLookupIndex();
        if (!index.TryGetValue(HashSegmentOf(encoded), out List<int>? bucket)) return false;
        foreach (int candidate in bucket)
        {
            if (SegmentEquals(pool, candidate, encoded))
            {
                offset = candidate;
                return true;
            }
        }
        return false;
    }

    /// <summary>独立字节数组的 FNV-1a 64 哈希（与 <see cref="HashSegment"/> 同算法）。</summary>
    private static ulong HashSegmentOf(byte[] data)
    {
        ulong h = 14695981039346656037UL;
        for (int i = 0; i < data.Length; i++)
        {
            h ^= data[i];
            h *= 1099511628211UL;
        }
        return h;
    }

    /// <summary>
    /// 取已有或追加新字符串的 magic。新字符串追加到 UTF-16 池尾部
    /// （真机 115 归档的 utf8 池为空，名称全部来自 utf16 池，保持同一承载方式）。
    /// </summary>
    public int GetOrAdd(string value)
    {
        if (_enableLookupIndex && TryGetMagic(value, out int magic)) return magic;
        int offset = _utf16.Count;
        byte[] bytes = Encoding.Unicode.GetBytes(value);
        _utf16.AddRange(bytes);
        _utf16.Add(0);
        _utf16.Add(0);
        if (_lookupIndex != null)
        {
            ulong h = HashSegmentOf(bytes);
            if (!_lookupIndex.TryGetValue(h, out List<int>? bucket))
            {
                bucket = new List<int>(1);
                _lookupIndex[h] = bucket;
            }
            bucket.Add(offset);
        }
        AppendedCount++;
        return offset | 1;
    }

    /// <summary>构建新的 name 表段字节（已按名称段密钥加密）。</summary>
    public byte[] BuildNameTableBytes()
    {
        byte[] cs1Raw = Pvf110Crypto.LcgDecryptName(Compress(_utf8), "utf8");
        byte[] cs2Raw = Pvf110Crypto.LcgDecryptName(Compress(_utf16), "utf16");
        using var ms = new MemoryStream();
        ms.Write(_head, 0, _head.Length);
        WriteU32(ms, NameUtf8Xor ^ (uint)cs1Raw.Length);
        WriteU32(ms, (uint)_utf8.Count ^ (uint)cs1Raw.Length);
        ms.Write(cs1Raw, 0, cs1Raw.Length);
        WriteU32(ms, NameCountA ^ (uint)cs2Raw.Length);
        WriteU32(ms, (uint)_utf16.Count ^ (uint)cs2Raw.Length);
        ms.Write(cs2Raw, 0, cs2Raw.Length);
        return ms.ToArray();
    }

    /// <summary>重建后的池字节（供 HASH 段排序使用）。</summary>
    public (byte[] utf8Pool, byte[] utf16Pool) Pools => (_utf8.ToArray(), _utf16.ToArray());

    private static void WriteU32(Stream s, uint v)
    {
        byte[] b = BitConverter.GetBytes(v);
        s.Write(b, 0, b.Length);
    }

    /// <summary>
    /// zlib 压缩池字节。真实 115 归档 utf16 池近 500 MB：直接把 <see cref="List{T}"/> 分块喂给压缩流，
    /// 不再先 <c>ToArray()</c> 复制一份（旧实现峰值多占一份池体积）。
    /// </summary>
    private byte[] Compress(List<byte> data)
    {
        // 大池（真实 115 归档 utf16 池近 500 MB）用 SmallestSize 压缩极其昂贵且无收益：
        // name 表体积不参与任何语义，客户端只按 os（明文长度）解压校验，
        // 因此超过阈值改用 Optimal，压缩级别写入诊断文本。
        var level = data.Count > 64 * 1024 * 1024 ? CompressionLevel.Optimal : CompressionLevel.SmallestSize;
        LastCompressionNote = $"utf8={_utf8.Count}B utf16={_utf16.Count}B level={level}";
        using var ms = new MemoryStream();
        using (var zs = new ZLibStream(ms, level, leaveOpen: true))
        {
            const int Chunk = 1 << 20;
            var buffer = new byte[Chunk];
            int offset = 0;
            while (offset < data.Count)
            {
                int take = Math.Min(Chunk, data.Count - offset);
                for (int i = 0; i < take; i++) buffer[i] = data[offset + i];
                zs.Write(buffer, 0, take);
                offset += take;
            }
        }
        return ms.ToArray();
    }
}
