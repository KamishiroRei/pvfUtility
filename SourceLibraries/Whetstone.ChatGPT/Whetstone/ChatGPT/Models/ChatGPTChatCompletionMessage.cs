using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTChatCompletionMessage
{
	[DefaultValue(MessageRole.User)]
	[JsonConverter(typeof(EnumConverter<MessageRole>))]
	[JsonPropertyName("role")]
	public MessageRole Role { get; set; } = MessageRole.User;

	[JsonPropertyName("content")]
	public string? Content { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("tool_calls")]
	public List<ChatGPTToolCall>? ToolCalls { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("tool_call_id")]
	public string? ToolCallId { get; set; }
}
