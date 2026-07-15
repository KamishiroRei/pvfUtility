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
	private static ResourceManager resourceManager;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (resourceManager == null)
			{
				resourceManager = new ResourceManager("PvfCode.Models.Pvf.lang", typeof(lang).Assembly);
			}
			return resourceManager;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	public static string PraseInfo => ResourceManager.GetString("PraseInfo", resourceCulture);

	public static string privatekey_pem => ResourceManager.GetString("privatekey.pem", resourceCulture);

	public static string publickey_pem => ResourceManager.GetString("publickey.pem", resourceCulture);

	public lang()
	{
	}
}
