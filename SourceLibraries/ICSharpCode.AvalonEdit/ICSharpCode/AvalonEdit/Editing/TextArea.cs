using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

public class TextArea : System.Windows.Controls.Control, IScrollInfo, IWeakEventListener, ITextEditorComponent, IServiceProvider
{
	private sealed class RestoreCaretAndSelectionUndoAction : IUndoableOperation
	{
		private WeakReference textAreaReference;

		private TextViewPosition caretPosition;

		private Selection selection;

		public RestoreCaretAndSelectionUndoAction(TextArea textArea)
		{
			textAreaReference = new WeakReference(textArea);
			caretPosition = textArea.Caret.NonValidatedPosition;
			selection = textArea.Selection;
		}

		public void Undo()
		{
			TextArea textArea = (TextArea)textAreaReference.Target;
			if (textArea != null)
			{
				textArea.Caret.Position = caretPosition;
				textArea.Selection = selection;
			}
		}

		public void Redo()
		{
			Undo();
		}
	}

	internal readonly ImeSupport ime;

	private ITextAreaInputHandler activeInputHandler;

	private bool isChangingInputHandler;

	private ImmutableStack<TextAreaStackedInputHandler> stackedInputHandlers = ImmutableStack<TextAreaStackedInputHandler>.Empty;

	public static readonly DependencyProperty DocumentProperty;

	public static readonly DependencyProperty OptionsProperty;

	private readonly TextView textView;

	private IScrollInfo scrollInfo;

	internal readonly Selection emptySelection;

	private Selection selection;

	public static readonly DependencyProperty SelectionBrushProperty;

	public static readonly DependencyProperty SelectionForegroundProperty;

	public static readonly DependencyProperty SelectionBorderProperty;

	public static readonly DependencyProperty SelectionCornerRadiusProperty;

	private bool ensureSelectionValidRequested;

	private int allowCaretOutsideSelection;

	private readonly Caret caret;

	private ObservableCollection<UIElement> leftMargins = new ObservableCollection<UIElement>();

	private IReadOnlySectionProvider readOnlySectionProvider = NoReadOnlySections.Instance;

	private ScrollViewer scrollOwner;

	private bool canVerticallyScroll;

	private bool canHorizontallyScroll;

	public static readonly DependencyProperty IndentationStrategyProperty;

	private bool isMouseCursorHidden;

	public static readonly DependencyProperty OverstrikeModeProperty;

	public TextAreaDefaultInputHandler DefaultInputHandler { get; private set; }

	public ITextAreaInputHandler ActiveInputHandler
	{
		get
		{
			return activeInputHandler;
		}
		set
		{
			if (value != null && value.TextArea != this)
			{
				throw new ArgumentException("The input handler was created for a different text area than this one.");
			}
			if (isChangingInputHandler)
			{
				throw new InvalidOperationException("Cannot set ActiveInputHandler recursively");
			}
			if (activeInputHandler == value)
			{
				return;
			}
			isChangingInputHandler = true;
			try
			{
				PopStackedInputHandler(stackedInputHandlers.LastOrDefault());
				if (activeInputHandler != null)
				{
					activeInputHandler.Detach();
				}
				activeInputHandler = value;
				value?.Attach();
			}
			finally
			{
				isChangingInputHandler = false;
			}
			if (this.ActiveInputHandlerChanged != null)
			{
				this.ActiveInputHandlerChanged(this, EventArgs.Empty);
			}
		}
	}

	public ImmutableStack<TextAreaStackedInputHandler> StackedInputHandlers => stackedInputHandlers;

	public TextDocument Document
	{
		get
		{
			return (TextDocument)GetValue(DocumentProperty);
		}
		set
		{
			SetValue(DocumentProperty, value);
		}
	}

	public TextEditorOptions Options
	{
		get
		{
			return (TextEditorOptions)GetValue(OptionsProperty);
		}
		set
		{
			SetValue(OptionsProperty, value);
		}
	}

