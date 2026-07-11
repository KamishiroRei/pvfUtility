using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;

namespace PvfCode.Views.Macro;

public class MacroTreeView : TreeListControl, IComponentConnector
{
	internal TreeListColumn columnKey;

	internal TreeListView treeListView;

	private bool cinhC4rGT6;

	public MacroTreeView()
	{
		InitializeComponent();
		base.Loaded += rK8hljF6Tv;
	}

	private void rK8hljF6Tv(object P_0, RoutedEventArgs P_1)
	{
		treeListView.ExpandAllNodes();
	}

	private void Ju3hjhJJnG(object P_0, ItemClickEventArgs P_1)
	{
		treeListView.ExpandAllNodes();
	}

	private void WCqhTaHDJx(object P_0, ItemClickEventArgs P_1)
	{
		treeListView.CollapseAllNodes();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!cinhC4rGT6)
		{
			cinhC4rGT6 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/macro/macrotreeview.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			columnKey = (TreeListColumn)target;
			break;
		case 2:
			treeListView = (TreeListView)target;
			break;
		case 3:
			((BarButtonItem)target).ItemClick += Ju3hjhJJnG;
			break;
		case 4:
			((BarButtonItem)target).ItemClick += WCqhTaHDJx;
			break;
		default:
			cinhC4rGT6 = true;
			break;
		}
	}
}
