using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using PvfCode.Dot;
using PvfCode.ViewModels.Store.Preview;

namespace PvfCode.Views.Store.Preview;

public class PreviewFileListComment : ThemedWindow, IComponentConnector
{
	private bool hApCJXsE6w;

	public PreviewFileListComment(Dictionary<string, TreelistCommentRes> source)
	{
		base.DataContext = new PreviewFileListCommentViewModel(source);
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!hApCJXsE6w)
		{
			hApCJXsE6w = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/store/preview/previewfilelistcomment.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		hApCJXsE6w = true;
	}
}
