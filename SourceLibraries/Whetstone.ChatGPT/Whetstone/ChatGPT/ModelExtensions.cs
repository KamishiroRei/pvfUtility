using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Whetstone.ChatGPT.Models;

namespace Whetstone.ChatGPT;

public static class ModelExtensions
{
	public static string? GetCompletionText(this ChatGPTChatCompletionResponse response)
	{
		return response?.Choices?[0]?.Message?.Content;
	}

	public static ChatGPTChatCompletionMessage? GetMessage(this ChatGPTChatCompletionResponse response)
	{
		return response?.Choices?[0]?.Message;
	}

	public static string? GetCompletionText(this ChatGPTChatCompletionStreamResponse response)
	{
		return response?.Choices?[0]?.Delta?.Content;
	}

	public static ChatGPTStreamedChatChoice? GetChoice(this ChatGPTChatCompletionStreamResponse response)
	{
		return response?.Choices?[0];
	}

	public static string? GetCompletionText(this ChatGPTCompletionResponse response)
	{
		return response?.Choices?[0]?.Text;
	}

	public static string? GetCompletionText(this ChatGPTCompletionStreamResponse response)
	{
		return response?.Choices?[0]?.Text;
	}

	public static string? GetEditedText(this ChatGPTCreateEditResponse response)
	{
		return response?.Choices?[0]?.Text;
	}

	public static string ToJsonL(this IEnumerable<ChatGPTFineTuneLine> tuningLines)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ChatGPTFineTuneLine tuningLine in tuningLines)
		{
			stringBuilder.AppendLine(JsonSerializer.Serialize(tuningLine));
		}
		return stringBuilder.ToString();
	}

	public static byte[] ToJsonLBinary(this IEnumerable<ChatGPTFineTuneLine> tuningLines)
	{
		return Encoding.UTF8.GetBytes(tuningLines.ToJsonL());
	}
}
