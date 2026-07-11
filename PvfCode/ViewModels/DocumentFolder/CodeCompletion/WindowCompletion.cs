using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
	private readonly TextEditorBase CwSu3n5h2Q;

	private readonly PvfFileType? FileType;

	[CompilerGenerated]
	private bool zmJuRVZAen;

	[CompilerGenerated]
	private bool KTSuNNpvdS;

	internal Grid rootGrid;

	internal CompletionList completionList;

	internal BarButtonItem BtnAddCompletionData;

	internal BarButtonItem BtnEditCompletionData;

	internal BarButtonItem BtnDeleteAddCompletionData;

	private bool vW2uzHU3rQ;

	public CompletionList CompletionList => completionList;

	public bool CloseAutomatically
	{
		[CompilerGenerated]
		get
		{
			return zmJuRVZAen;
		}
		[CompilerGenerated]
		set
		{
			zmJuRVZAen = value;
		}
	}

	protected override bool CloseOnFocusLost => CloseAutomatically;

	public bool CloseWhenCaretAtBeginning
	{
		[CompilerGenerated]
		get
		{
			return KTSuNNpvdS;
		}
		[CompilerGenerated]
		set
		{
			KTSuNNpvdS = value;
		}
	}

	public WindowCompletion(TextEditorBase editorBase, TextArea textArea, PvfFileType? pvfFileType, int startOffSet, int endOffset)
		: base(editorBase, textArea, startOffSet, endOffset)
	{
		try
		{
			CwSu3n5h2Q = editorBase;
			FileType = pvfFileType;
			InitializeComponent();
			CloseAutomatically = true;
			base.SizeToContent = SizeToContent.WidthAndHeight;
			base.MaxHeight = 250.0;
			base.MinHeight = 15.0;
			base.MinWidth = 300.0;
			otjiVCqVBe().PlacementTarget = this;
			otjiVCqVBe().Placement = PlacementMode.Right;
			zEXuPU3cCx();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.WindowCompletion");
		}
	}

	private void CeguEJwmr3(object? sender, EventArgs P_1)
	{
		if (otjiVCqVBe() != null)
		{
			otjiVCqVBe().DataContext = null;
		}
		Application.Current.MainWindow.Activate();
	}

	private void AeKuODWtBq(object P_0, SelectionChangedEventArgs P_1)
	{
		((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			try
			{
				if (otjiVCqVBe() != null)
				{
					otjiVCqVBe().IsOpen = false;
					CodeCompletionData selectedItem = completionList.SelectedItem;
					if (selectedItem != null)
					{
						CompletionListBox listBox = completionList.ListBox;
						if (listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem) is ListBoxItem)
						{
							Point val = ipEuKFoV5J();
							otjiVCqVBe().HorizontalOffset = val.X - 2.0;
							otjiVCqVBe().VerticalOffset = val.Y;
						}
						CodeCompletionToolTipViewModel codeCompletionToolTipViewModel = new CodeCompletionToolTipViewModel(selectedItem, FileType);
						otjiVCqVBe().DataContext = codeCompletionToolTipViewModel;
						otjiVCqVBe().IsOpen = true;
						codeCompletionToolTipViewModel.Loaded(null);
					}
				}
			}
			catch (Exception e)
			{
				AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.CompletionList_SelectionChanged");
			}
		}, Array.Empty<object>());
	}

	private Point ipEuKFoV5J()
	{
		CompletionListBox listBox = completionList.ListBox;
		UIElement obj = listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem) as UIElement;
		Point point = default(Point);
		obj.PointToScreen(default(Point));
		return obj.TranslatePoint(point, this);
	}

	private void fGvu938HZg(object? sender, EventArgs P_1)
	{
		try
		{
			Close();
			completionList.SelectedItem?.Complete(CwSu3n5h2Q, base.TextArea, new AnchorSegment(base.TextArea.Document, base.StartOffset, base.EndOffset - base.StartOffset), P_1);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.CompletionList_InsertionRequested");
		}
	}

	private void zEXuPU3cCx()
	{
		try
		{
			completionList.InsertionRequested += fGvu938HZg;
			completionList.SelectionChanged += AeKuODWtBq;
			CompletionList.CloseCompletionWindow += rY7uZpoP4n;
			base.TextArea.Caret.PositionChanged += MeQup4wIla;
			base.TextArea.MouseWheel += NYdu7OSRPV;
			base.TextArea.PreviewTextInput += AfMu0EiEyS;
			base.StateChanged += a7Guk3sMV0;
			parentWindow.StateChanged += a7Guk3sMV0;
			parentWindow.Deactivated += jyNuJPpQcl;
			otjiVCqVBe().Closed += CeguEJwmr3;
			completionList.Loaded += RZouUVIfAI;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.AttachEvents");
		}
	}

	private void rY7uZpoP4n(object? sender, EventArgs P_1)
	{
		Close();
	}

	private void jyNuJPpQcl(object? sender, EventArgs P_1)
	{
		if (otjiVCqVBe() != null)
		{
			otjiVCqVBe().IsOpen = false;
		}
	}

	private void a7Guk3sMV0(object? sender, EventArgs P_1)
	{
		try
		{
			if ((base.WindowState == WindowState.Minimized || parentWindow.WindowState == WindowState.Minimized) && otjiVCqVBe() != null)
			{
				otjiVCqVBe().IsOpen = false;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.WindowCompletion_StateChanged");
		}
	}

	protected override void DetachEvents()
	{
		completionList.InsertionRequested -= fGvu938HZg;
		completionList.SelectionChanged -= AeKuODWtBq;
		base.TextArea.Caret.PositionChanged -= MeQup4wIla;
		base.TextArea.MouseWheel -= NYdu7OSRPV;
		base.TextArea.PreviewTextInput -= AfMu0EiEyS;
		otjiVCqVBe().Closed -= CeguEJwmr3;
		base.StateChanged -= a7Guk3sMV0;
		parentWindow.StateChanged -= a7Guk3sMV0;
		otjiVCqVBe().Closed -= CeguEJwmr3;
		completionList.Loaded -= RZouUVIfAI;
		CompletionList.CloseCompletionWindow -= rY7uZpoP4n;
		base.DetachEvents();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		if (otjiVCqVBe() != null)
		{
			otjiVCqVBe().IsOpen = false;
			VfVi3lM8sF(null);
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

	private void AfMu0EiEyS(object P_0, TextCompositionEventArgs P_1)
	{
		try
		{
			P_1.Handled = CompletionWindowBase.RaiseEventPair(this, UIElement.PreviewTextInputEvent, UIElement.TextInputEvent, new TextCompositionEventArgs(P_1.Device, P_1.TextComposition));
		}
		catch (Exception)
		{
		}
	}

	private void NYdu7OSRPV(object P_0, MouseWheelEventArgs P_1)
	{
		P_1.Handled = CompletionWindowBase.RaiseEventPair(LenuX3G7xB(), UIElement.PreviewMouseWheelEvent, UIElement.MouseWheelEvent, new MouseWheelEventArgs(P_1.MouseDevice, P_1.Timestamp, P_1.Delta));
	}

	private UIElement LenuX3G7xB()
	{
		if (completionList == null)
		{
			return this;
		}
		return (UIElement)(completionList.ScrollViewer ?? ((object)completionList.ListBox) ?? ((object)completionList));
	}

	private void MeQup4wIla(object? sender, EventArgs P_1)
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

	private void RZouUVIfAI(object P_0, RoutedEventArgs P_1)
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
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.WindowCompletion_Loaded");
		}
	}

	private void QfwuctSPlh(object P_0, ItemClickEventArgs P_1)
	{
		try
		{
			WindowAddCodeCompletionData windowAddCodeCompletionData = new WindowAddCodeCompletionData(CodeCompletionData.Create(FileType.Value));
			windowAddCodeCompletionData.Owner = Application.Current.MainWindow;
			windowAddCodeCompletionData.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			windowAddCodeCompletionData.ShowDialog();
			Close();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.BtnAddCompletionData_ItemClick");
		}
	}

	private async void R8Ku8cIWpl(object P_0, ItemClickEventArgs P_1)
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
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.BtnDeleteAddCompletionData_ItemClick");
		}
	}

	private void BCruMwmGFT(object P_0, ItemClickEventArgs P_1)
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
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.BtnEditCompletionData_ItemClick");
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!vW2uzHU3rQ)
		{
			vW2uzHU3rQ = true;
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
			BtnAddCompletionData.ItemClick += QfwuctSPlh;
			break;
		case 4:
			BtnEditCompletionData = (BarButtonItem)target;
			BtnEditCompletionData.ItemClick += BCruMwmGFT;
			break;
		case 5:
			BtnDeleteAddCompletionData = (BarButtonItem)target;
			BtnDeleteAddCompletionData.ItemClick += R8Ku8cIWpl;
			break;
		default:
			vW2uzHU3rQ = true;
			break;
		}
	}

	[CompilerGenerated]
	private void aYxuVlAaqV()
	{
		try
		{
			if (otjiVCqVBe() == null)
			{
				return;
			}
			otjiVCqVBe().IsOpen = false;
			CodeCompletionData selectedItem = completionList.SelectedItem;
			if (selectedItem != null)
			{
				CompletionListBox listBox = completionList.ListBox;
				if (listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem) is ListBoxItem)
				{
					Point val = ipEuKFoV5J();
					otjiVCqVBe().HorizontalOffset = val.X - 2.0;
					otjiVCqVBe().VerticalOffset = val.Y;
				}
				CodeCompletionToolTipViewModel codeCompletionToolTipViewModel = new CodeCompletionToolTipViewModel(selectedItem, FileType);
				otjiVCqVBe().DataContext = codeCompletionToolTipViewModel;
				otjiVCqVBe().IsOpen = true;
				codeCompletionToolTipViewModel.Loaded(null);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowCompletion.CompletionList_SelectionChanged");
		}
	}
}
