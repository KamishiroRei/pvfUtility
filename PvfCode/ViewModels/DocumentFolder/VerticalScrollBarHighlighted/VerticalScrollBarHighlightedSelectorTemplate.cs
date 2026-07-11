using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PvfCode.ViewModels.DocumentFolder.VerticalScrollBarHighlighted;

public class VerticalScrollBarHighlightedSelectorTemplate : DataTemplateSelector
{
	[CompilerGenerated]
	private DataTemplate d9w5VgMJId;

	[CompilerGenerated]
	private DataTemplate Py653DK5VT;

	[CompilerGenerated]
	private DataTemplate CAn5Rni4ua;

	public DataTemplate SelectedTemplate
	{
		[CompilerGenerated]
		get
		{
			return d9w5VgMJId;
		}
		[CompilerGenerated]
		set
		{
			d9w5VgMJId = value;
		}
	}

	public DataTemplate FindResultTemplate
	{
		[CompilerGenerated]
		get
		{
			return Py653DK5VT;
		}
		[CompilerGenerated]
		set
		{
			Py653DK5VT = value;
		}
	}

	public DataTemplate MarkSameWordTemplate
	{
		[CompilerGenerated]
		get
		{
			return CAn5Rni4ua;
		}
		[CompilerGenerated]
		set
		{
			CAn5Rni4ua = value;
		}
	}

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
