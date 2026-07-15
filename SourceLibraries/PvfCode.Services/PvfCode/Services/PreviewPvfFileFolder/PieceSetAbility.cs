using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Mvvm.Native;
using PvfCode.Services.PvfParsingNew;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class PieceSetAbility
{
	public int Number { get; set; }

	public string parameter_basic_explain { get; set; }

	public List<SkillDataUp> SkillDataUpItems { get; set; }

	public string? EquWhiteAttributes { get; set; }

	public string? EquBlueAttributes { get; set; }

	public string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"[{Number}]套效果");
			if (EquWhiteAttributes != null)
			{
				stringBuilder.Append(EquWhiteAttributes);
			}
			if (EquBlueAttributes != null)
			{
				stringBuilder.Append(EquBlueAttributes);
			}
			if (!string.IsNullOrEmpty(parameter_basic_explain))
			{
				stringBuilder.AppendLine(parameter_basic_explain);
			}
			return stringBuilder.ToString();
		}
	}

	public PieceSetAbility(ScriptFileParserNew scriptFileParserNew, List<SectionBase> section, PvfGroup pvf)
	{
		SectionBase sectionBase = section.Where((SectionBase it) => it != null && it.Item.Type == ScriptType.Int).FirstOrDefault();
		if (sectionBase != null)
		{
			Number = sectionBase.Item.Data;
		}
		int num = section.IndexOf((SectionBase it) => it is PvfSection && it.GetSectionName() == "[parameter basic explain]");
		if (num != -1)
		{
			List<SectionBase> children = section[num].Children;
			if (children.Count >= 2)
			{
				ScriptItem nextItem = ((children[1].Item.Type == ScriptType.StringLinkIndex) ? children[2].Item : null);
				parameter_basic_explain = children[1].Item.GetItemTextNotChar(pvf, nextItem);
			}
		}
		SectionBase sectionBase2 = section.Where((SectionBase it) => it is PvfSection && it.GetSectionName() == "[skill data up]").FirstOrDefault();
		if (sectionBase2 != null)
		{
			LoadSkillData((PvfSection)sectionBase2, pvf);
		}
		EquWhiteAttributes = PvfFilePreviewHelper.GetEquWhiteAttributes(scriptFileParserNew, section, pvf);
		EquBlueAttributes = PvfFilePreviewHelper.EquBlueAttributes(scriptFileParserNew, section, pvf);
	}

	private void LoadSkillData(PvfSection section, PvfGroup pvf)
	{
		SkillDataUpItems = new List<SkillDataUp>();
		int num = (section.HasEndSection() ? (section.Children.Count - 2) : (section.Children.Count - 1));
		if (num % 7 != 0)
		{
			return;
		}
		num /= 7;
		List<SectionBase> children = section.Children;
		for (int i = 1; i < num; i += 7)
		{
			List<SectionBase> range = children.GetRange(i, 7);
			if (IsSkillDataRange(range))
			{
				SkillDataUp item = new SkillDataUp
				{
					JobDefaultTypeStr = pvf.Strtable.GetStringItem(range[0].Item.Data),
					JobTypeIndex = range[1].Item.Data,
					DungeonType = pvf.Strtable.GetStringItem(range[2].Item.Data),
					SkillStyle = pvf.Strtable.GetStringItem(range[3].Item.Data),
					SkillStyleValue = range[4].Item.Data,
					SkillAddType = pvf.Strtable.GetStringItem(range[5].Item.Data),
					SkillAddValue = range[6].Item.Data
				};
				SkillDataUpItems.Add(item);
			}
		}
	}

	private bool IsSkillDataRange(List<SectionBase> items)
	{
		if (items[0].Item.Type != ScriptType.String)
		{
			return false;
		}
		if (items[1].Item.Type != ScriptType.Int)
		{
			return false;
		}
		if (items[2].Item.Type != ScriptType.String)
		{
			return false;
		}
		if (items[3].Item.Type != ScriptType.String)
		{
			return false;
		}
		if (items[4].Item.Type != ScriptType.Int)
		{
			return false;
		}
		if (items[5].Item.Type != ScriptType.String)
		{
			return false;
		}
		if (items[6].Item.Type != ScriptType.Int)
		{
			return false;
		}
		return true;
	}
}
