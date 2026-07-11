using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PvfCode.ViewModels.independent_drop;

namespace PvfCode.Views.independent_drop;

public class ViewIndependent_drop : ThemedWindow, IComponentConnector
{
	internal ViewIndependent_drop win;

	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal ToolBarControl toolbarControl;

	internal GridControl gridMonster;

	internal ButtonEdit buttonEdit_MosterOrApcId;

	internal ButtonEdit buttonEditItemCode;

	internal ToolBarControl dropItemCodesControlBar;

	private bool gCOh7aM4p9;

	public ViewIndependent_drop()
	{
		Independent_drop_ViewModel dataContext = new Independent_drop_ViewModel(base.Close);
		base.DataContext = dataContext;
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Width = primaryScreenWidth * 0.6;
		base.Height = primaryScreenHeight * 0.6;
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		Application.Current.MainWindow?.Activate();
		((Independent_drop_ViewModel)base.DataContext).Dispose();
		base.DataContext = null;
		GC.Collect(2, GCCollectionMode.Optimized);
	}

	private void vVrh0001Ir(object P_0, ItemCancelEventArgs P_1)
	{
		P_1.Cancel = true;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!gCOh7aM4p9)
		{
			gCOh7aM4p9 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/independent_drop/viewindependent_drop.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
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
			win = (ViewIndependent_drop)target;
			break;
		case 2:
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += vVrh0001Ir;
			break;
		case 3:
			Root = (LayoutGroup)target;
			break;
		case 4:
			toolbarControl = (ToolBarControl)target;
			break;
		case 5:
			gridMonster = (GridControl)target;
			break;
		case 6:
			buttonEdit_MosterOrApcId = (ButtonEdit)target;
			break;
		case 7:
			buttonEditItemCode = (ButtonEdit)target;
			break;
		case 8:
			dropItemCodesControlBar = (ToolBarControl)target;
			break;
		default:
			gCOh7aM4p9 = true;
			break;
		}
	}
}
