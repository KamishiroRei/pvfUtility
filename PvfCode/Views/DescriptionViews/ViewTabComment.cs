using System;
using System.CodeDom.Compiler;
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
using PvfCode.ViewModels.Description.ViewTabComment;
using PvfCode.Controls;
using TextEditLib;

namespace PvfCode.Views.DescriptionViews;

public class ViewTabComment : ThemedWindow, IComponentConnector
{
	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal ButtonEdit textKeyword;

	internal ToolBarControl toolbarControl;

	internal TextEditLib.TextEdit editor;

	private bool _contentLoaded;

	public ViewTabComment()
	{
		ViewTabCommentViewModel dataContext = new ViewTabCommentViewModel(base.Close);
		base.DataContext = dataContext;
		InitializeComponent();
		InstallMarkdownEditor(dataContext);
	}

	private void InstallMarkdownEditor(ViewTabCommentViewModel viewModel)
	{
		UIElement originalContent = Content as UIElement;
		if (originalContent == null)
		{
			return;
		}
		Content = null;
		Grid root = new();
		root.RowDefinitions.Add(new RowDefinition());
		root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) });
		root.Children.Add(originalContent);
		PvfCommentManagementPanel markdownPanel = new() { DataContext = viewModel };
		Grid.SetRow(markdownPanel, 1);
		root.Children.Add(markdownPanel);
		Content = root;
		Width = Math.Max(Width, 1100);
		Height = Math.Max(Height, 820);
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		base.OnClosing(e);
		Application.Current.MainWindow.Activate();
	}

	private void OnDockItemClosing(object sender, ItemCancelEventArgs e)
	{
		e.Cancel = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/descriptionviews/viewtabcomment.xaml", UriKind.Relative);
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
			textKeyword = (ButtonEdit)target;
			break;
		case 4:
			toolbarControl = (ToolBarControl)target;
			break;
		case 5:
			editor = (TextEditLib.TextEdit)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
