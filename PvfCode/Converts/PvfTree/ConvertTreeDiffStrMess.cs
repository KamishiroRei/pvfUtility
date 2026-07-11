using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.Models.Pvf.PvfFileDiffModels.Enums;

namespace PvfCode.Converts.PvfTree;

public class ConvertTreeDiffStrMess : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		PvfTreeFileDiff pvfTreeFileDiff = (PvfTreeFileDiff)value;
		if (pvfTreeFileDiff.IsFile && pvfTreeFileDiff.Diffs != null && pvfTreeFileDiff.Diffs.Count > 0)
		{
			if (pvfTreeFileDiff.Diffs.Contains(PvfFileDiffType.FilePath))
			{
				return "Path";
			}
			return "Content";
		}
		return null;
	}

	public object ConvertBack(object P_0, Type P_1, object P_2, CultureInfo P_3)
	{
		return null;
	}

	public ConvertTreeDiffStrMess()
	{
	}
}
