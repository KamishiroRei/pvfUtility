using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

public class LineNumberMargin : AbstractMargin, IWeakEventListener
{
	private TextArea textArea;

	protected Typeface typeface;

	protected double emSize;

	protected int maxLineNumberLength = 1;

	private AnchorSegment selectionStart;

	private bool selecting;

	public LineNumberMargin()
	{
		base.FlowDirection = FlowDirection.LeftToRight;
	}

	static LineNumberMargin()
	{
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LineNumberMargin), new FrameworkPropertyMetadata(typeof(LineNumberMargin)));
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		typeface = this.CreateTypeface();
		emSize = (double)GetValue(TextBlock.FontSizeProperty);
		return new Size(TextFormatterFactory.CreateFormattedText(this, new string('9', maxLineNumberLength), typeface, emSize, (Brush)GetValue(Control.ForegroundProperty)).Width, 0.0);
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		TextView textView = base.TextView;
		Size renderSize = base.RenderSize;
		if (textView == null || !textView.VisualLinesValid)
		{
			return;
		}
		Brush foreground = (Brush)GetValue(Control.ForegroundProperty);
		foreach (VisualLine visualLine in textView.VisualLines)
		{
			FormattedText formattedText = TextFormatterFactory.CreateFormattedText(this, visualLine.FirstDocumentLine.LineNumber.ToString(CultureInfo.CurrentCulture), typeface, emSize, foreground);
			double textLineVisualYPosition = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.TextTop);
			drawingContext.DrawText(formattedText, new Point(renderSize.Width - formattedText.Width, textLineVisualYPosition - textView.VerticalOffset));
		}
	}

	protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
	{
		if (oldTextView != null)
		{
			oldTextView.VisualLinesChanged -= TextViewVisualLinesChanged;
		}
		base.OnTextViewChanged(oldTextView, newTextView);
		if (newTextView != null)
		{
			newTextView.VisualLinesChanged += TextViewVisualLinesChanged;
			textArea = newTextView.GetService(typeof(TextArea)) as TextArea;
		}
		else
		{
			textArea = null;
		}
		InvalidateVisual();
	}

	protected override void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
	{
		if (oldDocument != null)
		{
			PropertyChangedEventManager.RemoveListener(oldDocument, this, "LineCount");
		}
		base.OnDocumentChanged(oldDocument, newDocument);
		if (newDocument != null)
		{
			PropertyChangedEventManager.AddListener(newDocument, this, "LineCount");
		}
		OnDocumentLineCountChanged();
	}

	protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(PropertyChangedEventManager))
		{
			OnDocumentLineCountChanged();
			return true;
		}
		return false;
	}

	bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		return ReceiveWeakEvent(managerType, sender, e);
	}

	private void OnDocumentLineCountChanged()
	{
		int num = ((base.Document == null) ? 1 : base.Document.LineCount).ToString(CultureInfo.CurrentCulture).Length;
		if (num < 2)
		{
			num = 2;
		}
		if (num != maxLineNumberLength)
		{
			maxLineNumberLength = num;
			InvalidateMeasure();
		}
	}

	private void TextViewVisualLinesChanged(object sender, EventArgs e)
	{
		InvalidateVisual();
	}

	protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
	{
		base.OnMouseLeftButtonDown(e);
		if (e.Handled || base.TextView == null || textArea == null)
		{
			return;
		}
		e.Handled = true;
		textArea.Focus();
		SimpleSegment textLineSegment = GetTextLineSegment(e);
		if (textLineSegment == SimpleSegment.Invalid)
		{
			return;
		}
		textArea.Caret.Offset = textLineSegment.Offset + textLineSegment.Length;
		if (CaptureMouse())
		{
			selecting = true;
			selectionStart = new AnchorSegment(base.Document, textLineSegment.Offset, textLineSegment.Length);
			if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift && textArea.Selection is SimpleSelection simpleSelection)
			{
				selectionStart = new AnchorSegment(base.Document, simpleSelection.SurroundingSegment);
			}
			textArea.Selection = Selection.Create(textArea, selectionStart);
			if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
			{
				ExtendSelection(textLineSegment);
			}
			textArea.Caret.BringCaretToView(5.0);
		}
	}

	private SimpleSegment GetTextLineSegment(MouseEventArgs e)
	{
		Point position = e.GetPosition(base.TextView);
		position.X = 0.0;
		position.Y = position.Y.CoerceValue(0.0, base.TextView.ActualHeight);
		position.Y += base.TextView.VerticalOffset;
		VisualLine visualLineFromVisualTop = base.TextView.GetVisualLineFromVisualTop(position.Y);
		if (visualLineFromVisualTop == null)
		{
			return SimpleSegment.Invalid;
		}
		TextLine textLineByVisualYPosition = visualLineFromVisualTop.GetTextLineByVisualYPosition(position.Y);
		int textLineVisualStartColumn = visualLineFromVisualTop.GetTextLineVisualStartColumn(textLineByVisualYPosition);
		int visualColumn = textLineVisualStartColumn + textLineByVisualYPosition.Length;
		int offset = visualLineFromVisualTop.FirstDocumentLine.Offset;
		int num = visualLineFromVisualTop.GetRelativeOffset(textLineVisualStartColumn) + offset;
		int num2 = visualLineFromVisualTop.GetRelativeOffset(visualColumn) + offset;
		if (num2 == visualLineFromVisualTop.LastDocumentLine.Offset + visualLineFromVisualTop.LastDocumentLine.Length)
		{
			num2 += visualLineFromVisualTop.LastDocumentLine.DelimiterLength;
		}
		return new SimpleSegment(num, num2 - num);
	}

	private void ExtendSelection(SimpleSegment currentSeg)
	{
		if (currentSeg.Offset < selectionStart.Offset)
		{
			textArea.Caret.Offset = currentSeg.Offset;
			textArea.Selection = Selection.Create(textArea, currentSeg.Offset, selectionStart.Offset + selectionStart.Length);
		}
		else
		{
			textArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
			textArea.Selection = Selection.Create(textArea, selectionStart.Offset, currentSeg.Offset + currentSeg.Length);
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (selecting && textArea != null && base.TextView != null)
		{
			e.Handled = true;
			SimpleSegment textLineSegment = GetTextLineSegment(e);
			if (textLineSegment == SimpleSegment.Invalid)
			{
				return;
			}
			ExtendSelection(textLineSegment);
			textArea.Caret.BringCaretToView(5.0);
		}
		base.OnMouseMove(e);
	}

	protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
	{
		if (selecting)
		{
			selecting = false;
			selectionStart = null;
			ReleaseMouseCapture();
			e.Handled = true;
		}
		base.OnMouseLeftButtonUp(e);
	}

	protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
	{
		return new PointHitTestResult(this, hitTestParameters.HitPoint);
	}
}
