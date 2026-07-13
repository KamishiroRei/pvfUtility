using System.Collections.Generic;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.ViewModels.PvfDiffTool;

public class DiffEvents
{
	public delegate void DiffExtractSelectedDelegate(TreeViewType treeViewType, IEnumerable<string> fileList);

	public delegate void DiffSelectedFileCopyToPvf(TreeViewType treeViewType, IEnumerable<string> fileList);

	public event DiffSelectedFileCopyToPvf DiffSelectedFileCopyTo;

	public DiffEvents()
	{
	}
}
