using System;
using System.Collections.Generic;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using PvfCode.Models.Enums;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class PvfFilePreviewOptions : ViewModelBase
{
	private Dictionary<ThemeType, PvfFilePreviewColorOptions> LK2Ek5VtJq;

	private double? rV8ELcIB2t;

	public Dictionary<ThemeType, PvfFilePreviewColorOptions> ColorConfigs
	{
		get
		{
			if (LK2Ek5VtJq == null)
			{
				LK2Ek5VtJq = new Dictionary<ThemeType, PvfFilePreviewColorOptions>();
			}
			foreach (object value in Enum.GetValues(typeof(ThemeType)))
			{
				ThemeType themeType = (ThemeType)Enum.Parse(typeof(ThemeType), value.ToString());
				if (!LK2Ek5VtJq.ContainsKey(themeType))
				{
					LK2Ek5VtJq.Add(themeType, new PvfFilePreviewColorOptions(themeType));
				}
			}
			return LK2Ek5VtJq;
		}
	}

	[JsonIgnore]
	public PvfFilePreviewColorOptions CurrentColorConfing
	{
		get
		{
			if (ColorConfigs.TryGetValue(AppSetting.Instance.NowThemeType, out PvfFilePreviewColorOptions value))
			{
				return value;
			}
			return null;
		}
	}

	public double? FontSize
	{
		get
		{
			if (!rV8ELcIB2t.HasValue)
			{
				rV8ELcIB2t = 12.0;
			}
			return rV8ELcIB2t;
		}
		set
		{
			rV8ELcIB2t = value;
			RaisePropertyChanged("FontSize");
		}
	}

	public bool ForbidShowToolTip
	{
		get
		{
			return GetProperty(() => ForbidShowToolTip);
		}
		set
		{
			SetProperty(() => ForbidShowToolTip, value);
		}
	}

	public void ChangedCurrentColorConfing()
	{
		RaisePropertiesChanged("CurrentColorConfing");
	}

	public PvfFilePreviewOptions()
	{
	}
}
