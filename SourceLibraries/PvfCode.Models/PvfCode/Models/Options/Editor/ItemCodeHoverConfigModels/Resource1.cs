using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace PvfCode.Models.Options.Editor.ItemCodeHoverConfigModels;

[DebuggerNonUserCode]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[CompilerGenerated]
internal class Resource1
{
	private static ResourceManager KJkZOy8Yv9;

	private static CultureInfo P98ZrIV4s5;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (KJkZOy8Yv9 == null)
			{
				KJkZOy8Yv9 = new ResourceManager("PvfCode.Models.Options.Editor.ItemCodeHoverConfigModels.Resource1", typeof(Resource1).Assembly);
			}
			return KJkZOy8Yv9;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return P98ZrIV4s5;
		}
		set
		{
			P98ZrIV4s5 = value;
		}
	}

	internal static string Comment => ResourceManager.GetString("Comment", P98ZrIV4s5);

	internal Resource1()
	{
	}
}
