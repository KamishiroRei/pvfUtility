using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

public sealed class RectangleSelection : Selection
{
	public static readonly RoutedUICommand BoxSelectLeftByCharacter = Command("BoxSelectLeftByCharacter");

	public static readonly RoutedUICommand BoxSelectRightByCharacter = Command("BoxSelectRightByCharacter");

	public static readonly RoutedUICommand BoxSelectLeftByWord = Command("BoxSelectLeftByWord");

	public static readonly RoutedUICommand BoxSelectRightByWord = Command("BoxSelectRightByWord");

	public static readonly RoutedUICommand BoxSelectUpByLine = Command("BoxSelectUpByLine");

	public static readonly RoutedUICommand BoxSelectDownByLine = Command("BoxSelectDownByLine");

	public static readonly RoutedUICommand BoxSelectToLineStart = Command("BoxSelectToLineStart");

	public static readonly RoutedUICommand BoxSelectToLineEnd = Command("BoxSelectToLineEnd");

	private TextDocument document;

	private readonly int startLine;

	private readonly int endLine;

	private readonly double startXPos;

	private readonly double endXPos;

	private readonly int topLeftOffset;

	private readonly int bottomRightOffset;

	private readonly TextViewPosition start;

	private readonly TextViewPosition end;

	private readonly List<SelectionSegment> segments = new List<SelectionSegment>();

	public const string RectangularSelectionDataType = "AvalonEditRectangularSelection";

	public override int Length => Segments.Sum((SelectionSegment s) => s.Length);

	public override bool EnableVirtualSpace => true;

	public override ISegment SurroundingSegment => new SimpleSegment(topLeftOffset, bottomRightOffset - topLeftOffset);

	public override IEnumerable<SelectionSegment> Segments => segments;

	public override TextViewPosition StartPosition => start;

	public override TextViewPosition EndPosition => end;

	private static RoutedUICommand Command(string name)
	{
		return new RoutedUICommand(name, name, typeof(RectangleSelection));
	}

	public RectangleSelection(TextArea textArea, TextViewPosition start, TextViewPosition end)
		: base(textArea)
	{
		InitDocument();
		startLine = start.Line;
		endLine = end.Line;
		startXPos = GetXPos(textArea, start);
		endXPos = GetXPos(textArea, end);
		CalculateSegments();
		topLeftOffset = segments.First().StartOffset;
		bottomRightOffset = segments.Last().EndOffset;
		this.start = start;
		this.end = end;
	}

	private RectangleSelection(TextArea textArea, int startLine, double startXPos, TextViewPosition end)
		: base(textArea)
	{
		InitDocument();
		this.startLine = startLine;
		endLine = end.Line;
		this.startXPos = startXPos;
		endXPos = GetXPos(textArea, end);
		CalculateSegments();
		topLeftOffset = segments.First().StartOffset;
		bottomRightOffset = segments.Last().EndOffset;
		start = GetStart();
		this.end = end;
	}

	private RectangleSelection(TextArea textArea, TextViewPosition start, int endLine, double endXPos)
		: base(textArea)
	{
		InitDocument();
		startLine = start.Line;
		this.endLine = endLine;
		startXPos = GetXPos(textArea, start);
		this.endXPos = endXPos;
		CalculateSegments();
		topLeftOffset = segments.First().StartOffset;
		bottomRightOffset = segments.Last().EndOffset;
		this.start = start;
		end = GetEnd();
	}

