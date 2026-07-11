using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.NPK.Utils.AniModel;
using PvfCode.NPK.Utils.AniModel.Enums;
using PvfCode.NPK.Utils.Lib;

namespace PvfCode;

public class FRAMEModel : ViewModelBase
{
	private ImageRate? _IMAGE_RATE;

	private RGBA? rgba;

	private GRAPHIC_EFFECT_Base? _GRAPHIC_EFFECT;

	private CLIP _CLIP;

	private ImageSource _ImageSource;

	public string Name => "[FRAME" + Index.ToString("D3") + "]";

	public int Index { get; private set; }

	public AniImage Image { get; set; }

	public POINT IMAGE_POS { get; set; }

	public bool LOOP { get; set; }

	public bool SHADOW
	{
		get
		{
			return GetProperty(() => SHADOW);
		}
		set
		{
			SetProperty(() => SHADOW, value);
		}
	}

	public byte? INTERPOLATION { get; set; }

	public ushort? COORD { get; set; }

	public bool PRELOAD { get; set; }

	public ImageRate IMAGE_RATE
	{
		get
		{
			if (_IMAGE_RATE == null)
			{
				_IMAGE_RATE = new ImageRate(0f, 0f);
			}
			return _IMAGE_RATE;
		}
		set
		{
			_IMAGE_RATE = value;
			RaisePropertyChanged("IMAGE_RATE");
		}
	}

	public float? IMAGE_ROTATE
	{
		get
		{
			return GetProperty(() => IMAGE_ROTATE);
		}
		set
		{
			SetProperty(() => IMAGE_ROTATE, value);
		}
	}

	public RGBA RGBA
	{
		get
		{
			if (rgba == null)
			{
				rgba = new RGBA();
			}
			return rgba;
		}
		set
		{
			rgba = value;
			SetImageSourceEffect();
		}
	}

	public GRAPHIC_EFFECT_Base GRAPHIC_EFFECT
	{
		get
		{
			if (_GRAPHIC_EFFECT == null)
			{
				_GRAPHIC_EFFECT = new GRAPHIC_EFFECT_Base
				{
					Type = Effect_Item.NONE
				};
			}
			return _GRAPHIC_EFFECT;
		}
		set
		{
			_GRAPHIC_EFFECT = value;
			RaisePropertyChanged("GRAPHIC_EFFECT");
			SetImageSourceEffect();
		}
	}

	public List<BOX_Base>? BOX_List { get; set; }

	public DAMAGE_TYPE_Item? DAMAGE_TYPE
	{
		get
		{
			return GetProperty(() => DAMAGE_TYPE);
		}
		set
		{
			SetProperty(() => DAMAGE_TYPE, value);
		}
	}

	public string? PLAY_SOUND { get; set; }

	public int? SET_FLAG { get; set; }

	public FLIP_TYPE_Item? FLIP_TYPE
	{
		get
		{
			return GetProperty(() => FLIP_TYPE);
		}
		set
		{
			SetProperty(() => FLIP_TYPE, value);
		}
	}

	public bool LOOP_START { get; set; }

	public int? LOOP_END { get; set; }

	public CLIP CLIP
	{
		get
		{
			if (_CLIP == null)
			{
				_CLIP = new CLIP();
			}
			return _CLIP;
		}
		set
		{
			_CLIP = value;
			RaisePropertyChanged("CLIP");
		}
	}

	public int DELAY { get; set; }

	public ImageSource ImageSource
	{
		get
		{
			if (Image == null || string.IsNullOrEmpty(Image.Img) || Image.ImgFile == null)
			{
				return null;
			}
			if (_ImageSource == null)
			{
				_ImageSource = Image.ImgFile.GetImageSouce();
			}
			return _ImageSource;
		}
	}

	public double ActualX => (Image != null && Image.ImgFile != null) ? (IMAGE_POS.X + Image.ImgFile.X) : IMAGE_POS.X;

