using System.Windows;

namespace Settings.UserProfile;

public interface IViewSize
{
	double Left { get; set; }

	double Top { get; set; }

	double Width { get; set; }

	double Height { get; set; }

	WindowState WindowState { get; set; }
}
