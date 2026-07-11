using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewRenameNodeViewModel : ViewModelBase
{
	private readonly KeyValuePair<string, PvfTreeFileBase> oLmHxIq2pv;

	[CompilerGenerated]
	private PvfTreeFileBase WdvHQHV6sT;

	private readonly Action m5pHagpu5e;

	[CompilerGenerated]
	private string Ha8Hg2sPGi;

	public PvfTreeFileBase TreeFile
	{
		[CompilerGenerated]
		get
		{
			return WdvHQHV6sT;
		}
		[CompilerGenerated]
		set
		{
			WdvHQHV6sT = value;
		}
	}

	public string Caption
	{
		[CompilerGenerated]
		get
		{
			return Ha8Hg2sPGi;
		}
		[CompilerGenerated]
		set
		{
			Ha8Hg2sPGi = value;
		}
	}

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
		oLmHxIq2pv = treeFileDic;
		TreeFile = oLmHxIq2pv.Value;
		Caption = (TreeFile.IsFile ? AppSetting.Instance.GetIlogger().GetStr("ViewRenameNode_Label_FileName") : AppSetting.Instance.GetIlogger().GetStr("ViewRenameNode_Label_FolderName"));
		m5pHagpu5e = actionClose;
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
			m5pHagpu5e();
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
			group.DeleteTreeNode(oLmHxIq2pv);
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
			m5pHagpu5e();
		}
		else if (flag)
		{
			string extension = Path.GetExtension(NewFileName);
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FolderNameCannotHaveExtension"), extension));
		}
		else
		{
			pVF.RenameFolder(TreeFile.FullPath, TreeFile.FileName, NewFileName);
			AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.DeleteTreeNode(oLmHxIq2pv);
			await AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(pVF.GetFiles(newFilePath)));
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair2 = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(newFilePath);
			if (keyValuePair2.HasValue)
			{
				AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(keyValuePair2.Value);
			}
			m5pHagpu5e();
		}
	}

	[Command]
	public void OnCancel()
	{
		m5pHagpu5e();
	}
}
