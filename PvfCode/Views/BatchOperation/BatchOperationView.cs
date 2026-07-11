using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Editors;

namespace PvfCode.Views.BatchOperation;

public class BatchOperationView : ThemedWindow, IComponentConnector
{
	internal BatchOperationView win;

	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal ToolBarControl toolbarControl;

	internal ComboBoxEdit comboBoxEditFileTypesString;

	private bool w8DvM7M5o8;

	public BatchOperationView()
	{
		base.DataContext = new BatchOperationViewModel();
		InitializeComponent();
	}

	public BatchOperationViewModel GetVm()
	{
		return base.DataContext as BatchOperationViewModel;
	}

	private void pxdvcoUOO7(object P_0, ItemCancelEventArgs P_1)
	{
		P_1.Cancel = true;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		base.OnClosing(e);
		if (GetVm().RecordingLoading)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseStopRecordMacro"));
			e.Cancel = true;
		}
		else
		{
			Application.Current.MainWindow.Activate();
		}
	}

	private void KYUv8aQM78(object P_0, RoutedEventArgs P_1)
	{
		comboBoxEditFileTypesString.EditValue = null;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!w8DvM7M5o8)
		{
			w8DvM7M5o8 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/batchoperation/batchoperationview.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			win = (BatchOperationView)target;
			break;
		case 2:
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += pxdvcoUOO7;
			break;
		case 3:
			Root = (LayoutGroup)target;
			break;
		case 4:
			toolbarControl = (ToolBarControl)target;
			break;
		case 5:
			comboBoxEditFileTypesString = (ComboBoxEdit)target;
			break;
		case 6:
			((ButtonInfo)target).Click += KYUv8aQM78;
			break;
		default:
			w8DvM7M5o8 = true;
			break;
		}
	}
}
