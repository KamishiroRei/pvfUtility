using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT.Models;

[DebuggerDisplay("Id = {Id}, FineTunedModel = {FineTunedModel}, Model = {Model}, Status = {Status}")]
public class ChatGPTFineTuneJob
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("object")]
	public string? Object { get; set; }

	[JsonPropertyName("model")]
	public string? Model { get; set; }

	[JsonConverter(typeof(UnixEpochTimeJsonConverter))]
	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }

	[JsonPropertyName("events")]
	public List<ChatGPTEvent>? Events { get; set; }

	[JsonPropertyName("fine_tuned_model")]
	public string? FineTunedModel { get; set; }

	[JsonPropertyName("hyperparams")]
	public HyperParams? HyperParams { get; set; }

	[JsonPropertyName("organization_id")]
	public string? OrganizationId { get; set; }

	[JsonPropertyName("result_files")]
	public List<ChatGPTFileInfo>? ResultFiles { get; set; }

	[JsonPropertyName("status")]
	public string? Status { get; set; }

	[JsonPropertyName("validation_files")]
	public List<ChatGPTFileInfo>? ValidationFiles { get; set; }

	[JsonPropertyName("training_files")]
	public List<ChatGPTFileInfo>? TrainingFiles { get; set; }

	[JsonConverter(typeof(UnixEpochTimeJsonConverter))]
	[JsonPropertyName("updated_at")]
	public DateTime UpdatedAt { get; set; }
}
