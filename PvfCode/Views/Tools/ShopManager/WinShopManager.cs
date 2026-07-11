using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.Tools.ShopManager;

public class WinShopManager : ThemedWindow, IComponentConnector
{
	private bool LmqCEbXKH9;

	public WinShopManager()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!LmqCEbXKH9)
		{
			LmqCEbXKH9 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/tools/shopmanager/winshopmanager.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		LmqCEbXKH9 = true;
	}
}
