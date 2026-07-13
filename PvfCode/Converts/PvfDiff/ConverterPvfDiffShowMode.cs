using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.ViewModels.PvfDiffTool.Enums;

namespace PvfCode.Converts.PvfDiff;

internal class ConverterPvfDiffShowMode : IValueConverter
{
	private readonly string _pathDifferenceLabel;

	private readonly string _fileContentDifferenceLabel;

	private readonly string _pathAndFileDifferenceLabel;

	private readonly string _allFilesLabel;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (PvfDiffTreeShowFilesType)value switch
		{
			PvfDiffTreeShowFilesType.路径差异 => _pathDifferenceLabel,
			PvfDiffTreeShowFilesType.文件内容差异 => _fileContentDifferenceLabel,
			PvfDiffTreeShowFilesType.路径差异和文件差异 => _pathAndFileDifferenceLabel,
			PvfDiffTreeShowFilesType.所有文件 => _allFilesLabel,
			_ => _allFilesLabel,
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		string text = value.ToString();
		if (text == _pathDifferenceLabel)
		{
			return PvfDiffTreeShowFilesType.路径差异;
		}
		if (text == _fileContentDifferenceLabel)
		{
			return PvfDiffTreeShowFilesType.文件内容差异;
		}
		if (text == _pathAndFileDifferenceLabel)
		{
			return PvfDiffTreeShowFilesType.路径差异和文件差异;
		}
		return PvfDiffTreeShowFilesType.所有文件;
	}

	public ConverterPvfDiffShowMode()
	{
		_pathDifferenceLabel = AppSetting.Instance.GetIlogger().GetStr("PvfDiffControl_ShowMode_PathDiff");
		_fileContentDifferenceLabel = AppSetting.Instance.GetIlogger().GetStr("PvfDiffControl_ShowMode_FileContentDiff");
		_pathAndFileDifferenceLabel = AppSetting.Instance.GetIlogger().GetStr("PvfDiffControl_ShowMode_PathAndFileDiff");
		_allFilesLabel = AppSetting.Instance.GetIlogger().GetStr("PvfDiffControl_ShowMode_AllFile");
	}
}
