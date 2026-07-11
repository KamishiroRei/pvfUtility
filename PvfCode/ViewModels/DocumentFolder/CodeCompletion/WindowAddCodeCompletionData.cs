using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using PvfCode.Models.CodeCompletionModels;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class WindowAddCodeCompletionData : ThemedWindow, IComponentConnector
{
	internal CheckBox checkIsShare;

	private bool l0wuq2Gb5e;

	public WindowAddCodeCompletionData(CodeCompletionData data = null, bool isAdd = true)
	{
		base.DataContext = new WindowAddCodeCompletionDataViewModel(base.Close, data, isAdd);
		InitializeComponent();
		base.Closing += KpmuLycHPT;
	}

	private void KpmuLycHPT(object? sender, CancelEventArgs P_1)
	{
		Application.Current.MainWindow.Activate();
	}

	private void gnQungyQiK(object P_0, RoutedEventArgs P_1)
	{
		Close();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!l0wuq2Gb5e)
		{
			l0wuq2Gb5e = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/viewmodels/documentfolder/codecompletion/windowaddcodecompletiondata.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			checkIsShare = (CheckBox)target;
			break;
		case 2:
			((Button)target).Click += gnQungyQiK;
			break;
		default:
			l0wuq2Gb5e = true;
			break;
		}
	}
}
