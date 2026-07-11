using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

[DebuggerDisplay("Text = {Text}")]
public class ChatGPTAudioTranscriptionRequest
{
	[JsonPropertyName("file")]
	public ChatGPTFileContent? File { get; set; }

	[JsonPropertyName("model")]
	public string? Model { get; set; } = "whisper-1";

	[JsonPropertyName("prompt")]
	public string? Prompt { get; set; }

	[JsonPropertyName("temperature")]
	public float Temperature { get; set; }

	[JsonPropertyName("language")]
	public string? Language { get; set; }
}
