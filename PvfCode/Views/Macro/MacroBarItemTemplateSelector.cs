using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PvfCode.Models.Macro;

namespace PvfCode.Views.Macro;

public class MacroBarItemTemplateSelector : DataTemplateSelector
{
	[CompilerGenerated]
	private string MJXHztxpZy;

	[CompilerGenerated]
	private string gpshDTpE0S;

	public string ButtonItemTemplateKey
	{
		[CompilerGenerated]
		get
		{
			return MJXHztxpZy;
		}
		[CompilerGenerated]
		set
		{
			MJXHztxpZy = value;
		}
	}

	public string SubItemTemplateKey
	{
		[CompilerGenerated]
		get
		{
			return gpshDTpE0S;
		}
		[CompilerGenerated]
		set
		{
			gpshDTpE0S = value;
		}
	}

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (((KeyValuePair<string, MacroData>)item).Value.IsFile)
		{
			return Rw3HNNy7jW(container, ButtonItemTemplateKey);
		}
		return Rw3HNNy7jW(container, SubItemTemplateKey);
	}

	private DataTemplate Rw3HNNy7jW(DependencyObject P_0, object P_1)
	{
		if (P_0 is FrameworkContentElement)
		{
			return ((FrameworkContentElement)(object)P_0).TryFindResource(P_1) as DataTemplate;
		}
		if (P_0 is FrameworkElement)
		{
			return ((FrameworkElement)(object)P_0).TryFindResource(P_1) as DataTemplate;
		}
		return null;
	}

	public MacroBarItemTemplateSelector()
	{
	}
}
