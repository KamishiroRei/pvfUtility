namespace Pvf110.Core;

/// <summary>
/// 90CN(NKPI/ProtectedNKPI)/Pvf110 token 流 ↔ 经典 pvfUtility 脚本视图 适配器。
///
/// 经典视图 = 2 字节魔数 0xD0B0 + 每格 5 字节（经典 ScriptType 字节 + int32），
/// 字符串字段存"虚拟串表 ID"（由宿主用名称池构建，见 PvfCode Stringtable.LoadFromNamePool）。
/// 经典视图落地后 PvfFile.IsScriptFile 成立，旧版整条解析栈
/// （ScriptFileParserNew/CustomSectionFormat/ScriptFileCompilerOl/名称扫描/注释/IMG 链接）原样生效。
///
/// 标签映射（2026-09-01 全量普查 90CN 1,055,164 个 type-1 条目 / 约 1.35 亿 token 实证）：
///   数值族   0x00↔Int(2)  0x01↔IntEx(3)  0x02↔Float(4)                 —— payload 原样直通
///   0x03↔Section(5)  0x05↔Command(6)  0x06↔String(7)
///   0x07↔CommandSeparator(8)  0x08↔StringLinkIndex(9)  0x09↔StringLink(10)
///            —— 字符串族（含 Section），payload 需在池偏移 ↔ 虚拟串表 ID 间换算
/// 0x04 与 0x08/0x09 在 90CN 当前数据中零出现；0x08/0x09 按族规律对称映射保留。
/// 字符串族的 payload 在 110 侧是名称池 magic offset，在经典侧是虚拟串表 ID，
/// 由调用方提供的委托完成双向换算。
/// </summary>
public static class ClassicViewAdapter
{
    /// <summary>经典脚本魔数（小端 uint16 = 0xD0B0 = 53424）。</summary>
    public const ushort ClassicMagic = 0xD0B0;

    /// <summary>
    /// 110 token 流 → 经典视图。字符串族 token（Section/Command/String/CommandSeparator/
    /// StringLink 族，含 0x03 Section——其 payload 同样是名称池偏移）经
    /// <paramref name="acquireVirtualIdByOffset"/> 把池偏移换算为虚拟串表 ID；
    /// 数值族（0x00 Int / 0x01 IntEx / 0x02 Float）payload 原样直通。
    /// 遇到未知标签抛 <see cref="InvalidDataException"/>（逐文件失败，不静默丢数据）；
    /// 末尾不足 5 字节的残缺 token 按填充处理丢弃。
    /// </summary>
    public static byte[] ToClassicView(byte[] data, Func<int, int> acquireVirtualIdByOffset)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (acquireVirtualIdByOffset == null) throw new ArgumentNullException(nameof(acquireVirtualIdByOffset));

        byte[] output = new byte[2 + (data.Length / 5) * 5];
        output[0] = 0xB0;
        output[1] = 0xD0;
        int write = 2;
        for (int i = 0; i + 5 <= data.Length; i += 5)
        {
            byte tag = data[i];
            int payload = BitConverter.ToInt32(data, i + 1);
            byte classicTag;
            if (tag is 0x00 or 0x01 or 0x02)
            {
                classicTag = (byte)(tag + 2);            // 数值族：0x00→Int(2) 0x01→IntEx(3) 0x02→Float(4)
            }
            else if (tag == 0x03)
            {
                classicTag = 5;                          // Section：算术 +2，payload 是池偏移需映射
                payload = acquireVirtualIdByOffset(payload);
            }
            else if (tag is >= 0x05 and <= 0x09)
            {
                classicTag = (byte)(tag + 1);            // 字符串族：0x05→Command(6) … 0x09→StringLink(10)
                payload = acquireVirtualIdByOffset(payload);
            }
            else
            {
                throw new InvalidDataException($"未知的 110 token 标签 0x{tag:X2}（偏移 {i}），无法生成经典视图");
            }
            output[write] = classicTag;
            BitConverter.GetBytes(payload).CopyTo(output, write + 1);
            write += 5;
        }
        return output;
    }

    /// <summary>
    /// 经典视图 → 110 token 流。字符串族 token（Section(5) 与 Command(6)…StringLink(10)）
    /// 经 <paramref name="getVirtualText"/> 取文本、<paramref name="poolGetOrAdd"/> 换算名称池偏移
    /// （新字符串在池构建器中追加）；数值族 payload 原样直通。
    /// <paramref name="poolGetOrAdd"/> 为 null 且出现池外新字符串时抛
    /// "string not in name pool: …"（与 NKPI 增量保存的全量重建回退约定保持一致）。
    /// </summary>
    public static byte[] FromClassicView(byte[] classic, Func<int, string> getVirtualText, Func<string, int>? poolGetOrAdd)
    {
        if (classic == null) throw new ArgumentNullException(nameof(classic));
        if (getVirtualText == null) throw new ArgumentNullException(nameof(getVirtualText));
        if (classic.Length < 2 || BitConverter.ToUInt16(classic, 0) != ClassicMagic)
            throw new InvalidDataException("经典视图缺少 0xD0B0 魔数");

        byte[] output = new byte[Math.Max(0, (classic.Length - 2) / 5) * 5];
        int write = 0;
        for (int i = 2; i + 5 <= classic.Length; i += 5)
        {
            byte classicTag = classic[i];
            int payload = BitConverter.ToInt32(classic, i + 1);
            byte tag;
            if (classicTag is 2 or 3 or 4)
            {
                tag = (byte)(classicTag - 2);            // 数值族：Int(2)→0x00 IntEx(3)→0x01 Float(4)→0x02
            }
            else if (classicTag == 5)
            {
                tag = 0x03;                              // Section：算术 −2，payload 是虚拟串表 ID 需映射
                string text = getVirtualText(payload) ?? string.Empty;
                if (poolGetOrAdd == null)
                    throw new InvalidDataException("string not in name pool: " + text);
                payload = poolGetOrAdd(text);
            }
            else if (classicTag is >= 6 and <= 10)
            {
                tag = (byte)(classicTag - 1);            // 字符串族：Command(6)→0x05 … StringLink(10)→0x09
                string text = getVirtualText(payload) ?? string.Empty;
                if (poolGetOrAdd == null)
                    throw new InvalidDataException("string not in name pool: " + text);
                payload = poolGetOrAdd(text);
            }
            else
            {
                throw new InvalidDataException($"未知的经典 ScriptType {classicTag}（偏移 {i}），无法适配为 110 token");
            }
            output[write] = tag;
            BitConverter.GetBytes(payload).CopyTo(output, write + 1);
            write += 5;
        }
        return output;
    }
}
