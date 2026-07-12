using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PvfCode.AiAssistant;

public interface IAiAssistantTool
{
	AiToolDefinition Definition { get; }

	Task<string> ExecuteAsync(string argumentsJson, CancellationToken cancellationToken);
}

public sealed class AiAssistantTool : IAiAssistantTool
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	private readonly Func<JsonElement, CancellationToken, Task<object?>> _handler;

	public AiToolDefinition Definition { get; }

	public AiAssistantTool(
		string name,
		string description,
		string parametersJson,
		Func<JsonElement, CancellationToken, Task<object?>> handler)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);
		ArgumentNullException.ThrowIfNull(handler);
		using JsonDocument schema = JsonDocument.Parse(parametersJson);
		Definition = new AiToolDefinition(name, description, schema.RootElement.Clone());
		_handler = handler;
	}

	public async Task<string> ExecuteAsync(string argumentsJson, CancellationToken cancellationToken)
	{
		using JsonDocument arguments = JsonDocument.Parse(string.IsNullOrWhiteSpace(argumentsJson) ? "{}" : argumentsJson);
		if (arguments.RootElement.ValueKind != JsonValueKind.Object)
		{
			throw new InvalidOperationException("工具参数必须是 JSON 对象。");
		}
		object? result = await _handler(arguments.RootElement, cancellationToken).ConfigureAwait(false);
		return JsonSerializer.Serialize(result, JsonOptions);
	}
}
