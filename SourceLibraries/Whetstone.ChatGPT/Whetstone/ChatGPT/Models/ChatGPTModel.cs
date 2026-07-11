using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

[DebuggerDisplay("Id = {Id}, OwnedBy = {OwnedBy}, CreatedAt = {CreatedAt}")]
public class ChatGPTModel
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("object")]
	public string? Object { get; set; }

	[JsonConverter(typeof(UnixEpochTimeJsonConverter))]
	[JsonPropertyName("created")]
	public DateTime Created { get; set; }

	[JsonPropertyName("owned_by")]
	public string? OwnedBy { get; set; }

	[JsonPropertyName("permission")]
	public List<ChatGPTModelPermissions>? Permission { get; set; }

	[JsonPropertyName("root")]
	public string? Root { get; set; }

	[JsonPropertyName("parent")]
	public string? Parent { get; set; }
}
