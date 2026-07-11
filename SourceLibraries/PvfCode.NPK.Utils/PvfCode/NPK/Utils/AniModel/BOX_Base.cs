namespace PvfCode.NPK.Utils.AniModel;

public abstract class BOX_Base
{
	public int X { get; set; }

	public int Y { get; set; }

	public int Z { get; set; }

	public int L { get; set; }

	public int W { get; set; }

	public int H { get; set; }

	public BOX_Base(int x, int y, int z, int l, int w, int h)
	{
		X = x;
		Y = y;
		Z = z;
		L = l;
		W = w;
		H = h;
	}

	public string ToString(string section)
	{
		return $"\t{section}\r\n\t\t{X}\t{Y}\t{Z}\t{L}\t{W}\t{H}";
	}
}
