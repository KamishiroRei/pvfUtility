using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Views.PvfTreeFolder;

public class ViewEditTreeComment : ThemedWindow, IComponentConnector
{
	internal TextEdit textWarp;

	private bool HDsH55PoGF;

	public ViewEditTreeComment(PvfTreeFileBase treeFile)
	{
		base.DataContext = new ViewEditTreeCommentViewModel(treeFile, base.Close);
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
		if (!HDsH55PoGF)
		{
			HDsH55PoGF = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/pvftreefolder/viewedittreecomment.xaml", UriKind.Relative);
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
		if (connectionId == 1)
		{
			textWarp = (TextEdit)target;
		}
		else
		{
			HDsH55PoGF = true;
		}
	}
}
