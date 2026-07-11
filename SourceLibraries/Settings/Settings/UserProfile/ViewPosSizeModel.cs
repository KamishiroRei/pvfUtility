using System;
using System.Windows;
using System.Xml.Serialization;
using Settings.Interfaces;

namespace Settings.UserProfile;

[Serializable]
[XmlRoot(ElementName = "ControlPos", IsNullable = true)]
public class ViewPosSizeModel : IViewPosSizeModel
{
	private double mX;

	private double mY;

	private double mWidth;

	private double mHeight;

	private bool mIsMaximized;

	public static ViewSize DefaultSize => new ViewSize(50.0, 50.0, 800.0, 550.0);

	[XmlIgnore]
	public bool DefaultConstruct { get; private set; }

	[XmlAttribute(AttributeName = "X")]
	public double X
	{
		get
		{
			return mX;
		}
		set
		{
			if (mX != value)
			{
				mX = value;
			}
		}
	}

	[XmlAttribute(AttributeName = "Y")]
	public double Y
	{
		get
		{
			return mY;
		}
		set
		{
			if (mY != value)
			{
				mY = value;
			}
		}
	}

	[XmlAttribute(AttributeName = "Width")]
	public double Width
	{
		get
		{
			return mWidth;
		}
		set
		{
			if (mWidth != value)
			{
				mWidth = value;
			}
		}
	}

	[XmlAttribute(AttributeName = "Height")]
	public double Height
	{
		get
		{
			return mHeight;
		}
		set
		{
			if (mHeight != value)
			{
				mHeight = value;
			}
		}
	}

	[XmlAttribute(AttributeName = "IsMaximized")]
	public bool IsMaximized
	{
		get
		{
			return mIsMaximized;
		}
		set
		{
			if (mIsMaximized != value)
			{
				mIsMaximized = value;
			}
		}
	}

	public ViewPosSizeModel()
	{
		mX = 0.0;
		mY = 0.0;
		mWidth = 0.0;
		mHeight = 0.0;
		mIsMaximized = false;
		DefaultConstruct = true;
	}

	public ViewPosSizeModel(double x, double y, double width, double height, bool isMaximized = false)
	{
		mX = x;
		mY = y;
		mWidth = width;
		mHeight = height;
		mIsMaximized = isMaximized;
		DefaultConstruct = false;
	}

	public ViewPosSizeModel(ViewSize vs)
		: this(vs.X, vs.Y, vs.Width, vs.Height)
	{
	}

	public void SetValidPos(double SystemParameters_VirtualScreenLeft, double SystemParameters_VirtualScreenTop)
	{
		if (X < SystemParameters_VirtualScreenLeft)
		{
			X = SystemParameters_VirtualScreenLeft;
		}
		if (Y < SystemParameters_VirtualScreenTop)
		{
			Y = SystemParameters_VirtualScreenTop;
		}
	}

	public void SetWindowsState(IViewSize view)
	{
		if (view != null)
		{
			view.Left = X;
			view.Top = Y;
			view.Width = Width;
			view.Height = Height;
			view.WindowState = (IsMaximized ? WindowState.Maximized : WindowState.Normal);
		}
	}

	public void GetWindowsState(IViewSize view)
	{
		if (view != null)
		{
			X = view.Left;
			Y = view.Top;
			Width = view.Width;
			Height = view.Height;
			IsMaximized = view.WindowState == WindowState.Maximized;
		}
	}
}
