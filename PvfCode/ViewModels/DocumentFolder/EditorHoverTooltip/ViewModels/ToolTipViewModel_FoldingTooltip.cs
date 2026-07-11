using System.Runtime.CompilerServices;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Controls.TextEditorFolder;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.ViewModels;

public class ToolTipViewModel_FoldingTooltip : ToolTipViewModelBase
{
	[CompilerGenerated]
	private TextDocument BCEiC6xLX2;

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

	public TextDocument Document
	{
		[CompilerGenerated]
		get
		{
			return BCEiC6xLX2;
		}
		[CompilerGenerated]
		set
		{
			BCEiC6xLX2 = value;
		}
	}

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
