using DevExpress.Mvvm;
using ICSharpCode.AvalonEdit.Document;
using PvfCode.Controls.TextEditorFolder;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;

public abstract class ToolTipViewModelBase : ViewModelBase
{
	public readonly TextSegment TextSeg;

	public readonly PvfFile File;

	public readonly TextEditorBase Editor;

	public EditorTooltipDataTemplateSelector ContentTemplateSelector
	{
		get
		{
			return GetProperty(() => ContentTemplateSelector);
		}
		set
		{
			SetProperty<EditorTooltipDataTemplateSelector>(() => ContentTemplateSelector, value);
		}
	}

	public EditorTooltipType TooltipType
	{
		get
		{
			return GetProperty(() => TooltipType);
		}
		set
		{
			SetProperty(() => TooltipType, value);
		}
	}

	public virtual string Text => GetSegText();

	public void RaiseText()
	{
		RaisePropertyChanged("Text");
	}

	public ToolTipViewModelBase(TextSegment seg, PvfFile file, TextEditorBase editor, EditorTooltipDataTemplateSelector editorTooltipDataTemplateSelector)
	{
		TextSeg = seg;
		File = file;
		Editor = editor;
		ContentTemplateSelector = editorTooltipDataTemplateSelector;
	}

	public ToolTipViewModelBase(EditorTooltipDataTemplateSelector editorTooltipDataTemplateSelector)
	{
		ContentTemplateSelector = editorTooltipDataTemplateSelector;
	}

	public virtual string GetSegText()
	{
		return Editor?.Document?.GetText(TextSeg);
	}

	public virtual void Loaded()
	{
	}
}
