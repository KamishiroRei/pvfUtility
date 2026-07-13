using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.Dialogs;

public class DialogPvfPackSaveOptions : ThemedWindow, IComponentConnector
{
	private bool _contentLoaded;

	public DialogPvfPackSaveOptions()
	{
		InitializeComponent();
	}

	protected override async void OnClosed(EventArgs e)
	{
		await AppSetting.Instance.SaveSetting();
		Application.Current.MainWindow.Activate();
	}

	private void OnConfirmClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/dialogs/dialogpvfpacksaveoptions.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			((Button)target).Click += OnConfirmClick;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
