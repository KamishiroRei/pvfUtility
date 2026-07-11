using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using PvfCode.ViewModels.Login;

namespace PvfCode.Views.UserViews;

public class WindowEditUserInfo : ThemedWindow, IComponentConnector
{
	private bool YvECoO3mWN;

	public WindowEditUserInfo()
	{
		base.DataContext = new WindowEditUserInfoViewModel(base.Close);
		InitializeComponent();
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		Application.Current.MainWindow.Activate();
	}

	private void U46CwktZxD(object P_0, RoutedEventArgs P_1)
	{
		Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!YvECoO3mWN)
		{
			YvECoO3mWN = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/userviews/windowedituserinfo.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
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
		if (connectionId == 1)
		{
			((Button)target).Click += U46CwktZxD;
		}
		else
		{
			YvECoO3mWN = true;
		}
	}
}
