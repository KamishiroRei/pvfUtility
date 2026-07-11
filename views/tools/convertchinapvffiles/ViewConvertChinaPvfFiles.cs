using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using ViewModels.Tools.ConvertChinaPvfFiles;

namespace Views.Tools.ConvertChinaPvfFiles;

public class ViewConvertChinaPvfFiles : ThemedWindow, IComponentConnector
{
	private bool SGEuaQipP;

	public ViewConvertChinaPvfFiles()
	{
		base.DataContext = new ViewConvertChinaPvfFilesViewModel();
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!SGEuaQipP)
		{
			SGEuaQipP = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/tools/convertchinapvffiles/viewconvertchinapvffiles.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		SGEuaQipP = true;
	}
}
