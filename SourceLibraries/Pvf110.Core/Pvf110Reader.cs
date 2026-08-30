using System.Collections.Concurrent;
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
        byte[] stream = Pvf110Crypto.OpenLogicalStream(skdat, pvfBytes);
        Pvf110Header header = Pvf110Crypto.ParseHeader(stream);
        Pvf110Layout layout = Pvf110Crypto.ComputeLayout(header, stream.Length);
        var r = new Pvf110Reader(header, layout, stream);
        r.ParseNameTable();
        r.ParseFileTable();
        r.ParseGroups();
        return r;
    }

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
        string folder = ResolveName(e.PathOffset).Replace('\\', '/').Trim('/');
        string name = ResolveName(e.NameOffset).Replace('\\', '/');
        return folder.Length > 0 ? $"{folder}/{name}" : name;
    }

    private readonly ConcurrentDictionary<int, byte[]> _groupCache = new();

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
