using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode.Models.Pvf;

namespace PvfCode.ViewModels.DocumentFolder.KorElementGenerator;

public class KorFileElementGenerator : VisualLineElementGenerator, IDisposable
{
	private readonly DocumentHighlighter _highlighter;

	private readonly PvfFile _file;

	private int _stringTableIndex;

	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	public KorFileElementGenerator(DocumentHighlighter highlighter, PvfFile file)
	{
		_file = file;
		_highlighter = highlighter;
		_stringTableIndex = Pvf.Strview.Get_pvfstrlist().ToList()
			.FindIndex(item => item.StrFileName.ToLower() == _file.FileName);
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		TextDocument document = base.CurrentContext.Document;
		DocumentLine lastDocumentLine = base.CurrentContext.VisualLine.LastDocumentLine;
		if (lastDocumentLine.Length > 0)
		{
			HighlightedLine highlightedLine = _highlighter.HighlightLine(lastDocumentLine.LineNumber);
			if (highlightedLine == null || highlightedLine.Sections == null || highlightedLine.Sections.Count == 0)
			{
				return null;
			}
			if (highlightedLine.Sections[0].Color.Name == "KorStrIndex")
			{
				return new InlineObjectElement(0, new StringQuote(document.GetText(highlightedLine.Sections[0]), _stringTableIndex, SourceType.StringView));
			}
		}
		return null;
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		if (!AppSetting.Instance.EditConfig.OpenKorStrQuote)
		{
			return -1;
		}
		if (_stringTableIndex == -1)
		{
			return -1;
		}
		DocumentLine lastDocumentLine = base.CurrentContext.VisualLine.LastDocumentLine;
		if (lastDocumentLine.Length > 0)
		{
			HighlightedLine highlightedLine = _highlighter.HighlightLine(lastDocumentLine.LineNumber);
			if (highlightedLine == null || highlightedLine.Sections == null || highlightedLine.Sections.Count == 0)
			{
				return -1;
			}
			if (highlightedLine.Sections[0].Color.Name == "KorStrIndex")
			{
				return lastDocumentLine.Offset + lastDocumentLine.Length;
			}
		}
		return -1;
	}

	public void Dispose()
	{
	}
}
