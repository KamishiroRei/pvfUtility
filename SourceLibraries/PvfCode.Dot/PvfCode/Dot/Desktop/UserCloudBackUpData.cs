using System;

namespace PvfCode.Dot.Desktop;

public class UserCloudBackUpData : ModelBase
{
	private DateTime? _BackUpTime;

	private bool _AllowBackUp;

	public string? Data { get; set; }

	public DateTime? BackUpTime
	{
		get
		{
			return _BackUpTime;
		}
		set
		{
			_BackUpTime = value;
			DoNotify("BackUpTime");
		}
	}

	public bool AllowBackUp
	{
		get
		{
			return _AllowBackUp;
		}
		set
		{
			_AllowBackUp = value;
			DoNotify("AllowBackUp");
		}
	}
}
