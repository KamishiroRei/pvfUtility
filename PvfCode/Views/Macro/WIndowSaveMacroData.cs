using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
	private readonly MacroData TBohsE84wF;

	private readonly MacroType MacroType;

	private WIndowSaveMacroDataViewModel i8LhLQ1tvE;

	[CompilerGenerated]
	private bool cIShn0FisK;

	[CompilerGenerated]
	private string anNhqMTbiJ;

	[CompilerGenerated]
	private string jLChdAkkfs;

	private WindowMacroToolViewModel tZAheDC5m6;

	internal EditBox TextName;

	internal Grid savePathPanel;

	internal ButtonEdit btnSavePath;

	internal Button BtnSave;

	internal Button BtnCancel;

	private bool QAPhtQadN9;

	public string ShareCaption
	{
		[CompilerGenerated]
		get
		{
			return anNhqMTbiJ;
		}
		[CompilerGenerated]
		set
		{
			anNhqMTbiJ = value;
		}
	}

	public string ShareInstructions
	{
		[CompilerGenerated]
		get
		{
			return jLChdAkkfs;
		}
		[CompilerGenerated]
		set
		{
			jLChdAkkfs = value;
		}
	}

	[SpecialName]
	[CompilerGenerated]
	private bool BwFh1wakDx()
	{
		return cIShn0FisK;
	}

	[SpecialName]
	[CompilerGenerated]
	private void Dx0hwAk2Es(bool P_0)
	{
		cIShn0FisK = P_0;
	}

	public WIndowSaveMacroData(MacroData data, string title, bool showShareCheck = true, bool isShare = false)
	{
		TBohsE84wF = data;
		MacroType = data.MacroType;
		Dx0hwAk2Es(isShare);
		i8LhLQ1tvE = new WIndowSaveMacroDataViewModel();
		base.DataContext = i8LhLQ1tvE;
		InitializeComponent();
		TextName.Value = title;
	}

	protected override void OnClosed(EventArgs e)
	{
		Application.Current.MainWindow.Activate();
	}

	private void OLjhxp0MRn(object P_0, RoutedEventArgs P_1)
	{
		WindowMacroTool windowMacroTool = new WindowMacroTool(isTreeList: true, MacroType);
		windowMacroTool.Owner = this;
		windowMacroTool.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		windowMacroTool.ShowDialog();
		WindowMacroToolViewModel vM = windowMacroTool.VM;
		if (vM.IsSelect)
		{
			tZAheDC5m6 = vM;
			btnSavePath.EditValue = vM.SelectNodeToPath();
		}
	}

	private void h3YhQnAuMR(object P_0, RoutedEventArgs P_1)
	{
		base.DialogResult = false;
	}

	private async void TJ8haEtSnd(object P_0, RoutedEventArgs P_1)
	{
		if (string.IsNullOrEmpty(TextName.Value))
		{
			AppCore.ShowMsg("请先输入名称");
			return;
		}
		if (tZAheDC5m6 == null)
		{
			AppCore.ShowMsg("请先选择保存路径");
			return;
		}
		i8LhLQ1tvE.IsLoading = true;
		await tZAheDC5m6.Add(TextName.Value, TBohsE84wF);
		base.DialogResult = true;
		i8LhLQ1tvE.IsLoading = false;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!QAPhtQadN9)
		{
			QAPhtQadN9 = true;
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
			btnSavePath.DefaultButtonClick += OLjhxp0MRn;
			break;
		case 4:
			BtnSave = (Button)target;
			BtnSave.Click += TJ8haEtSnd;
			break;
		case 5:
			BtnCancel = (Button)target;
			BtnCancel.Click += h3YhQnAuMR;
			break;
		default:
			QAPhtQadN9 = true;
			break;
		}
	}

}
