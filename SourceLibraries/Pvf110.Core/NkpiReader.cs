using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text;

namespace Pvf110.Core;

/// <summary>读取器共享的 name 池接口（供 Pvf110Compiled 解编译复用）。</summary>
public interface IPvfNamePool
{
    byte[] Utf8Pool { get; }
    byte[] Utf16Pool { get; }
}

/// <summary>NKPI 归档格式变体。</summary>
public enum NkpiFormatKind
{
    /// <summary>标准 NKPI：ASCII 4B key 派生，guard 旋转，key=HeaD/HASH/GRPI/BodY/sTrA/sTrW。</summary>
    Standard,
    /// <summary>Protected NKPI（23.4.15.0+）：UTF-16 seed 派生，无 guard，key=hEAd/grpi/bODy/StRa/StRw。</summary>
    Protected,
}

/// <summary>
/// NKPI 系列 DNF Script.pvf 读取器（标准 + Protected）。
/// 布局：[Header 0x30][FileTable 24B*N][HashTable][NameTable(strA/strW)][GRPI 8B*M][Body zlib 组]。
/// - Standard：LCG ASCII 派生，guard 旋转，header=HeaD、hash=HASH、group=GRPI、body=BodY、name=sTrA/sTrW。
/// - Protected：LCG UTF-16 seed 派生（与 Pvf110 相同），无 guard，header=hEAd、group=grpi、body=bODy、name=StRa/StRw。
/// 两者均无外层 sk.dat 保护，直接 LCG 解密逻辑流。
/// </summary>
public sealed class NkpiReader : IPvfNamePool
{
    public const uint Signature = 0x69706B6Eu; // "nkpi"
    public const int HeaderSize = 0x30;
    public const int FileEntrySize = 0x18;
    public const int GroupEntrySize = 8;

    public const uint LcgA = 0x000343FD;
    public const uint SegInc = 0x00269EC3;  // header/hash/group/body 段
    public const uint NameInc = 0x00269EC9; // name 表

    public NkpiFormatKind Format { get; }
    public NkpiHeader Header { get; }
    public NkpiLayout Layout { get; }
    public byte[] Stream { get; }
    public byte[] Utf8Pool { get; }
    public byte[] Utf16Pool { get; }
    public List<NkpiEntry> Entries { get; } = new();
    public List<(int cumulative, int original)> Groups { get; } = new();

    private readonly ConcurrentDictionary<int, byte[]> _groupCache = new();

    private NkpiReader(NkpiFormatKind format, NkpiHeader header, NkpiLayout layout, byte[] stream,
        byte[] utf8Pool, byte[] utf16Pool)
    {
        Format = format;
        Header = header;
        Layout = layout;
        Stream = stream;
        Utf8Pool = utf8Pool;
        Utf16Pool = utf16Pool;
    }

