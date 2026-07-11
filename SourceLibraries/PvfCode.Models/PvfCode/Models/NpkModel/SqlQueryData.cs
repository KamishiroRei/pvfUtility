using DevExpress.Mvvm;

namespace PvfCode.Models.NpkModel;

public class SqlQueryData : ViewModelBase
{
	private string vyWZxryAvX;

	public string NpkName
	{
		get
		{
			return vyWZxryAvX?.ToLower();
		}
		set
		{
			vyWZxryAvX = value;
			RaisePropertyChanged("NpkName");
		}
	}

	public string ImgName
	{
		get
		{
			return GetProperty(() => ImgName);
		}
		set
		{
			SetProperty<string>(() => ImgName, value);
		}
	}

	public bool IsLike
	{
		get
		{
			return GetProperty(() => IsLike);
		}
		set
		{
			SetProperty(() => IsLike, value);
		}
	}

	public SqlQueryData()
	{
	}
}
