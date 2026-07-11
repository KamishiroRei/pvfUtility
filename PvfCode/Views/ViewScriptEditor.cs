using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views;

public class ViewScriptEditor : ThemedWindow, IComponentConnector
{
	private bool BO4Ch4X4u5;

	public ViewScriptEditor(ViewScriptEditorViewModel vm)
	{
		base.DataContext = vm;
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Height = primaryScreenHeight * 0.6;
		base.Width = primaryScreenWidth * 0.6;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		((ViewScriptEditorViewModel)base.DataContext).Dispose();
		base.DataContext = null;
		Application.Current.MainWindow.Activate();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!BO4Ch4X4u5)
		{
			BO4Ch4X4u5 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/viewscripteditor.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		BO4Ch4X4u5 = true;
	}
}
