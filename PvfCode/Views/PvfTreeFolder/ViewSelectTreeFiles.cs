using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewSelectTreeFiles : ThemedWindow, IComponentConnector
{
	internal ViewSelectTreeFilesViewModel ViewModel;

	private bool _contentLoaded;

	public ViewSelectTreeFiles(TreeViewType sourceType)
	{
		ViewModel = new ViewSelectTreeFilesViewModel(sourceType, base.Close);
		base.DataContext = ViewModel;
		InitializeComponent();
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		Application.Current.MainWindow.Activate();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/pvftreefolder/viewselecttreefiles.xaml", UriKind.Relative);
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
		_contentLoaded = true;
	}
}
