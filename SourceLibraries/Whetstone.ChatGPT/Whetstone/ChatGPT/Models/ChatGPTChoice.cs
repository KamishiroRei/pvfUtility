using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

[DebuggerDisplay("Text = {Text}")]
public class ChatGPTChoice
{
	[JsonPropertyName("text")]
	public string? Text { get; set; }

	[JsonPropertyName("index")]
	public int Index { get; set; }

	[JsonPropertyName("logprobs")]
	public object? LogProbabilities { get; set; }

	[JsonPropertyName("finish_reason")]
	public string? FinishReason { get; set; }
}
