using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;

namespace PvfCode.Views.GMTool;

public class GMToolMain : ThemedWindow, IComponentConnector
{
	internal ButtonInfo findBtn;

	internal GridControl gridUser;

	internal GridControl gridCharac;

	internal DockLayoutManager DemoDockContainer;

	internal GridControl gridItemCode;

	private bool uXpvF1b2pc;

	public GMToolMain()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!uXpvF1b2pc)
		{
			uXpvF1b2pc = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/gmtool/gmtoolmain.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			findBtn = (ButtonInfo)target;
			break;
		case 2:
			gridUser = (GridControl)target;
			break;
		case 3:
			gridCharac = (GridControl)target;
			break;
		case 4:
			DemoDockContainer = (DockLayoutManager)target;
			break;
		case 5:
			gridItemCode = (GridControl)target;
			break;
		default:
			uXpvF1b2pc = true;
			break;
		}
	}
}
