using System.Collections.Generic;
using System.Linq;

namespace PvfCode.Services.PvfParsingNew;

public class SectionBase
{
	public ScriptItem Item { get; set; }

	public List<SectionBase> Children { get; set; }

	public SectionBase(ScriptItem item)
	{
		Item = item;
	}

	public SectionBase()
	{
	}

	public virtual bool HasEndSection()
	{
		return false;
	}

	public virtual string GetSectionName()
	{
		return null;
	}

	public virtual void SetSectionName(string sectionName)
	{
	}

	public virtual bool HasIsRootSection()
	{
		return false;
	}

	public virtual int GetValueCount()
	{
		return 0;
	}

	public virtual List<string> GetValues(PvfGroup pvf)
	{
		return new List<string>();
	}

	public bool HasChildrenSection()
	{
		if (Children == null || Children.Count == 0)
		{
			return false;
		}
		return Children.Any((SectionBase it) => it is PvfSection);
	}
}
