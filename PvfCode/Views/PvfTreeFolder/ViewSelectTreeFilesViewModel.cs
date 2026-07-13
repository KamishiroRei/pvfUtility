using System;
using System.Windows;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewSelectTreeFilesViewModel : ViewModelBase
{
	private readonly Action closeAction;

	public bool IsLoading
	{
		get
		{
			return GetProperty(() => IsLoading);
		}
		set
		{
			SetProperty(() => IsLoading, value);
		}
	}

	public TreeViewType SourceType { get; set; }

	public Visibility SearchPanelComboBoxVisibility
	{
		get
		{
			if (SourceType != TreeViewType.SearchResult)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}
	}

	protected virtual IMessageBoxService MessageBoxService => GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

	public string SearchResultSelectedItem
	{
		get
		{
			return GetProperty(() => SearchResultSelectedItem);
		}
		set
		{
			SetProperty<string>(() => SearchResultSelectedItem, value, RefreshSearchResultTree);
		}
	}

	public PvfTreeViewModel TreeViewModel { get; set; }

	public bool IsOk { get; set; }

	private async void RefreshSearchResultTree()
	{
		if (!string.IsNullOrEmpty(SearchResultSelectedItem))
		{
			TreeViewModel.TreeGroupData.Trees = null;
			IsLoading = true;
			PooledList<string> fileList = AppCore.ViewModelBase.SearchResultViewModel.GetFileList(SearchResultSelectedItem);
			await TreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(fileList));
			IsLoading = false;
		}
	}

	public ViewSelectTreeFilesViewModel(TreeViewType sourceType, Action close)
	{
		SourceType = sourceType;
		closeAction = close;
		TreeViewModel = new PvfTreeViewModel(TreeViewType.SelectFiles);
		if (sourceType == TreeViewType.FileList)
		{
			TreeViewModel.TreeGroupData.Trees = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.GetTrees();
		}
		else
		{
			SearchResultSelectedItem = AppCore.ViewModelBase.SearchResultViewModel.SelectedItem;
		}
	}

	[Command]
	public void Cancel()
	{
		closeAction();
	}

	[Command]
	public void OnYes()
	{
		if (!TreeViewModel.IsSelectedNodes)
		{
			AppCore.ShowMsg(MessageBoxService, AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile_2"), isError: true);
			return;
		}
		IsOk = true;
		closeAction();
	}
}
