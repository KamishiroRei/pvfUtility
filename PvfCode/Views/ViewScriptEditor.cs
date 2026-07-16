using System;
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core;

namespace PvfCode.Views;

public partial class ViewScriptEditor : ThemedWindow
{
	internal ViewScriptEditor()
	{
		InitializeComponent();
	}

	public ViewScriptEditor(ViewScriptEditorViewModel vm)
	{
		base.DataContext = vm;
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Height = primaryScreenHeight * 0.6;
		base.Width = primaryScreenWidth * 0.6;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		((ViewScriptEditorViewModel)base.DataContext).Dispose();
		base.DataContext = null;
		Application.Current.MainWindow.Activate();
	}

}
