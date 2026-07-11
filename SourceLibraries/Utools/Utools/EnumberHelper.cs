using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Utools;

public class EnumberHelper
{
	public static List<EnumberEntity> EnumToList<T>()
	{
		List<EnumberEntity> list = new List<EnumberEntity>();
		foreach (object value in Enum.GetValues(typeof(T)))
		{
			EnumberEntity enumberEntity = new EnumberEntity();
			object[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), inherit: true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				DescriptionAttribute descriptionAttribute = customAttributes[0] as DescriptionAttribute;
				enumberEntity.Desction = descriptionAttribute.Description;
			}
			enumberEntity.EnumValue = Convert.ToInt32(value);
			enumberEntity.EnumName = value.ToString();
			list.Add(enumberEntity);
		}
		return list;
	}

	public static T StringToEnum<T>(string obj)
	{
		return (T)Enum.Parse(typeof(T), obj);
	}

	public static List<T> EnumToEnumList<T>()
	{
		List<T> list = new List<T>();
		foreach (object value in Enum.GetValues(typeof(T)))
		{
			list.Add((T)Enum.Parse(typeof(T), value.ToString()));
		}
		return list;
	}

	public EnumberHelper()
	{
	}
}
