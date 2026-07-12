using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTFunctionCall
{
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	[JsonPropertyName("arguments")]
	public string? Arguments { get; set; }
}
