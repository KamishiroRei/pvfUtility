using System.Collections.Generic;
using UnitComboLib.Models;
using UnitComboLib.Models.Unit;
using UnitComboLib.ViewModels;

namespace UnitComboLib;

public static class UnitViewModeService
{
	public static IUnitViewModel CreateInstance(IList<ListItem> list, Converter unitConverter, int defaultIndex = 0, double defaultValue = 100.0, string maxStringLengthValue = "#####")
	{
		return new UnitViewModel(list, unitConverter, defaultIndex, defaultValue)
		{
			MaxStringLengthValue = maxStringLengthValue
		};
	}
}
