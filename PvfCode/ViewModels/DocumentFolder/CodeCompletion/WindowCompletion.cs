using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Bars;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.LoggerBase;
using PvfCode.Models.CodeCompletionModels;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class WindowCompletion : CompletionWindowBase, IComponentConnector
{
	private readonly TextEditorBase editor;

	private readonly PvfFileType? fileType;

	internal Grid rootGrid;

	internal CompletionList completionList;

	internal BarButtonItem BtnAddCompletionData;

	internal BarButtonItem BtnEditCompletionData;

	internal BarButtonItem BtnDeleteAddCompletionData;

	private bool contentLoaded;

	public CompletionList CompletionList => completionList;

	public bool CloseAutomatically { get; set; }

	protected override bool CloseOnFocusLost => CloseAutomatically;

	public bool CloseWhenCaretAtBeginning { get; set; }

	public WindowCompletion(TextEditorBase editorBase, TextArea textArea, PvfFileType? pvfFileType, int startOffSet, int endOffset)
		: base(editorBase, textArea, startOffSet, endOffset)
	{
		try
		{
			editor = editorBase;
			fileType = pvfFileType;
			InitializeComponent();
			CloseAutomatically = true;
			base.SizeToContent = SizeToContent.WidthAndHeight;
			base.MaxHeight = 250.0;
			base.MinHeight = 15.0;
			base.MinWidth = 300.0;
			CompletionToolTip.PlacementTarget = this;
			CompletionToolTip.Placement = PlacementMode.Right;
			AttachEvents();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.WindowCompletion");
		}
	}

	private void OnToolTipClosed(object? sender, EventArgs e)
	{
		if (CompletionToolTip != null)
		{
			CompletionToolTip.DataContext = null;
		}
		Application.Current.MainWindow.Activate();
	}

	private void OnCompletionListSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			try
			{
				if (CompletionToolTip != null)
				{
					CompletionToolTip.IsOpen = false;
					CodeCompletionData selectedItem = completionList.SelectedItem;
					if (selectedItem != null)
					{
						CompletionListBox listBox = completionList.ListBox;
						if (listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem) is ListBoxItem)
						{
							Point position = GetToolTipPosition();
							CompletionToolTip.HorizontalOffset = position.X - 2.0;
							CompletionToolTip.VerticalOffset = position.Y;
						}
						CodeCompletionToolTipViewModel codeCompletionToolTipViewModel = new CodeCompletionToolTipViewModel(selectedItem, fileType);
						CompletionToolTip.DataContext = codeCompletionToolTipViewModel;
						CompletionToolTip.IsOpen = true;
						codeCompletionToolTipViewModel.Loaded(null);
					}
				}
			}
			catch (Exception exception)
			{
				AppCore.Logger.ErrorUploadDialog(exception, "WindowCompletion.CompletionList_SelectionChanged");
			}
		}, Array.Empty<object>());
	}

	private Point GetToolTipPosition()
	{
		CompletionListBox listBox = completionList.ListBox;
		UIElement obj = listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem) as UIElement;
		Point point = default(Point);
		obj.PointToScreen(default(Point));
		return obj.TranslatePoint(point, this);
	}

	private void OnInsertionRequested(object? sender, EventArgs e)
	{
		try
		{
			Close();
			completionList.SelectedItem?.Complete(editor, base.TextArea, new AnchorSegment(base.TextArea.Document, base.StartOffset, base.EndOffset - base.StartOffset), e);
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "WindowCompletion.CompletionList_InsertionRequested");
		}
	}

	private void AttachEvents()
	{
		try
		{
			completionList.InsertionRequested += OnInsertionRequested;
			completionList.SelectionChanged += OnCompletionListSelectionChanged;
			CompletionList.CloseCompletionWindow += OnCloseCompletionWindow;
			base.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
			base.TextArea.MouseWheel += OnTextAreaMouseWheel;
			base.TextArea.PreviewTextInput += OnTextAreaPreviewTextInput;
			base.StateChanged += OnWindowStateChanged;
			parentWindow.StateChanged += OnWindowStateChanged;
			parentWindow.Deactivated += OnParentWindowDeactivated;
			CompletionToolTip.Closed += OnToolTipClosed;
			completionList.Loaded += OnCompletionListLoaded;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.AttachEvents");
		}
	}

	private void OnCloseCompletionWindow(object? sender, EventArgs e)
	{
		Close();
	}

	private void OnParentWindowDeactivated(object? sender, EventArgs e)
	{
		if (CompletionToolTip != null)
		{
			CompletionToolTip.IsOpen = false;
		}
	}

	private void OnWindowStateChanged(object? sender, EventArgs e)
	{
		try
		{
			if ((base.WindowState == WindowState.Minimized || parentWindow.WindowState == WindowState.Minimized) && CompletionToolTip != null)
			{
				CompletionToolTip.IsOpen = false;
			}
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "WindowCompletion.WindowCompletion_StateChanged");
		}
	}

	protected override void DetachEvents()
	{
		completionList.InsertionRequested -= OnInsertionRequested;
		completionList.SelectionChanged -= OnCompletionListSelectionChanged;
		base.TextArea.Caret.PositionChanged -= OnCaretPositionChanged;
		base.TextArea.MouseWheel -= OnTextAreaMouseWheel;
		base.TextArea.PreviewTextInput -= OnTextAreaPreviewTextInput;
		CompletionToolTip.Closed -= OnToolTipClosed;
		base.StateChanged -= OnWindowStateChanged;
		parentWindow.StateChanged -= OnWindowStateChanged;
		parentWindow.Deactivated -= OnParentWindowDeactivated;
		completionList.Loaded -= OnCompletionListLoaded;
		CompletionList.CloseCompletionWindow -= OnCloseCompletionWindow;
		base.DetachEvents();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		if (CompletionToolTip != null)
		{
			CompletionToolTip.IsOpen = false;
			CompletionToolTip = null;
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled)
		{
			completionList?.HandleKey(e);
		}
	}

	private void OnTextAreaPreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		try
		{
			e.Handled = CompletionWindowBase.RaiseEventPair(this, UIElement.PreviewTextInputEvent, UIElement.TextInputEvent, new TextCompositionEventArgs(e.Device, e.TextComposition));
		}
		catch (Exception)
		{
		}
	}

	private void OnTextAreaMouseWheel(object sender, MouseWheelEventArgs e)
	{
		e.Handled = CompletionWindowBase.RaiseEventPair(GetScrollEventTarget(), UIElement.PreviewMouseWheelEvent, UIElement.MouseWheelEvent, new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
	}

	private UIElement GetScrollEventTarget()
	{
		if (completionList == null)
		{
			return this;
		}
		return (UIElement)(completionList.ScrollViewer ?? ((object)completionList.ListBox) ?? ((object)completionList));
	}

	private void OnCaretPositionChanged(object? sender, EventArgs e)
	{
		try
		{
			int offset = base.TextArea.Caret.Offset;
			if (offset == base.StartOffset)
			{
				if (CloseAutomatically && CloseWhenCaretAtBeginning)
				{
					Close();
				}
				else
				{
					completionList.SelectItem(string.Empty);
				}
				return;
			}
			if (offset < base.StartOffset || offset > base.EndOffset)
			{
				if (CloseAutomatically)
				{
					Close();
				}
				return;
			}
			TextDocument document = base.TextArea.Document;
			if (document != null)
			{
				if (string.IsNullOrEmpty(document.GetText(base.StartOffset, offset - base.StartOffset)))
				{
					Close();
				}
				else
				{
					completionList.SelectItem(document.GetText(base.StartOffset, offset - base.StartOffset));
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void OnCompletionListLoaded(object sender, RoutedEventArgs e)
	{
		try
		{
			TextDocument document = base.TextArea.Document;
			if (base.StartOffset > 0 && base.StartOffset <= document.TextLength && base.EndOffset >= 0 && base.EndOffset <= document.TextLength)
			{
				string text = document.GetText(base.StartOffset, base.EndOffset - base.StartOffset);
				if (!string.IsNullOrEmpty(text))
				{
					completionList?.SelectItem(text);
				}
			}
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "WindowCompletion.WindowCompletion_Loaded");
		}
	}

	private void OnAddCompletionData(object sender, ItemClickEventArgs e)
	{
		try
		{
			WindowAddCodeCompletionData windowAddCodeCompletionData = new WindowAddCodeCompletionData(CodeCompletionData.Create(fileType.Value));
			windowAddCodeCompletionData.Owner = Application.Current.MainWindow;
			windowAddCodeCompletionData.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			windowAddCodeCompletionData.ShowDialog();
			Close();
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "WindowCompletion.BtnAddCompletionData_ItemClick");
		}
	}

	private async void OnDeleteCompletionData(object sender, ItemClickEventArgs e)
	{
		try
		{
			CodeCompletionData selectedItem = completionList.SelectedItem;
			if (selectedItem != null)
			{
				if (AppSetting.Instance.EditConfig.DeleteCompletionData(selectedItem))
				{
					await AppCore.Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, "删除成功", Res.Instance.VisualStudioBlendLogo2015Pre_16x));
					Close();
				}
				else
				{
					AppCore.ShowMsg("你只能删除自己添加的数据 公开共享的数据无权删除！");
				}
			}
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "WindowCompletion.BtnDeleteAddCompletionData_ItemClick");
		}
	}

	private void OnEditCompletionData(object sender, ItemClickEventArgs e)
	{
		try
		{
			CodeCompletionData selectedItem = completionList.SelectedItem;
			if (selectedItem != null)
			{
				WindowAddCodeCompletionData windowAddCodeCompletionData = new WindowAddCodeCompletionData(selectedItem, isAdd: false);
				windowAddCodeCompletionData.Owner = Application.Current.MainWindow;
				windowAddCodeCompletionData.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				windowAddCodeCompletionData.ShowDialog();
				Close();
			}
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "WindowCompletion.BtnEditCompletionData_ItemClick");
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!contentLoaded)
		{
			contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/viewmodels/documentfolder/codecompletion/windowcompletion.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			rootGrid = (Grid)target;
			break;
		case 2:
			completionList = (CompletionList)target;
			break;
		case 3:
			BtnAddCompletionData = (BarButtonItem)target;
			BtnAddCompletionData.ItemClick += OnAddCompletionData;
			break;
		case 4:
			BtnEditCompletionData = (BarButtonItem)target;
			BtnEditCompletionData.ItemClick += OnEditCompletionData;
			break;
		case 5:
			BtnDeleteAddCompletionData = (BarButtonItem)target;
			BtnDeleteAddCompletionData.ItemClick += OnDeleteCompletionData;
			break;
		default:
			contentLoaded = true;
			break;
		}
	}
}
