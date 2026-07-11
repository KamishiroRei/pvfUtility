using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class HyperParams
{
	[JsonPropertyName("batch_size")]
	public int? BatchSize { get; set; }

	[JsonPropertyName("learning_rate_multiplier")]
	public float? LearningRateMultiplier { get; set; }

	[JsonPropertyName("n_epochs")]
	public int NumberOfEpochs { get; set; }

	[JsonPropertyName("prompt_loss_weight")]
	public float PromptLossWeight { get; set; }
}
