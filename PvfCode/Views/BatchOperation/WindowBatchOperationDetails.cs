using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;

namespace PvfCode.Views.BatchOperation;

public class WindowBatchOperationDetails : ThemedWindow, IComponentConnector
{
	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	private bool wHxBrTZ91o;

	public WindowBatchOperationDetails(IEnumerable<string> successFiles, IEnumerable<string> errorFiles)
	{
		base.DataContext = new WindowBatchOperationDetailsViewModel(successFiles, errorFiles);
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Height = primaryScreenHeight * 0.8;
		base.Width = primaryScreenWidth * 0.5;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		((WindowBatchOperationDetailsViewModel)base.DataContext).Clear();
		base.DataContext = null;
		Application.Current.MainWindow.Activate();
	}

	private void TcgBBhlhVj(object P_0, RoutedEventArgs P_1)
	{
		((WindowBatchOperationDetailsViewModel)base.DataContext).Loaded();
	}

	private void IZVBFG1TTG(object P_0, ItemCancelEventArgs P_1)
	{
		P_1.Cancel = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!wHxBrTZ91o)
		{
			wHxBrTZ91o = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/batchoperation/windowbatchoperationdetails.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((WindowBatchOperationDetails)target).Loaded += TcgBBhlhVj;
			break;
		case 2:
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += IZVBFG1TTG;
			break;
		case 3:
			Root = (LayoutGroup)target;
			break;
		default:
			wHxBrTZ91o = true;
			break;
		}
	}
}
