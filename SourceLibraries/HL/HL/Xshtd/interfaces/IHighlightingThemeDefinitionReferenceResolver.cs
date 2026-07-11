using HL.HighlightingTheme;

namespace HL.Xshtd.interfaces;

public interface IHighlightingThemeDefinitionReferenceResolver
{
	SyntaxDefinition GetThemeDefinition(string highlightingName);

	SyntaxDefinition GetThemeDefinition(string hlThemeName, string highlightingName);
}
