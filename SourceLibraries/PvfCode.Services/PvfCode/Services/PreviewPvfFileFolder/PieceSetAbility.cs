using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using DevExpress.Mvvm.Native;
using PvfCode.Services.PvfParsingNew;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class PieceSetAbility
{
	[CompilerGenerated]
	private int obN8k1d6jZ;

	[CompilerGenerated]
	private string CT78fxwfLZ;

	[CompilerGenerated]
	private List<SkillDataUp> RMZ87gskht;

	[CompilerGenerated]
	private string? LlJ8TQEDef;

	[CompilerGenerated]
	private string? xZG8Znxgmd;

	public int Number
	{
		[CompilerGenerated]
		get
		{
			return obN8k1d6jZ;
		}
		[CompilerGenerated]
		set
		{
			obN8k1d6jZ = value;
		}
	}

	public string parameter_basic_explain
	{
		[CompilerGenerated]
		get
		{
			return CT78fxwfLZ;
		}
		[CompilerGenerated]
		set
		{
			CT78fxwfLZ = value;
		}
	}

	public List<SkillDataUp> SkillDataUpItems
	{
		[CompilerGenerated]
		get
		{
			return RMZ87gskht;
		}
		[CompilerGenerated]
		set
		{
			RMZ87gskht = value;
		}
	}

	public string? EquWhiteAttributes
	{
		[CompilerGenerated]
		get
		{
			return LlJ8TQEDef;
		}
		[CompilerGenerated]
		set
		{
			LlJ8TQEDef = value;
		}
	}

	public string? EquBlueAttributes
	{
		[CompilerGenerated]
		get
		{
			return xZG8Znxgmd;
		}
		[CompilerGenerated]
		set
		{
			xZG8Znxgmd = value;
		}
	}

	public string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder2);
			handler.AppendLiteral("[");
			handler.AppendFormatted(Number);
			handler.AppendLiteral("]套效果");
			stringBuilder2.AppendLine(ref handler);
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
			Ou08bXn9vd((PvfSection)sectionBase2, pvf);
		}
		EquWhiteAttributes = PvfFilePreviewHelper.GetEquWhiteAttributes(scriptFileParserNew, section, pvf);
		EquBlueAttributes = PvfFilePreviewHelper.EquBlueAttributes(scriptFileParserNew, section, pvf);
	}

	private void Ou08bXn9vd(PvfSection P_0, PvfGroup P_1)
	{
		SkillDataUpItems = new List<SkillDataUp>();
		int num = (P_0.HasEndSection() ? (P_0.Children.Count - 2) : (P_0.Children.Count - 1));
		if (num % 7 != 0)
		{
			return;
		}
		num /= 7;
		List<SectionBase> children = P_0.Children;
		for (int i = 1; i < num; i += 7)
		{
			List<SectionBase> range = children.GetRange(i, 7);
			if (nUW8VbNt1K(range))
			{
				SkillDataUp item = new SkillDataUp
				{
					JobDefaultTypeStr = P_1.Strtable.GetStringItem(range[0].Item.Data),
					JobTypeIndex = range[1].Item.Data,
					DungeonType = P_1.Strtable.GetStringItem(range[2].Item.Data),
					SkillStyle = P_1.Strtable.GetStringItem(range[3].Item.Data),
					SkillStyleValue = range[4].Item.Data,
					SkillAddType = P_1.Strtable.GetStringItem(range[5].Item.Data),
					SkillAddValue = range[6].Item.Data
				};
				SkillDataUpItems.Add(item);
			}
		}
	}

	private bool nUW8VbNt1K(List<SectionBase> P_0)
	{
		if (P_0[0].Item.Type != ScriptType.String)
		{
			return false;
		}
		if (P_0[1].Item.Type != ScriptType.Int)
		{
			return false;
		}
		if (P_0[2].Item.Type != ScriptType.String)
		{
			return false;
		}
		if (P_0[3].Item.Type != ScriptType.String)
		{
			return false;
		}
		if (P_0[4].Item.Type != ScriptType.Int)
		{
			return false;
		}
		if (P_0[5].Item.Type != ScriptType.String)
		{
			return false;
		}
		if (P_0[6].Item.Type != ScriptType.Int)
		{
			return false;
		}
		return true;
	}
}
