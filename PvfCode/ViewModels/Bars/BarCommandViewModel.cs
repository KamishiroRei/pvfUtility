using System.Collections.Generic;
using System.Windows.Input;
using DevExpress.Xpf.Bars;

namespace PvfCode.ViewModels.Bars;

public class BarCommandViewModel : ViewModel
{
	private bool isEnabled;

	public ICommand Command { get; private set; }

	public List<BarCommandViewModel> Commands { get; set; }

	public BarItemDisplayMode DisplayMode { get; set; }

	public bool IsEnabled
	{
		get
		{
			return isEnabled;
		}
		set
		{
			isEnabled = value;
			DoNotify("IsEnabled");
		}
	}

	public BarType BarItemType { get; set; }

	public KeyGesture KeyGesture { get; set; }

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
