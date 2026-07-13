using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Editors;

namespace PvfCode.Views;

public class ViewExtractFiles : ThemedWindow, IComponentConnector
{
	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal ToolBarControl toolbarControl;

	internal ComboBoxEdit comboBoxEditFileTypesString;

	internal CheckBox checkAutoImportFileGroup;

	private bool _contentLoaded;

	public ViewExtractFiles(IEnumerable<string> targetExtractFiles = null)
	{
		base.DataContext = new ViewExtractFilesViewModels(base.Close, targetExtractFiles);
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		_ = SystemParameters.PrimaryScreenWidth;
		base.MaxHeight = primaryScreenHeight;
	}

	public ViewExtractFiles(PvfGroup pvf, IEnumerable<string> targetExtractFiles)
	{
		base.DataContext = new ViewExtractFilesViewModels(base.Close, pvf, targetExtractFiles);
		InitializeComponent();
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		((ViewExtractFilesViewModels)base.DataContext)?.Dispose();
		base.DataContext = null;
		Application.Current.MainWindow.Activate();
	}

	private void OnDockItemClosing(object sender, ItemCancelEventArgs e)
	{
		e.Cancel = true;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/viewextractfiles.xaml", UriKind.Relative);
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
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += OnDockItemClosing;
			break;
		case 2:
			Root = (LayoutGroup)target;
			break;
		case 3:
			toolbarControl = (ToolBarControl)target;
			break;
		case 4:
			comboBoxEditFileTypesString = (ComboBoxEdit)target;
			break;
		case 5:
			checkAutoImportFileGroup = (CheckBox)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
