using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PvfCode.Views.Preview;

public class PreviewPvfItem_EQU_Control : UserControl, IComponentConnector
{
	private bool R6OH8yVqM2;

	public PreviewPvfItem_EQU_Control()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!R6OH8yVqM2)
		{
			R6OH8yVqM2 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/preview/previewpvfitem_equ_control.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		R6OH8yVqM2 = true;
	}
}
