using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class GeneratedImage
{
	[JsonPropertyName("url")]
	public string? Url { get; set; }

	[JsonPropertyName("b64_json")]
	public string? Base64 { get; set; }
}
