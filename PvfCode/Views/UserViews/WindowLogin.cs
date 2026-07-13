using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.UserViews;

public class WindowLogin : ThemedWindow, IComponentConnector
{
	internal Grid loginView;

	internal Grid Reg;

	private bool _contentLoaded;

	public WindowLogin()
	{
		AppCore.ViewModelBase.LoginViewModel.Close = base.Close;
		base.DataContext = AppCore.ViewModelBase.LoginViewModel;
		InitializeComponent();
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		AppCore.ViewModelBase.LoginViewModel.Loaded(this);
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		Application.Current.MainWindow.Activate();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/userviews/windowlogin.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
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
			((WindowLogin)target).Loaded += OnLoaded;
			break;
		case 2:
			loginView = (Grid)target;
			break;
		case 3:
			Reg = (Grid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
