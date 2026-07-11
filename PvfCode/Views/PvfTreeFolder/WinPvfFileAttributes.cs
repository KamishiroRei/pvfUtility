using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.PvfTreeFolder;

public class WinPvfFileAttributes : ThemedWindow, IComponentConnector
{
	private bool EqWHJpePHU;

	public WinPvfFileAttributes(PvfTreeFileBase treeFilefile)
	{
		base.DataContext = new WinPvfFileAttributesViewModel(treeFilefile);
		InitializeComponent();
		_ = SystemParameters.PrimaryScreenHeight;
		_ = SystemParameters.PrimaryScreenWidth;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		Application.Current.MainWindow.Activate();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!EqWHJpePHU)
		{
			EqWHJpePHU = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/pvftreefolder/winpvffileattributes.cs.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		EqWHJpePHU = true;
	}
}
