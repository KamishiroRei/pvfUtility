using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTListResponse<T>
{
	[JsonPropertyName("object")]
	public string? Object { get; set; }

	[JsonPropertyName("data")]
	public List<T>? Data { get; set; }
}
