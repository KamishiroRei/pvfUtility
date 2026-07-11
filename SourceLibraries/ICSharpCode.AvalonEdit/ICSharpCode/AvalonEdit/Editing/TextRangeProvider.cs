using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Text;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing;

internal class TextRangeProvider : ITextRangeProvider
{
	private readonly TextArea textArea;

	private readonly TextDocument doc;

	private ISegment segment;

	private string ID => string.Format("({0}: {1})", GetHashCode().ToString("x8"), segment);

	public TextRangeProvider(TextArea textArea, TextDocument doc, ISegment segment)
	{
		this.textArea = textArea;
		this.doc = doc;
		this.segment = segment;
	}

	public TextRangeProvider(TextArea textArea, TextDocument doc, int offset, int length)
	{
		this.textArea = textArea;
		this.doc = doc;
		segment = new AnchorSegment(doc, offset, length);
	}

	[Conditional("DEBUG")]
	private static void Log(string format, params object[] args)
	{
	}

	public void AddToSelection()
	{
	}

	public ITextRangeProvider Clone()
	{
		return new TextRangeProvider(textArea, doc, segment);
	}

	public bool Compare(ITextRangeProvider range)
	{
		TextRangeProvider textRangeProvider = (TextRangeProvider)range;
		if (doc == textRangeProvider.doc && segment.Offset == textRangeProvider.segment.Offset)
		{
			return segment.EndOffset == textRangeProvider.segment.EndOffset;
		}
		return false;
	}

	private int GetEndpoint(TextPatternRangeEndpoint endpoint)
	{
		return endpoint switch
		{
			TextPatternRangeEndpoint.Start => segment.Offset, 
			TextPatternRangeEndpoint.End => segment.EndOffset, 
			_ => throw new ArgumentOutOfRangeException("endpoint"), 
		};
	}

	public int CompareEndpoints(TextPatternRangeEndpoint endpoint, ITextRangeProvider targetRange, TextPatternRangeEndpoint targetEndpoint)
	{
		TextRangeProvider textRangeProvider = (TextRangeProvider)targetRange;
		return GetEndpoint(endpoint).CompareTo(textRangeProvider.GetEndpoint(targetEndpoint));
	}

	public void ExpandToEnclosingUnit(TextUnit unit)
	{
		switch (unit)
		{
		case TextUnit.Character:
			ExpandToEnclosingUnit(CaretPositioningMode.Normal);
			break;
		case TextUnit.Format:
		case TextUnit.Word:
			ExpandToEnclosingUnit(CaretPositioningMode.WordStartOrSymbol);
			break;
		case TextUnit.Line:
		case TextUnit.Paragraph:
			segment = doc.GetLineByOffset(segment.Offset);
			break;
		case TextUnit.Document:
			segment = new AnchorSegment(doc, 0, doc.TextLength);
			break;
		case TextUnit.Page:
			break;
		}
	}

	private void ExpandToEnclosingUnit(CaretPositioningMode mode)
	{
		int nextCaretPosition = TextUtilities.GetNextCaretPosition(doc, segment.Offset + 1, LogicalDirection.Backward, mode);
		if (nextCaretPosition >= 0)
		{
			int nextCaretPosition2 = TextUtilities.GetNextCaretPosition(doc, nextCaretPosition, LogicalDirection.Forward, mode);
			if (nextCaretPosition2 >= 0)
			{
				segment = new AnchorSegment(doc, nextCaretPosition, nextCaretPosition2 - nextCaretPosition);
			}
		}
	}

	public ITextRangeProvider FindAttribute(int attribute, object value, bool backward)
	{
		return null;
	}

