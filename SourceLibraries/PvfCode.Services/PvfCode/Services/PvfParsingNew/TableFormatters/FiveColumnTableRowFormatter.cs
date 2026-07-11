using System.Collections.Generic;
using System.Text;
using PvfCode;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.PvfParsingNew.TableFormatters;

namespace PvfCode.Services.PvfParsingNew.TableFormatters;

internal class FiveColumnTableRowFormatter : TableRowFormatterBase
{
	public FiveColumnTableRowFormatter(string? pathPrefix)
		: base(pathPrefix)
	{
	}

	public override void AppendItem(StringBuilder output, List<SectionBase> sections, SectionBase section, PvfGroup group, int index, out int updatedIndex)
	{
		ScriptItem linkedString = section.Item.Type == ScriptType.StringLinkIndex ? section.Children[index + 1].Item : null;
		if (linkedString != null)
		{
			index++;
		}
		string itemText = section.Item.GetItemText(group, linkedString);
		output.Append(itemText);
		ColumnIndex++;
		bool endOfRow = ColumnIndex == 5;
		if (endOfRow)
		{
			ColumnIndex = 0;
		}
		AppendSeparator(output, endOfRow);
		updatedIndex = index;
	}
}
