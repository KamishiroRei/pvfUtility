using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode;

[JsonObject(MemberSerialization.OptOut)]
public class PublicSearchServiceOptions : ViewModelBase
{
	private bool? i87zmcqLA;

	private bool? zWHnAPxmlY;

	private bool? w6vnnYTHWt;

	private bool? cSTnk2iqym;

	private int? IC1nLC5UXt;

	public bool ToolbarShowNameSearchPanel
	{
		get
		{
			if (!i87zmcqLA.HasValue)
			{
				i87zmcqLA = true;
			}
			return i87zmcqLA.Value;
		}
		set
		{
			i87zmcqLA = value;
			RaisePropertyChanged("ToolbarShowNameSearchPanel");
		}
	}

	public bool OpenStringAndSectionCompletion
	{
		get
		{
			if (!zWHnAPxmlY.HasValue)
			{
				zWHnAPxmlY = true;
			}
			return zWHnAPxmlY.Value;
		}
		set
		{
			zWHnAPxmlY = value;
			RaisePropertyChanged("OpenStringAndSectionCompletion");
		}
	}

	public bool OpenNameCompletion
	{
		get
		{
			if (!w6vnnYTHWt.HasValue)
			{
				w6vnnYTHWt = true;
			}
			return w6vnnYTHWt.Value;
		}
		set
		{
			w6vnnYTHWt = value;
			RaisePropertyChanged("OpenNameCompletion");
		}
	}

	public bool OpenFilePathCompletion
	{
		get
		{
			if (!cSTnk2iqym.HasValue)
			{
				cSTnk2iqym = true;
			}
			return cSTnk2iqym.Value;
		}
		set
		{
			cSTnk2iqym = value;
			RaisePropertyChanged("OpenFilePathCompletion");
		}
	}

	public int TakeNumber
	{
		get
		{
			if (!IC1nLC5UXt.HasValue)
			{
				IC1nLC5UXt = 20;
			}
			return IC1nLC5UXt.Value;
		}
		set
		{
			IC1nLC5UXt = value;
			RaisePropertyChanged("TakeNumber");
		}
	}

	public bool DisableEnterShortcutsSearch
	{
		get
		{
			return GetProperty(() => DisableEnterShortcutsSearch);
		}
		set
		{
			SetProperty(() => DisableEnterShortcutsSearch, value);
		}
	}

	public PublicSearchServiceOptions()
	{
	}
}
