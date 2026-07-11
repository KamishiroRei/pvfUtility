using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Utools;

public class JsonStringNullToEmpty : DefaultContractResolver
{
	public JsonStringNullToEmpty()
	{
	}

	protected override IList<JsonProperty> CreateProperties(Type objectType, MemberSerialization memberSerialization)
	{
		return objectType.GetProperties().Select(property =>
		{
			JsonProperty jsonProperty = CreateProperty(property, memberSerialization);
			jsonProperty.ValueProvider = new NullStringValueProvider(property);
			return jsonProperty;
		}).ToList();
	}
}
