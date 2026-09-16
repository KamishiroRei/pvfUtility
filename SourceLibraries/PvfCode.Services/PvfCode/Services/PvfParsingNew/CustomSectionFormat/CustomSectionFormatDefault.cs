using System;
using System.Collections.Generic;
using System.Text;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

public class CustomSectionFormatDefault : CustomSectionFormatBase
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
		int num2 = num - 1;
		for (int j = 1; j < num; j++)
		{
			SectionBase sectionBase = section.Children[j];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, file, pvf, sectionBase.GetSectionName(), ParentSectionName);
				stringBuilder.Append(config.ProcessSectionText(pvf, file, sectionBase, layer + 1, dic, sectionName));
				if (j < num2 && !(section.Children[j + 1] is PvfSection))
				{
					stringBuilder.Append(Environment.NewLine);
					AddEndTab(stringBuilder, num2, j);
				}
				continue;
			}
			ScriptItem scriptItem = ScriptLinkText.TryGetLinkedLiteral(section.Children, j, section.Children.Count);
			if (scriptItem != null)
			{
				j++;
			}
			string itemText = sectionBase.Item.GetItemText(pvf, scriptItem);
			base.AppNewLine = false;
			if (j + 1 < num && section.Children[j + 1].Item != null)
			{
				ScriptType type = section.Children[j + 1].Item.Type;
				if (type != ScriptType.Int && type != ScriptType.Float)
				{
					base.AppNewLine = true;
				}
			}
			if (j == 1)
			{
				stringBuilder.Append(Environment.NewLine);
				for (int k = 0; k < layer; k++)
				{
					stringBuilder.Append("\t");
				}
				stringBuilder.Append(itemText);
				AppEndTabNew(stringBuilder, num2, j, layer);
			}
			else if (sectionBase.Item.Type == ScriptType.Int || sectionBase.Item.Type == ScriptType.Float)
			{
				stringBuilder.Append(itemText);
				AppEndTabNew(stringBuilder, num2, j, layer);
			}
			else
			{
				stringBuilder.Append(Environment.NewLine);
				for (int l = 0; l < layer; l++)
				{
					stringBuilder.Append("\t");
				}
				stringBuilder.Append(itemText);
				AppEndTabNew(stringBuilder, num2, j, layer);
			}
		}
		if (section.HasEndSection())
		{
			stringBuilder.Append(Environment.NewLine);
			for (int m = 0; m < layer - 1; m++)
			{
				stringBuilder.Append("\t");
			}
			stringBuilder.Append((section.Children[num].GetType() == typeof(PvfSection)) ? ProcessSectionText(pvf, file, section.Children[num], layer + 1) : section.Children[num].Item.GetItemText(pvf, section.Children[num].Item));
		}
		return stringBuilder.ToString();
	}

	public override void AppEndTabNew(StringBuilder sb, int count, int index, int layer)
	{
		if (base.AppNewLine || count == index)
		{
			if (AppSetting.Instance.EditConfig.AddEndTab)
			{
				sb.Append("\t");
			}
		}
		else
		{
			sb.Append("\t");
		}
		base.AppNewLine = false;
	}

	public CustomSectionFormatDefault()
	{
	}
}
