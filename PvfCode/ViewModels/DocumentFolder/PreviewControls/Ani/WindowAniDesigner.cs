using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.ViewModels.DocumentFolder.PreviewControls.Ani;

public class WindowAniDesigner : ThemedWindow, IComponentConnector
{
	private bool RwW4Su7JlK;

	public WindowAniDesigner()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!RwW4Su7JlK)
		{
			RwW4Su7JlK = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/viewmodels/documentfolder/previewcontrols/ani/windowanidesigner.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		RwW4Su7JlK = true;
	}
}
