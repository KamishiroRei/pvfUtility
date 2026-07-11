using System.ComponentModel;

namespace Swordfish.NET.Collections.Auxiliary;

public class ExtendedPropertyChangedEventArgs : PropertyChangedEventArgs
{
	public object OldValue { get; }

	public object NewValue { get; }

	public ExtendedPropertyChangedEventArgs(string propertyName, object oldValue, object newValue)
		: base(propertyName)
	{
		OldValue = oldValue;
		NewValue = newValue;
	}
}
