using System.Windows;
using System.Windows.Controls;
using PvfCode.ViewModels.DocumentFolder;

namespace PvfCode.ViewModels.TreeFolder;

public class TreeRowToolTipTemplateSelector : DataTemplateSelector
{
	public DataTemplate PvfFileItemPreivewDataTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		_ = (DocumentBase)item;
		return base.SelectTemplate(item, container);
	}

	public TreeRowToolTipTemplateSelector()
	{
	}
}
