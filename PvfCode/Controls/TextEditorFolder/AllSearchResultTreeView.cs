using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Grid;

namespace PvfCode.Controls.TextEditorFolder;

public class AllSearchResultTreeView : TreeListControl, IComponentConnector
{
	internal TreeListColumn columnKey;

	internal TreeListView treeListView;

	private bool zcFgDVFd7Z;

	public AllSearchResultTreeView()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!zcFgDVFd7Z)
		{
			zcFgDVFd7Z = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/texteditorfolder/allsearchresulttreeview.xaml", UriKind.Relative);
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
		default:
			zcFgDVFd7Z = true;
			break;
		}
	}
}
