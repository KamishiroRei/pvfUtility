using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using PvfCode.ViewModels.independent_drop;
using PvfCode.ViewModels.independent_drop.DropList;

namespace PvfCode.Views.independent_drop;

public class ViewDropSelectItemCodes : ThemedWindow, IComponentConnector
{
	internal ViewDropSelectItemCodes win;

	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	private bool JZNhkrwKOk;

	public ViewDropSelectItemCodes()
	{
		base.DataContext = new ViewDropSelectItemCodesViewModel();
		InitializeComponent();
	}

	public IEnumerable<ListItem> GetSelectedItmes()
	{
		ViewDropSelectItemCodesViewModel viewDropSelectItemCodesViewModel = (ViewDropSelectItemCodesViewModel)base.DataContext;
		if ((viewDropSelectItemCodesViewModel.SelectedItems == null || viewDropSelectItemCodesViewModel.SelectedItems.Count == 0) && !viewDropSelectItemCodesViewModel.GroupItems.Any())
		{
			return new List<ListItem>();
		}
		IEnumerable<ListItem> source;
		if (viewDropSelectItemCodesViewModel.GroupItems.Any())
		{
			source = dSEhZPtVqy(viewDropSelectItemCodesViewModel.GroupItems);
		}
		else
		{
			ListItemSelect[] array = viewDropSelectItemCodesViewModel.SelectedItems.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DropWeight = viewDropSelectItemCodesViewModel.DropWeight;
			}
			source = dSEhZPtVqy(viewDropSelectItemCodesViewModel.SelectedItems);
		}
		return source.Where((ListItem it) => it.ItemCode.HasValue);
	}

	private IEnumerable<ListItem> dSEhZPtVqy(IEnumerable<ListItemSelect> P_0)
	{
		List<ListItem> list = new List<ListItem>();
		foreach (ListItemSelect item in P_0)
		{
			list.Add(new ListItem
			{
				DropWeight = item.DropWeight,
				ItemCode = item.ItemCode
			});
		}
		return list;
	}

	private void SRuhJYlIFX(object P_0, ItemCancelEventArgs P_1)
	{
		P_1.Cancel = true;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!JZNhkrwKOk)
		{
			JZNhkrwKOk = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/independent_drop/viewdropselectitemcodes.xaml", UriKind.Relative);
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
			win = (ViewDropSelectItemCodes)target;
			break;
		case 2:
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += SRuhJYlIFX;
			break;
		case 3:
			Root = (LayoutGroup)target;
			break;
		default:
			JZNhkrwKOk = true;
			break;
		}
	}
}
