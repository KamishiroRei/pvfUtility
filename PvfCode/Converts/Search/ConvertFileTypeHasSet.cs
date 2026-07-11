using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PvfCode.Converts.Search;

public class ConvertFileTypeHasSet : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (List<string>)value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || (value != null && value.ToString()?.Length == 0))
		{
			return null;
		}
		List<string> list = new List<string>();
		if (value is string)
		{
			string[] array = value.ToString()?.Split(';');
			if (array == null)
			{
				return null;
			}
			string[] array2 = array;
			foreach (string item in array2)
			{
				list.Add(item);
			}
		}
		else
		{
			list = ((List<object>)value).ConvertAll((object it) => (string)it);
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list.ToHashSet().ToList();
	}

	public ConvertFileTypeHasSet()
	{
	}
}
