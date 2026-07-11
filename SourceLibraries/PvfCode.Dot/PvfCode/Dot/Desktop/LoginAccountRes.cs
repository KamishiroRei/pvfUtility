namespace PvfCode.Dot.Desktop;

public class LoginAccountRes : ModelBase
{
	private string _UserName;

	private string _Password;

	public string UserName
	{
		get
		{
			if (string.IsNullOrEmpty(_UserName))
			{
				_UserName = string.Empty;
			}
			return _UserName;
		}
		set
		{
			_UserName = value;
			DoNotify("UserName");
		}
	}

	public string Password
	{
		get
		{
			if (string.IsNullOrEmpty(_Password))
			{
				_Password = string.Empty;
			}
			return _Password;
		}
		set
		{
			_Password = value;
			DoNotify("Password");
		}
	}
}
