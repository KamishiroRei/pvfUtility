using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTCreateEmbeddingsRequest
{
	[JsonPropertyOrder(0)]
	[JsonInclude]
	[JsonPropertyName("model")]
	public string? Model { get; set; } = ChatGPTEmbeddingModels.Ada;

	[JsonPropertyOrder(1)]
	[JsonInclude]
	[JsonPropertyName("input")]
	public List<string>? Inputs { get; set; }

	[JsonPropertyOrder(2)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("user")]
	public string? User { get; set; }
}
