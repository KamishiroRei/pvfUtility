using System.Collections.Generic;
using System.Text;
using PvfCode;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.PvfParsingNew.TableFormatters;

namespace PvfCode.Services.PvfParsingNew.TableFormatters;

internal class ItemDictionaryTableRowFormatter : TableRowFormatterBase
{
	private readonly List<string> itemCategories;

	private int itemCode;

	public ItemDictionaryTableRowFormatter(string? pathPrefix)
		: base(pathPrefix)
	{
		itemCategories = new List<string>
		{
			"equipment",
			"stackable"
		};
		itemCode = -1;
	}

	public override void AppendItem(StringBuilder output, List<SectionBase> sections, SectionBase section, PvfGroup group, int index, out int updatedIndex)
	{
		ScriptItem linkedString = section.Item.Type == ScriptType.StringLinkIndex && section.Children != null && index + 1 < section.Children.Count ? section.Children[index + 1].Item : null;
		if (linkedString != null)
		{
			index++;
		}
		string text = section.Item.GetItemText(group, linkedString);
		ColumnIndex++;
		if (ColumnIndex == 1 && !int.TryParse(text, out itemCode))
		{
			itemCode = -1;
		}
		bool endOfRow = false;
		ScriptType type = section.Item.Type;
		if (type == ScriptType.String || (uint)(type - 9) <= 1u)
		{
			ColumnIndex = 0;
			endOfRow = true;
			if (itemCode == -1)
			{
				text = "`" + AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_StringLinkFirstColumnError") + "`";
			}
			else
			{
				string itemPath = group.ListFileTable.ItemCodeConvertFilePath(itemCategories, itemCode);
				text = itemPath != null
					? "`" + group.GetItemName(itemPath) + "`"
					: "`" + string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_CannotFindCode"), itemCode) + "`";
			}
		}
		output.Append(text);
		AppendSeparator(output, endOfRow);
		updatedIndex = index;
	}
}
