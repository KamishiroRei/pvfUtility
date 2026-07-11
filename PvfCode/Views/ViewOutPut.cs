using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using TextEditLib;

namespace PvfCode.Views;

public class ViewOutPut : UserControl, IComponentConnector
{
	internal TextEdit editor;

	private bool M7ZCC4UWjt;

	public ViewOutPut()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!M7ZCC4UWjt)
		{
			M7ZCC4UWjt = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/viewoutput.xaml", UriKind.Relative);
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
			M7ZCC4UWjt = true;
		}
	}
}
