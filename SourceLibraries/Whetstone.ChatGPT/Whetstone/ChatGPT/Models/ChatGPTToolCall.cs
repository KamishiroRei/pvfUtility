using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTToolCall
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("type")]
	public string? Type { get; set; } = "function";

	[JsonPropertyName("function")]
	public ChatGPTFunctionCall? Function { get; set; }
}
