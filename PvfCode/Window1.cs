using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PvfCode;

public class Window1 : Window, IComponentConnector
{
	internal Grid gridHeader;

	private bool KxQTJndoVe;

	public Window1()
	{
		InitializeComponent();
		Title = "NPC 商店布局预览";
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!KxQTJndoVe)
		{
			KxQTJndoVe = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/window1.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			gridHeader = (Grid)target;
		}
		else
		{
			KxQTJndoVe = true;
		}
	}
}
