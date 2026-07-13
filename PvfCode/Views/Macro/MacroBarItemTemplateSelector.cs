using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PvfCode.Models.Macro;

namespace PvfCode.Views.Macro;

public class MacroBarItemTemplateSelector : DataTemplateSelector
{
	public string ButtonItemTemplateKey { get; set; }

	public string SubItemTemplateKey { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (((KeyValuePair<string, MacroData>)item).Value.IsFile)
		{
			return FindTemplate(container, ButtonItemTemplateKey);
		}
		return FindTemplate(container, SubItemTemplateKey);
	}

	private DataTemplate FindTemplate(DependencyObject container, object resourceKey)
	{
		if (container is FrameworkContentElement contentElement)
		{
			return contentElement.TryFindResource(resourceKey) as DataTemplate;
		}
		if (container is FrameworkElement element)
		{
			return element.TryFindResource(resourceKey) as DataTemplate;
		}
		return null;
	}

	public MacroBarItemTemplateSelector()
	{
	}
}
