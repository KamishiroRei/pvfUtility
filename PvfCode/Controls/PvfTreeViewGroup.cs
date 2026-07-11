using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors;
using PvfCode.ViewModels;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Controls;

public class PvfTreeViewGroup : UserControl, IComponentConnector
{
	public static readonly DependencyProperty TreeAllowDropProperty;

	internal PvfTreeViewGroup treeGroup;

	internal PopupBaseEdit findKeywordText;

	internal ToolBarControl ToolBarCloseSearchPane;

	internal PvfTreeView treeListControlEx;

	private bool InDakhCexa;

	public bool TreeAllowDrop
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(TreeAllowDropProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TreeAllowDropProperty, (object)value);
		}
	}

	public PvfTreeViewGroup()
	{
		InitializeComponent();
	}

	private void gj0aJkKida(object P_0, RoutedEventArgs P_1)
	{
		PvfTreeViewModel pvfTreeViewModel = (PvfTreeViewModel)base.DataContext;
		if (pvfTreeViewModel != null && pvfTreeViewModel.TreeType == TreeViewType.FileList)
		{
			ToolBarCloseSearchPane.Visibility = Visibility.Collapsed;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!InDakhCexa)
		{
			InDakhCexa = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/pvftreeviewgroup.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
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
			treeGroup = (PvfTreeViewGroup)target;
			treeGroup.Loaded += gj0aJkKida;
			break;
		case 2:
			findKeywordText = (PopupBaseEdit)target;
			break;
		case 3:
			ToolBarCloseSearchPane = (ToolBarControl)target;
			break;
		case 4:
			treeListControlEx = (PvfTreeView)target;
			break;
		default:
			InDakhCexa = true;
			break;
		}
	}

	static PvfTreeViewGroup()
	{
		TreeAllowDropProperty = DependencyProperty.Register("TreeAllowDrop", typeof(bool), typeof(PvfTreeViewGroup), new PropertyMetadata((object)false));
	}
}
