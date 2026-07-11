using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Grid.TreeList;

namespace PvfCode.Converts.PvfTree;

public class ConverterTreeImage : IMultiValueConverter
{
	private int RlraYgmDAx;

	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values[1] is TreeListRowData treeListRowData)
		{
			RlraYgmDAx++;
			if (treeListRowData == null)
			{
				return null;
			}
			if (values[0] == null)
			{
				return null;
			}
			KeyValuePair<string, PvfTreeFileBase> keyValuePair = (KeyValuePair<string, PvfTreeFileBase>)values[0];
			PvfTreeFileBase value = keyValuePair.Value;
			if (value.IsFile)
			{
				switch (AppSetting.Instance.PvfConfig.GetPvfFileType(Path.GetExtension(keyValuePair.Key)))
				{
				case PvfFileType.lst:
					return Res.Instance.TreeFiles.Lst;
				case PvfFileType.str:
					return Res.Instance.TreeFiles.Str;
				case PvfFileType.ani:
					return Res.Instance.TreeFiles.Ani;
				case PvfFileType.ui:
					return Res.Instance.TreeFiles.Ui;
				default:
				{
					if (value.File != null && value.Pvf != null && ImagePack2Service.Instance.TreeGetIcon(value.Pvf, value.File, out ImageSource imageSource) && imageSource != null)
					{
						return imageSource;
					}
					return Res.Instance.TreeFiles.Script_16x;
				}
				}
			}
			if (treeListRowData.IsExpanded)
			{
				return Res.Instance.TreeFiles.FolderOpened;
			}
			return Res.Instance.TreeFiles.FolderClosed;
		}
		return null;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}

	public ConverterTreeImage()
	{
	}
}
