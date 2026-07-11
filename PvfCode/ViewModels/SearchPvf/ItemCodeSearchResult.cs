using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PvfCode.ViewModels.SearchPvf;

public class ItemCodeSearchResult : ModelBase
{
	private readonly PvfFile? File;

	[CompilerGenerated]
	private string ecdWi2hQFP;

	public string ItemCode
	{
		get
		{
			if (File == null)
			{
				return null;
			}
			if (File.ItemCode.HasValue)
			{
				return File.ItemCode.ToString();
			}
			return null;
		}
	}

	public string ItemName
	{
		get
		{
			if (!AppCore.ViewModelBase.PVF.PvfIsOpen)
			{
				return null;
			}
			return AppCore.ViewModelBase.PVF.GetItemName(File);
		}
	}

	public ImageSource ItemImage
	{
		get
		{
			if (File == null)
			{
				return null;
			}
			if (!ImagePack2Service.Instance.TreeGetIcon(AppCore.ViewModelBase.PVF, File, out ImageSource imageSource))
			{
				return null;
			}
			return imageSource;
		}
	}

	public string FullPath
	{
		[CompilerGenerated]
		get
		{
			return ecdWi2hQFP;
		}
		[CompilerGenerated]
		set
		{
			ecdWi2hQFP = value;
		}
	}

	public ItemCodeSearchResult(string fullPath)
	{
		FullPath = fullPath;
		File = AppCore.ViewModelBase.PVF.GetFile(fullPath);
	}
}
