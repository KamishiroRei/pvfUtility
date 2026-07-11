using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class CompletionWindowBase : Window
{
	private sealed class InputHandler : TextAreaStackedInputHandler
	{
		internal readonly CompletionWindowBase window;

		private const Key KeyDeadCharProcessed = Key.DeadCharProcessed;

		public InputHandler(CompletionWindowBase window)
			: base(window.TextArea)
		{
			this.window = window;
		}

		public override void Detach()
		{
			base.Detach();
			window.Close();
		}

		public override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key != Key.DeadCharProcessed)
			{
				e.Handled = RaiseEventPair(window, UIElement.PreviewKeyDownEvent, UIElement.KeyDownEvent, new System.Windows.Input.KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
			}
		}

		public override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key != Key.DeadCharProcessed)
			{
				e.Handled = RaiseEventPair(window, UIElement.PreviewKeyUpEvent, UIElement.KeyUpEvent, new System.Windows.Input.KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
			}
		}
	}

	private Window parentWindow;

	private TextDocument document;

	private InputHandler myInputHandler;

	private bool sourceIsInitialized;

	private Point visualLocation;

	private Point visualLocationTop;

	public TextArea TextArea { get; private set; }

	public int StartOffset { get; set; }

	public int EndOffset { get; set; }

	protected bool IsUp { get; private set; }

	protected virtual bool CloseOnFocusLost => true;

	private bool IsTextAreaFocused
	{
		get
		{
			if (parentWindow != null && !parentWindow.IsActive)
			{
				return false;
			}
			return TextArea.IsKeyboardFocused;
		}
	}

	public bool ExpectInsertionBeforeStart { get; set; }

	static CompletionWindowBase()
	{
		Window.WindowStyleProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(WindowStyle.None));
		Window.ShowActivatedProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(Boxes.False));
		Window.ShowInTaskbarProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(Boxes.False));
	}

	public CompletionWindowBase(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		TextArea = textArea;
		parentWindow = Window.GetWindow(textArea);
		base.Owner = parentWindow;
		AddHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), handledEventsToo: true);
		StartOffset = (EndOffset = TextArea.Caret.Offset);
		AttachEvents();
	}

	private void AttachEvents()
	{
		document = TextArea.Document;
		if (document != null)
		{
			document.Changing += textArea_Document_Changing;
		}
		TextArea.LostKeyboardFocus += TextAreaLostFocus;
		TextArea.TextView.ScrollOffsetChanged += TextViewScrollOffsetChanged;
		TextArea.DocumentChanged += TextAreaDocumentChanged;
		if (parentWindow != null)
		{
			parentWindow.LocationChanged += parentWindow_LocationChanged;
		}
		foreach (InputHandler item in TextArea.StackedInputHandlers.OfType<InputHandler>())
		{
			if (item.window.GetType() == GetType())
			{
				TextArea.PopStackedInputHandler(item);
			}
		}
		myInputHandler = new InputHandler(this);
		TextArea.PushStackedInputHandler(myInputHandler);
	}

	protected virtual void DetachEvents()
	{
		if (document != null)
		{
			document.Changing -= textArea_Document_Changing;
		}
		TextArea.LostKeyboardFocus -= TextAreaLostFocus;
		TextArea.TextView.ScrollOffsetChanged -= TextViewScrollOffsetChanged;
		TextArea.DocumentChanged -= TextAreaDocumentChanged;
		if (parentWindow != null)
		{
			parentWindow.LocationChanged -= parentWindow_LocationChanged;
		}
		TextArea.PopStackedInputHandler(myInputHandler);
	}

	private void TextViewScrollOffsetChanged(object sender, EventArgs e)
	{
		if (sourceIsInitialized)
		{
			IScrollInfo textView = TextArea.TextView;
			Rect rect = new Rect(textView.HorizontalOffset, textView.VerticalOffset, textView.ViewportWidth, textView.ViewportHeight);
			if (rect.Contains(visualLocation) || rect.Contains(visualLocationTop))
			{
				UpdatePosition();
			}
			else
			{
				Close();
			}
		}
	}

	private void TextAreaDocumentChanged(object sender, EventArgs e)
	{
		Close();
	}

	private void TextAreaLostFocus(object sender, RoutedEventArgs e)
	{
		base.Dispatcher.BeginInvoke(new Action(CloseIfFocusLost), DispatcherPriority.Background);
	}

	private void parentWindow_LocationChanged(object sender, EventArgs e)
	{
		UpdatePosition();
	}

	protected override void OnDeactivated(EventArgs e)
	{
		base.OnDeactivated(e);
		base.Dispatcher.BeginInvoke(new Action(CloseIfFocusLost), DispatcherPriority.Background);
	}

	protected static bool RaiseEventPair(UIElement target, RoutedEvent previewEvent, RoutedEvent @event, RoutedEventArgs args)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (previewEvent == null)
		{
			throw new ArgumentNullException("previewEvent");
		}
		if (@event == null)
		{
			throw new ArgumentNullException("event");
		}
		if (args == null)
		{
			throw new ArgumentNullException("args");
		}
		args.RoutedEvent = previewEvent;
		target.RaiseEvent(args);
		args.RoutedEvent = @event;
		target.RaiseEvent(args);
		return args.Handled;
	}

	private void OnMouseUp(object sender, MouseButtonEventArgs e)
	{
		ActivateParentWindow();
	}

	protected virtual void ActivateParentWindow()
	{
		if (parentWindow != null)
		{
			parentWindow.Activate();
		}
	}

	private void CloseIfFocusLost()
	{
		if (CloseOnFocusLost && !base.IsActive && !IsTextAreaFocused)
		{
			Close();
		}
	}

	protected override void OnSourceInitialized(EventArgs e)
	{
		base.OnSourceInitialized(e);
		if (document != null && StartOffset != TextArea.Caret.Offset)
		{
			SetPosition(new TextViewPosition(document.GetLocation(StartOffset)));
		}
		else
		{
			SetPosition(TextArea.Caret.Position);
		}
		sourceIsInitialized = true;
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		DetachEvents();
	}

	protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled && e.Key == Key.Escape)
		{
			e.Handled = true;
			Close();
		}
	}

	protected void SetPosition(TextViewPosition position)
	{
		TextView textView = TextArea.TextView;
		visualLocation = textView.GetVisualPosition(position, VisualYPosition.LineBottom);
		visualLocationTop = textView.GetVisualPosition(position, VisualYPosition.LineTop);
		UpdatePosition();
	}

	protected void UpdatePosition()
	{
		TextView textView = TextArea.TextView;
		if (PresentationSource.FromVisual(textView) == null)
		{
			return;
		}
		Point point = textView.PointToScreen(visualLocation - textView.ScrollOffset);
		Point point2 = textView.PointToScreen(visualLocationTop - textView.ScrollOffset);
		Size size = new Size(base.ActualWidth, base.ActualHeight).TransformToDevice(textView);
		Rect rect = new Rect(point, size);
		Rect rect2 = Screen.GetWorkingArea(point.ToSystemDrawing()).ToWpf();
		if (!rect2.Contains(rect))
		{
			if (rect.Left < rect2.Left)
			{
				rect.X = rect2.Left;
			}
			else if (rect.Right > rect2.Right)
			{
				rect.X = rect2.Right - rect.Width;
			}
			if (rect.Bottom > rect2.Bottom)
			{
				rect.Y = point2.Y - rect.Height;
				IsUp = true;
			}
			else
			{
				IsUp = false;
			}
			if (rect.Y < rect2.Top)
			{
				rect.Y = rect2.Top;
			}
		}
		rect = rect.TransformFromDevice(textView);
		base.Left = rect.X;
		base.Top = rect.Y;
	}

	protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
	{
		base.OnRenderSizeChanged(sizeInfo);
		if (sizeInfo.HeightChanged && IsUp)
		{
			base.Top += sizeInfo.PreviousSize.Height - sizeInfo.NewSize.Height;
		}
	}

	private void textArea_Document_Changing(object sender, DocumentChangeEventArgs e)
	{
		if (e.Offset + e.RemovalLength == StartOffset && e.RemovalLength > 0)
		{
			Close();
		}
		if (e.Offset == StartOffset && e.RemovalLength == 0 && ExpectInsertionBeforeStart)
		{
			StartOffset = e.GetNewOffset(StartOffset, AnchorMovementType.AfterInsertion);
			ExpectInsertionBeforeStart = false;
		}
		else
		{
			StartOffset = e.GetNewOffset(StartOffset, AnchorMovementType.BeforeInsertion);
		}
		EndOffset = e.GetNewOffset(EndOffset, AnchorMovementType.AfterInsertion);
	}
}
