using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Editors;

namespace PvfCode.Controls;

public class EditBox : UserControl, IComponentConnector
{
	public static readonly DependencyProperty NullTextProperty;

	public static readonly DependencyProperty CaptionProperty;

	public static readonly DependencyProperty ValueProperty;

	internal TextEdit input;

	private bool HPYaosbkb6;

	public string NullText
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(NullTextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(NullTextProperty, (object)value);
		}
	}

	public string Caption
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(CaptionProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CaptionProperty, (object)value);
		}
	}

	public string Value
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(ValueProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ValueProperty, (object)value);
		}
	}

	public EditBox()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!HPYaosbkb6)
		{
			HPYaosbkb6 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/editbox.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			input = (TextEdit)target;
		}
		else
		{
			HPYaosbkb6 = true;
		}
	}

	static EditBox()
	{
		NullTextProperty = DependencyProperty.Register("NullText", typeof(string), typeof(EditBox), new PropertyMetadata((object)""));
		CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(EditBox), new PropertyMetadata((object)""));
		ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(EditBox), new PropertyMetadata((PropertyChangedCallback)null));
	}
}
