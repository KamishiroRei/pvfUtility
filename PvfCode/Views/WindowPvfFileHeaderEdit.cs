using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views;

public class WindowPvfFileHeaderEdit : ThemedWindow, IComponentConnector
{
	internal TextBox txtGuid;

	internal TextBox txtVersion;

	internal Button save;

	internal Button cancel;

	private bool Ab9CQ49KHn;

	public WindowPvfFileHeaderEdit()
	{
		InitializeComponent();
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (pVF.PvfIsOpen)
		{
			txtGuid.Text = Encoding.UTF8.GetString(pVF.Guid);
			txtVersion.Text = pVF.FileVersion.ToString();
		}
	}

	private void GuxCGgMQO9(object P_0, RoutedEventArgs P_1)
	{
		Close();
	}

	private void qDTCxx1YCl(object P_0, RoutedEventArgs P_1)
	{
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		try
		{
			pVF.Guid = Encoding.UTF8.GetBytes(txtGuid.Text);
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg("Guid Error：" + ex.Message);
			return;
		}
		try
		{
			pVF.FileVersion = int.Parse(txtVersion.Text);
		}
		catch (Exception ex2)
		{
			AppCore.ShowMsg("Version Error：" + ex2.Message);
			return;
		}
		Close();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!Ab9CQ49KHn)
		{
			Ab9CQ49KHn = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/windowpvffileheaderedit.xaml", UriKind.Relative);
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
			txtGuid = (TextBox)target;
			break;
		case 2:
			txtVersion = (TextBox)target;
			break;
		case 3:
			save = (Button)target;
			save.Click += qDTCxx1YCl;
			break;
		case 4:
			cancel = (Button)target;
			cancel.Click += GuxCGgMQO9;
			break;
		default:
			Ab9CQ49KHn = true;
			break;
		}
	}
}
