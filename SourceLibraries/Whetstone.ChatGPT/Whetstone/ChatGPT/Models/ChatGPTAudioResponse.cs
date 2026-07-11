using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTAudioResponse
{
	[JsonPropertyName("task")]
	public string? Task { get; set; }

	[JsonPropertyName("language")]
	public string? Language { get; set; }

	[JsonPropertyName("duration")]
	public float? Duration { get; set; }

	[JsonPropertyName("segments")]
	public List<GPTAudioSegment>? Segments { get; set; }

	[JsonPropertyName("text")]
	public string? Text { get; set; }
}
