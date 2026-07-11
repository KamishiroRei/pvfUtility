using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Indentation;

public interface IIndentationStrategy
{
	void IndentLine(TextDocument document, DocumentLine line);

	void IndentLines(TextDocument document, int beginLine, int endLine);
}
