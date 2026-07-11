using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

public sealed class Caret
{
	private readonly TextArea textArea;

	private readonly TextView textView;

	private readonly CaretLayer caretAdorner;

	private bool visible;

	private double desiredXPos = double.NaN;

	private TextViewPosition position;

	private bool isInVirtualSpace;

	private int storedCaretOffset;

	private bool raisePositionChangedOnUpdateFinished;

	private bool visualColumnValid;

	internal const double MinimumDistanceToViewBorder = 30.0;

	private bool showScheduled;

	private bool hasWin32Caret;

	public TextViewPosition Position
	{
		get
		{
			ValidateVisualColumn();
			return position;
		}
		set
		{
			if (position != value)
			{
				position = value;
				storedCaretOffset = -1;
				ValidatePosition();
				InvalidateVisualColumn();
				RaisePositionChanged();
				if (visible)
				{
					Show();
				}
			}
		}
	}

	internal TextViewPosition NonValidatedPosition => position;

	public TextLocation Location
	{
		get
		{
			return position.Location;
		}
		set
		{
			Position = new TextViewPosition(value);
		}
	}

	public int Line
	{
		get
		{
			return position.Line;
		}
		set
		{
			Position = new TextViewPosition(value, position.Column);
		}
	}

	public int Column
	{
		get
		{
			return position.Column;
		}
		set
		{
			Position = new TextViewPosition(position.Line, value);
		}
	}

	public int VisualColumn
	{
		get
		{
			ValidateVisualColumn();
			return position.VisualColumn;
		}
		set
		{
			Position = new TextViewPosition(position.Line, position.Column, value);
		}
	}

	public bool IsInVirtualSpace
	{
		get
		{
			ValidateVisualColumn();
			return isInVirtualSpace;
		}
	}

	public int Offset
	{
		get
		{
			return textArea.Document?.GetOffset(position.Location) ?? 0;
		}
		set
		{
			TextDocument document = textArea.Document;
			if (document != null)
			{
				Position = new TextViewPosition(document.GetLocation(value));
				DesiredXPos = double.NaN;
			}
		}
	}

	public double DesiredXPos
	{
		get
		{
			return desiredXPos;
		}
		set
		{
			desiredXPos = value;
		}
	}

	public Brush CaretBrush
	{
		get
		{
			return caretAdorner.CaretBrush;
		}
		set
		{
			caretAdorner.CaretBrush = value;
		}
	}

	public event EventHandler PositionChanged;

	internal Caret(TextArea textArea)
	{
		this.textArea = textArea;
		textView = textArea.TextView;
		position = new TextViewPosition(1, 1, 0);
		caretAdorner = new CaretLayer(textArea);
		textView.InsertLayer(caretAdorner, KnownLayer.Caret, LayerInsertionPosition.Replace);
		textView.VisualLinesChanged += TextView_VisualLinesChanged;
		textView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
	}

	internal void UpdateIfVisible()
	{
		if (visible)
		{
			Show();
		}
	}

	private void TextView_VisualLinesChanged(object sender, EventArgs e)
	{
		if (visible)
		{
			Show();
		}
		InvalidateVisualColumn();
	}

	private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
	{
		if (caretAdorner != null)
		{
			caretAdorner.InvalidateVisual();
		}
	}

	internal void OnDocumentChanging()
	{
		storedCaretOffset = Offset;
		InvalidateVisualColumn();
	}

	internal void OnDocumentChanged(DocumentChangeEventArgs e)
	{
		InvalidateVisualColumn();
		if (storedCaretOffset >= 0)
		{
			int offset = e.GetNewOffset(movementType: (!textArea.Selection.IsEmpty && storedCaretOffset == textArea.Selection.SurroundingSegment.EndOffset) ? AnchorMovementType.BeforeInsertion : AnchorMovementType.Default, offset: storedCaretOffset);
			TextDocument document = textArea.Document;
			if (document != null)
			{
				Position = new TextViewPosition(document.GetLocation(offset), position.VisualColumn);
			}
		}
		storedCaretOffset = -1;
	}

