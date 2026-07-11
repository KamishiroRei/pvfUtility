using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace HL.Manager;

internal static class ExtensionMethods
{
	public const double Epsilon = 0.01;

	public static bool IsClose(this double d1, double d2)
	{
		if (d1 == d2)
		{
			return true;
		}
		return Math.Abs(d1 - d2) < 0.01;
	}

	public static double CoerceValue(this double value, double minimum, double maximum)
	{
		return Math.Max(Math.Min(value, maximum), minimum);
	}

	public static int CoerceValue(this int value, int minimum, int maximum)
	{
		return Math.Max(Math.Min(value, maximum), minimum);
	}

	public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elements)
	{
		foreach (T element in elements)
		{
			collection.Add(element);
		}
	}

	public static IEnumerable<T> Sequence<T>(T value)
	{
		yield return value;
	}

	public static string GetAttributeOrNull(this XmlElement element, string attributeName)
	{
		return element.GetAttributeNode(attributeName)?.Value;
	}

	public static bool? GetBoolAttribute(this XmlElement element, string attributeName)
	{
		XmlAttribute attributeNode = element.GetAttributeNode(attributeName);
		if (attributeNode == null)
		{
			return null;
		}
		return XmlConvert.ToBoolean(attributeNode.Value);
	}

	public static bool? GetBoolAttribute(this XmlReader reader, string attributeName)
	{
		string attribute = reader.GetAttribute(attributeName);
		if (attribute == null)
		{
			return null;
		}
		return XmlConvert.ToBoolean(attribute);
	}

	[Conditional("DEBUG")]
	public static void Log(bool condition, string format, params object[] args)
	{
		if (condition)
		{
			Console.WriteLine(DateTime.Now.ToString("hh:MM:ss") + ": " + string.Format(format, args) + Environment.NewLine + Environment.StackTrace);
		}
	}
}
