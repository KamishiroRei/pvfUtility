using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.SearchPvf;

public class WindowAddNewSearchResult : ThemedWindow, IComponentConnector
{
	internal TextBox input;

	internal Button buttonYes;

	private bool contentLoaded;

	public WindowAddNewSearchResult()
	{
		InitializeComponent();
		buttonYes.IsEnabled = false;
	}

	private void OnInputTextChanged(object sender, TextChangedEventArgs e)
	{
		buttonYes.IsEnabled = !string.IsNullOrEmpty(input.Text);
	}

	private void OnYesClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = true;
	}

	private void OnCancelClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			input.Focus();
		}, Array.Empty<object>());
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!contentLoaded)
		{
			contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/searchpvf/windowaddnewsearchresult.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((WindowAddNewSearchResult)target).Loaded += OnLoaded;
			break;
		case 2:
			input = (TextBox)target;
			input.TextChanged += OnInputTextChanged;
			break;
		case 3:
			buttonYes = (Button)target;
			buttonYes.Click += OnYesClick;
			break;
		case 4:
			((Button)target).Click += OnCancelClick;
			break;
		default:
			contentLoaded = true;
			break;
		}
	}
}
