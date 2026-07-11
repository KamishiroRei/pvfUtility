using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PvfCode.ViewModels.Login;

namespace PvfCode.Converts.Login;

public class ConverterLoginViewVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return Visibility.Visible;
		}
		LoginViewType loginViewType = (LoginViewType)value;
		if (value.ToString() == parameter.ToString())
		{
			return Visibility.Visible;
		}
		if (loginViewType == LoginViewType.ForgetPassword && parameter.ToString() == "Register")
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConverterLoginViewVisibility()
	{
	}
}
