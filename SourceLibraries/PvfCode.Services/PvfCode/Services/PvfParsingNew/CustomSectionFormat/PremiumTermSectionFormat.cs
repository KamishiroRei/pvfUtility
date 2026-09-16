using System;
using System.Collections.Generic;
using System.Text;
using PvfCode;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.PvfParsingNew.CustomSectionFormat;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

internal class PremiumTermSectionFormat : CustomSectionFormatBase
{
	public override string ProcessSectionText(PvfGroup P_0, PvfFile P_1, SectionBase P_2, int P_3, Dictionary<string, List<CustomSectionFormatBase>>? dic = null, string? ParentSectionName = null)
	{
		if (ParentSectionName != "[item]")
		{
			P_3 = 1;
		}
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
		int num2 = num - 1;
		for (int j = 1; j < num; j++)
		{
			SectionBase sectionBase = P_2.Children[j];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, P_1, P_0, sectionBase.GetSectionName(), ParentSectionName);
				stringBuilder.Append(config.ProcessSectionText(P_0, P_1, sectionBase, P_3 + 1, dic, sectionName));
				if (j < num2 && !(P_2.Children[j + 1] is PvfSection))
				{
					stringBuilder.Append(Environment.NewLine);
					AddEndTab(stringBuilder, num2, j);
				}
				continue;
			}
			ScriptItem scriptItem = ScriptLinkText.TryGetLinkedLiteral(P_2.Children, j, P_2.Children.Count);
			if (scriptItem != null)
			{
				j++;
			}
			string itemText = sectionBase.Item.GetItemText(P_0, scriptItem);
			base.AppNewLine = false;
			if (j + 1 < num && P_2.Children[j + 1].Item != null)
			{
				ScriptType type = P_2.Children[j + 1].Item.Type;
				if (type != ScriptType.Int && type != ScriptType.Float)
				{
					base.AppNewLine = true;
				}
			}
			if (j == 1)
			{
				stringBuilder.Append(Environment.NewLine);
				for (int k = 0; k < P_3; k++)
				{
					stringBuilder.Append("\t");
				}
				stringBuilder.Append(itemText);
				AppEndTabNew(stringBuilder, num2, j, P_3);
			}
			else if (sectionBase.Item.Type == ScriptType.Int || sectionBase.Item.Type == ScriptType.Float)
			{
				stringBuilder.Append(itemText);
				AppEndTabNew(stringBuilder, num2, j, P_3);
			}
			else
			{
				stringBuilder.Append(Environment.NewLine);
				for (int l = 0; l < P_3; l++)
				{
					stringBuilder.Append("\t");
				}
				stringBuilder.Append(itemText);
				AppEndTabNew(stringBuilder, num2, j, P_3);
			}
		}
		if (P_2.HasEndSection())
		{
			stringBuilder.Append(Environment.NewLine);
			for (int m = 0; m < P_3 - 1; m++)
			{
				stringBuilder.Append("\t");
			}
			stringBuilder.Append((P_2.Children[num].GetType() == typeof(PvfSection)) ? ProcessSectionText(P_0, P_1, P_2.Children[num], P_3 + 1) : P_2.Children[num].Item.GetItemText(P_0, P_2.Children[num].Item));
		}
		return stringBuilder.ToString();
	}

	public override void AppEndTabNew(StringBuilder P_0, int P_1, int P_2, int P_3)
	{
		if (base.AppNewLine || P_1 == P_2)
		{
			if (AppSetting.Instance.EditConfig.AddEndTab)
			{
				P_0.Append("\t");
			}
		}
		else
		{
			P_0.Append("\t");
		}
		base.AppNewLine = false;
	}

	public PremiumTermSectionFormat()
	{
	}
}
