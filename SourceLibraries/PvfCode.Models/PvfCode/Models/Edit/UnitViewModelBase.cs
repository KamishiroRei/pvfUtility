using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnitComboLib;
using UnitComboLib.Models;
using UnitComboLib.Models.Unit;
using UnitComboLib.Models.Unit.Screen;
using UnitComboLib.ViewModels;

namespace PvfCode.Models.Edit;

public class UnitViewModelBase
{
	public IUnitViewModel SizeUnitLabel { get; set; }

	public UnitViewModelBase()
	{
		InitializeSizeUnitLabel();
	}

	private void InitializeSizeUnitLabel()
	{
		ObservableCollection<ListItem> list = new ObservableCollection<ListItem>(GenerateScreenUnitList());
		SizeUnitLabel = UnitViewModeService.CreateInstance(list, new ScreenConverter(), 0, 100.0, "#####");
	}

	public static IEnumerable<ListItem> GenerateScreenUnitList()
	{
		List<ListItem> list = new List<ListItem>();
		ObservableCollection<string> defaultValues = new ObservableCollection<string>
		{
			"25",
			"50",
			"75",
			"100",
			"125",
			"150",
			"175",
			"200",
			"300",
			"400",
			"500"
		};
		list.Add(new ListItem(Itemkey.ScreenPercent, "Percent", "%", defaultValues));
		return list;
	}
}
