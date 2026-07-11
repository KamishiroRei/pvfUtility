using System;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm;

namespace PvfCode.LoggerBase;

public class NotificationViewModel : NotificationViewModelBase
{
	public NotificationViewModel(string title, string message, ImageSource icon, string? buttonTitle = null, Action? methods = null)
	{
		base.Title = title;
		base.Message = message;
		base.Icon = icon;
		if (methods != null)
		{
			base.MethodsCommand = new DelegateCommand(methods);
			if (buttonTitle != null)
			{
				base.ButtonTitle = buttonTitle;
				base.ButtonVisibility = Visibility.Visible;
			}
		}
	}
}
public class NotificationViewModel<T> : NotificationViewModelBase
{
	public ICommand<T> Command2 { get; set; }

	public NotificationViewModel(string title, string message, ImageSource icon, string? buttonTitle = null, ICommand<T>? command = null)
	{
		base.Title = title;
		base.Message = message;
		base.Icon = icon;
		if (command != null)
		{
			Command2 = command;
			if (buttonTitle != null)
			{
				base.ButtonTitle = buttonTitle;
				base.ButtonVisibility2 = Visibility.Visible;
			}
		}
	}
}
