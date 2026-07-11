namespace PvfCode.NPK.Utils.AniModel;

public class DAMAGE_BOX : BOX_Base
{
	public DAMAGE_BOX(int x, int y, int z, int l, int w, int h)
		: base(x, y, z, l, w, h)
	{
	}

	public override string ToString()
	{
		return ToString("[DAMAGE BOX]");
	}
}
