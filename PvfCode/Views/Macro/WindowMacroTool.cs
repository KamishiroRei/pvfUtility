using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Views.Macro;

public class WindowMacroTool : ThemedWindow, IComponentConnector
{
	internal WindowMacroTool win;

	private bool VdmhFYAY8G;

	public WindowMacroToolViewModel VM => (WindowMacroToolViewModel)base.DataContext;

	public WindowMacroTool(bool isTreeList, MacroType? macroType = null)
	{
		WindowMacroToolViewModel windowMacroToolViewModel = (WindowMacroToolViewModel)(base.DataContext = new WindowMacroToolViewModel(base.Close, isTreeList, macroType));
		InitializeComponent();
		windowMacroToolViewModel.Win = this;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		base.OnClosing(e);
		WindowMacroToolViewModel windowMacroToolViewModel = (WindowMacroToolViewModel)base.DataContext;
		if (windowMacroToolViewModel.IsUpdate && AppCore.Logger.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_MacroNotSaved")) != MessageResult.Yes)
		{
			e.Cancel = true;
			return;
		}
		windowMacroToolViewModel.Dispose();
		Application.Current.MainWindow.Activate();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!VdmhFYAY8G)
		{
			VdmhFYAY8G = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/macro/windowmacrotool.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			win = (WindowMacroTool)target;
		}
		else
		{
			VdmhFYAY8G = true;
		}
	}
}
