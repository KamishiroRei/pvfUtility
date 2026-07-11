using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Docking.Base;

namespace PvfCode.Views.SearchPvf.SearchName;

public class ViewSearchNameControl : UserControl, IComponentConnector
{
	private bool rfGHTPv9et;

	public ViewSearchNameControl()
	{
		InitializeComponent();
	}

	private void h8BHj4GU3K(object P_0, ItemCancelEventArgs P_1)
	{
		P_1.Cancel = true;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!rfGHTPv9et)
		{
			rfGHTPv9et = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/searchpvf/searchname/viewsearchnamecontrol.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		rfGHTPv9et = true;
	}
}
