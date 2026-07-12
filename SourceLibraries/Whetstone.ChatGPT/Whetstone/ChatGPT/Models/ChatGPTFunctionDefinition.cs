using System.Text.Json;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTFunctionDefinition
{
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	[JsonPropertyName("parameters")]
	public JsonElement Parameters { get; set; } = JsonSerializer.SerializeToElement(new object());
}
