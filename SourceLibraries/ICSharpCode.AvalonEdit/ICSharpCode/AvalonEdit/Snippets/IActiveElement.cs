using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets;

public interface IActiveElement
{
	bool IsEditable { get; }

	ISegment Segment { get; }

	void OnInsertionCompleted();

	void Deactivate(SnippetEventArgs e);
}
