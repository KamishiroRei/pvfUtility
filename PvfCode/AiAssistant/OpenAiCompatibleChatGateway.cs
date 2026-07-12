using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#if RECOVERED_WHESTONE_CHATGPT_SOURCE
using System.Linq;
using System.Net;
using System.Text.Json;
using Whetstone.ChatGPT;
using Whetstone.ChatGPT.Models;
#endif

namespace PvfCode.AiAssistant;

public sealed class OpenAiCompatibleChatGateway : IAiChatCompletionGateway
{
#if RECOVERED_WHESTONE_CHATGPT_SOURCE
	private const int MaximumResponseBytes = 2 * 1024 * 1024;

	private const string InvalidResponseMessage = "API 返回了无法识别的 Chat Completions 响应。";

	private static readonly HttpClient SharedClient = new()
	{
		Timeout = TimeSpan.FromMinutes(5)
	};

	private readonly HttpClient _httpClient;
#endif

	public OpenAiCompatibleChatGateway(HttpClient? httpClient = null)
	{
#if RECOVERED_WHESTONE_CHATGPT_SOURCE
		_httpClient = httpClient ?? SharedClient;
#else
		_ = httpClient;
#endif
	}

	public Task<AiCompletionMessage> CompleteAsync(
		AiAssistantConnection connection,
		IReadOnlyList<AiCompletionMessage> messages,
		IReadOnlyList<AiToolDefinition> tools,
		CancellationToken cancellationToken)
	{
#if RECOVERED_WHESTONE_CHATGPT_SOURCE
		return CompleteCoreAsync(connection, messages, tools, cancellationToken);
#else
		ArgumentNullException.ThrowIfNull(connection);
		connection.Validate();
		cancellationToken.ThrowIfCancellationRequested();
		_ = messages;
		_ = tools;
		return Task.FromException<AiCompletionMessage>(new NotSupportedException(
			"AI 助手需要使用恢复源码的 All、Core 或 Leaf 构建模式。"));
#endif
	}

#if RECOVERED_WHESTONE_CHATGPT_SOURCE
	private async Task<AiCompletionMessage> CompleteCoreAsync(
		AiAssistantConnection connection,
		IReadOnlyList<AiCompletionMessage> messages,
		IReadOnlyList<AiToolDefinition> tools,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(connection);
		connection.Validate();
		string apiKey = connection.ApiKey.Trim();
		var request = new ChatGPTChatCompletionRequest
		{
			Model = connection.Model.Trim(),
			Messages = messages.Select(ToChatMessage).ToList(),
			MaxTokens = connection.MaxOutputTokens,
			Tools = tools.Count == 0 ? null : tools.Select(ToChatTool).ToList(),
			ToolChoice = tools.Count == 0 ? null : "auto"
		};

		ChatGPTChatCompletionResponse? response;
		using (var client = new ChatGPTClient(
			new ChatGPTCredentials(apiKey),
			_httpClient,
			connection.GetBaseUri(),
			MaximumResponseBytes))
		{
			try
			{
				response = await client.CreateChatCompletionAsync(request, cancellationToken).ConfigureAwait(false);
			}
			catch (ChatGPTResponseTooLargeException ex)
			{
				throw new InvalidOperationException("API 响应超过 2 MiB 安全上限。", ex);
			}
			catch (ChatGPTException ex) when (ex.StatusCode.HasValue)
			{
				throw CreateHttpException(
					ex.StatusCode.Value,
					ex.ChatGPTError?.Message ?? ex.Message,
					apiKey);
			}
			catch (ChatGPTException ex)
			{
				throw new InvalidOperationException(
					InvalidResponseMessage + " " + SanitizeDetail(ex.Message, apiKey));
			}
			catch (JsonException ex)
			{
				throw new InvalidOperationException(InvalidResponseMessage, ex);
			}
		}

		ChatGPTChatCompletionMessage? message = response?.Choices is { Count: > 0 } choices
			? choices[0].Message
			: null;
		if (message == null || message.Role != MessageRole.Assistant)
		{
			throw new InvalidOperationException(InvalidResponseMessage);
		}

		return new AiCompletionMessage(
			AiCompletionRole.Assistant,
			message.Content,
			FromChatToolCalls(message.ToolCalls));
	}

	private static ChatGPTChatCompletionMessage ToChatMessage(AiCompletionMessage message)
	{
		return new ChatGPTChatCompletionMessage
		{
			Role = message.Role switch
			{
				AiCompletionRole.System => MessageRole.System,
				AiCompletionRole.User => MessageRole.User,
				AiCompletionRole.Assistant => MessageRole.Assistant,
				AiCompletionRole.Tool => MessageRole.Tool,
				_ => throw new ArgumentOutOfRangeException(nameof(message))
			},
			Content = message.Content,
			ToolCallId = message.ToolCallId,
			ToolCalls = message.ToolCalls?.Select(call => new ChatGPTToolCall
			{
				Id = call.Id,
				Type = "function",
				Function = new ChatGPTFunctionCall
				{
					Name = call.Name,
					Arguments = call.ArgumentsJson
				}
			}).ToList()
		};
	}

	private static ChatGPTTool ToChatTool(AiToolDefinition tool)
	{
		return new ChatGPTTool
		{
			Type = "function",
			Function = new ChatGPTFunctionDefinition
			{
				Name = tool.Name,
				Description = tool.Description,
				Parameters = tool.Parameters
			}
		};
	}

	private static IReadOnlyList<AiToolCall>? FromChatToolCalls(IReadOnlyList<ChatGPTToolCall>? toolCalls)
	{
		if (toolCalls == null)
		{
			return null;
		}

		List<AiToolCall> calls = new(toolCalls.Count);
		foreach (ChatGPTToolCall toolCall in toolCalls)
		{
			if (toolCall.Id == null || toolCall.Function?.Name == null)
			{
				throw new InvalidOperationException(InvalidResponseMessage);
			}
			calls.Add(new AiToolCall(
				toolCall.Id,
				toolCall.Function.Name,
				toolCall.Function.Arguments ?? "{}"));
		}
		return calls;
	}

	private static HttpRequestException CreateHttpException(HttpStatusCode statusCode, string? detail, string apiKey)
	{
		string safeDetail = SanitizeDetail(detail, apiKey);
		return new HttpRequestException($"Chat API 返回 {(int)statusCode}: {safeDetail}", null, statusCode);
	}

	private static string SanitizeDetail(string? detail, string apiKey)
	{
		string safeDetail = string.IsNullOrWhiteSpace(detail) ? "请求失败" : detail;
		if (!string.IsNullOrEmpty(apiKey))
		{
			safeDetail = safeDetail.Replace(apiKey, "[REDACTED]", StringComparison.Ordinal);
		}
		return safeDetail.Length > 500 ? safeDetail[..500] : safeDetail;
	}
#endif
}
