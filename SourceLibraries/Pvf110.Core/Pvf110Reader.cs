using System.IO.Compression;
using System.Text;

namespace Pvf110.Core;

/// <summary>Pvf110 逻辑流读取器：name 池、文件表、group/body、路径与文件内容。</summary>
public sealed class Pvf110Reader : IPvfNamePool
{
    private const uint NameUtf8Xor = 0xE7ADF7EA;
    private const uint NameCountA = 0xB8DEA7AC;

    public Pvf110Header Header { get; }
    public Pvf110Layout Layout { get; }
    public byte[] Stream { get; }

    /// <summary>打开本归档所用的 chunk key（来自 sk.dat）。重建时复用它，客户端 sk.dat 可保持不变。</summary>
    public byte[][] ChunkKeys { get; private set; } = Array.Empty<byte[]>();

    /// <summary>打开本归档所用的 sk.dat 原始字节。重建时若 chunk key 数量足够，直接原样回写，客户端 sk.dat 无需更换。</summary>
    public byte[] SkDatBytes { get; private set; } = Array.Empty<byte>();

    /// <summary>本次打开所用 sk.dat 的来源：<c>explicit</c>/<c>file</c>/<c>embedded</c>（经 <see cref="OpenPvf"/> 打开时有效）。</summary>
    public string SkDatSource { get; private set; } = "unknown";

    /// <summary>文件版 sk.dat 路径；使用内置 sk.dat 时为 null。</summary>
    public string? SkDatPath { get; private set; }

    /// <summary>HASH 段密文（与 <see cref="Stream"/> 中同一区段同步，仍保持 LCG 加密状态）。</summary>
    public byte[] HashEncrypted => Slice(Stream, Layout.HashOffset, Header.HashTableSize);

    /// <summary>name 表密文（仍保持名称段加密状态；重建时其前 8 字节与两段压缩体需原样保留或按池重算）。</summary>
    public byte[] NameTableBytes => Slice(Stream, Layout.NameOffset, Header.NameTableSize);

    public byte[] Utf8Pool { get; private set; } = Array.Empty<byte>();
    public byte[] Utf16Pool { get; private set; } = Array.Empty<byte>();
    public List<Pvf110Entry> Entries { get; } = new();
    public List<(int cumulative, int original)> Groups { get; } = new();

    private Pvf110Reader(Pvf110Header header, Pvf110Layout layout, byte[] stream)
    {
        Header = header;
        Layout = layout;
        Stream = stream;
    }

    public static Pvf110Reader Open(byte[] skdat, byte[] pvfBytes)
    {
        byte[] stream = Pvf110Crypto.OpenLogicalStream(skdat, pvfBytes, out byte[][] chunkKeys);
        Pvf110Header header = Pvf110Crypto.ParseHeader(stream);
        Pvf110Layout layout = Pvf110Crypto.ComputeLayout(header, stream.Length);
        var r = new Pvf110Reader(header, layout, stream)
        {
            ChunkKeys = chunkKeys,
            SkDatBytes = (byte[])skdat.Clone(),
        };
        r.ParseNameTable();
        r.ParseFileTable();
        r.ParseGroups();
        return r;
    }

    /// <summary>
    /// 从文件路径打开，并**自动**从配套客户端 EXE 现场派生外层包装密钥
    /// （<see cref="Pvf110ClientKeys.EnsureRegistered"/>：PVF 同目录或上溯数层找客户端主程序）。
    /// GUI 与 CLI 都走这一入口，调用方无需预先注册密钥，也无需设置任何环境变量。
    /// </summary>
    public static Pvf110Reader OpenFiles(string skdatPath, string pvfPath, string? clientExePath = null)
    {
        Pvf110ClientKeys.EnsureRegistered(pvfPath, clientExePath);
        return Open(File.ReadAllBytes(skdatPath), File.ReadAllBytes(pvfPath));
    }