    /// <summary>自动检测并打开 NKPI / ProtectedNKPI；失败抛异常。</summary>
    public static NkpiReader Open(byte[] pvfBytes)
    {
        // 先试 Protected（无 guard，UTF-16 seed），再试 Standard（guard + ASCII seed）
        NkpiHeader? header = TryDecodeHeader(ProtectedKeyWords["header"], seedMode: true, guard: false, pvfBytes);
        NkpiFormatKind format = NkpiFormatKind.Protected;
        if (header == null)
        {
            header = TryDecodeHeader(StandardKeyWords["header"], seedMode: false, guard: true, pvfBytes);
            if (header == null)
            {
                header = TryDecodeHeader(StandardKeyWords["header"], seedMode: false, guard: false, pvfBytes);
                format = NkpiFormatKind.Standard;
            }
            else
            {
                format = NkpiFormatKind.Standard;
            }
        }
        if (header == null)
            throw new InvalidDataException("NKPI/ProtectedNKPI header decryption failed (unsupported format)");

        // 布局校验
        NkpiLayout layout = ComputeLayout(header, pvfBytes.Length);

        // name 表
        byte[] nameRaw = Slice(pvfBytes, layout.NameOffset, layout.GroupOffset - layout.NameOffset);
        var (strA, strW) = format == NkpiFormatKind.Protected
            ? ParseNameTableProtected(nameRaw)
            : ParseNameTableStandard(nameRaw);

        // group 表
        var groups = new List<(int, int)>();
        {
            byte[] grpiRaw = Slice(pvfBytes, layout.GroupOffset, header.GroupCount * GroupEntrySize);
            DecryptKey(grpiRaw, "group", format);
            for (int i = 0; i < header.GroupCount; i++)
            {
                groups.Add((BitConverter.ToInt32(grpiRaw, i * 8), BitConverter.ToInt32(grpiRaw, i * 8 + 4)));
            }
        }

        var reader = new NkpiReader(format, header, layout, pvfBytes, strA, strW);
        reader.Groups.AddRange(groups);

        // file table（明文）
        for (int i = 0; i < header.FileCount; i++)
        {
            int off = HeaderSize + i * FileEntrySize;
            reader.Entries.Add(new NkpiEntry
            {
                Index = i,
                NameOffset = BitConverter.ToInt32(pvfBytes, off),
                PathOffset = BitConverter.ToInt32(pvfBytes, off + 4),
                ChunkIndex = BitConverter.ToInt32(pvfBytes, off + 8),
                DataOffset = BitConverter.ToInt32(pvfBytes, off + 12),
                DataSize = BitConverter.ToInt32(pvfBytes, off + 16),
                DataType = BitConverter.ToInt32(pvfBytes, off + 20),
            });
        }
        return reader;
    }

    /// <summary>快速检测：标准 NKPI 或 ProtectedNKPI 任一成功即返回 true。</summary>
    public static bool IsNkpi(byte[] pvfBytes)
    {
        if (pvfBytes.Length < HeaderSize) return false;
        if (TryDecodeHeader(ProtectedKeyWords["header"], seedMode: true, guard: false, pvfBytes) != null) return true;
        if (TryDecodeHeader(StandardKeyWords["header"], seedMode: false, guard: true, pvfBytes) != null) return true;
        return TryDecodeHeader(StandardKeyWords["header"], seedMode: false, guard: false, pvfBytes) != null;
    }

    // ─── Key 词表 ───────────────────────────────────────────────────────

    private static readonly Dictionary<string, string> StandardKeyWords = new()
    {
        ["header"] = "HeaD",
        ["hash"] = "HASH",
        ["group"] = "GRPI",
        ["body"] = "BodY",
        ["utf8"] = "sTrA",
        ["utf16"] = "sTrW",
    };

    private static readonly Dictionary<string, string> ProtectedKeyWords = new()
    {
        ["header"] = "hEAd",
        ["hash"] = "HASH",
        ["group"] = "grpi",
        ["body"] = "bODy",
        ["utf8"] = "StRa",
        ["utf16"] = "StRw",
    };

    // ─── LCG 加解密 ─────────────────────────────────────────────────────

    /// <summary>ASCII 4B key → LCG seed（标准 NKPI）。</summary>
    internal static uint DeriveSeedAscii(string asciiKey)
    {
        byte[] k = Encoding.ASCII.GetBytes(asciiKey);
        if (k.Length < 4) throw new ArgumentException("key must be 4 ASCII bytes", nameof(asciiKey));
        unchecked
        {
            return (uint)(k[0] * 0x76826701uL)
                + (uint)(0x1C1u * (uint)(k[3] + 0x1C1u * (uint)(k[2] + 0x1C1u * k[1])));
        }
    }