	private void InitDocument()
	{
		document = textArea.Document;
		if (document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
	}

	private static double GetXPos(TextArea textArea, TextViewPosition pos)
	{
		DocumentLine lineByNumber = textArea.Document.GetLineByNumber(pos.Line);
		VisualLine orConstructVisualLine = textArea.TextView.GetOrConstructVisualLine(lineByNumber);
		int visualColumn = orConstructVisualLine.ValidateVisualColumn(pos, allowVirtualSpace: true);
		TextLine textLine = orConstructVisualLine.GetTextLine(visualColumn, pos.IsAtEndOfLine);
		return orConstructVisualLine.GetTextLineVisualXPosition(textLine, visualColumn);
	}

	private void CalculateSegments()
	{
		DocumentLine documentLine = document.GetLineByNumber(Math.Min(startLine, endLine));
		do
		{
			VisualLine orConstructVisualLine = textArea.TextView.GetOrConstructVisualLine(documentLine);
			int visualColumn = orConstructVisualLine.GetVisualColumn(new Point(startXPos, 0.0), allowVirtualSpace: true);
			int visualColumn2 = orConstructVisualLine.GetVisualColumn(new Point(endXPos, 0.0), allowVirtualSpace: true);
			int offset = orConstructVisualLine.FirstDocumentLine.Offset;
			int startOffset = offset + orConstructVisualLine.GetRelativeOffset(visualColumn);
			int endOffset = offset + orConstructVisualLine.GetRelativeOffset(visualColumn2);
			segments.Add(new SelectionSegment(startOffset, visualColumn, endOffset, visualColumn2));
			documentLine = orConstructVisualLine.LastDocumentLine.NextLine;
		}
		while (documentLine != null && documentLine.LineNumber <= Math.Max(startLine, endLine));
	}

	private TextViewPosition GetStart()
	{
		SelectionSegment selectionSegment = ((startLine < endLine) ? segments.First() : segments.Last());
		if (startXPos < endXPos)
		{
			return new TextViewPosition(document.GetLocation(selectionSegment.StartOffset), selectionSegment.StartVisualColumn);
		}
		return new TextViewPosition(document.GetLocation(selectionSegment.EndOffset), selectionSegment.EndVisualColumn);
	}

	private TextViewPosition GetEnd()
	{
		SelectionSegment selectionSegment = ((startLine < endLine) ? segments.Last() : segments.First());
		if (startXPos < endXPos)
		{
			return new TextViewPosition(document.GetLocation(selectionSegment.EndOffset), selectionSegment.EndVisualColumn);
		}
		return new TextViewPosition(document.GetLocation(selectionSegment.StartOffset), selectionSegment.StartVisualColumn);
	}

	public override string GetText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (SelectionSegment segment in Segments)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(document.GetText(segment));
		}
		return stringBuilder.ToString();
	}

	public override Selection StartSelectionOrSetEndpoint(TextViewPosition startPosition, TextViewPosition endPosition)
	{
		return SetEndpoint(endPosition);
	}

	public override bool Equals(object obj)
	{
		if (obj is RectangleSelection rectangleSelection && rectangleSelection.textArea == textArea && rectangleSelection.topLeftOffset == topLeftOffset && rectangleSelection.bottomRightOffset == bottomRightOffset && rectangleSelection.startLine == startLine && rectangleSelection.endLine == endLine && rectangleSelection.startXPos == startXPos)
		{
			return rectangleSelection.endXPos == endXPos;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return topLeftOffset ^ bottomRightOffset;
	}

	public override Selection SetEndpoint(TextViewPosition endPosition)
	{
		return new RectangleSelection(textArea, startLine, startXPos, endPosition);
	}

	private int GetVisualColumnFromXPos(int line, double xPos)
	{
		return textArea.TextView.GetOrConstructVisualLine(textArea.Document.GetLineByNumber(line)).GetVisualColumn(new Point(xPos, 0.0), allowVirtualSpace: true);
	}

	public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
	{
		TextLocation location = textArea.Document.GetLocation(e.GetNewOffset(topLeftOffset, AnchorMovementType.AfterInsertion));
		TextLocation location2 = textArea.Document.GetLocation(e.GetNewOffset(bottomRightOffset, AnchorMovementType.BeforeInsertion));
		return new RectangleSelection(textArea, new TextViewPosition(location, GetVisualColumnFromXPos(location.Line, startXPos)), new TextViewPosition(location2, GetVisualColumnFromXPos(location2.Line, endXPos)));
	}

	public override void ReplaceSelectionWithText(string newText)
	{
		if (newText == null)
		{
			throw new ArgumentNullException("newText");
		}
		using (textArea.Document.RunUpdate())
		{
			int num = 0;
			int num2 = 0;
			int num3 = Math.Min(topLeftOffset, bottomRightOffset);
			int insertionLength;
			TextViewPosition pos;
			if (NewLineFinder.NextNewLine(newText, 0) == SimpleSegment.Invalid)
			{
				foreach (SelectionSegment item in Segments.Reverse())
				{
					ReplaceSingleLineText(textArea, item, newText, out insertionLength);
					num += insertionLength;
					num2 = insertionLength;
				}
				pos = new TextViewPosition(document.GetLocation(num3 + num2));
				textArea.Selection = new RectangleSelection(textArea, pos, Math.Max(startLine, endLine), GetXPos(textArea, pos));
			}
			else
			{
				string[] array = newText.Split(NewLineFinder.NewlineStrings, segments.Count, StringSplitOptions.None);
				Math.Min(startLine, endLine);
				for (int num4 = array.Length - 1; num4 >= 0; num4--)
				{
					ReplaceSingleLineText(textArea, segments[num4], array[num4], out insertionLength);
					num2 = insertionLength;
				}
				pos = new TextViewPosition(document.GetLocation(num3 + num2));
				textArea.ClearSelection();
			}
			textArea.Caret.Position = textArea.TextView.GetPosition(new Point(GetXPos(textArea, pos), textArea.TextView.GetVisualTopByDocumentLine(Math.Max(startLine, endLine)))).GetValueOrDefault();
		}
	}

	private void ReplaceSingleLineText(TextArea textArea, SelectionSegment lineSegment, string newText, out int insertionLength)
	{
		if (lineSegment.Length == 0)
		{
			if (newText.Length > 0 && textArea.ReadOnlySectionProvider.CanInsert(lineSegment.StartOffset))
			{
				newText = AddSpacesIfRequired(newText, new TextViewPosition(document.GetLocation(lineSegment.StartOffset), lineSegment.StartVisualColumn), new TextViewPosition(document.GetLocation(lineSegment.EndOffset), lineSegment.EndVisualColumn));
				textArea.Document.Insert(lineSegment.StartOffset, newText);
			}
		}
		else
		{
			ISegment[] deletableSegments = textArea.GetDeletableSegments(lineSegment);
			for (int num = deletableSegments.Length - 1; num >= 0; num--)
			{
				if (num == deletableSegments.Length - 1)
				{
					if (deletableSegments[num].Offset == SurroundingSegment.Offset && deletableSegments[num].Length == SurroundingSegment.Length)
					{
						newText = AddSpacesIfRequired(newText, new TextViewPosition(document.GetLocation(lineSegment.StartOffset), lineSegment.StartVisualColumn), new TextViewPosition(document.GetLocation(lineSegment.EndOffset), lineSegment.EndVisualColumn));
					}
					textArea.Document.Replace(deletableSegments[num], newText);
				}
				else
				{
					textArea.Document.Remove(deletableSegments[num]);
				}
			}
		}
		insertionLength = newText.Length;
	}

	public static bool PerformRectangularPaste(TextArea textArea, TextViewPosition startPosition, string text, bool selectInsertedText)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		int num = text.Count((char c) => c == '\n');
		TextLocation textLocation = new TextLocation(startPosition.Line + num, startPosition.Column);
		if (textLocation.Line <= textArea.Document.LineCount)
		{
			int offset = textArea.Document.GetOffset(textLocation);
			if (textArea.Selection.EnableVirtualSpace || textArea.Document.GetLocation(offset) == textLocation)
			{
				new RectangleSelection(textArea, startPosition, textLocation.Line, GetXPos(textArea, startPosition)).ReplaceSelectionWithText(text);
				if (selectInsertedText && textArea.Selection is RectangleSelection)
				{
					RectangleSelection rectangleSelection = (RectangleSelection)textArea.Selection;
					textArea.Selection = new RectangleSelection(textArea, startPosition, rectangleSelection.endLine, rectangleSelection.endXPos);
				}
				return true;
			}
		}
		return false;
	}

	public override DataObject CreateDataObject(TextArea textArea)
	{
		DataObject dataObject = base.CreateDataObject(textArea);
		if (EditingCommandHandler.ConfirmDataFormat(textArea, dataObject, "AvalonEditRectangularSelection"))
		{
			MemoryStream memoryStream = new MemoryStream(1);
			memoryStream.WriteByte(1);
			dataObject.SetData("AvalonEditRectangularSelection", memoryStream, autoConvert: false);
		}
		return dataObject;
	}

	public override string ToString()
	{
		return $"[RectangleSelection {startLine} {topLeftOffset} {startXPos} to {endLine} {bottomRightOffset} {endXPos}]";
	}
}
