using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTChatChoice
{
	[JsonPropertyName("index")]
	public int? Index { get; set; }

	[JsonPropertyName("message")]
	public ChatGPTChatCompletionMessage? Message { get; set; }

	[JsonPropertyName("finish_reason")]
	public string? FinishReason { get; set; }
}
