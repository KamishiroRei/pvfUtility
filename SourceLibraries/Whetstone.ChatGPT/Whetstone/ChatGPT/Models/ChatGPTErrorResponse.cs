using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTErrorResponse
{
	[JsonPropertyName("error")]
	public ChatGPTError? Error { get; set; }
}
