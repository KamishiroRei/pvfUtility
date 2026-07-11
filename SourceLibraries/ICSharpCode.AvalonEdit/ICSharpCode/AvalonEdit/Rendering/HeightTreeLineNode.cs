using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Rendering;

internal struct HeightTreeLineNode
{
	internal double height;

	internal List<CollapsedLineSection> collapsedSections;

	internal bool IsDirectlyCollapsed => collapsedSections != null;

	internal double TotalHeight
	{
		get
		{
			if (!IsDirectlyCollapsed)
			{
				return height;
			}
			return 0.0;
		}
	}

	internal HeightTreeLineNode(double height)
	{
		collapsedSections = null;
		this.height = height;
	}

	internal void AddDirectlyCollapsed(CollapsedLineSection section)
	{
		if (collapsedSections == null)
		{
			collapsedSections = new List<CollapsedLineSection>();
		}
		collapsedSections.Add(section);
	}

	internal void RemoveDirectlyCollapsed(CollapsedLineSection section)
	{
		collapsedSections.Remove(section);
		if (collapsedSections.Count == 0)
		{
			collapsedSections = null;
		}
	}
}
