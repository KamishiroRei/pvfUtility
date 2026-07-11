namespace ICSharpCode.AvalonEdit.Document;

public interface ILineTracker
{
	void BeforeRemoveLine(DocumentLine line);

	void SetLineLength(DocumentLine line, int newTotalLength);

	void LineInserted(DocumentLine insertionPos, DocumentLine newLine);

	void RebuildDocument();

	void ChangeComplete(DocumentChangeEventArgs e);
}
