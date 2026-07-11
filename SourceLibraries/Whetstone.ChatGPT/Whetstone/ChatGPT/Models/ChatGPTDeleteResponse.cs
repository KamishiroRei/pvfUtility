using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

[DebuggerDisplay("Id = {Id}, Deleted = {Deleted}")]
public class ChatGPTDeleteResponse
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("object")]
	public string? Object { get; set; }

	[JsonPropertyName("deleted")]
	public bool Deleted { get; set; }
}
