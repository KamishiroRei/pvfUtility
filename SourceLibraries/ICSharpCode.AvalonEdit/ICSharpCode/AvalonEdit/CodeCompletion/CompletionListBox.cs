using System;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class CompletionListBox : ListBox
{
	internal ScrollViewer scrollViewer;

	public int FirstVisibleItem
	{
		get
		{
			if (scrollViewer == null || scrollViewer.ExtentHeight == 0.0)
			{
				return 0;
			}
			return (int)((double)base.Items.Count * scrollViewer.VerticalOffset / scrollViewer.ExtentHeight);
		}
		set
		{
			value = value.CoerceValue(0, base.Items.Count - VisibleItemCount);
			if (scrollViewer != null)
			{
				scrollViewer.ScrollToVerticalOffset((double)value / (double)base.Items.Count * scrollViewer.ExtentHeight);
			}
		}
	}

	public int VisibleItemCount
	{
		get
		{
			if (scrollViewer == null || scrollViewer.ExtentHeight == 0.0)
			{
				return 10;
			}
			return Math.Max(3, (int)Math.Ceiling((double)base.Items.Count * scrollViewer.ViewportHeight / scrollViewer.ExtentHeight));
		}
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		scrollViewer = null;
		if (VisualChildrenCount > 0 && GetVisualChild(0) is Border border)
		{
			scrollViewer = border.Child as ScrollViewer;
		}
	}

	public void ClearSelection()
	{
		base.SelectedIndex = -1;
	}

	public void SelectIndex(int index)
	{
		if (index >= base.Items.Count)
		{
			index = base.Items.Count - 1;
		}
		if (index < 0)
		{
			index = 0;
		}
		base.SelectedIndex = index;
		ScrollIntoView(base.SelectedItem);
	}

	public void CenterViewOn(int index)
	{
		FirstVisibleItem = index - VisibleItemCount / 2;
	}
}
