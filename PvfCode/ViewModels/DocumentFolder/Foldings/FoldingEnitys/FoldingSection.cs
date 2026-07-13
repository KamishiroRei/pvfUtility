using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

public sealed class FoldingSection : TextSegment
{
	private readonly FoldingManager manager;

	private bool isFolded;

	public CollapsedLineSection[] collapsedSections;

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
				manager.xhMYEsYpfO(this);
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
					manager.xhMYEsYpfO(this);
				}
			}
		}
	}

	public string TextContent => manager.d6MYOknguE.GetText(base.StartOffset, base.EndOffset - base.StartOffset);

	public object Tag { get; set; }

	public void ValidateCollapsedLineSections()
	{
		if (!isFolded)
		{
			ClearCollapsedLineSections();
			return;
		}
		DocumentLine lineByOffset = manager.d6MYOknguE.GetLineByOffset(base.StartOffset.CoerceValue(0, manager.d6MYOknguE.TextLength));
		DocumentLine lineByOffset2 = manager.d6MYOknguE.GetLineByOffset(base.EndOffset.CoerceValue(0, manager.d6MYOknguE.TextLength));
		if (lineByOffset == lineByOffset2)
		{
			ClearCollapsedLineSections();
			return;
		}
		if (collapsedSections == null)
		{
			collapsedSections = new CollapsedLineSection[manager.mIhYKQMGji.Count];
		}
		DocumentLine nextLine = lineByOffset.NextLine;
		for (int i = 0; i < collapsedSections.Length; i++)
		{
			CollapsedLineSection collapsedLineSection = collapsedSections[i];
			if (collapsedLineSection == null || collapsedLineSection.Start != nextLine || collapsedLineSection.End != lineByOffset2)
			{
				collapsedLineSection?.Uncollapse();
				collapsedSections[i] = manager.mIhYKQMGji[i].CollapseLines(nextLine, lineByOffset2);
			}
		}
	}

	protected override void OnSegmentChanged()
	{
		ValidateCollapsedLineSections();
		base.OnSegmentChanged();
		if (base.IsConnectedToCollection)
		{
			manager.xhMYEsYpfO(this);
		}
	}

	public FoldingSection(FoldingManager manager, int startOffset, int endOffset)
	{
		this.manager = manager;
		base.StartOffset = startOffset;
		base.Length = endOffset - startOffset;
	}

	private void ClearCollapsedLineSections()
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
