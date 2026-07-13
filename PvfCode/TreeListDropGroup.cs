using System.Collections.Generic;
using Collections.Pooled;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.ViewModels;
using PvfCode.ViewModels.DocumentFolder;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.ViewModels.TreeFolder.Drop;

namespace PvfCode;

public class TreeListDropGroup
{
	public const string DropKey = "TreeListDropGroup：15427586-86B6-5410-5D88-7F139C0C1E9E";

	public static TreeListDropGroup _instance;

	public static TreeListDropGroup Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TreeListDropGroup();
			}
			return _instance;
		}
		set
		{
			_instance = value;
		}
	}

	public bool Success { get; set; }

	public TreeViewType Source { get; set; }

	public IList<KeyValuePair<string, PvfTreeFileBase>> Items { get; set; }

	public PvfTreeViewModel SourceViewModel { get; set; }

	public PooledSet<string> GetFilePaths()
	{
		return SourceViewModel.GetSelectedFilePaths(GetTreeType.File);
	}

	public void DropDocument(PooledSet<string> fileList, TextEditorBase editor, PvfFileDocument vm)
	{
		Success = true;
		DropDocumentModelBase dropDocumentModelBase = vm.File.FileType != PvfFileType.lst
			? new ItemCodeDropHandler()
			: new LstFileDropHandler();
		if (dropDocumentModelBase == null)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ThisFileTypeDeliverFail_NotSupport"), vm.File.FileType));
		}
		else
		{
			dropDocumentModelBase.Drop(fileList, editor, vm);
		}
	}

	public static void Clear()
	{
		Instance = null;
	}

}
