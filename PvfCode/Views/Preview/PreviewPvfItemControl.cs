using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PvfCode.Views.Preview;

public class PreviewPvfItemControl : UserControl, IComponentConnector
{
	public static readonly DependencyProperty ContentExProperty;

	private bool mqHHcRlulK;

	public object ContentEx
	{
		get
		{
			return ((DependencyObject)this).GetValue(ContentExProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ContentExProperty, value);
		}
	}

	public PreviewPvfItemControl()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!mqHHcRlulK)
		{
			mqHHcRlulK = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/preview/previewpvfitemcontrol.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		mqHHcRlulK = true;
	}

	static PreviewPvfItemControl()
	{
		ContentExProperty = DependencyProperty.Register("ContentEx", typeof(object), typeof(PreviewPvfItemControl), new PropertyMetadata((PropertyChangedCallback)null));
	}
}
