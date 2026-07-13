using System;
using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.Macro;

namespace PvfCode.Views.Macro;

internal class MacroTreeNodeImageSelector : TreeListNodeImageSelector
{
	public override ImageSource Select(TreeListRowData rowData)
	{
		if (rowData == null)
		{
			return null;
		}
		KeyValuePair<string, MacroData> keyValuePair = (KeyValuePair<string, MacroData>)rowData.Row;
		if (keyValuePair.Value.IsFile)
		{
			return Res.Instance.MacroIcon;
		}
		if (keyValuePair.Value.IsRoot)
		{
			return (MacroType)Enum.Parse(typeof(MacroType), keyValuePair.Key) switch
			{
				MacroType.全局搜索 => Res.Instance.FindinFiles_16x, 
				MacroType.批量处理 => Res.Instance.CollapseGroup_16x, 
				_ => null, 
			};
		}
		if (rowData.IsExpanded)
		{
			return Res.Instance.TreeFiles.FolderOpened;
		}
		return Res.Instance.TreeFiles.FolderClosed;
	}

	public MacroTreeNodeImageSelector()
	{
	}
}
