using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace PvfCode.Models.Options;

[CompilerGenerated]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
public class Resource1
{
	private static ResourceManager jvcEHR0Zmc;

	private static CultureInfo KGjE7OFKZe;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (jvcEHR0Zmc == null)
			{
				jvcEHR0Zmc = new ResourceManager("PvfCode.Models.Options.Resource1", typeof(Resource1).Assembly);
			}
			return jvcEHR0Zmc;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return KGjE7OFKZe;
		}
		set
		{
			KGjE7OFKZe = value;
		}
	}

	public static string Comment => ResourceManager.GetString("Comment", KGjE7OFKZe);

	public static string ItemCodeHoverConfig => ResourceManager.GetString("ItemCodeHoverConfig", KGjE7OFKZe);

	public static byte[] pvfUtilityLayout => (byte[])ResourceManager.GetObject("pvfUtilityLayout", KGjE7OFKZe);

	internal Resource1()
	{
	}
}
