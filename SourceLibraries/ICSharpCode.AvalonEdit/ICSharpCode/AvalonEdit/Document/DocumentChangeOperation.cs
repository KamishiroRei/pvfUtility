namespace ICSharpCode.AvalonEdit.Document;

internal sealed class DocumentChangeOperation : IUndoableOperationWithContext, IUndoableOperation
{
	private TextDocument document;

	private DocumentChangeEventArgs change;

	public DocumentChangeOperation(TextDocument document, DocumentChangeEventArgs change)
	{
		this.document = document;
		this.change = change;
	}

	public void Undo(UndoStack stack)
	{
		stack.RegisterAffectedDocument(document);
		stack.state = 2;
		Undo();
		stack.state = 1;
	}

	public void Redo(UndoStack stack)
	{
		stack.RegisterAffectedDocument(document);
		stack.state = 2;
		Redo();
		stack.state = 1;
	}

	public void Undo()
	{
		OffsetChangeMap offsetChangeMapOrNull = change.OffsetChangeMapOrNull;
		document.Replace(change.Offset, change.InsertionLength, change.RemovedText, offsetChangeMapOrNull?.Invert());
	}

	public void Redo()
	{
		document.Replace(change.Offset, change.RemovalLength, change.InsertedText, change.OffsetChangeMapOrNull);
	}
}
