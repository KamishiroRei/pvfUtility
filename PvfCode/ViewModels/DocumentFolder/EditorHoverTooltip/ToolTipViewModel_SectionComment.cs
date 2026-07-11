using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using PvfCode.LoggerBase;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;

public class ToolTipViewModel_SectionComment : ToolTipViewModelBase
{
	private readonly PvfFileType? FileType;

	private readonly PvfCommentDtoRes BdLijk5r1Y;

	[CompilerGenerated]
	private TextDocument GxuiTABjSw;

	public PvfCommentDto Comment
	{
		get
		{
			return GetProperty(() => Comment);
		}
		set
		{
			SetProperty<PvfCommentDto>(() => Comment, value);
		}
	}

	public bool IsShare
	{
		get
		{
			return GetProperty(() => IsShare);
		}
		set
		{
			SetProperty(() => IsShare, value);
		}
	}

	public bool IsEditing
	{
		get => GetProperty(() => IsEditing);
		set => SetProperty(() => IsEditing, value);
	}

	public string NickName
	{
		get
		{
			return GetProperty(() => NickName);
		}
		set
		{
			SetProperty<string>(() => NickName, value);
		}
	}

	public bool IsLoading
	{
		get
		{
			return GetProperty(() => IsLoading);
		}
		set
		{
			SetProperty(() => IsLoading, value);
		}
	}

	public TextDocument Document
	{
		[CompilerGenerated]
		get
		{
			return GxuiTABjSw;
		}
		[CompilerGenerated]
		set
		{
			GxuiTABjSw = value;
		}
	}

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

	public ToolTipViewModel_SectionComment(PvfCommentDtoRes res, EditorTooltipDataTemplateSelector editorTooltipDataTemplateSelector)
		: base(editorTooltipDataTemplateSelector)
	{
		Document = new TextDocument();
		IsShare = true;
		BdLijk5r1Y = res;
		FileType = res.FileType;
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(FileType.HasValue ? FileType.Value : PvfFileType.equ);
		NickName = AppCore.NickNameTemp;
	}

	public override async void Loaded()
	{
		IsLoading = true;
		ResultData<PvfCommentDto> resultData = await ServicePvfTabComment.Instance.GetPvfComment(BdLijk5r1Y);
		if (resultData.IsError)
		{
			Document.Text = resultData.Msg;
			Comment = new PvfCommentDto
			{
				Id = -1,
				PvfCommentType = BdLijk5r1Y.PvfCommentType
			};
		}
		else
		{
			Document.Text = resultData.Data.Comment;
			Comment = resultData.Data;
		}
		IsLoading = false;
	}

	[Command]
	public async void Save()
	{
		if (Comment == null)
		{
			Comment = new PvfCommentDto
			{
				Id = -1,
				PvfCommentType = BdLijk5r1Y.PvfCommentType
			};
		}
		IsLoading = true;
		Comment.Comment = Document.Text;
		if (!string.IsNullOrEmpty(NickName))
		{
			if (Comment.Authors != null && !Comment.Authors.Contains(NickName))
			{
				PvfCommentDto comment = Comment;
				comment.Authors = comment.Authors + "|" + NickName;
			}
			else
			{
				Comment.Authors = NickName;
			}
		}
		Comment.FileType = FileType;
		Comment.Section = BdLijk5r1Y.Section;
		Comment.PvfCommentType = BdLijk5r1Y.PvfCommentType;
		Comment.Create = DateTime.Now;
		Comment.UpdateTime = DateTime.Now;
		AppCore.NickNameTemp = NickName;
		await ServicePvfTabComment.Instance.AddComment(Comment);
		IsEditing = false;
		IsLoading = false;
		await AppCore.Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, "保存成功", Res.Instance.VisualStudioBlendLogo2015Pre_16x));
	}
}
