using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.BookMark;

public class WinDownLoadBookMarkOptions : ThemedWindow, IComponentConnector
{
	internal RadioButton mergeRadio;

	internal RadioButton coverRadio;

	internal Button btnok;

	private bool YRDvUpn3ki;

	public WinDownLoadBookMarkOptions()
	{
		InitializeComponent();
	}

	private void eqPvpWsAqx(object P_0, RoutedEventArgs P_1)
	{
		base.DialogResult = mergeRadio.IsChecked;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!YRDvUpn3ki)
		{
			YRDvUpn3ki = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/bookmark/windownloadbookmarkoptions.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			mergeRadio = (RadioButton)target;
			break;
		case 2:
			coverRadio = (RadioButton)target;
			break;
		case 3:
			btnok = (Button)target;
			btnok.Click += eqPvpWsAqx;
			break;
		default:
			YRDvUpn3ki = true;
			break;
		}
	}
}
