using System.Windows;
using System.Windows.Controls;
using PvfCode.Controls;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;

public class EditorTooltipDataTemplateSelector : DataTemplateSelector
{
	private static readonly DataTemplate MarkdownCommentTooltip = CreateMarkdownCommentTooltip();

	public DataTemplate FoldingTooltip { get; set; }

	public DataTemplate CommentTooltip { get; set; }

	public DataTemplate FilePath { get; set; }

	public DataTemplate ItemCode { get; set; }

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
