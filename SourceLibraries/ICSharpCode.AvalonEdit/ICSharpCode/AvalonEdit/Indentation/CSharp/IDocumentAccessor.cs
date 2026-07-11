namespace ICSharpCode.AvalonEdit.Indentation.CSharp;

public interface IDocumentAccessor
{
	bool IsReadOnly { get; }

	int LineNumber { get; }

	string Text { get; set; }

	bool MoveNext();
}
