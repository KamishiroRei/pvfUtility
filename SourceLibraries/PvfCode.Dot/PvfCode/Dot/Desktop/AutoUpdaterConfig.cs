namespace PvfCode.Dot.Desktop;

public class AutoUpdaterConfig
{
	public string DownloadUrl { get; set; }

	public string Version { get; set; }

	public string Changelog { get; set; }

	public Mandatory Mandatory { get; set; }
}
