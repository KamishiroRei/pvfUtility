using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfRangeControls;

public class RangePanel : Panel
{
	public static readonly DependencyProperty PositionProperty;

	public static readonly DependencyProperty AlignmentProperty;

	public static readonly DependencyProperty RangeProperty;

	public static readonly DependencyProperty OrientationProperty;

	public static readonly DependencyProperty MinimumProperty;

	public static readonly DependencyProperty MaximumProperty;

	protected override bool HasLogicalOrientation => true;

	protected override Orientation LogicalOrientation => Orientation;

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

	static RangePanel()
	{
		PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(double), typeof(RangePanel), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
		AlignmentProperty = DependencyProperty.RegisterAttached("Alignment", typeof(RangeAlignment), typeof(RangePanel), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)RangeAlignment.Center, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure), new ValidateValueCallback(IsValidRangeAlignment));
		RangeProperty = DependencyProperty.RegisterAttached("Range", typeof(double), typeof(RangePanel), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
		OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RangePanel), (PropertyMetadata)(object)new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnOrientationChanged)), new ValidateValueCallback(IsValidOrientation));
		MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RangePanel), (PropertyMetadata)(object)new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(CoerceMinimum)), new ValidateValueCallback(IsValidDoubleValue));
		MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RangePanel), (PropertyMetadata)(object)new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMaximumChanged), new CoerceValueCallback(CoerceMaximum)), new ValidateValueCallback(IsValidDoubleValue));
		UIElement.ClipToBoundsProperty.OverrideMetadata(typeof(RangePanel), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)true));
		FrameworkElement.HorizontalAlignmentProperty.OverrideMetadata(typeof(RangePanel), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)HorizontalAlignment.Stretch));
		FrameworkElement.VerticalAlignmentProperty.OverrideMetadata(typeof(RangePanel), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)VerticalAlignment.Stretch));
	}

	public static double GetPosition(DependencyObject obj)
	{
		return (double)obj.GetValue(PositionProperty);
	}

	public static void SetPosition(DependencyObject obj, double value)
	{
		obj.SetValue(PositionProperty, (object)value);
	}

	public static RangeAlignment GetAlignment(DependencyObject obj)
	{
		return (RangeAlignment)obj.GetValue(AlignmentProperty);
	}

	public static void SetAlignment(DependencyObject obj, RangeAlignment value)
	{
		obj.SetValue(AlignmentProperty, (object)value);
	}

	private static bool IsValidRangeAlignment(object value)
	{
		RangeAlignment rangeAlignment = (RangeAlignment)value;
		if (rangeAlignment != RangeAlignment.Begin && rangeAlignment != RangeAlignment.Center)
		{
			return rangeAlignment == RangeAlignment.End;
		}
		return true;
	}

	public static double GetRange(DependencyObject obj)
	{
		return (double)obj.GetValue(RangeProperty);
	}

	public static void SetRange(DependencyObject obj, double value)
	{
		obj.SetValue(RangeProperty, (object)value);
	}

	private static bool IsValidOrientation(object o)
	{
		Orientation orientation = (Orientation)o;
		if (orientation != Orientation.Horizontal)
		{
			return orientation == Orientation.Vertical;
		}
		return true;
	}

	private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is UIElement uIElement)
		{
			uIElement.InvalidateMeasure();
		}
	}

	private static bool IsValidDoubleValue(object value)
	{
		double d = (double)value;
		if (!double.IsNaN(d))
		{
			return !double.IsInfinity(d);
		}
		return false;
	}

	private static object CoerceMinimum(DependencyObject d, object value)
	{
		double maximum = ((RangePanel)(object)d).Maximum;
		if ((double)value > maximum)
		{
			return maximum;
		}
		return value;
	}

	private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		RangePanel obj = (RangePanel)(object)d;
		((DependencyObject)obj).CoerceValue(MaximumProperty);
		obj.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
	}

	protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
	{
	}

	private static object CoerceMaximum(DependencyObject d, object value)
	{
		double minimum = ((RangePanel)(object)d).Minimum;
		if ((double)value < minimum)
		{
			return minimum;
		}
		return value;
	}

	private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((RangePanel)(object)d).OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
	}

	protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
	{
	}

	public RangePanel()
	{
		base.ClipToBounds = true;
	}

	protected override Size MeasureOverride(Size constraint)
	{
		double num = 0.0;
		double num2 = 0.0;
		foreach (UIElement internalChild in base.InternalChildren)
		{
			internalChild.Measure(constraint);
			Rect itemPosition = GetItemPosition(constraint, internalChild);
			if (itemPosition.Right > num)
			{
				num = itemPosition.Right;
			}
			if (itemPosition.Bottom > num2)
			{
				num2 = itemPosition.Bottom;
			}
		}
		if (!double.IsNaN(base.Width))
		{
			num = base.Width;
		}
		if (!double.IsNaN(base.Height))
		{
			num2 = base.Height;
		}
		if (base.HorizontalAlignment == HorizontalAlignment.Stretch)
		{
			num = 0.0;
		}
		if (base.VerticalAlignment == VerticalAlignment.Stretch)
		{
			num2 = 0.0;
		}
		return new Size(num, num2);
	}

	private double ScaleToSize(double val, double size)
	{
		double num = Maximum - Minimum;
		return val / num * size;
	}

	protected override Size ArrangeOverride(Size arrangeSize)
	{
		foreach (UIElement internalChild in base.InternalChildren)
		{
			Rect itemPosition = GetItemPosition(arrangeSize, internalChild);
			internalChild.Arrange(itemPosition);
		}
		return arrangeSize;
	}

	private Rect GetItemPosition(Size arrangeSize, UIElement item)
	{
		if (item is ContentPresenter && ((DependencyObject)item).ReadLocalValue(PositionProperty) == DependencyProperty.UnsetValue && VisualTreeHelper.GetChild((DependencyObject)(object)item, 0) is UIElement uIElement)
		{
			item = uIElement;
		}
		double position = GetPosition((DependencyObject)(object)item);
		double range = GetRange((DependencyObject)(object)item);
		RangeAlignment alignment = GetAlignment((DependencyObject)(object)item);
		double num = 0.0;
		double num2 = 0.0;
		Size desiredSize = item.DesiredSize;
		double num3 = desiredSize.Width;
		desiredSize = item.DesiredSize;
		double num4 = desiredSize.Height;
		if (!double.IsNaN(range))
		{
			if (Orientation == Orientation.Horizontal)
			{
				num3 = ScaleToSize(range, arrangeSize.Width);
			}
			else
			{
				num4 = ScaleToSize(range, arrangeSize.Height);
			}
		}
		Size val = default(Size);
		if (Orientation == Orientation.Horizontal)
		{
			num = ScaleToSize(position, arrangeSize.Width);
			val = new Size(num3, arrangeSize.Height);
			num -= SizeAdjustment(alignment, num3);
		}
		else
		{
			num2 = ScaleToSize(position, arrangeSize.Height);
			val = new Size(arrangeSize.Width, num4);
			num2 -= SizeAdjustment(alignment, num4);
		}
		return new Rect(new Point(num, num2), val);
	}

	private static double SizeAdjustment(RangeAlignment align, double size)
	{
		return align switch
		{
			RangeAlignment.Center => size / 2.0, 
			RangeAlignment.End => size, 
			_ => 0.0, 
		};
	}

	protected override Geometry GetLayoutClip(Size layoutSlotSize)
	{
		return base.GetLayoutClip(layoutSlotSize);
	}
}
