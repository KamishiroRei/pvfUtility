namespace PvfCode.Dot.Desktop;

public class RegIsteredAccountRes : ModelBase
{
	private string _UserName;

	private string _Password;

	private string _Password2;

	private string _NickName;

	private long _Phone;

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

	public string Password2
	{
		get
		{
			if (string.IsNullOrEmpty(_Password2))
			{
				_Password2 = string.Empty;
			}
			return _Password2;
		}
		set
		{
			_Password2 = value;
			DoNotify("Password2");
		}
	}

	public string NickName
	{
		get
		{
			if (string.IsNullOrEmpty(_NickName))
			{
				_NickName = string.Empty;
			}
			return _NickName;
		}
		set
		{
			_NickName = value;
			DoNotify("NickName");
		}
	}

	public long Phone
	{
		get
		{
			return _Phone;
		}
		set
		{
			_Phone = value;
			DoNotify("Phone");
		}
	}
}
