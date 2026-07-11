using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace PvfCode.Properties;

[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
[CompilerGenerated]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance;

	public static Settings Default => defaultInstance;

	public Settings()
	{
	}

	static Settings()
	{
		defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
