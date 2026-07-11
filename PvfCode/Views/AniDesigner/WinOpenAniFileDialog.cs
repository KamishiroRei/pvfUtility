using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.AniDesigner;

public class WinOpenAniFileDialog : ThemedWindow, IComponentConnector
{
	internal WinOpenAniFileDialog win;

	private bool e5VBuPDfv5;

	public WinOpenAniFileDialog()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!e5VBuPDfv5)
		{
			e5VBuPDfv5 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/anidesigner/winopenanifiledialog.xaml", UriKind.Relative);
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
			win = (WinOpenAniFileDialog)target;
		}
		else
		{
			e5VBuPDfv5 = true;
		}
	}
}
