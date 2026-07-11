using System.Collections.Generic;
using HL.HighlightingTheme;

namespace HL.Xshtd.interfaces;

public interface IHighlightingThemeDefinition
{
	string Name { get; }

	IEnumerable<GlobalStyle> GlobalStyles { get; }

	SyntaxDefinition GetNamedSyntaxDefinition(string name);
}
