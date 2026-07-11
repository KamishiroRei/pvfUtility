using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels.AllSearchResultDir;

namespace PvfCode.Controls.TextEditorFolder;

public class AllSearchResultView : UserControl, IComponentConnector
{
	internal AllSearchResultView use1;

	internal ToolBarControl toolbarControl;

	internal AllSearchResultTreeView treeList;

	private bool UwogCqIRcr;

	public AllSearchResultView()
	{
		InitializeComponent();
		base.Loaded += zBbgl6mMaj;
	}

	private void zBbgl6mMaj(object P_0, RoutedEventArgs P_1)
	{
		AllSearchResultViewModel allSearchResultViewModel = (AllSearchResultViewModel)base.DataContext;
		if (allSearchResultViewModel != null)
		{
			allSearchResultViewModel.ContentToNodeMethods = z2EgjlMmPd;
		}
	}

	private TreeListNode z2EgjlMmPd(object P_0)
	{
		return treeList.View.GetNodeByContent(P_0);
	}

	private void lk4gTcTQG0(object P_0, ItemClickEventArgs P_1)
	{
		((AllSearchResultViewModel)base.DataContext)?.Clear();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!UwogCqIRcr)
		{
			UwogCqIRcr = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/texteditorfolder/allsearchresultview.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			use1 = (AllSearchResultView)target;
			break;
		case 2:
			toolbarControl = (ToolBarControl)target;
			break;
		case 3:
			((BarButtonItem)target).ItemClick += lk4gTcTQG0;
			break;
		case 4:
			treeList = (AllSearchResultTreeView)target;
			break;
		default:
			UwogCqIRcr = true;
			break;
		}
	}
}
