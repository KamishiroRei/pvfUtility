using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Whetstone.ChatGPT;

internal static class EnumExtensions
{
	internal const BindingFlags EnumBindings = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	internal static string GetDescriptionFromEnumValue<TEnum>(this TEnum value) where TEnum : struct, Enum
	{
		string text = value.ToString();
		FieldInfo field = value.GetType().GetField(text, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if ((object)field != null)
		{
			object[] customAttributes = field.GetCustomAttributes(typeof(EnumMemberAttribute), inherit: false);
			if (customAttributes.Length != 0)
			{
				EnumMemberAttribute enumMemberAttribute = (EnumMemberAttribute)customAttributes[0];
				text = ((enumMemberAttribute.Value == null) ? text : enumMemberAttribute.Value);
			}
		}
		return text;
	}
}
