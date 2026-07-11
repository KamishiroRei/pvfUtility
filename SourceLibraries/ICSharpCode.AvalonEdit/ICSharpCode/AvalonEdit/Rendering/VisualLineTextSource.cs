using System;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class VisualLineTextSource : TextSource, ITextRunConstructionContext
{
	private string cachedString;

	private int cachedStringOffset;

	public VisualLine VisualLine { get; private set; }

	public TextView TextView { get; set; }

	public TextDocument Document { get; set; }

	public TextRunProperties GlobalTextRunProperties { get; set; }

	public VisualLineTextSource(VisualLine visualLine)
	{
		VisualLine = visualLine;
	}

	public override TextRun GetTextRun(int textSourceCharacterIndex)
	{
		try
		{
			foreach (VisualLineElement element in VisualLine.Elements)
			{
				if (textSourceCharacterIndex >= element.VisualColumn && textSourceCharacterIndex < element.VisualColumn + element.VisualLength)
				{
					int num = textSourceCharacterIndex - element.VisualColumn;
					TextRun textRun = element.CreateTextRun(textSourceCharacterIndex, this);
					if (textRun == null)
					{
						throw new ArgumentNullException(element.GetType().Name + ".CreateTextRun");
					}
					if (textRun.Length == 0)
					{
						throw new ArgumentException("The returned TextRun must not have length 0.", element.GetType().Name + ".Length");
					}
					if (num + textRun.Length > element.VisualLength)
					{
						throw new ArgumentException("The returned TextRun is too long.", element.GetType().Name + ".CreateTextRun");
					}
					if (textRun is InlineObjectRun inlineObjectRun)
					{
						inlineObjectRun.VisualLine = VisualLine;
						VisualLine.hasInlineObjects = true;
						TextView.AddInlineObject(inlineObjectRun);
					}
					return textRun;
				}
			}
			if (TextView.Options.ShowEndOfLine && textSourceCharacterIndex == VisualLine.VisualLength)
			{
				return CreateTextRunForNewLine();
			}
			return new TextEndOfParagraph(1);
		}
		catch (Exception)
		{
			throw;
		}
	}

	private TextRun CreateTextRunForNewLine()
	{
		string text = "";
		DocumentLine lastDocumentLine = VisualLine.LastDocumentLine;
		if (lastDocumentLine.DelimiterLength == 2)
		{
			text = "¶";
		}
		else if (lastDocumentLine.DelimiterLength == 1)
		{
			text = Document.GetCharAt(lastDocumentLine.Offset + lastDocumentLine.Length) switch
			{
				'\r' => "\\r", 
				'\n' => "\\n", 
				_ => "?", 
			};
		}
		return new FormattedTextRun(new FormattedTextElement(TextView.cachedElements.GetTextForNonPrintableCharacter(text, this), 0), GlobalTextRunProperties);
	}

	public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
	{
		try
		{
			foreach (VisualLineElement element in VisualLine.Elements)
			{
				if (textSourceCharacterIndexLimit > element.VisualColumn && textSourceCharacterIndexLimit <= element.VisualColumn + element.VisualLength)
				{
					TextSpan<CultureSpecificCharacterBufferRange> precedingText = element.GetPrecedingText(textSourceCharacterIndexLimit, this);
					if (precedingText != null)
					{
						int num = textSourceCharacterIndexLimit - element.VisualColumn;
						if (precedingText.Length > num)
						{
							throw new ArgumentException("The returned TextSpan is too long.", element.GetType().Name + ".GetPrecedingText");
						}
						return precedingText;
					}
					break;
				}
			}
			CharacterBufferRange empty = CharacterBufferRange.Empty;
			return new TextSpan<CultureSpecificCharacterBufferRange>(empty.Length, new CultureSpecificCharacterBufferRange(null, empty));
		}
		catch (Exception)
		{
			throw;
		}
	}

	public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
	{
		throw new NotSupportedException();
	}

	public StringSegment GetText(int offset, int length)
	{
		if (cachedString != null && offset >= cachedStringOffset && offset + length <= cachedStringOffset + cachedString.Length)
		{
			return new StringSegment(cachedString, offset - cachedStringOffset, length);
		}
		cachedStringOffset = offset;
		return new StringSegment(cachedString = Document.GetText(offset, length));
	}
}
