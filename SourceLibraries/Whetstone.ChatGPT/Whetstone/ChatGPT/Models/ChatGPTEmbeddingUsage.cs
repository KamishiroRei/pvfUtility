using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTEmbeddingUsage
{
	[JsonPropertyName("prompt_tokens")]
	public int PromptTokens { get; set; }

	[JsonPropertyName("total_tokens")]
	public int TotalTokens { get; set; }
}
