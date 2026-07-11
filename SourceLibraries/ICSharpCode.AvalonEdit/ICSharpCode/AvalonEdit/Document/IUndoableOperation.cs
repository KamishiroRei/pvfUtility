namespace ICSharpCode.AvalonEdit.Document;

public interface IUndoableOperation
{
	void Undo();

	void Redo();
}
