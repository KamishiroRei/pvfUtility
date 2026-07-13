using System;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
#if RECOVERED_LEGACY_PVFCODE_DOT
using PvfCode.Compatibility;
#endif
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.LoggerBase;
using PvfCode.Models.CodeCompletionModels;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class CodeCompletionToolTipViewModel : ViewModelBase
{
	private readonly PvfFileType? fileType;

	private readonly PvfCommentDtoRes commentRequest;

	public bool CompletionDataIsShare
	{
		get
		{
			return GetProperty(() => CompletionDataIsShare);
		}
		set
		{
			SetProperty(() => CompletionDataIsShare, value);
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

	public CodeCompletionData CompletionData { get; set; }

	public TextDocument Document { get; set; }

	public TextDocument DocumentAutoText { get; set; }

	public int TabControlSelectedIndex
	{
		get
		{
			return GetProperty(() => TabControlSelectedIndex);
		}
		set
		{
			SetProperty(() => TabControlSelectedIndex, value, SelectedIndexChanged);
		}
	}

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

	public bool IsEditorComment
	{
		get
		{
			return GetProperty(() => IsEditorComment);
		}
		set
		{
			SetProperty(() => IsEditorComment, value);
		}
	}

	private void SelectedIndexChanged()
	{
		if (TabControlSelectedIndex == 1)
		{
			DocumentAutoText.Text = CompletionData.CompleteText;
		}
	}

	public CodeCompletionToolTipViewModel(CodeCompletionData completionData, PvfFileType? fileType)
	{
		try
		{
			DocumentAutoText = new TextDocument();
			CompletionDataIsShare = true;
			IsShare = true;
			PvfCommentType pvfCommentType = PvfCommentType.Section;
			switch (completionData.HighlightingType)
			{
			case HighlightingType.String:
				pvfCommentType = PvfCommentType.String;
				break;
			case HighlightingType.Section:
				pvfCommentType = PvfCommentType.Section;
				break;
			case HighlightingType.SectionEnd:
				pvfCommentType = PvfCommentType.Section;
				break;
			}
			commentRequest = new PvfCommentDtoRes
			{
				FileType = fileType,
				PvfCommentType = pvfCommentType,
				Section = completionData.Text
			};
			this.fileType = fileType;
			Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
			Document = new TextDocument();
			CompletionData = completionData;
			NickName = AppCore.NickNameTemp;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CodeCompletionToolTipViewModel.CodeCompletionToolTipViewModel");
		}
	}

	public async void Loaded(object sender)
	{
		await LoadPvfComment();
	}

	private async Task LoadPvfComment()
	{
		Document.Text = "";
		IsLoading = true;
		try
		{
			if (string.IsNullOrEmpty(CompletionData.Description))
			{
				ResultData<PvfCommentDto> resultData = await ServicePvfTabComment.Instance.GetPvfComment(commentRequest);
				if (resultData.IsError)
				{
					Document.Text = resultData.Msg;
					Comment = new PvfCommentDto { Comment = resultData.Msg };
				}
				else
				{
					Document.Text = resultData.Data.Comment;
					Comment = resultData.Data;
				}
			}
			else
			{
				Document.Text = CompletionData.Description;
#if RECOVERED_LEGACY_PVFCODE_DOT
				PvfCommentDto comment = new PvfCommentDto
				{
					Comment = CompletionData.Description
				};
				PvfCommentDtoCompatibility.SetTitle(comment, CompletionData.Text);
				Comment = comment;
#else
				Comment = new PvfCommentDto
				{
					Title = CompletionData.Text,
					Comment = CompletionData.Description
				};
#endif
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CodeCompletionToolTipViewModel.LoadPvfComment");
		}
		IsLoading = false;
	}

	[Command]
	public void OnEditorSetIsEditorComment()
	{
		IsEditorComment = true;
	}

	[Command]
	public async void SavePvfComment()
	{
		try
		{
			if (Comment == null)
			{
				Comment = new PvfCommentDto();
			}
			IsLoading = true;
			if (string.IsNullOrEmpty(CompletionData.Description))
			{
				if (Comment.Comment == Document.Text)
				{
					await AppCore.Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, "保存成功", Res.Instance.VisualStudioBlendLogo2015Pre_16x));
					IsLoading = false;
					IsEditorComment = false;
					return;
				}
				Comment.Comment = Document.Text;
				Comment.Authors = NickName;
				Comment.FileType = fileType;
				Comment.PvfCommentType = commentRequest.PvfCommentType;
				Comment.Create = DateTime.Now;
				Comment.UpdateTime = DateTime.Now;
				AppCore.NickNameTemp = NickName;
				ServicePvfTabComment.Instance.AddComment(Comment);
			}
			else
			{
				CompletionData.NickNames = CompletionData.NickNames + "|" + NickName;
				CompletionData.Description = Document.Text;
				AppSetting.Instance.EditConfig.SaveCompletionDatas(CompletionData);
				await AppSetting.Instance.SaveSetting();
			}
			IsEditorComment = false;
			IsLoading = false;
			await AppCore.Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, "保存成功", Res.Instance.VisualStudioBlendLogo2015Pre_16x));
		}
		catch (Exception e)
		{
			IsLoading = false;
			IsEditorComment = false;
			AppCore.Logger.ErrorUploadDialog(e, "CodeCompletionToolTipViewModel.SavePvfComment");
		}
	}

	[Command]
	public async void SaveCodeCompleData()
	{
		try
		{
			IsLoading = true;
			CompletionData.CompleteText = DocumentAutoText.Text;
			CompletionData.NickNames = CompletionData.NickNames + "|" + NickName;
			AppSetting.Instance.EditConfig.SaveCompletionDatas(CompletionData);
			await AppSetting.Instance.SaveSetting();
			await AppCore.Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, "保存成功", Res.Instance.VisualStudioBlendLogo2015Pre_16x));
			IsLoading = false;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CodeCompletionToolTipViewModel.SaveCodeCompleData");
		}
	}
}
