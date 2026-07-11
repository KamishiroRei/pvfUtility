using System;
using System.Collections.Generic;
using System.Text;
using PvfCode;
using PvfCode.Services.PvfParsingNew;

namespace PvfCode.Services.PvfParsingNew.TableFormatters;

internal abstract class TableRowFormatterBase : ICloneable
{
	protected int ColumnIndex { get; set; }

	public string? PathPrefix { get; }

	protected TableRowFormatterBase(string? pathPrefix)
	{
		PathPrefix = pathPrefix;
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public abstract void AppendItem(StringBuilder output, List<SectionBase> sections, SectionBase section, PvfGroup group, int index, out int updatedIndex);

	protected virtual void AppendSeparator(StringBuilder output, bool endOfRow)
	{
		if (endOfRow)
		{
			if (AppSetting.Instance.EditConfig.AddEndTab)
			{
				output.Append("\t");
			}
			output.Append(Environment.NewLine);
		}
		else
		{
			output.Append("\t");
		}
	}
}
