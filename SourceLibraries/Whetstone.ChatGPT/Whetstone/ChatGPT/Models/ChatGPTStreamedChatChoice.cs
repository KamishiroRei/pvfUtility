using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTStreamedChatChoice
{
	[JsonPropertyName("index")]
	public int? Index { get; set; }

	[JsonPropertyName("delta")]
	public ChatGPTChatCompletionMessage? Delta { get; set; }

	[JsonPropertyName("finish_reason")]
	public string? FinishReason { get; set; }
}
