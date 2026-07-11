using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class PvfDocumentOptions : ViewModelBase
{
	private int? uy1EAx81eC;

	private int? mpHEn84PEy;

	public int IconHeight
	{
		get
		{
			if (!uy1EAx81eC.HasValue)
			{
				uy1EAx81eC = 28;
			}
			return uy1EAx81eC.Value;
		}
		set
		{
			uy1EAx81eC = value;
			RaisePropertyChanged("IconHeight");
		}
	}

	public int IconWidth
	{
		get
		{
			if (!mpHEn84PEy.HasValue)
			{
				mpHEn84PEy = 28;
			}
			return mpHEn84PEy.Value;
		}
		set
		{
			mpHEn84PEy = value;
			RaisePropertyChanged("IconWidth");
		}
	}

	public bool CaptionNotAllowMenuItemNameIsFloat
	{
		get
		{
			return GetProperty(() => CaptionNotAllowMenuItemNameIsFloat);
		}
		set
		{
			SetProperty(() => CaptionNotAllowMenuItemNameIsFloat, value);
		}
	}

	public bool CaptionShowNotAllowItemNewHorizontalTabGroup
	{
		get
		{
			return GetProperty(() => CaptionShowNotAllowItemNewHorizontalTabGroup);
		}
		set
		{
			SetProperty(() => CaptionShowNotAllowItemNewHorizontalTabGroup, value);
		}
	}

	public bool CaptionNotAllowMenuItemNewVerticalTabGroup
	{
		get
		{
			return GetProperty(() => CaptionNotAllowMenuItemNewVerticalTabGroup);
		}
		set
		{
			SetProperty(() => CaptionNotAllowMenuItemNewVerticalTabGroup, value);
		}
	}

	public PvfDocumentOptions()
	{
	}
}
