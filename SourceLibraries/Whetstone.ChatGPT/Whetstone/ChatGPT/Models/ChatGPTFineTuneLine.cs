using System;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTFineTuneLine
{
	[JsonPropertyName("prompt")]
	public string? Prompt { get; set; }

	[JsonPropertyName("completion")]
	public string? Completion { get; set; }

	public ChatGPTFineTuneLine()
	{
	}

	public ChatGPTFineTuneLine(string prompt, string completion)
	{
		Prompt = prompt ?? throw new ArgumentNullException("prompt");
		Completion = completion ?? throw new ArgumentNullException("completion");
	}
}
