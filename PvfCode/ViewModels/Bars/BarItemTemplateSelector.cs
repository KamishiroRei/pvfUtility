using System.Windows;
using System.Windows.Controls;

namespace PvfCode.ViewModels.Bars;

public class BarItemTemplateSelector : DataTemplateSelector
{
	public DataTemplate BarCheckItemTemplate { get; set; }

	public DataTemplate BarItemTemplate { get; set; }

	public DataTemplate BarSubItemTemplate { get; set; }

	public DataTemplate BarItemSeparatorTemplate { get; set; }

	public DataTemplate BarComboBoxTemplate { get; set; }

	public DataTemplate BarLook { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item is BarCommandViewModel barCommandViewModel)
		{
			DataTemplate result = null;
			switch (barCommandViewModel.BarItemType)
			{
			case BarType.Default:
				result = BarItemTemplate;
				break;
			case BarType.ComboBox:
				result = BarComboBoxTemplate;
				break;
			case BarType.Separator:
				result = BarItemSeparatorTemplate;
				break;
			case BarType.SubItem:
				result = BarSubItemTemplate;
				break;
			case BarType.CheckBox:
				result = BarCheckItemTemplate;
				break;
			case BarType.Look:
				result = BarLook;
				break;
			}
			return result;
		}
		return base.SelectTemplate(item, container);
	}

	public BarItemTemplateSelector()
	{
	}
}
