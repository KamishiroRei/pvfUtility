using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.Models.BatchOperation.Enums;

namespace PvfCode.Converts.Batch;

public class ConverterBatchSourceComboxItem : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (TreeFilesSourceType)value switch
		{
			TreeFilesSourceType.所有待处理文件 => AppCore.Logger.GetStr("BatchOperationView_Source_AllFiles"), 
			TreeFilesSourceType.选中文件 => AppCore.Logger.GetStr("BatchOperationView_Source_SelectedFiles"), 
			_ => AppCore.Logger.GetStr("BatchOperationView_Source_SelectedFiles"), 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if (value.ToString() == AppCore.Logger.GetStr("BatchOperationView_Source_AllFiles"))
		{
			return TreeFilesSourceType.所有待处理文件;
		}
		return TreeFilesSourceType.选中文件;
	}

	public ConverterBatchSourceComboxItem()
	{
	}
}
