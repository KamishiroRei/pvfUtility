using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Editors;
using PvfCode.ViewModels.NpcShopEditor;

namespace PvfCode.Views.NpcShopEditor;

public class WinNpcShopEditor : ThemedWindow, IComponentConnector
{
	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal NpcShopItemListControlEx equGrid;

	internal NpcShopItemListControlEx stkGrid;

	internal Grid gridHeader;

	private bool viOHRUgpmN;

	public WinNpcShopEditor(string filePath = null)
	{
		InitializeComponent();
		RemoveDebugSearchStyles();
		Dispatcher.BeginInvoke((Action)RemoveDebugSearchStyles, DispatcherPriority.ApplicationIdle);
		if (filePath != null)
		{
			NpcShopEditorViewModel obj = base.DataContext as NpcShopEditorViewModel;
			FindNpcShopSource currentNpcShop = obj.NpcShopList.FirstOrDefault(item => item.File.FileName == filePath);
			obj.CurrentNpcShop = currentNpcShop;
		}
	}

	private void RemoveDebugSearchStyles()
	{
		RemoveDebugSearchStyles(this);
	}

	private static void RemoveDebugSearchStyles(DependencyObject parent)
	{
		if (parent is FrameworkElement element && element.Resources.Count > 0)
		{
			List<object> keysToRemove = element.Resources.Keys.Cast<object>()
				.Where(IsDebugSearchStyleKey)
				.ToList();
			foreach (object key in keysToRemove)
			{
				element.Resources.Remove(key);
			}
		}
		int childCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int index = 0; index < childCount; index++)
		{
			RemoveDebugSearchStyles(VisualTreeHelper.GetChild(parent, index));
		}
	}

	private static bool IsDebugSearchStyleKey(object key)
	{
		if (key is Type type && type == typeof(SearchControl))
		{
			return true;
		}
		PropertyInfo resourceKeyProperty = key.GetType().GetProperty("ResourceKey");
		return string.Equals(resourceKeyProperty?.GetValue(key)?.ToString(), "SearchPanelContentTemplate", StringComparison.Ordinal);
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		Application.Current.MainWindow?.Activate();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!viOHRUgpmN)
		{
			viOHRUgpmN = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/npcshopeditor/winnpcshopeditor.xaml", UriKind.Relative);
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
			DemoDockContainer = (DockLayoutManager)target;
			break;
		case 2:
			Root = (LayoutGroup)target;
			break;
		case 3:
			equGrid = (NpcShopItemListControlEx)target;
			break;
		case 4:
			stkGrid = (NpcShopItemListControlEx)target;
			break;
		case 5:
			gridHeader = (Grid)target;
			break;
		default:
			viOHRUgpmN = true;
			break;
		}
	}
}
