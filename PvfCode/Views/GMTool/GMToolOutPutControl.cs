using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using TextEditLib;

namespace PvfCode.Views.GMTool;

public class GMToolOutPutControl : UserControl, IComponentConnector
{
	internal TextEdit editor;

	private bool QRsvr8wjpV;

	public GMToolOutPutControl()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!QRsvr8wjpV)
		{
			QRsvr8wjpV = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/gmtool/gmtooloutputcontrol.xaml", UriKind.Relative);
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
			editor = (TextEdit)target;
		}
		else
		{
			QRsvr8wjpV = true;
		}
	}
}
