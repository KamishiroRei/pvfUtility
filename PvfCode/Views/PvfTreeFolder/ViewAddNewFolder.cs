using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewAddNewFolder : ThemedWindow, IComponentConnector
{
	internal ViewAddNewFolder win;

	internal TextEdit textInput;

	internal Button buttonOk;

	private bool _contentLoaded;

	public ViewAddNewFolder(KeyValuePair<string, PvfTreeFileBase>? keyValuePair)
	{
		base.DataContext = new ViewAddNewFolderViewModel(keyValuePair);
		InitializeComponent();
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		((DispatcherObject)textInput).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			textInput.Focus();
		}, Array.Empty<object>());
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/pvftreefolder/viewaddnewfolder.xaml", UriKind.Relative);
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
			win = (ViewAddNewFolder)target;
			win.Loaded += OnLoaded;
			break;
		case 2:
			textInput = (TextEdit)target;
			break;
		case 3:
			buttonOk = (Button)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
