using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTCreateEmbeddingsResponse
{
	[JsonPropertyName("object")]
	public string? Object { get; set; }

	[JsonPropertyName("data")]
	public List<Embeddings>? Data { get; set; }

	[JsonPropertyName("model")]
	public string? Model { get; set; }

	[JsonPropertyName("usage")]
	public ChatGPTEmbeddingUsage? Usage { get; set; }
}
