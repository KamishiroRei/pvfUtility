using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace TextEditLib.Extensions;

internal class HighlightCurrentLineBackgroundRendererDefault : IBackgroundRenderer
{
	private readonly TextEditDefault _Editor;

	public KnownLayer Layer => KnownLayer.Background;

	public HighlightCurrentLineBackgroundRendererDefault(TextEditDefault editor)
		: this()
	{
		_Editor = editor;
	}

	protected HighlightCurrentLineBackgroundRendererDefault()
	{
	}

	public void Draw(TextView textView, DrawingContext drawingContext)
	{
		if (_Editor == null || _Editor.Document == null || (_Editor.EditorCurrentLineBorderThickness == 0.0 && _Editor.EditorCurrentLineBackground == null))
		{
			return;
		}
		Pen pen = null;
		if (_Editor.EditorCurrentLineBorder != null)
		{
			pen = new Pen(_Editor.EditorCurrentLineBorder, _Editor.EditorCurrentLineBorderThickness);
			if (((Freezable)pen).CanFreeze)
			{
				((Freezable)pen).Freeze();
			}
		}
		textView.EnsureVisualLines();
		DocumentLine lineByOffset = _Editor.Document.GetLineByOffset(_Editor.CaretOffset);
		foreach (Rect item in BackgroundGeometryBuilder.GetRectsForSegment(textView, lineByOffset))
		{
			Rect current = item;
			drawingContext.DrawRectangle(_Editor.EditorCurrentLineBackground, pen, new Rect(current.Location, new Size(textView.ActualWidth, current.Height)));
		}
	}
}
