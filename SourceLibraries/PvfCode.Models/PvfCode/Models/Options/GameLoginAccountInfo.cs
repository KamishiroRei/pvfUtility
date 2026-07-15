using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class GameLoginAccountInfo : ViewModelBase
{
	public string UID
	{
		get
		{
			return GetProperty(() => UID);
		}
		set
		{
			SetProperty<string>(() => UID, value);
		}
	}

	public string UserName
	{
		get
		{
			return GetProperty(() => UserName);
		}
		set
		{
			SetProperty<string>(() => UserName, value);
		}
	}

	public string Password
	{
		get
		{
			return GetProperty(() => Password);
		}
		set
		{
			SetProperty<string>(() => Password, value);
		}
	}

	public bool UseUIDLogin
	{
		get
		{
			return GetProperty(() => UseUIDLogin);
		}
		set
		{
			SetProperty(() => UseUIDLogin, value);
		}
	}

	[JsonIgnore]
	public bool UserIsNull
	{
		get
		{
			if (UseUIDLogin)
			{
				return string.IsNullOrEmpty(UID);
			}
			if (!string.IsNullOrEmpty(UserName))
			{
				return string.IsNullOrEmpty(Password);
			}
			return true;
		}
	}

	public int ProcessId { get; set; }

	public bool GameIsStop
	{
		get
		{
			return GetProperty(() => GameIsStop);
		}
		set
		{
			SetProperty(() => GameIsStop, value);
		}
	}

	public string ClientPath { get; set; }
}
