using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Grid;

namespace PvfCode.Views.GameImagesExtract;

public class WindowGameImageExtract : ThemedWindow, IComponentConnector
{
	public const string WindowTitle = "游戏图像资源工具";

	internal WindowGameImageExtract win;

	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal GridControl logListGrid;

	internal GridControl ImgListGrid;

	private bool DYnvWp7f0K;

	public WindowGameImageExtract()
	{
		InitializeComponent();
		Title = WindowTitle;
		if (logListGrid.Columns.Count >= 3)
		{
			logListGrid.Columns[0].Header = "图像路径";
			logListGrid.Columns[1].Header = "当前值";
			logListGrid.Columns[2].Header = "全部值";
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!DYnvWp7f0K)
		{
			DYnvWp7f0K = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/gameimagesextract/windowgameimageextract.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			win = (WindowGameImageExtract)target;
			break;
		case 2:
			DemoDockContainer = (DockLayoutManager)target;
			break;
		case 3:
			Root = (LayoutGroup)target;
			break;
		case 4:
			logListGrid = (GridControl)target;
			break;
		case 5:
			ImgListGrid = (GridControl)target;
			break;
		default:
			DYnvWp7f0K = true;
			break;
		}
	}
}
