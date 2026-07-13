using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
	private readonly string? rootPath;

	public string FullPpath;

	internal Label labelRootPath;

	internal EditBox TextFileName;

	internal CheckBox newFileRegLst;

	internal Button BtnSave;

	internal Button BtnCancel;

	private bool contentLoaded;

	public bool NewFileRegLstFile { get; set; }

	public WinAddNewPvfFile(string? rootPath)
	{
		this.rootPath = rootPath;
		if (!string.IsNullOrEmpty(rootPath))
		{
			this.rootPath += "/";
		}
		else
		{
			this.rootPath = string.Empty;
		}
		InitializeComponent();
		labelRootPath.Content = this.rootPath;
		TextFileName.input.EditValueChanged += OnFileNameChanged;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		TextFileName.input.EditValueChanged -= OnFileNameChanged;
		Application.Current.MainWindow.Activate();
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		TextFileName.input.Focus();
	}

	private void OnFileNameChanged(object sender, EditValueChangedEventArgs e)
	{
		BtnSave.IsEnabled = !string.IsNullOrEmpty(TextFileName.Value);
	}

	private void OnSaveClick(object sender, RoutedEventArgs e)
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
		FullPpath = Path.Combine(rootPath, value).ToLower();
		if (AppCore.ViewModelBase.PVF.FileAny(FullPpath))
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileAlreadyExists_3"), FullPpath), isError: true);
			return;
		}
		NewFileRegLstFile = newFileRegLst.IsChecked.Value;
		base.DialogResult = true;
	}

	private void OnCancelClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
	}

	private void OnKeyDown(object sender, KeyEventArgs e)
	{
		if ((int)e.Key == 6)
		{
			OnSaveClick(null, null);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!contentLoaded)
		{
			contentLoaded = true;
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
			((WinAddNewPvfFile)target).KeyDown += OnKeyDown;
			((WinAddNewPvfFile)target).Loaded += OnLoaded;
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
			BtnSave.Click += OnSaveClick;
			break;
		case 6:
			BtnCancel = (Button)target;
			BtnCancel.Click += OnCancelClick;
			break;
		default:
			contentLoaded = true;
			break;
		}
	}
}