    /// <summary>
    /// 打开入口（CLI 与 GUI 共用）：sk.dat 三级回退（显式路径 → 文件探测 → 内置 sk.dat），
    /// 外层包装密钥优先用内置客户端密钥集，找不到时才从客户端 EXE 现场派生。
    /// 因此固定版单机客户端只要给 <paramref name="pvfPath"/> 即可打开，无需任何配套文件。
    /// </summary>
    public static Pvf110Reader OpenPvf(string pvfPath, string? skdatPath = null, string? clientExePath = null)
    {
        Pvf110ClientKeys.EnsureRegistered(pvfPath, clientExePath);
        var (bytes, source, path) = Pvf110Support.LoadSkDat(pvfPath, skdatPath);
        try
        {
            Pvf110Reader reader = Open(bytes, File.ReadAllBytes(pvfPath));
            reader.SkDatSource = source;
            reader.SkDatPath = path;
            return reader;
        }
        catch (Exception ex) when (ex is InvalidDataException or FormatException or IOException)
        {
            throw new InvalidDataException(
                $"不是可识别的 Pvf110 归档（{Path.GetFileName(pvfPath)}，sk.dat 来源={source}）：{ex.Message}" +
                (source == "embedded"
                    ? "；内置 sk.dat 只适用于内置密钥集对应的客户端版本，其他版本请把该客户端的 sk.dat 与 DFO.exe 放到 PVF 同目录（或设 PVF_SKDAT / PVF_CLIENT_EXE）。"
                    : ""),
                ex);
        }
    }

    /// <summary>name 表头部 8 字节（真机为保留/校验字段，重建时原样保留）。</summary>
    public byte[] NameTableHead => Slice(Stream, Layout.NameOffset, 8);

    private void ParseNameTable()
    {
        int ntOff = Layout.NameOffset;
        byte[] nt = new byte[Header.NameTableSize];
        Array.Copy(Stream, ntOff, nt, 0, Header.NameTableSize);

        uint e1 = BitConverter.ToUInt32(nt, 0);
        uint e2 = BitConverter.ToUInt32(nt, 4);

        int cs1 = unchecked((int)(BitConverter.ToUInt32(nt, 8) ^ NameUtf8Xor));
        int os1 = unchecked((int)(BitConverter.ToUInt32(nt, 12) ^ (uint)cs1));
        Utf8Pool = Inflate(Pvf110Crypto.LcgDecryptName(Slice(nt, 16, cs1), "utf8"), os1);

        int p = 16 + cs1;
        int cs2 = unchecked((int)(BitConverter.ToUInt32(nt, p) ^ NameCountA));
        int os2 = unchecked((int)(BitConverter.ToUInt32(nt, p + 4) ^ (uint)cs2));
        Utf16Pool = Inflate(Pvf110Crypto.LcgDecryptName(Slice(nt, p + 8, cs2), "utf16"), os2);
    }

    private void ParseFileTable()
    {
        int ftOff = Pvf110Crypto.HeaderSize;
        for (int i = 0; i < Header.EntryCount; i++)
        {
            int baseOff = ftOff + i * Pvf110Crypto.FileEntrySize;
            Entries.Add(new Pvf110Entry
            {
                Index = i,
                NameOffset = BitConverter.ToInt32(Stream, baseOff),
                PathOffset = BitConverter.ToInt32(Stream, baseOff + 4),
                ChunkIndex = BitConverter.ToInt32(Stream, baseOff + 8),
                DataOffset = BitConverter.ToInt32(Stream, baseOff + 12),
                DataSize = BitConverter.ToInt32(Stream, baseOff + 16),
                DataType = BitConverter.ToInt32(Stream, baseOff + 20),
            });
        }
    }

    private void ParseGroups()
    {
        byte[] raw = Slice(Stream, Layout.GroupOffset, Header.GroupCount * Pvf110Crypto.GroupEntrySize);
        byte[] plain = Pvf110Crypto.LcgDecryptKey(raw, "group");
        for (int i = 0; i < Header.GroupCount; i++)
        {
            int cumulative = BitConverter.ToInt32(plain, i * 8);
            int original = BitConverter.ToInt32(plain, i * 8 + 4);
            Groups.Add((cumulative, original));
        }
    }

