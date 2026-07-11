using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTCreateFineTuneRequest
{
	[JsonPropertyOrder(0)]
	[JsonPropertyName("training_file")]
	public string? TrainingFileId { get; set; }

	[JsonPropertyOrder(1)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("validation_file")]
	public string? ValidationFileId { get; set; }

	[JsonPropertyOrder(2)]
	[JsonPropertyName("model")]
	public string? Model { get; set; }

	[DefaultValue(4)]
	[JsonPropertyOrder(3)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("n_epochs")]
	public int NumberOfEpochs { get; set; } = 4;

	[JsonPropertyOrder(4)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("batch_size")]
	public int? BatchSize { get; set; }

	[JsonPropertyOrder(5)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("learning_rate_multiplier")]
	public float? LearningRateMultiplier { get; set; }

	[JsonPropertyOrder(6)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("compute_classification_metrics")]
	public bool ComputeClassificationMetrics { get; set; }

	[JsonPropertyOrder(7)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("classification_n_classes")]
	public int? NumberOfClasses { get; set; }

	[JsonPropertyOrder(8)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("classification_positive_class")]
	public string? PositiveClass { get; set; }

	[JsonPropertyOrder(9)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("classification_betas")]
	public List<float>? ClassificationBetas { get; set; }

	[JsonPropertyOrder(10)]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	[JsonPropertyName("suffix")]
	public string? Suffix { get; set; }
}
