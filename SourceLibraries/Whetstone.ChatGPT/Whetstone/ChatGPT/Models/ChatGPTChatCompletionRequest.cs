using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTChatCompletionRequest
{
	[JsonPropertyOrder(0)]
	[JsonInclude]
	[JsonPropertyName("model")]
	public string? Model { get; set; }

	[JsonPropertyOrder(1)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("messages")]
	public List<ChatGPTChatCompletionMessage>? Messages { get; set; }

	[JsonPropertyOrder(2)]
	[DefaultValue(16)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("max_tokens")]
	public int MaxTokens { get; set; } = 16;

	[JsonPropertyOrder(3)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("temperature")]
	public float Temperature { get; set; }

	[JsonPropertyOrder(4)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("top_p")]
	public float TopP { get; set; }

	[JsonPropertyOrder(5)]
	[DefaultValue(1)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("n")]
	public int CompletionResponses { get; set; }

	[JsonPropertyOrder(6)]
	[DefaultValue(false)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("stream")]
	public bool Stream { get; set; }

	[JsonPropertyOrder(7)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("stop")]
	public List<string>? Stop { get; set; }

	[JsonPropertyOrder(8)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("presence_penalty")]
	public float PresencePenalty { get; set; }

	[JsonPropertyOrder(9)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("frequency_penalty")]
	public float FrequencyPenalty { get; set; }

	[JsonPropertyOrder(10)]
	[DefaultValue(null)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("logit_bias")]
	public Dictionary<string, int>? LogitBias { get; set; }

	[JsonPropertyOrder(11)]
	[DefaultValue(null)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("user")]
	public string? User { get; set; }
}
