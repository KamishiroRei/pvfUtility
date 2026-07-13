using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using PvfCode.ViewModels.Store.Preview;

namespace PvfCode.Views.Store.Preview;

public class PreviewItemCodeHoverView : ThemedWindow, IComponentConnector
{
	private bool _contentLoaded;

	public PreviewItemCodeHoverView(string text)
	{
		base.DataContext = new PreviewItemCodeHoverViewViewModel(text);
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Width = primaryScreenWidth * 0.45;
		base.Height = primaryScreenHeight * 0.6;
	}

	private void OnClosed(object sender, EventArgs e)
	{
		Application.Current.MainWindow.Activate();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/store/preview/previewitemcodehoverview.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			((PreviewItemCodeHoverView)target).Closed += OnClosed;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
