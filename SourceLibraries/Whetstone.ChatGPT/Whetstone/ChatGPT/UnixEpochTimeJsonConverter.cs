using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Whetstone.ChatGPT;

public class UnixEpochTimeJsonConverter : JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64()).UtcDateTime;
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		writer.WriteNumberValue(((DateTimeOffset)value).ToUnixTimeSeconds());
	}
}
