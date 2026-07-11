using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting;

public class DocumentHighlighter : ILineTracker, IHighlighter, IDisposable
{
	private readonly CompressingTreeList<ImmutableStack<HighlightingSpan>> storedSpanStacks = new CompressingTreeList<ImmutableStack<HighlightingSpan>>(object.ReferenceEquals);

	private readonly CompressingTreeList<bool> isValid = new CompressingTreeList<bool>((bool a, bool b) => a == b);

	private readonly IDocument document;

	private readonly IHighlightingDefinition definition;

	private readonly HighlightingEngine engine;

	private readonly WeakLineTracker weakLineTracker;

	private bool isHighlighting;

	private bool isInHighlightingGroup;

	private bool isDisposed;

	private ImmutableStack<HighlightingSpan> initialSpanStack = ImmutableStack<HighlightingSpan>.Empty;

	private int firstInvalidLine;

	public IDocument Document => document;

	public ImmutableStack<HighlightingSpan> InitialSpanStack
	{
		get
		{
			return initialSpanStack;
		}
		set
		{
			initialSpanStack = value ?? ImmutableStack<HighlightingSpan>.Empty;
			InvalidateHighlighting();
		}
	}

	public HighlightingColor DefaultTextColor => null;

	public event HighlightingStateChangedEventHandler HighlightingStateChanged;

