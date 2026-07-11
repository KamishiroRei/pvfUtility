using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.ViewModels.Description.FileListDescription;

public class FileListDescriptionViewModel : ViewModelBase
{
	[CompilerGenerated]
	private Dictionary<string, TreelistCommentRes> sfLGNCXydx;

	[CompilerGenerated]
	private PvfTreeViewModel hPaGzeRkiu;

	[CompilerGenerated]
	private TreelistCommentRes Nd4xDmpbUR;

	private readonly Action Close;

	private Dictionary<string, TreelistCommentRes> Source
	{
		[CompilerGenerated]
		get
		{
			return sfLGNCXydx;
		}
		[CompilerGenerated]
		set
		{
			sfLGNCXydx = value;
		}
	}

	public PvfTreeViewModel TreeViewModel
	{
		[CompilerGenerated]
		get
		{
			return hPaGzeRkiu;
		}
		[CompilerGenerated]
		set
		{
			hPaGzeRkiu = value;
		}
	}

	public TreelistCommentRes SelectedItem
	{
		get
		{
			return GetProperty(() => SelectedItem);
		}
		set
		{
			SetProperty<TreelistCommentRes>(() => SelectedItem, value);
		}
	}

	public TreelistCommentRes AddData
	{
		[CompilerGenerated]
		get
		{
			return Nd4xDmpbUR;
		}
		[CompilerGenerated]
		set
		{
			Nd4xDmpbUR = value;
		}
	}

	public FileListDescriptionViewModel(Action close)
	{
		Source = new Dictionary<string, TreelistCommentRes>(AppSetting.Instance.PvfConfig.TreelistCommentDic);
		AddData = new TreelistCommentRes();
		TreeViewModel = new PvfTreeViewModel(TreeViewType.FileListDescription);
		TreeViewModel.SelectedRowChangedEvent += CLJGVPHaDC;
		Close = close;
	}

	[Command]
	public void OnDeleteSelectedItems()
	{
		if (!TreeViewModel.IsSelectedNodes)
		{
			return;
		}
		foreach (string selectedFilePath in TreeViewModel.GetSelectedFilePaths(GetTreeType.All))
		{
			if (Source.ContainsKey(selectedFilePath))
			{
				Source.Remove(selectedFilePath);
			}
		}
		TreeViewModel.RemoveSelectedNodes();
	}

	[Command]
	public async void Loaded()
	{
		await TreeViewModel.TreeGroupData.CreateFileListDescriptionTrees(Source.Keys, Source);
	}

	[Command]
	public void OnSelectedFilePath()
	{
	}

	[Command]
	public async void OnAdd()
	{
		if (string.IsNullOrEmpty(AddData.FilePath))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputFilePath"));
			return;
		}
		if (string.IsNullOrEmpty(AddData.Comment))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputComment"));
			return;
		}
		AddTreelistComment(AddData.CloneData());
		TreeViewModel.TreeGroupData.Clear();
		await TreeViewModel.TreeGroupData.CreateFileListDescriptionTrees(Source.Keys, Source);
		await Task.Delay(100);
		aVqGMnl7TF(AddData.FilePath);
	}

	public void AddTreelistComment(TreelistCommentRes treelistCommentRes)
	{
		if (Source.ContainsKey(treelistCommentRes.FilePath))
		{
			Source.Remove(treelistCommentRes.FilePath);
		}
		Source.Add(treelistCommentRes.FilePath, treelistCommentRes);
	}

	private void aVqGMnl7TF(string P_0)
	{
		TreeViewModel.TreeGroupData.FilePathGetTreeNode(P_0)?.Value.CommentDoNotify();
		AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(P_0)?.Value.CommentDoNotify();
		AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.FilePathGetTreeNode(P_0)?.Value.CommentDoNotify();
		TreeViewModel.GoToNode(P_0);
	}

	[Command]
	public void OnSaveSelectedItem()
	{
		if (TreeViewModel.IsSelectedNodes && SelectedItem != null)
		{
			if (string.IsNullOrEmpty(SelectedItem.Comment))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputComment"));
			}
			else
			{
				aVqGMnl7TF(SelectedItem.FilePath);
			}
		}
	}

	private void CLJGVPHaDC(KeyValuePair<string, PvfTreeFileBase> selectedRow)
	{
		if (Source.TryGetValue(selectedRow.Value.FullPath, out TreelistCommentRes value))
		{
			SelectedItem = value;
		}
		else
		{
			SelectedItem = null;
		}
	}

	[Command]
	public async void OnSave()
	{
		AppSetting.Instance.PvfConfig.TreelistCommentDic = Source;
		await AppSetting.Instance.SaveSetting();
		foreach (KeyValuePair<string, TreelistCommentRes> item in AppSetting.Instance.PvfConfig.TreelistCommentDic)
		{
			AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(item.Key)?.Value.CommentDoNotify();
		}
		Close?.Invoke();
	}

	[Command]
	public void Unloaded()
	{
		TreeViewModel.SelectedRowChangedEvent -= CLJGVPHaDC;
	}
}