    /// <summary>UTF-16 seed 派生（Protected NKPI 与 Pvf110 相同）。</summary>
    internal static uint DeriveSeedUtf16(string key)
    {
        byte[] key16 = Encoding.Unicode.GetBytes(key);
        if (key16.Length < 8) throw new ArgumentException("key must be 4 UTF-16 words", nameof(key));
        uint w0 = BitConverter.ToUInt16(key16, 0);
        uint w1 = BitConverter.ToUInt16(key16, 2);
        uint w2 = BitConverter.ToUInt16(key16, 4);
        uint w3 = BitConverter.ToUInt16(key16, 6);
        return unchecked(w0 * 0x339E9711 + ((w1 * 0x393 + w2) * 0x393 + w3) * 0x393);
    }

    internal static byte[] LcgDecrypt(byte[] data, uint seed, uint inc)
    {
        byte[] outb = (byte[])data.Clone();
        int n = outb.Length;
        int end = (n >> 2) << 2;
        int i = 0;
        uint state = seed;
        while (i < end)
        {
            uint t1 = unchecked(state * LcgA + inc);
            state = unchecked(t1 * LcgA + inc);
            uint xorKey = unchecked((t1 & 0xFFFF0000) + ((state >> 16) & 0xFFFF));
            uint v = BitConverter.ToUInt32(outb, i) ^ xorKey;
            BitConverter.GetBytes(v).CopyTo(outb, i);
            i += 4;
        }
        int tail = n - end;
        if (tail > 0)
        {
            uint t1 = unchecked(state * LcgA + inc);
            uint t2 = unchecked(t1 * LcgA + inc);
            uint finalKey = unchecked((t1 & 0xFFFF0000) + ((t2 >> 16) & 0xFFFF));
            byte[] kb = BitConverter.GetBytes(finalKey);
            for (int k = 0; k < tail; k++) outb[end + k] ^= kb[k];
        }
        return outb;
    }

    private static void DecryptKey(byte[] data, string label, NkpiFormatKind format)
    {
        byte[] dec = format == NkpiFormatKind.Protected
            ? LcgDecrypt(data, DeriveSeedUtf16(ProtectedKeyWords[label]), SegInc)
            : LcgDecrypt(data, DeriveSeedAscii(StandardKeyWords[label]), SegInc);
        Array.Copy(dec, 0, data, 0, data.Length);
    }

    private static void DecryptName(byte[] data, string utf8Or16, NkpiFormatKind format)
    {
        byte[] dec = format == NkpiFormatKind.Protected
            ? LcgDecrypt(data, DeriveSeedUtf16(ProtectedKeyWords[utf8Or16]), NameInc)
            : LcgDecrypt(data, DeriveSeedAscii(StandardKeyWords[utf8Or16]), NameInc);
        Array.Copy(dec, 0, data, 0, data.Length);
    }

    // ─── Header 解密 ────────────────────────────────────────────────────

    private static NkpiHeader? TryDecodeHeader(string key, bool seedMode, bool guard, byte[] pvfBytes)
    {
        byte[] headerBytes = new byte[HeaderSize];
        Array.Copy(pvfBytes, 0, headerBytes, 0, HeaderSize);
        if (guard)
            for (int i = 24; i < 28 && i < headerBytes.Length; i++) headerBytes[i] ^= 0x55;

        byte[] dec = seedMode
            ? LcgDecrypt(headerBytes, DeriveSeedUtf16(key), SegInc)
            : LcgDecrypt(headerBytes, DeriveSeedAscii(key), SegInc);

        uint magic = BitConverter.ToUInt32(dec, 0);
        if (magic != Signature) return null;

        var h = new NkpiHeader
        {
            Magic = magic,
            Guid = new byte[0x14],
            FileCount = BitConverter.ToInt32(dec, 0x18),
            Padding = BitConverter.ToInt32(dec, 0x1C),
            BodySize = BitConverter.ToInt32(dec, 0x20),
            GroupCount = BitConverter.ToInt32(dec, 0x24),
            HashTableSize = BitConverter.ToInt32(dec, 0x28),
            NameTableSize = BitConverter.ToInt32(dec, 0x2C),
        };
        Array.Copy(dec, 4, h.Guid, 0, 0x14);

        if (h.FileCount < 0 || h.BodySize < 0 || h.GroupCount < 0 || h.HashTableSize < 0 || h.NameTableSize < 0)
            return null;

        try
        {
            int expected = HeaderSize
                + h.FileCount * FileEntrySize
                + h.HashTableSize
                + h.NameTableSize
                + h.GroupCount * GroupEntrySize
                + h.BodySize;
            if (expected != pvfBytes.Length) return null;
        }
        catch { return null; }

        return h;
    }

