using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document;

public sealed class TextDocument : IDocument, ITextSource, IServiceProvider, INotifyPropertyChanged
{
	private readonly object lockObject = new object();

	private Thread owner = Thread.CurrentThread;

	private readonly Rope<char> rope;

	private readonly DocumentLineTree lineTree;

	private readonly LineManager lineManager;

	private readonly TextAnchorTree anchorTree;

	private readonly TextSourceVersionProvider versionProvider = new TextSourceVersionProvider();

	private WeakReference cachedText;

	private int beginUpdateCount;

	private int oldTextLength;

	private int oldLineCount;

	private bool fireTextChanged;

	internal bool inDocumentChanging;

	private readonly ObservableCollection<ILineTracker> lineTrackers = new ObservableCollection<ILineTracker>();

	private UndoStack undoStack;

	private IServiceProvider serviceProvider;

	private string fileName;

	public string Text
	{
		get
		{
			VerifyAccess();
			string text = ((cachedText != null) ? (cachedText.Target as string) : null);
			if (text == null)
			{
				text = rope.ToString();
				cachedText = new WeakReference(text);
			}
			return text;
		}
		set
		{
			VerifyAccess();
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Replace(0, rope.Length, value);
		}
	}

	public int TextLength
	{
		get
		{
			VerifyAccess();
			return rope.Length;
		}
	}

	public ITextSourceVersion Version => versionProvider.CurrentVersion;

	public bool IsInUpdate
	{
		get
		{
			VerifyAccess();
			return beginUpdateCount > 0;
		}
	}

	public IList<DocumentLine> Lines => lineTree;

	public IList<ILineTracker> LineTrackers
	{
		get
		{
			VerifyAccess();
			return lineTrackers;
		}
	}

	public UndoStack UndoStack
	{
		get
		{
			return undoStack;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			if (value != undoStack)
			{
				undoStack.ClearAll();
				undoStack = value;
				OnPropertyChanged("UndoStack");
			}
		}
	}

	public int LineCount
	{
		get
		{
			VerifyAccess();
			return lineTree.LineCount;
		}
	}

	public IServiceProvider ServiceProvider
	{
		get
		{
			VerifyAccess();
			if (serviceProvider == null)
			{
				ServiceContainer serviceContainer = new ServiceContainer();
				serviceContainer.AddService(typeof(IDocument), this);
				serviceContainer.AddService(typeof(TextDocument), this);
				serviceProvider = serviceContainer;
			}
			return serviceProvider;
		}
		set
		{
			VerifyAccess();
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			serviceProvider = value;
		}
	}

	public string FileName
	{
		get
		{
			return fileName;
		}
		set
		{
			if (fileName != value)
			{
				fileName = value;
				OnFileNameChanged(EventArgs.Empty);
			}
		}
	}

	public event EventHandler TextChanged;

	event EventHandler IDocument.ChangeCompleted
	{
		add
		{
			TextChanged += value;
		}
		remove
		{
			TextChanged -= value;
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public event EventHandler<DocumentChangeEventArgs> Changing;

	private event EventHandler<TextChangeEventArgs> textChanging;

	event EventHandler<TextChangeEventArgs> IDocument.TextChanging
	{
		add
		{
			textChanging += value;
		}
		remove
		{
			textChanging -= value;
		}
	}

	public event EventHandler<DocumentChangeEventArgs> Changed;

	private event EventHandler<TextChangeEventArgs> textChanged;

	event EventHandler<TextChangeEventArgs> IDocument.TextChanged
	{
		add
		{
			textChanged += value;
		}
		remove
		{
			textChanged -= value;
		}
	}

	public event EventHandler UpdateStarted;

	public event EventHandler UpdateFinished;

	public event EventHandler FileNameChanged;

	public void VerifyAccess()
	{
		if (Thread.CurrentThread != owner)
		{
			throw new InvalidOperationException("TextDocument can be accessed only from the thread that owns it.");
		}
	}

	public void SetOwnerThread(Thread newOwner)
	{
		lock (lockObject)
		{
			if (owner != null)
			{
				VerifyAccess();
			}
			owner = newOwner;
		}
	}

	public TextDocument()
		: this(string.Empty)
	{
	}

	public TextDocument(IEnumerable<char> initialText)
	{
		if (initialText == null)
		{
			throw new ArgumentNullException("initialText");
		}
		rope = new Rope<char>(initialText);
		lineTree = new DocumentLineTree(this);
		lineManager = new LineManager(lineTree, this);
		lineTrackers.CollectionChanged += delegate
		{
			lineManager.UpdateListOfLineTrackers();
		};
		anchorTree = new TextAnchorTree(this);
		undoStack = new UndoStack();
		FireChangeEvents();
	}

	public TextDocument(ITextSource initialText)
		: this(GetTextFromTextSource(initialText))
	{
	}

	private static IEnumerable<char> GetTextFromTextSource(ITextSource textSource)
	{
		if (textSource == null)
		{
			throw new ArgumentNullException("textSource");
		}
		if (textSource is RopeTextSource ropeTextSource)
		{
			return ropeTextSource.GetRope();
		}
		if (textSource is TextDocument textDocument)
		{
			return textDocument.rope;
		}
		return textSource.Text;
	}

	private void ThrowIfRangeInvalid(int offset, int length)
	{
		if (offset < 0 || offset > rope.Length)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + rope.Length.ToString(CultureInfo.InvariantCulture));
		}
		if (length < 0 || offset + length > rope.Length)
		{
			throw new ArgumentOutOfRangeException("length", length, "0 <= length, offset(" + offset + ")+length <= " + rope.Length.ToString(CultureInfo.InvariantCulture));
		}
	}

