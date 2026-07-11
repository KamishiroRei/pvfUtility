using DevExpress.Mvvm;

namespace PvfCode.ViewModels.TreeFolder;

public class SearchPanelOptions : BindableBase
{
	public bool ItemName
	{
		get
		{
			return GetProperty(() => ItemName);
		}
		set
		{
			SetProperty(() => ItemName, value);
		}
	}

	public bool ItemCode
	{
		get
		{
			return GetProperty(() => ItemCode);
		}
		set
		{
			SetProperty(() => ItemCode, value);
		}
	}

	public bool Comment
	{
		get
		{
			return GetProperty(() => Comment);
		}
		set
		{
			SetProperty(() => Comment, value);
		}
	}

	public bool FilePath
	{
		get
		{
			return GetProperty(() => FilePath);
		}
		set
		{
			SetProperty(() => FilePath, value);
		}
	}

	public bool ConvertTw
	{
		get
		{
			return GetProperty(() => ConvertTw);
		}
		set
		{
			SetProperty(() => ConvertTw, value);
		}
	}

	public SearchPanelOptions()
	{
		ItemName = true;
		ItemCode = true;
		Comment = true;
		FilePath = true;
		ConvertTw = true;
	}
}
