using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace PvfCode.Models.Pvf;

[CompilerGenerated]
[DebuggerNonUserCode]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
public class lang
{
	private static ResourceManager loKkMRlAo8;

	private static CultureInfo rlokapFxj9;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (loKkMRlAo8 == null)
			{
				loKkMRlAo8 = new ResourceManager("PvfCode.Models.Pvf.lang", typeof(lang).Assembly);
			}
			return loKkMRlAo8;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
	{
		get
		{
			return rlokapFxj9;
		}
		set
		{
			rlokapFxj9 = value;
		}
	}

	public static string PraseInfo => ResourceManager.GetString("PraseInfo", rlokapFxj9);

	public static string privatekey_pem => ResourceManager.GetString("privatekey.pem", rlokapFxj9);

	public static string publickey_pem => ResourceManager.GetString("publickey.pem", rlokapFxj9);

	public lang()
	{
	}
}
