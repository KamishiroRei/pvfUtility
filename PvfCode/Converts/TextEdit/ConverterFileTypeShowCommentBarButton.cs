using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts.TextEdit;

public class ConverterFileTypeShowCommentBarButton : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return false;
		}
		return (PvfFileType)value == PvfFileType.nut;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterFileTypeShowCommentBarButton()
	{
	}
}
