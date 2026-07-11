using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Utools;

namespace PvfCode.ViewModels;

public class SettingMenuItem
{
	private ObservableConcurrentDictionaryEx<string, SettingMenuItem> O8CFqM1ouN;

	[CompilerGenerated]
	private DataTemplate? hSNFdLp2AU;

	[CompilerGenerated]
	private string JN0FeD6LDt;

	public ObservableConcurrentDictionaryEx<string, SettingMenuItem> Children
	{
		get
		{
			if (O8CFqM1ouN == null)
			{
				O8CFqM1ouN = new ObservableConcurrentDictionaryEx<string, SettingMenuItem>();
			}
			return O8CFqM1ouN;
		}
		set
		{
			O8CFqM1ouN = value;
		}
	}

	public DataTemplate? Data
	{
		[CompilerGenerated]
		get
		{
			return hSNFdLp2AU;
		}
		[CompilerGenerated]
		set
		{
			hSNFdLp2AU = value;
		}
	}

	public string Parname
	{
		[CompilerGenerated]
		get
		{
			return JN0FeD6LDt;
		}
		[CompilerGenerated]
		set
		{
			JN0FeD6LDt = value;
		}
	}

	public bool HaveChildren()
	{
		if (O8CFqM1ouN == null)
		{
			return false;
		}
		return O8CFqM1ouN.Any();
	}

	public SettingMenuItem()
	{
	}
}
