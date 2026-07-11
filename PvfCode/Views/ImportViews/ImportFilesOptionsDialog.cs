using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.ImportViews;

public class ImportFilesOptionsDialog : ThemedWindow, IComponentConnector
{
	internal bool DVqhc7IK51;

	internal ImportFilesOptionsDialog win;

	internal TextBlock TextMessage;

	internal Button BtnImport;

	internal Button BtnCancel;

	internal Button BtnChangedOptions;

	private bool GSmh8hY1jC;

	public ImportFilesOptionsDialog(string message)
	{
		InitializeComponent();
		TextMessage.Text = message;
	}

	private void Jg2hXBl6He(object P_0, RoutedEventArgs P_1)
	{
		base.DialogResult = true;
	}

	private void XmNhpV79bO(object P_0, RoutedEventArgs P_1)
	{
		base.DialogResult = false;
	}

	private void F3ZhU97Q3L(object P_0, RoutedEventArgs P_1)
	{
		DVqhc7IK51 = true;
		base.DialogResult = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!GSmh8hY1jC)
		{
			GSmh8hY1jC = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/importviews/importfilesoptionsdialog.xaml", UriKind.Relative);
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
			win = (ImportFilesOptionsDialog)target;
			break;
		case 2:
			TextMessage = (TextBlock)target;
			break;
		case 3:
			BtnImport = (Button)target;
			BtnImport.Click += Jg2hXBl6He;
			break;
		case 4:
			BtnCancel = (Button)target;
			BtnCancel.Click += XmNhpV79bO;
			break;
		case 5:
			BtnChangedOptions = (Button)target;
			BtnChangedOptions.Click += F3ZhU97Q3L;
			break;
		default:
			GSmh8hY1jC = true;
			break;
		}
	}
}
