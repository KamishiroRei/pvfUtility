using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using TextEditLib;

namespace PvfCode.Controls.TextEditorFolder;

public class DefaultScriptEditor : TextEdit, IComponentConnector
{
	private bool y74gvK9qaR;

	public DefaultScriptEditor()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!y74gvK9qaR)
		{
			y74gvK9qaR = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/texteditorfolder/defaultscripteditor.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		y74gvK9qaR = true;
	}
}
