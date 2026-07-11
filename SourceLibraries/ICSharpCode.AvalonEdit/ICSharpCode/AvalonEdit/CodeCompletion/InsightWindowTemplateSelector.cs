using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.AvalonEdit.CodeCompletion;

internal sealed class InsightWindowTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item is string)
		{
			return (DataTemplate)((FrameworkElement)container).FindResource("TextBlockTemplate");
		}
		return null;
	}
}
