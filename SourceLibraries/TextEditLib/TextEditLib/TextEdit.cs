using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode;
using PvfCode.Models.Pvf.Enums;
using TextEditLib.Extensions;
using UnitComboLib.ViewModels;

namespace TextEditLib;

public class TextEdit : TextEditor, IDisposable
{
	public static readonly DependencyProperty PvfEncodingTypeProperty;

	public static readonly DependencyProperty CaretBrushProperty;

	public static readonly DependencyProperty VerticalScrollBarVisibilityExProperty;

	public static readonly DependencyProperty VerticalScrollBarVisibilityBaseProperty;

	public static readonly DependencyProperty ContentMarginProperty;

	public static readonly DependencyProperty EditorTypeProperty;

	public static readonly DependencyProperty CaretOffsetExProperty;

	public static readonly DependencyProperty ShowSpacesProperty;

	public static readonly DependencyProperty ShowTabsProperty;

	public static readonly DependencyProperty ShowEndOfLineProperty;

	public static readonly DependencyProperty EnableTextDragDropProperty;

	public static readonly DependencyProperty EnableVirtualSpaceProperty;

	public static readonly DependencyProperty SizeUnitLabelProperty;

	private static readonly DependencyProperty EditorCurrentLineBackgroundProperty;

	public static readonly DependencyProperty EditorCurrentLineBorderProperty;

	public static readonly DependencyProperty EditorCurrentLineBorderThicknessProperty;

	public static readonly DependencyProperty LineProperty;

	public static readonly DependencyProperty ColumnProperty;

	public static readonly DependencyProperty LineTextLightProperty;

	private new bool IsLoaded { get; set; }

	public EncodingType PvfEncodingType
	{
		get
		{
			return (EncodingType)((DependencyObject)this).GetValue(PvfEncodingTypeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PvfEncodingTypeProperty, (object)value);
		}
	}

	public Brush CaretBrush
	{
		get
		{
			return (Brush)((DependencyObject)this).GetValue(CaretBrushProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CaretBrushProperty, (object)value);
		}
	}

