using System;
using System.Collections.Generic;
using System.Text;
using PvfCode;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.PvfParsingNew.CustomSectionFormat;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

internal class SpecialPassiveObjectSectionFormat : CustomSectionFormatBase
{
	private enum HellPartyFormatPhase
	{
		Header,
		SectionMarker,
		ThreeColumnRows,
		EndMarker,
		FiveColumnRows
	}

	public override string ProcessSectionText(PvfGroup group, PvfFile file, SectionBase section, int layer, Dictionary<string, List<CustomSectionFormatBase>>? dic = null, string? parentSectionName = null)
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
		if (section.Children != null && section.Children.Count > 7 && section.Children[6].Item.GetItemText(group, section.Children[6].Item) == "`[hellparty]`")
		{
			return FormatHellPartySection(group, file, section, layer, dic, parentSectionName, stringBuilder);
		}
		int num2 = 0;
		int num3 = num - 1;
		bool flag = true;
		int num4 = 5;
		int num5 = 0;
		for (int k = 1; k < num; k++)
		{
			SectionBase sectionBase = section.Children[k];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, file, group, sectionBase.GetSectionName(), parentSectionName);
				stringBuilder.Append(config.ProcessSectionText(group, file, sectionBase, layer + 1, dic, sectionName));
				if (k < num3 && !(section.Children[k + 1] is PvfSection))
				{
					stringBuilder.Append(Environment.NewLine);
					AddEndTab(stringBuilder, num3, k);
				}
				continue;
			}
			ScriptItem scriptItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[k + 1].Item : null);
			if (scriptItem != null)
			{
				k++;
			}
			string itemText = sectionBase.Item.GetItemText(group, scriptItem);
			stringBuilder.Append(itemText);
			num2++;
			if (flag)
			{
				if (num2 == 5)
				{
					num4 = sectionBase.Item.Data;
					flag = false;
					base.AppNewLine = true;
					num2 = 0;
					if (num4 == 0)
					{
						flag = true;
					}
				}
				AppEndTabNew(stringBuilder, 5, num2, layer);
				continue;
			}
			if (num2 == 6)
			{
				num2 = 0;
				if (k != num3)
				{
					base.AppNewLine = true;
				}
				num5++;
				if (num5 == num4)
				{
					num5 = 0;
					flag = true;
					num4 = 0;
				}
			}
			AppEndTabNew(stringBuilder, 6, num2, layer);
		}
		if (section.HasEndSection())
		{
			stringBuilder.Append(Environment.NewLine);
			for (int l = 0; l < layer - 1; l++)
			{
				stringBuilder.Append("\t");
			}
			stringBuilder.Append((section.Children[num].GetType() == typeof(PvfSection)) ? ProcessSectionText(group, file, section.Children[num], layer + 1) : section.Children[num].Item.GetItemText(group, section.Children[num].Item));
		}
		return stringBuilder.ToString();
	}

	private string FormatHellPartySection(PvfGroup group, PvfFile file, SectionBase section, int layer, Dictionary<string, List<CustomSectionFormatBase>>? dic, string? parentSectionName, StringBuilder output)
	{
		int num = section.Children.Count;
		if (section.HasEndSection())
		{
			num--;
		}
		int num2 = 0;
		int num3 = num - 1;
		HellPartyFormatPhase phase = HellPartyFormatPhase.Header;
		for (int i = 1; i < num; i++)
		{
			SectionBase sectionBase = section.Children[i];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, file, group, sectionBase.GetSectionName(), parentSectionName);
				output.Append(config.ProcessSectionText(group, file, sectionBase, layer + 1, dic, section.GetSectionName()));
				if (i < num3 && !(section.Children[i + 1] is PvfSection))
				{
					output.Append(Environment.NewLine);
					AddEndTab(output, num3, i);
				}
				continue;
			}
			ScriptItem scriptItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[i + 1].Item : null);
			if (scriptItem != null)
			{
				i++;
			}
			string itemText = sectionBase.Item.GetItemText(group, scriptItem);
			output.Append(itemText);
			num2++;
			if (phase == HellPartyFormatPhase.Header && num2 == 5)
			{
				base.AppNewLine = true;
				num2 = 0;
				AppendNewLineAndIndent(output, layer);
				phase = HellPartyFormatPhase.SectionMarker;
				continue;
			}
			switch (phase)
			{
			case HellPartyFormatPhase.SectionMarker:
				num2 = 0;
				AppendNewLineAndIndent(output, layer);
				phase = HellPartyFormatPhase.ThreeColumnRows;
				break;
			case HellPartyFormatPhase.ThreeColumnRows:
				if (num2 == 3)
				{
					base.AppNewLine = true;
					num2 = 0;
					AppendNewLineAndIndent(output, layer);
				}
				else if (itemText == "`[/hellparty]`")
				{
					base.AppNewLine = true;
					num2 = 0;
					AppendNewLineAndIndent(output, layer);
					phase = HellPartyFormatPhase.FiveColumnRows;
				}
				else
				{
					AddEndTab(output, 3, layer);
				}
				break;
			case HellPartyFormatPhase.EndMarker:
				base.AppNewLine = true;
				num2 = 0;
				AppendNewLineAndIndent(output, layer);
				phase = HellPartyFormatPhase.FiveColumnRows;
				break;
			case HellPartyFormatPhase.FiveColumnRows:
				if (num2 == 5)
				{
					base.AppNewLine = true;
					num2 = 0;
					AppendNewLineAndIndent(output, layer);
				}
				else if (sectionBase.Item.Type == ScriptType.String)
				{
					base.AppNewLine = true;
					num2 = 0;
					AppendNewLineAndIndent(output, layer);
				}
				else
				{
					AddEndTab(output, 5, layer);
				}
				break;
			default:
				AppEndTabNew(output, 0, num2, layer);
				break;
			}
		}
		if (section.HasEndSection())
		{
			output.Append(Environment.NewLine);
			for (int j = 0; j < layer - 1; j++)
			{
				output.Append("\t");
			}
			output.Append((section.Children[num].GetType() == typeof(PvfSection)) ? ProcessSectionText(group, file, section.Children[num], layer + 1) : section.Children[num].Item.GetItemText(group, section.Children[num].Item));
		}
		return output.ToString();
	}

	public SpecialPassiveObjectSectionFormat()
	{
	}
}
