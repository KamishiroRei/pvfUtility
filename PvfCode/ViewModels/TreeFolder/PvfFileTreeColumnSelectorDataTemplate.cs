using System.Windows;
using System.Windows.Controls;

namespace PvfCode.ViewModels.TreeFolder;

public class PvfFileTreeColumnSelectorDataTemplate : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object P_0, DependencyObject P_1)
	{
		string resourceKey = "PvfFileTreeColumnTemplate";
		return ((FrameworkElement)(object)P_1).FindResource(resourceKey) as DataTemplate;
	}

	public PvfFileTreeColumnSelectorDataTemplate()
	{
	}
}
