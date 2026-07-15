using System;
using System.Collections.Generic;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ViewModels.DocumentFolder.BackgroundRenderers;

public class LineGuideLines : IBackgroundRenderer
{
	public List<TextSegment> TextSegments { get; set; }

	public KnownLayer Layer => KnownLayer.Selection;

	public LineGuideLines()
	{
		TextSegments = new List<TextSegment>();
	}

	public void Update(TextDocument document)
	{
		IList<DocumentLine> lines = document.Lines;
		if (lines.Count == 0)
		{
			return;
		}
		foreach (DocumentLine item in lines)
		{
			if (item.Length > 0)
			{
				string text = document.GetText(item);
				CountLeadingTabs(text);
			}
		}
	}

	private int CountLeadingTabs(string text)
	{
		if (text.Length <= 1 || text[0] != '\t')
		{
			return 0;
		}
		if (text.Length <= 2 || text[1] != '\t')
		{
			return 1;
		}
		if (text.Length <= 3 || text[2] != '\t')
		{
			return 2;
		}
		if (text.Length <= 4 || text[3] != '\t')
		{
			return 3;
		}
		if (text.Length <= 5 || text[4] != '\t')
		{
			return 4;
		}
		if (text.Length <= 6 || text[5] != '\t')
		{
			return 5;
		}
		if (text.Length <= 7 || text[6] != '\t')
		{
			return 6;
		}
		return 0;
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		throw new NotImplementedException();
	}
}
