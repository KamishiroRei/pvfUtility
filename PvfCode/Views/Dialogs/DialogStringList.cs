using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.Dialogs;

public class DialogStringList : ThemedWindow, IComponentConnector
{
	internal Button BtnYes;

	internal Button BtnNo;

	internal Button BtnCancel;

	private bool contentLoaded;

	public List<string> StringList { get; set; }

	public DialogStringListViewModelResult Result { get; set; }

	public DialogStringList(DialogStringListViewModel vm)
	{
		Result = DialogStringListViewModelResult.Cancel;
		base.DataContext = vm;
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.MaxHeight = primaryScreenHeight * 0.6;
		base.MaxWidth = primaryScreenWidth * 0.8;
	}

	private void OnYesClick(object sender, RoutedEventArgs e)
	{
		Result = DialogStringListViewModelResult.Yes;
		Close();
	}

	private void OnNoClick(object sender, RoutedEventArgs e)
	{
		Result = DialogStringListViewModelResult.No;
		Close();
	}

	private void OnCancelClick(object sender, RoutedEventArgs e)
	{
		Result = DialogStringListViewModelResult.Cancel;
		Close();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!contentLoaded)
		{
			contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/dialogs/dialogstringlist.xaml", UriKind.Relative);
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
			BtnYes = (Button)target;
			BtnYes.Click += OnYesClick;
			break;
		case 2:
			BtnNo = (Button)target;
			BtnNo.Click += OnNoClick;
			break;
		case 3:
			BtnCancel = (Button)target;
			BtnCancel.Click += OnCancelClick;
			break;
		default:
			contentLoaded = true;
			break;
		}
	}
}
