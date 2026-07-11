using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace PvfCode.Controls.AvatarControl;

public class AvatarControlEx : UserControl, IComponentConnector
{
	public static readonly DependencyProperty ImageSourceProperty;

	internal AvatarControlEx avatar;

	internal Image image1;

	private bool jRq6djAJy8;

	public ImageSource ImageSource
	{
		get
		{
			return (ImageSource)((DependencyObject)this).GetValue(ImageSourceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ImageSourceProperty, (object)value);
		}
	}

	public AvatarControlEx()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!jRq6djAJy8)
		{
			jRq6djAJy8 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/avatarcontrol/avatarcontrolex.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			avatar = (AvatarControlEx)target;
			break;
		case 2:
			image1 = (Image)target;
			break;
		default:
			jRq6djAJy8 = true;
			break;
		}
	}

	static AvatarControlEx()
	{
		ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(AvatarControlEx), new PropertyMetadata((PropertyChangedCallback)null));
	}
}
