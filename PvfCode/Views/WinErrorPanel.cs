using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;

namespace PvfCode.Views;

public class WinErrorPanel : ThemedWindow, IComponentConnector
{
	internal TextEdit textError;

	private bool jUtC609c1k;

	public WinErrorPanel(string errorMess)
	{
		InitializeComponent();
		base.Title = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ErrorWhenSave"), "请发送给作者QQ：812143836");
		textError.EditValue = errorMess;
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.MaxHeight = primaryScreenHeight * 0.8;
		base.MaxWidth = primaryScreenWidth * 0.6;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!jUtC609c1k)
		{
			jUtC609c1k = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/winerrorpanel.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			textError = (TextEdit)target;
		}
		else
		{
			jUtC609c1k = true;
		}
	}
}
