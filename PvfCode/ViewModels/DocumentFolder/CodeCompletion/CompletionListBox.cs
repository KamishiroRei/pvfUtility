using System;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Utils;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class CompletionListBox : ListBox
{
	internal ScrollViewer L5uik30Kqg;

	public int FirstVisibleItem
	{
		get
		{
			if (L5uik30Kqg == null || L5uik30Kqg.ExtentHeight == 0.0)
			{
				return 0;
			}
			return (int)((double)base.Items.Count * L5uik30Kqg.VerticalOffset / L5uik30Kqg.ExtentHeight);
		}
		set
		{
			value = value.CoerceValue(0, base.Items.Count - VisibleItemCount);
			if (L5uik30Kqg != null)
			{
				L5uik30Kqg.ScrollToVerticalOffset((double)value / (double)base.Items.Count * L5uik30Kqg.ExtentHeight);
			}
		}
	}

	public int VisibleItemCount
	{
		get
		{
			if (L5uik30Kqg == null || L5uik30Kqg.ExtentHeight == 0.0)
			{
				return 10;
			}
			return Math.Max(3, (int)Math.Ceiling((double)base.Items.Count * L5uik30Kqg.ViewportHeight / L5uik30Kqg.ExtentHeight));
		}
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		try
		{
			L5uik30Kqg = null;
			if (VisualChildrenCount > 0 && GetVisualChild(0) is Border border)
			{
				L5uik30Kqg = border.Child as ScrollViewer;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionListBox.OnApplyTemplate");
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

	public CompletionListBox()
	{
	}
}
