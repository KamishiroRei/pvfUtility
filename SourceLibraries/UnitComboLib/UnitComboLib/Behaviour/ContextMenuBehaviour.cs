using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitComboLib.Behaviour;

public class ContextMenuBehaviour
{
	private static readonly DependencyProperty MenuListProperty = DependencyProperty.RegisterAttached("MenuList", typeof(ContextMenu), typeof(ContextMenuBehaviour), (PropertyMetadata)(object)new UIPropertyMetadata(null, new PropertyChangedCallback(OnMenuListChanged)));

	public static ContextMenu GetMenuList(DependencyObject obj)
	{
		return (ContextMenu)obj.GetValue(MenuListProperty);
	}

	public static void SetMenuList(DependencyObject obj, ContextMenu value)
	{
		obj.SetValue(MenuListProperty, (object)value);
	}

	private static void OnMenuListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		FrameworkElement frameworkElement = d as FrameworkElement;
		if (frameworkElement != null)
		{
			frameworkElement.MouseLeftButtonUp += element_MouseLeftButtonUp;
		}
		else
		{
			frameworkElement.MouseLeftButtonUp -= element_MouseLeftButtonUp;
		}
	}

	private static void element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		FrameworkElement frameworkElement = sender as FrameworkElement;
		ContextMenu menuList = GetMenuList((DependencyObject)(object)frameworkElement);
		if (menuList != null)
		{
			menuList.PlacementTarget = (UIElement)sender;
			menuList.IsOpen = true;
		}
		else if (frameworkElement != null)
		{
			frameworkElement.ContextMenu.PlacementTarget = (UIElement)sender;
			frameworkElement.ContextMenu.IsOpen = true;
		}
	}
}
