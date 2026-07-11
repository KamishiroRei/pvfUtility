using System;
using System.Collections.Generic;
using System.Text;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

public class CustomSectionFormat_Stk_Blueprint : CustomSectionFormatBase
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
		int? num3 = null;
		int num4 = 2;
		int num5 = 0;
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
			ScriptItem nextItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[k + 1].Item : null);
			string itemText = sectionBase.Item.GetItemText(pvf, nextItem);
			stringBuilder.Append(itemText);
			if (!num3.HasValue)
			{
				num3 = (int.TryParse(itemText, out var result) ? result : 10);
				num3 *= 2;
				num5 = 0;
				base.AppNewLine = true;
				num2 = 0;
			}
			else
			{
				num2++;
				num5++;
				if (num2 == num4)
				{
					if (num5 == num3)
					{
						num3 = null;
					}
					num2 = 0;
					base.AppNewLine = true;
				}
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

	public CustomSectionFormat_Stk_Blueprint()
	{
	}
}
