using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class GPTAudioSegment
{
	[JsonPropertyName("id")]
	public int? Id { get; set; }

	[JsonPropertyName("seek")]
	public int? Seek { get; set; }

	[JsonPropertyName("start")]
	public float? Start { get; set; }

	[JsonPropertyName("end")]
	public float? End { get; set; }

	[JsonPropertyName("text")]
	public string? Text { get; set; }

	[JsonPropertyName("tokens")]
	public List<int>? Tokens { get; set; }

	[JsonPropertyName("temperature")]
	public float? Temperature { get; set; }

	[JsonPropertyName("avg_logprob")]
	public double? AverageLogProbability { get; set; }

	[JsonPropertyName("compression_ratio")]
	public double? CompressionRatio { get; set; }

	[JsonPropertyName("no_speech_prob")]
	public double? NoSpeechProbability { get; set; }

	[JsonPropertyName("transient")]
	public bool Transient { get; set; }
}
