using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class GameLoginAccountInfo : ViewModelBase
{
	[CompilerGenerated]
	private int h3MLojklCe;

	[CompilerGenerated]
	private string cxML21mHuU;

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

	public int ProcessId
	{
		[CompilerGenerated]
		get
		{
			return h3MLojklCe;
		}
		[CompilerGenerated]
		set
		{
			h3MLojklCe = value;
		}
	}

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

	public string ClientPath
	{
		[CompilerGenerated]
		get
		{
			return cxML21mHuU;
		}
		[CompilerGenerated]
		set
		{
			cxML21mHuU = value;
		}
	}

	public GameLoginAccountInfo()
	{
	}
}
