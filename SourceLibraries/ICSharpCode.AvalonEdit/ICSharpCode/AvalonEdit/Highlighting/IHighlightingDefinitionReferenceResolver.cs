namespace ICSharpCode.AvalonEdit.Highlighting;

public interface IHighlightingDefinitionReferenceResolver
{
	IHighlightingDefinition GetDefinition(string name);
}
