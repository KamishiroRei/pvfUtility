namespace PvfCode.NPK.Utils.AniModel;

public class RGB
{
	public double R { get; set; }

	public double G { get; set; }

	public double B { get; set; }

	public RGB(double r, double g, double b)
	{
		R = r;
		G = g;
		B = b;
	}

	public override string ToString()
	{
		return $"{R}\t{G}\t{B}";
	}

	public bool IsNull()
	{
		return (R == 0.0) & (G == 0.0) & (B == 0.0);
	}
}