	public Visibility VerticalScrollBarVisibilityEx
	{
		get
		{
			return (Visibility)((DependencyObject)this).GetValue(VerticalScrollBarVisibilityExProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(VerticalScrollBarVisibilityExProperty, (object)value);
		}
	}

	public ScrollBarVisibility VerticalScrollBarVisibilityBase
	{
		get
		{
			return (ScrollBarVisibility)((DependencyObject)this).GetValue(VerticalScrollBarVisibilityBaseProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(VerticalScrollBarVisibilityBaseProperty, (object)value);
		}
	}

	public ScrollViewer GetScrollViewer => FindVisualChild2<ScrollViewer>((DependencyObject)(object)this);

	public Thickness ContentMargin
	{
		get
		{
			return (Thickness)((DependencyObject)this).GetValue(ContentMarginProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ContentMarginProperty, (object)value);
		}
	}

	public TextEditorType EditorType
	{
		get
		{
			return (TextEditorType)((DependencyObject)this).GetValue(EditorTypeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(EditorTypeProperty, (object)value);
		}
	}

	public int CaretOffsetEx
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(CaretOffsetExProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CaretOffsetExProperty, (object)value);
		}
	}

	public bool ShowSpaces
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(ShowSpacesProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ShowSpacesProperty, (object)value);
		}
	}

	public bool ShowTabs
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(ShowTabsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ShowTabsProperty, (object)value);
		}
	}

	public bool ShowEndOfLine
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(ShowEndOfLineProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ShowEndOfLineProperty, (object)value);
		}
	}

	public bool EnableTextDragDrop
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(EnableTextDragDropProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(EnableTextDragDropProperty, (object)value);
		}
	}

	public bool EnableVirtualSpace
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(EnableVirtualSpaceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(EnableVirtualSpaceProperty, (object)value);
		}
	}

	public IUnitViewModel SizeUnitLabel
	{
		get
		{
			return (IUnitViewModel)((DependencyObject)this).GetValue(SizeUnitLabelProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SizeUnitLabelProperty, (object)value);
		}
	}

	public int Line
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(LineProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LineProperty, (object)value);
		}
	}

	public int Column
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(ColumnProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ColumnProperty, (object)value);
		}
	}

	public int LineTextLight
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(LineTextLightProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LineTextLightProperty, (object)value);
		}
	}

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

	~TextEdit()
	{
	}

	private static void CaretBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		TextEdit textEdit = (TextEdit)(object)d;
		if (textEdit != null)
		{
			textEdit.TextArea.Caret.CaretBrush = (Brush)e.NewValue;
		}
	}

	private void SetVerScrollVisibility(ScrollViewer? scrollViewer)
	{
		try
		{
			if (VerticalScrollBarVisibilityBase == ScrollBarVisibility.Auto)
			{
				if (scrollViewer == null)
				{
					scrollViewer = FindVisualChild2<ScrollViewer>((DependencyObject)(object)this);
				}
				if (scrollViewer == null)
				{
					VerticalScrollBarVisibilityEx = Visibility.Collapsed;
				}
				else
				{
					VerticalScrollBarVisibilityEx = scrollViewer.ComputedVerticalScrollBarVisibility;
				}
				return;
			}
			switch (VerticalScrollBarVisibilityBase)
			{
			case ScrollBarVisibility.Disabled:
			case ScrollBarVisibility.Hidden:
				VerticalScrollBarVisibilityEx = Visibility.Collapsed;
				break;
			case ScrollBarVisibility.Visible:
				VerticalScrollBarVisibilityEx = Visibility.Visible;
				break;
			case ScrollBarVisibility.Auto:
				break;
			}
		}
		catch (Exception)
		{
		}
	}

	public static T FindVisualChild2<T>(DependencyObject obj) where T : DependencyObject
	{
		try
		{
			if (obj != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(obj, i);
					if (child == null || !(child is T result))
					{
						T val = FindVisualChild2<T>(child);
						if (val != null)
						{
							return val;
						}
						continue;
					}
					return result;
				}
			}
			return default(T);
		}
		catch (Exception)
		{
			return default(T);
		}
	}

	private static void TotalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		TextEdit textEdit = (TextEdit)(object)d;
		if (textEdit != null && textEdit.EditorType == TextEditorType.OutPut)
		{
			textEdit.ScrollToEnd();
		}
	}

	private static void ShowSpacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEdit textEdit)
		{
			textEdit.TextArea.Options.ShowSpaces = (bool)e.NewValue;
		}
	}

	private static void ShowTabsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEdit textEdit)
		{
			textEdit.Options.ShowTabs = (bool)e.NewValue;
		}
	}

	private static void ShowEndOfLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEdit textEdit)
		{
			textEdit.Options.ShowEndOfLine = (bool)e.NewValue;
		}
	}

	private static void EnableTextDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEdit textEdit)
		{
			textEdit.Options.EnableTextDragDrop = (bool)e.NewValue;
		}
	}

	private static void EnableVirtualSpaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEdit textEdit)
		{
			textEdit.Options.EnableVirtualSpace = (bool)e.NewValue;
		}
	}

	static TextEdit()
	{
		PvfEncodingTypeProperty = DependencyProperty.Register("PvfEncodingType", typeof(EncodingType), typeof(TextEdit), new PropertyMetadata((object)AppSetting.Instance.PvfConfig.DefaultEncoding));
		CaretBrushProperty = DependencyProperty.Register("CaretBrush", typeof(Brush), typeof(TextEdit), new PropertyMetadata((object)null, new PropertyChangedCallback(CaretBrushPropertyChanged)));
		VerticalScrollBarVisibilityExProperty = DependencyProperty.Register("VerticalScrollBarVisibilityEx", typeof(Visibility), typeof(TextEdit), new PropertyMetadata((object)Visibility.Visible));
		VerticalScrollBarVisibilityBaseProperty = DependencyProperty.Register("VerticalScrollBarVisibilityBase", typeof(ScrollBarVisibility), typeof(TextEdit), new PropertyMetadata((object)ScrollBarVisibility.Auto));
		ContentMarginProperty = DependencyProperty.Register("ContentMargin", typeof(Thickness), typeof(TextEdit), new PropertyMetadata((object)new Thickness(0.0, 0.0, 0.0, 0.0)));
		EditorTypeProperty = DependencyProperty.Register("EditorType", typeof(TextEditorType), typeof(TextEdit), new PropertyMetadata((object)TextEditorType.Editor));
		CaretOffsetExProperty = DependencyProperty.Register("CaretOffsetEx", typeof(int), typeof(TextEdit), new PropertyMetadata((object)0, new PropertyChangedCallback(TotalPropertyChanged)));
		ShowSpacesProperty = DependencyProperty.Register("ShowSpaces", typeof(bool), typeof(TextEdit), new PropertyMetadata((object)false, new PropertyChangedCallback(ShowSpacesChanged)));
		ShowTabsProperty = DependencyProperty.Register("ShowTabs", typeof(bool), typeof(TextEdit), new PropertyMetadata((object)false, new PropertyChangedCallback(ShowTabsChanged)));
		ShowEndOfLineProperty = DependencyProperty.Register("ShowEndOfLine", typeof(bool), typeof(TextEdit), new PropertyMetadata((object)false, new PropertyChangedCallback(ShowEndOfLineChanged)));
		EnableTextDragDropProperty = DependencyProperty.Register("EnableTextDragDrop", typeof(bool), typeof(TextEdit), new PropertyMetadata((object)true, new PropertyChangedCallback(EnableTextDragDropChanged)));
		EnableVirtualSpaceProperty = DependencyProperty.Register("EnableVirtualSpace", typeof(bool), typeof(TextEdit), new PropertyMetadata((object)false, new PropertyChangedCallback(EnableVirtualSpaceChanged)));
		SizeUnitLabelProperty = DependencyProperty.Register("SizeUnitLabel", typeof(IUnitViewModel), typeof(TextEdit), new PropertyMetadata((PropertyChangedCallback)null));
		EditorCurrentLineBackgroundProperty = DependencyProperty.Register("EditorCurrentLineBackground", typeof(Brush), typeof(TextEdit), (PropertyMetadata)(object)new UIPropertyMetadata((object)new SolidColorBrush(Colors.Transparent)));
		EditorCurrentLineBorderProperty = DependencyProperty.Register("EditorCurrentLineBorder", typeof(Brush), typeof(TextEdit), new PropertyMetadata((object)new SolidColorBrush(Color.FromArgb(96, SystemColors.HighlightBrush.Color.R, SystemColors.HighlightBrush.Color.G, SystemColors.HighlightBrush.Color.B))));
		EditorCurrentLineBorderThicknessProperty = DependencyProperty.Register("EditorCurrentLineBorderThickness", typeof(double), typeof(TextEdit), new PropertyMetadata((object)2.0));
		LineProperty = DependencyProperty.Register("Line", typeof(int), typeof(TextEdit), new PropertyMetadata((object)0));
		ColumnProperty = DependencyProperty.Register("Column", typeof(int), typeof(TextEdit), new PropertyMetadata((object)0));
		LineTextLightProperty = DependencyProperty.Register("LineTextLight", typeof(int), typeof(TextEdit), new PropertyMetadata((object)0));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEdit), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)typeof(TextEdit)));
	}

	public TextEdit()
	{
		DataObject.AddSettingDataHandler((DependencyObject)(object)this, onTextViewSettingDataHandler);
		base.Loaded += TextEdit_Loaded;
	}

	public void onTextViewSettingDataHandler(object sender, DataObjectSettingDataEventArgs e)
	{
		if ((sender as TextEditor).TextArea.TextView != null && e.Format == DataFormats.Html)
		{
			e.CancelCommand();
		}
	}

	private void TextEdit_Unloaded(object sender, RoutedEventArgs e)
	{
		base.Unloaded -= TextEdit_Unloaded;
		base.PreviewMouseWheel -= textEditor_PreviewMouseWheel;
		base.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
	}

	private void LoadedSetOptions()
	{
		base.Options.ShowTabs = ShowTabs;
		base.Options.ShowSpaces = ShowSpaces;
		base.Options.EnableTextDragDrop = EnableTextDragDrop;
		base.Options.EnableVirtualSpace = EnableVirtualSpace;
		base.Options.ShowEndOfLine = ShowEndOfLine;
	}

	private void TextEdit_Loaded(object sender, RoutedEventArgs e)
	{
		if (!IsLoaded)
		{
			IsLoaded = true;
			LoadedSetOptions();
		}
		AdjustCurrentLineBackground();
		base.PreviewMouseWheel += textEditor_PreviewMouseWheel;
		base.TextArea.Caret.PositionChanged += Caret_PositionChanged;
		base.Unloaded += TextEdit_Unloaded;
		ScrollViewer getScrollViewer = GetScrollViewer;
		if (getScrollViewer != null)
		{
			getScrollViewer.ScrollChanged += Bar_ScrollChanged;
		}
		SetVerScrollVisibility(null);
	}

	private void Bar_ScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		SetVerScrollVisibility((ScrollViewer)sender);
	}

	public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
	{
		if (obj != null)
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
				{
					return (T)(object)child;
				}
				T val = FindVisualChild<T>(child);
				if (val != null)
				{
					return val;
				}
			}
		}
		return default(T);
	}

	private void Caret_PositionChanged(object sender, EventArgs e)
	{
		if (base.TextArea != null)
		{
			Column = base.TextArea.Caret.Column;
			Line = base.TextArea.Caret.Line;
			if (base.Document != null)
			{
				LineTextLight = base.Document.GetLineByNumber(Line).Length;
			}
		}
		else
		{
			Column = 0;
			Line = 0;
			LineTextLight = 0;
		}
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
		base.TextArea.TextView.BackgroundRenderers.Add(new HighlightCurrentLineBackgroundRenderer(this));
	}

	private void textEditor_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		if ((int)Keyboard.Modifiers == 2)
		{
			double num = base.FontSize + (double)e.Delta / 25.0;
			if (num < 6.0)
			{
				base.FontSize = 6.0;
			}
			else if (num > 200.0)
			{
				base.FontSize = 200.0;
			}
			else
			{
				base.FontSize = num;
			}
			e.Handled = true;
		}
	}

	public void Dispose()
	{
		ScrollViewer getScrollViewer = GetScrollViewer;
		if (getScrollViewer != null)
		{
			getScrollViewer.ScrollChanged -= Bar_ScrollChanged;
		}
		base.Unloaded -= TextEdit_Unloaded;
		base.PreviewMouseWheel -= textEditor_PreviewMouseWheel;
		base.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
		base.Loaded -= TextEdit_Loaded;
		DataObject.RemoveSettingDataHandler((DependencyObject)(object)this, onTextViewSettingDataHandler);
	}
}
