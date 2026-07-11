namespace Settings.Interfaces;

public interface IOptions
{
	bool IsDirty { get; set; }

	string LanguageSelected { get; set; }

	bool ReloadOpenFilesOnAppStart { get; set; }

	string SourceFilePath { get; set; }

	string DefaultSourceLanguage { get; set; }

	string DefaultTargetLanguage { get; set; }

	string DefaultDefaultSourceLanguage { get; }

	string DefaultDefaultTargetLanguage { get; }

	int DefaultIconSize { get; }

	int IconSizeMin { get; }

	int IconSizeMax { get; }

	int DefaultFontSize { get; }

	int FontSizeMin { get; }

	int FontSizeMax { get; }

	void SetDirtyFlag(bool flag);

	void SetIconSize(int size);

	void SetFontSize(int size);
}
