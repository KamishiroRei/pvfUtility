using System;
using System.Collections.Generic;
using System.Text;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

public class CustomSectionFormat_recipelistmakeequip : CustomSectionFormatBase
{
	public string ProcessSectionTextT(PvfGroup pvf, PvfFile file, SectionBase section, int layer, Dictionary<string, List<CustomSectionFormatBase>>? dic = null, string? ParentSectionName = null)
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
		int num3 = 6;
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
			num2++;
			if (num2 > 6)
			{
				if (num2 == 7)
				{
					if (itemText == "0")
					{
						num2 = 0;
						base.AppNewLine = true;
					}
					else
					{
						num3 = 8;
					}
				}
				else if (num2 == num3)
				{
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
		int count = num - 1;
		for (int j = 0; j < layer; j++)
		{
			stringBuilder.Append("\t");
		}
		int num2 = 0;
		int num3 = 7;
		bool flag = false;
		bool flag2 = false;
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
			num2++;
			if (num2 > 5)
			{
				if (num2 == 6)
				{
					int result;
					int num4 = (int.TryParse(itemText, out result) ? result : 0);
					num3 += ((num4 != 0) ? num4 : 0);
					flag = true;
				}
				if (num2 == num3)
				{
					if (flag)
					{
						flag = false;
						flag2 = true;
						int result2;
						int num5 = (int.TryParse(itemText, out result2) ? result2 : 0);
						num3 += num5;
						if (num3 == num2)
						{
							base.AppNewLine = true;
							flag2 = false;
							num2 = 0;
							num3 = 7;
						}
					}
					else if (flag2)
					{
						flag2 = false;
						num2 = 0;
						base.AppNewLine = true;
						num3 = 7;
					}
				}
				AppEndTabNew(stringBuilder, count, k, layer);
			}
			else
			{
				AppEndTabNew(stringBuilder, count, k, layer);
			}
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

	public CustomSectionFormat_recipelistmakeequip()
	{
	}
}
