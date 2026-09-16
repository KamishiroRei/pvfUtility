using System;
using PvfCode.Models.Pvf;

namespace PvfCode.Services.PvfParsingNew;

public class ScriptItem
{
	public ScriptType Type { get; set; }

	public int Data { get; set; }

	public string GetItemText(PvfPack pvf, ScriptItem? nextItem = null)
	{
		switch (Type)
		{
		case ScriptType.Int:
			return Data.ToString();
		case ScriptType.Float:
			return DataHelper.FormatFloat(BitConverter.ToSingle(BitConverter.GetBytes(Data), 0));
		case ScriptType.IntEx:
			return $"{{{(int)Type}={Data}}}";
		case ScriptType.Section:
			return GetStringTableValue(pvf, Data);
		case ScriptType.String:
			return "`" + GetStringTableValue(pvf, Data) + "`";
		case ScriptType.Command:
		case ScriptType.CommandSeparator:
			return $"{{{(int)Type}=`{GetStringTableValue(pvf, Data)}`}}";
		case ScriptType.StringLinkIndex:
		{
			// 1 token 版式（110 适配：ProtectedNKPI / Pvf110）：链接后**没有** StringLink 伴生项。
			// 旧实现无条件解引用 nextItem，对此类 token 抛 NullReferenceException（115 归档大面积触发）。
			// 文本形式取 `{9=<Data>}`：9 即 StringLinkIndex 的经典标签号，payload 是经典视图域的
			// 虚拟串表 ID；编译器按 `{n=value}` 原样写回**单个** token，保存时可逐 token 还原，
			// 不会像 <id::name`text`> 形式那样额外生成伴生 StringLink token。
			if (nextItem == null)
			{
				return $"{{{(int)ScriptType.StringLinkIndex}={Data}}}";
			}
			string text = GetStringTableValue(pvf, nextItem.Data);
			if (AppSetting.Instance.PvfConfig.AutoConvertStringLink)
			{
				return "`" + pvf.Strview.GetStrText(Data, text, autoConvertStr: true).Replace("\\n", "\r\n") + "`";
			}
			return $"<{Data}::{text}`{pvf.Strview.GetStrText(Data, text, autoConvertStr: true)}`>";
		}
		default:
			throw new ArgumentOutOfRangeException();
		case ScriptType.StringLink:
			return "";
		}
	}

	public string GetItemTextNotChar(PvfPack pvf, ScriptItem? nextItem = null)
	{
		switch (Type)
		{
		case ScriptType.Int:
			return Data.ToString();
		case ScriptType.Float:
			return DataHelper.FormatFloat(BitConverter.ToSingle(BitConverter.GetBytes(Data), 0));
		case ScriptType.IntEx:
			return $"{{{(int)Type}={Data}}}";
		case ScriptType.Section:
			return GetStringTableValue(pvf, Data);
		case ScriptType.String:
			return GetStringTableValue(pvf, Data) ?? "";
		case ScriptType.Command:
		case ScriptType.CommandSeparator:
			return $"{{{(int)Type}=`{GetStringTableValue(pvf, Data)}`}}";
		case ScriptType.StringLinkIndex:
		{
			// 同上：1 token 版式（无伴生项）必须显式处理，不得解引用 nextItem。
			if (nextItem == null)
			{
				return $"{{{(int)ScriptType.StringLinkIndex}={Data}}}";
			}
			string strname = GetStringTableValue(pvf, nextItem.Data);
			if (AppSetting.Instance.PvfConfig.AutoConvertStringLink)
			{
				return pvf.Strview.GetStrText(Data, strname, autoConvertStr: true).Replace("\\n", "\r\n") ?? "";
			}
			return pvf.Strview.GetStrText(Data, strname, autoConvertStr: true);
		}
		default:
			throw new ArgumentOutOfRangeException();
		case ScriptType.StringLink:
			return string.Empty;
		}
	}

	private string GetStringTableValue(PvfPack pvf, int index)
	{
		return pvf.Strtable.GetStringItem(index, autoConvertStr: true);
	}

	public ScriptItem()
	{
	}
}
