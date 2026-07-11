using System.Windows;
using DevExpress.Mvvm;

namespace PvfCode.NPK.Utils.AniModel;

public class CLIP : BindableBase
{
	public short X1
	{
		get
		{
			return GetProperty(() => X1);
		}
		set
		{
			SetProperty(() => X1, value, Up);
		}
	}

	public short X2
	{
		get
		{
			return GetProperty(() => X2);
		}
		set
		{
			SetProperty(() => X2, value, Up);
		}
	}

	public short Y1
	{
		get
		{
			return GetProperty(() => Y1);
		}
		set
		{
			SetProperty(() => Y1, value, Up);
		}
	}

	public short Y2
	{
		get
		{
			return GetProperty(() => Y2);
		}
		set
		{
			SetProperty(() => Y2, value, Up);
		}
	}

	public Point Center => new Point((double)X2, (double)Y2);

	public Rect RECT
	{
		get
		{
			Rect result = new Rect(1.0, 1.0, 1.0, 1.0);
			result.X = X1;
			result.Y = Y1;
			result.Width = X2;
			result.Height = Y2;
			return result;
		}
	}

	public CLIP()
	{
	}

	public CLIP(short x1, short y1, short x2, short y2)
	{
		X1 = x1;
		Y1 = y1;
		X2 = x2;
		Y2 = y2;
	}

	private void Up()
	{
		RaisePropertyChanged("RECT");
		RaisePropertyChanged("Center");
	}

	public bool IsNull()
	{
		if (X1 == 0 && Y1 == 0 && Y2 == 0)
		{
			return X2 == 0;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{X1}\t{X2}\t{Y1}\t{Y2}";
	}
}
