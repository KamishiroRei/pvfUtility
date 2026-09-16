using System;
using System.Collections.Generic;
using System.Text;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

public class CustomSectionFormat_qst_reward__selection_int_data : CustomSectionFormatBase
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
		int num3 = 0;
		for (int k = 1; k < num; k++)
		{
			SectionBase sectionBase = section.Children[k];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, file, pvf, sectionBase.GetSectionName(), ParentSectionName);
				stringBuilder.Append(config.ProcessSectionText(pvf, file, sectionBase, 1, dic, ParentSectionName));
				continue;
			}
			ScriptItem scriptItem = ScriptLinkText.TryGetLinkedLiteral(section.Children, k, section.Children.Count);
			string itemText = sectionBase.Item.GetItemText(pvf, scriptItem);
			if (scriptItem != null)
			{
				k++;
			}
			num2++;
			stringBuilder.Append(itemText);
			if (num2 == 2)
			{
				if (sectionBase.Item.Type != ScriptType.String)
				{
					base.AppNewLine = true;
					num3 = 0;
					num2 = 0;
					AppEndTabNew(stringBuilder, count, k, layer);
					continue;
				}
				num3 = 5;
			}
			if (num3 > 0 && num2 == num3)
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

	public CustomSectionFormat_qst_reward__selection_int_data()
	{
	}
}
