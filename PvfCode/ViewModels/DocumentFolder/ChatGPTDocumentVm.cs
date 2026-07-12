using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.AiAssistant;
using PvfCode.ViewModels.DocumentFolder.Enums;

namespace PvfCode.ViewModels.DocumentFolder;

public class ChatGPTDocumentVm : DocumentBase
{
	private readonly AiAssistantSettingsStore _settingsStore;

	private readonly KnowledgePackService _knowledgePack;

	private readonly AiAssistantAgent _agent;

	private CancellationTokenSource? _requestCancellation;

	private bool _disposed;

	public ObservableCollection<AiChatMessage> Messages { get; } = new();

	public string Keyword
	{
		get => GetProperty(() => Keyword);
		set => SetProperty(() => Keyword, value);
	}

	public string ApiEndpoint
	{
		get => GetProperty(() => ApiEndpoint);
		set => SetProperty(() => ApiEndpoint, value);
	}

	public string Model
	{
		get => GetProperty(() => Model);
		set => SetProperty(() => Model, value);
	}

	public string ApiKey
	{
		get => GetProperty(() => ApiKey);
		set => SetProperty(() => ApiKey, value);
	}

	public int MaxOutputTokens
	{
		get => GetProperty(() => MaxOutputTokens);
		set => SetProperty(() => MaxOutputTokens, value);
	}

	public bool AllowPvfRead
	{
		get => GetProperty(() => AllowPvfRead);
		set => SetProperty(() => AllowPvfRead, value);
	}

	public bool IsSending
	{
		get => GetProperty(() => IsSending);
		private set => SetProperty(() => IsSending, value);
	}

	public string StatusText
	{
		get => GetProperty(() => StatusText);
		private set => SetProperty(() => StatusText, value);
	}

	public ChatGPTDocumentVm(string documentPath = "chatGPT")
		: base(documentPath)
	{
		base.DocumentType = PvfFileDocumentType.chatGPT;
		base.Icon = Res.Instance.ChatGPTICON;
		_settingsStore = new AiAssistantSettingsStore();
		_knowledgePack = new KnowledgePackService();
		_agent = new AiAssistantAgent(new OpenAiCompatibleChatGateway());

		AiAssistantConnection connection = _settingsStore.Load();
		ApiEndpoint = connection.Endpoint;
		Model = connection.Model;
		ApiKey = connection.ApiKey;
		MaxOutputTokens = connection.MaxOutputTokens;
		StatusText = _knowledgePack.IsAvailable ? "就绪" : "知识包不可用";
	}

	[Command]
	public async Task OnSend()
	{
		if (_disposed || IsSending || string.IsNullOrWhiteSpace(Keyword))
		{
			return;
		}

		AiAssistantConnection connection = CreateConnectionSnapshot();
		try
		{
			connection.Validate();
		}
		catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
		{
			StatusText = ex.Message;
			AppCore.ShowMsg(ex.Message, isError: true);
			return;
		}

		string prompt = Keyword.Trim();
		Keyword = string.Empty;
		Messages.Add(new AiChatMessage(AiCompletionRole.User, prompt));
		IReadOnlyList<AiCompletionMessage> conversation = Messages
			.Where(message => !message.IsError && message.Role is AiCompletionRole.User or AiCompletionRole.Assistant)
			.Select(message => new AiCompletionMessage(message.Role, message.Content))
			.ToArray();
		AiChatMessage pendingMessage = new(AiCompletionRole.Assistant, "正在处理...");
		Messages.Add(pendingMessage);

		IsSending = true;
		StatusText = "正在请求";
		_requestCancellation = new CancellationTokenSource();
		CancellationTokenSource requestCancellation = _requestCancellation;
		try
		{
			string? currentDocumentPath = AppCore.ViewModelBase?.RootDocument?.Documents
				.FirstOrDefault(document => document.IsActive)
				?.DocumentPath;
			IReadOnlyList<IAiAssistantTool> tools = PvfAssistantToolCatalog.Create(
				AppCore.ViewModelBase.PVF,
				_knowledgePack,
				currentDocumentPath,
				AllowPvfRead);
			AiAgentReply reply = await _agent.ReplyAsync(connection, conversation, tools, requestCancellation.Token);
			if (_disposed)
			{
				return;
			}

			pendingMessage.Content = reply.Content;
			int successfulTools = reply.ToolTraces.Count(trace => trace.Succeeded);
			StatusText = successfulTools == 0 ? "完成" : $"完成 · {successfulTools} 次只读查询";
		}
		catch (OperationCanceledException) when (requestCancellation.IsCancellationRequested)
		{
			if (!_disposed)
			{
				pendingMessage.IsError = true;
				pendingMessage.Content = "已取消。";
				StatusText = "已取消";
			}
		}
		catch (Exception ex)
		{
			if (!_disposed)
			{
				string message = GetSafeErrorMessage(ex, connection.ApiKey.Trim());
				pendingMessage.IsError = true;
				pendingMessage.Content = message;
				StatusText = "请求失败";
			}
		}
		finally
		{
			if (ReferenceEquals(_requestCancellation, requestCancellation))
			{
				_requestCancellation = null;
			}
			requestCancellation.Dispose();
			if (!_disposed)
			{
				IsSending = false;
			}
		}
	}

	[Command]
	public void OnCancel()
	{
		CancelCurrentRequest();
	}

	public void CancelCurrentRequest()
	{
		_requestCancellation?.Cancel();
	}

	[Command]
	public void OnClear()
	{
		if (!IsSending)
		{
			Messages.Clear();
			StatusText = _knowledgePack.IsAvailable ? "就绪" : "知识包不可用";
		}
	}

	[Command]
	public async Task OnSaveSettings()
	{
		try
		{
			await _settingsStore.SaveAsync(CreateConnectionSnapshot());
			StatusText = "连接设置已保存";
		}
		catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException)
		{
			StatusText = ex.Message;
			AppCore.ShowMsg(ex.Message, isError: true);
		}
	}

	private AiAssistantConnection CreateConnectionSnapshot()
	{
		return new AiAssistantConnection
		{
			Endpoint = ApiEndpoint,
			Model = Model,
			ApiKey = ApiKey,
			MaxOutputTokens = MaxOutputTokens
		};
	}

	private static string GetSafeErrorMessage(Exception exception, string apiKey)
	{
		string message = exception switch
		{
			HttpRequestException => exception.Message,
			TaskCanceledException => "请求超时。",
			InvalidOperationException => exception.Message,
			_ => "请求失败，请检查连接设置和服务状态。"
		};
		if (!string.IsNullOrEmpty(apiKey))
		{
			message = message.Replace(apiKey, "[REDACTED]", StringComparison.Ordinal);
		}
		return message;
	}

	public override void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;
		CancelCurrentRequest();
		ApiKey = string.Empty;
	}
}
