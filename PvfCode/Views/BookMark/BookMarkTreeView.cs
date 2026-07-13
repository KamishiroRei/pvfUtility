using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;

namespace PvfCode.Views.BookMark;

public class BookMarkTreeView : TreeListControl, IComponentConnector
{
	internal BookMarkTreeView treeList;

	internal TreeListColumn columnKey;

	internal TreeListView treeListView;

	private bool _contentLoaded;

	public BookMarkTreeView()
	{
		InitializeComponent();
	}

	private void OnExpandAllClick(object sender, ItemClickEventArgs e)
	{
		treeListView.ExpandAllNodes();
	}

	private void OnCollapseAllClick(object sender, ItemClickEventArgs e)
	{
		treeListView.CollapseAllNodes();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/bookmark/bookmarktreeview.xaml", UriKind.Relative);
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
			treeList = (BookMarkTreeView)target;
			break;
		case 2:
			columnKey = (TreeListColumn)target;
			break;
		case 3:
			treeListView = (TreeListView)target;
			break;
		case 4:
			((BarButtonItem)target).ItemClick += OnExpandAllClick;
			break;
		case 5:
			((BarButtonItem)target).ItemClick += OnCollapseAllClick;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