    public static NkpiLayout ComputeLayout(NkpiHeader header, int fileSize)
    {
        int hashOffset = HeaderSize + header.FileCount * FileEntrySize;
        int nameOffset = hashOffset + header.HashTableSize;
        int groupOffset = nameOffset + header.NameTableSize;
        int bodyOffset = groupOffset + header.GroupCount * GroupEntrySize;
        int endOffset = bodyOffset + header.BodySize;
        if (endOffset != fileSize)
            throw new InvalidDataException($"NKPI layout end {endOffset} != file size {fileSize}");
        return new NkpiLayout
        {
            HashOffset = hashOffset,
            NameOffset = nameOffset,
            GroupOffset = groupOffset,
            BodyOffset = bodyOffset,
            EndOffset = endOffset,
        };
    }

    // ─── Name 表解析 ────────────────────────────────────────────────────

    /// <summary>标准 NKPI name 表：encField ^ xorConst = 压缩大小，第二字段=解压大小。</summary>
    private static (byte[] strA, byte[] strW) ParseNameTableStandard(byte[] nameRaw)
    {
        const uint xorA = 0xAA74472E;
        const uint xorW = 0x9A82F037;

        int p = 8; // 跳过 8B 前缀
        int csA = unchecked((int)(BitConverter.ToUInt32(nameRaw, p) ^ xorA));
        int osA = unchecked((int)BitConverter.ToUInt32(nameRaw, p + 4));
        byte[] encA = Slice(nameRaw, p + 8, csA);
        DecryptName(encA, "utf8", NkpiFormatKind.Standard);
        byte[] strA = Inflate(encA, osA, out _);

        p += 8 + csA;
        int csW = unchecked((int)(BitConverter.ToUInt32(nameRaw, p) ^ xorW));
        int osW = unchecked((int)BitConverter.ToUInt32(nameRaw, p + 4));
        byte[] encW = Slice(nameRaw, p + 8, csW);
        DecryptName(encW, "utf16", NkpiFormatKind.Standard);
        byte[] strW = Inflate(encW, osW, out _);
        return (strA, strW);
    }

    /// <summary>Protected NKPI name 表：encSize ^ xorConst = 压缩大小；原始大小 = encodedOriginalSize ^ encSize。</summary>
    private static (byte[] strA, byte[] strW) ParseNameTableProtected(byte[] nameRaw)
    {
        const uint xorA = 0xAA74472E;
        const uint xorW = 0x9A82F037;

        int p = 8; // 跳过 8B 前缀
        uint encSizeField = BitConverter.ToUInt32(nameRaw, p);
        uint encOrigField = BitConverter.ToUInt32(nameRaw, p + 4);
        p += 8;
        int csA = unchecked((int)(encSizeField ^ xorA));
        byte[] encA = Slice(nameRaw, p, csA);
        DecryptName(encA, "utf8", NkpiFormatKind.Protected);
        int osA = unchecked((int)(encOrigField ^ (uint)csA));
        byte[] strA = Inflate(encA, osA, out _);
        p += csA;

        uint encSizeFieldW = BitConverter.ToUInt32(nameRaw, p);
        uint encOrigFieldW = BitConverter.ToUInt32(nameRaw, p + 4);
        p += 8;
        int csW = unchecked((int)(encSizeFieldW ^ xorW));
        byte[] encW = Slice(nameRaw, p, csW);
        DecryptName(encW, "utf16", NkpiFormatKind.Protected);
        int osW = unchecked((int)(encOrigFieldW ^ (uint)csW));
        byte[] strW = Inflate(encW, osW, out _);
        return (strA, strW);
    }