	private void ValidatePosition()
	{
		if (position.Line < 1)
		{
			position.Line = 1;
		}
		if (position.Column < 1)
		{
			position.Column = 1;
		}
		if (position.VisualColumn < -1)
		{
			position.VisualColumn = -1;
		}
		TextDocument document = textArea.Document;
		if (document == null)
		{
			return;
		}
		if (position.Line > document.LineCount)
		{
			position.Line = document.LineCount;
			position.Column = document.GetLineByNumber(position.Line).Length + 1;
			position.VisualColumn = -1;
			return;
		}
		DocumentLine lineByNumber = document.GetLineByNumber(position.Line);
		if (position.Column > lineByNumber.Length + 1)
		{
			position.Column = lineByNumber.Length + 1;
			position.VisualColumn = -1;
		}
	}

	private void RaisePositionChanged()
	{
		if (textArea.Document != null && textArea.Document.IsInUpdate)
		{
			raisePositionChangedOnUpdateFinished = true;
		}
		else if (this.PositionChanged != null)
		{
			this.PositionChanged(this, EventArgs.Empty);
		}
	}

	internal void OnDocumentUpdateFinished()
	{
		if (raisePositionChangedOnUpdateFinished && this.PositionChanged != null)
		{
			this.PositionChanged(this, EventArgs.Empty);
		}
	}

	private void ValidateVisualColumn()
	{
		if (!visualColumnValid)
		{
			TextDocument document = textArea.Document;
			if (document != null)
			{
				DocumentLine lineByNumber = document.GetLineByNumber(position.Line);
				RevalidateVisualColumn(textView.GetOrConstructVisualLine(lineByNumber));
			}
		}
	}

	private void InvalidateVisualColumn()
	{
		visualColumnValid = false;
	}

	private void RevalidateVisualColumn(VisualLine visualLine)
	{
		if (visualLine == null)
		{
			throw new ArgumentNullException("visualLine");
		}
		visualColumnValid = true;
		int offset = textView.Document.GetOffset(position.Location);
		int offset2 = visualLine.FirstDocumentLine.Offset;
		position.VisualColumn = visualLine.ValidateVisualColumn(position, textArea.Selection.EnableVirtualSpace);
		int nextCaretPosition = visualLine.GetNextCaretPosition(position.VisualColumn - 1, LogicalDirection.Forward, CaretPositioningMode.Normal, textArea.Selection.EnableVirtualSpace);
		if (nextCaretPosition != position.VisualColumn)
		{
			int nextCaretPosition2 = visualLine.GetNextCaretPosition(position.VisualColumn + 1, LogicalDirection.Backward, CaretPositioningMode.Normal, textArea.Selection.EnableVirtualSpace);
			if (nextCaretPosition < 0 && nextCaretPosition2 < 0)
			{
				throw ThrowUtil.NoValidCaretPosition();
			}
			int num = ((nextCaretPosition < 0) ? (-1) : (visualLine.GetRelativeOffset(nextCaretPosition) + offset2));
			int num2 = ((nextCaretPosition2 < 0) ? (-1) : (visualLine.GetRelativeOffset(nextCaretPosition2) + offset2));
			int visualColumn;
			int offset3;
			if (nextCaretPosition < 0)
			{
				visualColumn = nextCaretPosition2;
				offset3 = num2;
			}
			else if (nextCaretPosition2 < 0)
			{
				visualColumn = nextCaretPosition;
				offset3 = num;
			}
			else if (Math.Abs(num2 - offset) < Math.Abs(num - offset))
			{
				visualColumn = nextCaretPosition2;
				offset3 = num2;
			}
			else
			{
				visualColumn = nextCaretPosition;
				offset3 = num;
			}
			Position = new TextViewPosition(textView.Document.GetLocation(offset3), visualColumn);
		}
		isInVirtualSpace = position.VisualColumn > visualLine.VisualLength;
	}

