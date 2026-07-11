using PvfCode.NPK.Utils.AniModel.Enums;

namespace PvfCode.NPK.Utils.AniModel;

public class SPECTRUM
{
	public byte SPECTRUM_ { get; set; }

	public int SPECTRUM_TERM { get; set; }

	public int SPECTRUM_LIFE_TIME { get; set; }

	public RGBA? SPECTRUM_COLOR { get; set; }

	public Effect_Item SPECTRUM_EFFECT { get; set; }

	public override string ToString()
	{
		return $"[SPECTRUM]\r\n\t{SPECTRUM_}\r\n\t[SPECTRUM TERM]\r\n\t\t{SPECTRUM_TERM}\r\n\t[SPECTRUM LIFE TIME]\r\n\t\t{SPECTRUM_LIFE_TIME}\r\n\t[SPECTRUM COLOR]\r\n\t\t{SPECTRUM_COLOR.ToString()}\r\n\t[SPECTRUM EFFECT]\r\n\t\t`{SPECTRUM_EFFECT}`";
	}
}
