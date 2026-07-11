using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.ViewModels.PvfDiffTool;

public class DiffEvents
{
	public delegate void DiffExtractSelectedDelegate(TreeViewType treeViewType, IEnumerable<string> fileList);

	public delegate void DiffSelectedFileCopyToPvf(TreeViewType treeViewType, IEnumerable<string> fileList);

	[CompilerGenerated]
	private DiffSelectedFileCopyToPvf doFWZOEw3i;

	public event DiffSelectedFileCopyToPvf DiffSelectedFileCopyTo
	{
		[CompilerGenerated]
		add
		{
			DiffSelectedFileCopyToPvf diffSelectedFileCopyToPvf = doFWZOEw3i;
			DiffSelectedFileCopyToPvf diffSelectedFileCopyToPvf2;
			do
			{
				diffSelectedFileCopyToPvf2 = diffSelectedFileCopyToPvf;
				DiffSelectedFileCopyToPvf value2 = (DiffSelectedFileCopyToPvf)Delegate.Combine(diffSelectedFileCopyToPvf2, value);
				diffSelectedFileCopyToPvf = Interlocked.CompareExchange(ref doFWZOEw3i, value2, diffSelectedFileCopyToPvf2);
			}
			while ((object)diffSelectedFileCopyToPvf != diffSelectedFileCopyToPvf2);
		}
		[CompilerGenerated]
		remove
		{
			DiffSelectedFileCopyToPvf diffSelectedFileCopyToPvf = doFWZOEw3i;
			DiffSelectedFileCopyToPvf diffSelectedFileCopyToPvf2;
			do
			{
				diffSelectedFileCopyToPvf2 = diffSelectedFileCopyToPvf;
				DiffSelectedFileCopyToPvf value2 = (DiffSelectedFileCopyToPvf)Delegate.Remove(diffSelectedFileCopyToPvf2, value);
				diffSelectedFileCopyToPvf = Interlocked.CompareExchange(ref doFWZOEw3i, value2, diffSelectedFileCopyToPvf2);
			}
			while ((object)diffSelectedFileCopyToPvf != diffSelectedFileCopyToPvf2);
		}
	}

	public DiffEvents()
	{
	}
}
