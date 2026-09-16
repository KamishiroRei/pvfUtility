using System.Collections.Generic;
using System.Text;
using PvfCode;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.PvfParsingNew.TableFormatters;

namespace PvfCode.Services.PvfParsingNew.TableFormatters;

internal class FixedColumnTableRowFormatter : TableRowFormatterBase
{
	public int GroupLength { get; }

	public FixedColumnTableRowFormatter(int groupLength, string? pathPrefix)
		: base(pathPrefix)
	{
		GroupLength = groupLength;
	}

	public override void AppendItem(StringBuilder output, List<SectionBase> sections, SectionBase section, PvfGroup group, int index, out int updatedIndex)
	{
		ScriptItem linkedString = section.Item.Type == ScriptType.StringLinkIndex && section.Children != null && index + 1 < section.Children.Count ? section.Children[index + 1].Item : null;
		if (linkedString != null)
		{
			index++;
		}
		string itemText = section.Item.GetItemText(group, linkedString);
		output.Append(itemText);
		ColumnIndex++;
		bool endOfRow = ColumnIndex == GroupLength;
		if (endOfRow)
		{
			ColumnIndex = 0;
		}
		AppendSeparator(output, endOfRow);
		updatedIndex = index;
	}
}
