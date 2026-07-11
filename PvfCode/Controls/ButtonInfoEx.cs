using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Editors;

namespace PvfCode.Controls;

public class ButtonInfoEx : ButtonInfo, IComponentConnector
{
	private bool pETawgYu4Q;

	public ButtonInfoEx()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!pETawgYu4Q)
		{
			pETawgYu4Q = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/buttoninfoex.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		pETawgYu4Q = true;
	}
}
