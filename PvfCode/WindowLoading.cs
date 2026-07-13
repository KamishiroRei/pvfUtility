using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode;

public class WindowLoading : ThemedWindow, IComponentConnector
{
	internal TextBlock title;

	private bool _contentLoaded;

	public WindowLoading(string text)
	{
		base.DataContext = new WindowLoadingViewModel(text);
		InitializeComponent();
	}

	public WindowLoading(string text, CancellationTokenSource cancellationTokenSource)
	{
		base.DataContext = new WindowLoadingViewModel(text, cancellationTokenSource);
		InitializeComponent();
	}

	private void OnKeyDown(object sender, KeyEventArgs e)
	{
		if ((int)e.Key == 13)
		{
			Close();
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/windowloading.xaml", UriKind.Relative);
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
			((WindowLoading)target).KeyDown += OnKeyDown;
			break;
		case 2:
			title = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
