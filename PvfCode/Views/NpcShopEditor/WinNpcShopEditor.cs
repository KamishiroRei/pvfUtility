using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PvfCode.Controls;
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
		BindSourceItemSelection();
		AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(SynchronizeShopItemSelection), handledEventsToo: true);
		AddHandler(Keyboard.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(SynchronizeShopItemSelection), handledEventsToo: true);
		InstallPurchaseDataEditors();
		RemoveDebugSearchStyles();
		Dispatcher.BeginInvoke((Action)RemoveDebugSearchStyles, DispatcherPriority.ApplicationIdle);
		if (filePath != null)
		{
			NpcShopEditorViewModel obj = base.DataContext as NpcShopEditorViewModel;
			FindNpcShopSource currentNpcShop = obj.NpcShopList.FirstOrDefault(item => item.File.FileName == filePath);
			obj.CurrentNpcShop = currentNpcShop;
		}
	}

	private void BindSourceItemSelection()
	{
		Binding selectionBinding = new Binding(nameof(NpcShopEditorViewModel.CurrentPurchaseItem))
		{
			Mode = BindingMode.OneWayToSource
		};
		equGrid.SetBinding(GridControl.CurrentItemProperty, selectionBinding);
		stkGrid.SetBinding(GridControl.CurrentItemProperty, new Binding(nameof(NpcShopEditorViewModel.CurrentPurchaseItem))
		{
			Mode = BindingMode.OneWayToSource
		});
	}

	private void InstallPurchaseDataEditors()
	{
		LayoutPanel? itemPropertiesPanel = DemoDockContainer.GetItems()
			.OfType<LayoutPanel>()
			.FirstOrDefault(panel => string.Equals(panel.Caption?.ToString(), "物品属性", StringComparison.Ordinal));
		if (itemPropertiesPanel?.Content is not StackPanel propertyEditors)
		{
			return;
		}

		List<EditBox> existingEditors = propertyEditors.Children.OfType<EditBox>().ToList();
		if (existingEditors.Count > 0)
		{
			BindEditor(existingEditors[0], nameof(NpcShopEditorViewModel.CurrentPurchaseItemCode), BindingMode.TwoWay);
			existingEditors[0].SetBinding(IsEnabledProperty, new Binding(nameof(NpcShopEditorViewModel.CanEditCurrentPurchaseItemCode)));
		}
		if (existingEditors.Count > 1)
		{
			BindEditor(existingEditors[1], "CurrentPurchaseItem.ItemName", BindingMode.OneWay);
		}
		if (existingEditors.Any(editor => AutomationProperties.GetAutomationId(editor) == "NpcShopPurchaseEditor.Price"))
		{
			return;
		}

		propertyEditors.Children.Add(CreatePurchaseEditor(
			"金币价格：",
			"CurrentPurchaseItem.Price",
			"NpcShopPurchaseEditor.Price"));
		propertyEditors.Children.Add(CreatePurchaseEditor(
			"所需材料ID：",
			"CurrentPurchaseItem.NeedMaterialItemCode",
			"NpcShopPurchaseEditor.MaterialId"));
		EditBox materialNameEditor = new EditBox
		{
			Caption = "所需材料名：",
			IsEnabled = false
		};
		AutomationProperties.SetAutomationId(materialNameEditor, "NpcShopPurchaseEditor.MaterialName");
		BindEditor(materialNameEditor, "CurrentPurchaseItem.NeedMaterialItemName", BindingMode.OneWay);
		propertyEditors.Children.Add(materialNameEditor);
		propertyEditors.Children.Add(CreatePurchaseEditor(
			"所需材料数量：",
			"CurrentPurchaseItem.NeedMaterialCount",
			"NpcShopPurchaseEditor.MaterialCount"));
	}

	private static EditBox CreatePurchaseEditor(string caption, string bindingPath, string automationId)
	{
		EditBox editor = new EditBox
		{
			Caption = caption
		};
		AutomationProperties.SetAutomationId(editor, automationId);
		BindEditor(editor, bindingPath, BindingMode.TwoWay);
		editor.SetBinding(IsEnabledProperty, new Binding("CurrentPurchaseItem.CanEditPurchaseData"));
		return editor;
	}

	private static void BindEditor(EditBox editor, string bindingPath, BindingMode mode)
	{
		editor.SetBinding(EditBox.ValueProperty, new Binding(bindingPath)
		{
			Mode = mode,
			UpdateSourceTrigger = mode == BindingMode.TwoWay
				? UpdateSourceTrigger.PropertyChanged
				: UpdateSourceTrigger.Default
		});
	}

	private void SynchronizeShopItemSelection(object sender, MouseButtonEventArgs e)
	{
		SynchronizeShopItemSelection(e.OriginalSource as DependencyObject);
	}

	private void SynchronizeShopItemSelection(object sender, KeyboardFocusChangedEventArgs e)
	{
		SynchronizeShopItemSelection(e.NewFocus as DependencyObject);
	}

	private void SynchronizeShopItemSelection(DependencyObject? eventSource)
	{
		GridControl? grid = FindVisualParent<GridControl>(eventSource);
		if (grid?.CurrentItem is not NpcShopItem item || DataContext is not NpcShopEditorViewModel viewModel)
		{
			return;
		}
		if (ReferenceEquals(grid, equGrid) || ReferenceEquals(grid, stkGrid) || grid.DataContext is NpcShopPageViewModel)
		{
			viewModel.CurrentPurchaseItem = item;
		}
	}

	private static T? FindVisualParent<T>(DependencyObject? child) where T : DependencyObject
	{
		while (child != null)
		{
			if (child is T match)
			{
				return match;
			}
			child = child switch
			{
				Visual or Visual3D => VisualTreeHelper.GetParent(child),
				ContentElement contentElement => ContentOperations.GetParent(contentElement) ?? LogicalTreeHelper.GetParent(contentElement),
				_ => LogicalTreeHelper.GetParent(child)
			};
		}
		return null;
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
