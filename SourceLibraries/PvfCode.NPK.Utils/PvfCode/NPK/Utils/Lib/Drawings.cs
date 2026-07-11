using System.Drawing;

namespace PvfCode.NPK.Utils.Lib;

public static class Drawings
{
	public static string GetString(this Size size)
	{
		return $"[{size.Width},{size.Height}]";
	}

	public static Size Star(this Size size, decimal step)
	{
		int width = (int)((decimal)size.Width * step);
		int height = (int)((decimal)size.Height * step);
		return new Size(width, height);
	}

	public static string GetString(this Point point)
	{
		return $"[{point.X},{point.Y}]";
	}

	public static Point Star(this Point point, decimal step)
	{
		int x = (int)((decimal)point.X * step);
		int y = (int)((decimal)point.Y * step);
		return new Point(x, y);
	}

	public static Point Add(this Point p1, Point p2)
	{
		int x = p1.X + p2.X;
		int y = p1.Y + p2.Y;
		return new Point(x, y);
	}

	public static Point Divide(this Point point, decimal step)
	{
		int x = (int)((decimal)point.X / step);
		int y = (int)((decimal)point.Y / step);
		return new Point(x, y);
	}

	public static Point Minus(this Point p1, Point p2)
	{
		int x = p1.X - p2.X;
		int y = p1.Y - p2.Y;
		return new Point(x, y);
	}

	public static Point Reverse(this Point point)
	{
		return new Point(-point.X, -point.Y);
	}

	public static Rectangle Add(this Rectangle rect1, Rectangle rect2)
	{
		int x = rect1.X + rect2.X;
		int y = rect1.X + rect2.Y;
		int width = rect1.Width + rect2.Width;
		int height = rect1.Height + rect2.Height;
		return new Rectangle(x, y, width, height);
	}
}
