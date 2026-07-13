using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.PreviewPvfFileFolder;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.ViewModels.TreeFolder.Enums;
using Utools;

namespace PvfCode;

public abstract class PvfTreeFileBase : ModelBase
{
	internal readonly PvfGroup Pvf;

	public readonly short Level;

	private TreeImageType _imageType;

	private ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> _children;

	public TreeViewType TreeType { get; set; }

	public PvfFile? File { get; private set; }

	public bool? IsShearStatus { get; set; }

	public bool IsFile { get; private set; }

	public string FullPath { get; private set; }

	public string FileName { get; private set; }

	public string? ItemName
	{
		get
		{
			if (!IsFile || File == null)
			{
				return null;
			}
			return Pvf.GetItemName(File);
		}
	}

	public PvfSkillKind? SkillKind => IsFile && File != null ? PvfSkillClassifier.GetKind(Pvf, File, ItemName) : null;

	public string ItemCodeStr
	{
		get
		{
			if (!IsFile || File == null)
			{
				return null;
			}
			int? itemCode = File.ItemCode;
			if (!itemCode.HasValue)
			{
				return null;
			}
			return $"<{itemCode}>";
		}
	}

	public TreeImageType ImageType
	{
		get
		{
			return _imageType;
		}
		set
		{
			if (value != _imageType)
			{
				_imageType = value;
				DoNotify("ImageType");
			}
		}
	}

	public ImageSource Image
	{
		get
		{
			if (IsFile)
			{
				PvfFileType pvfFileType = AppSetting.Instance.PvfConfig.GetPvfFileType(Path.GetExtension(FileName));
				switch (pvfFileType)
				{
				case PvfFileType.lst:
					ImageType = TreeImageType.DefaultFileIcon;
					return Res.Instance.TreeFiles.Lst;
				case PvfFileType.str:
					ImageType = TreeImageType.DefaultFileIcon;
					return Res.Instance.TreeFiles.Str;
				case PvfFileType.ani:
					ImageType = TreeImageType.DefaultFileIcon;
					return Res.Instance.TreeFiles.Ani;
				case PvfFileType.ui:
					ImageType = TreeImageType.DefaultFileIcon;
					return Res.Instance.TreeFiles.Ui;
				default:
				{
					if (File != null && Pvf != null && ImagePack2Service.Instance.TreeGetIcon(Pvf, File, out ImageSource imageSource) && imageSource != null)
					{
						if (pvfFileType == PvfFileType.qst)
						{
							ImageType = TreeImageType.QstIcon;
						}
						else
						{
							ImageType = TreeImageType.FileIcon;
						}
						return imageSource;
					}
					ImageType = TreeImageType.DefaultFileIcon;
					return Res.Instance.TreeFiles.Script_16x;
				}
				}
			}
			ImageType = TreeImageType.Folder;
			return Res.Instance.TreeFiles.FolderClosed;
		}
	}

	public int? Rarity
	{
		get
		{
			if (!IsFile || File == null || Pvf == null || !File.IsScriptFile)
			{
				return null;
			}
			if (!File.GetRarity((PvfPack)Pvf, out int rarity))
			{
				return null;
			}
			return rarity;
		}
	}

	public AttachType? AttachType
	{
		get
		{
			PvfFileType? fileType = GetFileType();
			if (!fileType.HasValue || fileType != PvfFileType.equ)
			{
				return null;
			}
			PvfFile? file = File;
			if (file != null && file.GetAttachType(Pvf, out var attachType))
			{
				return attachType;
			}
			return null;
		}
	}

	public FilePreviewDataBase? ItemPreviewData
	{
		get
		{
			if (AppSetting.Instance.PvfFilePreviewOptions.ForbidShowToolTip)
			{
				return null;
			}
			if (IsFile && File != null && Pvf != null)
			{
				return FilePreviewDataBase.Create(Pvf, File, Image);
			}
			return null;
		}
	}

	public bool PreviewItemVisibility
	{
		get
		{
			if (IsFile && File != null && Pvf != null)
			{
				switch (File.FileType)
				{
				case PvfFileType.equ:
					return File.IsScriptFile;
				case PvfFileType.stk:
					return File.IsScriptFile;
				case PvfFileType.shp:
					return File.IsScriptFile;
				}
			}
			return false;
		}
	}

	public abstract string? DetailedComment { get; }

	public abstract string? Comment { get; }

	public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> Children
	{
		get
		{
			if (_children == null)
			{
				_children = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
			}
			return _children;
		}
		set
		{
			_children = value;
		}
	}

	public PvfTreeFileBase(PvfGroup pvf, string fullPath, string fileName, bool isFile, short level)
	{
		Pvf = pvf;
		IsFile = isFile;
		FullPath = fullPath;
		FileName = fileName;
		Level = level;
		if (isFile && pvf != null && pvf.PvfIsOpen && pvf.FileList.TryGetValue(fullPath, out PvfFile value))
		{
			File = value;
		}
	}

	public void DoNotifyStatus()
	{
		DoNotify("IsShearStatus");
	}

	public void SetIsFile(bool val)
	{
		IsFile = val;
	}

	public bool FileIsNull()
	{
		return File == null;
	}

	public void CommentDoNotify()
	{
		DoNotify("Comment");
		DoNotify("DetailedComment");
	}

	public void FileNameDoNotify()
	{
		DoNotify("ItemName");
		DoNotify("ItemCodeStr");
		DoNotify("Rarity");
		DoNotify("Image");
		DoNotify("ItemPreviewData");
		DoNotify("AttachType");
		DoNotify("SkillKind");
	}

	public bool HaveChildren()
	{
		if (_children == null)
		{
			return false;
		}
		return _children.Any();
	}

	public int ChildrenCount()
	{
		if (_children == null)
		{
			return 0;
		}
		return _children.Count();
	}

	public virtual bool IsFileMethon()
	{
		if (!IsFile)
		{
			return !HaveChildren();
		}
		return true;
	}

	public async Task<ImageSource> GetIcon()
	{
		if (IsFile && File != null)
		{
			ResultData<ImageSource> resultData = await Task.Run(() => AppCore.ViewModelBase.PVF.GetScriptIconSource(File));
			if (resultData != null && resultData.Data != null)
			{
				return resultData.Data;
			}
		}
		return Res.Instance.TreeFiles.GetFileIcon(IsFile, (File == null) ? ((PvfFileType?)null) : new PvfFileType?(File.FileType));
	}

	public PvfFileType? GetFileType()
	{
		if (!IsFile)
		{
			return null;
		}
		string extension = Path.GetExtension(FileName);
		if (string.IsNullOrEmpty(extension))
		{
			return null;
		}
		return AppSetting.Instance.PvfConfig.GetPvfFileType(extension);
	}

	[Command]
	public void OnSwitchExplain()
	{
		AppCore.ShowMsg("当前项目没有可切换的说明内容。");
	}

	public void Order()
	{
		if (HaveChildren())
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
			observableConcurrentDictionaryEx.AddRange(from it in _children
				orderby it.Key
				orderby it.Value.IsFile descending
				select it);
			_children = observableConcurrentDictionaryEx;
		}
	}
}
