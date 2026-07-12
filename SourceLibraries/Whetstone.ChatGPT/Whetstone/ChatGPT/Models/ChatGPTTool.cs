using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTTool
{
	[JsonPropertyName("type")]
	public string? Type { get; set; } = "function";

	[JsonPropertyName("function")]
	public ChatGPTFunctionDefinition? Function { get; set; }
}
