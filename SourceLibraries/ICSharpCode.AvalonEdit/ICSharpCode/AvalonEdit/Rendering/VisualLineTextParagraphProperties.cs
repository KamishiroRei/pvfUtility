using System.Windows;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class VisualLineTextParagraphProperties : TextParagraphProperties
{
	internal TextRunProperties defaultTextRunProperties;

	internal TextWrapping textWrapping;

	internal double tabSize;

	internal double indent;

	internal bool firstLineInParagraph;

	internal FlowDirection flowDirection;

	public override double DefaultIncrementalTab => tabSize;

	public override FlowDirection FlowDirection => flowDirection;

	public override TextAlignment TextAlignment => TextAlignment.Left;

	public override double LineHeight => double.NaN;

	public override bool FirstLineInParagraph => firstLineInParagraph;

	public override TextRunProperties DefaultTextRunProperties => defaultTextRunProperties;

	public override TextWrapping TextWrapping => textWrapping;

	public override TextMarkerProperties TextMarkerProperties => null;

	public override double Indent => indent;
}
