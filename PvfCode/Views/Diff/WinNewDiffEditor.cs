using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using PvfCode.ViewModels.Diff;

namespace PvfCode.Views.Diff;

public class WinNewDiffEditor : WindowBase, IComponentConnector
{
	internal Grid leftGrid;

	internal ButtonEdit leftButtonEdit;

	private bool yvGvS906VJ;

	public WinNewDiffEditor(WinNewDiffViewModel? winNewDiffViewModel = null)
	{
		if (winNewDiffViewModel == null)
		{
			winNewDiffViewModel = new WinNewDiffViewModel();
		}
		base.DataContext = winNewDiffViewModel;
		InitializeComponent();
		_ = SystemParameters.PrimaryScreenHeight;
		_ = SystemParameters.PrimaryScreenWidth;
		Window mainWindow = Application.Current.MainWindow;
		base.Height = mainWindow.Height * 0.8;
		base.Width = mainWindow.Width * 0.8;
	}

	protected override void OnClosed(EventArgs e)
	{
		((WinNewDiffViewModel)base.DataContext)?.Dispose();
		AppCore.ViewModelBase.BarsVm.DiffIsOpen = false;
		Application.Current.MainWindow.Activate();
	}

	private void WinNewDiffEditor_StateChanged(object? sender, EventArgs e)
	{
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			leftGrid.Width = base.ActualWidth / 2.0 - 18.0;
		}, Array.Empty<object>());
	}

	private void WinNewDiffEditor_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		leftGrid.Width = base.Width / 2.0 - 18.0;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!yvGvS906VJ)
		{
			yvGvS906VJ = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/diff/winnewdiffeditor.xaml", UriKind.Relative);
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
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			leftGrid = (Grid)target;
			break;
		case 2:
			leftButtonEdit = (ButtonEdit)target;
			break;
		default:
			yvGvS906VJ = true;
			break;
		}
	}

	[CompilerGenerated]
	private void TBrv5Cfcil()
	{
		leftGrid.Width = base.ActualWidth / 2.0 - 18.0;
	}
}
