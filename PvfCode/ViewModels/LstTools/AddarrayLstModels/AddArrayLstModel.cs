using DevExpress.Mvvm;

namespace PvfCode.ViewModels.LstTools.AddarrayLstModels;

public class AddArrayLstModel : ViewModelBase
{
	public string ArrarLstString
	{
		get
		{
			return GetProperty(() => ArrarLstString);
		}
		set
		{
			SetProperty<string>(() => ArrarLstString, value);
		}
	}

	public CodeRepeatManageType CodeRepeatManageType
	{
		get
		{
			return GetProperty(() => CodeRepeatManageType);
		}
		set
		{
			SetProperty(() => CodeRepeatManageType, value);
		}
	}

	public PathRepeatManageType PathRepeatManageType
	{
		get
		{
			return GetProperty(() => PathRepeatManageType);
		}
		set
		{
			SetProperty(() => PathRepeatManageType, value);
		}
	}

	public PathRepeatManageType PathNotAnyManageType
	{
		get
		{
			return GetProperty(() => PathNotAnyManageType);
		}
		set
		{
			SetProperty(() => PathNotAnyManageType, value);
		}
	}

	public AddArrayLstModel()
	{
		CodeRepeatManageType = CodeRepeatManageType.跳过;
		PathRepeatManageType = PathRepeatManageType.跳过;
		PathNotAnyManageType = PathRepeatManageType.跳过;
		ArrarLstString = AppSetting.Instance.GetIlogger().GetStr("AddArrayLstDescription");
	}
}
