using Newtonsoft.Json;

namespace PvfCode.Dot.Desktop;

[JsonObject(MemberSerialization.OptOut)]
public class LoginResultDto : ModelBase
{
	private string _NickName;

	private string _Avatar;

	private bool isAdmin;

	private long _Phone;

	public string UserName { get; set; }

	public int UserId { get; set; }

	public string NickName
	{
		get
		{
			if (string.IsNullOrEmpty(_NickName))
			{
				_NickName = UserName;
			}
			return _NickName;
		}
		set
		{
			_NickName = value;
			DoNotify("NickName");
		}
	}

	public string Avatar
	{
		get
		{
			return _Avatar;
		}
		set
		{
			_Avatar = value;
			DoNotify("Avatar");
		}
	}

	public bool IsAdmin
	{
		get
		{
			return isAdmin;
		}
		set
		{
			isAdmin = value;
			DoNotify("IsAdmin");
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

	public TokenResultDot TokenData { get; set; }
}
