using System;
using System.Diagnostics;
using System.Windows.Input;

namespace UnitComboLib.Command;

public class RelayCommand<T> : ICommand
{
	private readonly Action<T> mExecute;

	private readonly Predicate<T> mCanExecute;

	public event EventHandler CanExecuteChanged
	{
		add
		{
			if (mCanExecute != null)
			{
				CommandManager.RequerySuggested += value;
			}
		}
		remove
		{
			if (mCanExecute != null)
			{
				CommandManager.RequerySuggested -= value;
			}
		}
	}

	public RelayCommand(Action<T> execute)
		: this(execute, (Predicate<T>)null)
	{
	}

	public RelayCommand(Action<T> execute, Predicate<T> canExecute)
	{
		if (execute == null)
		{
			throw new ArgumentNullException("execute");
		}
		mExecute = execute;
		mCanExecute = canExecute;
	}

	[DebuggerStepThrough]
	public bool CanExecute(object parameter)
	{
		if (mCanExecute != null)
		{
			return mCanExecute((T)parameter);
		}
		return true;
	}

	public void Execute(object parameter)
	{
		mExecute((T)parameter);
	}
}
internal class RelayCommand : ICommand
{
	private readonly Action mExecute;

	private readonly Func<bool> mCanExecute;

	public event EventHandler CanExecuteChanged
	{
		add
		{
			if (mCanExecute != null)
			{
				CommandManager.RequerySuggested += value;
			}
		}
		remove
		{
			if (mCanExecute != null)
			{
				CommandManager.RequerySuggested -= value;
			}
		}
	}

	public RelayCommand(Action execute)
		: this(execute, null)
	{
	}

	public RelayCommand(RelayCommand inputRC)
		: this(inputRC.mExecute, inputRC.mCanExecute)
	{
	}

	public RelayCommand(Action execute, Func<bool> canExecute)
	{
		if (execute == null)
		{
			throw new ArgumentNullException("execute");
		}
		mExecute = execute;
		mCanExecute = canExecute;
	}

	[DebuggerStepThrough]
	public bool CanExecute(object parameter)
	{
		if (mCanExecute != null)
		{
			return mCanExecute();
		}
		return true;
	}

	public void Execute(object parameter)
	{
		mExecute();
	}
}
