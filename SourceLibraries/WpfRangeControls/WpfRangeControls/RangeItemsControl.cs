using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfRangeControls;

public class RangeItemsControl : ItemsControl
{
	public static DependencyProperty OrientationProperty;

	public static DependencyProperty MinimumProperty;

	public static DependencyProperty MaximumProperty;

	public Orientation Orientation
	{
		get
		{
			return (Orientation)((DependencyObject)this).GetValue(OrientationProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OrientationProperty, (object)value);
		}
	}

	[Bindable(true)]
	[Category("Behavior")]
	public double Minimum
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MinimumProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MinimumProperty, (object)value);
		}
	}

	[Bindable(true)]
	[Category("Behavior")]
	public double Maximum
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MaximumProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaximumProperty, (object)value);
		}
	}

	static RangeItemsControl()
	{
		OrientationProperty = RangePanel.OrientationProperty.AddOwner(typeof(RangeItemsControl));
		MinimumProperty = RangePanel.MinimumProperty.AddOwner(typeof(RangeItemsControl));
		MaximumProperty = RangePanel.MaximumProperty.AddOwner(typeof(RangeItemsControl));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeItemsControl), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)typeof(RangeItemsControl)));
	}
}
