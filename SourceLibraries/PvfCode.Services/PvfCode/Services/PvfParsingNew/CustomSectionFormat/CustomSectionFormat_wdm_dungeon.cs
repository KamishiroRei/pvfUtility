using System;
using System.Collections.Generic;
using System.Text;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

public class CustomSectionFormat_wdm_dungeon : CustomSectionFormatBase
{
	public override string ProcessSectionText(PvfGroup pvf, PvfFile file, SectionBase section, int layer, Dictionary<string, List<CustomSectionFormatBase>>? dic = null, string? ParentSectionName = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = section.Children.Count;
		if (section.HasEndSection())
		{
			num--;
		}
		stringBuilder.Append(Environment.NewLine);
		for (int i = 0; i < layer - 1; i++)
		{
			stringBuilder.Append("\t");
		}
		string sectionName = section.GetSectionName();
		stringBuilder.Append(sectionName);
		stringBuilder.Append(Environment.NewLine);
		for (int j = 0; j < layer; j++)
		{
			stringBuilder.Append("\t");
		}
		int num2 = 0;
		int count = num - 1;
		for (int k = 1; k < num; k++)
		{
			SectionBase sectionBase = section.Children[k];
			if (sectionBase is PvfSection)
			{
				AppendNestedSection(pvf, sectionBase, stringBuilder);
				continue;
			}
			ScriptItem scriptItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[k + 1].Item : null);
			string itemText = sectionBase.Item.GetItemText(pvf, scriptItem);
			stringBuilder.Append(itemText);
			num2++;
			if (scriptItem != null && scriptItem.Type == ScriptType.Section)
			{
				base.AppNewLine = true;
				num2 = 0;
			}
			if (num2 == 2)
			{
				base.AppNewLine = true;
				num2 = 0;
			}
			AppEndTabNew(stringBuilder, count, k, layer);
		}
		if (section.HasEndSection())
		{
			stringBuilder.Append(Environment.NewLine);
			for (int l = 0; l < layer - 1; l++)
			{
				stringBuilder.Append("\t");
			}
			stringBuilder.Append((section.Children[num].GetType() == typeof(PvfSection)) ? ProcessSectionText(pvf, file, section.Children[num], layer + 1) : section.Children[num].Item.GetItemText(pvf, section.Children[num].Item));
		}
		return stringBuilder.ToString();
	}

	private void AppendNestedSection(PvfGroup pvf, SectionBase section, StringBuilder output)
	{
		int count = section.Children.Count;
		if (output.Length > 0 && output[output.Length - 1] != '\t')
		{
			output.Append("\t");
		}
		output.Append(section.GetSectionName());
		output.Append("\t");
		int count2 = count - 1;
		for (int i = 1; i < count; i++)
		{
			SectionBase sectionBase = section.Children[i];
			ScriptItem nextItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[i + 1].Item : null);
			string itemText = sectionBase.Item.GetItemText(pvf, nextItem);
			if (i == 2)
			{
				output.Append(Environment.NewLine);
				output.Append("\t");
				output.Append(itemText);
			}
			else
			{
				output.Append(itemText);
			}
			AppEndTabNew(output, count2, i, 0);
		}
	}

	public CustomSectionFormat_wdm_dungeon()
	{
	}
}
