using System;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace PvfCode;

public class NavigationData : ViewModelBase
{
	[CompilerGenerated]
	private string jhkj5CEH6p;

	[CompilerGenerated]
	private int usZjSpAphj;

	private int b6ijAHvrCk;

	private int BS0j4t4D2V;

	private bool WVIjY74Hy1;

	internal Action<NavigationData> RFCjyyD80B;

	[CompilerGenerated]
	private string kIpjiTcI6O;

	public string FilePath
	{
		[CompilerGenerated]
		get
		{
			return jhkj5CEH6p;
		}
		[CompilerGenerated]
		set
		{
			jhkj5CEH6p = value;
		}
	}

	public int DocumentOffset
	{
		[CompilerGenerated]
		get
		{
			return usZjSpAphj;
		}
		[CompilerGenerated]
		set
		{
			usZjSpAphj = value;
		}
	}

	public int Line
	{
		get
		{
			if (b6ijAHvrCk <= 0)
			{
				b6ijAHvrCk = 1;
			}
			return b6ijAHvrCk;
		}
		set
		{
			b6ijAHvrCk = value;
		}
	}

	public int Column
	{
		get
		{
			if (BS0j4t4D2V <= 0)
			{
				BS0j4t4D2V = 1;
			}
			return BS0j4t4D2V;
		}
		set
		{
			BS0j4t4D2V = value;
		}
	}

	public bool IsChecked
	{
		get
		{
			return WVIjY74Hy1;
		}
		set
		{
			WVIjY74Hy1 = value;
			RaisePropertyChanged("IsChecked");
		}
	}

	public string Text
	{
		[CompilerGenerated]
		get
		{
			return kIpjiTcI6O;
		}
		[CompilerGenerated]
		set
		{
			kIpjiTcI6O = value;
		}
	}

	public bool TextVisibility => !string.IsNullOrEmpty(Text);

	[Command]
	public void OnClick()
	{
		RFCjyyD80B?.Invoke(this);
	}

	public NavigationData()
	{
	}
}
