using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views;

public class WindowSelectDefaultTheme : ThemedWindow, IComponentConnector
{
	private bool jy7Cg9Q9JX;

	public WindowSelectDefaultTheme()
	{
		InitializeComponent();
	}

	private void UElCa69Ica(object P_0, EventArgs P_1)
	{
		Application.Current.MainWindow.Activate();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!jy7Cg9Q9JX)
		{
			jy7Cg9Q9JX = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/windowselectdefaulttheme.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			((WindowSelectDefaultTheme)target).Closed += UElCa69Ica;
		}
		else
		{
			jy7Cg9Q9JX = true;
		}
	}
}
