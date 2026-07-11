using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnitComboLib.Models.Unit;

namespace UnitComboLib.Models;

public class ListItem
{
	private ObservableCollection<string> _DefaultValues;

	public Itemkey Key { get; private set; }

	public string DisplayNameLong { get; private set; }

	public string DisplayNameShort { get; private set; }

	public string DisplayNameLongWithShort => $"{DisplayNameShort} ({DisplayNameLong})";

	public ObservableCollection<string> DefaultValues
	{
		get
		{
			if (_DefaultValues.Count > 11)
			{
				_DefaultValues.Clear();
				foreach (string item in new List<string>
				{
					"25", "50", "75", "100", "125", "150", "175", "200", "300", "400",
					"500"
				})
				{
					_DefaultValues.Add(item);
				}
			}
			return _DefaultValues;
		}
		set
		{
			_DefaultValues = value;
		}
	}

	public ListItem(Itemkey key, string displayNameLong, string displayNameShort, ObservableCollection<string> defaultValues)
	{
		Key = key;
		DisplayNameLong = ((displayNameLong == null) ? "(null)" : displayNameLong);
		DisplayNameShort = ((displayNameShort == null) ? "(null)" : displayNameShort);
		DefaultValues = defaultValues;
	}

	protected ListItem()
	{
	}
}
