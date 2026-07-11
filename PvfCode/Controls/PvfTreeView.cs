using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using PvfCode.ViewModels;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Controls;

public class PvfTreeView : TreeListControl, IComponentConnector
{
	internal PvfTreeView tree;

	internal TreeListColumn columnKey;

	internal CustomTreeListView treeListView;

	internal BarButtonItem importAs;

	private bool doPaZ8eneX;

	public PvfTreeView()
	{
		InitializeComponent();
	}

	private void treeListView_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (((int)Keyboard.Modifiers & 2) != 2)
		{
			_ = base.View.FocusedNode;
		}
	}

	private void ieDaEMNBik(string P_0, TreeListNode P_1)
	{
	}

	private bool JGXaOobl1T(string P_0, TreeListNode P_1, IEnumerable<TreeListNode> P_2)
	{
		PvfTreeViewModel pvfTreeViewModel = BlLa9HQnBc();
		bool flag = false;
		bool result = false;
		foreach (TreeListNode item in P_2)
		{
			if (flag)
			{
				KeyValuePair<string, PvfTreeFileBase> keyValuePair = ReWaPvpbaJ(item.Content);
				if (keyValuePair.Key[0].ToString() == P_0)
				{
					pvfTreeViewModel.GoToNode(keyValuePair.Value.FullPath);
					result = true;
					break;
				}
			}
			if (item == P_1)
			{
				flag = true;
			}
		}
		return result;
	}

	private IDictionary<string, PvfTreeFileBase> UGVaKi9tmY()
	{
		return ((PvfTreeViewModel)base.DataContext).TreeGroupData.Trees;
	}

	private PvfTreeViewModel BlLa9HQnBc()
	{
		return (PvfTreeViewModel)base.DataContext;
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

	private void treeListView_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		if (((int)Keyboard.Modifiers & 2) != 2)
		{
			TreeListNode focusedNode = base.View.FocusedNode;
			ieDaEMNBik(e.Text, focusedNode);
		}
	}

	private KeyValuePair<string, PvfTreeFileBase> ReWaPvpbaJ(object P_0)
	{
		return (KeyValuePair<string, PvfTreeFileBase>)P_0;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!doPaZ8eneX)
		{
			doPaZ8eneX = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/pvftreeview.xaml", UriKind.Relative);
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
			tree = (PvfTreeView)target;
			break;
		case 2:
			columnKey = (TreeListColumn)target;
			break;
		case 3:
			treeListView = (CustomTreeListView)target;
			break;
		case 4:
			importAs = (BarButtonItem)target;
			break;
		default:
			doPaZ8eneX = true;
			break;
		}
	}
}
