using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts.MainWindow;

public class ConvertBarStaticPvfStatus : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			return string.Format(AppCore.Logger.GetStr("mainWin_StatusBar_PvfStatus_Opened"), value);
		}
		return AppCore.Logger.GetStr("mainWin_StatusBar_PvfStatus_Ready");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConvertBarStaticPvfStatus()
	{
	}
}
