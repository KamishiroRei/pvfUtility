using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PvfCode.ViewModels.Bars;

public class BarItemTemplateSelector : DataTemplateSelector
{
	[CompilerGenerated]
	private DataTemplate ychxdbqgjn;

	[CompilerGenerated]
	private DataTemplate HaYxee8LEi;

	[CompilerGenerated]
	private DataTemplate hlDxtuTauZ;

	[CompilerGenerated]
	private DataTemplate nFwxbJHUi0;

	[CompilerGenerated]
	private DataTemplate OGExIaED6H;

	[CompilerGenerated]
	private DataTemplate SgnxEKO6no;

	public DataTemplate BarCheckItemTemplate
	{
		[CompilerGenerated]
		get
		{
			return ychxdbqgjn;
		}
		[CompilerGenerated]
		set
		{
			ychxdbqgjn = value;
		}
	}

	public DataTemplate BarItemTemplate
	{
		[CompilerGenerated]
		get
		{
			return HaYxee8LEi;
		}
		[CompilerGenerated]
		set
		{
			HaYxee8LEi = value;
		}
	}

	public DataTemplate BarSubItemTemplate
	{
		[CompilerGenerated]
		get
		{
			return hlDxtuTauZ;
		}
		[CompilerGenerated]
		set
		{
			hlDxtuTauZ = value;
		}
	}

	public DataTemplate BarItemSeparatorTemplate
	{
		[CompilerGenerated]
		get
		{
			return nFwxbJHUi0;
		}
		[CompilerGenerated]
		set
		{
			nFwxbJHUi0 = value;
		}
	}

	public DataTemplate BarComboBoxTemplate
	{
		[CompilerGenerated]
		get
		{
			return OGExIaED6H;
		}
		[CompilerGenerated]
		set
		{
			OGExIaED6H = value;
		}
	}

	public DataTemplate BarLook
	{
		[CompilerGenerated]
		get
		{
			return SgnxEKO6no;
		}
		[CompilerGenerated]
		set
		{
			SgnxEKO6no = value;
		}
	}

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
