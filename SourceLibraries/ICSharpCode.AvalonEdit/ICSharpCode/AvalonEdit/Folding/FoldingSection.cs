using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Folding;

public sealed class FoldingSection : TextSegment
{
	private readonly FoldingManager manager;

	private bool isFolded;

	internal CollapsedLineSection[] collapsedSections;

	private string title;

	public bool IsFolded
	{
		get
		{
			return isFolded;
		}
		set
		{
			if (isFolded != value)
			{
				isFolded = value;
				ValidateCollapsedLineSections();
				manager.Redraw(this);
			}
		}
	}

	public string Title
	{
		get
		{
			return title;
		}
		set
		{
			if (title != value)
			{
				title = value;
				if (IsFolded)
				{
					manager.Redraw(this);
				}
			}
		}
	}

	public string TextContent => manager.document.GetText(base.StartOffset, base.EndOffset - base.StartOffset);

	public object Tag { get; set; }

	internal void ValidateCollapsedLineSections()
	{
		if (!isFolded)
		{
			RemoveCollapsedLineSection();
			return;
		}
		DocumentLine lineByOffset = manager.document.GetLineByOffset(base.StartOffset.CoerceValue(0, manager.document.TextLength));
		DocumentLine lineByOffset2 = manager.document.GetLineByOffset(base.EndOffset.CoerceValue(0, manager.document.TextLength));
		if (lineByOffset == lineByOffset2)
		{
			RemoveCollapsedLineSection();
			return;
		}
		if (collapsedSections == null)
		{
			collapsedSections = new CollapsedLineSection[manager.textViews.Count];
		}
		DocumentLine nextLine = lineByOffset.NextLine;
		for (int i = 0; i < collapsedSections.Length; i++)
		{
			CollapsedLineSection collapsedLineSection = collapsedSections[i];
			if (collapsedLineSection == null || collapsedLineSection.Start != nextLine || collapsedLineSection.End != lineByOffset2)
			{
				collapsedLineSection?.Uncollapse();
				collapsedSections[i] = manager.textViews[i].CollapseLines(nextLine, lineByOffset2);
			}
		}
	}

	protected override void OnSegmentChanged()
	{
		ValidateCollapsedLineSections();
		base.OnSegmentChanged();
		if (base.IsConnectedToCollection)
		{
			manager.Redraw(this);
		}
	}

	internal FoldingSection(FoldingManager manager, int startOffset, int endOffset)
	{
		this.manager = manager;
		base.StartOffset = startOffset;
		base.Length = endOffset - startOffset;
	}

	private void RemoveCollapsedLineSection()
	{
		if (collapsedSections == null)
		{
			return;
		}
		CollapsedLineSection[] array = collapsedSections;
		foreach (CollapsedLineSection collapsedLineSection in array)
		{
			if (collapsedLineSection != null && collapsedLineSection.Start != null)
			{
				collapsedLineSection.Uncollapse();
			}
		}
		collapsedSections = null;
	}
}
