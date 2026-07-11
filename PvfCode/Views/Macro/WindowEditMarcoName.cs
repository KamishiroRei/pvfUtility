using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using PvfCode.Controls;

namespace PvfCode.Views.Macro;

public class WindowEditMarcoName : ThemedWindow, IComponentConnector
{
	internal EditBox Input;

	private bool BDEhBIMZ29;

	public WindowEditMarcoName(string title, string dirName)
	{
		InitializeComponent();
		base.Title = title;
		Input.Value = dirName;
	}

	private void z3JhHnWjuM(object P_0, RoutedEventArgs P_1)
	{
		Input.input.Focus();
		if (Input.Value != null)
		{
			Input.input.SelectAll();
		}
	}

	private void QIRhhbLOEV(object P_0, RoutedEventArgs P_1)
	{
		if (string.IsNullOrEmpty(Input.Value))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputFolderName"));
		}
		else
		{
			base.DialogResult = true;
		}
	}

	private void ytMhvtnmwg(object P_0, RoutedEventArgs P_1)
	{
		base.DialogResult = false;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!BDEhBIMZ29)
		{
			BDEhBIMZ29 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/macro/windoweditmarconame.xaml", UriKind.Relative);
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
			((WindowEditMarcoName)target).Loaded += z3JhHnWjuM;
			break;
		case 2:
			Input = (EditBox)target;
			break;
		case 3:
			((Button)target).Click += QIRhhbLOEV;
			break;
		case 4:
			((Button)target).Click += ytMhvtnmwg;
			break;
		default:
			BDEhBIMZ29 = true;
			break;
		}
	}
}
