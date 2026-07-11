using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using TextEditLib;

namespace PvfCode.Controls.TextEditorFolder;

internal static class TextEditExtensions
{
	public static Point ToDeviceIndependentPoint(this Point point, Visual visual)
	{
		Matrix transformFromDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
		return transformFromDevice.Transform(point);
	}

	internal static DocumentLine GetCaretDocumentLine(this TextEdit editor)
	{
		return editor.Document.GetLineByOffset(editor.CaretOffset);
	}

	internal static int GetCaretLineOffset(this TextEdit editor)
	{
		return editor.GetCaretDocumentLine()?.Offset ?? 0;
	}

	public static IList<HighlightedSection> GetCaretHighlightedSections(this TextEdit editor)
	{
		List<HighlightedSection> emptyResult = new List<HighlightedSection>();
		DocumentLine documentLine = editor.GetCaretDocumentLine();
		if (documentLine == null)
		{
			return emptyResult;
		}

		IHighlighter highlighter = (IHighlighter)editor.TextArea.GetService(typeof(IHighlighter));
		HighlightedLine highlightedLine = highlighter.HighlightLine(documentLine.LineNumber);
		return highlightedLine?.Sections ?? emptyResult;
	}

	public static bool IsCommentAt(this TextEdit editor, int line, int column)
	{
		IHighlighter highlighter = (IHighlighter)editor.TextArea.GetService(typeof(IHighlighter));
		if (highlighter == null)
		{
			return false;
		}

		int offset = editor.Document.GetOffset(line, column);
		return highlighter
			.HighlightLine(editor.Document.GetLineByNumber(line).LineNumber)
			.Sections
			.Any(section =>
				section.Offset <= offset &&
				section.Offset + section.Length >= offset &&
				section.Color.Name == "Comment");
	}
}
