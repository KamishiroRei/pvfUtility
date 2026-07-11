using System.IO;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.ViewModels;

public class ToolTipViewModel_FilePathHoverTooltip : ToolTipViewModelBase
{
	private readonly string FilePath;

	public bool ImagePack2NotLoaded
	{
		get
		{
			return GetProperty(() => ImagePack2NotLoaded);
		}
		set
		{
			SetProperty(() => ImagePack2NotLoaded, value);
		}
	}

	public ToolTipViewModel_FilePathHoverTooltip(string filePath, EditorTooltipDataTemplateSelector editorTooltipDataTemplateSelector)
		: base(editorTooltipDataTemplateSelector)
	{
		FilePath = filePath;
	}

	public override void Loaded()
	{
		if (!string.IsNullOrEmpty(FilePath))
		{
			string extension = Path.GetExtension(FilePath);
			if (extension != null && extension.ToLower() == ".img")
			{
				ImagePack2NotLoaded = ImagePack2Service.Instance.Count > 0;
			}
			else
			{
				ImagePack2NotLoaded = true;
			}
		}
		else
		{
			ImagePack2NotLoaded = true;
		}
	}
}
