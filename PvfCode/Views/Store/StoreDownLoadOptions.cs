using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.Store;

public class StoreDownLoadOptions : ThemedWindow, IComponentConnector
{
	internal RadioButton check1;

	internal RadioButton check2;

	internal Button ButtonOk;

	private bool vNBCZDEtRB;

	[DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
	private static extern int AUICOX9SjS(IntPtr P_0, int P_1);

	[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
	private static extern int liPCK4aQIW(IntPtr P_0, int P_1, int P_2);

	public StoreDownLoadOptions(string content1, string content2)
	{
		InitializeComponent();
		check1.Content = content1;
		check2.Content = content2;
		check1.IsChecked = true;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		Application.Current.MainWindow.Activate();
		base.DialogResult = check1.IsChecked;
	}

	private void qX6C98CVQV(object P_0, RoutedEventArgs P_1)
	{
		base.DialogResult = check1.IsChecked;
	}

	private void s8oCP90cdo(object P_0, RoutedEventArgs P_1)
	{
		IntPtr handle = new WindowInteropHelper(this).Handle;
		liPCK4aQIW(handle, -16, AUICOX9SjS(handle, -16) & -524289);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!vNBCZDEtRB)
		{
			vNBCZDEtRB = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/store/storedownloadoptions.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((StoreDownLoadOptions)target).Loaded += s8oCP90cdo;
			break;
		case 2:
			check1 = (RadioButton)target;
			break;
		case 3:
			check2 = (RadioButton)target;
			break;
		case 4:
			ButtonOk = (Button)target;
			ButtonOk.Click += qX6C98CVQV;
			break;
		default:
			vNBCZDEtRB = true;
			break;
		}
	}
}
