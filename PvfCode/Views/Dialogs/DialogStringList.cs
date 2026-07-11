using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.Dialogs;

public class DialogStringList : ThemedWindow, IComponentConnector
{
	[CompilerGenerated]
	private List<string> DgDvuxBjaN;

	[CompilerGenerated]
	private DialogStringListViewModelResult oqyvGo6AmA;

	internal Button BtnYes;

	internal Button BtnNo;

	internal Button BtnCancel;

	private bool DdJvxlp7LY;

	public List<string> StringList
	{
		[CompilerGenerated]
		get
		{
			return DgDvuxBjaN;
		}
		[CompilerGenerated]
		set
		{
			DgDvuxBjaN = value;
		}
	}

	public DialogStringListViewModelResult Result
	{
		[CompilerGenerated]
		get
		{
			return oqyvGo6AmA;
		}
		[CompilerGenerated]
		set
		{
			oqyvGo6AmA = value;
		}
	}

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

	private void PNBvYKJ98A(object P_0, RoutedEventArgs P_1)
	{
		Result = DialogStringListViewModelResult.Yes;
		Close();
	}

	private void UyevyXyyef(object P_0, RoutedEventArgs P_1)
	{
		Result = DialogStringListViewModelResult.No;
		Close();
	}

	private void qXLviQVML6(object P_0, RoutedEventArgs P_1)
	{
		Result = DialogStringListViewModelResult.Cancel;
		Close();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!DdJvxlp7LY)
		{
			DdJvxlp7LY = true;
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
			BtnYes.Click += PNBvYKJ98A;
			break;
		case 2:
			BtnNo = (Button)target;
			BtnNo.Click += UyevyXyyef;
			break;
		case 3:
			BtnCancel = (Button)target;
			BtnCancel.Click += qXLviQVML6;
			break;
		default:
			DdJvxlp7LY = true;
			break;
		}
	}
}
