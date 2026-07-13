using System;
using System.Globalization;
using System.Windows.Data;
using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Converts.StoreList;

internal class ConverterStoreDataType : IValueConverter
{
	private readonly string _bookmarkLabel;

	private readonly string _macroLabel;

	private readonly string _fileListCommentLabel;

	private readonly string _scriptTagTranslationLabel;

	private readonly string _codeCompletionLabel;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (StoreType)value switch
		{
			StoreType.书签 => _bookmarkLabel,
			StoreType.宏 => _macroLabel,
			StoreType.文件资源管理器注释 => _fileListCommentLabel,
			StoreType.脚本文件标签翻译 => _scriptTagTranslationLabel,
			StoreType.代码智能提示 => _codeCompletionLabel,
			_ => _codeCompletionLabel,
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		string text = value.ToString();
		if (text == _bookmarkLabel)
		{
			return StoreType.书签;
		}
		if (text == _macroLabel)
		{
			return StoreType.宏;
		}
		if (text == _fileListCommentLabel)
		{
			return StoreType.文件资源管理器注释;
		}
		if (text == _scriptTagTranslationLabel)
		{
			return StoreType.脚本文件标签翻译;
		}
		return StoreType.代码智能提示;
	}

	public ConverterStoreDataType()
	{
		_bookmarkLabel = AppSetting.Instance.GetIlogger().GetStr("ViewStoreList_StoreDataType_Bookmark");
		_macroLabel = AppSetting.Instance.GetIlogger().GetStr("ViewStoreList_StoreDataType_Macro");
		_fileListCommentLabel = AppSetting.Instance.GetIlogger().GetStr("ViewStoreList_StoreDataType_FileListComment");
		_scriptTagTranslationLabel = AppSetting.Instance.GetIlogger().GetStr("ViewStoreList_StoreDataType_ScriptFileTabComment");
		_codeCompletionLabel = AppSetting.Instance.GetIlogger().GetStr("ViewStoreList_StoreDataType_ItemCodeHover");
	}
}
