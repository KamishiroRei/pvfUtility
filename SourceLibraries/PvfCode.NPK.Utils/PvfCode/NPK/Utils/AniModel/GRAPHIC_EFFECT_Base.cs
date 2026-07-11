using System.ComponentModel;
using DevExpress.Mvvm;
using PvfCode.NPK.Utils.AniModel.Enums;

namespace PvfCode.NPK.Utils.AniModel;

public class GRAPHIC_EFFECT_Base : BindableBase
{
	private RGB _RGB;

	[RefreshProperties(RefreshProperties.All)]
	public Effect_Item Type
	{
		get
		{
			return GetProperty(() => Type);
		}
		set
		{
			SetProperty(() => Type, value);
		}
	}

	public RGB RGB
	{
		get
		{
			if (_RGB == null)
			{
				_RGB = new RGB(0.0, 0.0, 0.0);
			}
			return _RGB;
		}
		set
		{
			_RGB = value;
			RaisePropertyChanged("RGB");
		}
	}

	public short X
	{
		get
		{
			return GetProperty(() => X);
		}
		set
		{
			SetProperty(() => X, value);
		}
	}

	public short Y
	{
		get
		{
			return GetProperty(() => Y);
		}
		set
		{
			SetProperty(() => Y, value);
		}
	}

	public GRAPHIC_EFFECT_Base()
	{
	}

	public GRAPHIC_EFFECT_Base(short x, short y, Effect_Item type)
	{
		X = x;
		Y = y;
		Type = type;
	}

	public override string ToString()
	{
		return Type.ToString();
	}
}
