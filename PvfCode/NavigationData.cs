using System;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace PvfCode;

public class NavigationData : ViewModelBase
{
	private int _line;

	private int _column;

	private bool _isChecked;

	internal Action<NavigationData> Navigate { get; set; }

	public string FilePath { get; set; }

	public int DocumentOffset { get; set; }

	public int Line
	{
		get
		{
			if (_line <= 0)
			{
				_line = 1;
			}
			return _line;
		}
		set
		{
			_line = value;
		}
	}

	public int Column
	{
		get
		{
			if (_column <= 0)
			{
				_column = 1;
			}
			return _column;
		}
		set
		{
			_column = value;
		}
	}

	public bool IsChecked
	{
		get
		{
			return _isChecked;
		}
		set
		{
			_isChecked = value;
			RaisePropertyChanged(nameof(IsChecked));
		}
	}

	public string Text { get; set; }

	public bool TextVisibility => !string.IsNullOrEmpty(Text);

	[Command]
	public void OnClick()
	{
		Navigate?.Invoke(this);
	}
}
