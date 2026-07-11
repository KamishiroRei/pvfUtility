using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.Mvvm;

namespace PvfCode.ViewModels.SearchPvf.SearchName;

public class ItemNameSearchResultBase : ViewModelBase
{
	public virtual int? ItemCode
	{
		get
		{
			return GetItemCode();
		}
		set
		{
		}
	}

	public ImageSource ItemImage
	{
		get
		{
			PvfGroup pVF = AppCore.ViewModelBase.PVF;
			if (pVF != null)
			{
				string text;
				if (FilePath == null)
				{
					List<string> lstNames = new List<string>
					{
						"equipment",
						"stackable"
					};
					if (!ItemCode.HasValue)
					{
						return null;
					}
					text = pVF.ListFileTable.ItemCodeConvertFilePath(lstNames, ItemCode.Value);
				}
				else
				{
					text = FilePath;
				}
				if (text == null)
				{
					return null;
				}
				if (pVF.FileList.TryGetValue(text, out PvfFile value))
				{
					if (!ImagePack2Service.Instance.TreeGetIcon(pVF, value, out ImageSource imageSource))
					{
						return null;
					}
					return imageSource;
				}
			}
			return null;
		}
	}

	public virtual string ItemName => GetItemName();

	public virtual string FilePath
	{
		get
		{
			return GetProperty(() => FilePath);
		}
		set
		{
			SetProperty<string>(() => FilePath, value);
		}
	}

	public ItemNameSearchResultBase()
	{
	}

	public ItemNameSearchResultBase(string filePath)
	{
		FilePath = filePath;
	}

	public int? GetItemCode()
	{
		return AppCore.ViewModelBase.PVF.GetFile(FilePath)?.ItemCode;
	}

	public string GetItemName()
	{
		PvfFile file = AppCore.ViewModelBase.PVF.GetFile(FilePath);
		if (file != null)
		{
			return AppCore.ViewModelBase.PVF.GetItemName(file);
		}
		return string.Empty;
	}
}