	public DocumentHighlighter(TextDocument document, IHighlightingDefinition definition)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		if (definition == null)
		{
			throw new ArgumentNullException("definition");
		}
		this.document = document;
		this.definition = definition;
		engine = new HighlightingEngine(definition.MainRuleSet);
		document.VerifyAccess();
		weakLineTracker = WeakLineTracker.Register(document, this);
		InvalidateSpanStacks();
	}

	public void Dispose()
	{
		if (weakLineTracker != null)
		{
			weakLineTracker.Deregister();
		}
		isDisposed = true;
	}

	void ILineTracker.BeforeRemoveLine(DocumentLine line)
	{
		CheckIsHighlighting();
		int lineNumber = line.LineNumber;
		storedSpanStacks.RemoveAt(lineNumber);
		isValid.RemoveAt(lineNumber);
		if (lineNumber < isValid.Count)
		{
			isValid[lineNumber] = false;
			if (lineNumber < firstInvalidLine)
			{
				firstInvalidLine = lineNumber;
			}
		}
	}

	void ILineTracker.SetLineLength(DocumentLine line, int newTotalLength)
	{
		CheckIsHighlighting();
		int lineNumber = line.LineNumber;
		isValid[lineNumber] = false;
		if (lineNumber < firstInvalidLine)
		{
			firstInvalidLine = lineNumber;
		}
	}

	void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
	{
		CheckIsHighlighting();
		int lineNumber = newLine.LineNumber;
		storedSpanStacks.Insert(lineNumber, null);
		isValid.Insert(lineNumber, item: false);
		if (lineNumber < firstInvalidLine)
		{
			firstInvalidLine = lineNumber;
		}
	}

	void ILineTracker.RebuildDocument()
	{
		InvalidateSpanStacks();
	}

	void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
	{
	}

	public void InvalidateHighlighting()
	{
		InvalidateSpanStacks();
		OnHighlightStateChanged(1, document.LineCount);
	}

	private void InvalidateSpanStacks()
	{
		CheckIsHighlighting();
		storedSpanStacks.Clear();
		storedSpanStacks.Add(initialSpanStack);
		storedSpanStacks.InsertRange(1, document.LineCount, null);
		isValid.Clear();
		isValid.Add(item: true);
		isValid.InsertRange(1, document.LineCount, item: false);
		firstInvalidLine = 1;
	}

	public HighlightedLine HighlightLine(int lineNumber)
	{
		ThrowUtil.CheckInRangeInclusive(lineNumber, "lineNumber", 1, document.LineCount);
		CheckIsHighlighting();
		isHighlighting = true;
		try
		{
			HighlightUpTo(lineNumber - 1);
			IDocumentLine lineByNumber = document.GetLineByNumber(lineNumber);
			HighlightedLine result = engine.HighlightLine(document, lineByNumber);
			UpdateTreeList(lineNumber);
			return result;
		}
		finally
		{
			isHighlighting = false;
		}
	}

	public ImmutableStack<HighlightingSpan> GetSpanStack(int lineNumber)
	{
		ThrowUtil.CheckInRangeInclusive(lineNumber, "lineNumber", 0, document.LineCount);
		if (firstInvalidLine <= lineNumber)
		{
			UpdateHighlightingState(lineNumber);
		}
		return storedSpanStacks[lineNumber];
	}

	public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
	{
		return from s in GetSpanStack(lineNumber)
			select s.SpanColor into s
			where s != null
			select s;
	}

	private void CheckIsHighlighting()
	{
		if (isDisposed)
		{
			throw new ObjectDisposedException("DocumentHighlighter");
		}
		if (isHighlighting)
		{
			throw new InvalidOperationException("Invalid call - a highlighting operation is currently running.");
		}
	}

	public void UpdateHighlightingState(int lineNumber)
	{
		CheckIsHighlighting();
		isHighlighting = true;
		try
		{
			HighlightUpTo(lineNumber);
		}
		finally
		{
			isHighlighting = false;
		}
	}

	private void HighlightUpTo(int targetLineNumber)
	{
		for (int i = 0; i <= targetLineNumber; i++)
		{
			if (firstInvalidLine > i)
			{
				if (firstInvalidLine > targetLineNumber)
				{
					engine.CurrentSpanStack = storedSpanStacks[targetLineNumber];
					break;
				}
				engine.CurrentSpanStack = storedSpanStacks[firstInvalidLine - 1];
				i = firstInvalidLine;
			}
			engine.ScanLine(document, document.GetLineByNumber(i));
			UpdateTreeList(i);
		}
	}

	private void UpdateTreeList(int lineNumber)
	{
		if (!EqualSpanStacks(engine.CurrentSpanStack, storedSpanStacks[lineNumber]))
		{
			isValid[lineNumber] = true;
			storedSpanStacks[lineNumber] = engine.CurrentSpanStack;
			if (lineNumber + 1 < isValid.Count)
			{
				isValid[lineNumber + 1] = false;
				firstInvalidLine = lineNumber + 1;
			}
			else
			{
				firstInvalidLine = int.MaxValue;
			}
			if (lineNumber + 1 < document.LineCount)
			{
				OnHighlightStateChanged(lineNumber + 1, lineNumber + 1);
			}
		}
		else if (firstInvalidLine == lineNumber)
		{
			isValid[lineNumber] = true;
			firstInvalidLine = isValid.IndexOf(item: false);
			if (firstInvalidLine < 0)
			{
				firstInvalidLine = int.MaxValue;
			}
		}
	}

	private static bool EqualSpanStacks(ImmutableStack<HighlightingSpan> a, ImmutableStack<HighlightingSpan> b)
	{
		if (a == b)
		{
			return true;
		}
		if (a == null || b == null)
		{
			return false;
		}
		while (!a.IsEmpty && !b.IsEmpty)
		{
			if (a.Peek() != b.Peek())
			{
				return false;
			}
			a = a.Pop();
			b = b.Pop();
			if (a == b)
			{
				return true;
			}
		}
		if (a.IsEmpty)
		{
			return b.IsEmpty;
		}
		return false;
	}

	protected virtual void OnHighlightStateChanged(int fromLineNumber, int toLineNumber)
	{
		if (this.HighlightingStateChanged != null)
		{
			this.HighlightingStateChanged(fromLineNumber, toLineNumber);
		}
	}

	public void BeginHighlighting()
	{
		if (isInHighlightingGroup)
		{
			throw new InvalidOperationException("Highlighting group is already open");
		}
		isInHighlightingGroup = true;
	}

	public void EndHighlighting()
	{
		if (!isInHighlightingGroup)
		{
			throw new InvalidOperationException("Highlighting group is not open");
		}
		isInHighlightingGroup = false;
	}

	public HighlightingColor GetNamedColor(string name)
	{
		return definition.GetNamedColor(name);
	}
}
