using System;
using System.Collections.Generic;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels;
using PvfCode.ViewModels.TreeFolder;
using Utools;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewSelectFolderViewModel : ViewModelBase
{
	private readonly Action closeAction;

	public KeyValuePair<string, PvfTreeFileBase>? SelectedItem;

	protected virtual IMessageBoxService MessageBoxService => GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

	public PvfTreeViewModel TreeViewModel { get; set; }

	public ViewSelectFolderViewModel(Action closeAction)
	{
		this.closeAction = closeAction;
		TreeViewModel = new PvfTreeViewModel(TreeViewType.SelectFolder);
		InitializeFolderTree();
	}

	private void InitializeFolderTree()
	{
		ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		if (observableConcurrentDictionaryEx == null)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseLoadPvfPackFirst"));
			return;
		}
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.GetTrees())
		{
			if (!item.Value.IsFile)
			{
				observableConcurrentDictionaryEx.TryAdd(item.Key, item.Value);
			}
		}
		TreeViewModel.TreeGroupData.Trees = observableConcurrentDictionaryEx;
	}

	[Command]
	public void OnYes()
	{
		if (!TreeViewModel.SelectedNodeBindgBase.HasValue)
		{
			AppCore.ShowMsg(MessageBoxService, AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFolderFirst"));
			return;
		}
		SelectedItem = TreeViewModel.SelectedNodeBindgBase;
		closeAction();
	}

	[Command]
	public void OnClose()
	{
		closeAction();
	}
}
