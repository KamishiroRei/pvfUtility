using System.Collections.Generic;

namespace PvfCode.Services.PvfParsingNew;

public class PvfSection : SectionBase
{
	public string SectionName { get; set; }

	public bool HasEndTag { get; set; }

	public bool IsRootSection { get; set; }

	public List<SectionBase> Items { get; set; }

	public PvfSection(string sectionName, bool hasEndTag, bool isRootSection = false)
	{
		SectionName = sectionName;
		HasEndTag = hasEndTag;
		Items = new List<SectionBase>();
		base.Children = new List<SectionBase>();
		IsRootSection = isRootSection;
	}

	public override bool HasEndSection()
	{
		return HasEndTag;
	}

	public override string GetSectionName()
	{
		return SectionName;
	}

	public override void SetSectionName(string sectionName)
	{
		SectionName = sectionName;
	}

	public override bool HasIsRootSection()
	{
		return IsRootSection;
	}

	public override int GetValueCount()
	{
		if (base.Children == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 1; i < base.Children.Count; i++)
		{
			SectionBase sectionBase = base.Children[i];
			if (!(sectionBase is PvfSection))
			{
				if (ScriptLinkText.TryGetLinkedLiteral(base.Children, i, base.Children.Count) != null)
				{
					i++;
				}
				num++;
			}
		}
		return num;
	}

	public override List<string> GetValues(PvfGroup pvf)
	{
		List<string> list = new List<string>();
		for (int i = 1; i < base.Children.Count; i++)
		{
			SectionBase sectionBase = base.Children[i];
			if (!(sectionBase is PvfSection))
			{
				ScriptItem scriptItem = ScriptLinkText.TryGetLinkedLiteral(base.Children, i, base.Children.Count);
				if (scriptItem != null)
				{
					i++;
				}
				list.Add(sectionBase.Item.GetItemText(pvf, scriptItem));
			}
		}
		return list;
	}
}
