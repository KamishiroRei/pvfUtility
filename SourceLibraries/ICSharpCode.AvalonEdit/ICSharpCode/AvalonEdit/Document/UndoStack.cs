using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document;

public sealed class UndoStack : INotifyPropertyChanged
{
	internal const int StateListen = 0;

	internal const int StatePlayback = 1;

	internal const int StatePlaybackModifyDocument = 2;

	internal int state;

	private Deque<IUndoableOperation> undostack = new Deque<IUndoableOperation>();

	private Deque<IUndoableOperation> redostack = new Deque<IUndoableOperation>();

	private int sizeLimit = int.MaxValue;

	private int undoGroupDepth;

	private int actionCountInUndoGroup;

	private int optionalActionCount;

	private object lastGroupDescriptor;

	private bool allowContinue;

	private int elementsOnUndoUntilOriginalFile;

	private bool isOriginalFile = true;

	private List<TextDocument> affectedDocuments;

	public bool IsOriginalFile => isOriginalFile;

	public bool AcceptChanges => state == 0;

	public bool CanUndo => undostack.Count > 0;

	public bool CanRedo => redostack.Count > 0;

	public int SizeLimit
	{
		get
		{
			return sizeLimit;
		}
		set
		{
			if (value < 0)
			{
				ThrowUtil.CheckNotNegative(value, "value");
			}
			if (sizeLimit != value)
			{
				sizeLimit = value;
				NotifyPropertyChanged("SizeLimit");
				if (undoGroupDepth == 0)
				{
					EnforceSizeLimit();
				}
			}
		}
	}

	public object LastGroupDescriptor => lastGroupDescriptor;

	public event PropertyChangedEventHandler PropertyChanged;

	private void RecalcIsOriginalFile()
	{
		bool flag = elementsOnUndoUntilOriginalFile == 0;
		if (flag != isOriginalFile)
		{
			isOriginalFile = flag;
			NotifyPropertyChanged("IsOriginalFile");
		}
	}

	public void MarkAsOriginalFile()
	{
		elementsOnUndoUntilOriginalFile = 0;
		RecalcIsOriginalFile();
	}

	public void DiscardOriginalFileMarker()
	{
		elementsOnUndoUntilOriginalFile = int.MinValue;
		RecalcIsOriginalFile();
	}

	private void FileModified(int newElementsOnUndoStack)
	{
		if (elementsOnUndoUntilOriginalFile != int.MinValue)
		{
			elementsOnUndoUntilOriginalFile += newElementsOnUndoStack;
			if (elementsOnUndoUntilOriginalFile > undostack.Count)
			{
				elementsOnUndoUntilOriginalFile = int.MinValue;
			}
		}
	}

	private void EnforceSizeLimit()
	{
		while (undostack.Count > sizeLimit)
		{
			undostack.PopFront();
		}
		while (redostack.Count > sizeLimit)
		{
			redostack.PopFront();
		}
	}

	public void StartUndoGroup()
	{
		StartUndoGroup(null);
	}

	public void StartUndoGroup(object groupDescriptor)
	{
		if (undoGroupDepth == 0)
		{
			actionCountInUndoGroup = 0;
			optionalActionCount = 0;
			lastGroupDescriptor = groupDescriptor;
		}
		undoGroupDepth++;
	}

	public void StartContinuedUndoGroup(object groupDescriptor = null)
	{
		if (undoGroupDepth == 0)
		{
			actionCountInUndoGroup = ((allowContinue && undostack.Count > 0) ? 1 : 0);
			optionalActionCount = 0;
			lastGroupDescriptor = groupDescriptor;
		}
		undoGroupDepth++;
	}

	public void EndUndoGroup()
	{
		if (undoGroupDepth == 0)
		{
			throw new InvalidOperationException("There are no open undo groups");
		}
		undoGroupDepth--;
		if (undoGroupDepth != 0)
		{
			return;
		}
		allowContinue = true;
		if (actionCountInUndoGroup == optionalActionCount)
		{
			for (int i = 0; i < optionalActionCount; i++)
			{
				undostack.PopBack();
			}
			allowContinue = false;
		}
		else if (actionCountInUndoGroup > 1)
		{
			undostack.PushBack(new UndoOperationGroup(undostack, actionCountInUndoGroup));
			FileModified(-actionCountInUndoGroup + 1 + optionalActionCount);
		}
		EnforceSizeLimit();
		RecalcIsOriginalFile();
	}

	private void ThrowIfUndoGroupOpen()
	{
		if (undoGroupDepth != 0)
		{
			undoGroupDepth = 0;
			throw new InvalidOperationException("No undo group should be open at this point");
		}
		if (state != 0)
		{
			throw new InvalidOperationException("This method cannot be called while an undo operation is being performed");
		}
	}

