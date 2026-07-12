using System;
using DevExpress.Mvvm;

namespace PvfCode.AiAssistant;

public sealed class AiChatMessage : ViewModelBase
{
	public AiCompletionRole Role { get; }

	public bool IsError
	{
		get => GetProperty(() => IsError);
		set
		{
			SetProperty(() => IsError, value);
			RaisePropertyChanged(nameof(RoleLabel));
		}
	}

	public bool IsUser => Role == AiCompletionRole.User;

	public string RoleLabel => IsUser ? "你" : IsError ? "错误" : "PVF 助手";

	public string TimestampLabel { get; }

	public string Content
	{
		get => GetProperty(() => Content);
		set => SetProperty(() => Content, value);
	}

	public AiChatMessage(AiCompletionRole role, string content, bool isError = false)
	{
		Role = role;
		IsError = isError;
		Content = content;
		TimestampLabel = DateTime.Now.ToString("HH:mm");
	}
}
