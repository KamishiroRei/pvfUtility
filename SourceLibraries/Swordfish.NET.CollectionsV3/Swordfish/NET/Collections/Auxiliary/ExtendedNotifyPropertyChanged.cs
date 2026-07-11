using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Swordfish.NET.Collections.Auxiliary;

public class ExtendedNotifyPropertyChanged : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	protected bool SetProperty<T>(ref T current, T newValue, [CallerMemberName] string propertyName = "")
	{
		if (!EqualityComparer<T>.Default.Equals(current, newValue))
		{
			T val = current;
			current = newValue;
			RaisePropertyChangedWithValues(propertyName, val, newValue);
			return true;
		}
		return false;
	}

	protected bool SetProperty<T>(ref T current, T newValue, params string[] propertyNames)
	{
		if (!EqualityComparer<T>.Default.Equals(current, newValue))
		{
			current = newValue;
			RaisePropertyChanged(propertyNames);
			return true;
		}
		return false;
	}

	protected virtual void RaisePropertyChanged(params string[] propertyNames)
	{
		foreach (string propertyName in propertyNames)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		if (propertyNames.Length == 0)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
		}
	}

	protected virtual void RaisePropertyChangedWithValues(string propertyName, object oldValue, object newValue)
	{
		this.PropertyChanged?.Invoke(this, new ExtendedPropertyChangedEventArgs(propertyName, oldValue, newValue));
	}
}
