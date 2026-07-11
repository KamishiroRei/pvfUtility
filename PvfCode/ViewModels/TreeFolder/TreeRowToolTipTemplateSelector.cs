using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PvfCode.ViewModels.DocumentFolder;

namespace PvfCode.ViewModels.TreeFolder;

public class TreeRowToolTipTemplateSelector : DataTemplateSelector
{
	[CompilerGenerated]
	private DataTemplate UJCrPQKJLq;

	public DataTemplate PvfFileItemPreivewDataTemplate
	{
		[CompilerGenerated]
		get
		{
			return UJCrPQKJLq;
		}
		[CompilerGenerated]
		set
		{
			UJCrPQKJLq = value;
		}
	}

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		_ = (DocumentBase)item;
		return base.SelectTemplate(item, container);
	}

	public TreeRowToolTipTemplateSelector()
	{
	}
}
