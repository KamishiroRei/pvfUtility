using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Collections.Pooled;
using PvfCode;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.ViewModels.TreeFolder.Enums;

namespace dRvgUYFXgiUlumD56M2;

internal class qjqilnF7lAFbCxZ5lIf : ModelBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass17_0
	{
		public TreeGroup treeGroup;

		public IEnumerable<KeyValuePair<string, PvfTreeFileBase>> PXWLijs7Fh;

		public _003C_003Ec__DisplayClass17_0()
		{
		}

		internal Task? kChLyuEmC4()
		{
			return treeGroup.SetFilesCutStatus(PXWLijs7Fh);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_0
	{
		public qjqilnF7lAFbCxZ5lIf b7gLGZwWUd;

		public TreeGroup treeGroup;

		public _003C_003Ec__DisplayClass18_0()
		{
		}

		internal Task? AU2Lu3ZVya()
		{
			return treeGroup.ClearFileCopyStatus(b7gLGZwWUd.lpeFRRxb72);
		}
	}

	private static qjqilnF7lAFbCxZ5lIf WTorCbAuxx;

	[CompilerGenerated]
	private HashSet<string>? Kb2rHjxCmq;

	[CompilerGenerated]
	private TreeFileCopyStatus? MpUrhaZ73Y;

	[CompilerGenerated]
	private TreeViewType? T7Lrv7SpF9;

	public static qjqilnF7lAFbCxZ5lIf Instance
	{
		get
		{
			if (WTorCbAuxx == null)
			{
				WTorCbAuxx = new qjqilnF7lAFbCxZ5lIf();
			}
			return WTorCbAuxx;
		}
	}

	public HashSet<string>? lpeFRRxb72
	{
		[CompilerGenerated]
		get
		{
			return Kb2rHjxCmq;
		}
		[CompilerGenerated]
		set
		{
			Kb2rHjxCmq = value;
		}
	}

	public TreeViewType? TreeType
	{
		[CompilerGenerated]
		get
		{
			return T7Lrv7SpF9;
		}
		[CompilerGenerated]
		set
		{
			T7Lrv7SpF9 = value;
		}
	}

	public bool PasedIsEnabled => lpeFRRxb72 != null;

	[SpecialName]
	[CompilerGenerated]
	public TreeFileCopyStatus? F4OFNpbEoT()
	{
		return MpUrhaZ73Y;
	}

	[SpecialName]
	[CompilerGenerated]
	public void ruCFzf6r5g(TreeFileCopyStatus? P_0)
	{
		MpUrhaZ73Y = P_0;
	}

	public async Task Pi8FpQMdM7(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> treeFiles, TreeFileCopyStatus? P_1, TreeViewType? P_2)
	{
		_003C_003Ec__DisplayClass17_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass17_0();
		CS_0024_003C_003E8__locals7.PXWLijs7Fh = treeFiles;
		await D8FFUxEc3P();
		CS_0024_003C_003E8__locals7.treeGroup = ((TreeType == TreeViewType.SearchResult) ? AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData : AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData);
		if (P_1 == TreeFileCopyStatus.剪切)
		{
			await Task.Run(() => CS_0024_003C_003E8__locals7.treeGroup.SetFilesCutStatus(CS_0024_003C_003E8__locals7.PXWLijs7Fh));
		}
		if (AppSetting.Instance.TreeSetting.CopyFilesSetToClipboard)
		{
			PooledSet<string> pooledSet = CS_0024_003C_003E8__locals7.treeGroup.SelectedNodesToFilePaths(CS_0024_003C_003E8__locals7.PXWLijs7Fh, GetTreeType.File);
			if (pooledSet != null && pooledSet.Count > 0)
			{
				AppCore.CopyString(string.Join("\r\n", pooledSet));
			}
			else
			{
				AppCore.CopyString("");
			}
		}
		lpeFRRxb72 = CS_0024_003C_003E8__locals7.PXWLijs7Fh.Select<KeyValuePair<string, PvfTreeFileBase>, string>((KeyValuePair<string, PvfTreeFileBase> it) => it.Value.FullPath).ToHashSet();
		ruCFzf6r5g(P_1);
		TreeType = P_2;
		DoNotify("PasedIsEnabled");
	}

	public async Task D8FFUxEc3P()
	{
		_003C_003Ec__DisplayClass18_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass18_0();
		CS_0024_003C_003E8__locals4.b7gLGZwWUd = this;
		if (F4OFNpbEoT() == TreeFileCopyStatus.剪切)
		{
			CS_0024_003C_003E8__locals4.treeGroup = ((TreeType == TreeViewType.SearchResult) ? AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData : AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData);
			await Task.Run(() => CS_0024_003C_003E8__locals4.treeGroup.ClearFileCopyStatus(CS_0024_003C_003E8__locals4.b7gLGZwWUd.lpeFRRxb72));
		}
	}

	public async void ckWFcDK6M8(string P_0)
	{
		if (lpeFRRxb72 == null)
		{
			return;
		}
		if (F4OFNpbEoT() != TreeFileCopyStatus.从磁盘导入)
		{
			WindowLoading loading = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Pasting"), Application.Current.MainWindow);
			loading.Show();
			TreeGroup treeGroup = ((TreeType == TreeViewType.SearchResult) ? AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData : AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData);
			PvfGroup pVF = AppCore.ViewModelBase.PVF;
			if (F4OFNpbEoT() == TreeFileCopyStatus.剪切)
			{
				treeGroup.DeleteTreeNode(lpeFRRxb72);
			}
			List<string> list = new List<string>();
			foreach (string item in lpeFRRxb72)
			{
				foreach (string item2 in pVF.MoveFile(item, P_0, F4OFNpbEoT() == TreeFileCopyStatus.剪切))
				{
					list.Add(item2);
				}
			}
			if (list.Count > 0)
			{
				await AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(list));
			}
			loading.Close();
		}
		TcyF8C1Thg();
	}

	public void TcyF8C1Thg()
	{
		lpeFRRxb72 = null;
		ruCFzf6r5g(null);
		TreeType = null;
		DoNotify("PasedIsEnabled");
	}

	public qjqilnF7lAFbCxZ5lIf()
	{
	}
}
