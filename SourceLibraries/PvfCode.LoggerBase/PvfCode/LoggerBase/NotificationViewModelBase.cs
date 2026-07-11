using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PvfCode.LoggerBase;

public class NotificationViewModelBase : ModelBase
{
	private string _ButtonTitle;

	private string _Title;

	private string _Message;

	private ImageSource _Icon;

	public string ButtonTitle
	{
		get
		{
			return _ButtonTitle;
		}
		set
		{
			_ButtonTitle = value;
			DoNotify("ButtonTitle");
		}
	}

	public Visibility ButtonVisibility { get; set; }

	public Visibility ButtonVisibility2 { get; set; }

	public string Title
	{
		get
		{
			return _Title;
		}
		set
		{
			_Title = value;
		}
	}

	public string Message
	{
		get
		{
			return _Message;
		}
		set
		{
			_Message = value;
		}
	}

	public ImageSource Icon
	{
		get
		{
			return _Icon;
		}
		set
		{
			_Icon = value;
		}
	}

	public ICommand MethodsCommand { get; set; }

	public NotificationViewModelBase()
	{
		ButtonVisibility = Visibility.Collapsed;
		ButtonVisibility2 = Visibility.Collapsed;
	}
}
