using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using PvfCode.Dot.Desktop;
using PvfCode.ViewModels.Store.Preview;

namespace PvfCode.Views.Store.Preview;

public class PreviewTabCommentView : ThemedWindow, IComponentConnector
{
	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal ToolBarControl toolbarControl;

	private bool XNbCpsNnhF;

	public PreviewTabCommentView(List<PvfCommentDto> items)
	{
		base.DataContext = new PreviewTabCommentViewViewModel(items);
		InitializeComponent();
	}

	private void uUEC7wBsxf(object P_0, ItemCancelEventArgs P_1)
	{
		P_1.Cancel = true;
	}

	private void j8wCXdC5s0(object P_0, EventArgs P_1)
	{
		Application.Current.MainWindow.Activate();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!XNbCpsNnhF)
		{
			XNbCpsNnhF = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/store/preview/previewtabcommentview.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
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
			((PreviewTabCommentView)target).Closed += j8wCXdC5s0;
			break;
		case 2:
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += uUEC7wBsxf;
			break;
		case 3:
			Root = (LayoutGroup)target;
			break;
		case 4:
			toolbarControl = (ToolBarControl)target;
			break;
		default:
			XNbCpsNnhF = true;
			break;
		}
	}
}
