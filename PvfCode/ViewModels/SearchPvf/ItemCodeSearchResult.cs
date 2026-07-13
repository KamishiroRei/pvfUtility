using System.Windows.Media;

namespace PvfCode.ViewModels.SearchPvf;

public class ItemCodeSearchResult : ModelBase
{
	private readonly PvfFile? _file;

	public string ItemCode
	{
		get
		{
			if (_file == null)
			{
				return null;
			}
			if (_file.ItemCode.HasValue)
			{
				return _file.ItemCode.ToString();
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
			return AppCore.ViewModelBase.PVF.GetItemName(_file);
		}
	}

	public ImageSource ItemImage
	{
		get
		{
			if (_file == null)
			{
				return null;
			}
			if (!ImagePack2Service.Instance.TreeGetIcon(AppCore.ViewModelBase.PVF, _file, out ImageSource imageSource))
			{
				return null;
			}
			return imageSource;
		}
	}

	public string FullPath { get; set; }

	public ItemCodeSearchResult(string fullPath)
	{
		FullPath = fullPath;
		_file = AppCore.ViewModelBase.PVF.GetFile(fullPath);
	}
}
