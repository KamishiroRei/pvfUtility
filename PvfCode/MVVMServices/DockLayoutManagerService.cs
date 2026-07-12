using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Layout.Core;

namespace PvfCode.MVVMServices;

public class DockLayoutManagerService : ServiceBase, IDockLayoutManagerService
{
	private DockLayoutManager LayoutManager => (DockLayoutManager)AssociatedObject;

	public FloatGroup AddFloatPanel(Control userControl, string caption)
	{
		LayoutPanel layoutPanel = LayoutManager.DockController.AddPanel(DockType.Fill);
		layoutPanel.Caption = caption;
		layoutPanel.Content = userControl;
		layoutPanel.DataContext = userControl.DataContext;
		layoutPanel.IsActive = true;
		layoutPanel.Padding = new Thickness(0.0);
		return LayoutManager.DockController.Float(layoutPanel);
	}

	public void Float(object panelViewModel)
	{
		BaseLayoutItem item = LayoutManager.GetItems().FirstOrDefault(it => it.DataContext == panelViewModel);
		if (item == null)
		{
			return;
		}

		FloatGroup floatGroup = LayoutManager.DockController.Float(item);
		floatGroup.SizeToContent = SizeToContent.Height;
		floatGroup.ItemWidth = new GridLength(600.0);
		floatGroup.HorizontalAlignment = HorizontalAlignment.Center;
		floatGroup.VerticalAlignment = VerticalAlignment.Center;
	}

	public void SetFloatPanelCenter(FloatGroup floatPanel)
	{
		BaseLayoutItem firstItem = floatPanel.GetItems().FirstOrDefault();
		if (firstItem == null)
		{
			return;
		}

		double left = Application.Current.MainWindow.ActualWidth / 2.0 - firstItem.ActualWidth / 2.0;
		double top = Application.Current.MainWindow.ActualHeight / 2.0 - firstItem.ActualHeight / 2.0;
		floatPanel.FloatLocation = new Point(left, top);
	}

	public void SetFloatPanelAutoHeight(FloatGroup floatPanel, SizeToContent sizeToContent)
	{
		floatPanel.SizeToContent = sizeToContent;
	}

	public void ClosePanel(object panelViewModel)
	{
		foreach (BaseLayoutItem item in LayoutManager.GetItems())
		{
			if (item.DataContext == panelViewModel)
			{
				LayoutManager.DockController.Close(item);
			}
		}
	}

	public void ShowContextMenu(object panelViewModel)
	{
		BaseLayoutItem item = LayoutManager.GetItems().FirstOrDefault(it => it.DataContext == panelViewModel);
		if (item != null)
		{
			LayoutManager.ShowContextMenu(item);
		}
	}

	public bool SplitRight(object panelViewModel)
	{
		DocumentPanel panel = LayoutManager.GetItems()
			.OfType<DocumentPanel>()
			.FirstOrDefault(it => it.DataContext == panelViewModel);
		return panel != null && LayoutManager.DockController.CreateNewDocumentGroup(panel, Orientation.Horizontal);
	}

	public void SetFloatPanelAutoHeight(object panelViewModel, SizeToContent sizeToContent)
	{
		if (LayoutManager.FloatGroups == null)
		{
			return;
		}

		foreach (FloatGroup floatGroup in LayoutManager.FloatGroups)
		{
			bool containsPanel = floatGroup.GetItems()?.Any(it => it.DataContext == panelViewModel) == true;
			if (containsPanel)
			{
				floatGroup.SizeToContent = sizeToContent;
				break;
			}
		}
	}
}
