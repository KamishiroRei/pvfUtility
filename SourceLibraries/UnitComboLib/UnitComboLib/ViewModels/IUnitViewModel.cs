using System.Collections.ObjectModel;
using System.Windows.Input;
using UnitComboLib.Models;

namespace UnitComboLib.ViewModels;

public interface IUnitViewModel
{
	string Error { get; }

	string this[string propertyName] { get; }

	string MaxStringLengthValue { get; set; }

	double MaxValue { get; }

	double MinValue { get; }

	int ScreenPoints { get; set; }

	ListItem SelectedItem { get; set; }

	ICommand SetSelectedItemCommand { get; }

	string StringValue { get; set; }

	ObservableCollection<ListItem> UnitList { get; }

	double Value { get; set; }

	string ValueTip { get; }
}