	private Rect CalcCaretRectangle(VisualLine visualLine)
	{
		if (!visualColumnValid)
		{
			RevalidateVisualColumn(visualLine);
		}
		TextLine textLine = visualLine.GetTextLine(position.VisualColumn, position.IsAtEndOfLine);
		double textLineVisualXPosition = visualLine.GetTextLineVisualXPosition(textLine, position.VisualColumn);
		double textLineVisualYPosition = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextTop);
		return new Rect(height: visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextBottom) - textLineVisualYPosition, x: textLineVisualXPosition, y: textLineVisualYPosition, width: SystemParameters.CaretWidth);
	}

	private Rect CalcCaretOverstrikeRectangle(VisualLine visualLine)
	{
		if (!visualColumnValid)
		{
			RevalidateVisualColumn(visualLine);
		}
		int visualColumn = position.VisualColumn;
		int nextCaretPosition = visualLine.GetNextCaretPosition(visualColumn, LogicalDirection.Forward, CaretPositioningMode.Normal, allowVirtualSpace: true);
		TextLine textLine = visualLine.GetTextLine(visualColumn);
		Rect result;
		if (visualColumn < visualLine.VisualLength)
		{
			result = textLine.GetTextBounds(visualColumn, nextCaretPosition - visualColumn)[0].Rectangle;
			result.Y += visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.LineTop);
		}
		else
		{
			double textLineVisualXPosition = visualLine.GetTextLineVisualXPosition(textLine, visualColumn);
			double textLineVisualXPosition2 = visualLine.GetTextLineVisualXPosition(textLine, nextCaretPosition);
			double textLineVisualYPosition = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextTop);
			double textLineVisualYPosition2 = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextBottom);
			result = new Rect(textLineVisualXPosition, textLineVisualYPosition, textLineVisualXPosition2 - textLineVisualXPosition, textLineVisualYPosition2 - textLineVisualYPosition);
		}
		if (result.Width < SystemParameters.CaretWidth)
		{
			result.Width = SystemParameters.CaretWidth;
		}
		return result;
	}

	public Rect CalculateCaretRectangle()
	{
		if (textView != null && textView.Document != null)
		{
			VisualLine orConstructVisualLine = textView.GetOrConstructVisualLine(textView.Document.GetLineByNumber(position.Line));
			if (!textArea.OverstrikeMode)
			{
				return CalcCaretRectangle(orConstructVisualLine);
			}
			return CalcCaretOverstrikeRectangle(orConstructVisualLine);
		}
		return Rect.Empty;
	}

	public void BringCaretToView()
	{
		BringCaretToView(30.0);
	}

	internal void BringCaretToView(double border)
	{
		Rect rectangle = CalculateCaretRectangle();
		if (!rectangle.IsEmpty)
		{
			rectangle.Inflate(border, border);
			textView.MakeVisible(rectangle);
		}
	}

	public void Show()
	{
		visible = true;
		if (!showScheduled)
		{
			showScheduled = true;
			textArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(ShowInternal));
		}
	}

	private void ShowInternal()
	{
		showScheduled = false;
		if (!visible || caretAdorner == null || textView == null)
		{
			return;
		}
		VisualLine visualLine = textView.GetVisualLine(position.Line);
		if (visualLine != null)
		{
			Rect caretRectangle = (textArea.OverstrikeMode ? CalcCaretOverstrikeRectangle(visualLine) : CalcCaretRectangle(visualLine));
			if (!hasWin32Caret)
			{
				hasWin32Caret = Win32.CreateCaret(textView, caretRectangle.Size);
			}
			if (hasWin32Caret)
			{
				Win32.SetCaretPosition(textView, caretRectangle.Location - textView.ScrollOffset);
			}
			caretAdorner.Show(caretRectangle);
			textArea.ime.UpdateCompositionWindow();
		}
		else
		{
			caretAdorner.Hide();
		}
	}

	public void Hide()
	{
		visible = false;
		if (hasWin32Caret)
		{
			Win32.DestroyCaret();
			hasWin32Caret = false;
		}
		if (caretAdorner != null)
		{
			caretAdorner.Hide();
		}
	}

	[Conditional("DEBUG")]
	private static void Log(string text)
	{
	}
}
