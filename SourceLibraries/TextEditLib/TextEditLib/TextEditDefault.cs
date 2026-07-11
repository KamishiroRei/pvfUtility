using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using TextEditLib.Extensions;

namespace TextEditLib;

public class TextEditDefault : TextEditor
{
	private static readonly DependencyProperty EditorCurrentLineBackgroundProperty;

	public static readonly DependencyProperty EditorCurrentLineBorderProperty;

	public static readonly DependencyProperty EditorCurrentLineBorderThicknessProperty;

	public Brush EditorCurrentLineBackground
	{
		get
		{
			return (Brush)((DependencyObject)this).GetValue(EditorCurrentLineBackgroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(EditorCurrentLineBackgroundProperty, (object)value);
		}
	}

	public Brush EditorCurrentLineBorder
	{
		get
		{
			return (Brush)((DependencyObject)this).GetValue(EditorCurrentLineBorderProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(EditorCurrentLineBorderProperty, (object)value);
		}
	}

	public double EditorCurrentLineBorderThickness
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(EditorCurrentLineBorderThicknessProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(EditorCurrentLineBorderThicknessProperty, (object)value);
		}
	}

	static TextEditDefault()
	{
		EditorCurrentLineBackgroundProperty = DependencyProperty.Register("EditorCurrentLineBackground", typeof(Brush), typeof(TextEditDefault), (PropertyMetadata)(object)new UIPropertyMetadata((object)new SolidColorBrush(Colors.Transparent)));
		EditorCurrentLineBorderProperty = DependencyProperty.Register("EditorCurrentLineBorder", typeof(Brush), typeof(TextEditDefault), new PropertyMetadata((object)new SolidColorBrush(Color.FromArgb(96, SystemColors.HighlightBrush.Color.R, SystemColors.HighlightBrush.Color.G, SystemColors.HighlightBrush.Color.B))));
		EditorCurrentLineBorderThicknessProperty = DependencyProperty.Register("EditorCurrentLineBorderThickness", typeof(double), typeof(TextEditDefault), new PropertyMetadata((object)2.0));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEditDefault), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)typeof(TextEditDefault)));
	}

	public TextEditDefault()
	{
		base.Loaded += TextEdit_Loaded;
	}

	private void TextEdit_Loaded(object sender, RoutedEventArgs e)
	{
		AdjustCurrentLineBackground();
	}

	private void AdjustCurrentLineBackground()
	{
		HighlightCurrentLineBackgroundRenderer highlightCurrentLineBackgroundRenderer = null;
		foreach (IBackgroundRenderer backgroundRenderer in base.TextArea.TextView.BackgroundRenderers)
		{
			if (backgroundRenderer != null && backgroundRenderer is HighlightCurrentLineBackgroundRenderer)
			{
				highlightCurrentLineBackgroundRenderer = backgroundRenderer as HighlightCurrentLineBackgroundRenderer;
			}
		}
		if (highlightCurrentLineBackgroundRenderer != null)
		{
			base.TextArea.TextView.BackgroundRenderers.Remove(highlightCurrentLineBackgroundRenderer);
		}
		base.TextArea.TextView.BackgroundRenderers.Add(new HighlightCurrentLineBackgroundRendererDefault(this));
	}
}
