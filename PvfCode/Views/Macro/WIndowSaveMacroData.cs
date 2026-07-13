using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using PvfCode.Controls;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.Macro;

namespace PvfCode.Views.Macro;

public class WIndowSaveMacroData : ThemedWindow, IComponentConnector
{
	private readonly MacroData macroData;

	private readonly MacroType MacroType;

	private WIndowSaveMacroDataViewModel viewModel;

	private WindowMacroToolViewModel selectedFolder;

	internal EditBox TextName;

	internal Grid savePathPanel;

	internal ButtonEdit btnSavePath;

	internal Button BtnSave;

	internal Button BtnCancel;

	private bool contentLoaded;

	public string ShareCaption { get; set; }

	public string ShareInstructions { get; set; }

	public WIndowSaveMacroData(MacroData data, string title, bool showShareCheck = true, bool isShare = false)
	{
		macroData = data;
		MacroType = data.MacroType;
		viewModel = new WIndowSaveMacroDataViewModel();
		base.DataContext = viewModel;
		InitializeComponent();
		TextName.Value = title;
	}

	protected override void OnClosed(EventArgs e)
	{
		Application.Current.MainWindow.Activate();
	}

	private void OnSelectSavePath(object sender, RoutedEventArgs e)
	{
		WindowMacroTool windowMacroTool = new WindowMacroTool(isTreeList: true, MacroType);
		windowMacroTool.Owner = this;
		windowMacroTool.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		windowMacroTool.ShowDialog();
		WindowMacroToolViewModel vM = windowMacroTool.VM;
		if (vM.IsSelect)
		{
			selectedFolder = vM;
			btnSavePath.EditValue = vM.SelectNodeToPath();
		}
	}

	private void OnCancelClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
	}

	private async void OnSaveClick(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(TextName.Value))
		{
			AppCore.ShowMsg("请先输入名称");
			return;
		}
		if (selectedFolder == null)
		{
			AppCore.ShowMsg("请先选择保存路径");
			return;
		}
		viewModel.IsLoading = true;
		await selectedFolder.Add(TextName.Value, macroData);
		base.DialogResult = true;
		viewModel.IsLoading = false;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!contentLoaded)
		{
			contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/macro/windowsavemacrodata.xaml", UriKind.Relative);
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
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			TextName = (EditBox)target;
			break;
		case 2:
			savePathPanel = (Grid)target;
			break;
		case 3:
			btnSavePath = (ButtonEdit)target;
			btnSavePath.DefaultButtonClick += OnSelectSavePath;
			break;
		case 4:
			BtnSave = (Button)target;
			BtnSave.Click += OnSaveClick;
			break;
		case 5:
			BtnCancel = (Button)target;
			BtnCancel.Click += OnCancelClick;
			break;
		default:
			contentLoaded = true;
			break;
		}
	}

}
