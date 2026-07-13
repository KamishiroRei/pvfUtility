using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Mvvm;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using PvfCode.Dot.Desktop;
using PvfCode.ViewModels.BookMarkEdit;

namespace PvfCode.Views.BookMark;

public class BookMarkEditView : ThemedWindow, IComponentConnector
{
	internal BookMarkEditView win;

	internal BarContainerControl toolBar;

	internal BookMarkTreeView treeListView;

	internal DevExpress.Xpf.LayoutControl.GroupBox shareGroup;

	private bool _contentLoaded;

	public BookMarkEditView(bool isTreeList, bool isPreview = false, BookMarkGroupDto previewSource = null)
	{
		BookMarkEditViewViewModel dataContext = new BookMarkEditViewViewModel(base.Close, isTreeList, isPreview, previewSource);
		base.DataContext = dataContext;
		InitializeComponent();
		_ = SystemParameters.PrimaryScreenHeight;
		_ = SystemParameters.PrimaryScreenWidth;
		if (isPreview)
		{
			shareGroup.Visibility = Visibility.Collapsed;
			toolBar.Visibility = Visibility.Collapsed;
		}
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		((BookMarkEditViewViewModel)base.DataContext).Loaded();
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		if (((BookMarkEditViewViewModel)base.DataContext).IsUpdate && AppCore.Logger.ShowDialog("当前修改的书签尚未保存，您确定要放弃修改并退出吗？") != MessageResult.Yes)
		{
			e.Cancel = true;
		}
		else
		{
			Application.Current.MainWindow.Activate();
		}
	}

	private void OnCloseClick(object sender, RoutedEventArgs e)
	{
		Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/bookmark/bookmarkeditview.xaml", UriKind.Relative);
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
			win = (BookMarkEditView)target;
			win.Loaded += OnLoaded;
			break;
		case 2:
			toolBar = (BarContainerControl)target;
			break;
		case 3:
			treeListView = (BookMarkTreeView)target;
			break;
		case 4:
			shareGroup = (DevExpress.Xpf.LayoutControl.GroupBox)target;
			break;
		case 5:
			((Button)target).Click += OnCloseClick;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
