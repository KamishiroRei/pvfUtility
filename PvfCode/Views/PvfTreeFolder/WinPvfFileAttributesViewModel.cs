using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels.TreeFolder;
using Utools;

namespace PvfCode.Views.PvfTreeFolder;

public class WinPvfFileAttributesViewModel : ViewModelBase
{
	[CompilerGenerated]
	private PvfTreeFileBase gHEHk2CgN0;

	[CompilerGenerated]
	private PvfFile? heXH0V3EpU;

	[CompilerGenerated]
	private string UOyH7T41C1;

	[CompilerGenerated]
	private string VjAHXrYqvZ;

	[CompilerGenerated]
	private string X0IHpcIu8Z;

	public PvfTreeFileBase TreeFile
	{
		[CompilerGenerated]
		get
		{
			return gHEHk2CgN0;
		}
		[CompilerGenerated]
		set
		{
			gHEHk2CgN0 = value;
		}
	}

	public PvfFile? File
	{
		[CompilerGenerated]
		get
		{
			return heXH0V3EpU;
		}
		[CompilerGenerated]
		set
		{
			heXH0V3EpU = value;
		}
	}

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

	public string FileType
	{
		[CompilerGenerated]
		get
		{
			return UOyH7T41C1;
		}
		[CompilerGenerated]
		set
		{
			UOyH7T41C1 = value;
		}
	}

	public string FileSize
	{
		[CompilerGenerated]
		get
		{
			return VjAHXrYqvZ;
		}
		[CompilerGenerated]
		set
		{
			VjAHXrYqvZ = value;
		}
	}

	public string FolderInfo
	{
		[CompilerGenerated]
		get
		{
			return X0IHpcIu8Z;
		}
		[CompilerGenerated]
		set
		{
			X0IHpcIu8Z = value;
		}
	}

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
