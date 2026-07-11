using System;
using System.Collections.Generic;
using System.Text;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

public class CustomSectionFormat_map_monster : CustomSectionFormatBase
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
		int num3 = 10;
		for (int k = 1; k < num; k++)
		{
			SectionBase sectionBase = section.Children[k];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, file, pvf, sectionBase.GetSectionName(), ParentSectionName);
				stringBuilder.Append(config.ProcessSectionText(pvf, file, sectionBase, 1, dic, ParentSectionName));
				stringBuilder.Append(Environment.NewLine);
				continue;
			}
			ScriptItem scriptItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[k + 1].Item : null);
			if (scriptItem != null)
			{
				k++;
			}
			string itemText = sectionBase.Item.GetItemText(pvf, scriptItem);
			stringBuilder.Append(itemText);
			num2++;
			if (num2 >= 10)
			{
				if (num2 == 10)
				{
					if (!(itemText == "`[normal]`") && !(itemText == "`[boss]`"))
					{
						if (!(itemText == "`[NPC]`"))
						{
							if ((itemText == "`[champion]`" || itemText == "`[super champion]`") && k + 1 < section.Children.Count)
							{
								int data = section.Children[k + 1].Item.Data;
								num3 += ((data == 0) ? 1 : data);
							}
						}
						else
						{
							num3 = 12;
						}
					}
					else
					{
						base.AppNewLine = true;
						num2 = 0;
						num3 = 10;
					}
				}
				if (num2 == num3)
				{
					base.AppNewLine = true;
					num2 = 0;
					num3 = 10;
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

	public CustomSectionFormat_map_monster()
	{
	}
}
