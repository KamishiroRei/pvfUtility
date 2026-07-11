using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Grid;

namespace PvfCode.Views;

public class ErrorListView : UserControl, IComponentConnector
{
	internal TableView view;

	private bool VRyTMbrwEB;

	public ErrorListView()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!VRyTMbrwEB)
		{
			VRyTMbrwEB = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/errorlistview.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			view = (TableView)target;
		}
		else
		{
			VRyTMbrwEB = true;
		}
	}
}
