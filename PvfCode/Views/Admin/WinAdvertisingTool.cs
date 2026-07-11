using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.Admin;

public class WinAdvertisingTool : ThemedWindow, IComponentConnector
{
	private bool Dd7BG3GAZl;

	public WinAdvertisingTool()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!Dd7BG3GAZl)
		{
			Dd7BG3GAZl = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/admin/winadvertisingtool.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		Dd7BG3GAZl = true;
	}
}
