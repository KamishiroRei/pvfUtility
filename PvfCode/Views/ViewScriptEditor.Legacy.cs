#if RECOVERED_WPF_LEGACY_RESOURCES
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace PvfCode.Views;

public partial class ViewScriptEditor : IComponentConnector
{
	private bool legacyContentLoaded;

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (legacyContentLoaded)
		{
			return;
		}

		legacyContentLoaded = true;
		Uri resourceLocator = new(
			"/pvfUtility;V2026.1.22.2;component/views/viewscripteditor.xaml",
			UriKind.Relative);
		Application.LoadComponent(this, resourceLocator);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		legacyContentLoaded = true;
	}
}
#endif
