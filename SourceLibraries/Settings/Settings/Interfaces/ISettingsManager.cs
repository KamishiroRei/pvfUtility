using System.Collections.Generic;
using Settings.ProgramSettings;

namespace Settings.Interfaces;

public interface ISettingsManager : IOptionsPanel
{
	IProfile SessionData { get; }

	int IconSizeMin { get; }

	int IconSizeMax { get; }

	int FontSizeMin { get; }

	int FontSizeMax { get; }

	int DefaultIconSize { get; }

	int DefaultFontSize { get; }

	int DefaultFixedFontSize { get; }

	void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft, double SystemParameters_VirtualScreenTop);

	void LoadSessionData(string sessionDataFileName);

	bool SaveSessionData(string sessionDataFileName, IProfile model);

	IEnumerable<LanguageCollection> GetSupportedLanguages();
}
