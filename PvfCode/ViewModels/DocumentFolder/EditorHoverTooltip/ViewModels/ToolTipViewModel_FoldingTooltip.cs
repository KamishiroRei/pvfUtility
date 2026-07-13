using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Controls.TextEditorFolder;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.ViewModels;

public class ToolTipViewModel_FoldingTooltip : ToolTipViewModelBase
{
	public IHighlightingDefinition Highlighting
	{
		get
		{
			return GetProperty(() => Highlighting);
		}
		set
		{
			SetProperty<IHighlightingDefinition>(() => Highlighting, value);
		}
	}

	public TextDocument Document { get; set; }

	public ToolTipViewModel_FoldingTooltip(PvfFile file, TextSegment seg, TextEditorBase editor, EditorTooltipDataTemplateSelector selector)
		: base(seg, file, editor, selector)
	{
		Document = new TextDocument
		{
			Text = GetSegText()
		};
	}

	public override void Loaded()
	{
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(File.FileType);
	}
}
