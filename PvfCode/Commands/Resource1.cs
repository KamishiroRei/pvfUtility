using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace PvfCode.Commands;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resource1
{
	private static ResourceManager QVU6e3GV8Y;

	private static CultureInfo gan6tsBpJu;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (QVU6e3GV8Y == null)
			{
				QVU6e3GV8Y = new ResourceManager("Commands.Resource1", typeof(Resource1).Assembly);
			}
			return QVU6e3GV8Y;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return gan6tsBpJu;
		}
		set
		{
			gan6tsBpJu = value;
		}
	}

	internal static string ItemCodeHoverConfig => ResourceManager.GetString("ItemCodeHoverConfig", gan6tsBpJu);

	internal Resource1()
	{
	}
}
