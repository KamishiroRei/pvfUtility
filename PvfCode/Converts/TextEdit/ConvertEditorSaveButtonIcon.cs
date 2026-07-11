using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts.TextEdit;

public class ConvertEditorSaveButtonIcon : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || !System.Convert.ToBoolean(value))
		{
			return Res.Instance.Editor.SaveFileDialogControl_16x;
		}
		return Res.Instance.Editor.SaveFileDialogControl_16x_2;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConvertEditorSaveButtonIcon()
	{
	}
}
