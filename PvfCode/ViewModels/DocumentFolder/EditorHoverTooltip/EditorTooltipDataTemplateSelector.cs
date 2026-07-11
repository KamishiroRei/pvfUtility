using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PvfCode.Controls;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;

public class EditorTooltipDataTemplateSelector : DataTemplateSelector
{
	private static readonly DataTemplate MarkdownCommentTooltip = CreateMarkdownCommentTooltip();
	[CompilerGenerated]
	private DataTemplate SppyULXZPi;

	[CompilerGenerated]
	private DataTemplate yJCyc7mk4K;

	[CompilerGenerated]
	private DataTemplate JVvy8yooVr;

	[CompilerGenerated]
	private DataTemplate wHfyMgixLo;

	public DataTemplate FoldingTooltip
	{
		[CompilerGenerated]
		get
		{
			return SppyULXZPi;
		}
		[CompilerGenerated]
		set
		{
			SppyULXZPi = value;
		}
	}

	public DataTemplate CommentTooltip
	{
		[CompilerGenerated]
		get
		{
			return yJCyc7mk4K;
		}
		[CompilerGenerated]
		set
		{
			yJCyc7mk4K = value;
		}
	}

	public DataTemplate FilePath
	{
		[CompilerGenerated]
		get
		{
			return JVvy8yooVr;
		}
		[CompilerGenerated]
		set
		{
			JVvy8yooVr = value;
		}
	}

	public DataTemplate ItemCode
	{
		[CompilerGenerated]
		get
		{
			return wHfyMgixLo;
		}
		[CompilerGenerated]
		set
		{
			wHfyMgixLo = value;
		}
	}

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		ToolTipViewModelBase toolTipViewModelBase = (ToolTipViewModelBase)item;
		if (toolTipViewModelBase == null)
		{
			return null;
		}
		return toolTipViewModelBase.TooltipType switch
		{
			EditorTooltipType.Folding => FoldingTooltip, 
			EditorTooltipType.FilePath => FilePath, 
			EditorTooltipType.Comment => MarkdownCommentTooltip, 
			EditorTooltipType.ItemCode => ItemCode, 
			_ => base.SelectTemplate(item, container), 
		};
	}

	public EditorTooltipDataTemplateSelector()
	{
	}

	private static DataTemplate CreateMarkdownCommentTooltip()
	{
		return new DataTemplate
		{
			VisualTree = new FrameworkElementFactory(typeof(PvfCommentTooltipView))
		};
	}
}