    public string ResolveName(int magicOffset)
    {
        if (magicOffset < 0) return $"<neg:{magicOffset}>";
        uint v = unchecked((uint)magicOffset);
        if ((v & 1) == 0)
        {
            int pos = (int)(v >> 1);
            if (pos < 0 || pos >= Utf8Pool.Length) return $"<bad:{v:X8}>";
            int end = Array.IndexOf(Utf8Pool, (byte)0, pos);
            if (end < 0) end = Utf8Pool.Length;
            return Encoding.UTF8.GetString(Utf8Pool, pos, end - pos);
        }
        int pos16 = (int)((v >> 1) * 2);
        if (pos16 < 0 || pos16 + 1 >= Utf16Pool.Length) return $"<bad:{v:X8}>";
        int end16 = pos16;
        while (end16 + 1 < Utf16Pool.Length && !(Utf16Pool[end16] == 0 && Utf16Pool[end16 + 1] == 0))
            end16 += 2;
        return Encoding.Unicode.GetString(Utf16Pool, pos16, end16 - pos16);
    }

    public string FilePath(Pvf110Entry e)
    {
        if (PathMemoEnabled)
        {
            string?[] memo = _pathMemo ??= new string?[Entries.Count];
            string? cached = memo[e.Index];
            if (cached != null) return cached;
            string resolved = ComposePath(e);
            memo[e.Index] = resolved;
            return resolved;
        }
        return ComposePath(e);
    }

    private string ComposePath(Pvf110Entry e)
    {
        string folder = ResolveName(e.PathOffset).Replace('\\', '/').Trim('/');
        string name = ResolveName(e.NameOffset).Replace('\\', '/');
        return folder.Length > 0 ? $"{folder}/{name}" : name;
    }

    /// <summary>
    /// 路径解析记忆化开关（默认关闭）。
    /// 打开后每个条目只解析一次路径（<see cref="Entries"/> 数量 × 2 次名称池解码 → 1 次），
    /// 供"本来就要为全部条目建索引"的调用方（GUI 打开/保存、批量写回）使用：
    /// 这些调用方已经把路径字符串常驻在字典键里，记忆化表只额外持有引用数组
    /// （百万级条目约 8 字节/条），却省掉每次遍历重新分配等量字符串。
    /// 只做流式列目录/校验的调用方（CLI list/validate）保持关闭，避免常驻内存上升。
    /// </summary>
    public bool PathMemoEnabled { get; set; }

    private string?[]? _pathMemo;

    private readonly GroupCache _groupCache = new();

    public byte[] GroupData(int index)
    {
        return _groupCache.GetOrAdd(index, idx =>
        {
            if (idx < 0 || idx >= Groups.Count) throw new IndexOutOfRangeException($"group {idx}");
            int cumulative = Groups[idx].cumulative;
            int original = Groups[idx].original;
            int prev = idx > 0 ? Groups[idx - 1].cumulative : 0;
            byte[] enc = Slice(Stream, Layout.BodyOffset + prev, cumulative - prev);
            byte[] dec = Pvf110Crypto.LcgDecryptKey(enc, "body");
            return Inflate(dec, original);
        });
    }

    public byte[] ReadEntry(Pvf110Entry e)
    {
        byte[] group = GroupData(e.ChunkIndex);
        return Slice(group, e.DataOffset, e.DataSize);
    }

    private static byte[] Inflate(byte[] compressed, int expected)
    {
        using MemoryStream ms = new(compressed);
        using ZLibStream zs = new(ms, CompressionMode.Decompress);
        using MemoryStream outMs = new();
        zs.CopyTo(outMs);
        byte[] result = outMs.ToArray();
        if (result.Length != expected)
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

public sealed class Pvf110Entry
{
    public int Index;
    public int NameOffset;
    public int PathOffset;
    public int ChunkIndex;
    public int DataOffset;
    public int DataSize;
    public int DataType;
}