    // ─── 名称解析 ────────────────────────────────────────────────────────

    public string ResolveName(int magicOffset)
    {
        if (magicOffset < 0) return "";
        uint v = unchecked((uint)magicOffset);
        if ((v & 1) == 0)
        {
            int pos = (int)(v >> 1);
            if (pos < 0 || pos >= Utf8Pool.Length) return "";
            int end = Array.IndexOf(Utf8Pool, (byte)0, pos);
            if (end < 0) end = Utf8Pool.Length;
            return Encoding.UTF8.GetString(Utf8Pool, pos, end - pos);
        }
        int pos16 = (int)((v >> 1) * 2);
        if (pos16 < 0 || pos16 + 1 >= Utf16Pool.Length) return "";
        int end16 = pos16;
        while (end16 + 1 < Utf16Pool.Length && !(Utf16Pool[end16] == 0 && Utf16Pool[end16 + 1] == 0))
            end16 += 2;
        return Encoding.Unicode.GetString(Utf16Pool, pos16, end16 - pos16);
    }

    /// <summary>兼容 Pvf110Compiled 的 Resolve 接口。</summary>
    public string Resolve(int v) => ResolveName(v);

    public string FilePath(NkpiEntry e)
    {
        string folder = ResolveName(e.PathOffset).Replace('\\', '/').Trim('/');
        string name = ResolveName(e.NameOffset).Replace('\\', '/');
        return folder.Length > 0 ? $"{folder}/{name}" : name;
    }

    // ─── Body 读取 ──────────────────────────────────────────────────────

    public byte[] GroupData(int index)
    {
        return _groupCache.GetOrAdd(index, idx =>
        {
            if (idx < 0 || idx >= Groups.Count) throw new IndexOutOfRangeException($"group {idx}");
            int cumulative = Groups[idx].cumulative;
            int original = Groups[idx].original;
            int prev = idx > 0 ? Groups[idx - 1].cumulative : 0;
            byte[] enc = Slice(Stream, Layout.BodyOffset + prev, cumulative - prev);
            DecryptKey(enc, "body", Format);
            return Inflate(enc, original, out _);
        });
    }

    public byte[] ReadEntry(NkpiEntry e)
    {
        byte[] group = GroupData(e.ChunkIndex);
        if (e.DataOffset < 0 || e.DataSize < 0 || e.DataOffset + e.DataSize > group.Length)
            throw new InvalidDataException($"entry {e.Index} out of group bounds");
        return Slice(group, e.DataOffset, e.DataSize);
    }

    // ─── 工具函数 ────────────────────────────────────────────────────────

    private static byte[] Inflate(byte[] compressed, int expected, out int actual)
    {
        using MemoryStream ms = new(compressed);
        using ZLibStream zs = new(ms, CompressionMode.Decompress);
        using MemoryStream outMs = new();
        zs.CopyTo(outMs);
        byte[] result = outMs.ToArray();
        actual = result.Length;
        if (expected >= 0 && result.Length != expected)
            throw new InvalidDataException($"zlib size mismatch {result.Length} != {expected}");
        return result;
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

public sealed class NkpiHeader
{
    public uint Magic;
    public byte[] Guid = Array.Empty<byte>();
    public int FileCount;
    public int Padding;
    public int BodySize;
    public int GroupCount;
    public int HashTableSize;
    public int NameTableSize;
}

public sealed class NkpiLayout
{
    public int HashOffset;
    public int NameOffset;
    public int GroupOffset;
    public int BodyOffset;
    public int EndOffset;
}

public sealed class NkpiEntry
{
    public int Index;
    public int NameOffset;
    public int PathOffset;
    public int ChunkIndex;
    public int DataOffset;
    public int DataSize;
    public int DataType;
}
