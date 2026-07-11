using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ModerationCategoryScores
{
	[JsonPropertyName("hate")]
	public float Hate { get; set; }

	[JsonPropertyName("hate/threatening")]
	public float HateThreatening { get; set; }

	[JsonPropertyName("elf-harm")]
	public float SelfHarm { get; set; }

	[JsonPropertyName("sexual")]
	public float Sexual { get; set; }

	[JsonPropertyName("sexual/minors")]
	public float SexualMinors { get; set; }

	[JsonPropertyName("violence")]
	public float Violence { get; set; }

	[JsonPropertyName("violence/graphic")]
	public float ViolenceGraphic { get; set; }
}
