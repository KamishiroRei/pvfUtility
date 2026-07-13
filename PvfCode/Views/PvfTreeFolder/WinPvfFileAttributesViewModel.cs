using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels.TreeFolder;
using Utools;

namespace PvfCode.Views.PvfTreeFolder;

public class WinPvfFileAttributesViewModel : ViewModelBase
{
	public PvfTreeFileBase TreeFile { get; set; }

	public PvfFile? File { get; set; }

	public ImageSource Img
	{
		get
		{
			return GetProperty(() => Img);
		}
		set
		{
			SetProperty<ImageSource>(() => Img, value);
		}
	}

	public string FileType { get; set; }

	public string FileSize { get; set; }

	public string FolderInfo { get; set; }

	public WinPvfFileAttributesViewModel(PvfTreeFileBase treeFilefile)
	{
		TreeFile = treeFilefile;
		File = treeFilefile.File;
		if (TreeFile.IsFile)
		{
			FileType = File?.FileType.ToString();
			FileSize = FileHelper.CountSize(Convert.ToInt64(File?.DataLen));
			return;
		}
		FileType = AppSetting.Instance.GetIlogger().GetStr("mess_Folder");
		List<string> folderChildren = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.GetFolderChildren(TreeFile);
		List<PvfFile> files = AppCore.ViewModelBase.PVF.GetFiles(folderChildren);
		if (files != null && files.Count > 0)
		{
			FileSize = FileHelper.CountSize(Convert.ToInt64(files.Sum((PvfFile it) => it.DataLen)));
		}
		TreeGroup treeGroupData = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData;
		FolderInfo = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileCountAndFolderCount"), folderChildren.Count, treeGroupData.GetFolderChildrenFolderCount(treeFilefile));
	}

	[Command]
	public async void Loaded()
	{
		Img = await TreeFile.GetIcon();
	}
}
