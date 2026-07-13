using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using PvfCode.Controls.TextEditorFolder;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class CompletionWindowBase : ThemedWindow
{
	private sealed class InputHandler : TextAreaStackedInputHandler
	{
		internal readonly CompletionWindowBase Window;

		public InputHandler(CompletionWindowBase window)
			: base(window.TextArea)
		{
			Window = window;
		}

		public override void Detach()
		{
			base.Detach();
			Window.Close();
		}

		public override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key != Key.DeadCharProcessed)
			{
				e.Handled = RaiseEventPair(Window, UIElement.PreviewKeyDownEvent, UIElement.KeyDownEvent, new System.Windows.Input.KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
			}
		}

		public override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key != Key.DeadCharProcessed)
			{
				e.Handled = RaiseEventPair(Window, UIElement.PreviewKeyUpEvent, UIElement.KeyUpEvent, new System.Windows.Input.KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
			}
		}
	}

	public Window parentWindow;

	private TextDocument document;

	private InputHandler inputHandler;

	private bool sourceIsInitialized;

	private Point visualLocation;

	private Point visualLocationTop;

	internal CodeCompletionToolTip CompletionToolTip { get; set; }

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
		Window.WindowStyleProperty.OverrideMetadata(typeof(CompletionWindowBase), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)WindowStyle.None));
		Window.ShowActivatedProperty.OverrideMetadata(typeof(CompletionWindowBase), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Boxes.False));
		Window.ShowInTaskbarProperty.OverrideMetadata(typeof(CompletionWindowBase), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Boxes.False));
	}

	public CompletionWindowBase(TextEditorBase editorBase, TextArea textArea, int startOffSet, int endOffset)
	{
		CompletionToolTip = new CodeCompletionToolTip();
		try
		{
			if (textArea == null)
			{
				throw new ArgumentNullException("textArea");
			}
			TextArea = textArea;
			parentWindow = Window.GetWindow((DependencyObject)(object)textArea);
			base.Owner = parentWindow;
			AddHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), handledEventsToo: true);
			base.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
			StartOffset = startOffSet;
			EndOffset = endOffset;
			base.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
			AttachEvents();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionWindowBase.CompletionWindowBase");
		}
	}

	private void AttachEvents()
	{
		try
		{
			document = TextArea.Document;
			if (document != null)
			{
				document.Changing += TextAreaDocumentChanging;
			}
			TextArea.LostKeyboardFocus += TextAreaLostFocus;
			CompletionToolTip.editor.TextArea.LostKeyboardFocus += TextAreaLostFocus;
			TextArea.TextView.ScrollOffsetChanged += TextViewScrollOffsetChanged;
			TextArea.DocumentChanged += TextAreaDocumentChanged;
			if (parentWindow != null)
			{
				parentWindow.LocationChanged += ParentWindowLocationChanged;
			}
			foreach (InputHandler item in TextArea.StackedInputHandlers.OfType<InputHandler>())
			{
				if (item.Window.GetType() == GetType())
				{
					TextArea.PopStackedInputHandler(item);
				}
			}
			inputHandler = new InputHandler(this);
			TextArea.PushStackedInputHandler(inputHandler);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionWindowBase.AttachEvents");
		}
	}

	protected virtual void DetachEvents()
	{
		try
		{
			if (document != null)
			{
				document.Changing -= TextAreaDocumentChanging;
			}
			TextArea.LostKeyboardFocus -= TextAreaLostFocus;
			CompletionToolTip.editor.TextArea.LostKeyboardFocus -= TextAreaLostFocus;
			TextArea.TextView.ScrollOffsetChanged -= TextViewScrollOffsetChanged;
			TextArea.DocumentChanged -= TextAreaDocumentChanged;
			if (parentWindow != null)
			{
				parentWindow.LocationChanged -= ParentWindowLocationChanged;
			}
			TextArea.PopStackedInputHandler(inputHandler);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionWindowBase.DetachEvents");
		}
	}

	private void TextViewScrollOffsetChanged(object? sender, EventArgs e)
	{
		try
		{
			if (sourceIsInitialized)
			{
				IScrollInfo textView = TextArea.TextView;
				Rect visibleArea = new Rect(textView.HorizontalOffset, textView.VerticalOffset, textView.ViewportWidth, textView.ViewportHeight);
				if (visibleArea.Contains(visualLocation) || visibleArea.Contains(visualLocationTop))
				{
					UpdatePosition();
				}
				else
				{
					Close();
				}
			}
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "CompletionWindowBase.TextViewScrollOffsetChanged");
		}
	}

	private void TextAreaDocumentChanged(object? sender, EventArgs e)
	{
		Close();
	}

	private void TextAreaLostFocus(object? sender, RoutedEventArgs e)
	{
		try
		{
			if (!CompletionToolTip.editor.TextArea.Focusable)
			{
				((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)new Action(CloseIfFocusLost), DispatcherPriority.Background, Array.Empty<object>());
			}
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "CompletionWindowBase.TextAreaLostFocus");
		}
	}

	private void ParentWindowLocationChanged(object? sender, EventArgs e)
	{
		UpdatePosition();
	}

	protected override void OnDeactivated(EventArgs e)
	{
		base.OnDeactivated(e);
		((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)new Action(CloseIfFocusLost), DispatcherPriority.Background, Array.Empty<object>());
	}

	protected static bool RaiseEventPair(UIElement target, RoutedEvent previewEvent, RoutedEvent @event, RoutedEventArgs args)
	{
		try
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
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionWindowBase.RaiseEventPair");
			return false;
		}
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
		Point topPoint = textView.PointToScreen(visualLocationTop - textView.ScrollOffset);
		Size size = ExtensionMethods.TransformToDevice(new Size(base.ActualWidth, base.ActualHeight), (Visual)textView);
		Rect rect = new Rect(point, size);
		Rect workingArea = Screen.GetWorkingArea(point.ToSystemDrawing()).ToWpf();
		if (!workingArea.Contains(rect))
		{
			if (rect.Left < workingArea.Left)
			{
				rect.X = workingArea.Left;
			}
			else if (rect.Right > workingArea.Right)
			{
				rect.X = workingArea.Right - rect.Width;
			}
			if (rect.Bottom > workingArea.Bottom)
			{
				rect.Y = topPoint.Y - rect.Height;
				IsUp = true;
			}
			else
			{
				IsUp = false;
			}
			if (rect.Y < workingArea.Top)
			{
				rect.Y = workingArea.Top;
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

	private void TextAreaDocumentChanging(object? sender, DocumentChangeEventArgs e)
	{
		try
		{
			if ((e.Offset + e.RemovalLength == StartOffset && e.RemovalLength > 0) || e.Offset == StartOffset)
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
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "CompletionWindowBase.TextArea_Document_Changing");
		}
	}
}
