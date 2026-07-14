using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels;

namespace PvfCode.Controls.TextEditorFolder;

public class SearchPanel : UserControl, IComponentConnector
{
	public static readonly DependencyProperty MaxWidthValueProperty;

	internal SearchPanel searchControl;

	internal Grid gridMain;

	internal SimpleButton switchReplace;

	internal AutoSuggestEdit findKeywordText;

	internal BarSplitButtonItem searchButton;

	internal BarButtonItem btnFindNext;

	internal AutoSuggestEdit replaceKeywordTextBox;

	internal Rectangle bottom;

	private bool contentLoaded;

	public double MaxWidthValue
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MaxWidthValueProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaxWidthValueProperty, (object)value);
		}
	}

	public SearchPanel()
	{
		InitializeComponent();
	}

	private void OnPaste(object sender, DataObjectPastingEventArgs e)
	{
		if (!e.DataObject.GetDataPresent(typeof(string)))
		{
			return;
		}
		string text = (string)e.DataObject.GetData(typeof(string));
		if (string.IsNullOrEmpty(text) || !text.Contains("\r\n") || !(sender is AutoSuggestEdit))
		{
			return;
		}
		AutoSuggestEdit autoSuggestEdit = (AutoSuggestEdit)sender;
		if (autoSuggestEdit.SelectionLength > 0)
		{
			SearchViewModel searchViewModel = (SearchViewModel)base.DataContext;
			if (autoSuggestEdit.Name == "findKeywordText")
			{
				searchViewModel.Config.FindKeyword = ReplaceAt(searchViewModel.Config.FindKeyword, autoSuggestEdit.SelectionStart, autoSuggestEdit.SelectionLength, text);
			}
			else
			{
				searchViewModel.Config.ReplaceKeyword = ReplaceAt(searchViewModel.Config.ReplaceKeyword, autoSuggestEdit.SelectionStart, autoSuggestEdit.SelectionLength, text);
			}
		}
		else
		{
			autoSuggestEdit.Text = autoSuggestEdit.Text.Insert(autoSuggestEdit.CaretIndex, text);
		}
		e.CancelCommand();
	}

	public static string ReplaceAt(string str, int index, int length, string replace)
	{
		return string.Create(str.Length - length + replace.Length, (str, index, length, replace), delegate(Span<char> span, (string str, int index, int length, string replace) state)
		{
			ReadOnlySpan<char> readOnlySpan = state.str.AsSpan().Slice(0, state.index);
			readOnlySpan.CopyTo(span);
			readOnlySpan = state.replace.AsSpan();
			readOnlySpan.CopyTo(span.Slice(state.index));
			readOnlySpan = state.str.AsSpan();
			readOnlySpan.Slice(state.index + state.length).CopyTo(span.Slice(state.index + state.replace.Length));
		});
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		SearchViewModel searchViewModel = (SearchViewModel)base.DataContext;
		if (searchViewModel != null)
		{
			searchViewModel.SetFindKeywordFocused = FocusFindKeyword;
			DataObject.AddPastingHandler(findKeywordText, OnPaste);
			DataObject.AddPastingHandler(replaceKeywordTextBox, OnPaste);
		}
	}

	private void FocusFindKeyword()
	{
		Dispatcher.BeginInvoke((Action)(() => findKeywordText.Focus()));
	}

	private void OnFindKeywordKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
		{
			SearchViewModel obj = (SearchViewModel)base.DataContext;
			obj.OnFindMain();
			e.Handled = true;
		}
	}

	private void OnReplaceKeywordKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
		{
			((SearchViewModel)base.DataContext).OnReplace(isReplaceAll: false);
			e.Handled = true;
		}
	}

	private void OnResizeDragDelta(object sender, DragDeltaEventArgs e)
	{
		Grid grid = gridMain;
		FrameworkElement frameworkElement = sender as FrameworkElement;
		if (frameworkElement.HorizontalAlignment == HorizontalAlignment.Left)
		{
			double num = (double.IsNaN(grid.Width) ? grid.ActualWidth : grid.Width) - e.HorizontalChange;
			if (num > MaxWidthValue - 32.0)
			{
				num = MaxWidthValue - 32.0;
			}
			if (num < 300.0)
			{
				num = 300.0;
			}
			if (frameworkElement.HorizontalAlignment != HorizontalAlignment.Center && frameworkElement.HorizontalAlignment != HorizontalAlignment.Stretch && num >= 0.0)
			{
				grid.Width = num;
			}
		}
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
		DataObject.RemovePastingHandler(findKeywordText, OnPaste);
		DataObject.RemovePastingHandler(replaceKeywordTextBox, OnPaste);
	}

	private void OnKeywordPreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key != Key.Enter)
		{
			AutoSuggestEdit autoSuggestEdit = (AutoSuggestEdit)sender;
			if (!string.IsNullOrEmpty(autoSuggestEdit.SelectedText) && autoSuggestEdit.SelectedText.Contains("\r\n") && autoSuggestEdit.SelectedText == autoSuggestEdit.Text)
			{
				autoSuggestEdit.Text = string.Empty;
			}
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!contentLoaded)
		{
			contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/texteditorfolder/searchpanel.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			searchControl = (SearchPanel)target;
			searchControl.Loaded += OnLoaded;
			searchControl.Unloaded += OnUnloaded;
			break;
		case 2:
			gridMain = (Grid)target;
			break;
		case 3:
			switchReplace = (SimpleButton)target;
			break;
		case 4:
			findKeywordText = (AutoSuggestEdit)target;
			findKeywordText.KeyDown += OnFindKeywordKeyDown;
			findKeywordText.PreviewKeyDown += OnKeywordPreviewKeyDown;
			break;
		case 5:
			searchButton = (BarSplitButtonItem)target;
			break;
		case 6:
			btnFindNext = (BarButtonItem)target;
			break;
		case 7:
			replaceKeywordTextBox = (AutoSuggestEdit)target;
			replaceKeywordTextBox.KeyDown += OnReplaceKeywordKeyDown;
			replaceKeywordTextBox.PreviewKeyDown += OnKeywordPreviewKeyDown;
			break;
		case 8:
			((Thumb)target).DragDelta += OnResizeDragDelta;
			break;
		case 9:
			bottom = (Rectangle)target;
			break;
		default:
			contentLoaded = true;
			break;
		}
	}

	static SearchPanel()
	{
		MaxWidthValueProperty = DependencyProperty.Register("MaxWidthValue", typeof(double), typeof(SearchPanel), new PropertyMetadata((object)0.0));
	}
}
