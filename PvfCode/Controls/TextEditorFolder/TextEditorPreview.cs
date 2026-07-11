using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace PvfCode.Controls.TextEditorFolder;

public class TextEditorPreview : UserControl, IComponentConnector
{
	public static readonly DependencyProperty MaxWidthValueProperty;

	public static readonly DependencyProperty MaxHeightValueProperty;

	internal Grid gridMain;

	private bool XgNgN4lERM;

	public double MaxWidthValue
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MaxWidthValueProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaxWidthValueProperty, (object)value);
		}
	}

	public double MaxHeightValue
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MaxHeightValueProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaxHeightValueProperty, (object)value);
		}
	}

	public TextEditorPreview()
	{
		InitializeComponent();
	}

	private void K4bg34eNYo(object P_0, DragDeltaEventArgs P_1)
	{
		Grid grid = gridMain;
		FrameworkElement frameworkElement = P_0 as FrameworkElement;
		if (frameworkElement.HorizontalAlignment == HorizontalAlignment.Left)
		{
			double num = (double.IsNaN(grid.Width) ? grid.ActualWidth : grid.Width) - P_1.HorizontalChange;
			if (num > MaxWidthValue - 32.0)
			{
				num = MaxWidthValue - 32.0;
			}
			if (num < 200.0)
			{
				num = 200.0;
			}
			if (frameworkElement.HorizontalAlignment != HorizontalAlignment.Center && frameworkElement.HorizontalAlignment != HorizontalAlignment.Stretch && num >= 0.0)
			{
				AppSetting.Instance.EditConfig.PreviewAniPanelWidht = num;
			}
			double num2 = (double.IsNaN(grid.Height) ? grid.ActualHeight : grid.Height) - P_1.VerticalChange;
			if (num2 > MaxHeightValue - 32.0)
			{
				num2 = MaxHeightValue - 32.0;
			}
			if (num2 < 200.0)
			{
				num2 = 200.0;
			}
			if (frameworkElement.VerticalAlignment != VerticalAlignment.Center && frameworkElement.VerticalAlignment != VerticalAlignment.Stretch && num2 >= 0.0)
			{
				AppSetting.Instance.EditConfig.PreviewAniPanelHeight = num2;
			}
		}
	}

	private async void F4rgR4JyBG(object P_0, DragCompletedEventArgs P_1)
	{
		await AppSetting.Instance.SaveSetting();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!XgNgN4lERM)
		{
			XgNgN4lERM = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/texteditorfolder/texteditorpreview.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			gridMain = (Grid)target;
			break;
		case 2:
			((Thumb)target).DragCompleted += F4rgR4JyBG;
			((Thumb)target).DragDelta += K4bg34eNYo;
			break;
		default:
			XgNgN4lERM = true;
			break;
		}
	}

	static TextEditorPreview()
	{
		MaxWidthValueProperty = DependencyProperty.Register("MaxWidthValue", typeof(double), typeof(TextEditorPreview), new PropertyMetadata((object)0.0));
		MaxHeightValueProperty = DependencyProperty.Register("MaxHeightValue", typeof(double), typeof(TextEditorPreview), new PropertyMetadata((object)0.0));
	}
}
