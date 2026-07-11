using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

namespace PvfCode.ViewModels.DocumentFolder.Foldings;

public class TabFoldingStrategy
{
	private sealed class FoldRegion
	{
		public int Indentation { get; }

		public int LineOffset { get; }

		public int LineEndOffset { get; }

		public int StartOffset => LineOffset + Indentation - 1;

		public int TextLength => LineEndOffset - StartOffset;

		public FoldRegion(int indentation, int lineOffset, int lineEndOffset)
		{
			Indentation = indentation;
			LineOffset = lineOffset;
			LineEndOffset = lineEndOffset;
		}
	}

	private TextView _textView;

	public void UpdateFoldings(FoldingManager manager, TextDocument document, TextView textView)
	{
		_textView = textView;
		int firstErrorOffset;
		IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out firstErrorOffset);
		manager.UpdateFoldings(newFoldings, firstErrorOffset);
	}

	public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
	{
		firstErrorOffset = -1;
		return CreateNewFoldings(document);
	}

	public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document)
	{
		List<NewFolding> foldings = new List<NewFolding>();
		List<FoldRegion> openRegions = new List<FoldRegion>();
		int previousIndentation = 0;
		foreach (DocumentLine line in document.Lines)
		{
			int indentation = 0;
			for (int offset = line.Offset; offset < line.EndOffset && document.GetCharAt(offset) == '\t'; offset++)
			{
				indentation++;
			}
			if (indentation > previousIndentation)
			{
				openRegions.Add(new FoldRegion(indentation, line.PreviousLine.Offset, line.PreviousLine.EndOffset));
			}
			else if (indentation < previousIndentation)
			{
				foreach (FoldRegion region in openRegions.FindAll(region => region.Indentation > indentation))
				{
					NewFolding folding = new NewFolding(region.StartOffset, line.PreviousLine.EndOffset)
					{
						Name = document.GetText(region.StartOffset, region.TextLength)
					};
					foldings.Add(folding);
					openRegions.Remove(region);
				}
			}
			previousIndentation = indentation;
		}
		foreach (FoldRegion region in openRegions)
		{
			foldings.Add(new NewFolding(region.StartOffset, document.TextLength));
		}
		foldings.Sort((left, right) => left.StartOffset.CompareTo(right.StartOffset));
		return foldings;
	}
}
