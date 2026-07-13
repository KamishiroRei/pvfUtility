using System.Windows;
using System.Windows.Controls;

namespace PvfCode.ViewModels.TreeFolder;

public class PvfFileTreeColumnSelectorDataTemplate : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		string resourceKey = "PvfFileTreeColumnTemplate";
		return ((FrameworkElement)container).FindResource(resourceKey) as DataTemplate;
	}

	public PvfFileTreeColumnSelectorDataTemplate()
	{
	}
}
