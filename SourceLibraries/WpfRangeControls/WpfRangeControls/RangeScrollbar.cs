using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace WpfRangeControls;

[TemplatePart(Name = "PART_RangeOverlay", Type = typeof(RangeItemsControl))]
[ContentProperty("Items")]
public class RangeScrollbar : ScrollBar, INotifyPropertyChanged
{
	private RangeItemsControl _RangeControl;

	private ObservableCollection<UIElement> _iItems = new ObservableCollection<UIElement>();

	public static readonly DependencyProperty ItemsSourceProperty;

	public static readonly DependencyProperty ItemTemplateProperty;

	public static readonly DependencyProperty ItemTemplateSelectorProperty;

	public static readonly DependencyProperty AlternationCountProperty;

	[Bindable(false)]
	[Category("Content")]
	public RangeItemsControl RangeControl => _RangeControl;

	[Bindable(true)]
	[Category("Content")]
	public IList Items
	{
		get
		{
			if (_RangeControl == null)
			{
				ApplyTemplate();
			}
			if (_RangeControl != null)
			{
				return _RangeControl.Items;
			}
			return _iItems;
		}
	}

	[Bindable(true)]
	[Category("Content")]
	public IEnumerable ItemsSource
	{
		get
		{
			return (IEnumerable)((DependencyObject)this).GetValue(ItemsSourceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemsSourceProperty, (object)value);
		}
	}

	[Bindable(true)]
	[Category("Content")]
	public DataTemplate ItemTemplate
	{
		get
		{
			return (DataTemplate)((DependencyObject)this).GetValue(ItemTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemTemplateProperty, (object)value);
		}
	}

	[Bindable(true)]
	[Category("Content")]
	public DataTemplateSelector ItemTemplateSelector
	{
		get
		{
			return (DataTemplateSelector)((DependencyObject)this).GetValue(ItemTemplateSelectorProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemTemplateSelectorProperty, (object)value);
		}
	}

	[Bindable(true)]
	[Category("Content")]
	public int AlternationCount
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(AlternationCountProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(AlternationCountProperty, (object)value);
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	static RangeScrollbar()
	{
		ItemsSourceProperty = ItemsControl.ItemsSourceProperty.AddOwner(typeof(RangeScrollbar));
		ItemTemplateProperty = ItemsControl.ItemTemplateProperty.AddOwner(typeof(RangeScrollbar));
		ItemTemplateSelectorProperty = ItemsControl.ItemTemplateSelectorProperty.AddOwner(typeof(RangeScrollbar));
		AlternationCountProperty = ItemsControl.AlternationCountProperty.AddOwner(typeof(RangeScrollbar));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeScrollbar), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)typeof(RangeScrollbar)));
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		_RangeControl = GetTemplateChild("PART_RangeOverlay") as RangeItemsControl;
		if (_RangeControl == null || _iItems == null || _iItems.Count <= 0)
		{
			return;
		}
		foreach (UIElement iItem in _iItems)
		{
			_RangeControl.Items.Add(iItem);
		}
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs("Items"));
		}
		_iItems = null;
	}
}
