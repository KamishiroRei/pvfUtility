using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.ViewModels.independent_drop.Enums;

namespace PvfCode.Converts.independent_drop;

public class ConvertMonsterType : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if ((MonsterType)value == MonsterType.怪物)
		{
			return AppCore.Logger.GetStr("ViewIndependent_drop_MonsterType_Monster");
		}
		return AppCore.Logger.GetStr("ViewIndependent_drop_MonsterType_APC");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (value.ToString() == AppCore.Logger.GetStr("ViewIndependent_drop_MonsterType_Monster"))
		{
			return MonsterType.怪物;
		}
		return MonsterType.APC;
	}

	public ConvertMonsterType()
	{
	}
}
