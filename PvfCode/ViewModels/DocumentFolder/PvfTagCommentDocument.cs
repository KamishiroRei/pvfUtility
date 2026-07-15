using System;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.ViewModels.DocumentFolder.Enums;

namespace PvfCode.ViewModels.DocumentFolder;

public sealed class PvfTagCommentDocument : DocumentBase
{
	public const string DocumentPathPrefix = "pvf-tag://";

	private readonly PvfCommentDtoRes request;
	private Task loadTask;

	public override string FileName => $"编辑标签: [{request.Section}]";

	public PvfCommentDto Comment
	{
		get => GetProperty(() => Comment);
		private set => SetProperty<PvfCommentDto>(() => Comment, value);
	}

	public bool IsSaving
	{
		get => GetProperty(() => IsSaving);
		private set
		{
			if (SetProperty(() => IsSaving, value))
			{
				RaisePropertyChanged(nameof(CanSave));
			}
		}
	}

	public bool CanSave => !IsLoading && !IsSaving;

	public string NickName
	{
		get => GetProperty(() => NickName);
		set => SetProperty(() => NickName, value);
	}

	public string Status
	{
		get => GetProperty(() => Status);
		private set => SetProperty(() => Status, value);
	}

	public PvfCommentDtoRes CommentRequest => request;

	public event EventHandler Saved;

	public PvfTagCommentDocument(PvfCommentDtoRes commentRequest)
		: base(CreateDocumentPath(commentRequest))
	{
		request = commentRequest ?? throw new ArgumentNullException(nameof(commentRequest));
		DocumentType = PvfFileDocumentType.PVF标签注释;
		Comment = CreateEmptyComment();
		NickName = AppCore.NickNameTemp;
	}

	public static string CreateDocumentPath(PvfCommentDtoRes commentRequest)
	{
		if (commentRequest == null)
		{
			throw new ArgumentNullException(nameof(commentRequest));
		}
		string fileType = commentRequest.FileType?.ToString() ?? "unknown";
		string commentType = commentRequest.PvfCommentType.ToString();
		string section = NormalizeSection(commentRequest.Section).ToLowerInvariant();
		return $"{DocumentPathPrefix}{fileType.ToLowerInvariant()}/{commentType.ToLowerInvariant()}/{Uri.EscapeDataString(section)}";
	}

	public static string NormalizeSection(string section)
	{
		return (section ?? string.Empty)
			.Replace("[/", string.Empty, StringComparison.Ordinal)
			.Replace("[", string.Empty, StringComparison.Ordinal)
			.Replace("]", string.Empty, StringComparison.Ordinal)
			.Replace("`", string.Empty, StringComparison.Ordinal)
			.Trim();
	}

	public Task LoadAsync()
	{
		return loadTask ??= LoadCoreAsync();
	}

	private async Task LoadCoreAsync()
	{
		SetLoading(true);
		try
		{
			ResultData<PvfCommentDto> result = await ServicePvfTabComment.Instance.GetPvfComment(request);
			if (result.IsError || result.Data == null)
			{
				Status = string.IsNullOrWhiteSpace(result.Msg) ? "未找到标签注释，可以新建。" : result.Msg;
				Comment = CreateEmptyComment();
			}
			else
			{
				Comment = result.Data;
				Status = string.Empty;
			}
			EnsureCommentIdentity();
		}
		catch (Exception ex)
		{
			Status = ex.Message;
			Comment = CreateEmptyComment();
			EnsureCommentIdentity();
		}
		finally
		{
			SetLoading(false);
		}
	}

	[Command]
	public async void Save()
	{
		await SaveAsync();
	}

	public async Task<bool> SaveAsync()
	{
		if (IsSaving)
		{
			return false;
		}
		await LoadAsync();
		IsSaving = true;
		try
		{
			EnsureCommentIdentity();
			ApplyAuthor();
			ResultData result = await ServicePvfTabComment.Instance.AddComment(Comment);
			if (result.IsError)
			{
				Status = result.Msg;
				return false;
			}
			Status = "保存成功";
			Saved?.Invoke(this, EventArgs.Empty);
			return true;
		}
		catch (Exception ex)
		{
			Status = ex.Message;
			return false;
		}
		finally
		{
			IsSaving = false;
		}
	}

	[Command]
	public void Close()
	{
		AppCore.ViewModelBase.RootDocument.OnClose(this);
	}

	private PvfCommentDto CreateEmptyComment()
	{
		return new PvfCommentDto
		{
			FileType = request.FileType,
			PvfCommentType = request.PvfCommentType,
			Section = NormalizeSection(request.Section),
			Comment = string.Empty,
			Title = string.Empty,
			OfficialDescription = string.Empty
		};
	}

	private void EnsureCommentIdentity()
	{
		if (Comment == null)
		{
			Comment = CreateEmptyComment();
		}
		Comment.FileType = request.FileType;
		Comment.PvfCommentType = request.PvfCommentType;
		Comment.Section = NormalizeSection(request.Section);
	}

	private void ApplyAuthor()
	{
		if (string.IsNullOrWhiteSpace(NickName))
		{
			return;
		}
		string nickName = NickName.Trim();
		if (string.IsNullOrWhiteSpace(Comment.Authors))
		{
			Comment.Authors = nickName;
		}
		else if (!Comment.Authors.Contains(nickName, StringComparison.Ordinal))
		{
			Comment.Authors += "|" + nickName;
		}
		AppCore.NickNameTemp = nickName;
	}

	private void SetLoading(bool value)
	{
		IsLoading = value;
		RaisePropertyChanged(nameof(CanSave));
	}

	public override void Dispose()
	{
		Saved = null;
	}
}
