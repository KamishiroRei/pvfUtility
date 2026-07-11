using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace UnitComboLib.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
	{
		MemberExpression memberExpression = ((!(property.Body is UnaryExpression)) ? ((MemberExpression)property.Body) : ((MemberExpression)((UnaryExpression)property.Body).Operand));
		OnPropertyChanged(memberExpression.Member.Name);
	}

	private void OnPropertyChanged(string propertyName)
	{
		try
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		catch
		{
		}
	}
}
