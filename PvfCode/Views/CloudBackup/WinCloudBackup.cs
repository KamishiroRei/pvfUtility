using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.CloudBackup;

public class WinCloudBackup : ThemedWindow, IComponentConnector
{
	private bool QOEvn232rG;

	public WinCloudBackup()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!QOEvn232rG)
		{
			QOEvn232rG = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/cloudbackup/wincloudbackup.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		QOEvn232rG = true;
	}
}
