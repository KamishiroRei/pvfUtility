using System;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class PvfReleaseLog : ViewModelBase
{
	[JsonObject(MemberSerialization.OptOut)]
	public class PvfReleaseData : ViewModelBase
	{
		public string TargetPath
		{
			get
			{
				return GetProperty(() => TargetPath);
			}
			set
			{
				SetProperty<string>(() => TargetPath, value);
			}
		}

		public DateTime? Time
		{
			get
			{
				return GetProperty(() => Time);
			}
			set
			{
				SetProperty(() => Time, value);
			}
		}

		public string Error
		{
			get
			{
				return GetProperty(() => Error);
			}
			set
			{
				SetProperty<string>(() => Error, value);
			}
		}

		public bool IsSuccess
		{
			get
			{
				return GetProperty(() => IsSuccess);
			}
			set
			{
				SetProperty(() => IsSuccess, value);
			}
		}

		public PvfReleaseData()
		{
		}
	}

	private PvfReleaseClientOptions NQVEp6mQfj;

	private PvfReleaseData H47EDLeplp;

	private PvfReleaseData nvZE370G1a;

	public PvfReleaseClientOptions ClientReleaseOptions
	{
		get
		{
			if (NQVEp6mQfj == null)
			{
				NQVEp6mQfj = new PvfReleaseClientOptions();
			}
			return NQVEp6mQfj;
		}
		set
		{
			NQVEp6mQfj = value;
		}
	}

	public PvfReleaseType? LastReleaseType
	{
		get
		{
			return GetProperty(() => LastReleaseType);
		}
		set
		{
			SetProperty(() => LastReleaseType, value);
			RaisePropertyChanged("LastLog");
		}
	}

	[JsonIgnore]
	public PvfReleaseData LastLog
	{
		get
		{
			if (!LastReleaseType.HasValue)
			{
				return null;
			}
			return LastReleaseType switch
			{
				PvfReleaseType.客户端 => ClientLog, 
				PvfReleaseType.服务端 => ServerLog, 
				_ => null, 
			};
		}
	}

	public PvfReleaseData ServerLog
	{
		get
		{
			if (H47EDLeplp == null)
			{
				H47EDLeplp = new PvfReleaseData();
			}
			return H47EDLeplp;
		}
		set
		{
			H47EDLeplp = value;
		}
	}

	public PvfReleaseData ClientLog
	{
		get
		{
			if (nvZE370G1a == null)
			{
				nvZE370G1a = new PvfReleaseData();
			}
			return nvZE370G1a;
		}
		set
		{
			nvZE370G1a = value;
		}
	}

	public PvfReleaseLog()
	{
	}
}
