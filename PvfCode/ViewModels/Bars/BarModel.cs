using System.Collections.Generic;

namespace PvfCode.ViewModels.Bars;

public class BarModel : ViewModel
{
	public List<BarCommandViewModel> Commands { get; set; }

	public bool IsMainMenu { get; set; }

	public BarModel(string displayName)
	{
		DisplayName = displayName;
	}
}
