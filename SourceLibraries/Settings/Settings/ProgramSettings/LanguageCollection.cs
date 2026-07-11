namespace Settings.ProgramSettings;

public class LanguageCollection
{
	public string Language { get; set; }

	public string Locale { get; set; }

	public string Name { get; set; }

	public string BCP47
	{
		get
		{
			if (!string.IsNullOrEmpty(Locale))
			{
				return $"{Language}-{Locale}";
			}
			return $"{Language}";
		}
	}

	public string DisplayName => $"{Name} {BCP47}";
}
