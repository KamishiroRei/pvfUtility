using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTCreateModerationRequest
{
	[JsonPropertyOrder(0)]
	[JsonInclude]
	[JsonPropertyName("input")]
	public List<string>? Inputs { get; set; }

	[JsonPropertyOrder(1)]
	[JsonConverter(typeof(EnumConverter<ModerationModels>))]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("model")]
	public ModerationModels? Model { get; set; }
}
