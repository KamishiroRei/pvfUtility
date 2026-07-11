using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PvfCode.Models.Pvf.PvfFileDiffModels.Enums;

namespace PvfCode.Converts.PvfTree;

public class ConvertTreeDiffTypeBackBrush : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		PvfTreeFileDiff pvfTreeFileDiff = (PvfTreeFileDiff)value;
		if (pvfTreeFileDiff.IsFile && pvfTreeFileDiff.Diffs != null)
		{
			if (pvfTreeFileDiff.Diffs.Contains(PvfFileDiffType.FilePath))
			{
				return Application.Current.TryFindResource("TreePvfDiffRowBackBrush");
			}
			return "#A7CD97";
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConvertTreeDiffTypeBackBrush()
	{
	}
}
