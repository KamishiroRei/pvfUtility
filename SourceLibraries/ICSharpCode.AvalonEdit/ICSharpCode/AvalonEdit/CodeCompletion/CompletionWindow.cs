using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class CompletionWindow : CompletionWindowBase
{
	private readonly CompletionList completionList = new CompletionList();

	private ToolTip toolTip = new ToolTip();

	public CompletionList CompletionList => completionList;

	public bool CloseAutomatically { get; set; }

	protected override bool CloseOnFocusLost => CloseAutomatically;

	public bool CloseWhenCaretAtBeginning { get; set; }

	public CompletionWindow(TextArea textArea)
		: base(textArea)
	{
		CloseAutomatically = true;
		base.SizeToContent = SizeToContent.Height;
		base.MaxHeight = 300.0;
		base.Width = 175.0;
		base.Content = completionList;
		base.MinHeight = 15.0;
		base.MinWidth = 30.0;
		toolTip.PlacementTarget = this;
		toolTip.Placement = PlacementMode.Right;
		toolTip.Closed += toolTip_Closed;
		AttachEvents();
	}

	private void toolTip_Closed(object sender, RoutedEventArgs e)
	{
		if (toolTip != null)
		{
			toolTip.Content = null;
		}
	}

	private void completionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ICompletionData selectedItem = completionList.SelectedItem;
		if (selectedItem == null)
		{
			return;
		}
		object description = selectedItem.Description;
		if (description != null)
		{
			if (description is string text)
			{
				toolTip.Content = new TextBlock
				{
					Text = text,
					TextWrapping = TextWrapping.Wrap
				};
			}
			else
			{
				toolTip.Content = description;
			}
			toolTip.IsOpen = true;
		}
		else
		{
			toolTip.IsOpen = false;
		}
	}

	private void completionList_InsertionRequested(object sender, EventArgs e)
	{
		Close();
		completionList.SelectedItem?.Complete(base.TextArea, new AnchorSegment(base.TextArea.Document, base.StartOffset, base.EndOffset - base.StartOffset), e);
	}

	private void AttachEvents()
	{
		completionList.InsertionRequested += completionList_InsertionRequested;
		completionList.SelectionChanged += completionList_SelectionChanged;
		base.TextArea.Caret.PositionChanged += CaretPositionChanged;
		base.TextArea.MouseWheel += textArea_MouseWheel;
		base.TextArea.PreviewTextInput += textArea_PreviewTextInput;
	}

	protected override void DetachEvents()
	{
		completionList.InsertionRequested -= completionList_InsertionRequested;
		completionList.SelectionChanged -= completionList_SelectionChanged;
		base.TextArea.Caret.PositionChanged -= CaretPositionChanged;
		base.TextArea.MouseWheel -= textArea_MouseWheel;
		base.TextArea.PreviewTextInput -= textArea_PreviewTextInput;
		base.DetachEvents();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		if (toolTip != null)
		{
			toolTip.IsOpen = false;
			toolTip = null;
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled)
		{
			completionList.HandleKey(e);
		}
	}

	private void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		e.Handled = CompletionWindowBase.RaiseEventPair(this, UIElement.PreviewTextInputEvent, UIElement.TextInputEvent, new TextCompositionEventArgs(e.Device, e.TextComposition));
	}

	private void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
	{
		e.Handled = CompletionWindowBase.RaiseEventPair(GetScrollEventTarget(), UIElement.PreviewMouseWheelEvent, UIElement.MouseWheelEvent, new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
	}

	private UIElement GetScrollEventTarget()
	{
		if (completionList == null)
		{
			return this;
		}
		return (UIElement)(completionList.ScrollViewer ?? ((object)completionList.ListBox) ?? ((object)completionList));
	}

	private void CaretPositionChanged(object sender, EventArgs e)
	{
		int offset = base.TextArea.Caret.Offset;
		if (offset == base.StartOffset)
		{
			if (CloseAutomatically && CloseWhenCaretAtBeginning)
			{
				Close();
			}
			else
			{
				completionList.SelectItem(string.Empty);
			}
			return;
		}
		if (offset < base.StartOffset || offset > base.EndOffset)
		{
			if (CloseAutomatically)
			{
				Close();
			}
			return;
		}
		TextDocument textDocument = base.TextArea.Document;
		if (textDocument != null)
		{
			completionList.SelectItem(textDocument.GetText(base.StartOffset, offset - base.StartOffset));
		}
	}
}
