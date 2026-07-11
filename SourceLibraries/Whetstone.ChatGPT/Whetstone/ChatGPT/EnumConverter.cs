using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT;

public class EnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
	public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		string text = reader.GetString();
		if (text != null)
		{
			return GetEnumValue(text);
		}
		return default(TEnum);
	}

	public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.GetDescriptionFromEnumValue());
	}

	private static IEnumerable<TEnum> GetEnumValues()
	{
		return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
	}

	private static TEnum GetEnumValue(string enumMemberText)
	{
		if (Enum.TryParse<TEnum>(enumMemberText, ignoreCase: true, out var result))
		{
			return result;
		}
		IEnumerable<TEnum> enumValues = GetEnumValues();
		Dictionary<string, TEnum> dictionary = new Dictionary<string, TEnum>();
		foreach (TEnum item in enumValues)
		{
			string descriptionFromEnumValue = item.GetDescriptionFromEnumValue();
			if (descriptionFromEnumValue != null)
			{
				dictionary.Add(descriptionFromEnumValue, item);
			}
		}
		if (dictionary.TryGetValue(enumMemberText, out var value))
		{
			return value;
		}
		throw new JsonException("Could not resolve value " + enumMemberText + " in enum " + typeof(TEnum).FullName);
	}
}
