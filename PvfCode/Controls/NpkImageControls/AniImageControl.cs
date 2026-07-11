using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PvfCode.Controls.NpkImageControls;

public class AniImageControl : UserControl, IComponentConnector
{
	private bool NP36q2Kb2E;

	public AniImageControl()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!NP36q2Kb2E)
		{
			NP36q2Kb2E = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/npkimagecontrols/aniimagecontrol.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		NP36q2Kb2E = true;
	}
}
