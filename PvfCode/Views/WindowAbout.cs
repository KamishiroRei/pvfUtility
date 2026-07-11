using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using PvfCode.ViewModels;

namespace PvfCode.Views;

public class WindowAbout : ThemedWindow, IComponentConnector
{
	internal Label labelVersion;

	internal ListBox listLibs;

	internal ListBox DownLoadListBox;

	private bool WAcCWionhK;

	public WindowAbout()
	{
		base.DataContext = new WindowAboutViewModel();
		InitializeComponent();
		labelVersion.Content = Application.ResourceAssembly.GetName().Version.ToString();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Height = primaryScreenHeight * 0.4;
		base.Width = primaryScreenWidth * 0.4;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!WAcCWionhK)
		{
			WAcCWionhK = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/windowabout.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			labelVersion = (Label)target;
			break;
		case 2:
			listLibs = (ListBox)target;
			break;
		case 3:
			DownLoadListBox = (ListBox)target;
			break;
		default:
			WAcCWionhK = true;
			break;
		}
	}
}
