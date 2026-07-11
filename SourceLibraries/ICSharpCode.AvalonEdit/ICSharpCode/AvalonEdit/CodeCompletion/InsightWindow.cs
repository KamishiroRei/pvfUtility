using System;
using System.Windows;
using System.Windows.Forms;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class InsightWindow : CompletionWindowBase
{
	public bool CloseAutomatically { get; set; }

	protected override bool CloseOnFocusLost => CloseAutomatically;

	static InsightWindow()
	{
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(InsightWindow), new FrameworkPropertyMetadata(typeof(InsightWindow)));
		Window.AllowsTransparencyProperty.OverrideMetadata(typeof(InsightWindow), new FrameworkPropertyMetadata(Boxes.True));
	}

	public InsightWindow(TextArea textArea)
		: base(textArea)
	{
		CloseAutomatically = true;
		AttachEvents();
	}

	protected override void OnSourceInitialized(EventArgs e)
	{
		Rect rect = base.TextArea.Caret.CalculateCaretRectangle();
		Rect rect2 = Screen.FromPoint(base.TextArea.TextView.PointToScreen(rect.Location - base.TextArea.TextView.ScrollOffset).ToSystemDrawing()).WorkingArea.ToWpf().TransformFromDevice(this);
		base.MaxHeight = rect2.Height;
		base.MaxWidth = Math.Min(rect2.Width, Math.Max(1000.0, rect2.Width * 0.6));
		base.OnSourceInitialized(e);
	}

	private void AttachEvents()
	{
		base.TextArea.Caret.PositionChanged += CaretPositionChanged;
	}

	protected override void DetachEvents()
	{
		base.TextArea.Caret.PositionChanged -= CaretPositionChanged;
		base.DetachEvents();
	}

	private void CaretPositionChanged(object sender, EventArgs e)
	{
		if (CloseAutomatically)
		{
			int offset = base.TextArea.Caret.Offset;
			if (offset < base.StartOffset || offset > base.EndOffset)
			{
				Close();
			}
		}
	}
}
