using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using PvfCode.Dot.Desktop;

namespace PvfCode.ViewModels.BookMarkEdit.Models;

public class BookMarkTreeNodeImageSelector : TreeListNodeImageSelector
{
	public override ImageSource Select(TreeListRowData P_0)
	{
		if (P_0 == null)
		{
			return null;
		}
		KeyValuePair<string, BookMarkDto> keyValuePair = (KeyValuePair<string, BookMarkDto>)P_0.Row;
		if (keyValuePair.Value.IsFile)
		{
			return AppSetting.Instance.PvfConfig.GetPvfFileType(Path.GetExtension(keyValuePair.Key)) switch
			{
				PvfFileType.lst => Res.Instance.TreeFiles.Lst, 
				PvfFileType.str => Res.Instance.TreeFiles.Str, 
				PvfFileType.ani => Res.Instance.TreeFiles.Ani, 
				PvfFileType.ui => Res.Instance.TreeFiles.Ui, 
				_ => Res.Instance.TreeFiles.Script_16x, 
			};
		}
		if (P_0.IsExpanded)
		{
			return Res.Instance.TreeFiles.FolderOpened;
		}
		return Res.Instance.TreeFiles.FolderClosed;
	}

	public BookMarkTreeNodeImageSelector()
	{
	}
}
