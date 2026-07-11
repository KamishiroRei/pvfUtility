using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.Tools;

public class WorlddropTool : ThemedWindow, IComponentConnector
{
	private bool qwXCndjjXF;

	public WorlddropTool()
	{
		base.DataContext = new WorldDropToolViewModel();
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!qwXCndjjXF)
		{
			qwXCndjjXF = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/tools/worlddroptool.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		qwXCndjjXF = true;
	}
}
