using System;
using System.Collections.Generic;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class TextViewCachedElements : IDisposable
{
	private TextFormatter formatter;

	private Dictionary<string, TextLine> nonPrintableCharacterTexts;

	public TextLine GetTextForNonPrintableCharacter(string text, ITextRunConstructionContext context)
	{
		if (nonPrintableCharacterTexts == null)
		{
			nonPrintableCharacterTexts = new Dictionary<string, TextLine>();
		}
		if (!nonPrintableCharacterTexts.TryGetValue(text, out var value))
		{
			VisualLineElementTextRunProperties visualLineElementTextRunProperties = new VisualLineElementTextRunProperties(context.GlobalTextRunProperties);
			visualLineElementTextRunProperties.SetForegroundBrush(context.TextView.NonPrintableCharacterBrush);
			if (formatter == null)
			{
				formatter = TextFormatterFactory.Create(context.TextView);
			}
			value = FormattedTextElement.PrepareText(formatter, text, visualLineElementTextRunProperties);
			nonPrintableCharacterTexts[text] = value;
		}
		return value;
	}

	public void Dispose()
	{
		if (nonPrintableCharacterTexts != null)
		{
			foreach (TextLine value in nonPrintableCharacterTexts.Values)
			{
				value.Dispose();
			}
		}
		if (formatter != null)
		{
			formatter.Dispose();
		}
	}
}
