using System;
using System.Collections.Generic;
using System.Text;
using PvfCode;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.PvfParsingNew.CustomSectionFormat;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

internal class EquipmentLotteryListSectionFormat : CustomSectionFormatBase
{
	public override string ProcessSectionText(PvfGroup P_0, PvfFile P_1, SectionBase P_2, int P_3, Dictionary<string, List<CustomSectionFormatBase>>? dic = null, string? ParentSectionName = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = P_2.Children.Count;
		if (P_2.HasEndSection())
		{
			num--;
		}
		stringBuilder.Append(Environment.NewLine);
		for (int i = 0; i < P_3 - 1; i++)
		{
			stringBuilder.Append("\t");
		}
		string sectionName = P_2.GetSectionName();
		stringBuilder.Append(sectionName);
		stringBuilder.Append(Environment.NewLine);
		int count = num - 1;
		for (int j = 0; j < P_3; j++)
		{
			stringBuilder.Append("\t");
		}
		int num2 = 0;
		int num3 = 6;
		bool flag = false;
		bool flag2 = false;
		for (int k = 1; k < num; k++)
		{
			SectionBase sectionBase = P_2.Children[k];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, P_1, P_0, sectionBase.GetSectionName(), ParentSectionName);
				stringBuilder.Append(config.ProcessSectionText(P_0, P_1, sectionBase, 1, dic, ParentSectionName));
				continue;
			}
			ScriptItem nextItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? P_2.Children[k + 1].Item : null);
			string itemText = sectionBase.Item.GetItemText(P_0, nextItem);
			stringBuilder.Append(itemText);
			num2++;
			if (num2 > 4)
			{
				if (num2 == 5)
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
							num3 = 6;
						}
					}
					else if (flag2)
					{
						flag2 = false;
						num2 = 0;
						base.AppNewLine = true;
						num3 = 6;
					}
				}
				AppEndTabNew(stringBuilder, count, k, P_3);
			}
			else
			{
				AppEndTabNew(stringBuilder, count, k, P_3);
			}
		}
		if (P_2.HasEndSection())
		{
			stringBuilder.Append(Environment.NewLine);
			for (int l = 0; l < P_3 - 1; l++)
			{
				stringBuilder.Append("\t");
			}
			stringBuilder.Append((P_2.Children[num].GetType() == typeof(PvfSection)) ? ProcessSectionText(P_0, P_1, P_2.Children[num], P_3 + 1) : P_2.Children[num].Item.GetItemText(P_0, P_2.Children[num].Item));
		}
		return stringBuilder.ToString();
	}

	public EquipmentLotteryListSectionFormat()
	{
	}
}
