using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DevExpress.Xpf.Bars;

namespace PvfCode.ViewModels.Bars;

public class BarCommandViewModel : ViewModel
{
	[CompilerGenerated]
	private ICommand tLlx6ljG6T;

	[CompilerGenerated]
	private List<BarCommandViewModel> q2Dx1NxuoX;

	[CompilerGenerated]
	private BarItemDisplayMode klJxwmkhfs;

	private bool PV1xoKosnM;

	[CompilerGenerated]
	private BarType TT6xsE0T8c;

	[CompilerGenerated]
	private KeyGesture hVyxL0CjEP;

	public ICommand Command
	{
		[CompilerGenerated]
		get
		{
			return tLlx6ljG6T;
		}
		[CompilerGenerated]
		private set
		{
			tLlx6ljG6T = value;
		}
	}

	public List<BarCommandViewModel> Commands
	{
		[CompilerGenerated]
		get
		{
			return q2Dx1NxuoX;
		}
		[CompilerGenerated]
		set
		{
			q2Dx1NxuoX = value;
		}
	}

	public BarItemDisplayMode DisplayMode
	{
		[CompilerGenerated]
		get
		{
			return klJxwmkhfs;
		}
		[CompilerGenerated]
		set
		{
			klJxwmkhfs = value;
		}
	}

	public bool IsEnabled
	{
		get
		{
			return PV1xoKosnM;
		}
		set
		{
			PV1xoKosnM = value;
			DoNotify("IsEnabled");
		}
	}

	public BarType BarItemType
	{
		[CompilerGenerated]
		get
		{
			return TT6xsE0T8c;
		}
		[CompilerGenerated]
		set
		{
			TT6xsE0T8c = value;
		}
	}

	public KeyGesture KeyGesture
	{
		[CompilerGenerated]
		get
		{
			return hVyxL0CjEP;
		}
		[CompilerGenerated]
		set
		{
			hVyxL0CjEP = value;
		}
	}

	public BarCommandViewModel()
	{
	}

	public BarCommandViewModel(string displayName, List<BarCommandViewModel> subCommands)
		: this(displayName, null, subCommands)
	{
	}

	public BarCommandViewModel(string displayName, ICommand? command = null)
		: this(displayName, command, null)
	{
	}

	private BarCommandViewModel(string displayName, ICommand? command = null, List<BarCommandViewModel>? subCommands = null)
	{
		IsEnabled = true;
		DisplayName = displayName;
		Command = command;
		Commands = subCommands;
	}
}
