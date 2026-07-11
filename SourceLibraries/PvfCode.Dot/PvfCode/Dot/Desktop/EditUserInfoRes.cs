namespace PvfCode.Dot.Desktop;

public class EditUserInfoRes : ModelBase
{
	private string _NickName;

	private byte[] _AvatarBytes;

	private long _Phone;

	public string NickName
	{
		get
		{
			return _NickName;
		}
		set
		{
			_NickName = value;
			DoNotify("NickName");
		}
	}

	public byte[] AvatarBytes
	{
		get
		{
			return _AvatarBytes;
		}
		set
		{
			_AvatarBytes = value;
			DoNotify("AvatarBytes");
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