	public TextView TextView => textView;

	public Selection Selection
	{
		get
		{
			return selection;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.textArea != this)
			{
				throw new ArgumentException("Cannot use a Selection instance that belongs to another text area.");
			}
			if (object.Equals(selection, value))
			{
				return;
			}
			if (textView != null)
			{
				ISegment surroundingSegment = selection.SurroundingSegment;
				ISegment surroundingSegment2 = value.SurroundingSegment;
				if (!Selection.EnableVirtualSpace && selection is SimpleSelection && value is SimpleSelection && surroundingSegment != null && surroundingSegment2 != null)
				{
					int offset = surroundingSegment.Offset;
					int offset2 = surroundingSegment2.Offset;
					if (offset != offset2)
					{
						textView.Redraw(Math.Min(offset, offset2), Math.Abs(offset - offset2), DispatcherPriority.Render);
					}
					int endOffset = surroundingSegment.EndOffset;
					int endOffset2 = surroundingSegment2.EndOffset;
					if (endOffset != endOffset2)
					{
						textView.Redraw(Math.Min(endOffset, endOffset2), Math.Abs(endOffset - endOffset2), DispatcherPriority.Render);
					}
				}
				else
				{
					textView.Redraw(surroundingSegment, DispatcherPriority.Render);
					textView.Redraw(surroundingSegment2, DispatcherPriority.Render);
				}
			}
			selection = value;
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, EventArgs.Empty);
			}
			CommandManager.InvalidateRequerySuggested();
		}
	}

	public Brush SelectionBrush
	{
		get
		{
			return (Brush)GetValue(SelectionBrushProperty);
		}
		set
		{
			SetValue(SelectionBrushProperty, value);
		}
	}

	public Brush SelectionForeground
	{
		get
		{
			return (Brush)GetValue(SelectionForegroundProperty);
		}
		set
		{
			SetValue(SelectionForegroundProperty, value);
		}
	}

	public Pen SelectionBorder
	{
		get
		{
			return (Pen)GetValue(SelectionBorderProperty);
		}
		set
		{
			SetValue(SelectionBorderProperty, value);
		}
	}

	public double SelectionCornerRadius
	{
		get
		{
			return (double)GetValue(SelectionCornerRadiusProperty);
		}
		set
		{
			SetValue(SelectionCornerRadiusProperty, value);
		}
	}

	public MouseSelectionMode MouseSelectionMode
	{
		get
		{
			if (DefaultInputHandler.MouseSelection is SelectionMouseHandler selectionMouseHandler)
			{
				return selectionMouseHandler.MouseSelectionMode;
			}
			return MouseSelectionMode.None;
		}
		set
		{
			if (DefaultInputHandler.MouseSelection is SelectionMouseHandler selectionMouseHandler)
			{
				selectionMouseHandler.MouseSelectionMode = value;
			}
		}
	}

	public Caret Caret => caret;

	public ObservableCollection<UIElement> LeftMargins => leftMargins;

	public IReadOnlySectionProvider ReadOnlySectionProvider
	{
		get
		{
			return readOnlySectionProvider;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			readOnlySectionProvider = value;
			CommandManager.InvalidateRequerySuggested();
		}
	}

	bool IScrollInfo.CanVerticallyScroll
	{
		get
		{
			if (scrollInfo == null)
			{
				return false;
			}
			return scrollInfo.CanVerticallyScroll;
		}
		set
		{
			canVerticallyScroll = value;
			if (scrollInfo != null)
			{
				scrollInfo.CanVerticallyScroll = value;
			}
		}
	}

	bool IScrollInfo.CanHorizontallyScroll
	{
		get
		{
			if (scrollInfo == null)
			{
				return false;
			}
			return scrollInfo.CanHorizontallyScroll;
		}
		set
		{
			canHorizontallyScroll = value;
			if (scrollInfo != null)
			{
				scrollInfo.CanHorizontallyScroll = value;
			}
		}
	}

	double IScrollInfo.ExtentWidth
	{
		get
		{
			if (scrollInfo == null)
			{
				return 0.0;
			}
			return scrollInfo.ExtentWidth;
		}
	}

	double IScrollInfo.ExtentHeight
	{
		get
		{
			if (scrollInfo == null)
			{
				return 0.0;
			}
			return scrollInfo.ExtentHeight;
		}
	}

	double IScrollInfo.ViewportWidth
	{
		get
		{
			if (scrollInfo == null)
			{
				return 0.0;
			}
			return scrollInfo.ViewportWidth;
		}
	}

	double IScrollInfo.ViewportHeight
	{
		get
		{
			if (scrollInfo == null)
			{
				return 0.0;
			}
			return scrollInfo.ViewportHeight;
		}
	}

	double IScrollInfo.HorizontalOffset
	{
		get
		{
			if (scrollInfo == null)
			{
				return 0.0;
			}
			return scrollInfo.HorizontalOffset;
		}
	}

	double IScrollInfo.VerticalOffset
	{
		get
		{
			if (scrollInfo == null)
			{
				return 0.0;
			}
			return scrollInfo.VerticalOffset;
		}
	}

	ScrollViewer IScrollInfo.ScrollOwner
	{
		get
		{
			if (scrollInfo == null)
			{
				return null;
			}
			return scrollInfo.ScrollOwner;
		}
		set
		{
			if (scrollInfo != null)
			{
				scrollInfo.ScrollOwner = value;
			}
			else
			{
				scrollOwner = value;
			}
		}
	}

	public IIndentationStrategy IndentationStrategy
	{
		get
		{
			return (IIndentationStrategy)GetValue(IndentationStrategyProperty);
		}
		set
		{
			SetValue(IndentationStrategyProperty, value);
		}
	}

	public bool OverstrikeMode
	{
		get
		{
			return (bool)GetValue(OverstrikeModeProperty);
		}
		set
		{
			SetValue(OverstrikeModeProperty, value);
		}
	}

	public event EventHandler ActiveInputHandlerChanged;

	public event EventHandler DocumentChanged;

	public event PropertyChangedEventHandler OptionChanged;

	public event EventHandler SelectionChanged;

	public event TextCompositionEventHandler TextEntering;

	public event TextCompositionEventHandler TextEntered;

	public event EventHandler<TextEventArgs> TextCopied;

	static TextArea()
	{
		DocumentProperty = TextView.DocumentProperty.AddOwner(typeof(TextArea), new FrameworkPropertyMetadata(OnDocumentChanged));
		OptionsProperty = TextView.OptionsProperty.AddOwner(typeof(TextArea), new FrameworkPropertyMetadata(OnOptionsChanged));
		SelectionBrushProperty = DependencyProperty.Register("SelectionBrush", typeof(Brush), typeof(TextArea));
		SelectionForegroundProperty = DependencyProperty.Register("SelectionForeground", typeof(Brush), typeof(TextArea));
		SelectionBorderProperty = DependencyProperty.Register("SelectionBorder", typeof(Pen), typeof(TextArea));
		SelectionCornerRadiusProperty = DependencyProperty.Register("SelectionCornerRadius", typeof(double), typeof(TextArea), new FrameworkPropertyMetadata(3.0));
		IndentationStrategyProperty = DependencyProperty.Register("IndentationStrategy", typeof(IIndentationStrategy), typeof(TextArea), new FrameworkPropertyMetadata(new DefaultIndentationStrategy()));
		OverstrikeModeProperty = DependencyProperty.Register("OverstrikeMode", typeof(bool), typeof(TextArea), new FrameworkPropertyMetadata(Boxes.False));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TextArea), new FrameworkPropertyMetadata(typeof(TextArea)));
		KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(TextArea), new FrameworkPropertyMetadata(Boxes.True));
		KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TextArea), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
		UIElement.FocusableProperty.OverrideMetadata(typeof(TextArea), new FrameworkPropertyMetadata(Boxes.True));
	}

	public TextArea()
		: this(new TextView())
	{
	}

	protected TextArea(TextView textView)
	{
		if (textView == null)
		{
			throw new ArgumentNullException("textView");
		}
		this.textView = textView;
		Options = textView.Options;
		selection = (emptySelection = new EmptySelection(this));
		textView.Services.AddService(typeof(TextArea), this);
		textView.LineTransformers.Add(new SelectionColorizer(this));
		textView.InsertLayer(new SelectionLayer(this), KnownLayer.Selection, LayerInsertionPosition.Replace);
		caret = new Caret(this);
		caret.PositionChanged += delegate
		{
			RequestSelectionValidation();
		};
		caret.PositionChanged += CaretPositionChanged;
		AttachTypingEvents();
		ime = new ImeSupport(this);
		leftMargins.CollectionChanged += leftMargins_CollectionChanged;
		DefaultInputHandler = new TextAreaDefaultInputHandler(this);
		ActiveInputHandler = DefaultInputHandler;
	}

	public void PushStackedInputHandler(TextAreaStackedInputHandler inputHandler)
	{
		if (inputHandler == null)
		{
			throw new ArgumentNullException("inputHandler");
		}
		stackedInputHandlers = stackedInputHandlers.Push(inputHandler);
		inputHandler.Attach();
	}

	public void PopStackedInputHandler(TextAreaStackedInputHandler inputHandler)
	{
		if (stackedInputHandlers.Any((TextAreaStackedInputHandler i) => i == inputHandler))
		{
			ITextAreaInputHandler textAreaInputHandler;
			do
			{
				textAreaInputHandler = stackedInputHandlers.Peek();
				stackedInputHandlers = stackedInputHandlers.Pop();
				textAreaInputHandler.Detach();
			}
			while (textAreaInputHandler != inputHandler);
		}
	}

	private static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
	{
		((TextArea)dp).OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
	}

	private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
	{
		if (oldValue != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument>.RemoveListener(oldValue, this);
			WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument>.RemoveListener(oldValue, this);
			WeakEventManagerBase<TextDocumentWeakEventManager.UpdateStarted, TextDocument>.RemoveListener(oldValue, this);
			WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument>.RemoveListener(oldValue, this);
		}
		textView.Document = newValue;
		if (newValue != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument>.AddListener(newValue, this);
			WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument>.AddListener(newValue, this);
			WeakEventManagerBase<TextDocumentWeakEventManager.UpdateStarted, TextDocument>.AddListener(newValue, this);
			WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument>.AddListener(newValue, this);
		}
		caret.Location = new TextLocation(1, 1);
		ClearSelection();
		if (this.DocumentChanged != null)
		{
			this.DocumentChanged(this, EventArgs.Empty);
		}
		CommandManager.InvalidateRequerySuggested();
	}

	protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
	{
		if (this.OptionChanged != null)
		{
			this.OptionChanged(this, e);
		}
	}

	private static void OnOptionsChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
	{
		((TextArea)dp).OnOptionsChanged((TextEditorOptions)e.OldValue, (TextEditorOptions)e.NewValue);
	}

	private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
	{
		if (oldValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.RemoveListener(oldValue, this);
		}
		textView.Options = newValue;
		if (newValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.AddListener(newValue, this);
		}
		OnOptionChanged(new PropertyChangedEventArgs(null));
	}

	protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(TextDocumentWeakEventManager.Changing))
		{
			OnDocumentChanging();
			return true;
		}
		if (managerType == typeof(TextDocumentWeakEventManager.Changed))
		{
			OnDocumentChanged((DocumentChangeEventArgs)e);
			return true;
		}
		if (managerType == typeof(TextDocumentWeakEventManager.UpdateStarted))
		{
			OnUpdateStarted();
			return true;
		}
		if (managerType == typeof(TextDocumentWeakEventManager.UpdateFinished))
		{
			OnUpdateFinished();
			return true;
		}
		if (managerType == typeof(PropertyChangedWeakEventManager))
		{
			OnOptionChanged((PropertyChangedEventArgs)e);
			return true;
		}
		return false;
	}

	bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		return ReceiveWeakEvent(managerType, sender, e);
	}

	private void OnDocumentChanging()
	{
		caret.OnDocumentChanging();
	}

	private void OnDocumentChanged(DocumentChangeEventArgs e)
	{
		caret.OnDocumentChanged(e);
		Selection = selection.UpdateOnDocumentChange(e);
	}

	private void OnUpdateStarted()
	{
		Document.UndoStack.PushOptional(new RestoreCaretAndSelectionUndoAction(this));
	}

	private void OnUpdateFinished()
	{
		caret.OnDocumentUpdateFinished();
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		scrollInfo = textView;
		ApplyScrollInfo();
	}

	public void ClearSelection()
	{
		Selection = emptySelection;
	}

	private void RequestSelectionValidation()
	{
		if (!ensureSelectionValidRequested && allowCaretOutsideSelection == 0)
		{
			ensureSelectionValidRequested = true;
			base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(EnsureSelectionValid));
		}
	}

	private void EnsureSelectionValid()
	{
		ensureSelectionValidRequested = false;
		if (allowCaretOutsideSelection == 0 && !selection.IsEmpty && !selection.Contains(caret.Offset))
		{
			ClearSelection();
		}
	}

	public IDisposable AllowCaretOutsideSelection()
	{
		VerifyAccess();
		allowCaretOutsideSelection++;
		return new CallbackOnDispose(delegate
		{
			VerifyAccess();
			allowCaretOutsideSelection--;
			RequestSelectionValidation();
		});
	}

	private void CaretPositionChanged(object sender, EventArgs e)
	{
		if (textView != null)
		{
			textView.HighlightedLine = Caret.Line;
		}
	}

	private void leftMargins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems != null)
		{
			foreach (ITextViewConnect item in e.OldItems.OfType<ITextViewConnect>())
			{
				item.RemoveFromTextView(textView);
			}
		}
		if (e.NewItems == null)
		{
			return;
		}
		foreach (ITextViewConnect item2 in e.NewItems.OfType<ITextViewConnect>())
		{
			item2.AddToTextView(textView);
		}
	}

	private void ApplyScrollInfo()
	{
		if (scrollInfo != null)
		{
			scrollInfo.ScrollOwner = scrollOwner;
			scrollInfo.CanVerticallyScroll = canVerticallyScroll;
			scrollInfo.CanHorizontallyScroll = canHorizontallyScroll;
			scrollOwner = null;
		}
	}

	void IScrollInfo.LineUp()
	{
		if (scrollInfo != null)
		{
			scrollInfo.LineUp();
		}
	}

	void IScrollInfo.LineDown()
	{
		if (scrollInfo != null)
		{
			scrollInfo.LineDown();
		}
	}

	void IScrollInfo.LineLeft()
	{
		if (scrollInfo != null)
		{
			scrollInfo.LineLeft();
		}
	}

	void IScrollInfo.LineRight()
	{
		if (scrollInfo != null)
		{
			scrollInfo.LineRight();
		}
	}

	void IScrollInfo.PageUp()
	{
		if (scrollInfo != null)
		{
			scrollInfo.PageUp();
		}
	}

	void IScrollInfo.PageDown()
	{
		if (scrollInfo != null)
		{
			scrollInfo.PageDown();
		}
	}

	void IScrollInfo.PageLeft()
	{
		if (scrollInfo != null)
		{
			scrollInfo.PageLeft();
		}
	}

	void IScrollInfo.PageRight()
	{
		if (scrollInfo != null)
		{
			scrollInfo.PageRight();
		}
	}

	void IScrollInfo.MouseWheelUp()
	{
		if (scrollInfo != null)
		{
			scrollInfo.MouseWheelUp();
		}
	}

	void IScrollInfo.MouseWheelDown()
	{
		if (scrollInfo != null)
		{
			scrollInfo.MouseWheelDown();
		}
	}

	void IScrollInfo.MouseWheelLeft()
	{
		if (scrollInfo != null)
		{
			scrollInfo.MouseWheelLeft();
		}
	}

	void IScrollInfo.MouseWheelRight()
	{
		if (scrollInfo != null)
		{
			scrollInfo.MouseWheelRight();
		}
	}

	void IScrollInfo.SetHorizontalOffset(double offset)
	{
		if (scrollInfo != null)
		{
			scrollInfo.SetHorizontalOffset(offset);
		}
	}

	void IScrollInfo.SetVerticalOffset(double offset)
	{
		if (scrollInfo != null)
		{
			scrollInfo.SetVerticalOffset(offset);
		}
	}

	Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
	{
		if (scrollInfo != null)
		{
			return scrollInfo.MakeVisible(visual, rectangle);
		}
		return Rect.Empty;
	}

	protected override void OnMouseDown(MouseButtonEventArgs e)
	{
		base.OnMouseDown(e);
		Focus();
	}

	protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
	{
		base.OnGotKeyboardFocus(e);
		ime.OnGotKeyboardFocus(e);
		caret.Show();
	}

	protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
	{
		base.OnLostKeyboardFocus(e);
		caret.Hide();
		ime.OnLostKeyboardFocus(e);
	}

	protected virtual void OnTextEntering(TextCompositionEventArgs e)
	{
		if (this.TextEntering != null)
		{
			this.TextEntering(this, e);
		}
	}

	protected virtual void OnTextEntered(TextCompositionEventArgs e)
	{
		if (this.TextEntered != null)
		{
			this.TextEntered(this, e);
		}
	}

	protected override void OnTextInput(TextCompositionEventArgs e)
	{
		base.OnTextInput(e);
		if (!e.Handled && Document != null && !string.IsNullOrEmpty(e.Text) && !(e.Text == "\u001b") && !(e.Text == "\b"))
		{
			HideMouseCursor();
			PerformTextInput(e);
			e.Handled = true;
		}
	}

	public void PerformTextInput(string text)
	{
		TextComposition composition = new TextComposition(InputManager.Current, this, text);
		TextCompositionEventArgs e = new TextCompositionEventArgs(Keyboard.PrimaryDevice, composition);
		e.RoutedEvent = UIElement.TextInputEvent;
		PerformTextInput(e);
	}

	public void PerformTextInput(TextCompositionEventArgs e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		OnTextEntering(e);
		if (e.Handled)
		{
			return;
		}
		if (e.Text == "\n" || e.Text == "\r" || e.Text == "\r\n")
		{
			ReplaceSelectionWithNewLine();
		}
		else
		{
			if (OverstrikeMode && Selection.IsEmpty && Document.GetLineByNumber(Caret.Line).EndOffset > Caret.Offset)
			{
				EditingCommands.SelectRightByCharacter.Execute(null, this);
			}
			ReplaceSelectionWithText(e.Text);
		}
		OnTextEntered(e);
		caret.BringCaretToView();
	}

	private void ReplaceSelectionWithNewLine()
	{
		string newLineFromDocument = TextUtilities.GetNewLineFromDocument(Document, Caret.Line);
		using (Document.RunUpdate())
		{
			ReplaceSelectionWithText(newLineFromDocument);
			if (IndentationStrategy != null)
			{
				DocumentLine lineByNumber = Document.GetLineByNumber(Caret.Line);
				ISegment[] deletableSegments = GetDeletableSegments(lineByNumber);
				if (deletableSegments.Length == 1 && deletableSegments[0].Offset == lineByNumber.Offset && deletableSegments[0].Length == lineByNumber.Length)
				{
					IndentationStrategy.IndentLine(Document, lineByNumber);
				}
			}
		}
	}

	internal void RemoveSelectedText()
	{
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		selection.ReplaceSelectionWithText(string.Empty);
	}

	internal void ReplaceSelectionWithText(string newText)
	{
		if (newText == null)
		{
			throw new ArgumentNullException("newText");
		}
		if (Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		selection.ReplaceSelectionWithText(newText);
	}

	internal ISegment[] GetDeletableSegments(ISegment segment)
	{
		ISegment[] array = (ReadOnlySectionProvider.GetDeletableSegments(segment) ?? throw new InvalidOperationException("ReadOnlySectionProvider.GetDeletableSegments returned null")).ToArray();
		int num = segment.Offset;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Offset < num)
			{
				throw new InvalidOperationException("ReadOnlySectionProvider returned incorrect segments (outside of input segment / wrong order)");
			}
			num = array[i].EndOffset;
		}
		if (num > segment.EndOffset)
		{
			throw new InvalidOperationException("ReadOnlySectionProvider returned incorrect segments (outside of input segment / wrong order)");
		}
		return array;
	}

	protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
	{
		base.OnPreviewKeyDown(e);
		foreach (TextAreaStackedInputHandler stackedInputHandler in stackedInputHandlers)
		{
			if (e.Handled)
			{
				break;
			}
			stackedInputHandler.OnPreviewKeyDown(e);
		}
	}

	protected override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs e)
	{
		base.OnPreviewKeyUp(e);
		foreach (TextAreaStackedInputHandler stackedInputHandler in stackedInputHandlers)
		{
			if (e.Handled)
			{
				break;
			}
			stackedInputHandler.OnPreviewKeyUp(e);
		}
	}

	protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
	{
		base.OnKeyDown(e);
		TextView.InvalidateCursorIfMouseWithinTextView();
	}

	protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
	{
		base.OnKeyUp(e);
		TextView.InvalidateCursorIfMouseWithinTextView();
	}

	private void AttachTypingEvents()
	{
		base.MouseEnter += delegate
		{
			ShowMouseCursor();
		};
		base.MouseLeave += delegate
		{
			ShowMouseCursor();
		};
		base.PreviewMouseMove += delegate
		{
			ShowMouseCursor();
		};
		base.TouchEnter += delegate
		{
			ShowMouseCursor();
		};
		base.TouchLeave += delegate
		{
			ShowMouseCursor();
		};
		base.PreviewTouchMove += delegate
		{
			ShowMouseCursor();
		};
	}

	private void ShowMouseCursor()
	{
		if (isMouseCursorHidden)
		{
			System.Windows.Forms.Cursor.Show();
			isMouseCursorHidden = false;
		}
	}

	private void HideMouseCursor()
	{
		if (Options.HideCursorWhileTyping && !isMouseCursorHidden && base.IsMouseOver)
		{
			isMouseCursorHidden = true;
			System.Windows.Forms.Cursor.Hide();
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new TextAreaAutomationPeer(this);
	}

	protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
	{
		return new PointHitTestResult(this, hitTestParameters.HitPoint);
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);
		if (e.Property == SelectionBrushProperty || e.Property == SelectionBorderProperty || e.Property == SelectionForegroundProperty || e.Property == SelectionCornerRadiusProperty)
		{
			textView.Redraw();
		}
		else if (e.Property == OverstrikeModeProperty)
		{
			caret.UpdateIfVisible();
		}
	}

	public virtual object GetService(Type serviceType)
	{
		return textView.GetService(serviceType);
	}

	protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
	{
		base.OnPreviewMouseUp(e);
		if (TextView != null)
		{
			TextView.PreviewUp(e);
		}
	}

	internal void OnTextCopied(TextEventArgs e)
	{
		if (this.TextCopied != null)
		{
			this.TextCopied(this, e);
		}
	}
}
