using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using PvfCode.ViewModels.independent_drop.DropList;

namespace PvfCode.Views.independent_drop;

public class ViewAddItemCodeArray : ThemedWindow, IComponentConnector
{
	internal List<ListItem> XHEhKC1k16;

	internal ViewAddItemCodeArray win;

	private bool twFh9Nnqow;

	public ViewAddItemCodeArray()
	{
		ViewAddItemCodeArrayViewModel dataContext = new ViewAddItemCodeArrayViewModel();
		base.DataContext = dataContext;
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!twFh9Nnqow)
		{
			twFh9Nnqow = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/independent_drop/viewadditemcodearray.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			win = (ViewAddItemCodeArray)target;
		}
		else
		{
			twFh9Nnqow = true;
		}
	}
}
