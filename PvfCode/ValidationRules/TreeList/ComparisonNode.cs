using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace PvfCode.ValidationRules.TreeList;

public class ComparisonNode : DependencyObject
{
	public static readonly DependencyProperty ValueProperty;

	public static readonly DependencyProperty BindingToTriggerProperty;

	public static readonly DependencyProperty SourceProperty;

	public PvfTreeFileRename Value
	{
		get
		{
			return (PvfTreeFileRename)((DependencyObject)this).GetValue(ValueProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ValueProperty, (object)value);
		}
	}

	public object BindingToTrigger
	{
		get
		{
			return ((DependencyObject)this).GetValue(BindingToTriggerProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(BindingToTriggerProperty, value);
		}
	}

	public IDictionary<string, PvfTreeFileBase> Source
	{
		get
		{
			return (IDictionary<string, PvfTreeFileBase>)((DependencyObject)this).GetValue(SourceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SourceProperty, (object)value);
		}
	}

	private static void UJaQh7majZ(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		BindingOperations.GetBindingExpressionBase((DependencyObject)(object)(ComparisonNode)(object)P_0, BindingToTriggerProperty)?.UpdateSource();
	}

	public ComparisonNode()
	{
	}

	static ComparisonNode()
	{
		ValueProperty = DependencyProperty.Register("Value", typeof(PvfTreeFileRename), typeof(ComparisonNode), new PropertyMetadata((object)null, new PropertyChangedCallback(UJaQh7majZ)));
		BindingToTriggerProperty = DependencyProperty.Register("BindingToTrigger", typeof(object), typeof(ComparisonNode), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		SourceProperty = DependencyProperty.Register("Source", typeof(IDictionary<string, PvfTreeFileBase>), typeof(ComparisonNode), new PropertyMetadata((PropertyChangedCallback)null));
	}
}
