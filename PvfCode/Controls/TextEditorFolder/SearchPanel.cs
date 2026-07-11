using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

	private bool giJgumVlkN;

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

	private void AAEg2yKpEX(object P_0, DataObjectPastingEventArgs P_1)
	{
		if (!P_1.DataObject.GetDataPresent(typeof(string)))
		{
			return;
		}
		string text = (string)P_1.DataObject.GetData(typeof(string));
		if (string.IsNullOrEmpty(text) || !text.Contains("\r\n") || !(P_0 is AutoSuggestEdit))
		{
			return;
		}
		AutoSuggestEdit autoSuggestEdit = (AutoSuggestEdit)P_0;
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
		P_1.CancelCommand();
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

	private void PDigfdd8oL(object P_0, RoutedEventArgs P_1)
	{
		SearchViewModel searchViewModel = (SearchViewModel)base.DataContext;
		if (searchViewModel != null)
		{
			searchViewModel.SetFindKeywordFocused = lnog55EmaH;
			DataObject.AddPastingHandler((DependencyObject)(object)findKeywordText, AAEg2yKpEX);
			DataObject.AddPastingHandler((DependencyObject)(object)replaceKeywordTextBox, AAEg2yKpEX);
		}
	}

	private void lnog55EmaH()
	{
		((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			findKeywordText.Focus();
		}, Array.Empty<object>());
	}

	private void T22gSOJMY4(object P_0, KeyEventArgs P_1)
	{
		if ((int)P_1.Key == 6 && ((int)Keyboard.Modifiers & 2) != 2)
		{
			SearchViewModel obj = (SearchViewModel)base.DataContext;
			string.IsNullOrEmpty(obj.Config.FindKeyword);
			obj.OnFindMain();
			P_1.Handled = true;
		}
	}

	private void KJlgAHpP3d(object P_0, KeyEventArgs P_1)
	{
		if ((int)P_1.Key == 6 && ((int)Keyboard.Modifiers & 2) != 2)
		{
			((SearchViewModel)base.DataContext).OnReplace(isReplaceAll: false);
			P_1.Handled = true;
		}
	}

	private void cVog4g4w1f(object P_0, DragDeltaEventArgs P_1)
	{
		Grid grid = gridMain;
		FrameworkElement frameworkElement = P_0 as FrameworkElement;
		if (frameworkElement.HorizontalAlignment == HorizontalAlignment.Left)
		{
			_ = grid.Margin.Right;
			_ = grid.Margin.Left;
			_ = P_1.HorizontalChange;
			double num = (double.IsNaN(grid.Width) ? grid.ActualWidth : grid.Width) - P_1.HorizontalChange;
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

	private void ai2gYRCSoL(object P_0, RoutedEventArgs P_1)
	{
		DataObject.RemovePastingHandler((DependencyObject)(object)findKeywordText, AAEg2yKpEX);
		DataObject.RemovePastingHandler((DependencyObject)(object)replaceKeywordTextBox, AAEg2yKpEX);
	}

	private void awAgyj1yFm(object P_0, KeyEventArgs P_1)
	{
		if ((int)P_1.Key != 6)
		{
			AutoSuggestEdit autoSuggestEdit = (AutoSuggestEdit)P_0;
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
		if (!giJgumVlkN)
		{
			giJgumVlkN = true;
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
			searchControl.Loaded += PDigfdd8oL;
			searchControl.Unloaded += ai2gYRCSoL;
			break;
		case 2:
			gridMain = (Grid)target;
			break;
		case 3:
			switchReplace = (SimpleButton)target;
			break;
		case 4:
			findKeywordText = (AutoSuggestEdit)target;
			findKeywordText.KeyDown += T22gSOJMY4;
			findKeywordText.PreviewKeyDown += awAgyj1yFm;
			break;
		case 5:
			searchButton = (BarSplitButtonItem)target;
			break;
		case 6:
			btnFindNext = (BarButtonItem)target;
			break;
		case 7:
			replaceKeywordTextBox = (AutoSuggestEdit)target;
			replaceKeywordTextBox.KeyDown += KJlgAHpP3d;
			replaceKeywordTextBox.PreviewKeyDown += awAgyj1yFm;
			break;
		case 8:
			((Thumb)target).DragDelta += cVog4g4w1f;
			break;
		case 9:
			bottom = (Rectangle)target;
			break;
		default:
			giJgumVlkN = true;
			break;
		}
	}

	static SearchPanel()
	{
		MaxWidthValueProperty = DependencyProperty.Register("MaxWidthValue", typeof(double), typeof(SearchPanel), new PropertyMetadata((object)0.0));
	}

	[CompilerGenerated]
	private void u2KgicpFke()
	{
		findKeywordText.Focus();
	}
}
