using System.Windows;

namespace PvfCode.ValidationRules.TreeList;

public class BindingProxy : Freezable
{
	public static readonly DependencyProperty DataProperty;

	public object Data
	{
		get
		{
			return ((DependencyObject)this).GetValue(DataProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DataProperty, value);
		}
	}

	protected override Freezable CreateInstanceCore()
	{
		return (Freezable)(object)new BindingProxy();
	}

	public BindingProxy()
	{
	}

	static BindingProxy()
	{
		DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), (PropertyMetadata)(object)new UIPropertyMetadata(null));
	}
}
