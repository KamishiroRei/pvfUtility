using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class GameOptions : ViewModelBase
{
	private GameLoginAccountInfo F3jL6cSNSn;

	private GameLoginAccountInfo YUULy1y71e;

	private GameServerOptions LZGLwED6Yx;

	public GameLoginAccountInfo GameUserA
	{
		get
		{
			if (F3jL6cSNSn == null)
			{
				F3jL6cSNSn = new GameLoginAccountInfo();
			}
			return F3jL6cSNSn;
		}
		set
		{
			F3jL6cSNSn = value;
		}
	}

	public GameLoginAccountInfo GameUserB
	{
		get
		{
			if (YUULy1y71e == null)
			{
				YUULy1y71e = new GameLoginAccountInfo();
			}
			return YUULy1y71e;
		}
		set
		{
			YUULy1y71e = value;
		}
	}

	public string GameClientPath
	{
		get
		{
			return GetProperty(() => GameClientPath);
		}
		set
		{
			SetProperty<string>(() => GameClientPath, value);
		}
	}

	public GameServerOptions GameServerOptions
	{
		get
		{
			if (LZGLwED6Yx == null)
			{
				LZGLwED6Yx = new GameServerOptions();
			}
			return LZGLwED6Yx;
		}
		set
		{
			LZGLwED6Yx = value;
		}
	}

	public GameOptions()
	{
	}
}
