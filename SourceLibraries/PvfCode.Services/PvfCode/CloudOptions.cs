namespace PvfCode;

public class CloudOptions
{
	public string DocumentIndexUrl { get; set; }

	public string LogServerUrl { get; set; }

	public string ApiUrl { get; set; }

	public string Log_API_KEY_HERE { get; set; }

	public void InitDefault()
	{
		DocumentIndexUrl = string.Empty;
		LogServerUrl = string.Empty;
		ApiUrl = string.Empty;
		Log_API_KEY_HERE = string.Empty;
	}

	public CloudOptions()
	{
	}
}
