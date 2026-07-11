using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit;

[Localizability(LocalizationCategory.Text)]
[ContentProperty("Text")]
public class TextEditor : Control, ITextEditorComponent, IServiceProvider, IWeakEventListener
{
	public static readonly DependencyProperty DocumentProperty;

	public static readonly DependencyProperty OptionsProperty;

	private readonly TextArea textArea;

	private ScrollViewer scrollViewer;

	public static readonly DependencyProperty SyntaxHighlightingProperty;

	private IVisualLineTransformer colorizer;

	public static readonly DependencyProperty WordWrapProperty;

	public static readonly DependencyProperty IsReadOnlyProperty;

	public static readonly DependencyProperty IsModifiedProperty;

	public static readonly DependencyProperty ShowLineNumbersProperty;

	public static readonly DependencyProperty LineNumbersForegroundProperty;

	public static readonly DependencyProperty EncodingProperty;

	public static readonly RoutedEvent PreviewMouseHoverEvent;

	public static readonly RoutedEvent MouseHoverEvent;

	public static readonly RoutedEvent PreviewMouseHoverStoppedEvent;

	public static readonly RoutedEvent MouseHoverStoppedEvent;

	public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty;

	public static readonly DependencyProperty VerticalScrollBarVisibilityProperty;

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

	[Localizability(LocalizationCategory.Text)]
	[DefaultValue("")]
	public string Text
	{
		get
		{
			TextDocument document = Document;
			if (document == null)
			{
				return string.Empty;
			}
			return document.Text;
		}
		set
		{
			TextDocument document = GetDocument();
			document.Text = value ?? string.Empty;
			CaretOffset = 0;
			document.UndoStack.ClearAll();
		}
	}

	public TextArea TextArea => textArea;

	internal ScrollViewer ScrollViewer => scrollViewer;

	public IHighlightingDefinition SyntaxHighlighting
	{
		get
		{
			return (IHighlightingDefinition)GetValue(SyntaxHighlightingProperty);
		}
		set
		{
			SetValue(SyntaxHighlightingProperty, value);
		}
	}

	public bool WordWrap
	{
		get
		{
			return (bool)GetValue(WordWrapProperty);
		}
		set
		{
			SetValue(WordWrapProperty, Boxes.Box(value));
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return (bool)GetValue(IsReadOnlyProperty);
		}
		set
		{
			SetValue(IsReadOnlyProperty, Boxes.Box(value));
		}
	}

	public bool IsModified
	{
		get
		{
			return (bool)GetValue(IsModifiedProperty);
		}
		set
		{
			SetValue(IsModifiedProperty, Boxes.Box(value));
		}
	}

	public bool ShowLineNumbers
	{
		get
		{
			return (bool)GetValue(ShowLineNumbersProperty);
		}
		set
		{
			SetValue(ShowLineNumbersProperty, Boxes.Box(value));
		}
	}

	public Brush LineNumbersForeground
	{
		get
		{
			return (Brush)GetValue(LineNumbersForegroundProperty);
		}
		set
		{
			SetValue(LineNumbersForegroundProperty, value);
		}
	}

	public bool CanRedo => CanExecute(ApplicationCommands.Redo);

	public bool CanUndo => CanExecute(ApplicationCommands.Undo);

	public double ExtentHeight
	{
		get
		{
			if (scrollViewer == null)
			{
				return 0.0;
			}
			return scrollViewer.ExtentHeight;
		}
	}

	public double ExtentWidth
	{
		get
		{
			if (scrollViewer == null)
			{
				return 0.0;
			}
			return scrollViewer.ExtentWidth;
		}
	}

	public double ViewportHeight
	{
		get
		{
			if (scrollViewer == null)
			{
				return 0.0;
			}
			return scrollViewer.ViewportHeight;
		}
	}

	public double ViewportWidth
	{
		get
		{
			if (scrollViewer == null)
			{
				return 0.0;
			}
			return scrollViewer.ViewportWidth;
		}
	}

	public double VerticalOffset
	{
		get
		{
			if (scrollViewer == null)
			{
				return 0.0;
			}
			return scrollViewer.VerticalOffset;
		}
	}

