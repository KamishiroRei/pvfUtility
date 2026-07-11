using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ViewModels.DocumentFolder.BackgroundRenderers;

public class LineGuideLines : IBackgroundRenderer
{
	[CompilerGenerated]
	private List<TextSegment> PH6PvWvbZ;

	public List<TextSegment> TextSegments
	{
		[CompilerGenerated]
		get
		{
			return PH6PvWvbZ;
		}
		[CompilerGenerated]
		set
		{
			PH6PvWvbZ = value;
		}
	}

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
				faE9tyVWV(text);
			}
		}
	}

	private int faE9tyVWV(string P_0)
	{
		if (P_0.Length <= 1 || P_0[0] != '\t')
		{
			return 0;
		}
		if (P_0.Length <= 2 || P_0[1] != '\t')
		{
			return 1;
		}
		if (P_0.Length <= 3 || P_0[2] != '\t')
		{
			return 2;
		}
		if (P_0.Length <= 4 || P_0[3] != '\t')
		{
			return 3;
		}
		if (P_0.Length <= 5 || P_0[4] != '\t')
		{
			return 4;
		}
		if (P_0.Length <= 6 || P_0[5] != '\t')
		{
			return 5;
		}
		if (P_0.Length <= 7 || P_0[6] != '\t')
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
