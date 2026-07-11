using SettingsModel.Interfaces;

namespace SettingsModel.Models;

public static class Factory
{
	public static IEngine CreateEngine()
	{
		return new OptionsEngine();
	}
}
