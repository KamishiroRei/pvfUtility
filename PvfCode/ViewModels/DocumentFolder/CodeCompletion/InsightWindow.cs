using System;
using System.Windows;
using System.Windows.Forms;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;
using PvfCode.Controls.TextEditorFolder;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class InsightWindow : CompletionWindowBase
{
	public bool CloseAutomatically { get; set; }

	protected override bool CloseOnFocusLost => CloseAutomatically;

	static InsightWindow()
	{
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(InsightWindow), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)typeof(InsightWindow)));
		Window.AllowsTransparencyProperty.OverrideMetadata(typeof(InsightWindow), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Boxes.True));
	}

	public InsightWindow(TextEditorBase editorBase, TextArea textArea, int startOffset, int endOffset)
		: base(editorBase, textArea, startOffset, endOffset)
	{
		CloseAutomatically = true;
		AttachEvents();
	}

	protected override void OnSourceInitialized(EventArgs e)
	{
		Rect val = base.TextArea.Caret.CalculateCaretRectangle();
		Rect val2 = Screen.FromPoint(base.TextArea.TextView.PointToScreen(val.Location - base.TextArea.TextView.ScrollOffset).ToSystemDrawing()).WorkingArea.ToWpf().TransformFromDevice(this);
		base.MaxHeight = val2.Height;
		base.MaxWidth = Math.Min(val2.Width, Math.Max(1000.0, val2.Width * 0.6));
		base.OnSourceInitialized(e);
	}

	private void AttachEvents()
	{
		base.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
	}

	protected override void DetachEvents()
	{
		base.TextArea.Caret.PositionChanged -= OnCaretPositionChanged;
		base.DetachEvents();
	}

	private void OnCaretPositionChanged(object? sender, EventArgs e)
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
