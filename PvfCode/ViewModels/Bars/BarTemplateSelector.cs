using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PvfCode.ViewModels.Bars;

public class BarTemplateSelector : DataTemplateSelector
{
	[CompilerGenerated]
	private DataTemplate KlSxOeXWc9;

	[CompilerGenerated]
	private DataTemplate no5xKooQwX;

	public DataTemplate MainMenuTemplate
	{
		[CompilerGenerated]
		get
		{
			return KlSxOeXWc9;
		}
		[CompilerGenerated]
		set
		{
			KlSxOeXWc9 = value;
		}
	}

	public DataTemplate ToolbarTemplate
	{
		[CompilerGenerated]
		get
		{
			return no5xKooQwX;
		}
		[CompilerGenerated]
		set
		{
			no5xKooQwX = value;
		}
	}

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
