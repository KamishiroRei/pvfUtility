using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using PvfCode.Controls;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewRenameNode : ThemedWindow, IComponentConnector
{
	internal EditBox editBox;

	private bool _contentLoaded;

	public ViewRenameNode(KeyValuePair<string, PvfTreeFileBase> treeFileDic)
	{
		ViewRenameNodeViewModel dataContext = new ViewRenameNodeViewModel(treeFileDic, base.Close);
		base.DataContext = dataContext;
		InitializeComponent();
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		Focus();
		((DispatcherObject)this).Dispatcher.Invoke((Action)delegate
		{
			editBox.input.Focus();
		});
		string text = editBox.input.Text;
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (string.IsNullOrEmpty(Path.GetExtension(text)))
		{
			editBox.input.Select(0, text.Length);
			return;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
		if (!string.IsNullOrEmpty(fileNameWithoutExtension))
		{
			((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)(() =>
				editBox.input.Select(0, fileNameWithoutExtension.Length)), Array.Empty<object>());
		}
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
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/pvftreefolder/viewrenamenode.xaml", UriKind.Relative);
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
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((ViewRenameNode)target).Loaded += OnLoaded;
			break;
		case 2:
			editBox = (EditBox)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
