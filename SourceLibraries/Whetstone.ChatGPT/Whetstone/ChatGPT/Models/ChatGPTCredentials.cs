using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTCredentials
{
	[Required]
	[DataType(DataType.Password)]
	[JsonPropertyName("apiKey")]
	public string? ApiKey { get; set; }

	[JsonPropertyName("organization")]
	public string? Organization { get; set; }

	public ChatGPTCredentials()
	{
	}

	public ChatGPTCredentials(string apiKey)
	{
		ApiKey = apiKey;
	}

	public ChatGPTCredentials(string apiKey, string? organization)
	{
		ApiKey = apiKey;
		Organization = organization;
	}
}
