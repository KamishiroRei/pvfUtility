namespace Settings.UserProfile;

public class ViewSize
{
	public double X { get; private set; }

	public double Y { get; private set; }

	public double Width { get; private set; }

	public double Height { get; private set; }

	public ViewSize(double x, double y, double width, double height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}
}
