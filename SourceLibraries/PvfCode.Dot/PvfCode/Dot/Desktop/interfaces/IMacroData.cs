using System;
using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Dot.Desktop.interfaces;

public interface IMacroData
{
	string Title { get; set; }

	string Instructions { get; set; }

	string DetailedInstructions { get; set; }

	object Data { get; set; }

	MacroType MacroType { get; set; }

	int DownLoadNumber { get; set; }

	DateTime Create { get; set; }
}
