using System;
using System.Collections.Generic;
using System.Text;
using PvfCode;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.PvfParsingNew.CustomSectionFormat;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

internal class AvatarSelectAbilitySectionFormat : CustomSectionFormatBase
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
		for (int j = 0; j < P_3; j++)
		{
			stringBuilder.Append("\t");
		}
		int num2 = 0;
		int num3 = 0;
		int count = num - 1;
		for (int k = 1; k < num; k++)
		{
			SectionBase sectionBase = P_2.Children[k];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, P_1, P_0, sectionBase.GetSectionName(), ParentSectionName);
				stringBuilder.Append(config.ProcessSectionText(P_0, P_1, sectionBase, 1, dic, ParentSectionName));
				continue;
			}
			ScriptItem scriptItem = ScriptLinkText.TryGetLinkedLiteral(P_2.Children, k, P_2.Children.Count);
			if (scriptItem != null)
			{
				k++;
			}
			string itemText = sectionBase.Item.GetItemText(P_0, scriptItem);
			stringBuilder.Append(itemText);
			num2++;
			if (num2 == 1)
			{
				num3 = ((itemText == "`[SKILL_LEVEL]`") ? 4 : 3);
			}
			if (num2 == num3)
			{
				base.AppNewLine = true;
				num2 = 0;
			}
			AppEndTabNew(stringBuilder, count, k, P_3);
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

	public AvatarSelectAbilitySectionFormat()
	{
	}
}
