using System;
using System.Collections.ObjectModel;
using System.Linq;
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

	private readonly FoldingManager foldingManager;

	public GetVisibleOffsetDelegate GetVisibleOffset;

	public KnownLayer Layer => KnownLayer.Selection;

	public FoldingGuideLines(FoldingManager foldingManager, GetVisibleOffsetDelegate getVisibleOffsetDelegate)
	{
		this.foldingManager = foldingManager;
		GetVisibleOffset = getVisibleOffsetDelegate;
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (!AppSetting.Instance.EditConfig.UseFoldingGuideLines || foldingManager == null || foldingManager.hBGY9EMuL6 == null || !foldingManager.hBGY9EMuL6.Any() || textView == null || drawingContext == null || !textView.VisualLinesValid)
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
			ReadOnlyCollection<FoldingSection> visibleFoldings = foldingManager.hBGY9EMuL6.FindOverlappingSegments(offset, endOffset - offset);
			if (visibleFoldings == null)
			{
				return;
			}
			foreach (FoldingSection folding in visibleFoldings.Where((FoldingSection item) => !item.IsFolded))
			{
				if (folding.StartOffset >= 0 && folding.StartOffset <= textView.Document.TextLength && folding.EndOffset >= 0 && folding.EndOffset <= textView.Document.TextLength)
				{
					DocumentLine lineByOffset = textView.Document.GetLineByOffset(folding.StartOffset);
					ISegment leadingWhitespace = TextUtilities.GetLeadingWhitespace(textView.Document, lineByOffset);
					DocumentLine lineByOffset2 = textView.Document.GetLineByOffset(folding.EndOffset);
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
}
