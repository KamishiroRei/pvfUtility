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
