using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

[DebuggerDisplay("Message = {Message}, Object = {@Object}, Level = {Level}, CreatedAt = {CreatedAt}")]
public class ChatGPTEvent
{
	[JsonPropertyName("object")]
	public string? Object { get; set; }

	[JsonConverter(typeof(UnixEpochTimeJsonConverter))]
	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }

	[JsonPropertyName("level")]
	public string? Level { get; set; }

	[JsonPropertyName("message")]
	public string? Message { get; set; }
}
