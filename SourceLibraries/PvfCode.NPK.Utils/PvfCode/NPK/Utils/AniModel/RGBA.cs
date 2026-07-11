using System.Windows.Media;
using PvfCode.NPK.Utils.Models;

namespace PvfCode.NPK.Utils.AniModel;

public class RGBA
{
	public double R { get; set; }

	public double G { get; set; }

	public double B { get; set; }

	public double A { get; set; }

	public Color Color
	{
		get
		{
			return Color.FromArgb((byte)A, (byte)R, (byte)G, (byte)B);
		}
		set
		{
			A = (int)value.A;
			R = (int)value.R;
			G = (int)value.G;
			B = (int)value.B;
		}
	}

	public RGBA(double r, double g, double b, double a)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	public RGBA()
	{
	}

	public override string ToString()
	{
		return $"{R}\t{G}\t{B}\t{A}";
	}

	public bool IsNull()
	{
		if (R == 0.0 && G == 0.0 && B == 0.0)
		{
			return A == 0.0;
		}
		return false;
	}

	public void InitImageDyeing(ImgFile imgFile)
	{
		if (imgFile != null && !IsNull())
		{
			Color? color = (IsNull() ? ((Color?)null) : new Color?(Color));
			imgFile.AniImageDye(color);
		}
	}
}
