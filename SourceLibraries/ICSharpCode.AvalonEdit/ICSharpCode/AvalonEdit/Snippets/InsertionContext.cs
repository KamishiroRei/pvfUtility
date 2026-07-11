using System;
using System.Collections.Generic;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Snippets;

public class InsertionContext : IWeakEventListener
{
	private enum Status
	{
		Insertion,
		RaisingInsertionCompleted,
		Interactive,
		RaisingDeactivated,
		Deactivated
	}

	private Status currentStatus;

	private readonly int startPosition;

	private AnchorSegment wholeSnippetAnchor;

	private bool deactivateIfSnippetEmpty;

	private Dictionary<SnippetElement, IActiveElement> elementMap = new Dictionary<SnippetElement, IActiveElement>();

	private List<IActiveElement> registeredElements = new List<IActiveElement>();

	private SnippetInputHandler myInputHandler;

	public TextArea TextArea { get; private set; }

	public TextDocument Document { get; private set; }

	public string SelectedText { get; private set; }

	public string Indentation { get; private set; }

	public string Tab { get; private set; }

	public string LineTerminator { get; private set; }

	public int InsertionPosition { get; set; }

	public int StartPosition
	{
		get
		{
			if (wholeSnippetAnchor != null)
			{
				return wholeSnippetAnchor.Offset;
			}
			return startPosition;
		}
	}

	public IEnumerable<IActiveElement> ActiveElements => registeredElements;

	public event EventHandler InsertionCompleted;

	public event EventHandler<SnippetEventArgs> Deactivated;

	public InsertionContext(TextArea textArea, int insertionPosition)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		TextArea = textArea;
		Document = textArea.Document;
		SelectedText = textArea.Selection.GetText();
		InsertionPosition = insertionPosition;
		startPosition = insertionPosition;
		DocumentLine lineByOffset = Document.GetLineByOffset(insertionPosition);
		ISegment whitespaceAfter = TextUtilities.GetWhitespaceAfter(Document, lineByOffset.Offset);
		Indentation = Document.GetText(whitespaceAfter.Offset, Math.Min(whitespaceAfter.EndOffset, insertionPosition) - whitespaceAfter.Offset);
		Tab = textArea.Options.IndentationString;
		LineTerminator = TextUtilities.GetNewLineFromDocument(Document, lineByOffset.LineNumber);
	}

	public void InsertText(string text)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (currentStatus != Status.Insertion)
		{
			throw new InvalidOperationException();
		}
		text = text.Replace("\t", Tab);
		using (Document.RunUpdate())
		{
			int num = 0;
			SimpleSegment simpleSegment;
			while ((simpleSegment = NewLineFinder.NextNewLine(text, num)) != SimpleSegment.Invalid)
			{
				string text2 = text.Substring(num, simpleSegment.Offset - num) + LineTerminator + Indentation;
				Document.Insert(InsertionPosition, text2);
				InsertionPosition += text2.Length;
				num = simpleSegment.EndOffset;
			}
			string text3 = text.Substring(num);
			Document.Insert(InsertionPosition, text3);
			InsertionPosition += text3.Length;
		}
	}

	public void RegisterActiveElement(SnippetElement owner, IActiveElement element)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (currentStatus != Status.Insertion)
		{
			throw new InvalidOperationException();
		}
		elementMap.Add(owner, element);
		registeredElements.Add(element);
	}

	public IActiveElement GetActiveElement(SnippetElement owner)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (elementMap.TryGetValue(owner, out var value))
		{
			return value;
		}
		return null;
	}

	public void RaiseInsertionCompleted(EventArgs e)
	{
		if (currentStatus != Status.Insertion)
		{
			throw new InvalidOperationException();
		}
		if (e == null)
		{
			e = EventArgs.Empty;
		}
		currentStatus = Status.RaisingInsertionCompleted;
		int insertionPosition = InsertionPosition;
		wholeSnippetAnchor = new AnchorSegment(Document, startPosition, insertionPosition - startPosition);
		WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument>.AddListener(Document, this);
		deactivateIfSnippetEmpty = insertionPosition != startPosition;
		foreach (IActiveElement registeredElement in registeredElements)
		{
			registeredElement.OnInsertionCompleted();
		}
		if (this.InsertionCompleted != null)
		{
			this.InsertionCompleted(this, e);
		}
		currentStatus = Status.Interactive;
		if (registeredElements.Count == 0)
		{
			Deactivate(new SnippetEventArgs(DeactivateReason.NoActiveElements));
			return;
		}
		myInputHandler = new SnippetInputHandler(this);
		foreach (TextAreaStackedInputHandler stackedInputHandler in TextArea.StackedInputHandlers)
		{
			if (stackedInputHandler is SnippetInputHandler)
			{
				TextArea.PopStackedInputHandler(stackedInputHandler);
			}
		}
		TextArea.PushStackedInputHandler(myInputHandler);
	}

	public void Deactivate(SnippetEventArgs e)
	{
		if (currentStatus == Status.Deactivated || currentStatus == Status.RaisingDeactivated)
		{
			return;
		}
		if (currentStatus != Status.Interactive)
		{
			throw new InvalidOperationException("Cannot call Deactivate() until RaiseInsertionCompleted() has finished.");
		}
		if (e == null)
		{
			e = new SnippetEventArgs(DeactivateReason.Unknown);
		}
		WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument>.RemoveListener(Document, this);
		currentStatus = Status.RaisingDeactivated;
		TextArea.PopStackedInputHandler(myInputHandler);
		foreach (IActiveElement registeredElement in registeredElements)
		{
			registeredElement.Deactivate(e);
		}
		if (this.Deactivated != null)
		{
			this.Deactivated(this, e);
		}
		currentStatus = Status.Deactivated;
	}

	bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		return ReceiveWeakEvent(managerType, sender, e);
	}

	protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(TextDocumentWeakEventManager.UpdateFinished))
		{
			if (wholeSnippetAnchor.Length == 0 && deactivateIfSnippetEmpty)
			{
				Deactivate(new SnippetEventArgs(DeactivateReason.Deleted));
			}
			return true;
		}
		return false;
	}

	public void Link(ISegment mainElement, ISegment[] boundElements)
	{
		SnippetReplaceableTextElement snippetReplaceableTextElement = new SnippetReplaceableTextElement
		{
			Text = Document.GetText(mainElement)
		};
		RegisterActiveElement(snippetReplaceableTextElement, new ReplaceableActiveElement(this, mainElement.Offset, mainElement.EndOffset));
		foreach (ISegment segment in boundElements)
		{
			SnippetBoundElement snippetBoundElement = new SnippetBoundElement
			{
				TargetElement = snippetReplaceableTextElement
			};
			TextAnchor textAnchor = Document.CreateAnchor(segment.Offset);
			textAnchor.MovementType = AnchorMovementType.BeforeInsertion;
			textAnchor.SurviveDeletion = true;
			TextAnchor textAnchor2 = Document.CreateAnchor(segment.EndOffset);
			textAnchor2.MovementType = AnchorMovementType.BeforeInsertion;
			textAnchor2.SurviveDeletion = true;
			RegisterActiveElement(snippetBoundElement, new BoundActiveElement(this, snippetReplaceableTextElement, snippetBoundElement, new AnchorSegment(textAnchor, textAnchor2)));
		}
	}
}
