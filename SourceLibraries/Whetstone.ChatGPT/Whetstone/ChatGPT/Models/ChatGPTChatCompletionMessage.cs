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
}
