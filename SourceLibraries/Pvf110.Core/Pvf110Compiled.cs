using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Pvf110.Core;

/// <summary>Pvf110 type-1 数据文件编译格式：token = TAG + int32，字符串引用 name 池。</summary>
public sealed class Pvf110Compiled
{
    public const byte TagI32 = 0x00;
    public const byte TagF32 = 0x02;
    public const byte TagS03 = 0x03;
    public const byte TagS06 = 0x06;
    public const byte TagS08 = 0x08;

    private readonly IPvfNamePool _reader;
    private Dictionary<string, int>? _poolIndex;

    public Pvf110Compiled(Pvf110Reader reader)
    {
        _reader = reader;
    }

    public Pvf110Compiled(NkpiReader reader)
    {
        _reader = reader;
    }

    public string Resolve(int v)
    {
        uint u = unchecked((uint)v);
        if ((u & 1) == 0)
        {
            int pos = (int)(u >> 1);
            if (pos < 0 || pos >= _reader.Utf8Pool.Length) return $"<bad:{u:X8}>";
            int end = Array.IndexOf(_reader.Utf8Pool, (byte)0, pos);
            if (end < 0) end = _reader.Utf8Pool.Length;
            return Encoding.UTF8.GetString(_reader.Utf8Pool, pos, end - pos);
        }
        int pos16 = (int)((u >> 1) * 2);
        if (pos16 < 0 || pos16 + 1 >= _reader.Utf16Pool.Length) return $"<bad:{u:X8}>";
        int end16 = pos16;
        while (end16 + 1 < _reader.Utf16Pool.Length && !(_reader.Utf16Pool[end16] == 0 && _reader.Utf16Pool[end16 + 1] == 0))
            end16 += 2;
        return Encoding.Unicode.GetString(_reader.Utf16Pool, pos16, end16 - pos16);
    }

    /// <summary>[(tag, value)] value 对字符串是解析后文本，对 I32/F32 是数值。</summary>
    public List<(byte tag, object value, string raw)> DecodeTokens(byte[] data)
    {
        List<(byte, object, string)> outL = new();
        int i = 0, n = data.Length;
        while (i < n)
        {
            if (i + 5 > n) { outL.Add((0xFF, data[i..].ToHex(), data[i..].ToHex())); break; }
            byte tag = data[i];
            int val = BitConverter.ToInt32(data, i + 1);
            string raw = data.AsSpan(i, 5).ToHex();
            if (tag is TagS03 or TagS06 or TagS08)
                outL.Add((tag, Resolve(val), raw));
            else if (tag == TagI32)
                outL.Add((tag, val, raw));
            else if (tag == TagF32)
                outL.Add((tag, BitConverter.ToSingle(data, i + 1), raw));
            else
                outL.Add((tag, data.AsSpan(i, 5).ToHex(), raw));
            i += 5;
        }
        return outL;
    }

