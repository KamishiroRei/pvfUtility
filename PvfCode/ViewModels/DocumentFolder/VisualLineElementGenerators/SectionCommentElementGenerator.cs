using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode.Controls.TextEditorFolder;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.VisualLineElementGenerators;

internal class SectionCommentElementGenerator : VisualLineElementGenerator, IDisposable
{
	private readonly PvfFile file;

	private readonly TextEditorBase editor;

	public SectionCommentElementGenerator(TextEditorBase editor, PvfFile file)
	{
		this.editor = editor;
		this.file = file;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		TextDocument document = CurrentContext.Document;
		DocumentLine line = document.GetLineByOffset(offset);
		if (CurrentContext.TextView.GetLineIsCollapsed(line.LineNumber))
		{
			return null;
		}

		string lineText = document.GetText(line);
		if (string.IsNullOrEmpty(lineText) || lineText.IndexOf('`') == -1)
		{
			return null;
		}

		string sectionName = StrHelper.GetMiddleStr(lineText, "`", "`");
		if (!string.IsNullOrEmpty(sectionName))
		{
			sectionName = sectionName.ToLower();
		}

		TextSegment segment = new TextSegment
		{
			StartOffset = line.Offset,
			EndOffset = line.EndOffset,
			Length = line.Length
		};
		return new InlineObjectElement(
			0,
			new LstSectionCommentView(file, sectionName, SelectSegment, segment));
	}

	private void SelectSegment(TextSegment segment)
	{
		try
		{
			editor.Select(segment.StartOffset, segment.Length);
		}
		catch (Exception)
		{
		}
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		DocumentLine line = CurrentContext.VisualLine.LastDocumentLine;
		if (CurrentContext.TextView.GetLineIsCollapsed(line.LineNumber))
		{
			return -1;
		}
		return line.Length > 0 ? line.Offset + line.Length : -1;
	}

	public void Dispose()
	{
	}
}
