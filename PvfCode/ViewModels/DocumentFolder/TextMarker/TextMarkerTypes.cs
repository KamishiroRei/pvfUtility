using System;

namespace PvfCode.ViewModels.DocumentFolder.TextMarker;

[Flags]
public enum TextMarkerTypes
{
	None = 0,
	SquigglyUnderline = 1,
	NormalUnderline = 2,
	DottedUnderline = 4,
	LineInScrollBar = 0x100,
	ScrollBarRightTriangle = 0x400,
	ScrollBarLeftTriangle = 0x800,
	CircleInScrollBar = 0x1000
}
