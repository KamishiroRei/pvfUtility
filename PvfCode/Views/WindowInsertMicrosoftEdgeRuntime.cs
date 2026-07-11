using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views;

public class WindowInsertMicrosoftEdgeRuntime : ThemedWindow, IComponentConnector
{
	private bool tVFCmZpe7s;

	public WindowInsertMicrosoftEdgeRuntime()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!tVFCmZpe7s)
		{
			tVFCmZpe7s = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/windowinsertmicrosoftedgeruntime.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		tVFCmZpe7s = true;
	}
}
