using System;
using System.Globalization;

namespace Utools;

public static class IConvertibleExtensions
{
	public static T ConvertTo<T>(this IConvertible value) where T : IConvertible
	{
		return (T)value.ConvertTo(typeof(T));
	}

	public static T TryConvertTo<T>(this IConvertible value, T defaultValue = default(T)) where T : IConvertible
	{
		try
		{
			return (T)value.ConvertTo(typeof(T));
		}
		catch
		{
			return defaultValue;
		}
	}

	public static bool TryConvertTo<T>(this IConvertible value, out T result) where T : IConvertible
	{
		try
		{
			result = (T)value.ConvertTo(typeof(T));
			return true;
		}
		catch
		{
			result = default(T);
			return false;
		}
	}

	public static bool TryConvertTo(this IConvertible value, Type type, out object result)
	{
		try
		{
			result = value.ConvertTo(type);
			return true;
		}
		catch
		{
			result = null;
			return false;
		}
	}

	public static object ConvertTo(this IConvertible value, Type type)
	{
		if (value == null)
		{
			return null;
		}
		if (type.IsEnum)
		{
			return Enum.Parse(type, value.ToString(CultureInfo.InvariantCulture));
		}
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (!underlyingType.IsEnum)
			{
				return Convert.ChangeType(value, underlyingType);
			}
			return Enum.Parse(underlyingType, value.ToString(CultureInfo.CurrentCulture));
		}
		return Convert.ChangeType(value, type);
	}
}
