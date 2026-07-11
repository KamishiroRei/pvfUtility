using DevExpress.Mvvm;

namespace PvfCode.NPK.Utils.AniModel;

public class POINT : BindableBase
{
	public int X
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

	public int Y
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

	public POINT(int x, int y)
	{
		X = x;
		Y = y;
	}

	public override string ToString()
	{
		return $"({X},{Y})";
	}
}
