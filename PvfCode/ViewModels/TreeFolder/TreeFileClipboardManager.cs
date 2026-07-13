using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Collections.Pooled;
using PvfCode;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.ViewModels.TreeFolder.Enums;

namespace dRvgUYFXgiUlumD56M2;

internal class qjqilnF7lAFbCxZ5lIf : ModelBase
{
	private static qjqilnF7lAFbCxZ5lIf instance;

	public static qjqilnF7lAFbCxZ5lIf Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new qjqilnF7lAFbCxZ5lIf();
			}
			return instance;
		}
	}

	public HashSet<string>? FilePaths { get; set; }

	public TreeViewType? TreeType { get; set; }

	public bool PasedIsEnabled => FilePaths != null;

	public TreeFileCopyStatus? CopyStatus { get; set; }

	public async Task SetClipboardAsync(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> treeFiles, TreeFileCopyStatus? copyStatus, TreeViewType? treeType)
	{
		await ClearCutStatusAsync();
		TreeGroup treeGroup = (TreeType == TreeViewType.SearchResult) ? AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData : AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData;
		if (copyStatus == TreeFileCopyStatus.剪切)
		{
			await Task.Run(() => treeGroup.SetFilesCutStatus(treeFiles));
		}
		if (AppSetting.Instance.TreeSetting.CopyFilesSetToClipboard)
		{
			PooledSet<string> selectedFilePaths = treeGroup.SelectedNodesToFilePaths(treeFiles, GetTreeType.File);
			if (selectedFilePaths != null && selectedFilePaths.Count > 0)
			{
				AppCore.CopyString(string.Join("\r\n", selectedFilePaths));
			}
			else
			{
				AppCore.CopyString("");
			}
		}
		FilePaths = treeFiles.Select((KeyValuePair<string, PvfTreeFileBase> item) => item.Value.FullPath).ToHashSet();
		CopyStatus = copyStatus;
		TreeType = treeType;
		DoNotify("PasedIsEnabled");
	}

	public async Task ClearCutStatusAsync()
	{
		if (CopyStatus == TreeFileCopyStatus.剪切)
		{
			TreeGroup treeGroup = (TreeType == TreeViewType.SearchResult) ? AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData : AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData;
			await Task.Run(() => treeGroup.ClearFileCopyStatus(FilePaths));
		}
	}

	public async void PasteFiles(string targetPath)
	{
		if (FilePaths == null)
		{
			return;
		}
		if (CopyStatus != TreeFileCopyStatus.从磁盘导入)
		{
			WindowLoading loading = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Pasting"), Application.Current.MainWindow);
			loading.Show();
			TreeGroup treeGroup = (TreeType == TreeViewType.SearchResult) ? AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData : AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData;
			PvfGroup pvf = AppCore.ViewModelBase.PVF;
			if (CopyStatus == TreeFileCopyStatus.剪切)
			{
				treeGroup.DeleteTreeNode(FilePaths);
			}
			List<string> pastedFilePaths = new List<string>();
			foreach (string sourcePath in FilePaths)
			{
				foreach (string pastedFilePath in pvf.MoveFile(sourcePath, targetPath, CopyStatus == TreeFileCopyStatus.剪切))
				{
					pastedFilePaths.Add(pastedFilePath);
				}
			}
			if (pastedFilePaths.Count > 0)
			{
				await AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(pastedFilePaths));
			}
			loading.Close();
		}
		Clear();
	}

	public void Clear()
	{
		FilePaths = null;
		CopyStatus = null;
		TreeType = null;
		DoNotify("PasedIsEnabled");
	}

	public qjqilnF7lAFbCxZ5lIf()
	{
	}
}
