using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class CaretLayer : Layer
{
	private TextArea textArea;

	private bool isVisible;

	private Rect caretRectangle;

	private DispatcherTimer caretBlinkTimer = new DispatcherTimer();

	private bool blink;

	internal Brush CaretBrush;

	public CaretLayer(TextArea textArea)
		: base(textArea.TextView, KnownLayer.Caret)
	{
		this.textArea = textArea;
		base.IsHitTestVisible = false;
		caretBlinkTimer.Tick += caretBlinkTimer_Tick;
	}

	private void caretBlinkTimer_Tick(object sender, EventArgs e)
	{
		blink = !blink;
		InvalidateVisual();
	}

	public void Show(Rect caretRectangle)
	{
		this.caretRectangle = caretRectangle;
		isVisible = true;
		StartBlinkAnimation();
		InvalidateVisual();
	}

	public void Hide()
	{
		if (isVisible)
		{
			isVisible = false;
			StopBlinkAnimation();
			InvalidateVisual();
		}
	}

	private void StartBlinkAnimation()
	{
		TimeSpan caretBlinkTime = Win32.CaretBlinkTime;
		blink = true;
		if (caretBlinkTime.TotalMilliseconds > 0.0)
		{
			caretBlinkTimer.Interval = caretBlinkTime;
			caretBlinkTimer.Start();
		}
	}

	private void StopBlinkAnimation()
	{
		caretBlinkTimer.Stop();
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		base.OnRender(drawingContext);
		if (isVisible && blink)
		{
			Brush brush = CaretBrush;
			if (brush == null)
			{
				brush = (Brush)textView.GetValue(TextBlock.ForegroundProperty);
			}
			if (textArea.OverstrikeMode && brush is SolidColorBrush { Color: var color })
			{
				brush = new SolidColorBrush(Color.FromArgb(100, color.R, color.G, color.B));
				brush.Freeze();
			}
			Rect rect = new Rect(caretRectangle.X - textView.HorizontalOffset, caretRectangle.Y - textView.VerticalOffset, caretRectangle.Width, caretRectangle.Height);
			drawingContext.DrawRectangle(brush, null, PixelSnapHelpers.Round(rect, PixelSnapHelpers.GetPixelSize(this)));
		}
	}
}
