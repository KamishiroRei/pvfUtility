using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

namespace PvfCode.ViewModels.DocumentFolder.BackgroundRenderers;

public class FoldingGuideLines : IBackgroundRenderer
{
	public delegate TextSegment? GetVisibleOffsetDelegate();

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public int WOwIP4RPS7;

		public int Fc6IZoNJue;

		public _003C_003Ec__DisplayClass7_0()
		{
		}

		internal bool D2LI98WX66(FoldingSection f)
		{
			if (!f.IsFolded)
			{
				if (f.StartOffset < WOwIP4RPS7 || f.StartOffset > Fc6IZoNJue)
				{
					if (f.EndOffset >= WOwIP4RPS7)
					{
						return f.StartOffset <= Fc6IZoNJue;
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}

	private readonly FoldingManager XQfG2pAD5B;

	public GetVisibleOffsetDelegate GetVisibleOffset;

	public KnownLayer Layer => KnownLayer.Selection;

	public FoldingGuideLines(FoldingManager foldingManager, GetVisibleOffsetDelegate getVisibleOffsetDelegate)
	{
		XQfG2pAD5B = foldingManager;
		GetVisibleOffset = getVisibleOffsetDelegate;
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (!AppSetting.Instance.EditConfig.UseFoldingGuideLines || XQfG2pAD5B == null || XQfG2pAD5B.hBGY9EMuL6 == null || !XQfG2pAD5B.hBGY9EMuL6.Any() || textView == null || drawingContext == null || !textView.VisualLinesValid)
		{
			return;
		}
		try
		{
			ReadOnlyCollection<VisualLine> visualLines = textView.VisualLines;
			if (visualLines.Count == 0)
			{
				return;
			}
			int offset = visualLines.First().FirstDocumentLine.Offset;
			int endOffset = visualLines.Last().LastDocumentLine.EndOffset;
			ReadOnlyCollection<FoldingSection> readOnlyCollection = XQfG2pAD5B.hBGY9EMuL6.FindOverlappingSegments(offset, endOffset - offset);
			if (readOnlyCollection == null)
			{
				return;
			}
			foreach (FoldingSection item in readOnlyCollection.Where((FoldingSection it) => !it.IsFolded))
			{
				if (item.StartOffset >= 0 && item.StartOffset <= textView.Document.TextLength && item.EndOffset >= 0 && item.EndOffset <= textView.Document.TextLength)
				{
					DocumentLine lineByOffset = textView.Document.GetLineByOffset(item.StartOffset);
					ISegment leadingWhitespace = TextUtilities.GetLeadingWhitespace(textView.Document, lineByOffset);
					DocumentLine lineByOffset2 = textView.Document.GetLineByOffset(item.EndOffset);
					TextLocation location = textView.Document.GetLocation(leadingWhitespace.EndOffset);
					TextLocation location2 = textView.Document.GetLocation(textView.Document.GetOffset(lineByOffset2.LineNumber, location.Column));
					TextViewPosition position = new TextViewPosition(location);
					TextViewPosition position2 = new TextViewPosition(location2);
					Point val = textView.GetVisualPosition(position, VisualYPosition.LineBottom);
					Point val2 = textView.GetVisualPosition(position2, VisualYPosition.LineMiddle);
					val -= textView.ScrollOffset;
					val2 -= textView.ScrollOffset;
					Brush foldingGuideLineBrush = AppSetting.Instance.EditConfig.GetFoldingGuideLineBrush(leadingWhitespace.Length);
					Pen pen = new Pen
					{
						DashStyle = new DashStyle
						{
							Dashes = new DoubleCollection { 0.0, 2.0 }
						},
						Brush = foldingGuideLineBrush,
						Thickness = 1.0
					};
					((Freezable)pen).Freeze();
					drawingContext.DrawLine(pen, val, new Point(val.X, val2.Y));
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "折叠引导线错误.Draw");
		}
	}

	private IEnumerable<FoldingSection> UNGGmvhj2Y(ReadOnlyCollection<VisualLine> P_0)
	{
		try
		{
			_003C_003Ec__DisplayClass7_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass7_0();
			CS_0024_003C_003E8__locals6.WOwIP4RPS7 = P_0.First().FirstDocumentLine.Offset;
			CS_0024_003C_003E8__locals6.Fc6IZoNJue = P_0.Last().LastDocumentLine.EndOffset;
			return XQfG2pAD5B.AllFoldings.Where((FoldingSection f) => !f.IsFolded && ((f.StartOffset >= CS_0024_003C_003E8__locals6.WOwIP4RPS7 && f.StartOffset <= CS_0024_003C_003E8__locals6.Fc6IZoNJue) || (f.EndOffset >= CS_0024_003C_003E8__locals6.WOwIP4RPS7 && f.StartOffset <= CS_0024_003C_003E8__locals6.Fc6IZoNJue)));
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "FoldingGuideLines.GetSource");
			return null;
		}
	}
}
