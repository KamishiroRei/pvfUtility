using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PvfCode.Views;

public class ViewPvfRelease : UserControl, IComponentConnector
{
	private bool CGDCH3bZCy;

	public ViewPvfRelease()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!CGDCH3bZCy)
		{
			CGDCH3bZCy = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/viewpvfrelease.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		CGDCH3bZCy = true;
	}
}
