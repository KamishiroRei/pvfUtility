using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using PvfCode.ViewModels.Description.FileListDescription;

namespace PvfCode.Views.DescriptionViews;

public class ViewFileListDescription : ThemedWindow, IComponentConnector
{
	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal ToolBarControl toolbarControl;

	private bool hcQvocPB0v;

	public ViewFileListDescription()
	{
		FileListDescriptionViewModel dataContext = new FileListDescriptionViewModel(base.Close);
		base.DataContext = dataContext;
		InitializeComponent();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		Application.Current.MainWindow.Activate();
	}

	private void pKAv1xk1f4(object P_0, ItemCancelEventArgs P_1)
	{
		P_1.Cancel = true;
	}

	private void PyMvwmH4d8(object P_0, RoutedEventArgs P_1)
	{
		Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!hcQvocPB0v)
		{
			hcQvocPB0v = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/descriptionviews/viewfilelistdescription.xaml", UriKind.Relative);
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
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += pKAv1xk1f4;
			break;
		case 2:
			Root = (LayoutGroup)target;
			break;
		case 3:
			toolbarControl = (ToolBarControl)target;
			break;
		case 4:
			((Button)target).Click += PyMvwmH4d8;
			break;
		default:
			hcQvocPB0v = true;
			break;
		}
	}
}
