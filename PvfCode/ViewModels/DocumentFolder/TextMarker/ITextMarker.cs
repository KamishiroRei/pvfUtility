using System;
using System.Windows;
using System.Windows.Media;

namespace PvfCode.ViewModels.DocumentFolder.TextMarker;

public interface ITextMarker
{
	int StartOffset { get; }

	int EndOffset { get; }

	int Length { get; }

	bool IsDeleted { get; }

	Color? BackgroundColor { get; set; }

	Color? ForegroundColor { get; set; }

	FontWeight? FontWeight { get; set; }

	FontStyle? FontStyle { get; set; }

	TextMarkerTypes MarkerTypes { get; set; }

	Color MarkerColor { get; set; }

	object Tag { get; set; }

	object ToolTip { get; set; }

	event EventHandler Deleted;

	void Delete();
}
