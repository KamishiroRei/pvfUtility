using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Converts.StoreList;

internal class ConverterStoreDataType : IValueConverter
{
	private string QX6a5NTd6G;

	private string N2raSws6CX;

	private string FileListComment;

	private string LnraAf0x0Y;

	private string Y27a4ruEY6;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (StoreType)value switch
		{
			StoreType.书签 => QX6a5NTd6G, 
			StoreType.宏 => N2raSws6CX, 
			StoreType.文件资源管理器注释 => FileListComment, 
			StoreType.脚本文件标签翻译 => LnraAf0x0Y, 
			StoreType.代码智能提示 => Y27a4ruEY6, 
			_ => Y27a4ruEY6, 
		};
	}

	public object ConvertBack(object P_0, Type P_1, object P_2, CultureInfo P_3)
	{
		if (P_0 == null)
		{
			return null;
		}
		string text = P_0.ToString();
		if (text == QX6a5NTd6G)
		{
			return StoreType.书签;
		}
		if (text == N2raSws6CX)
		{
			return StoreType.宏;
		}
		if (text == FileListComment)
		{
			return StoreType.文件资源管理器注释;
		}
		if (text == LnraAf0x0Y)
		{
			return StoreType.脚本文件标签翻译;
		}
		return StoreType.代码智能提示;
	}

	public ConverterStoreDataType()
	{
		QX6a5NTd6G = AppSetting.Instance.GetIlogger().GetStr("ViewStoreList_StoreDataType_Bookmark");
		N2raSws6CX = AppSetting.Instance.GetIlogger().GetStr("ViewStoreList_StoreDataType_Macro");
		FileListComment = AppSetting.Instance.GetIlogger().GetStr("ViewStoreList_StoreDataType_FileListComment");
		LnraAf0x0Y = AppSetting.Instance.GetIlogger().GetStr("ViewStoreList_StoreDataType_ScriptFileTabComment");
		Y27a4ruEY6 = AppSetting.Instance.GetIlogger().GetStr("ViewStoreList_StoreDataType_ItemCodeHover");
	}
}
