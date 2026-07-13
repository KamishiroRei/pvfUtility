using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.ViewModels.Description.FileListDescription;

public class FileListDescriptionViewModel : ViewModelBase
{
	private readonly Action closeAction;

	private Dictionary<string, TreelistCommentRes> Source { get; set; }

	public PvfTreeViewModel TreeViewModel { get; set; }

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

	public TreelistCommentRes AddData { get; set; }

	public FileListDescriptionViewModel(Action close)
	{
		Source = new Dictionary<string, TreelistCommentRes>(AppSetting.Instance.PvfConfig.TreelistCommentDic);
		AddData = new TreelistCommentRes();
		TreeViewModel = new PvfTreeViewModel(TreeViewType.FileListDescription);
		TreeViewModel.SelectedRowChangedEvent += OnSelectedRowChanged;
		closeAction = close;
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
		RefreshComment(AddData.FilePath);
	}

	public void AddTreelistComment(TreelistCommentRes treelistCommentRes)
	{
		if (Source.ContainsKey(treelistCommentRes.FilePath))
		{
			Source.Remove(treelistCommentRes.FilePath);
		}
		Source.Add(treelistCommentRes.FilePath, treelistCommentRes);
	}

	private void RefreshComment(string filePath)
	{
		TreeViewModel.TreeGroupData.FilePathGetTreeNode(filePath)?.Value.CommentDoNotify();
		AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(filePath)?.Value.CommentDoNotify();
		AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.FilePathGetTreeNode(filePath)?.Value.CommentDoNotify();
		TreeViewModel.GoToNode(filePath);
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
				RefreshComment(SelectedItem.FilePath);
			}
		}
	}

	private void OnSelectedRowChanged(KeyValuePair<string, PvfTreeFileBase> selectedRow)
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
		closeAction?.Invoke();
	}

	[Command]
	public void Unloaded()
	{
		TreeViewModel.SelectedRowChangedEvent -= OnSelectedRowChanged;
	}
}
