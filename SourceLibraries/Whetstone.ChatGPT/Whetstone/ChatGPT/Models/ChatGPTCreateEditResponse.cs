using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTCreateEditResponse
{
	[JsonPropertyName("object")]
	public string? Object { get; set; }

	[JsonConverter(typeof(UnixEpochTimeJsonConverter))]
	[JsonPropertyName("created")]
	public DateTime Created { get; set; }

	[JsonPropertyName("choices")]
	public List<ChatGPTChoice>? Choices { get; set; }

	[JsonPropertyName("usage")]
	public ChatGPTUsage? Usage { get; set; }
}
