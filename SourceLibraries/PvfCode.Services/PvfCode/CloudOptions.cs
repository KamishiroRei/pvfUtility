using System.Runtime.CompilerServices;

namespace PvfCode;

public class CloudOptions
{
	[CompilerGenerated]
	private string dUyHK8sIA;

	[CompilerGenerated]
	private string C76A381OK;

	[CompilerGenerated]
	private string qHhaxmBGw;

	[CompilerGenerated]
	private string BRTKjS8Zl;

	public string DocumentIndexUrl
	{
		[CompilerGenerated]
		get
		{
			return dUyHK8sIA;
		}
		[CompilerGenerated]
		set
		{
			dUyHK8sIA = value;
		}
	}

	public string LogServerUrl
	{
		[CompilerGenerated]
		get
		{
			return C76A381OK;
		}
		[CompilerGenerated]
		set
		{
			C76A381OK = value;
		}
	}

	public string ApiUrl
	{
		[CompilerGenerated]
		get
		{
			return qHhaxmBGw;
		}
		[CompilerGenerated]
		set
		{
			qHhaxmBGw = value;
		}
	}

	public string Log_API_KEY_HERE
	{
		[CompilerGenerated]
		get
		{
			return BRTKjS8Zl;
		}
		[CompilerGenerated]
		set
		{
			BRTKjS8Zl = value;
		}
	}

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
