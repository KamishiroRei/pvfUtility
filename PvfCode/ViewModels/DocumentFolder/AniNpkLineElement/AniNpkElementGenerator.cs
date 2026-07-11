using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.AniNpkLineElement;

public class AniNpkElementGenerator : VisualLineElementGenerator, IDisposable
{
	private readonly IHighlighter Hn0G553wl7;

	public AniNpkElementGenerator(IHighlighter currenHighlighter)
	{
		Hn0G553wl7 = currenHighlighter;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		DocumentLine lineByOffset = base.CurrentContext.Document.GetLineByOffset(offset);
		if (base.CurrentContext.TextView.GetLineIsCollapsed(lineByOffset.LineNumber))
		{
			return null;
		}
		HighlightedLine highlightedLine = Hn0G553wl7.HighlightLine(lineByOffset.LineNumber);
		if (highlightedLine != null && highlightedLine.Sections != null && highlightedLine.Sections.Count > 0)
		{
			HighlightedSection highlightedSection = highlightedLine.Sections[highlightedLine.Sections.Count - 1];
			if (highlightedSection.Color.Name == "String")
			{
				string text = base.CurrentContext.Document.GetText(highlightedSection).Replace("`", string.Empty);
				if (Path.GetExtension(text).ToLower() == ".img" && offset == lineByOffset.EndOffset)
				{
					DocumentLine nextLine = lineByOffset.NextLine;
					if (nextLine != null)
					{
						highlightedLine = Hn0G553wl7.HighlightLine(nextLine.LineNumber);
						if (highlightedLine != null && highlightedLine.Sections != null && highlightedLine.Sections.Count == 1)
						{
							HighlightedSection highlightedSection2 = highlightedLine.Sections[0];
							if (highlightedSection2.Color.Name == "Digits")
							{
								int result;
								int value = (int.TryParse(base.CurrentContext.Document.GetText(highlightedSection2), out result) ? result : (-1));
								return new InlineObjectElement(0, new ImgRightVirtualLineView(new KeyValuePair<string, int>(text, value)));
							}
						}
					}
				}
			}
		}
		return null;
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		if (base.CurrentContext.TextView.GetLineIsCollapsed(base.CurrentContext.VisualLine.LastDocumentLine.LineNumber))
		{
			return -1;
		}
		Q7eGfggC95(startOffset, out var result);
		return result;
	}

	private bool Q7eGfggC95(int P_0, out int P_1)
	{
		DocumentLine lastDocumentLine = base.CurrentContext.VisualLine.LastDocumentLine;
		if (lastDocumentLine.Length > 0)
		{
			HighlightedLine highlightedLine = Hn0G553wl7.HighlightLine(lastDocumentLine.LineNumber);
			if (highlightedLine != null && highlightedLine.Sections != null && highlightedLine.Sections.Count > 0)
			{
				HighlightedSection highlightedSection = highlightedLine.Sections[highlightedLine.Sections.Count - 1];
				if (highlightedSection.Color.Name == "String" && Path.GetExtension(base.CurrentContext.Document.GetText(highlightedSection).Replace("`", string.Empty)).ToLower() == ".img")
				{
					P_1 = lastDocumentLine.Offset + lastDocumentLine.Length;
					return true;
				}
			}
		}
		P_1 = -1;
		return false;
	}

	public void Dispose()
	{
	}
}
