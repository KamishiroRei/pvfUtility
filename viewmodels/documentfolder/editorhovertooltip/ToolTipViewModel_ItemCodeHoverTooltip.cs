using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm.DataAnnotations;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode;
using PvfCode.Dot;
using PvfCode.Models.Options.Editor.ItemCodeHoverConfigModels;
using PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;

namespace ViewModels.DocumentFolder.EditorHoverTooltip;

public class ToolTipViewModel_ItemCodeHoverTooltip : ToolTipViewModelBase
{
	public TextDocument Document { get; set; }

	public IHighlightingDefinition Highlighting { get; set; }

	private string? FilePath { get; set; }

	public ItemCodeHoverInfoBase Data { get; set; }

	public ImageSource ImageSource
	{
		get
		{
			return GetProperty(() => ImageSource);
		}
		set
		{
			SetProperty<ImageSource>(() => ImageSource, value);
		}
	}

	public Visibility ShowImage
	{
		get
		{
			return GetProperty(() => ShowImage);
		}
		set
		{
			SetProperty(() => ShowImage, value);
		}
	}

	public bool ShowGoToButton { get; set; }

	public ToolTipViewModel_ItemCodeHoverTooltip(string? filePath, int itemCode, ItemCodeHoverInfoBase info, EditorTooltipDataTemplateSelector editorTooltipDataTemplateSelector)
		: base(editorTooltipDataTemplateSelector)
	{
		try
		{
			ShowGoToButton = true;
			Data = info;
			Document = new TextDocument();
			if (filePath == null)
			{
				Document.Text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("ToolTipViewModel_ItemCodeHoverTooltip_NotFoundItemCode"), itemCode);
			}
			else
			{
				PvfGroup pVF = AppCore.ViewModelBase.PVF;
				if (pVF.FileList.TryGetValue(filePath, out PvfFile value))
				{
					string text = pVF.GetItemName(value);
					if (!string.IsNullOrEmpty(text))
					{
						text = text.Replace("\r\n", "");
					}
					Document.Text = $"`{text}`\t{itemCode}";
				}
			}
			Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
			FilePath = filePath;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "IemCodeToItemNamePopupViewModel.IemCodeToItemNamePopupViewModel");
		}
	}

	public ToolTipViewModel_ItemCodeHoverTooltip(string msg, ItemCodeHoverInfoBase info, EditorTooltipDataTemplateSelector editorTooltipDataTemplateSelector)
		: base(editorTooltipDataTemplateSelector)
	{
		Data = info;
		Document = new TextDocument();
		Document.Text = msg;
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
		ShowGoToButton = false;
	}

	public override async void Loaded()
	{
		ShowImage = Visibility.Collapsed;
		ResultData<ImageSource> resultData = await Task.Run(() => AppCore.ViewModelBase.PVF.GetScriptIconSource(FilePath));
		if (resultData.Data != null)
		{
			ImageSource = resultData.Data;
		}
		ShowImage = ((ImageSource == null) ? Visibility.Collapsed : Visibility.Visible);
	}

	[Command]
	public void GoToFile()
	{
		if (FilePath != null)
		{
			AppCore.ViewModelBase.RootDocument.AddDocument(FilePath, gotoNode: true);
		}
	}

}
