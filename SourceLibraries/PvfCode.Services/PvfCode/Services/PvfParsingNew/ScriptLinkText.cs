using System.Collections.Generic;
using PvfCode.Models.Pvf;

namespace PvfCode.Services.PvfParsingNew;

/// <summary>
/// <c>StringLinkIndex</c>（经典 9 ／ 110 tag 0x08）的"链接后是否跟随名称字面量"自适应。
///
/// 两种版式并存，必须按数据实测判断，不能一律取后一项：
/// <list type="bullet">
/// <item>2 token 版式（经典格式）：<c>StringLinkIndex</c> + 名称 <c>String</c>，其后再跟类型/数值字段。
/// 显示文本经 <c>Strview</c>（stringlist 索引）解析。</item>
/// <item>1 token 版式（110 适配：NKPI / ProtectedNKPI / Pvf110）：链接的 payload 已经指向名称池字符串
/// （形如 <c>&lt;3::equipmentpartset_0&gt;</c>），其后**直接**是下一个字段
/// （如套装行的 <c>[hat avatar]</c> 类型 String）。</item>
/// </list>
///
/// 判据：链接后第一项是 <c>String</c>/<c>StringLinkIndex</c>，且再后一项也是 <c>String</c>（典型为类型字段）时，
/// 判为 2 token 版式。90CN 数据中 0x08 零出现，判据对既有行为无影响；115 数据全为 1 token 版式，
/// 旧实现的"一律取后一项"会把类型字段当成名称并跳过它，导致整段行错位。
/// </summary>
public static class ScriptLinkText
{
    /// <summary>
    /// 判断 <paramref name="linkIndex"/> 处是否是与"名称字面量"配对的 2 token 链接；
    /// 是则返回该字面量，否则返回 null（1 token 版式）。
    /// </summary>
    public static ScriptItem? TryGetLinkedLiteral(List<SectionBase>? children, int linkIndex, int count)
    {
        if (children == null) return null;
        if (linkIndex < 0 || linkIndex >= count) return null;
        SectionBase? link = children[linkIndex];
        // 前置条件：本项自身必须是 StringLinkIndex（否则"其后是否跟字面量"无意义）。
        // 90CN 数据无 0x08，此判据对既有行为完全无影响。
        if (link?.Item?.Type != ScriptType.StringLinkIndex) return null;
        int literalIndex = linkIndex + 1;
        int fieldIndex = linkIndex + 2;
        if (literalIndex >= count || fieldIndex >= count) return null;
        SectionBase? literal = children[literalIndex];
        SectionBase? field = children[fieldIndex];
        if (literal?.Item == null || field?.Item == null) return null;
        if (literal is PvfSection || field is PvfSection) return null;
        if (literal.Item.Type != ScriptType.String && literal.Item.Type != ScriptType.StringLinkIndex) return null;
        // 2 token 版式的特征是：链接 → 名称字面量 → 类型 String；1 token 版式此处是类型 String → 数值。
        return field.Item.Type == ScriptType.String ? literal.Item : null;
    }

    /// <summary>
    /// 取链接/普通字符串 token 的显示文本（不带引号）。
    /// 2 token 版式沿用 <c>Strview</c> 解析路径；1 token 版式取链接自身的虚拟串表文本。
    /// </summary>
    public static string Resolve(PvfPack pvf, ScriptItem? item, ScriptItem? linkedLiteral)
    {
        if (item == null) return string.Empty;
        if (item.Type != ScriptType.StringLinkIndex)
            return item.GetItemTextNotChar(pvf);
        if (linkedLiteral != null)
            return item.GetItemTextNotChar(pvf, linkedLiteral);
        return pvf.Strtable.GetStringItem(item.Data) ?? string.Empty;
    }

    /// <summary>
    /// 行版式测量用：若 <paramref name="index"/> 处是 1 token 链接，返回 0；
    /// 若是 2 token 链接（含字面量），返回 1（调用方应额外跳过的项数）。
    /// </summary>
    public static int ExtraLinkedLiteralCount(List<SectionBase>? children, int index, int count)
        => TryGetLinkedLiteral(children, index, count) != null ? 1 : 0;
}
