using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class OverloadViewer : Control
{
	public static readonly DependencyProperty TextProperty;

	public static readonly DependencyProperty ProviderProperty;

	public string Text
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(TextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TextProperty, (object)value);
		}
	}

	public IOverloadProvider Provider
	{
		get
		{
			return (IOverloadProvider)((DependencyObject)this).GetValue(ProviderProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ProviderProperty, (object)value);
		}
	}

	static OverloadViewer()
	{
		TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OverloadViewer));
		ProviderProperty = DependencyProperty.Register("Provider", typeof(IOverloadProvider), typeof(OverloadViewer));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(OverloadViewer), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)typeof(OverloadViewer)));
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		((Button)base.Template.FindName("PART_UP", this)).Click += delegate(object P_0, RoutedEventArgs P_1)
		{
			P_1.Handled = true;
			ChangeIndex(-1);
		};
		((Button)base.Template.FindName("PART_DOWN", this)).Click += delegate(object P_0, RoutedEventArgs P_1)
		{
			P_1.Handled = true;
			ChangeIndex(1);
		};
	}

	public void ChangeIndex(int relativeIndexChange)
	{
		IOverloadProvider provider = Provider;
		if (provider != null)
		{
			int num = provider.SelectedIndex + relativeIndexChange;
			if (num < 0)
			{
				num = provider.Count - 1;
			}
			if (num >= provider.Count)
			{
				num = 0;
			}
			provider.SelectedIndex = num;
		}
	}

	public OverloadViewer()
	{
	}

	[CompilerGenerated]
	private void J5kugsIsvU(object P_0, RoutedEventArgs P_1)
	{
		P_1.Handled = true;
		ChangeIndex(-1);
	}

	[CompilerGenerated]
	private void dVPu63HKKX(object P_0, RoutedEventArgs P_1)
	{
		P_1.Handled = true;
		ChangeIndex(1);
	}
}
