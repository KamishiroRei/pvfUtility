using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels;
using PvfCode.ViewModels.TreeFolder;
using Utools;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewSelectFolderViewModel : ViewModelBase
{
	private readonly Action xqDHoPojck;

	[CompilerGenerated]
	private PvfTreeViewModel LnJHsZhYh0;

	public KeyValuePair<string, PvfTreeFileBase>? SelectedItem;

	protected virtual IMessageBoxService MessageBoxService => GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

	public PvfTreeViewModel TreeViewModel
	{
		[CompilerGenerated]
		get
		{
			return LnJHsZhYh0;
		}
		[CompilerGenerated]
		set
		{
			LnJHsZhYh0 = value;
		}
	}

	public ViewSelectFolderViewModel(Action closeAction)
	{
		xqDHoPojck = closeAction;
		TreeViewModel = new PvfTreeViewModel(TreeViewType.SelectFolder);
		hFVHw9gA4S();
	}

	private void hFVHw9gA4S()
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
		xqDHoPojck();
	}

	[Command]
	public void OnClose()
	{
		xqDHoPojck();
	}
}
