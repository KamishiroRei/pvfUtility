using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.ViewModels.PvfDiffTool.Enums;

namespace PvfCode.Converts.PvfDiff;

internal class ConverterPvfDiffShowMode : IValueConverter
{
	private string Sq9ayBhjkg;

	private string PeoaifidX5;

	private string cXoauTnvbT;

	private string BPlaGq4XDf;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (PvfDiffTreeShowFilesType)value switch
		{
			PvfDiffTreeShowFilesType.路径差异 => Sq9ayBhjkg, 
			PvfDiffTreeShowFilesType.文件内容差异 => PeoaifidX5, 
			PvfDiffTreeShowFilesType.路径差异和文件差异 => cXoauTnvbT, 
			PvfDiffTreeShowFilesType.所有文件 => BPlaGq4XDf, 
			_ => BPlaGq4XDf, 
		};
	}

	public object ConvertBack(object P_0, Type P_1, object P_2, CultureInfo P_3)
	{
		if (P_0 == null)
		{
			return null;
		}
		string text = P_0.ToString();
		if (text == Sq9ayBhjkg)
		{
			return PvfDiffTreeShowFilesType.路径差异;
		}
		if (text == PeoaifidX5)
		{
			return PvfDiffTreeShowFilesType.文件内容差异;
		}
		if (text == cXoauTnvbT)
		{
			return PvfDiffTreeShowFilesType.路径差异和文件差异;
		}
		return PvfDiffTreeShowFilesType.所有文件;
	}

	public ConverterPvfDiffShowMode()
	{
		Sq9ayBhjkg = AppSetting.Instance.GetIlogger().GetStr("PvfDiffControl_ShowMode_PathDiff");
		PeoaifidX5 = AppSetting.Instance.GetIlogger().GetStr("PvfDiffControl_ShowMode_FileContentDiff");
		cXoauTnvbT = AppSetting.Instance.GetIlogger().GetStr("PvfDiffControl_ShowMode_PathAndFileDiff");
		BPlaGq4XDf = AppSetting.Instance.GetIlogger().GetStr("PvfDiffControl_ShowMode_AllFile");
	}
}
