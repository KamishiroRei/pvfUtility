using System.Runtime.CompilerServices;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

public sealed class FoldingSection : TextSegment
{
	private readonly FoldingManager m6EyHmoQ5G;

	private bool eCoyhLpoCn;

	public CollapsedLineSection[] collapsedSections;

	private string title;

	[CompilerGenerated]
	private object OwxyvJuGSj;

	public bool IsFolded
	{
		get
		{
			return eCoyhLpoCn;
		}
		set
		{
			if (eCoyhLpoCn != value)
			{
				eCoyhLpoCn = value;
				ValidateCollapsedLineSections();
				m6EyHmoQ5G.xhMYEsYpfO(this);
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
					m6EyHmoQ5G.xhMYEsYpfO(this);
				}
			}
		}
	}

	public string TextContent => m6EyHmoQ5G.d6MYOknguE.GetText(base.StartOffset, base.EndOffset - base.StartOffset);

	public object Tag
	{
		[CompilerGenerated]
		get
		{
			return OwxyvJuGSj;
		}
		[CompilerGenerated]
		set
		{
			OwxyvJuGSj = value;
		}
	}

	public void ValidateCollapsedLineSections()
	{
		if (!eCoyhLpoCn)
		{
			woQyCgVA2w();
			return;
		}
		DocumentLine lineByOffset = m6EyHmoQ5G.d6MYOknguE.GetLineByOffset(base.StartOffset.CoerceValue(0, m6EyHmoQ5G.d6MYOknguE.TextLength));
		DocumentLine lineByOffset2 = m6EyHmoQ5G.d6MYOknguE.GetLineByOffset(base.EndOffset.CoerceValue(0, m6EyHmoQ5G.d6MYOknguE.TextLength));
		if (lineByOffset == lineByOffset2)
		{
			woQyCgVA2w();
			return;
		}
		if (collapsedSections == null)
		{
			collapsedSections = new CollapsedLineSection[m6EyHmoQ5G.mIhYKQMGji.Count];
		}
		DocumentLine nextLine = lineByOffset.NextLine;
		for (int i = 0; i < collapsedSections.Length; i++)
		{
			CollapsedLineSection collapsedLineSection = collapsedSections[i];
			if (collapsedLineSection == null || collapsedLineSection.Start != nextLine || collapsedLineSection.End != lineByOffset2)
			{
				collapsedLineSection?.Uncollapse();
				collapsedSections[i] = m6EyHmoQ5G.mIhYKQMGji[i].CollapseLines(nextLine, lineByOffset2);
			}
		}
	}

	protected override void OnSegmentChanged()
	{
		ValidateCollapsedLineSections();
		base.OnSegmentChanged();
		if (base.IsConnectedToCollection)
		{
			m6EyHmoQ5G.xhMYEsYpfO(this);
		}
	}

	public FoldingSection(FoldingManager manager, int startOffset, int endOffset)
	{
		m6EyHmoQ5G = manager;
		base.StartOffset = startOffset;
		base.Length = endOffset - startOffset;
	}

	private void woQyCgVA2w()
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
