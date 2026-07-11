using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.NPK.Utils.AniModel.Enums;

namespace PvfCode.Converts.AniDesigner;

public class ConverterFLIP_TYPE_X : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return 1;
		}
		if (value is FLIP_TYPE_Item fLIP_TYPE_Item)
		{
			return (fLIP_TYPE_Item != FLIP_TYPE_Item.VERTICAL) ? 1 : (-1);
		}
		return 1;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public ConverterFLIP_TYPE_X()
	{
	}
}
