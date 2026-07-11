using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using Collections.Pooled;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewReNmaeNodes : ThemedWindow, IComponentConnector
{
	internal ViewReNmaeNodes win;

	internal TreeListControl tree;

	internal TreeListColumn columnKey;

	internal CustomTreeListView treeListView;

	private bool A4nH6PRvXa;

	public ViewReNmaeNodes(PooledList<string> fileList)
	{
		base.DataContext = new ViewReNmaeNodesViewMode(fileList);
		InitializeComponent();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		Application.Current.MainWindow.Activate();
	}

	private void View_CustomColumnSort(object sender, TreeListCustomColumnSortEventArgs e)
	{
		KeyValuePair<string, PvfTreeFileBase> keyValuePair = (KeyValuePair<string, PvfTreeFileBase>)e.Node1.Content;
		KeyValuePair<string, PvfTreeFileBase> keyValuePair2 = (KeyValuePair<string, PvfTreeFileBase>)e.Node2.Content;
		if (keyValuePair.Value.IsFile != keyValuePair2.Value.IsFile)
		{
			e.Result = (keyValuePair.Value.IsFile ? 1 : (-1));
			e.Handled = true;
		}
	}

	private void treeListView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
	{
		if (tree.View.FocusedNode != null)
		{
			tree.View.FocusedNode.IsExpanded = !tree.View.FocusedNode.IsExpanded;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!A4nH6PRvXa)
		{
			A4nH6PRvXa = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/pvftreefolder/viewrenmaenodes.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			win = (ViewReNmaeNodes)target;
			break;
		case 2:
			tree = (TreeListControl)target;
			break;
		case 3:
			columnKey = (TreeListColumn)target;
			break;
		case 4:
			treeListView = (CustomTreeListView)target;
			break;
		default:
			A4nH6PRvXa = true;
			break;
		}
	}
}