	internal void RegisterAffectedDocument(TextDocument document)
	{
		if (affectedDocuments == null)
		{
			affectedDocuments = new List<TextDocument>();
		}
		if (!affectedDocuments.Contains(document))
		{
			affectedDocuments.Add(document);
			document.BeginUpdate();
		}
	}

	private void CallEndUpdateOnAffectedDocuments()
	{
		if (affectedDocuments == null)
		{
			return;
		}
		foreach (TextDocument affectedDocument in affectedDocuments)
		{
			affectedDocument.EndUpdate();
		}
		affectedDocuments = null;
	}

	public void Undo()
	{
		ThrowIfUndoGroupOpen();
		if (undostack.Count > 0)
		{
			lastGroupDescriptor = null;
			allowContinue = false;
			IUndoableOperation undoableOperation = undostack.PopBack();
			redostack.PushBack(undoableOperation);
			state = 1;
			try
			{
				RunUndo(undoableOperation);
			}
			finally
			{
				state = 0;
				FileModified(-1);
				CallEndUpdateOnAffectedDocuments();
			}
			RecalcIsOriginalFile();
			if (undostack.Count == 0)
			{
				NotifyPropertyChanged("CanUndo");
			}
			if (redostack.Count == 1)
			{
				NotifyPropertyChanged("CanRedo");
			}
		}
	}

	internal void RunUndo(IUndoableOperation op)
	{
		if (op is IUndoableOperationWithContext undoableOperationWithContext)
		{
			undoableOperationWithContext.Undo(this);
		}
		else
		{
			op.Undo();
		}
	}

	public void Redo()
	{
		ThrowIfUndoGroupOpen();
		if (redostack.Count > 0)
		{
			lastGroupDescriptor = null;
			allowContinue = false;
			IUndoableOperation undoableOperation = redostack.PopBack();
			undostack.PushBack(undoableOperation);
			state = 1;
			try
			{
				RunRedo(undoableOperation);
			}
			finally
			{
				state = 0;
				FileModified(1);
				CallEndUpdateOnAffectedDocuments();
			}
			RecalcIsOriginalFile();
			if (redostack.Count == 0)
			{
				NotifyPropertyChanged("CanRedo");
			}
			if (undostack.Count == 1)
			{
				NotifyPropertyChanged("CanUndo");
			}
		}
	}

	internal void RunRedo(IUndoableOperation op)
	{
		if (op is IUndoableOperationWithContext undoableOperationWithContext)
		{
			undoableOperationWithContext.Redo(this);
		}
		else
		{
			op.Redo();
		}
	}

	public void Push(IUndoableOperation operation)
	{
		Push(operation, isOptional: false);
	}

	public void PushOptional(IUndoableOperation operation)
	{
		if (undoGroupDepth == 0)
		{
			throw new InvalidOperationException("Cannot use PushOptional outside of undo group");
		}
		Push(operation, isOptional: true);
	}

	private void Push(IUndoableOperation operation, bool isOptional)
	{
		if (operation == null)
		{
			throw new ArgumentNullException("operation");
		}
		if (state == 0 && sizeLimit > 0)
		{
			bool num = undostack.Count == 0;
			bool num2 = undoGroupDepth == 0;
			if (num2)
			{
				StartUndoGroup();
			}
			undostack.PushBack(operation);
			actionCountInUndoGroup++;
			if (isOptional)
			{
				optionalActionCount++;
			}
			else
			{
				FileModified(1);
			}
			if (num2)
			{
				EndUndoGroup();
			}
			if (num)
			{
				NotifyPropertyChanged("CanUndo");
			}
			ClearRedoStack();
		}
	}

	public void ClearRedoStack()
	{
		if (redostack.Count != 0)
		{
			redostack.Clear();
			NotifyPropertyChanged("CanRedo");
			if (elementsOnUndoUntilOriginalFile < 0)
			{
				elementsOnUndoUntilOriginalFile = int.MinValue;
			}
		}
	}

	public void ClearAll()
	{
		ThrowIfUndoGroupOpen();
		actionCountInUndoGroup = 0;
		optionalActionCount = 0;
		if (undostack.Count != 0)
		{
			lastGroupDescriptor = null;
			allowContinue = false;
			undostack.Clear();
			NotifyPropertyChanged("CanUndo");
		}
		ClearRedoStack();
	}

	internal void Push(TextDocument document, DocumentChangeEventArgs e)
	{
		if (state == 1)
		{
			throw new InvalidOperationException("Document changes during undo/redo operations are not allowed.");
		}
		if (state == 2)
		{
			state = 1;
		}
		else
		{
			Push(new DocumentChangeOperation(document, e));
		}
	}

	private void NotifyPropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
