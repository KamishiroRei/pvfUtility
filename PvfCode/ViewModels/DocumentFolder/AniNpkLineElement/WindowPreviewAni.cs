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
using PvfCode.Services.PvfParsingNew.EditorPrivew;

namespace PvfCode.ViewModels.DocumentFolder.AniNpkLineElement;

public class WindowPreviewAni : ThemedWindow, IComponentConnector
{
	internal BarContainerControl toolBar;

	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	private bool KetGGrCYbe;

	public WindowPreviewAni(IList<PrivewAniData> items)
	{
		base.DataContext = new WindowPreviewAniViewModel(items);
		InitializeComponent();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		Application.Current.MainWindow.Activate();
	}

	private void iMPGu3mXyF(object P_0, ItemCancelEventArgs P_1)
	{
		P_1.Cancel = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!KetGGrCYbe)
		{
			KetGGrCYbe = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/viewmodels/documentfolder/aninpklineelement/windowpreviewani.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			toolBar = (BarContainerControl)target;
			break;
		case 2:
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += iMPGu3mXyF;
			break;
		case 3:
			Root = (LayoutGroup)target;
			break;
		default:
			KetGGrCYbe = true;
			break;
		}
	}
}
