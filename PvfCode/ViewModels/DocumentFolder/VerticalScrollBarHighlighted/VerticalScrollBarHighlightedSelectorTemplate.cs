using System.Windows;
using System.Windows.Controls;

namespace PvfCode.ViewModels.DocumentFolder.VerticalScrollBarHighlighted;

public class VerticalScrollBarHighlightedSelectorTemplate : DataTemplateSelector
{
	public DataTemplate SelectedTemplate { get; set; }

	public DataTemplate FindResultTemplate { get; set; }

	public DataTemplate MarkSameWordTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		return ((VerticalScrollBarHighlightedData)item).Type switch
		{
			VerticalScrollBarHighlightedType.当前选中行 => SelectedTemplate, 
			VerticalScrollBarHighlightedType.同音词 => MarkSameWordTemplate, 
			VerticalScrollBarHighlightedType.查找结果 => FindResultTemplate, 
			_ => null, 
		};
	}

	public VerticalScrollBarHighlightedSelectorTemplate()
	{
	}
}
