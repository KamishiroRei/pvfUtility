using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.KorElementGenerator;

internal class StringQuoteElementGenerator : VisualLineElementGenerator, IDisposable
{
	private readonly DocumentHighlighter highlighter;

	public StringQuoteElementGenerator(DocumentHighlighter highlighter)
	{
		this.highlighter = highlighter;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		TextDocument document = CurrentContext.Document;
		DocumentLine line = CurrentContext.VisualLine.LastDocumentLine;
		if (line.Length <= 0)
		{
			return null;
		}

		HighlightedLine highlightedLine = highlighter.HighlightLine(line.LineNumber);
		if (highlightedLine?.Sections == null || highlightedLine.Sections.Count != 2)
		{
			return null;
		}

		HighlightedSection indexSection = highlightedLine.Sections[0];
		if (indexSection.Color.Name != "Section")
		{
			return null;
		}

		string indexText = document.GetText(indexSection);
		if (indexText.Length <= 2)
		{
			return null;
		}

		int index = int.TryParse(indexText.Substring(1, indexText.Length - 2), out int parsedIndex)
			? parsedIndex
			: -1;
		string value = document.GetText(highlightedLine.Sections[1]);
		return new InlineObjectElement(0, new StringQuote(value, index, SourceType.StringTable));
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		if (!AppSetting.Instance.EditConfig.OpenKorStrQuote)
		{
			return -1;
		}

		DocumentLine line = CurrentContext.VisualLine.LastDocumentLine;
		if (line.Length <= 0)
		{
			return -1;
		}

		HighlightedLine highlightedLine = highlighter.HighlightLine(line.LineNumber);
		if (highlightedLine?.Sections == null || highlightedLine.Sections.Count != 2)
		{
			return -1;
		}

		return highlightedLine.Sections[0].Color.Name == "Section" &&
			highlightedLine.Sections[1].Color.Name == "String"
			? line.Offset + line.Length
			: -1;
	}

	public void Dispose()
	{
	}
}
