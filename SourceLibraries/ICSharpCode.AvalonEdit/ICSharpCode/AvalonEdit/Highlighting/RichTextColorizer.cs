using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Highlighting;

public class RichTextColorizer : DocumentColorizingTransformer
{
	private readonly RichTextModel richTextModel;

	public RichTextColorizer(RichTextModel richTextModel)
	{
		if (richTextModel == null)
		{
			throw new ArgumentNullException("richTextModel");
		}
		this.richTextModel = richTextModel;
	}

	protected override void ColorizeLine(DocumentLine line)
	{
		foreach (HighlightedSection section in richTextModel.GetHighlightedSections(line.Offset, line.Length))
		{
			if (!HighlightingColorizer.IsEmptyColor(section.Color))
			{
				ChangeLinePart(section.Offset, section.Offset + section.Length, delegate(VisualLineElement visualLineElement)
				{
					HighlightingColorizer.ApplyColorToElement(visualLineElement, section.Color, base.CurrentContext);
				});
			}
		}
	}
}
