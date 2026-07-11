using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PvfCode.NPK.Utils.AniModel;

namespace PvfCode.NPK.Utils.Converter;

public class ConverterColorToRgba : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Color.FromArgb(0, 0, 0, 0);
		}
		if (value is RGBA rGBA)
		{
			return Color.FromArgb((byte)rGBA.A, (byte)rGBA.R, (byte)rGBA.G, (byte)rGBA.B);
		}
		return Color.FromArgb(0, 0, 0, 0);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return new RGBA();
		}
		if (value is Color color)
		{
			return new RGBA((int)color.R, (int)color.G, (int)color.B, (int)color.A);
		}
		return null;
	}
}
