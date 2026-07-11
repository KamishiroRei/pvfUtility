using System;
using System.Linq;
using System.Runtime.CompilerServices;
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
	private sealed class oZ9fUlIStpwgK8RA3iW : TextAreaStackedInputHandler
	{
		internal readonly CompletionWindowBase nCPIAy6rYm;

		public oZ9fUlIStpwgK8RA3iW(CompletionWindowBase P_0)
			: base(P_0.TextArea)
		{
			nCPIAy6rYm = P_0;
		}

		public override void Detach()
		{
			base.Detach();
			nCPIAy6rYm.Close();
		}

		public override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs P_0)
		{
			if ((int)P_0.Key != 172)
			{
				P_0.Handled = RaiseEventPair(nCPIAy6rYm, UIElement.PreviewKeyDownEvent, UIElement.KeyDownEvent, new System.Windows.Input.KeyEventArgs(P_0.KeyboardDevice, P_0.InputSource, P_0.Timestamp, P_0.Key));
			}
		}

		public override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs P_0)
		{
			if ((int)P_0.Key != 172)
			{
				P_0.Handled = RaiseEventPair(nCPIAy6rYm, UIElement.PreviewKeyUpEvent, UIElement.KeyUpEvent, new System.Windows.Input.KeyEventArgs(P_0.KeyboardDevice, P_0.InputSource, P_0.Timestamp, P_0.Key));
			}
		}
	}

	[CompilerGenerated]
	private CodeCompletionToolTip nFvuje0dq0;

	[CompilerGenerated]
	private TextArea ErfuTFhrly;

	public Window parentWindow;

	private TextDocument vkduCxdZ7A;

	private readonly TextEditorBase tTHuHG1vrV;

	[CompilerGenerated]
	private int PDZuha1jvE;

	[CompilerGenerated]
	private int Wu0uvy26TF;

	[CompilerGenerated]
	private bool BUDuBYRAiQ;

	private oZ9fUlIStpwgK8RA3iW qgkuFbAlYP;

	private bool od7ursuWO3;

	private Point FGLuWyeg1J;

	private Point G3AumBsaTv;

	[CompilerGenerated]
	private bool xP6u2DP23h;

	public TextArea TextArea
	{
		[CompilerGenerated]
		get
		{
			return ErfuTFhrly;
		}
		[CompilerGenerated]
		private set
		{
			ErfuTFhrly = value;
		}
	}

	public int StartOffset
	{
		[CompilerGenerated]
		get
		{
			return PDZuha1jvE;
		}
		[CompilerGenerated]
		set
		{
			PDZuha1jvE = value;
		}
	}

	public int EndOffset
	{
		[CompilerGenerated]
		get
		{
			return Wu0uvy26TF;
		}
		[CompilerGenerated]
		set
		{
			Wu0uvy26TF = value;
		}
	}

	protected bool IsUp
	{
		[CompilerGenerated]
		get
		{
			return BUDuBYRAiQ;
		}
		[CompilerGenerated]
		private set
		{
			BUDuBYRAiQ = value;
		}
	}

	protected virtual bool CloseOnFocusLost => true;

	public bool ExpectInsertionBeforeStart
	{
		[CompilerGenerated]
		get
		{
			return xP6u2DP23h;
		}
		[CompilerGenerated]
		set
		{
			xP6u2DP23h = value;
		}
	}

	[SpecialName]
	[CompilerGenerated]
	internal CodeCompletionToolTip otjiVCqVBe()
	{
		return nFvuje0dq0;
	}

	[SpecialName]
	[CompilerGenerated]
	internal void VfVi3lM8sF(CodeCompletionToolTip P_0)
	{
		nFvuje0dq0 = P_0;
	}

	static CompletionWindowBase()
	{
		Window.WindowStyleProperty.OverrideMetadata(typeof(CompletionWindowBase), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)WindowStyle.None));
		Window.ShowActivatedProperty.OverrideMetadata(typeof(CompletionWindowBase), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Boxes.False));
		Window.ShowInTaskbarProperty.OverrideMetadata(typeof(CompletionWindowBase), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Boxes.False));
	}

	public CompletionWindowBase(TextEditorBase editorBase, TextArea textArea, int startOffSet, int endOffset)
	{
		nFvuje0dq0 = new CodeCompletionToolTip();
		try
		{
			if (textArea == null)
			{
				throw new ArgumentNullException("textArea");
			}
			TextArea = textArea;
			tTHuHG1vrV = editorBase;
			parentWindow = Window.GetWindow((DependencyObject)(object)textArea);
			base.Owner = parentWindow;
			AddHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(KXeicgA63Q), handledEventsToo: true);
			base.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
			StartOffset = startOffSet;
			EndOffset = endOffset;
			base.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
			om0i05npY6();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionWindowBase.CompletionWindowBase");
		}
	}

	private void om0i05npY6()
	{
		try
		{
			vkduCxdZ7A = TextArea.Document;
			if (vkduCxdZ7A != null)
			{
				vkduCxdZ7A.Changing += F8MiMGkLw4;
			}
			TextArea.LostKeyboardFocus += zCkip94p3H;
			otjiVCqVBe().editor.TextArea.LostKeyboardFocus += zCkip94p3H;
			TextArea.TextView.ScrollOffsetChanged += g8Bi7idYPu;
			TextArea.DocumentChanged += qBhiXLccAT;
			if (parentWindow != null)
			{
				parentWindow.LocationChanged += BBCiUsRtOT;
			}
			foreach (oZ9fUlIStpwgK8RA3iW item in TextArea.StackedInputHandlers.OfType<oZ9fUlIStpwgK8RA3iW>())
			{
				if (((object)item.nCPIAy6rYm).GetType() == ((object)this).GetType())
				{
					TextArea.PopStackedInputHandler(item);
				}
			}
			qgkuFbAlYP = new oZ9fUlIStpwgK8RA3iW(this);
			TextArea.PushStackedInputHandler(qgkuFbAlYP);
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
			if (vkduCxdZ7A != null)
			{
				vkduCxdZ7A.Changing -= F8MiMGkLw4;
			}
			TextArea.LostKeyboardFocus -= zCkip94p3H;
			otjiVCqVBe().editor.TextArea.LostKeyboardFocus -= zCkip94p3H;
			TextArea.TextView.ScrollOffsetChanged -= g8Bi7idYPu;
			TextArea.DocumentChanged -= qBhiXLccAT;
			if (parentWindow != null)
			{
				parentWindow.LocationChanged -= BBCiUsRtOT;
			}
			TextArea.PopStackedInputHandler(qgkuFbAlYP);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionWindowBase.DetachEvents");
		}
	}

	private void g8Bi7idYPu(object? sender, EventArgs P_1)
	{
		try
		{
			if (od7ursuWO3)
			{
				IScrollInfo textView = TextArea.TextView;
				Rect val = default(Rect);
				val = new Rect(textView.HorizontalOffset, textView.VerticalOffset, textView.ViewportWidth, textView.ViewportHeight);
				if (val.Contains(FGLuWyeg1J) || val.Contains(G3AumBsaTv))
				{
					UpdatePosition();
				}
				else
				{
					Close();
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionWindowBase.TextViewScrollOffsetChanged");
		}
	}

	private void qBhiXLccAT(object? sender, EventArgs P_1)
	{
		Close();
	}

	private void zCkip94p3H(object? sender, RoutedEventArgs P_1)
	{
		try
		{
			if (!otjiVCqVBe().editor.TextArea.Focusable)
			{
				((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)new Action(P8Di88FQCO), (DispatcherPriority)4, Array.Empty<object>());
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionWindowBase.TextAreaLostFocus");
		}
	}

	private void BBCiUsRtOT(object? sender, EventArgs P_1)
	{
		UpdatePosition();
	}

	protected override void OnDeactivated(EventArgs e)
	{
		base.OnDeactivated(e);
		((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)new Action(P8Di88FQCO), (DispatcherPriority)4, Array.Empty<object>());
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

	private void KXeicgA63Q(object P_0, MouseButtonEventArgs P_1)
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

	private void P8Di88FQCO()
	{
		if (CloseOnFocusLost && !base.IsActive && !yCWuDQhjIX())
		{
			Close();
		}
	}

	[SpecialName]
	private bool yCWuDQhjIX()
	{
		if (parentWindow != null && !parentWindow.IsActive)
		{
			return false;
		}
		return TextArea.IsKeyboardFocused;
	}

	protected override void OnSourceInitialized(EventArgs e)
	{
		base.OnSourceInitialized(e);
		if (vkduCxdZ7A != null && StartOffset != TextArea.Caret.Offset)
		{
			SetPosition(new TextViewPosition(vkduCxdZ7A.GetLocation(StartOffset)));
		}
		else
		{
			SetPosition(TextArea.Caret.Position);
		}
		od7ursuWO3 = true;
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		DetachEvents();
	}

	protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled && (int)e.Key == 13)
		{
			e.Handled = true;
			Close();
		}
	}

	protected void SetPosition(TextViewPosition position)
	{
		TextView textView = TextArea.TextView;
		FGLuWyeg1J = textView.GetVisualPosition(position, VisualYPosition.LineBottom);
		G3AumBsaTv = textView.GetVisualPosition(position, VisualYPosition.LineTop);
		UpdatePosition();
	}

	protected void UpdatePosition()
	{
		TextView textView = TextArea.TextView;
		if (PresentationSource.FromVisual(textView) == null)
		{
			return;
		}
		Point val = textView.PointToScreen(FGLuWyeg1J - textView.ScrollOffset);
		Point val2 = textView.PointToScreen(G3AumBsaTv - textView.ScrollOffset);
		Size val3 = ExtensionMethods.TransformToDevice(new Size(base.ActualWidth, base.ActualHeight), (Visual)textView);
		Rect val4 = default(Rect);
		val4 = new Rect(val, val3);
		Rect val5 = Screen.GetWorkingArea(val.ToSystemDrawing()).ToWpf();
		if (!val5.Contains(val4))
		{
			if (val4.Left < val5.Left)
			{
				val4.X = val5.Left;
			}
			else if (val4.Right > val5.Right)
			{
				val4.X = val5.Right - val4.Width;
			}
			if (val4.Bottom > val5.Bottom)
			{
				val4.Y = val2.Y - val4.Height;
				IsUp = true;
			}
			else
			{
				IsUp = false;
			}
			if (val4.Y < val5.Top)
			{
				val4.Y = val5.Top;
			}
		}
		val4 = val4.TransformFromDevice(textView);
		base.Left = val4.X;
		base.Top = val4.Y;
	}

	protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
	{
		base.OnRenderSizeChanged(sizeInfo);
		if (sizeInfo.HeightChanged && IsUp)
		{
			double top = base.Top;
			Size val = sizeInfo.PreviousSize;
			double height = val.Height;
			val = sizeInfo.NewSize;
			base.Top = top + (height - val.Height);
		}
	}

	private void F8MiMGkLw4(object? sender, DocumentChangeEventArgs P_1)
	{
		try
		{
			if ((P_1.Offset + P_1.RemovalLength == StartOffset && P_1.RemovalLength > 0) || P_1.Offset == StartOffset)
			{
				Close();
			}
			if (P_1.Offset == StartOffset && P_1.RemovalLength == 0 && ExpectInsertionBeforeStart)
			{
				StartOffset = P_1.GetNewOffset(StartOffset, AnchorMovementType.AfterInsertion);
				ExpectInsertionBeforeStart = false;
			}
			else
			{
				StartOffset = P_1.GetNewOffset(StartOffset, AnchorMovementType.BeforeInsertion);
			}
			EndOffset = P_1.GetNewOffset(EndOffset, AnchorMovementType.AfterInsertion);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionWindowBase.TextArea_Document_Changing");
		}
	}
}
