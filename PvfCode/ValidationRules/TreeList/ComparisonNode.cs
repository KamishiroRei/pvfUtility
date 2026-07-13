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

	private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		BindingOperations.GetBindingExpressionBase((ComparisonNode)dependencyObject, BindingToTriggerProperty)?.UpdateSource();
	}

	public ComparisonNode()
	{
	}

	static ComparisonNode()
	{
		ValueProperty = DependencyProperty.Register("Value", typeof(PvfTreeFileRename), typeof(ComparisonNode), new PropertyMetadata((object)null, new PropertyChangedCallback(OnValueChanged)));
		BindingToTriggerProperty = DependencyProperty.Register("BindingToTrigger", typeof(object), typeof(ComparisonNode), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		SourceProperty = DependencyProperty.Register("Source", typeof(IDictionary<string, PvfTreeFileBase>), typeof(ComparisonNode), new PropertyMetadata((PropertyChangedCallback)null));
	}
}
