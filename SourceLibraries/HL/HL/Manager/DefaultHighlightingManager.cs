using System;
using System.IO;
using System.Xml;
using HL.HighlightingTheme;
using HL.Resources;
using HL.Xshtd.interfaces;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace HL.Manager;

internal sealed class DefaultHighlightingManager : ThemedHighlightingManager
{
	public new static readonly DefaultHighlightingManager Instance;

	static DefaultHighlightingManager()
	{
		DefaultHighlightingManager defaultHighlightingManager = new DefaultHighlightingManager();
		HLTheme hLTheme = new HLTheme("Light", "HL.Resources.Light", "Light");
		defaultHighlightingManager.ThemedHighlightingAdd(hLTheme.Key, hLTheme);
		defaultHighlightingManager.SetCurrentThemeInternal(hLTheme.Key);
		hLTheme = new HLTheme("VS2019_Dark", "Light", "VS2019 Dark", "HL.Resources.Themes", "VS2019_Dark.xshtd", defaultHighlightingManager);
		defaultHighlightingManager.ThemedHighlightingAdd(hLTheme.Key, hLTheme);
		HLResources.RegisterBuiltInHighlightings(defaultHighlightingManager, defaultHighlightingManager.CurrentTheme);
		Instance = defaultHighlightingManager;
	}

	internal void RegisterHighlighting(IHLTheme theme, string name, string[] extensions, string resourceName)
	{
		try
		{
			RegisterHighlighting(name, extensions, LoadHighlighting(theme, name, resourceName));
		}
		catch (HighlightingDefinitionInvalidException innerException)
		{
			throw new InvalidOperationException("The built-in highlighting '" + name + "' is invalid.", innerException);
		}
	}

	private Func<IHighlightingDefinition> LoadHighlighting(IHLTheme theme, string name, string resourceName)
	{
		return delegate
		{
			XshdSyntaxDefinition syntaxDefinition;
			using (Stream input = HLResources.OpenStream(GetPrefix(base.CurrentTheme.HLBaseKey), resourceName))
			{
				using XmlTextReader reader = new XmlTextReader(input);
				syntaxDefinition = HighlightingLoader.LoadXshd(reader, skipValidation: true);
			}
			IHighlightingThemeDefinition hlTheme = theme.HlTheme;
			SyntaxDefinition themedHighlights = null;
			if (hlTheme != null)
			{
				themedHighlights = hlTheme.GetNamedSyntaxDefinition(name);
			}
			return HighlightingLoader.Load(themedHighlights, syntaxDefinition, this);
		};
	}
}
