using System;
using System.Reflection;
using System.Xml;
using Newtonsoft.Json;

namespace Utools;

public static class ObjectConversion
{
	public static T JsonToObject<T>(this string json)
	{
		return JsonConvert.DeserializeObject<T>(json);
	}

	public static T DeepCopy<T>(this T obj)
	{
		if (obj == null || obj is string || obj.GetType().IsValueType)
		{
			return obj;
		}
		object obj2 = Activator.CreateInstance(obj.GetType());
		FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			try
			{
				fieldInfo.SetValue(obj2, fieldInfo.GetValue(obj).DeepCopy());
			}
			catch
			{
			}
		}
		return (T)obj2;
	}

	public static long ToLong(this object obj)
	{
		try
		{
			return Convert.ToInt64(obj);
		}
		catch (Exception)
		{
			return -1L;
		}
	}

	public static int ToInt(this object obj)
	{
		try
		{
			return Convert.ToInt32(obj);
		}
		catch (Exception)
		{
			return -1;
		}
	}

	public static bool ToBoolen(this object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!bool.TryParse(obj.ToString(), out var result))
		{
			return false;
		}
		return result;
	}

	public static string ToJson(this object obj)
	{
		return JsonConvert.SerializeObject(obj);
	}

	public static string XmlToJson(this string xmlStr)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xmlStr);
		return JsonConvert.SerializeXmlNode(xmlDocument);
	}
}
