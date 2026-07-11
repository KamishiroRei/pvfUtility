using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering;

public sealed class VisualLine
{
	private enum LifetimePhase : byte
	{
		Generating,
		Transforming,
		Live,
		Disposed
	}

	private TextView textView;

	private List<VisualLineElement> elements;

	internal bool hasInlineObjects;

	private LifetimePhase phase;

	private ReadOnlyCollection<TextLine> textLines;

	private VisualLineDrawingVisual visual;

	public TextDocument Document { get; private set; }

	public DocumentLine FirstDocumentLine { get; private set; }

	public DocumentLine LastDocumentLine { get; private set; }

	public ReadOnlyCollection<VisualLineElement> Elements { get; private set; }

	public ReadOnlyCollection<TextLine> TextLines
	{
		get
		{
			if ((int)phase < 2)
			{
				throw new InvalidOperationException();
			}
			return textLines;
		}
	}

	public int StartOffset => FirstDocumentLine.Offset;

	public int VisualLength { get; private set; }

	public int VisualLengthWithEndOfLineMarker
	{
		get
		{
			int num = VisualLength;
			if (textView.Options.ShowEndOfLine && LastDocumentLine.NextLine != null)
			{
				num++;
			}
			return num;
		}
	}

	public double Height { get; private set; }

	public double VisualTop { get; internal set; }

	public bool IsDisposed => phase == LifetimePhase.Disposed;

	internal VisualLine(TextView textView, DocumentLine firstDocumentLine)
	{
		this.textView = textView;
		Document = textView.Document;
		FirstDocumentLine = firstDocumentLine;
	}

