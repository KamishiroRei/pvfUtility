using System;
using System.Globalization;
using System.Windows.Data;

namespace PvfCode.Converts.PvfTree;

public class ConvertTreeRowIsDiff : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		PvfTreeFileDiff pvfTreeFileDiff = (PvfTreeFileDiff)value;
		if (pvfTreeFileDiff.IsFile)
		{
			return pvfTreeFileDiff.Diffs != null && pvfTreeFileDiff.Diffs.Count > 0;
		}
		return false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConvertTreeRowIsDiff()
	{
	}
}
