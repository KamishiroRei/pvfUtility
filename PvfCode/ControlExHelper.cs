using System.Windows;
using System.Windows.Media;

namespace PvfCode;

public static class ControlExHelper
{
	public static T FindVisualChild<T>(this DependencyObject obj) where T : DependencyObject
	{
		if (obj != null)
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
				{
					return (T)(object)child;
				}
				T val = child.FindVisualChild<T>();
				if (val != null)
				{
					return val;
				}
			}
		}
		return default(T);
	}
}
