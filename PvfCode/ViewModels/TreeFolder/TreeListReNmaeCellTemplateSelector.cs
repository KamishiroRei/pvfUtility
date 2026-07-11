using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;

namespace PvfCode.ViewModels.TreeFolder;

public class TreeListReNmaeCellTemplateSelector : DataTemplateSelector
{
	[CompilerGenerated]
	private DataTemplate Ol2rzeoGc6;

	[CompilerGenerated]
	private DataTemplate ssZWDIQEXu;

	public DataTemplate Folder
	{
		[CompilerGenerated]
		get
		{
			return Ol2rzeoGc6;
		}
		[CompilerGenerated]
		set
		{
			Ol2rzeoGc6 = value;
		}
	}

	public DataTemplate File
	{
		[CompilerGenerated]
		get
		{
			return ssZWDIQEXu;
		}
		[CompilerGenerated]
		set
		{
			ssZWDIQEXu = value;
		}
	}

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
