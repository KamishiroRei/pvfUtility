using System.Collections.Generic;
using System.Text.Json;

namespace PvfCode.AiAssistant;

public enum AiCompletionRole
{
	System,
	User,
	Assistant,
	Tool
}

public sealed record AiToolDefinition(string Name, string Description, JsonElement Parameters);

public sealed record AiToolCall(string Id, string Name, string ArgumentsJson);

public sealed record AiCompletionMessage(
	AiCompletionRole Role,
	string? Content,
	IReadOnlyList<AiToolCall>? ToolCalls = null,
	string? ToolCallId = null);

public interface IAiChatCompletionGateway
{
	System.Threading.Tasks.Task<AiCompletionMessage> CompleteAsync(
		AiAssistantConnection connection,
		IReadOnlyList<AiCompletionMessage> messages,
		IReadOnlyList<AiToolDefinition> tools,
		System.Threading.CancellationToken cancellationToken);
}
