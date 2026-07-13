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
using PvfCode.ViewModels.SearchPvf.SearchName;

namespace PvfCode.Views.SearchPvf.SearchName;

public class WindowSearchItemName : ThemedWindow, IComponentConnector
{
	internal WindowSearchItemName win;

	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	private bool _contentLoaded;

	public int? ItemCode
	{
		get
		{
			SearchNameViewModel<ItemNameSearchResultBase> searchNameViewModel = (SearchNameViewModel<ItemNameSearchResultBase>)base.DataContext;
			if (searchNameViewModel.SelectedItem == null)
			{
				return null;
			}
			return searchNameViewModel.SelectedItem.ItemCode;
		}
	}

	public WindowSearchItemName()
	{
		base.DataContext = new SearchNameViewModel<ItemNameSearchResultBase>();
		InitializeComponent();
	}

	public WindowSearchItemName(string title, bool showGroupPanel, IEnumerable<string>? pathNames = null, bool initItems = false)
	{
		base.DataContext = new SearchNameViewModel<ItemNameSearchResultBase>(title, showGroupPanel, pathNames, initItems);
		InitializeComponent();
		base.Width = 400.0;
	}

	private void OnDockItemClosing(object sender, ItemCancelEventArgs e)
	{
		e.Cancel = true;
	}

	public IEnumerable<int> ItemCodes()
	{
		SearchNameViewModel<ItemNameSearchResultBase> searchNameViewModel = (SearchNameViewModel<ItemNameSearchResultBase>)base.DataContext;
		if ((searchNameViewModel.SelectedItems == null || searchNameViewModel.SelectedItems.Count == 0) && !searchNameViewModel.GroupItems.Any())
		{
			return new List<int>();
		}
		IEnumerable<ItemNameSearchResultBase> source = ((!searchNameViewModel.GroupItems.Any()) ? ((IEnumerable<ItemNameSearchResultBase>)searchNameViewModel.SelectedItems) : ((IEnumerable<ItemNameSearchResultBase>)searchNameViewModel.GroupItems));
		return from it in source
			where it.ItemCode.HasValue
			select it.ItemCode.Value;
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		Application.Current.MainWindow.Activate();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/searchpvf/searchname/windowsearchitemname.xaml", UriKind.Relative);
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
			win = (WindowSearchItemName)target;
			break;
		case 2:
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += OnDockItemClosing;
			break;
		case 3:
			Root = (LayoutGroup)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
