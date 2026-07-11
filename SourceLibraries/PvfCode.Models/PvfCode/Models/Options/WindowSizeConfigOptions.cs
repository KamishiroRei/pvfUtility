using System.Collections.Generic;
using System.Windows;

namespace PvfCode.Models.Options;

public class WindowSizeConfigOptions
{
	private Dictionary<string, WindowSizeConfig> ooqE1QSgKO;

	public Dictionary<string, WindowSizeConfig> Windows
	{
		get
		{
			if (ooqE1QSgKO == null)
			{
				ooqE1QSgKO = new Dictionary<string, WindowSizeConfig>();
			}
			return ooqE1QSgKO;
		}
		set
		{
			ooqE1QSgKO = value;
		}
	}

	public void Save(string title, double height, double width, WindowState windowState)
	{
		if (Windows.ContainsKey(title))
		{
			WindowSizeConfig windowSizeConfig = Windows[title];
			windowSizeConfig.Height = height;
			windowSizeConfig.Widht = width;
			windowSizeConfig.WindowState = windowState;
		}
		else
		{
			WindowSizeConfig windowSizeConfig2 = new WindowSizeConfig();
			windowSizeConfig2.Height = height;
			windowSizeConfig2.Widht = width;
			windowSizeConfig2.WindowState = windowState;
			Windows.Add(title, windowSizeConfig2);
		}
	}

	public void InitWindow(Window window)
	{
		if (window != null && !string.IsNullOrEmpty(window.Title) && Windows.TryGetValue(window.Title, out WindowSizeConfig value))
		{
			window.Width = value.Widht;
			window.Height = value.Height;
			if (value.WindowState == WindowState.Maximized)
			{
				window.WindowState = value.WindowState;
			}
		}
	}

	public WindowSizeConfigOptions()
	{
	}
}
