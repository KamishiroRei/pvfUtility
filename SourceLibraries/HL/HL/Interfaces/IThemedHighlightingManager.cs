using System;
using System.Collections.ObjectModel;
using HL.Manager;
using HL.Xshtd.interfaces;
using ICSharpCode.AvalonEdit.Highlighting;

namespace HL.Interfaces;

public interface IThemedHighlightingManager : IHighlightingDefinitionReferenceResolver, IHighlightingThemeDefinitionReferenceResolver
{
	IHLTheme CurrentTheme { get; }

	ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions { get; }

	IHighlightingDefinition GetDefinitionByExtension(string extension);

	void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting);

	void RegisterHighlighting(string name, string[] extensions, Func<IHighlightingDefinition> lazyLoadedHighlighting);

	void SetCurrentTheme(string name);
}
