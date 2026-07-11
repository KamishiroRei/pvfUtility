using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PvfCode.NPK.Utils.Converter;

public class ConverterColorToRgbaText : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (value is Color color)
		{
			return $"{color.R},{color.G},{color.B},{color.A}";
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (value is string text)
		{
			string[] array = text.Split(",", StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 4)
			{
				return Color.FromArgb(0, 0, 0, 0);
			}
			return Color.FromArgb(ToByte(array[3]), ToByte(array[0]), ToByte(array[1]), ToByte(array[2]));
		}
		return Color.FromArgb(0, 0, 0, 0);
	}

	private byte ToByte(string str)
	{
		if (!byte.TryParse(str, out var result))
		{
			return 0;
		}
		return result;
	}
}
