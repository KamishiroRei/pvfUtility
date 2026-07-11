using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

	private bool BQBCVP6JgQ;

	public VeiwSearchPvf()
	{
		base.DataContext = AppCore.ViewModelBase.SearchResultViewModel;
		AppCore.ViewModelBase.SearchResultViewModel.SearchUiViewModel.CloseAction = base.Close;
		InitializeComponent();
	}

	private void ucBCUX3FhX(object P_0, RoutedEventArgs P_1)
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

	private void rFgCcrTEb4(object P_0, RoutedEventArgs P_1)
	{
		comboBoxEditFileTypesString.EditValue = null;
	}

	private void A5WC8ZbLSK(object P_0, KeyEventArgs P_1)
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
		if (!BQBCVP6JgQ)
		{
			BQBCVP6JgQ = true;
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
			((VeiwSearchPvf)target).Loaded += ucBCUX3FhX;
			break;
		case 2:
			gridMain = (Grid)target;
			break;
		case 3:
			textKeyword = (AutoSuggestEdit)target;
			textKeyword.PreviewKeyDown += A5WC8ZbLSK;
			break;
		case 4:
			radioIsFindAllPath = (RadioButton)target;
			break;
		case 5:
			comboBoxEditFileTypesString = (ComboBoxEdit)target;
			break;
		case 6:
			((ButtonInfo)target).Click += rFgCcrTEb4;
			break;
		case 7:
			btnStartSearch = (Button)target;
			break;
		default:
			BQBCVP6JgQ = true;
			break;
		}
	}

	[CompilerGenerated]
	private void np3CMCu36M()
	{
		textKeyword.Focus();
	}
}
