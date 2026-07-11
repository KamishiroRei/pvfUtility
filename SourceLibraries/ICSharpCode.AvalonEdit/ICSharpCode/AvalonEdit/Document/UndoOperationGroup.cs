using System;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document;

internal sealed class UndoOperationGroup : IUndoableOperationWithContext, IUndoableOperation
{
	private IUndoableOperation[] undolist;

	public UndoOperationGroup(Deque<IUndoableOperation> stack, int numops)
	{
		if (stack == null)
		{
			throw new ArgumentNullException("stack");
		}
		undolist = new IUndoableOperation[numops];
		for (int i = 0; i < numops; i++)
		{
			undolist[i] = stack.PopBack();
		}
	}

	public void Undo()
	{
		for (int i = 0; i < undolist.Length; i++)
		{
			undolist[i].Undo();
		}
	}

	public void Undo(UndoStack stack)
	{
		for (int i = 0; i < undolist.Length; i++)
		{
			stack.RunUndo(undolist[i]);
		}
	}

	public void Redo()
	{
		for (int num = undolist.Length - 1; num >= 0; num--)
		{
			undolist[num].Redo();
		}
	}

	public void Redo(UndoStack stack)
	{
		for (int num = undolist.Length - 1; num >= 0; num--)
		{
			stack.RunRedo(undolist[num]);
		}
	}
}
