using Settings.Interfaces;
using SettingsModel.Interfaces;
using SettingsModel.Models;

namespace Settings.ProgramSettings;

internal class OptionsPanel : IOptionsPanel
{
	private IEngine mQuery;

	public IEngine Options
	{
		get
		{
			return mQuery;
		}
		private set
		{
			mQuery = value;
		}
	}

	public OptionsPanel()
	{
		mQuery = Factory.CreateEngine();
	}
}
