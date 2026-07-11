using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Office.UI;

namespace PvfCode.Views.AniDesigner;

public class WindowAniDesigner : ThemedWindow, IComponentConnector
{
	internal WindowAniDesigner win;

	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal BarSplitButtonColorEditItem biFormatParagraphBackColor;

	internal ColorEdit colorEdit;

	internal BarCheckItem showImageBorder;

	internal BarSplitButtonColorEditItem biFormatParagraphBorderColor;

	internal ColorEdit colorEditImageBorder;

	private bool bL1BiaMe3x;

	public WindowAniDesigner()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!bL1BiaMe3x)
		{
			bL1BiaMe3x = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/anidesigner/windowanidesigner.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			win = (WindowAniDesigner)target;
			break;
		case 2:
			DemoDockContainer = (DockLayoutManager)target;
			break;
		case 3:
			Root = (LayoutGroup)target;
			break;
		case 4:
			biFormatParagraphBackColor = (BarSplitButtonColorEditItem)target;
			break;
		case 5:
			colorEdit = (ColorEdit)target;
			break;
		case 6:
			showImageBorder = (BarCheckItem)target;
			break;
		case 7:
			biFormatParagraphBorderColor = (BarSplitButtonColorEditItem)target;
			break;
		case 8:
			colorEditImageBorder = (ColorEdit)target;
			break;
		default:
			bL1BiaMe3x = true;
			break;
		}
	}
}