	public double HorizontalOffset
	{
		get
		{
			if (scrollViewer == null)
			{
				return 0.0;
			}
			return scrollViewer.HorizontalOffset;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string SelectedText
	{
		get
		{
			if (textArea.Document != null && !textArea.Selection.IsEmpty)
			{
				return textArea.Document.GetText(textArea.Selection.SurroundingSegment);
			}
			return string.Empty;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (textArea.Document != null)
			{
				int selectionStart = SelectionStart;
				int selectionLength = SelectionLength;
				textArea.Document.Replace(selectionStart, selectionLength, value);
				textArea.Selection = Selection.Create(textArea, selectionStart, selectionStart + value.Length);
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int CaretOffset
	{
		get
		{
			return textArea.Caret.Offset;
		}
		set
		{
			textArea.Caret.Offset = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int SelectionStart
	{
		get
		{
			if (textArea.Selection.IsEmpty)
			{
				return textArea.Caret.Offset;
			}
			return textArea.Selection.SurroundingSegment.Offset;
		}
		set
		{
			Select(value, SelectionLength);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int SelectionLength
	{
		get
		{
			if (!textArea.Selection.IsEmpty)
			{
				return textArea.Selection.SurroundingSegment.Length;
			}
			return 0;
		}
		set
		{
			Select(SelectionStart, value);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int LineCount => Document?.LineCount ?? 1;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Encoding Encoding
	{
		get
		{
			return (Encoding)GetValue(EncodingProperty);
		}
		set
		{
			SetValue(EncodingProperty, value);
		}
	}

	public ScrollBarVisibility HorizontalScrollBarVisibility
	{
		get
		{
			return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
		}
		set
		{
			SetValue(HorizontalScrollBarVisibilityProperty, value);
		}
	}

	public ScrollBarVisibility VerticalScrollBarVisibility
	{
		get
		{
			return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
		}
		set
		{
			SetValue(VerticalScrollBarVisibilityProperty, value);
		}
	}

	public event EventHandler DocumentChanged;

	public event PropertyChangedEventHandler OptionChanged;

	public event EventHandler TextChanged;

	public event MouseEventHandler PreviewMouseHover
	{
		add
		{
			AddHandler(PreviewMouseHoverEvent, value);
		}
		remove
		{
			RemoveHandler(PreviewMouseHoverEvent, value);
		}
	}

	public event MouseEventHandler MouseHover
	{
		add
		{
			AddHandler(MouseHoverEvent, value);
		}
		remove
		{
			RemoveHandler(MouseHoverEvent, value);
		}
	}

	public event MouseEventHandler PreviewMouseHoverStopped
	{
		add
		{
			AddHandler(PreviewMouseHoverStoppedEvent, value);
		}
		remove
		{
			RemoveHandler(PreviewMouseHoverStoppedEvent, value);
		}
	}

	public event MouseEventHandler MouseHoverStopped
	{
		add
		{
			AddHandler(MouseHoverStoppedEvent, value);
		}
		remove
		{
			RemoveHandler(MouseHoverStoppedEvent, value);
		}
	}

	static TextEditor()
	{
		DocumentProperty = TextView.DocumentProperty.AddOwner(typeof(TextEditor), new FrameworkPropertyMetadata(OnDocumentChanged));
		OptionsProperty = TextView.OptionsProperty.AddOwner(typeof(TextEditor), new FrameworkPropertyMetadata(OnOptionsChanged));
		SyntaxHighlightingProperty = DependencyProperty.Register("SyntaxHighlighting", typeof(IHighlightingDefinition), typeof(TextEditor), new FrameworkPropertyMetadata(OnSyntaxHighlightingChanged));
		WordWrapProperty = DependencyProperty.Register("WordWrap", typeof(bool), typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.False));
		IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.False, OnIsReadOnlyChanged));
		IsModifiedProperty = DependencyProperty.Register("IsModified", typeof(bool), typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.False, OnIsModifiedChanged));
		ShowLineNumbersProperty = DependencyProperty.Register("ShowLineNumbers", typeof(bool), typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.False, OnShowLineNumbersChanged));
		LineNumbersForegroundProperty = DependencyProperty.Register("LineNumbersForeground", typeof(Brush), typeof(TextEditor), new FrameworkPropertyMetadata(Brushes.Gray, OnLineNumbersForegroundChanged));
		EncodingProperty = DependencyProperty.Register("Encoding", typeof(Encoding), typeof(TextEditor));
		PreviewMouseHoverEvent = TextView.PreviewMouseHoverEvent.AddOwner(typeof(TextEditor));
		MouseHoverEvent = TextView.MouseHoverEvent.AddOwner(typeof(TextEditor));
		PreviewMouseHoverStoppedEvent = TextView.PreviewMouseHoverStoppedEvent.AddOwner(typeof(TextEditor));
		MouseHoverStoppedEvent = TextView.MouseHoverStoppedEvent.AddOwner(typeof(TextEditor));
		HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty.AddOwner(typeof(TextEditor), new FrameworkPropertyMetadata(ScrollBarVisibility.Visible));
		VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty.AddOwner(typeof(TextEditor), new FrameworkPropertyMetadata(ScrollBarVisibility.Visible));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEditor), new FrameworkPropertyMetadata(typeof(TextEditor)));
		UIElement.FocusableProperty.OverrideMetadata(typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.True));
	}

	public TextEditor()
		: this(new TextArea())
	{
	}

	protected TextEditor(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		this.textArea = textArea;
		textArea.TextView.Services.AddService(typeof(TextEditor), this);
		SetCurrentValue(OptionsProperty, textArea.Options);
		SetCurrentValue(DocumentProperty, new TextDocument());
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new TextEditorAutomationPeer(this);
	}

	protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
	{
		base.OnGotKeyboardFocus(e);
		if (e.NewFocus == this)
		{
			Keyboard.Focus(textArea);
			e.Handled = true;
		}
	}

	protected virtual void OnDocumentChanged(EventArgs e)
	{
		if (this.DocumentChanged != null)
		{
			this.DocumentChanged(this, e);
		}
	}

	private static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
	{
		((TextEditor)dp).OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
	}

	private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
	{
		if (oldValue != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument>.RemoveListener(oldValue, this);
			PropertyChangedEventManager.RemoveListener(oldValue.UndoStack, this, "IsOriginalFile");
		}
		textArea.Document = newValue;
		if (newValue != null)
		{
			WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument>.AddListener(newValue, this);
			PropertyChangedEventManager.AddListener(newValue.UndoStack, this, "IsOriginalFile");
		}
		OnDocumentChanged(EventArgs.Empty);
		OnTextChanged(EventArgs.Empty);
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
		((TextEditor)dp).OnOptionsChanged((TextEditorOptions)e.OldValue, (TextEditorOptions)e.NewValue);
	}

	private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
	{
		if (oldValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.RemoveListener(oldValue, this);
		}
		textArea.Options = newValue;
		if (newValue != null)
		{
			WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.AddListener(newValue, this);
		}
		OnOptionChanged(new PropertyChangedEventArgs(null));
	}

	protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(PropertyChangedWeakEventManager))
		{
			OnOptionChanged((PropertyChangedEventArgs)e);
			return true;
		}
		if (managerType == typeof(TextDocumentWeakEventManager.TextChanged))
		{
			OnTextChanged(e);
			return true;
		}
		if (managerType == typeof(PropertyChangedEventManager))
		{
			return HandleIsOriginalChanged((PropertyChangedEventArgs)e);
		}
		return false;
	}

	bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		return ReceiveWeakEvent(managerType, sender, e);
	}

