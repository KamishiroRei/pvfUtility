using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PvfCode.ViewModels.Bars;

public class BarModel : ViewModel
{
	[CompilerGenerated]
	private List<BarCommandViewModel> hSXxnPldG3;

	[CompilerGenerated]
	private bool zaPxqs5u0j;

	public List<BarCommandViewModel> Commands
	{
		[CompilerGenerated]
		get
		{
			return hSXxnPldG3;
		}
		[CompilerGenerated]
		set
		{
			hSXxnPldG3 = value;
		}
	}

	public bool IsMainMenu
	{
		[CompilerGenerated]
		get
		{
			return zaPxqs5u0j;
		}
		[CompilerGenerated]
		set
		{
			zaPxqs5u0j = value;
		}
	}

	public BarModel(string displayName)
	{
		DisplayName = displayName;
	}
}