	public double ActualY => (Image != null && Image.ImgFile != null) ? (IMAGE_POS.Y + Image.ImgFile.Y) : IMAGE_POS.Y;

	public FRAMEModel(int index)
	{
		Index = index;
	}

	[Command]
	public void PositionChanged()
	{
		RaisePropertyChanged("ActualX");
		RaisePropertyChanged("ActualY");
	}

	[Command]
	public void GRAPHIC_EFFECTChanged()
	{
		SetImageSourceEffect();
	}

	public void SetImageSourceEffect()
	{
		if (Image != null && !string.IsNullOrEmpty(Image.Img) && Image.ImgFile != null)
		{
			byte[] array = Image.ImgFile.GetImageBytes();
			if (!RGBA.IsNull())
			{
				array = array.ImageDye(rgba.Color);
			}
			if (GRAPHIC_EFFECT.Type == Effect_Item.LINEARDODGE)
			{
				array.LinearBrun();
			}
			_ImageSource = array.ByteArrayToBitmapSource2(Image.ImgFile.Size.Width, Image.ImgFile.Size.Height);
			RaisePropertyChanged("ImageSource");
		}
	}

	public string ToString(StringBuilder bui)
	{
		StringBuilder stringBuilder = bui;
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder);
		handler.AppendLiteral("[FRAME");
		handler.AppendFormatted(Index.ToString("D3"));
		handler.AppendLiteral("]");
		stringBuilder2.AppendLine(ref handler);
		bui.AppendLine(Image.GetStringData());
		stringBuilder = bui;
		StringBuilder stringBuilder3 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(17, 2, stringBuilder);
		handler.AppendLiteral("\t[IMAGE POS]\r\n\t\t");
		handler.AppendFormatted(IMAGE_POS.X);
		handler.AppendLiteral("\t");
		handler.AppendFormatted(IMAGE_POS.Y);
		stringBuilder3.AppendLine(ref handler);
		if (LOOP)
		{
			stringBuilder = bui;
			StringBuilder stringBuilder4 = stringBuilder;
			handler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder);
			handler.AppendLiteral("\t[LOOP]\r\n\t\t");
			handler.AppendFormatted(LOOP);
			stringBuilder4.AppendLine(ref handler);
		}
		if (SHADOW)
		{
			bui.AppendLine("\t[SHADOW]\r\n\t\t1");
		}
		if (COORD.HasValue)
		{
			stringBuilder = bui;
			StringBuilder stringBuilder5 = stringBuilder;
			handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder);
			handler.AppendLiteral("\t[COORD]\r\n\t\t");
			handler.AppendFormatted(COORD);
			stringBuilder5.AppendLine(ref handler);
		}
		if (PRELOAD)
		{
			bui.AppendLine("\t[PRELOAD]\r\n\t\t1");
		}
		if (IMAGE_RATE != null)
		{
			bui.AppendLine(IMAGE_RATE.ToString());
		}
		if (IMAGE_ROTATE.HasValue)
		{
			stringBuilder = bui;
			StringBuilder stringBuilder6 = stringBuilder;
			handler = new StringBuilder.AppendInterpolatedStringHandler(19, 1, stringBuilder);
			handler.AppendLiteral("\t[IMAGE ROTATE]\r\n\t\t");
			handler.AppendFormatted(IMAGE_ROTATE);
			stringBuilder6.AppendLine(ref handler);
		}
		if (RGBA != null)
		{
			stringBuilder = bui;
			StringBuilder stringBuilder7 = stringBuilder;
			handler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder);
			handler.AppendLiteral("\t[RGBA]\r\n\t\t");
			handler.AppendFormatted(RGBA.ToString());
			stringBuilder7.AppendLine(ref handler);
		}
		if (GRAPHIC_EFFECT != null)
		{
			GRAPHIC_EFFECTConvertString(bui);
		}
		_ = DELAY;
		stringBuilder = bui;
		StringBuilder stringBuilder8 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder);
		handler.AppendLiteral("\t[DELAY]\r\n\t\t");
		handler.AppendFormatted(DELAY);
		stringBuilder8.AppendLine(ref handler);
		if (DAMAGE_TYPE.HasValue)
		{
			stringBuilder = bui;
			StringBuilder stringBuilder9 = stringBuilder;
			handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder);
			handler.AppendLiteral("\t[DELAY]\r\n\t\t");
			handler.AppendFormatted(DAMAGE_TYPE);
			stringBuilder9.AppendLine(ref handler);
		}
		if (PLAY_SOUND != null)
		{
			stringBuilder = bui;
			StringBuilder stringBuilder10 = stringBuilder;
			handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder);
			handler.AppendLiteral("\t[DELAY]\r\n\t\t");
			handler.AppendFormatted(PLAY_SOUND);
			stringBuilder10.AppendLine(ref handler);
		}
		if (SET_FLAG.HasValue)
		{
			stringBuilder = bui;
			StringBuilder stringBuilder11 = stringBuilder;
			handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder);
			handler.AppendLiteral("\t[DELAY]\r\n\t\t");
			handler.AppendFormatted(SET_FLAG);
			stringBuilder11.AppendLine(ref handler);
		}
		if (FLIP_TYPE.HasValue)
		{
			stringBuilder = bui;
			StringBuilder stringBuilder12 = stringBuilder;
			handler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder);
			handler.AppendLiteral("\t[FLIP TYPE]\r\n\t\t");
			handler.AppendFormatted(FLIP_TYPE);
			stringBuilder12.AppendLine(ref handler);
		}
		if (LOOP_START)
		{
			bui.AppendLine("\t[DELAY]");
		}
		if (LOOP_END.HasValue)
		{
			stringBuilder = bui;
			StringBuilder stringBuilder13 = stringBuilder;
			handler = new StringBuilder.AppendInterpolatedStringHandler(15, 1, stringBuilder);
			handler.AppendLiteral("\t[LOOP_END]\r\n\t\t");
			handler.AppendFormatted(LOOP_END);
			stringBuilder13.AppendLine(ref handler);
		}
		if (CLIP != null)
		{
			stringBuilder = bui;
			StringBuilder stringBuilder14 = stringBuilder;
			handler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder);
			handler.AppendLiteral("\t[CLIP]\r\n\t\t");
			handler.AppendFormatted(CLIP.ToString());
			stringBuilder14.AppendLine(ref handler);
		}
		bui.AppendLine(IMAGE_RATE.ToString());
		return bui.ToString();
	}

	private void GRAPHIC_EFFECTConvertString(StringBuilder bui)
	{
		if (GRAPHIC_EFFECT.Type != Effect_Item.NONE)
		{
			StringBuilder stringBuilder = bui;
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(27, 1, stringBuilder);
			handler.AppendLiteral("\r\n\t[GRAPHIC EFFECT]\r\n\t\t`");
			handler.AppendFormatted(GRAPHIC_EFFECT.Type);
			handler.AppendLiteral("`\r\n");
			stringBuilder2.Append(ref handler);
			switch (GRAPHIC_EFFECT.Type)
			{
			case Effect_Item.MONOCHROME:
				bui.Append("\t\t");
				bui.Append(GRAPHIC_EFFECT.RGB.ToString());
				break;
			case Effect_Item.SPACEDISTORT:
			{
				bui.Append("\t\t");
				stringBuilder = bui;
				StringBuilder stringBuilder3 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder);
				handler.AppendFormatted(GRAPHIC_EFFECT.X);
				handler.AppendLiteral("\t");
				handler.AppendFormatted(GRAPHIC_EFFECT.Y);
				stringBuilder3.Append(ref handler);
				break;
			}
			}
		}
	}

	public void ChangeIndex(int index)
	{
		Index = index;
	}
}
