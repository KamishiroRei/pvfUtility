using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTCompletionStreamResponse
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("object")]
	public string? Object { get; set; }

	[JsonConverter(typeof(UnixEpochTimeJsonConverter))]
	[JsonPropertyName("created")]
	public DateTime Created { get; set; }

	[JsonPropertyName("choices")]
	public List<ChatGPTChoice>? Choices { get; set; }

	[JsonPropertyName("model")]
	public string? Model { get; set; }
}
