using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.Models.Options.Enums;

namespace PvfCode.Converts.independent_drop;

public class Converterdrop_RadioButton_InsertPositionType : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (InsertListOrder)value switch
		{
			InsertListOrder.首行插入 => AppCore.Logger.GetStr("ViewIndependent_drop_RadioButton_InsertPosition_Top"), 
			InsertListOrder.尾行插入 => AppCore.Logger.GetStr("ViewIndependent_drop_RadioButton_InsertPosition_Bottom"), 
			_ => AppCore.Logger.GetStr("ViewIndependent_drop_RadioButton_InsertPosition_Bottom"), 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (value.ToString() == AppCore.Logger.GetStr("ViewIndependent_drop_RadioButton_InsertPosition_Top"))
		{
			return InsertListOrder.首行插入;
		}
		return InsertListOrder.尾行插入;
	}

	public Converterdrop_RadioButton_InsertPositionType()
	{
	}
}
