using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;

namespace PvfCode.ViewModels.TreeFolder;

public class TreeListReNmaeCellTemplateSelector : DataTemplateSelector
{
	public DataTemplate Folder { get; set; }

	public DataTemplate File { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (!((KeyValuePair<string, PvfTreeFileBase>)((EditGridCellData)item).Row).Value.IsFile)
		{
			return Folder;
		}
		return File;
	}

	public TreeListReNmaeCellTemplateSelector()
	{
	}
}
