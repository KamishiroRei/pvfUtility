using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using PvfCode.ViewModels.SearchPvf;

namespace PvfCode.Views;

public class WindowItemCodeSearch : ThemedWindow, IComponentConnector
{
	internal LayoutGroup Root;

	private bool N73TVQ8YFk;

	public WindowItemCodeSearch()
	{
		base.DataContext = new WindowItemCodeSearchViewModel();
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Height = primaryScreenHeight * 0.6;
		base.Width = primaryScreenWidth * 0.6;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		Application.Current.MainWindow.Activate();
	}

	public void GridControl_SelectionChanged(object sender)
	{
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!N73TVQ8YFk)
		{
			N73TVQ8YFk = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/searchpvf/windowitemcodesearch.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			Root = (LayoutGroup)target;
		}
		else
		{
			N73TVQ8YFk = true;
		}
	}
}
