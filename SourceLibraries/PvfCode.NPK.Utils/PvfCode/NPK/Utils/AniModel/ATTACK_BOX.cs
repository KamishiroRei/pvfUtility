namespace PvfCode.NPK.Utils.AniModel;

public class ATTACK_BOX : BOX_Base
{
	public ATTACK_BOX(int x, int y, int z, int l, int w, int h)
		: base(x, y, z, l, w, h)
	{
	}

	public override string ToString()
	{
		return ToString("[ATTACK BOX]");
	}
}