	public ITextRangeProvider FindText(string text, bool backward, bool ignoreCase)
	{
		string text2 = doc.GetText(segment);
		StringComparison comparisonType = (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		int num = (backward ? text2.LastIndexOf(text, comparisonType) : text2.IndexOf(text, comparisonType));
		if (num >= 0)
		{
			return new TextRangeProvider(textArea, doc, segment.Offset + num, text.Length);
		}
		return null;
	}

	public object GetAttributeValue(int attribute)
	{
		return null;
	}

	public double[] GetBoundingRectangles()
	{
		TextView textView = textArea.TextView;
		PresentationSource.FromVisual(textArea);
		List<double> list = new List<double>();
		foreach (Rect item in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
		{
			Point point = textView.PointToScreen(item.TopLeft);
			Point point2 = textView.PointToScreen(item.BottomRight);
			list.Add(point.X);
			list.Add(point.Y);
			list.Add(point2.X - point.X);
			list.Add(point2.Y - point.Y);
		}
		return list.ToArray();
	}

	public IRawElementProviderSimple[] GetChildren()
	{
		return new IRawElementProviderSimple[0];
	}

	public IRawElementProviderSimple GetEnclosingElement()
	{
		return ((UIElementAutomationPeer.FromElement(textArea) as TextAreaAutomationPeer) ?? throw new NotSupportedException()).Provider;
	}

	public string GetText(int maxLength)
	{
		if (maxLength < 0)
		{
			return doc.GetText(segment);
		}
		return doc.GetText(segment.Offset, Math.Min(segment.Length, maxLength));
	}

	public int Move(TextUnit unit, int count)
	{
		int result = MoveEndpointByUnit(TextPatternRangeEndpoint.Start, unit, count);
		segment = new SimpleSegment(segment.Offset, 0);
		ExpandToEnclosingUnit(unit);
		return result;
	}

	public void MoveEndpointByRange(TextPatternRangeEndpoint endpoint, ITextRangeProvider targetRange, TextPatternRangeEndpoint targetEndpoint)
	{
		TextRangeProvider textRangeProvider = (TextRangeProvider)targetRange;
		SetEndpoint(endpoint, textRangeProvider.GetEndpoint(targetEndpoint));
	}

	private void SetEndpoint(TextPatternRangeEndpoint endpoint, int targetOffset)
	{
		if (endpoint == TextPatternRangeEndpoint.Start)
		{
			segment = new AnchorSegment(doc, targetOffset, Math.Max(0, segment.EndOffset - targetOffset));
			return;
		}
		int num = Math.Min(segment.Offset, targetOffset);
		segment = new AnchorSegment(doc, num, targetOffset - num);
	}

	public int MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count)
	{
		int num = GetEndpoint(endpoint);
		switch (unit)
		{
		case TextUnit.Character:
			num = MoveOffset(num, CaretPositioningMode.Normal, count);
			break;
		case TextUnit.Format:
		case TextUnit.Word:
			num = MoveOffset(num, CaretPositioningMode.WordStart, count);
			break;
		case TextUnit.Line:
		case TextUnit.Paragraph:
		{
			int lineNumber = doc.GetLineByOffset(num).LineNumber;
			int number = Math.Max(1, Math.Min(doc.LineCount, lineNumber + count));
			num = doc.GetLineByNumber(number).Offset;
			break;
		}
		case TextUnit.Document:
			num = ((count >= 0) ? doc.TextLength : 0);
			break;
		}
		SetEndpoint(endpoint, num);
		return count;
	}

	private int MoveOffset(int offset, CaretPositioningMode mode, int count)
	{
		LogicalDirection direction = ((count >= 0) ? LogicalDirection.Forward : LogicalDirection.Backward);
		count = Math.Abs(count);
		for (int i = 0; i < count; i++)
		{
			int nextCaretPosition = TextUtilities.GetNextCaretPosition(doc, offset, direction, mode);
			if (nextCaretPosition == offset || nextCaretPosition < 0)
			{
				break;
			}
			offset = nextCaretPosition;
		}
		return offset;
	}

	public void RemoveFromSelection()
	{
	}

	public void ScrollIntoView(bool alignToTop)
	{
	}

	public void Select()
	{
		textArea.Selection = new SimpleSelection(textArea, new TextViewPosition(doc.GetLocation(segment.Offset)), new TextViewPosition(doc.GetLocation(segment.EndOffset)));
	}
}