	internal void ConstructVisualElements(ITextRunConstructionContext context, VisualLineElementGenerator[] generators)
	{
		VisualLineElementGenerator[] array = generators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StartGeneration(context);
		}
		elements = new List<VisualLineElement>();
		PerformVisualElementConstruction(generators);
		array = generators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].FinishGeneration();
		}
		TextRunProperties globalTextRunProperties = context.GlobalTextRunProperties;
		foreach (VisualLineElement element in elements)
		{
			element.SetTextRunProperties(new VisualLineElementTextRunProperties(globalTextRunProperties));
		}
		Elements = elements.AsReadOnly();
		CalculateOffsets();
		phase = LifetimePhase.Transforming;
	}

	private void PerformVisualElementConstruction(VisualLineElementGenerator[] generators)
	{
		TextDocument document = Document;
		int num = FirstDocumentLine.Offset;
		int num2 = num + FirstDocumentLine.Length;
		LastDocumentLine = FirstDocumentLine;
		int num3 = 0;
		while (num + num3 <= num2)
		{
			int num4 = num2;
			VisualLineElementGenerator[] array = generators;
			foreach (VisualLineElementGenerator visualLineElementGenerator in array)
			{
				visualLineElementGenerator.cachedInterest = visualLineElementGenerator.GetFirstInterestedOffset(num + num3);
				if (visualLineElementGenerator.cachedInterest != -1)
				{
					if (visualLineElementGenerator.cachedInterest < num)
					{
						throw new ArgumentOutOfRangeException(visualLineElementGenerator.GetType().Name + ".GetFirstInterestedOffset", visualLineElementGenerator.cachedInterest, "GetFirstInterestedOffset must not return an offset less than startOffset. Return -1 to signal no interest.");
					}
					if (visualLineElementGenerator.cachedInterest < num4)
					{
						num4 = visualLineElementGenerator.cachedInterest;
					}
				}
			}
			if (num4 > num)
			{
				int length = num4 - num;
				elements.Add(new VisualLineText(this, length));
				num = num4;
			}
			num3 = 1;
			array = generators;
			foreach (VisualLineElementGenerator visualLineElementGenerator2 in array)
			{
				if (visualLineElementGenerator2.cachedInterest != num)
				{
					continue;
				}
				VisualLineElement visualLineElement = visualLineElementGenerator2.ConstructElement(num);
				if (visualLineElement == null)
				{
					continue;
				}
				elements.Add(visualLineElement);
				if (visualLineElement.DocumentLength > 0)
				{
					num3 = 0;
					num += visualLineElement.DocumentLength;
					if (num <= num2)
					{
						break;
					}
					DocumentLine lineByOffset = document.GetLineByOffset(num);
					num2 = lineByOffset.Offset + lineByOffset.Length;
					LastDocumentLine = lineByOffset;
					if (num2 >= num)
					{
						break;
					}
					throw new InvalidOperationException("The VisualLineElementGenerator " + visualLineElementGenerator2.GetType().Name + " produced an element which ends within the line delimiter");
				}
			}
		}
	}

	private void CalculateOffsets()
	{
		int num = 0;
		int num2 = 0;
		foreach (VisualLineElement element in elements)
		{
			element.VisualColumn = num;
			element.RelativeTextOffset = num2;
			num += element.VisualLength;
			num2 += element.DocumentLength;
		}
		VisualLength = num;
	}

	internal void RunTransformers(ITextRunConstructionContext context, IVisualLineTransformer[] transformers)
	{
		for (int i = 0; i < transformers.Length; i++)
		{
			transformers[i].Transform(context, elements);
		}
		if (elements.Any((VisualLineElement e) => e.TextRunProperties.TypographyProperties != null))
		{
			foreach (VisualLineElement element in elements)
			{
				if (element.TextRunProperties.TypographyProperties == null)
				{
					element.TextRunProperties.SetTypographyProperties(new DefaultTextRunTypographyProperties());
				}
			}
		}
		phase = LifetimePhase.Live;
	}

	public void ReplaceElement(int elementIndex, params VisualLineElement[] newElements)
	{
		ReplaceElement(elementIndex, 1, newElements);
	}

	public void ReplaceElement(int elementIndex, int count, params VisualLineElement[] newElements)
	{
		if (phase != LifetimePhase.Transforming)
		{
			throw new InvalidOperationException("This method may only be called by line transformers.");
		}
		int num = 0;
		for (int i = elementIndex; i < elementIndex + count; i++)
		{
			num += elements[i].DocumentLength;
		}
		int num2 = 0;
		foreach (VisualLineElement visualLineElement in newElements)
		{
			num2 += visualLineElement.DocumentLength;
		}
		if (num != num2)
		{
			throw new InvalidOperationException("Old elements have document length " + num + ", but new elements have length " + num2);
		}
		elements.RemoveRange(elementIndex, count);
		elements.InsertRange(elementIndex, newElements);
		CalculateOffsets();
	}

	internal void SetTextLines(List<TextLine> textLines)
	{
		this.textLines = textLines.AsReadOnly();
		Height = 0.0;
		foreach (TextLine textLine in textLines)
		{
			Height += textLine.Height;
		}
	}

	public int GetVisualColumn(int relativeTextOffset)
	{
		ThrowUtil.CheckNotNegative(relativeTextOffset, "relativeTextOffset");
		foreach (VisualLineElement element in elements)
		{
			if (element.RelativeTextOffset <= relativeTextOffset && element.RelativeTextOffset + element.DocumentLength >= relativeTextOffset)
			{
				return element.GetVisualColumn(relativeTextOffset);
			}
		}
		return VisualLength;
	}

	public int GetRelativeOffset(int visualColumn)
	{
		ThrowUtil.CheckNotNegative(visualColumn, "visualColumn");
		int num = 0;
		foreach (VisualLineElement element in elements)
		{
			if (element.VisualColumn <= visualColumn && element.VisualColumn + element.VisualLength > visualColumn)
			{
				return element.GetRelativeOffset(visualColumn);
			}
			num += element.DocumentLength;
		}
		return num;
	}

	public TextLine GetTextLine(int visualColumn)
	{
		return GetTextLine(visualColumn, isAtEndOfLine: false);
	}

	public TextLine GetTextLine(int visualColumn, bool isAtEndOfLine)
	{
		if (visualColumn < 0)
		{
			throw new ArgumentOutOfRangeException("visualColumn");
		}
		if (visualColumn >= VisualLengthWithEndOfLineMarker)
		{
			return TextLines[TextLines.Count - 1];
		}
		foreach (TextLine textLine in TextLines)
		{
			if (isAtEndOfLine ? (visualColumn <= textLine.Length) : (visualColumn < textLine.Length))
			{
				return textLine;
			}
			visualColumn -= textLine.Length;
		}
		throw new InvalidOperationException("Shouldn't happen (VisualLength incorrect?)");
	}

	public double GetTextLineVisualYPosition(TextLine textLine, VisualYPosition yPositionMode)
	{
		if (textLine == null)
		{
			throw new ArgumentNullException("textLine");
		}
		double num = VisualTop;
		foreach (TextLine textLine2 in TextLines)
		{
			if (textLine2 == textLine)
			{
				return yPositionMode switch
				{
					VisualYPosition.LineTop => num, 
					VisualYPosition.LineMiddle => num + textLine2.Height / 2.0, 
					VisualYPosition.LineBottom => num + textLine2.Height, 
					VisualYPosition.TextTop => num + textLine2.Baseline - textView.DefaultBaseline, 
					VisualYPosition.TextBottom => num + textLine2.Baseline - textView.DefaultBaseline + textView.DefaultLineHeight, 
					VisualYPosition.TextMiddle => num + textLine2.Baseline - textView.DefaultBaseline + textView.DefaultLineHeight / 2.0, 
					VisualYPosition.Baseline => num + textLine2.Baseline, 
					_ => throw new ArgumentException("Invalid yPositionMode:" + yPositionMode), 
				};
			}
			num += textLine2.Height;
		}
		throw new ArgumentException("textLine is not a line in this VisualLine");
	}

	public int GetTextLineVisualStartColumn(TextLine textLine)
	{
		if (!TextLines.Contains(textLine))
		{
			throw new ArgumentException("textLine is not a line in this VisualLine");
		}
		int num = 0;
		foreach (TextLine textLine2 in TextLines)
		{
			if (textLine2 == textLine)
			{
				break;
			}
			num += textLine2.Length;
		}
		return num;
	}

	public TextLine GetTextLineByVisualYPosition(double visualTop)
	{
		double num = VisualTop;
		foreach (TextLine textLine in TextLines)
		{
			num += textLine.Height;
			if (visualTop + 0.0001 < num)
			{
				return textLine;
			}
		}
		return TextLines[TextLines.Count - 1];
	}

	public Point GetVisualPosition(int visualColumn, VisualYPosition yPositionMode)
	{
		TextLine textLine = GetTextLine(visualColumn);
		double textLineVisualXPosition = GetTextLineVisualXPosition(textLine, visualColumn);
		double textLineVisualYPosition = GetTextLineVisualYPosition(textLine, yPositionMode);
		return new Point(textLineVisualXPosition, textLineVisualYPosition);
	}

	internal Point GetVisualPosition(int visualColumn, bool isAtEndOfLine, VisualYPosition yPositionMode)
	{
		TextLine textLine = GetTextLine(visualColumn, isAtEndOfLine);
		double textLineVisualXPosition = GetTextLineVisualXPosition(textLine, visualColumn);
		double textLineVisualYPosition = GetTextLineVisualYPosition(textLine, yPositionMode);
		return new Point(textLineVisualXPosition, textLineVisualYPosition);
	}

	public double GetTextLineVisualXPosition(TextLine textLine, int visualColumn)
	{
		if (textLine == null)
		{
			throw new ArgumentNullException("textLine");
		}
		double num = textLine.GetDistanceFromCharacterHit(new CharacterHit(Math.Min(visualColumn, VisualLengthWithEndOfLineMarker), 0));
		if (visualColumn > VisualLengthWithEndOfLineMarker)
		{
			num += (double)(visualColumn - VisualLengthWithEndOfLineMarker) * textView.WideSpaceWidth;
		}
		return num;
	}

	public int GetVisualColumn(Point point)
	{
		return GetVisualColumn(point, textView.Options.EnableVirtualSpace);
	}

	public int GetVisualColumn(Point point, bool allowVirtualSpace)
	{
		return GetVisualColumn(GetTextLineByVisualYPosition(point.Y), point.X, allowVirtualSpace);
	}

	internal int GetVisualColumn(Point point, bool allowVirtualSpace, out bool isAtEndOfLine)
	{
		TextLine textLineByVisualYPosition = GetTextLineByVisualYPosition(point.Y);
		int visualColumn = GetVisualColumn(textLineByVisualYPosition, point.X, allowVirtualSpace);
		isAtEndOfLine = visualColumn >= GetTextLineVisualStartColumn(textLineByVisualYPosition) + textLineByVisualYPosition.Length;
		return visualColumn;
	}

	public int GetVisualColumn(TextLine textLine, double xPos, bool allowVirtualSpace)
	{
		if (xPos > textLine.WidthIncludingTrailingWhitespace && allowVirtualSpace && textLine == TextLines[TextLines.Count - 1])
		{
			int num = (int)Math.Round((xPos - textLine.WidthIncludingTrailingWhitespace) / textView.WideSpaceWidth, MidpointRounding.AwayFromZero);
			return VisualLengthWithEndOfLineMarker + num;
		}
		CharacterHit characterHitFromDistance = textLine.GetCharacterHitFromDistance(xPos);
		return characterHitFromDistance.FirstCharacterIndex + characterHitFromDistance.TrailingLength;
	}

	public int ValidateVisualColumn(TextViewPosition position, bool allowVirtualSpace)
	{
		return ValidateVisualColumn(Document.GetOffset(position.Location), position.VisualColumn, allowVirtualSpace);
	}

	public int ValidateVisualColumn(int offset, int visualColumn, bool allowVirtualSpace)
	{
		int offset2 = FirstDocumentLine.Offset;
		if (visualColumn < 0)
		{
			return GetVisualColumn(offset - offset2);
		}
		if (GetRelativeOffset(visualColumn) + offset2 != offset)
		{
			return GetVisualColumn(offset - offset2);
		}
		if (visualColumn > VisualLength && !allowVirtualSpace)
		{
			return VisualLength;
		}
		return visualColumn;
	}

	public int GetVisualColumnFloor(Point point)
	{
		return GetVisualColumnFloor(point, textView.Options.EnableVirtualSpace);
	}

	public int GetVisualColumnFloor(Point point, bool allowVirtualSpace)
	{
		bool isAtEndOfLine;
		return GetVisualColumnFloor(point, allowVirtualSpace, out isAtEndOfLine);
	}

	internal int GetVisualColumnFloor(Point point, bool allowVirtualSpace, out bool isAtEndOfLine)
	{
		TextLine textLineByVisualYPosition = GetTextLineByVisualYPosition(point.Y);
		if (point.X > textLineByVisualYPosition.WidthIncludingTrailingWhitespace)
		{
			isAtEndOfLine = true;
			if (allowVirtualSpace && textLineByVisualYPosition == TextLines[TextLines.Count - 1])
			{
				int num = (int)((point.X - textLineByVisualYPosition.WidthIncludingTrailingWhitespace) / textView.WideSpaceWidth);
				return VisualLengthWithEndOfLineMarker + num;
			}
			return GetTextLineVisualStartColumn(textLineByVisualYPosition) + textLineByVisualYPosition.Length;
		}
		isAtEndOfLine = false;
		return textLineByVisualYPosition.GetCharacterHitFromDistance(point.X).FirstCharacterIndex;
	}

	public TextViewPosition GetTextViewPosition(int visualColumn)
	{
		int offset = GetRelativeOffset(visualColumn) + FirstDocumentLine.Offset;
		return new TextViewPosition(Document.GetLocation(offset), visualColumn);
	}

	public TextViewPosition GetTextViewPosition(Point visualPosition, bool allowVirtualSpace)
	{
		bool isAtEndOfLine;
		int visualColumn = GetVisualColumn(visualPosition, allowVirtualSpace, out isAtEndOfLine);
		int offset = GetRelativeOffset(visualColumn) + FirstDocumentLine.Offset;
		TextViewPosition result = new TextViewPosition(Document.GetLocation(offset), visualColumn);
		result.IsAtEndOfLine = isAtEndOfLine;
		return result;
	}

	public TextViewPosition GetTextViewPositionFloor(Point visualPosition, bool allowVirtualSpace)
	{
		bool isAtEndOfLine;
		int visualColumnFloor = GetVisualColumnFloor(visualPosition, allowVirtualSpace, out isAtEndOfLine);
		int offset = GetRelativeOffset(visualColumnFloor) + FirstDocumentLine.Offset;
		TextViewPosition result = new TextViewPosition(Document.GetLocation(offset), visualColumnFloor);
		result.IsAtEndOfLine = isAtEndOfLine;
		return result;
	}

	internal void Dispose()
	{
		if (phase == LifetimePhase.Disposed)
		{
			return;
		}
		phase = LifetimePhase.Disposed;
		foreach (TextLine textLine in TextLines)
		{
			textLine.Dispose();
		}
	}

	public int GetNextCaretPosition(int visualColumn, LogicalDirection direction, CaretPositioningMode mode, bool allowVirtualSpace)
	{
		if (!HasStopsInVirtualSpace(mode))
		{
			allowVirtualSpace = false;
		}
		if (elements.Count == 0)
		{
			if (allowVirtualSpace)
			{
				if (direction == LogicalDirection.Forward)
				{
					return Math.Max(0, visualColumn + 1);
				}
				if (visualColumn > 0)
				{
					return visualColumn - 1;
				}
				return -1;
			}
			if (visualColumn < 0 && direction == LogicalDirection.Forward)
			{
				return 0;
			}
			if (visualColumn > 0 && direction == LogicalDirection.Backward)
			{
				return 0;
			}
			return -1;
		}
		if (direction == LogicalDirection.Backward)
		{
			if (visualColumn > VisualLength && !elements[elements.Count - 1].HandlesLineBorders && HasImplicitStopAtLineEnd(mode))
			{
				if (allowVirtualSpace)
				{
					return visualColumn - 1;
				}
				return VisualLength;
			}
			int num = elements.Count - 1;
			while (num >= 0 && elements[num].VisualColumn >= visualColumn)
			{
				num--;
			}
			while (num >= 0)
			{
				int nextCaretPosition = elements[num].GetNextCaretPosition(Math.Min(visualColumn, elements[num].VisualColumn + elements[num].VisualLength + 1), direction, mode);
				if (nextCaretPosition >= 0)
				{
					return nextCaretPosition;
				}
				num--;
			}
			if (visualColumn > 0 && !elements[0].HandlesLineBorders && HasImplicitStopAtLineStart(mode))
			{
				return 0;
			}
		}
		else
		{
			if (visualColumn < 0 && !elements[0].HandlesLineBorders && HasImplicitStopAtLineStart(mode))
			{
				return 0;
			}
			int num;
			for (num = 0; num < elements.Count && elements[num].VisualColumn + elements[num].VisualLength <= visualColumn; num++)
			{
			}
			for (; num < elements.Count; num++)
			{
				int nextCaretPosition2 = elements[num].GetNextCaretPosition(Math.Max(visualColumn, elements[num].VisualColumn - 1), direction, mode);
				if (nextCaretPosition2 >= 0)
				{
					return nextCaretPosition2;
				}
			}
			if ((allowVirtualSpace || !elements[elements.Count - 1].HandlesLineBorders) && HasImplicitStopAtLineEnd(mode))
			{
				if (visualColumn < VisualLength)
				{
					return VisualLength;
				}
				if (allowVirtualSpace)
				{
					return visualColumn + 1;
				}
			}
		}
		return -1;
	}

	private static bool HasStopsInVirtualSpace(CaretPositioningMode mode)
	{
		if (mode != CaretPositioningMode.Normal)
		{
			return mode == CaretPositioningMode.EveryCodepoint;
		}
		return true;
	}

	private static bool HasImplicitStopAtLineStart(CaretPositioningMode mode)
	{
		if (mode != CaretPositioningMode.Normal)
		{
			return mode == CaretPositioningMode.EveryCodepoint;
		}
		return true;
	}

	private static bool HasImplicitStopAtLineEnd(CaretPositioningMode mode)
	{
		return true;
	}

	internal VisualLineDrawingVisual Render()
	{
		if (visual == null)
		{
			visual = new VisualLineDrawingVisual(this, textView.FlowDirection);
		}
		return visual;
	}
}
