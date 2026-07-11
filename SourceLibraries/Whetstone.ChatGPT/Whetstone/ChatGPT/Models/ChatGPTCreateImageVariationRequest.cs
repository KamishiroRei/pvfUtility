using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTCreateImageVariationRequest
{
	[JsonPropertyName("image")]
	public ChatGPTFileContent? Image { get; set; }

	[DefaultValue(1)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("n")]
	public int NumberOfImagesToGenerate { get; set; } = 1;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[DefaultValue(CreatedImageSize.Size1024)]
	[JsonConverter(typeof(EnumConverter<CreatedImageSize>))]
	[JsonPropertyName("size")]
	public CreatedImageSize Size { get; set; } = CreatedImageSize.Size1024;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[DefaultValue(CreatedImageFormat.Url)]
	[JsonConverter(typeof(EnumConverter<CreatedImageFormat>))]
	[JsonPropertyName("response_format")]
	public CreatedImageFormat ResponseFormat { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("user")]
	public string? User { get; set; }
}
