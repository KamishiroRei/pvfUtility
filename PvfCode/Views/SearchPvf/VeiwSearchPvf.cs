using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using PvfCode.Services.SearchModel;
using PvfCode.ViewModels;

namespace PvfCode.Views.SearchPvf;

public class VeiwSearchPvf : ThemedWindow, IComponentConnector
{
	internal Grid gridMain;

	internal AutoSuggestEdit textKeyword;

	internal RadioButton radioIsFindAllPath;

	internal ComboBoxEdit comboBoxEditFileTypesString;

	internal Button btnStartSearch;

	private bool contentLoaded;

	public VeiwSearchPvf()
	{
		base.DataContext = AppCore.ViewModelBase.SearchResultViewModel;
		AppCore.ViewModelBase.SearchResultViewModel.SearchUiViewModel.CloseAction = base.Close;
		InitializeComponent();
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		if (((SearchResultTreeViewModel)base.DataContext).SearchUiViewModel.Config.Type != SearchType.ScriptContent)
		{
			((DispatcherObject)this).Dispatcher.Invoke((Action)delegate
			{
				textKeyword.Focus();
			});
		}
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		Application.Current.MainWindow.Activate();
	}

	private void OnClearFileTypesClick(object sender, RoutedEventArgs e)
	{
		comboBoxEditFileTypesString.EditValue = null;
	}

	private void OnKeywordPreviewKeyDown(object sender, KeyEventArgs e)
	{
		if ((int)e.Key != 6)
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
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/searchpvf/veiwsearchpvf.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((VeiwSearchPvf)target).Loaded += OnLoaded;
			break;
		case 2:
			gridMain = (Grid)target;
			break;
		case 3:
			textKeyword = (AutoSuggestEdit)target;
			textKeyword.PreviewKeyDown += OnKeywordPreviewKeyDown;
			break;
		case 4:
			radioIsFindAllPath = (RadioButton)target;
			break;
		case 5:
			comboBoxEditFileTypesString = (ComboBoxEdit)target;
			break;
		case 6:
			((ButtonInfo)target).Click += OnClearFileTypesClick;
			break;
		case 7:
			btnStartSearch = (Button)target;
			break;
		default:
			contentLoaded = true;
			break;
		}
	}
}
