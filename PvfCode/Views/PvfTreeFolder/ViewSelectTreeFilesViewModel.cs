using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewSelectTreeFilesViewModel : ViewModelBase
{
	[CompilerGenerated]
	private TreeViewType kg3HdlJY4M;

	[CompilerGenerated]
	private PvfTreeViewModel YMyHeeFkK8;

	private readonly Action Close;

	[CompilerGenerated]
	private bool bjjHtTlMfr;

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

	public TreeViewType SourceType
	{
		[CompilerGenerated]
		get
		{
			return kg3HdlJY4M;
		}
		[CompilerGenerated]
		set
		{
			kg3HdlJY4M = value;
		}
	}

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
			SetProperty<string>(() => SearchResultSelectedItem, value, iV5HqH9Mru);
		}
	}

	public PvfTreeViewModel TreeViewModel
	{
		[CompilerGenerated]
		get
		{
			return YMyHeeFkK8;
		}
		[CompilerGenerated]
		set
		{
			YMyHeeFkK8 = value;
		}
	}

	public bool IsOk
	{
		[CompilerGenerated]
		get
		{
			return bjjHtTlMfr;
		}
		[CompilerGenerated]
		set
		{
			bjjHtTlMfr = value;
		}
	}

	private async void iV5HqH9Mru()
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
		Close = close;
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
		Close();
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
		Close();
	}
}
