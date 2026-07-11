using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTCreateEditRequest
{
	[JsonPropertyOrder(0)]
	[JsonPropertyName("model")]
	[JsonInclude]
	public string? Model { get; set; }

	[JsonPropertyOrder(1)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("input")]
	public string? Input { get; set; }

	[JsonPropertyOrder(2)]
	[JsonPropertyName("instruction")]
	[JsonInclude]
	public string? Instruction { get; set; }

	[JsonPropertyOrder(3)]
	[DefaultValue(1)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("n")]
	public int EditResponses { get; set; }

	[JsonPropertyOrder(4)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("Temperature")]
	public float Temperature { get; set; }

	[JsonPropertyOrder(5)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("top_p")]
	public float TopP { get; set; }
}