	private TextDocument GetDocument()
	{
		return Document ?? throw ThrowUtil.NoDocumentAssigned();
	}

	protected virtual void OnTextChanged(EventArgs e)
	{
		if (this.TextChanged != null)
		{
			this.TextChanged(this, e);
		}
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		scrollViewer = (ScrollViewer)base.Template.FindName("PART_ScrollViewer", this);
	}

	private bool CanExecute(RoutedUICommand command)
	{
		return command.CanExecute(null, textArea);
	}

	private void Execute(RoutedUICommand command)
	{
		command.Execute(null, textArea);
	}

	private static void OnSyntaxHighlightingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((TextEditor)d).OnSyntaxHighlightingChanged(e.NewValue as IHighlightingDefinition);
	}

	private void OnSyntaxHighlightingChanged(IHighlightingDefinition newValue)
	{
		if (colorizer != null)
		{
			textArea.TextView.LineTransformers.Remove(colorizer);
			colorizer = null;
		}
		if (newValue != null)
		{
			colorizer = CreateColorizer(newValue);
			if (colorizer != null)
			{
				textArea.TextView.LineTransformers.Insert(0, colorizer);
			}
		}
	}

	protected virtual IVisualLineTransformer CreateColorizer(IHighlightingDefinition highlightingDefinition)
	{
		if (highlightingDefinition == null)
		{
			throw new ArgumentNullException("highlightingDefinition");
		}
		return new HighlightingColorizer(highlightingDefinition);
	}

	private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEditor textEditor)
		{
			if ((bool)e.NewValue)
			{
				textEditor.TextArea.ReadOnlySectionProvider = ReadOnlySectionDocument.Instance;
			}
			else
			{
				textEditor.TextArea.ReadOnlySectionProvider = NoReadOnlySections.Instance;
			}
			if (UIElementAutomationPeer.FromElement(textEditor) is TextEditorAutomationPeer textEditorAutomationPeer)
			{
				textEditorAutomationPeer.RaiseIsReadOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
			}
		}
	}

	private static void OnIsModifiedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (!(d is TextEditor { Document: { UndoStack: var undoStack } }))
		{
			return;
		}
		if ((bool)e.NewValue)
		{
			if (undoStack.IsOriginalFile)
			{
				undoStack.DiscardOriginalFileMarker();
			}
		}
		else
		{
			undoStack.MarkAsOriginalFile();
		}
	}

	private bool HandleIsOriginalChanged(PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "IsOriginalFile")
		{
			TextDocument document = Document;
			if (document != null)
			{
				SetCurrentValue(IsModifiedProperty, Boxes.Box(!document.UndoStack.IsOriginalFile));
			}
			return true;
		}
		return false;
	}

	private static void OnShowLineNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		TextEditor textEditor = (TextEditor)d;
		ObservableCollection<UIElement> leftMargins = textEditor.TextArea.LeftMargins;
		if ((bool)e.NewValue)
		{
			LineNumberMargin lineNumberMargin = new LineNumberMargin();
			Line line = (Line)DottedLineMargin.Create();
			leftMargins.Insert(0, lineNumberMargin);
			leftMargins.Insert(1, line);
			Binding binding = new Binding("LineNumbersForeground")
			{
				Source = textEditor
			};
			line.SetBinding(Shape.StrokeProperty, binding);
			lineNumberMargin.SetBinding(Control.ForegroundProperty, binding);
			return;
		}
		for (int i = 0; i < leftMargins.Count; i++)
		{
			if (leftMargins[i] is LineNumberMargin)
			{
				leftMargins.RemoveAt(i);
				if (i < leftMargins.Count && DottedLineMargin.IsDottedLineMargin(leftMargins[i]))
				{
					leftMargins.RemoveAt(i);
				}
				break;
			}
		}
	}

	private static void OnLineNumbersForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (((TextEditor)d).TextArea.LeftMargins.FirstOrDefault((UIElement margin) => margin is LineNumberMargin) is LineNumberMargin lineNumberMargin)
		{
			lineNumberMargin.SetValue(Control.ForegroundProperty, e.NewValue);
		}
	}

	public void AppendText(string textData)
	{
		TextDocument document = GetDocument();
		document.Insert(document.TextLength, textData);
	}

	public void BeginChange()
	{
		GetDocument().BeginUpdate();
	}

	public void Copy()
	{
		Execute(ApplicationCommands.Copy);
	}

	public void Cut()
	{
		Execute(ApplicationCommands.Cut);
	}

	public IDisposable DeclareChangeBlock()
	{
		return GetDocument().RunUpdate();
	}

	public void Delete()
	{
		Execute(ApplicationCommands.Delete);
	}

	public void EndChange()
	{
		GetDocument().EndUpdate();
	}

	public void LineDown()
	{
		if (scrollViewer != null)
		{
			scrollViewer.LineDown();
		}
	}

	public void LineLeft()
	{
		if (scrollViewer != null)
		{
			scrollViewer.LineLeft();
		}
	}

	public void LineRight()
	{
		if (scrollViewer != null)
		{
			scrollViewer.LineRight();
		}
	}

	public void LineUp()
	{
		if (scrollViewer != null)
		{
			scrollViewer.LineUp();
		}
	}

	public void PageDown()
	{
		if (scrollViewer != null)
		{
			scrollViewer.PageDown();
		}
	}

	public void PageUp()
	{
		if (scrollViewer != null)
		{
			scrollViewer.PageUp();
		}
	}

	public void PageLeft()
	{
		if (scrollViewer != null)
		{
			scrollViewer.PageLeft();
		}
	}

	public void PageRight()
	{
		if (scrollViewer != null)
		{
			scrollViewer.PageRight();
		}
	}

	public void Paste()
	{
		Execute(ApplicationCommands.Paste);
	}

	public bool Redo()
	{
		if (CanExecute(ApplicationCommands.Redo))
		{
			Execute(ApplicationCommands.Redo);
			return true;
		}
		return false;
	}

	public void ScrollToEnd()
	{
		ApplyTemplate();
		if (scrollViewer != null)
		{
			scrollViewer.ScrollToEnd();
		}
	}

	public void ScrollToHome()
	{
		ApplyTemplate();
		if (scrollViewer != null)
		{
			scrollViewer.ScrollToHome();
		}
	}

	public void ScrollToHorizontalOffset(double offset)
	{
		ApplyTemplate();
		if (scrollViewer != null)
		{
			scrollViewer.ScrollToHorizontalOffset(offset);
		}
	}

	public void ScrollToVerticalOffset(double offset)
	{
		ApplyTemplate();
		if (scrollViewer != null)
		{
			scrollViewer.ScrollToVerticalOffset(offset);
		}
	}

	public void SelectAll()
	{
		Execute(ApplicationCommands.SelectAll);
	}

	public bool Undo()
	{
		if (CanExecute(ApplicationCommands.Undo))
		{
			Execute(ApplicationCommands.Undo);
			return true;
		}
		return false;
	}

	public void Select(int start, int length)
	{
		int num = ((Document != null) ? Document.TextLength : 0);
		if (start < 0 || start > num)
		{
			throw new ArgumentOutOfRangeException("start", start, "Value must be between 0 and " + num);
		}
		if (length < 0 || start + length > num)
		{
			throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + (num - start));
		}
		textArea.Selection = Selection.Create(textArea, start, start + length);
		textArea.Caret.Offset = start + length;
	}

	public void Clear()
	{
		Text = string.Empty;
	}

	public void Load(Stream stream)
	{
		using (StreamReader streamReader = FileReader.OpenStream(stream, Encoding ?? Encoding.UTF8))
		{
			Text = streamReader.ReadToEnd();
			SetCurrentValue(EncodingProperty, streamReader.CurrentEncoding);
		}
		SetCurrentValue(IsModifiedProperty, Boxes.False);
	}

	public void Load(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
		Load(stream);
	}

	public void Save(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		Encoding encoding = Encoding;
		TextDocument document = Document;
		StreamWriter streamWriter = ((encoding != null) ? new StreamWriter(stream, encoding) : new StreamWriter(stream));
		document?.WriteTextTo(streamWriter);
		streamWriter.Flush();
		SetCurrentValue(IsModifiedProperty, Boxes.False);
	}

	public void Save(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		using FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
		Save(stream);
	}

	object IServiceProvider.GetService(Type serviceType)
	{
		return textArea.GetService(serviceType);
	}

	public TextViewPosition? GetPositionFromPoint(Point point)
	{
		if (Document == null)
		{
			return null;
		}
		TextView textView = TextArea.TextView;
		return textView.GetPosition(TranslatePoint(point, textView) + textView.ScrollOffset);
	}

	public void ScrollToLine(int line)
	{
		ScrollTo(line, -1);
	}

	public void ScrollTo(int line, int column)
	{
		ScrollTo(line, column, VisualYPosition.LineMiddle, (scrollViewer != null) ? (scrollViewer.ViewportHeight / 2.0) : 0.0, 0.3);
	}

	public void ScrollTo(int line, int column, VisualYPosition yPositionMode, double referencedVerticalViewPortOffset, double minimumScrollFraction)
	{
		TextView textView = textArea.TextView;
		TextDocument document = textView.Document;
		if (scrollViewer == null || document == null)
		{
			return;
		}
		if (line < 1)
		{
			line = 1;
		}
		if (line > document.LineCount)
		{
			line = document.LineCount;
		}
		if (!((IScrollInfo)textView).CanHorizontallyScroll)
		{
			VisualLine orConstructVisualLine = textView.GetOrConstructVisualLine(document.GetLineByNumber(line));
			for (double num = referencedVerticalViewPortOffset; num > 0.0; num -= orConstructVisualLine.Height)
			{
				DocumentLine previousLine = orConstructVisualLine.FirstDocumentLine.PreviousLine;
				if (previousLine == null)
				{
					break;
				}
				orConstructVisualLine = textView.GetOrConstructVisualLine(previousLine);
			}
		}
		Point visualPosition = textArea.TextView.GetVisualPosition(new TextViewPosition(line, Math.Max(1, column)), yPositionMode);
		double num2 = visualPosition.Y - referencedVerticalViewPortOffset;
		if (Math.Abs(num2 - scrollViewer.VerticalOffset) > minimumScrollFraction * scrollViewer.ViewportHeight)
		{
			scrollViewer.ScrollToVerticalOffset(Math.Max(0.0, num2));
		}
		if (column <= 0)
		{
			return;
		}
		if (visualPosition.X > scrollViewer.ViewportWidth - 60.0)
		{
			double num3 = Math.Max(0.0, visualPosition.X - scrollViewer.ViewportWidth / 2.0);
			if (Math.Abs(num3 - scrollViewer.HorizontalOffset) > minimumScrollFraction * scrollViewer.ViewportWidth)
			{
				scrollViewer.ScrollToHorizontalOffset(num3);
			}
		}
		else
		{
			scrollViewer.ScrollToHorizontalOffset(0.0);
		}
	}
}
