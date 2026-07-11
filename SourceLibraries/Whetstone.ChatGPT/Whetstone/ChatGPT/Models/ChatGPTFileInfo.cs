using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

[DebuggerDisplay("Id = {Id}, filename = {Filename}, Purpose = {Purpose}")]
public class ChatGPTFileInfo
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("object")]
	public string? Object { get; set; }

	[JsonPropertyName("bytes")]
	public int Bytes { get; set; }

	[JsonConverter(typeof(UnixEpochTimeJsonConverter))]
	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }

	[JsonPropertyName("filename")]
	public string? Filename { get; set; }

	[JsonPropertyName("purpose")]
	public string? Purpose { get; set; }
}
