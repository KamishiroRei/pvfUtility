using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using PvfCode.Controls;

namespace PvfCode.Views.PvfTreeFolder;

public class WinAddNewPvfFile : ThemedWindow, IComponentConnector
{
	private readonly string? fi0H9cKXUE;

	[CompilerGenerated]
	private bool f9PHPEV0Hb;

	public string FullPpath;

	internal Label labelRootPath;

	internal EditBox TextFileName;

	internal CheckBox newFileRegLst;

	internal Button BtnSave;

	internal Button BtnCancel;

	private bool Ry6HZIbid0;

	public bool NewFileRegLstFile
	{
		[CompilerGenerated]
		get
		{
			return f9PHPEV0Hb;
		}
		[CompilerGenerated]
		set
		{
			f9PHPEV0Hb = value;
		}
	}

	public WinAddNewPvfFile(string? rootPath)
	{
		fi0H9cKXUE = rootPath;
		if (!string.IsNullOrEmpty(rootPath))
		{
			fi0H9cKXUE += "/";
		}
		else
		{
			fi0H9cKXUE = string.Empty;
		}
		InitializeComponent();
		labelRootPath.Content = fi0H9cKXUE;
		TextFileName.input.EditValueChanged += SwkHIZEqJO;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		TextFileName.input.EditValueChanged -= SwkHIZEqJO;
		Application.Current.MainWindow.Activate();
	}

	private void UGAHbpujwG(object P_0, RoutedEventArgs P_1)
	{
		TextFileName.input.Focus();
	}

	private void SwkHIZEqJO(object P_0, EditValueChangedEventArgs P_1)
	{
		BtnSave.IsEnabled = !string.IsNullOrEmpty(TextFileName.Value);
	}

	private void BxFHEIn6jH(object P_0, RoutedEventArgs P_1)
	{
		string value = TextFileName.Value;
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		if (value == "/")
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_InvalidFileName"));
			return;
		}
		value = value.Replace("\\", "/");
		if (value[0] == '/')
		{
			value = value.Substring(1, value.Length - 1);
		}
		if (string.IsNullOrEmpty(Path.GetExtension(value)))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNameMustHaveExtension"), isError: true);
			return;
		}
		FullPpath = Path.Combine(fi0H9cKXUE, value).ToLower();
		if (AppCore.ViewModelBase.PVF.FileAny(FullPpath))
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileAlreadyExists_3"), FullPpath), isError: true);
			return;
		}
		NewFileRegLstFile = newFileRegLst.IsChecked.Value;
		base.DialogResult = true;
	}

	private void dS5HOybWnc(object P_0, RoutedEventArgs P_1)
	{
		base.DialogResult = false;
	}

	private void WXIHKUVfof(object P_0, KeyEventArgs P_1)
	{
		if ((int)P_1.Key == 6)
		{
			BxFHEIn6jH(null, null);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!Ry6HZIbid0)
		{
			Ry6HZIbid0 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/pvftreefolder/winaddnewpvffile.xaml", UriKind.Relative);
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
			((WinAddNewPvfFile)target).KeyDown += WXIHKUVfof;
			((WinAddNewPvfFile)target).Loaded += UGAHbpujwG;
			break;
		case 2:
			labelRootPath = (Label)target;
			break;
		case 3:
			TextFileName = (EditBox)target;
			break;
		case 4:
			newFileRegLst = (CheckBox)target;
			break;
		case 5:
			BtnSave = (Button)target;
			BtnSave.Click += BxFHEIn6jH;
			break;
		case 6:
			BtnCancel = (Button)target;
			BtnCancel.Click += dS5HOybWnc;
			break;
		default:
			Ry6HZIbid0 = true;
			break;
		}
	}
}
