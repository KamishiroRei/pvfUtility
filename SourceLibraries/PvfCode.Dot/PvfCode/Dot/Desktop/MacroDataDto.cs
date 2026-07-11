using System;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Dot.Desktop.interfaces;

namespace PvfCode.Dot.Desktop;

public class MacroDataDto : IMacroData
{
	private string _NickName;

	public int Id { get; set; }

	public string Title { get; set; }

	public string Instructions { get; set; }

	public string DetailedInstructions { get; set; }

	public object Data { get; set; }

	public MacroType MacroType { get; set; }

	public bool IsShare { get; set; }

	public int DownLoadNumber { get; set; }

	public string NickName
	{
		get
		{
			if (string.IsNullOrEmpty(_NickName))
			{
				_NickName = "热心网友";
			}
			return _NickName;
		}
		set
		{
			_NickName = value;
		}
	}

	public string Avatar { get; set; }

	public DateTime Create { get; set; }
}
