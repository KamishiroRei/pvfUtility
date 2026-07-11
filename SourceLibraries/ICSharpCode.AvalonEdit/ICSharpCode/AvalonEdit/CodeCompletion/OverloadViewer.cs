using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class OverloadViewer : Control
{
	public static readonly DependencyProperty TextProperty;

	public static readonly DependencyProperty ProviderProperty;

	public string Text
	{
		get
		{
			return (string)GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

	public IOverloadProvider Provider
	{
		get
		{
			return (IOverloadProvider)GetValue(ProviderProperty);
		}
		set
		{
			SetValue(ProviderProperty, value);
		}
	}

	static OverloadViewer()
	{
		TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OverloadViewer));
		ProviderProperty = DependencyProperty.Register("Provider", typeof(IOverloadProvider), typeof(OverloadViewer));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(OverloadViewer), new FrameworkPropertyMetadata(typeof(OverloadViewer)));
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		((Button)base.Template.FindName("PART_UP", this)).Click += delegate(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			ChangeIndex(-1);
		};
		((Button)base.Template.FindName("PART_DOWN", this)).Click += delegate(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
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
}
