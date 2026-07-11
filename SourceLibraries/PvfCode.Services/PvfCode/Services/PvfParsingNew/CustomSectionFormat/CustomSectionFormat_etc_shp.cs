using System;
using System.Collections.Generic;
using System.Text;
using Utools;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

public class CustomSectionFormat_etc_shp : CustomSectionFormatBase
{
	public int ItemCodeIndex { get; set; }

	public int ItemNameIndex { get; set; }

	public int MaxLen { get; set; }

	public List<string> LstNames { get; set; }

	public CustomSectionFormat_etc_shp(int convertIndex, List<string> lstNames, int maxLen, int itemNameIndex)
	{
		ItemCodeIndex = convertIndex;
		LstNames = lstNames;
		MaxLen = maxLen;
		ItemNameIndex = itemNameIndex;
	}

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
		int itemCode = 0;
		for (int k = 1; k < num; k++)
		{
			SectionBase sectionBase = section.Children[k];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, file, pvf, sectionBase.GetSectionName(), ParentSectionName);
				stringBuilder.Append(config.ProcessSectionText(pvf, file, sectionBase, 1, dic, ParentSectionName));
				continue;
			}
			ScriptItem scriptItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[k + 1].Item : null);
			if (scriptItem != null)
			{
				k++;
			}
			string text = sectionBase.Item.GetItemText(pvf, scriptItem);
			num2++;
			if (num2 == ItemCodeIndex)
			{
				itemCode = text.ToInt();
			}
			if (num2 == ItemNameIndex && sectionBase.Item.Type == ScriptType.String)
			{
				string text2 = pvf.ListFileTable.ItemCodeConvertFilePath(LstNames, itemCode);
				if (text2 != null)
				{
					text = "`" + pvf.GetItemName(text2) + "`";
				}
			}
			stringBuilder.Append(text);
			if (num2 == MaxLen)
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
}
