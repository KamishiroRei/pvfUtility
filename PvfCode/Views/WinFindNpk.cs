using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;

namespace PvfCode.Views;

public class WinFindNpk : ThemedWindow, IComponentConnector
{
	internal GridControl gridMonster;

	private bool tjaC1D1Ov7;

	public WinFindNpk()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!tjaC1D1Ov7)
		{
			tjaC1D1Ov7 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/winfindnpk.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			gridMonster = (GridControl)target;
		}
		else
		{
			tjaC1D1Ov7 = true;
		}
	}
}
