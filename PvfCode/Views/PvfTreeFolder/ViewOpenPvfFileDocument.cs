using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using PvfCode.Controls;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewOpenPvfFileDocument : ThemedWindow, IComponentConnector
{
	internal EditBox editBox;

	internal CheckBox CheckGoToNode;

	internal Button BtnOk;

	internal Button BtnCancel;

	private bool NrkHyE2xJb;

	public ViewOpenPvfFileDocument()
	{
		InitializeComponent();
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		Application.Current.MainWindow.Activate();
	}

	private void IaGHSBC9Py(object P_0, RoutedEventArgs P_1)
	{
		editBox.input.Focus();
	}

	private void RniH4elEmv(object P_0, RoutedEventArgs P_1)
	{
		string value = editBox.Value;
		if (!string.IsNullOrEmpty(value))
		{
			value = value.Trim().ToLower();
			if (!AppCore.ViewModelBase.PVF.FileAny(value))
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), value), isError: true);
				return;
			}
			AppCore.ViewModelBase.RootDocument.AddDocument(value, CheckGoToNode.IsChecked.Value);
			Close();
		}
	}

	private void xFlHY1gIQ0(object P_0, RoutedEventArgs P_1)
	{
		Close();
	}

	private void CheckGoToNode_KeyDown(object sender, KeyEventArgs e)
	{
		if ((int)e.Key == 6)
		{
			RniH4elEmv(null, null);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!NrkHyE2xJb)
		{
			NrkHyE2xJb = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/pvftreefolder/viewopenpvffiledocument.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((ViewOpenPvfFileDocument)target).Loaded += IaGHSBC9Py;
			break;
		case 2:
			editBox = (EditBox)target;
			break;
		case 3:
			CheckGoToNode = (CheckBox)target;
			break;
		case 4:
			BtnOk = (Button)target;
			BtnOk.Click += RniH4elEmv;
			break;
		case 5:
			BtnCancel = (Button)target;
			BtnCancel.Click += xFlHY1gIQ0;
			break;
		default:
			NrkHyE2xJb = true;
			break;
		}
	}
}
