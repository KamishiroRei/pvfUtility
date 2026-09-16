using System;
using System.Collections.Generic;
using System.Text;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

public class CustomSectionFormat_Worlddrop : CustomSectionFormatBase
{
	public int MaxLen { get; set; }

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
		bool flag = false;
		int count = num - 1;
		for (int k = 1; k < num; k++)
		{
			SectionBase sectionBase = section.Children[k];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, file, pvf, sectionBase.GetSectionName(), ParentSectionName);
				stringBuilder.Append(config.ProcessSectionText(pvf, file, sectionBase, 1, dic, ParentSectionName));
				continue;
			}
			ScriptItem nextItem = ScriptLinkText.TryGetLinkedLiteral(section.Children, k, section.Children.Count);
			string itemText = sectionBase.Item.GetItemText(pvf, nextItem);
			stringBuilder.Append(itemText);
			num2++;
			if (flag)
			{
				if (num2 == MaxLen)
				{
					num2 = 0;
					base.AppNewLine = true;
				}
				if (int.TryParse(itemText, out var result) && result == -1)
				{
					num2 = 0;
					base.AppNewLine = true;
				}
			}
			else if (num2 == MaxLen)
			{
				flag = true;
				num2 = 0;
				base.AppNewLine = true;
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

	public CustomSectionFormat_Worlddrop()
	{
	}
}
