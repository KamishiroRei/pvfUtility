using System;
using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Dot.Desktop;

public class MacroDataRes : ModelBase
{
	private string _Title;

	private string _Instructions;

	private string _DetailedInstructions;

	public string Title
	{
		get
		{
			if (_Title == null)
			{
				_Title = string.Empty;
			}
			return _Title;
		}
		set
		{
			_Title = value;
			DoNotify("Title");
		}
	}

	public string Instructions
	{
		get
		{
			if (_Instructions == null)
			{
				_Instructions = string.Empty;
			}
			return _Instructions;
		}
		set
		{
			_Instructions = value;
			DoNotify("Instructions");
		}
	}

	public string DetailedInstructions
	{
		get
		{
			if (_DetailedInstructions == null)
			{
				_DetailedInstructions = string.Empty;
			}
			return _DetailedInstructions;
		}
		set
		{
			_DetailedInstructions = value;
			DoNotify("DetailedInstructions");
		}
	}

	public object Data { get; set; }

	public MacroType MacroType { get; set; }

	public DateTime Create { get; set; }
}
