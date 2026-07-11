using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Docking;
using PvfCode.ViewModels.LstTools;

namespace PvfCode.Views.LstTools;

public class WinLstTools : WindowBase, IComponentConnector
{
	internal LayoutGroup Root;

	internal LayoutGroup rightRoot;

	private bool dRihOhYfT5;

	public WinLstTools(KeyValuePair<string, string>? selectedItem = null)
	{
		base.DataContext = new WinLstToolsViewModel(selectedItem);
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Width = primaryScreenWidth * 0.5;
		base.Height = primaryScreenHeight * 0.65;
	}

	private void WinLstTools_Loaded(object sender, RoutedEventArgs e)
	{
		((WinLstToolsViewModel)base.DataContext).Loaded();
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		base.OnClosing(e);
		Application.Current.MainWindow.Activate();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!dRihOhYfT5)
		{
			dRihOhYfT5 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/lsttools/winlsttools.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			Root = (LayoutGroup)target;
			break;
		case 2:
			rightRoot = (LayoutGroup)target;
			break;
		default:
			dRihOhYfT5 = true;
			break;
		}
	}
}
