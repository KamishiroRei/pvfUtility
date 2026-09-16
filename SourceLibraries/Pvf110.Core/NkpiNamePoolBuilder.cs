using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace Pvf110.Core;

/// <summary>
/// ProtectedNKPI/NKPI 名称池增量构建器。
///
/// 既有字符串的 magic offset 保持不变；缺少的字符串按现有 PVF 规则追加到
/// UTF-8 或 UTF-16 池，并在最后重新压缩、加密名称表。调用方可在编译新增
/// type-1 文件时使用 <see cref="GetOrAdd"/>，从而让脚本 token 与名称表一次性
/// 对齐。
/// </summary>
public sealed class NkpiNamePoolBuilder
{
    private const uint StrAXor = 0xAA74472E;
    private const uint StrWXor = 0x9A82F037;

    private readonly NkpiReader _original;
    private readonly Dictionary<string, int> _offsets = new(StringComparer.Ordinal);
    private readonly List<byte> _utf8;
    private readonly List<byte> _utf16;
    private readonly byte[] _prefix;

    public NkpiNamePoolBuilder(NkpiReader original)
    {
        _original = original ?? throw new ArgumentNullException(nameof(original));
        _utf8 = new List<byte>(original.Utf8Pool);
        _utf16 = new List<byte>(original.Utf16Pool);
        if ((_utf16.Count & 1) != 0)
            _utf16.Add(0);

        _prefix = new byte[Math.Min(8, original.Header.NameTableSize)];
        if (_prefix.Length > 0)
            Array.Copy(original.Stream, original.Layout.NameOffset, _prefix, 0, _prefix.Length);

        // 与 Pvf110Compiled.BuildPoolIndex 保持一致：UTF-16 池优先，避免
        // 同文字符串在既有 PVF 中发生 offset 漂移。
        AddExistingPool(_utf16.ToArray(), isUtf16: true);
        AddExistingPool(_utf8.ToArray(), isUtf16: false);
    }

    public byte[] Utf8Pool => _utf8.ToArray();
    public byte[] Utf16Pool => _utf16.ToArray();

    /// <summary>解析当前构建器中的 magic offset，供哈希表重建使用。</summary>
    public string Resolve(int magicOffset)
    {
        if (magicOffset < 0)
            return string.Empty;
        uint value = unchecked((uint)magicOffset);
        if ((value & 1) == 0)
        {
            int pos = checked((int)(value >> 1));
            if (pos < 0 || pos >= _utf8.Count)
                return string.Empty;
            int end = _utf8.IndexOf(0, pos);
            if (end < 0)
                end = _utf8.Count;
            return Encoding.UTF8.GetString(CollectionsMarshal.AsSpan(_utf8)[pos..end]);
        }

        int pos16 = checked((int)((value >> 1) * 2));
        if (pos16 < 0 || pos16 + 1 >= _utf16.Count)
            return string.Empty;
        int end16 = pos16;
        while (end16 + 1 < _utf16.Count && !(_utf16[end16] == 0 && _utf16[end16 + 1] == 0))
            end16 += 2;
        return Encoding.Unicode.GetString(CollectionsMarshal.AsSpan(_utf16)[pos16..end16]);
    }

    /// <summary>取得既有字符串 offset；没有时按名称池规则追加并返回新 offset。</summary>
    public int GetOrAdd(string value)
    {
        value ??= string.Empty;
        if (_offsets.TryGetValue(value, out int existing))
            return existing;

        bool useUtf16 = value.Any(c => c > 127) || (value.Length > 0 && char.IsWhiteSpace(value[0]));
        if (useUtf16)
        {
            if ((_utf16.Count & 1) != 0)
                _utf16.Add(0);
            int byteOffset = _utf16.Count;
            _utf16.AddRange(Encoding.Unicode.GetBytes(value));
            _utf16.Add(0);
            _utf16.Add(0);
            int magic = checked((byteOffset / 2) * 2 | 1);
            _offsets.Add(value, magic);
            return magic;
        }

        int utf8Offset = _utf8.Count;
        _utf8.AddRange(Encoding.UTF8.GetBytes(value));
        _utf8.Add(0);
        int utf8Magic = checked(utf8Offset * 2);
        _offsets.Add(value, utf8Magic);
        return utf8Magic;
    }

    /// <summary>按当前归档变体生成加密名称表。</summary>
    public byte[] BuildRawNameTable()
    {
        byte[] compressedA = Compress(_utf8.ToArray());
        byte[] encryptedA = NkpiRepacker.EncryptName(compressedA, _original, "utf8");
        byte[] compressedW = Compress(_utf16.ToArray());
        byte[] encryptedW = NkpiRepacker.EncryptName(compressedW, _original, "utf16");

        using MemoryStream output = new();
        output.Write(_prefix, 0, _prefix.Length);
        // name 表的第二字段表示解压后的池大小。ProtectedNKPI 只是把它
        // 进一步编码为 originalSize ^ compressedSize；不能把 compressed
        // length 自身再传进去，否则读取器会按错误大小校验 zlib 结果。
        WriteBlock(output, encryptedA, _utf8.Count, StrAXor, _original.Format == NkpiFormatKind.Protected);
        WriteBlock(output, encryptedW, _utf16.Count, StrWXor, _original.Format == NkpiFormatKind.Protected);
        return output.ToArray();
    }

    private void AddExistingPool(byte[] pool, bool isUtf16)
    {
        if (isUtf16)
        {
            for (int pos = 0; pos + 1 < pool.Length;)
            {
                int end = pos;
                while (end + 1 < pool.Length && !(pool[end] == 0 && pool[end + 1] == 0))
                    end += 2;
                string value = Encoding.Unicode.GetString(pool, pos, end - pos);
                _offsets.TryAdd(value, checked((pos / 2) * 2 | 1));
                pos = end + 2;
            }
            return;
        }

        for (int pos = 0; pos < pool.Length;)
        {
            int end = Array.IndexOf(pool, (byte)0, pos);
            if (end < 0)
                end = pool.Length;
            string value = Encoding.UTF8.GetString(pool, pos, end - pos);
            if (end > pos)
                _offsets.TryAdd(value, checked(pos * 2));
            pos = end + 1;
        }
    }

    private static void WriteBlock(Stream output, byte[] encrypted, int decompressedSize, uint xorConst, bool protectedFormat)
    {
        uint first = unchecked((uint)encrypted.Length) ^ xorConst;
        uint second = protectedFormat
            ? unchecked((uint)decompressedSize) ^ unchecked((uint)encrypted.Length)
            : unchecked((uint)decompressedSize);
        output.Write(BitConverter.GetBytes(first));
        output.Write(BitConverter.GetBytes(second));
        output.Write(encrypted);
    }

    private static byte[] Compress(byte[] data)
    {
        using MemoryStream output = new();
        using (ZLibStream zlib = new(output, CompressionLevel.SmallestSize, leaveOpen: true))
            zlib.Write(data, 0, data.Length);
        return output.ToArray();
    }
}
