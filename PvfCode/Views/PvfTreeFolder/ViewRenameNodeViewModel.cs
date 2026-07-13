using System;
using System.Collections.Generic;
using System.IO;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewRenameNodeViewModel : ViewModelBase
{
	private readonly KeyValuePair<string, PvfTreeFileBase> treeFileEntry;

	private readonly Action closeAction;

	public PvfTreeFileBase TreeFile { get; set; }

	public string Caption { get; set; }

	public string NewFileName
	{
		get
		{
			return GetProperty(() => NewFileName);
		}
		set
		{
			SetProperty<string>(() => NewFileName, value);
		}
	}

	public ViewRenameNodeViewModel(KeyValuePair<string, PvfTreeFileBase> treeFileDic, Action actionClose)
	{
		treeFileEntry = treeFileDic;
		TreeFile = treeFileEntry.Value;
		Caption = (TreeFile.IsFile ? AppSetting.Instance.GetIlogger().GetStr("ViewRenameNode_Label_FileName") : AppSetting.Instance.GetIlogger().GetStr("ViewRenameNode_Label_FolderName"));
		closeAction = actionClose;
		NewFileName = TreeFile.FileName;
	}

	[Command]
	public async void OnSave()
	{
		if (string.IsNullOrEmpty(NewFileName))
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("ViewRenameNode_Label_PleaseInputNew"), Caption));
			return;
		}
		if (NewFileName == TreeFile.FileName)
		{
			closeAction();
		}
		NewFileName = NewFileName.Replace("\\", null).Replace("/", null).ToLower();
		bool flag = !string.IsNullOrEmpty(Path.GetExtension(NewFileName));
		string newFilePath = TreeFile.FullPath.Substring(0, TreeFile.FullPath.Length - TreeFile.FileName.Length) + NewFileName.ToLower();
		if (AppCore.ViewModelBase.PVF.FileAny(newFilePath))
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileAlreadyExists"), newFilePath));
			return;
		}
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (TreeFile.IsFile)
		{
			if (!flag)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger().GetStr("mess_FileNameMustHaveExtension"), isError: true);
				return;
			}
			AppCore.ViewModelBase.PVF.RenameFile(TreeFile.FullPath, newFilePath);
			TreeGroup group = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData;
			group.DeleteTreeNode(treeFileEntry);
			await group.CreateTrees(new PooledList<string> { newFilePath }, null, group._Trees);
			if (group.ShowSearchResulTrees)
			{
				await group.CreateTrees(new PooledList<string> { newFilePath }, null, group._SearchResultTrees);
			}
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(newFilePath);
			if (keyValuePair.HasValue)
			{
				AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(keyValuePair.Value);
			}
			closeAction();
		}
		else if (flag)
		{
			string extension = Path.GetExtension(NewFileName);
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FolderNameCannotHaveExtension"), extension));
		}
		else
		{
			pVF.RenameFolder(TreeFile.FullPath, TreeFile.FileName, NewFileName);
			AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.DeleteTreeNode(treeFileEntry);
			await AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(pVF.GetFiles(newFilePath)));
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair2 = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(newFilePath);
			if (keyValuePair2.HasValue)
			{
				AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(keyValuePair2.Value);
			}
			closeAction();
		}
	}

	[Command]
	public void OnCancel()
	{
		closeAction();
	}
}
