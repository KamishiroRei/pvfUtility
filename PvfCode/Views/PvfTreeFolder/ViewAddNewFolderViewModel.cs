using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewAddNewFolderViewModel : ViewModelBase
{
	private readonly KeyValuePair<string, PvfTreeFileBase>? Source;

	[CompilerGenerated]
	private bool nkkH2u61eY;

	[CompilerGenerated]
	private string o7WHfZ7Sym;

	protected virtual IMessageBoxService MessageBoxService => GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

	public bool IsRoot
	{
		[CompilerGenerated]
		get
		{
			return nkkH2u61eY;
		}
		[CompilerGenerated]
		set
		{
			nkkH2u61eY = value;
		}
	}

	public string ParentFolderName
	{
		[CompilerGenerated]
		get
		{
			return o7WHfZ7Sym;
		}
		[CompilerGenerated]
		set
		{
			o7WHfZ7Sym = value;
		}
	}

	public string FolderName
	{
		get
		{
			return GetProperty(() => FolderName);
		}
		set
		{
			SetProperty<string>(() => FolderName, value);
		}
	}

	public ViewAddNewFolderViewModel(KeyValuePair<string, PvfTreeFileBase>? source)
	{
		IsRoot = !source.HasValue;
		Source = source;
		if (source.HasValue)
		{
			ParentFolderName = source.Value.Value.FullPath + "/";
		}
	}

	[Command]
	public async void OnOK(Window win)
	{
		if (!string.IsNullOrEmpty(Path.GetExtension(FolderName)))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_FolderCannotContainExtension"), isError: true);
			return;
		}
		string newFolderName = (ParentFolderName + FolderName).ToLower();
		if (AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.Any(newFolderName))
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FolderAlreadyExists"), newFolderName), isError: true);
			return;
		}
		await AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.CreateTreesFolder(new PooledList<string> { newFolderName });
		KeyValuePair<string, PvfTreeFileBase>? row = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(newFolderName);
		AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(row);
		win.Close();
	}

	[Command]
	public void OnCancel(Window win)
	{
		win.Close();
	}
}
