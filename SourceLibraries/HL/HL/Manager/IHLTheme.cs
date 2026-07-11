using System.Collections.ObjectModel;
using HL.HighlightingTheme;
using HL.Xshtd;
using HL.Xshtd.interfaces;
using ICSharpCode.AvalonEdit.Highlighting;

namespace HL.Manager;

public interface IHLTheme
{
	string Key { get; }

	string HLBasePrefix { get; }

	string HLBaseKey { get; }

	string HLThemePrefix { get; }

	string HLThemeFileName { get; }

	string DisplayName { get; }

	ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions { get; }

	IHighlightingThemeDefinition HlTheme { get; }

	bool IsBuiltInThemesRegistered { get; set; }

	IHighlightingDefinition GetDefinition(string name);

	IHighlightingDefinition GetDefinitionByExtension(string extension);

	void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting);

	SyntaxDefinition GetThemeDefinition(string highlightingName);

	XhstdThemeDefinition ResolveHighLightingTheme(string hLPrefix, string hLThemeName);
}
