using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace PvfCode.Services.PvfParsingNew;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resource1
{
	private static ResourceManager resourceManager;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceManager == null)
			{
				resourceManager = new ResourceManager("PvfCode.Services.PvfParsingNew.Resource1", typeof(Resource1).Assembly);
			}
			return resourceManager;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
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

	internal static string ScriptFileParserConfiger => ResourceManager.GetString("ScriptFileParserConfiger", resourceCulture);

	internal Resource1()
	{
	}
}
