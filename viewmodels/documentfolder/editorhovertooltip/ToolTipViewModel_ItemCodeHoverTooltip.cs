using System;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private TextDocument IRvbmF4t7;

	[CompilerGenerated]
	private IHighlightingDefinition nZXI4eyCk;

	[CompilerGenerated]
	private string? nlJEXO1lr;

	[CompilerGenerated]
	private ItemCodeHoverInfoBase eDaOyxVP7;

	[CompilerGenerated]
	private bool gDtKAIMaL;

	public TextDocument Document
	{
		[CompilerGenerated]
		get
		{
			return IRvbmF4t7;
		}
		[CompilerGenerated]
		set
		{
			IRvbmF4t7 = value;
		}
	}

	public IHighlightingDefinition Highlighting
	{
		[CompilerGenerated]
		get
		{
			return nZXI4eyCk;
		}
		[CompilerGenerated]
		set
		{
			nZXI4eyCk = value;
		}
	}

	private string? FilePath
	{
		[CompilerGenerated]
		get
		{
			return nlJEXO1lr;
		}
		[CompilerGenerated]
		set
		{
			nlJEXO1lr = value;
		}
	}

	public ItemCodeHoverInfoBase Data
	{
		[CompilerGenerated]
		get
		{
			return eDaOyxVP7;
		}
		[CompilerGenerated]
		set
		{
			eDaOyxVP7 = value;
		}
	}

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

	public bool ShowGoToButton
	{
		[CompilerGenerated]
		get
		{
			return gDtKAIMaL;
		}
		[CompilerGenerated]
		set
		{
			gDtKAIMaL = value;
		}
	}

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
					TextDocument document = Document;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler.AppendLiteral("`");
					defaultInterpolatedStringHandler.AppendFormatted(text);
					defaultInterpolatedStringHandler.AppendLiteral("`\t");
					defaultInterpolatedStringHandler.AppendFormatted(itemCode);
					document.Text = defaultInterpolatedStringHandler.ToStringAndClear();
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

	[CompilerGenerated]
	private ResultData<ImageSource> Doadfep2p()
	{
		return AppCore.ViewModelBase.PVF.GetScriptIconSource(FilePath);
	}
}