	public string GetText(int offset, int length)
	{
		VerifyAccess();
		return rope.ToString(offset, length);
	}

	public string GetText(ISegment segment)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		return GetText(segment.Offset, segment.Length);
	}

	public int IndexOf(char c, int startIndex, int count)
	{
		return rope.IndexOf(c, startIndex, count);
	}

	public int LastIndexOf(char c, int startIndex, int count)
	{
		return rope.LastIndexOf(c, startIndex, count);
	}

	public int IndexOfAny(char[] anyOf, int startIndex, int count)
	{
		return rope.IndexOfAny(anyOf, startIndex, count);
	}

	public int IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
	{
		return rope.IndexOf(searchText, startIndex, count, comparisonType);
	}

	public int LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
	{
		return rope.LastIndexOf(searchText, startIndex, count, comparisonType);
	}

	public char GetCharAt(int offset)
	{
		return rope[offset];
	}

	public ITextSource CreateSnapshot()
	{
		lock (lockObject)
		{
			return new RopeTextSource(rope, versionProvider.CurrentVersion);
		}
	}

	public ITextSource CreateSnapshot(int offset, int length)
	{
		lock (lockObject)
		{
			return new RopeTextSource(rope.GetRange(offset, length));
		}
	}

	public TextReader CreateReader()
	{
		lock (lockObject)
		{
			return new RopeTextReader(rope);
		}
	}

	public TextReader CreateReader(int offset, int length)
	{
		lock (lockObject)
		{
			return new RopeTextReader(rope.GetRange(offset, length));
		}
	}

	public void WriteTextTo(TextWriter writer)
	{
		VerifyAccess();
		rope.WriteTo(writer, 0, rope.Length);
	}

	public void WriteTextTo(TextWriter writer, int offset, int length)
	{
		VerifyAccess();
		rope.WriteTo(writer, offset, length);
	}

	public IDisposable RunUpdate()
	{
		BeginUpdate();
		return new CallbackOnDispose(EndUpdate);
	}

	public void BeginUpdate()
	{
		VerifyAccess();
		if (inDocumentChanging)
		{
			throw new InvalidOperationException("Cannot change document within another document change.");
		}
		beginUpdateCount++;
		if (beginUpdateCount == 1)
		{
			undoStack.StartUndoGroup();
			if (this.UpdateStarted != null)
			{
				this.UpdateStarted(this, EventArgs.Empty);
			}
		}
	}

	public void EndUpdate()
	{
		VerifyAccess();
		if (inDocumentChanging)
		{
			throw new InvalidOperationException("Cannot end update within document change.");
		}
		if (beginUpdateCount == 0)
		{
			throw new InvalidOperationException("No update is active.");
		}
		if (beginUpdateCount == 1)
		{
			FireChangeEvents();
			undoStack.EndUndoGroup();
			beginUpdateCount = 0;
			if (this.UpdateFinished != null)
			{
				this.UpdateFinished(this, EventArgs.Empty);
			}
		}
		else
		{
			beginUpdateCount--;
		}
	}

	void IDocument.StartUndoableAction()
	{
		BeginUpdate();
	}

	void IDocument.EndUndoableAction()
	{
		EndUpdate();
	}

	IDisposable IDocument.OpenUndoGroup()
	{
		return RunUpdate();
	}

	internal void FireChangeEvents()
	{
		while (fireTextChanged)
		{
			fireTextChanged = false;
			if (this.TextChanged != null)
			{
				this.TextChanged(this, EventArgs.Empty);
			}
			OnPropertyChanged("Text");
			int length = rope.Length;
			if (length != oldTextLength)
			{
				oldTextLength = length;
				OnPropertyChanged("TextLength");
			}
			int lineCount = lineTree.LineCount;
			if (lineCount != oldLineCount)
			{
				oldLineCount = lineCount;
				OnPropertyChanged("LineCount");
			}
		}
	}

	private void OnPropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public void Insert(int offset, string text)
	{
		Replace(offset, 0, new StringTextSource(text), null);
	}

	public void Insert(int offset, ITextSource text)
	{
		Replace(offset, 0, text, null);
	}

	public void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType)
	{
		if (defaultAnchorMovementType == AnchorMovementType.BeforeInsertion)
		{
			Replace(offset, 0, new StringTextSource(text), OffsetChangeMappingType.KeepAnchorBeforeInsertion);
		}
		else
		{
			Replace(offset, 0, new StringTextSource(text), null);
		}
	}

	public void Insert(int offset, ITextSource text, AnchorMovementType defaultAnchorMovementType)
	{
		if (defaultAnchorMovementType == AnchorMovementType.BeforeInsertion)
		{
			Replace(offset, 0, text, OffsetChangeMappingType.KeepAnchorBeforeInsertion);
		}
		else
		{
			Replace(offset, 0, text, null);
		}
	}

	public void Remove(ISegment segment)
	{
		Replace(segment, string.Empty);
	}

	public void Remove(int offset, int length)
	{
		Replace(offset, length, StringTextSource.Empty);
	}

	public void Replace(ISegment segment, string text)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		Replace(segment.Offset, segment.Length, new StringTextSource(text), null);
	}

	public void Replace(ISegment segment, ITextSource text)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		Replace(segment.Offset, segment.Length, text, null);
	}

	public void Replace(int offset, int length, string text)
	{
		Replace(offset, length, new StringTextSource(text), null);
	}

	public void Replace(int offset, int length, ITextSource text)
	{
		Replace(offset, length, text, null);
	}

	public void Replace(int offset, int length, string text, OffsetChangeMappingType offsetChangeMappingType)
	{
		Replace(offset, length, new StringTextSource(text), offsetChangeMappingType);
	}

	public void Replace(int offset, int length, ITextSource text, OffsetChangeMappingType offsetChangeMappingType)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		switch (offsetChangeMappingType)
		{
		case OffsetChangeMappingType.Normal:
			Replace(offset, length, text, null);
			break;
		case OffsetChangeMappingType.KeepAnchorBeforeInsertion:
			Replace(offset, length, text, OffsetChangeMap.FromSingleElement(new OffsetChangeMapEntry(offset, length, text.TextLength, removalNeverCausesAnchorDeletion: false, defaultAnchorMovementIsBeforeInsertion: true)));
			break;
		case OffsetChangeMappingType.RemoveAndInsert:
		{
			if (length == 0 || text.TextLength == 0)
			{
				Replace(offset, length, text, null);
				break;
			}
			OffsetChangeMap offsetChangeMap = new OffsetChangeMap(2);
			offsetChangeMap.Add(new OffsetChangeMapEntry(offset, length, 0));
			offsetChangeMap.Add(new OffsetChangeMapEntry(offset, 0, text.TextLength));
			offsetChangeMap.Freeze();
			Replace(offset, length, text, offsetChangeMap);
			break;
		}
		case OffsetChangeMappingType.CharacterReplace:
			if (length == 0 || text.TextLength == 0)
			{
				Replace(offset, length, text, null);
			}
			else if (text.TextLength > length)
			{
				OffsetChangeMapEntry entry = new OffsetChangeMapEntry(offset + length - 1, 1, 1 + text.TextLength - length);
				Replace(offset, length, text, OffsetChangeMap.FromSingleElement(entry));
			}
			else if (text.TextLength < length)
			{
				OffsetChangeMapEntry entry2 = new OffsetChangeMapEntry(offset + text.TextLength, length - text.TextLength, 0, removalNeverCausesAnchorDeletion: true, defaultAnchorMovementIsBeforeInsertion: false);
				Replace(offset, length, text, OffsetChangeMap.FromSingleElement(entry2));
			}
			else
			{
				Replace(offset, length, text, OffsetChangeMap.Empty);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("offsetChangeMappingType", offsetChangeMappingType, "Invalid enum value");
		}
	}

	public void Replace(int offset, int length, string text, OffsetChangeMap offsetChangeMap)
	{
		Replace(offset, length, new StringTextSource(text), offsetChangeMap);
	}

	public void Replace(int offset, int length, ITextSource text, OffsetChangeMap offsetChangeMap)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		text = text.CreateSnapshot();
		offsetChangeMap?.Freeze();
		BeginUpdate();
		try
		{
			inDocumentChanging = true;
			try
			{
				ThrowIfRangeInvalid(offset, length);
				DoReplace(offset, length, text, offsetChangeMap);
			}
			finally
			{
				inDocumentChanging = false;
			}
		}
		finally
		{
			EndUpdate();
		}
	}

	private void DoReplace(int offset, int length, ITextSource newText, OffsetChangeMap offsetChangeMap)
	{
		if (length == 0 && newText.TextLength == 0)
		{
			return;
		}
		if (length == 1 && newText.TextLength == 1 && offsetChangeMap == null)
		{
			offsetChangeMap = OffsetChangeMap.Empty;
		}
		ITextSource removedText = ((length == 0) ? StringTextSource.Empty : ((length >= 100) ? ((ITextSource)new RopeTextSource(rope.GetRange(offset, length))) : ((ITextSource)new StringTextSource(rope.ToString(offset, length)))));
		DocumentChangeEventArgs e = new DocumentChangeEventArgs(offset, removedText, newText, offsetChangeMap);
		if (this.Changing != null)
		{
			this.Changing(this, e);
		}
		if (this.textChanging != null)
		{
			this.textChanging(this, e);
		}
		undoStack.Push(this, e);
		cachedText = null;
		fireTextChanged = true;
		DelayedEvents delayedEvents = new DelayedEvents();
		lock (lockObject)
		{
			versionProvider.AppendChange(e);
			if (offset == 0 && length == rope.Length)
			{
				rope.Clear();
				if (newText is RopeTextSource ropeTextSource)
				{
					rope.InsertRange(0, ropeTextSource.GetRope());
				}
				else
				{
					rope.InsertText(0, newText.Text);
				}
				lineManager.Rebuild();
			}
			else
			{
				rope.RemoveRange(offset, length);
				lineManager.Remove(offset, length);
				if (newText is RopeTextSource ropeTextSource2)
				{
					rope.InsertRange(offset, ropeTextSource2.GetRope());
				}
				else
				{
					rope.InsertText(offset, newText.Text);
				}
				lineManager.Insert(offset, newText);
			}
		}
		if (offsetChangeMap == null)
		{
			anchorTree.HandleTextChange(e.CreateSingleChangeMapEntry(), delayedEvents);
		}
		else
		{
			foreach (OffsetChangeMapEntry item in offsetChangeMap)
			{
				anchorTree.HandleTextChange(item, delayedEvents);
			}
		}
		lineManager.ChangeComplete(e);
		delayedEvents.RaiseEvents();
		if (this.Changed != null)
		{
			this.Changed(this, e);
		}
		if (this.textChanged != null)
		{
			this.textChanged(this, e);
		}
	}

	public DocumentLine GetLineByNumber(int number)
	{
		VerifyAccess();
		if (number < 1 || number > lineTree.LineCount)
		{
			throw new ArgumentOutOfRangeException("number", number, "Value must be between 1 and " + lineTree.LineCount);
		}
		return lineTree.GetByNumber(number);
	}

	IDocumentLine IDocument.GetLineByNumber(int lineNumber)
	{
		return GetLineByNumber(lineNumber);
	}

	public DocumentLine GetLineByOffset(int offset)
	{
		VerifyAccess();
		if (offset < 0 || offset > rope.Length)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + rope.Length);
		}
		return lineTree.GetByOffset(offset);
	}

	IDocumentLine IDocument.GetLineByOffset(int offset)
	{
		return GetLineByOffset(offset);
	}

	public int GetOffset(TextLocation location)
	{
		return GetOffset(location.Line, location.Column);
	}

	public int GetOffset(int line, int column)
	{
		DocumentLine lineByNumber = GetLineByNumber(line);
		if (column <= 0)
		{
			return lineByNumber.Offset;
		}
		if (column > lineByNumber.Length)
		{
			return lineByNumber.EndOffset;
		}
		return lineByNumber.Offset + column - 1;
	}

	public TextLocation GetLocation(int offset)
	{
		DocumentLine lineByOffset = GetLineByOffset(offset);
		return new TextLocation(lineByOffset.LineNumber, offset - lineByOffset.Offset + 1);
	}

	public TextAnchor CreateAnchor(int offset)
	{
		VerifyAccess();
		if (offset < 0 || offset > rope.Length)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + rope.Length.ToString(CultureInfo.InvariantCulture));
		}
		return anchorTree.CreateAnchor(offset);
	}

	ITextAnchor IDocument.CreateAnchor(int offset)
	{
		return CreateAnchor(offset);
	}

	[Conditional("DEBUG")]
	internal void DebugVerifyAccess()
	{
		VerifyAccess();
	}

	internal string GetLineTreeAsString()
	{
		return "Not available in release build.";
	}

	internal string GetTextAnchorTreeAsString()
	{
		return "Not available in release build.";
	}

	object IServiceProvider.GetService(Type serviceType)
	{
		return ServiceProvider.GetService(serviceType);
	}

	private void OnFileNameChanged(EventArgs e)
	{
		this.FileNameChanged?.Invoke(this, e);
	}
}
