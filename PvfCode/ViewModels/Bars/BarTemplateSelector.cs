using System.Windows;
using System.Windows.Controls;

namespace PvfCode.ViewModels.Bars;

public class BarTemplateSelector : DataTemplateSelector
{
	public DataTemplate MainMenuTemplate { get; set; }

	public DataTemplate ToolbarTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item is BarModel barModel)
		{
			if (!barModel.IsMainMenu)
			{
				return ToolbarTemplate;
			}
			return MainMenuTemplate;
		}
		return base.SelectTemplate(item, container);
	}

	public BarTemplateSelector()
	{
	}
}
