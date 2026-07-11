using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.SplashScreen;

public class PvfCodeSplashScreenWindow : SplashScreenWindow, IComponentConnector
{
	internal DXImage PART_Logo;

	internal TextBlock PART_Title;

	internal TextBlock PART_SubTitle;

	private bool xEhQACgeK9;

	public PvfCodeSplashScreenWindow()
	{
		InitializeComponent();
		PART_Title.Text = "pvfUtility";
		PART_SubTitle.Text = Application.ResourceAssembly.GetName().Version.ToString();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!xEhQACgeK9)
		{
			xEhQACgeK9 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/splashscreen/fluentsplashscreen/pvfcodesplashscreenwindow.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			PART_Logo = (DXImage)target;
			break;
		case 2:
			PART_Title = (TextBlock)target;
			break;
		case 3:
			PART_SubTitle = (TextBlock)target;
			break;
		default:
			xEhQACgeK9 = true;
			break;
		}
	}
}