    public string ToText(byte[] data)
    {
        StringBuilder sb = new();
        foreach (var (tag, value, _) in DecodeTokens(data))
        {
            switch (tag)
            {
                case TagS03: sb.Append(FmtString((string)value)); break;
                case TagS06: sb.Append('\t').Append(FmtString((string)value)); break;
                case TagS08: sb.Append("\t{S08}").Append(Escape((string)value)); break;
                case TagI32: sb.Append('\t').Append(value); break;
                case TagF32: sb.Append('\t').Append(FormatFloat((float)value)); break;
                default: sb.Append("RAW\t").Append(value); break;
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }

    /// <summary>
    /// 经典 pvf 脚本风格文本（GUI 编辑器显示用）：
    /// #PVF_File 头；`[标签]` 字符串独立成行；普通字符串反引号包裹独立成行；
    /// S06 缩进字符串与 int/float 以 \t 缩进独立成值行。由 FromText 解析还原
    /// （解析器同时兼容旧 ToText 逐行格式与多值行）。
    /// </summary>
    public string ToScriptText(byte[] data)
    {
        StringBuilder sb = new("#PVF_File\r\n");
        bool lineOpen = false;
        foreach (var (tag, value, _) in DecodeTokens(data))
        {
            switch (tag)
            {
                case TagS03:
                {
                    if (lineOpen) sb.Append("\r\n");
                    string s = (string)value;
                    if (s.Length > 0 && s[0] == '[')
                        sb.Append(Escape(s)).Append("\r\n");
                    else
                        sb.Append('`').Append(Escape(s)).Append("`\r\n");
                    lineOpen = false;
                    break;
                }
                case TagS06:
                case TagS08:
                    if (lineOpen) sb.Append("\r\n");
                    if (tag == TagS06)
                        sb.Append('\t').Append(FmtString((string)value)).Append("\r\n");
                    else
                        sb.Append("\t{S08}").Append(Escape((string)value)).Append("\r\n");
                    lineOpen = false;
                    break;
                case TagI32:
                    sb.Append('\t').Append(value);
                    lineOpen = true;
                    break;
                case TagF32:
                    sb.Append('\t').Append(FormatFloat((float)value));
                    lineOpen = true;
                    break;
                default:
                    if (lineOpen) sb.Append("\r\n");
                    sb.Append("RAW\t").Append(value).Append("\r\n");
                    lineOpen = false;
                    break;
            }
        }
        if (lineOpen) sb.Append("\r\n");
        return sb.ToString();
    }

    /// <summary>字符串值：纯数字形式用反引号包裹，避免与 I32/F32 混淆。</summary>
    private static string FmtString(string s)
    {
        string esc = Escape(s);
        if (s.Length > 0 &&
            (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
             || float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out _)))
            return "`" + esc + "`";
        return esc;
    }

    private static bool IsBacktickWrapped(string s)
        => s.Length >= 2 && s[0] == '`' && s[^1] == '`';

    public byte[] FromText(string text, Func<string, int>? missingStringResolver = null)
    {
        if (_reader is null) throw new InvalidOperationException("need reader for pool lookup");
        BuildPoolIndex();
        List<byte> outB = new();
        string[] parts = text.Split('\n');
        // 去掉文件末尾换行产生的空元素；中间的空行 = S03 空字符串 token
        int last = parts.Length;
        if (last > 0 && parts[last - 1].Length == 0) last--;
        int start = 0;
        if (last > 0 && parts[0].AsSpan().TrimEnd('\r').SequenceEqual("#PVF_File"))
            start = 1; // 经典脚本头
        for (int i = start; i < last; i++)
        {
            string line = parts[i];
            if (line.Length > 0 && line[^1] == '\r') line = line[..^1];
            if (line.StartsWith("RAW\t")) { outB.AddRange(Convert.FromHexString(line[4..])); continue; }
            if (line.Length == 0)
            {
                // S03 空字符串
                if (!TryGetOffset("", missingStringResolver, out int off0))
                    throw new InvalidDataException("empty string not in name pool");
                outB.Add(TagS03);
                outB.AddRange(BitConverter.GetBytes(off0));
                continue;
            }
            if (line.StartsWith('\t'))
            {
                // 值行：\t 分隔的多个值（int / float / `字符串` / {S08}...）
                string[] segments = line.Split('\t');
                bool any = false;
                foreach (string raw in segments)
                {
                    if (raw.Length == 0) continue;
                    any = true;
                    if (raw.StartsWith("{S08}"))
                    {
                        if (!TryGetOffset(Unescape(raw[5..]), missingStringResolver, out int offS8))
                            throw new InvalidDataException($"string not in name pool: {raw[5..]}");
                        outB.Add(TagS08);
                        outB.AddRange(BitConverter.GetBytes(offS8));
                        continue;
                    }
                    if (IsBacktickWrapped(raw))
                    {
                        if (!TryGetOffset(Unescape(raw[1..^1]), missingStringResolver, out int offS6))
                            throw new InvalidDataException($"string not in name pool: {raw[1..^1]}");
                        outB.Add(TagS06);
                        outB.AddRange(BitConverter.GetBytes(offS6));
                        continue;
                    }
                    if (long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out long iv))
                    {
                        outB.Add(TagI32);
                        outB.AddRange(BitConverter.GetBytes(unchecked((int)iv)));
                        continue;
                    }
                    if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float fv))
                    {
                        outB.Add(TagF32);
                        outB.AddRange(BitConverter.GetBytes(fv));
                        continue;
                    }
                    if (!TryGetOffset(Unescape(raw), missingStringResolver, out int offV))
                        throw new InvalidDataException($"string not in name pool: {raw}");
                    outB.Add(TagS06);
                    outB.AddRange(BitConverter.GetBytes(offV));
                }
                if (!any)
                {
                    // 整行只有一个 "\t"：S06 空字符串
                    if (!TryGetOffset("", missingStringResolver, out int offE))
                        throw new InvalidDataException("empty string not in name pool");
                    outB.Add(TagS06);
                    outB.AddRange(BitConverter.GetBytes(offE));
                }
                continue;
            }
            // 字符串行：反引号包裹或原文（如 [标签]）
            string content;
            byte tag2;
            if (IsBacktickWrapped(line))
            {
                content = Unescape(line[1..^1]);
                tag2 = TagS03;
            }
            else
            {
                content = Unescape(line);
                tag2 = TagS03;
            }
            if (!TryGetOffset(content, missingStringResolver, out int off))
                throw new InvalidDataException($"string not in name pool: {content}");
            outB.Add(tag2);
            outB.AddRange(BitConverter.GetBytes(off));
        }
        return outB.ToArray();
    }

    private bool TryGetOffset(string value, Func<string, int>? missingStringResolver, out int offset)
    {
        if (_poolIndex!.TryGetValue(value, out offset))
            return true;
        if (missingStringResolver == null)
            return false;
        offset = missingStringResolver(value);
        return true;
    }

    /// <summary>转义字符串中的反斜杠/换行/回车/制表/反引号，避免破坏文本行结构。</summary>
    private static string Escape(string s)
        => s.Replace("\\", "\\\\")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("`", "\\`");

    /// <summary>反转义 Escape 的输出（先还原 \\ 再还原其余）。</summary>
    private static string Unescape(string s)
    {
        var sb = new StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '\\' && i + 1 < s.Length)
            {
                char n = s[i + 1];
                if (n == '\\') { sb.Append('\\'); i++; continue; }
                if (n == 'r') { sb.Append('\r'); i++; continue; }
                if (n == 'n') { sb.Append('\n'); i++; continue; }
                if (n == 't') { sb.Append('\t'); i++; continue; }
                if (n == '`') { sb.Append('`'); i++; continue; }
            }
            sb.Append(s[i]);
        }
        return sb.ToString();
    }

    private static string FormatFloat(float value)
    {
        string s = value.ToString("R", CultureInfo.InvariantCulture);
        // 确保与整数区分：整数形式的浮点附加 ".0"
        if (!s.Contains('.') && !s.Contains('E') && !s.Contains('e'))
            s += ".0";
        return s;
    }

    private void BuildPoolIndex()
    {
        if (_poolIndex != null) return;
        var idx = new Dictionary<string, int>();
        byte[] pool = _reader.Utf16Pool;
        int pos = 0, n = pool.Length;
        while (pos + 1 < n)
        {
            int end = pos;
            while (end + 1 < n && !(pool[end] == 0 && pool[end + 1] == 0)) end += 2;
            if (end == pos)
            {
                idx.TryAdd("", pos | 1);
                pos += 2;
            }
            else
            {
                idx.TryAdd(Encoding.Unicode.GetString(pool, pos, end - pos), pos | 1);
                pos = end + 2;
            }
        }
        byte[] pool8 = _reader.Utf8Pool;
        pos = 0;
        int n8 = pool8.Length;
        while (pos < n8)
        {
            int end = Array.IndexOf(pool8, (byte)0, pos);
            if (end < 0) end = n8;
            if (end > pos)
                idx.TryAdd(Encoding.UTF8.GetString(pool8, pos, end - pos), pos << 1);
            pos = end + 1;
        }
        _poolIndex = idx;
    }
}

internal static class HexExt
{
    public static string ToHex(this ReadOnlySpan<byte> bytes)
        => Convert.ToHexString(bytes);
}
