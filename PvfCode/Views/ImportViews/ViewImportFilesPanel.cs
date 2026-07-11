using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Editors;
using PvfCode.Controls;

namespace PvfCode.Views.ImportViews;

public class ViewImportFilesPanel : UserControl, IComponentConnector
{
	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal ToolBarControl toolbarControl;

	internal PvfTreeViewGroup TreeGroupPanel;

	internal ButtonEdit textTargetPath;

	internal ComboBoxEdit comboBoxEditFileTypesString;

	private bool tdrhR42NlB;

	public ViewImportFilesPanel()
	{
		InitializeComponent();
	}

	private void W1EhMvAw07(object P_0, ItemCancelEventArgs P_1)
	{
		P_1.Cancel = true;
	}

	private void kLihVp1GLH(object P_0, RoutedEventArgs P_1)
	{
		comboBoxEditFileTypesString.EditValue = null;
	}

	private void gswh3qKupf(object P_0, RoutedEventArgs P_1)
	{
		textTargetPath.EditValue = null;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!tdrhR42NlB)
		{
			tdrhR42NlB = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/importviews/viewimportfilespanel.xaml", UriKind.Relative);
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
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += W1EhMvAw07;
			break;
		case 2:
			Root = (LayoutGroup)target;
			break;
		case 3:
			toolbarControl = (ToolBarControl)target;
			break;
		case 4:
			TreeGroupPanel = (PvfTreeViewGroup)target;
			break;
		case 5:
			textTargetPath = (ButtonEdit)target;
			break;
		case 6:
			((ButtonInfo)target).Click += gswh3qKupf;
			break;
		case 7:
			comboBoxEditFileTypesString = (ComboBoxEdit)target;
			break;
		case 8:
			((ButtonInfo)target).Click += kLihVp1GLH;
			break;
		default:
			tdrhR42NlB = true;
			break;
		}
	}
}
