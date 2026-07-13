using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;

namespace PvfCode.ViewModels.TreeFolder;

public class PvfTreeNodeImageSelector : TreeListNodeImageSelector
{
	public override ImageSource Select(TreeListRowData rowData)
	{
		if (rowData == null)
		{
			return null;
		}
		KeyValuePair<string, PvfTreeFileBase> keyValuePair = (KeyValuePair<string, PvfTreeFileBase>)rowData.Row;
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
		if (rowData.IsExpanded)
		{
			return Res.Instance.TreeFiles.FolderOpened;
		}
		return Res.Instance.TreeFiles.FolderClosed;
	}

	public PvfTreeNodeImageSelector()
	{
	}
}
