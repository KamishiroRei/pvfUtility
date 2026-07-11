using System;
using System.Globalization;
using System.Text;
using System.Windows.Documents;

namespace ICSharpCode.AvalonEdit.Document;

public static class TextUtilities
{
	private static readonly string[] c0Table = new string[32]
	{
		"NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS", "HT",
		"LF", "VT", "FF", "CR", "SO", "SI", "DLE", "DC1", "DC2", "DC3",
		"DC4", "NAK", "SYN", "ETB", "CAN", "EM", "SUB", "ESC", "FS", "GS",
		"RS", "US"
	};

	private static readonly string[] delAndC1Table = new string[33]
	{
		"DEL", "PAD", "HOP", "BPH", "NBH", "IND", "NEL", "SSA", "ESA", "HTS",
		"HTJ", "VTS", "PLD", "PLU", "RI", "SS2", "SS3", "DCS", "PU1", "PU2",
		"STS", "CCH", "MW", "SPA", "EPA", "SOS", "SGCI", "SCI", "CSI", "ST",
		"OSC", "PM", "APC"
	};

	public static int FindNextNewLine(ITextSource text, int offset, out string newLineType)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (offset < 0 || offset > text.TextLength)
		{
			throw new ArgumentOutOfRangeException("offset", offset, "offset is outside of text source");
		}
		SimpleSegment simpleSegment = NewLineFinder.NextNewLine(text, offset);
		if (simpleSegment == SimpleSegment.Invalid)
		{
			newLineType = null;
			return -1;
		}
		if (simpleSegment.Length == 2)
		{
			newLineType = "\r\n";
		}
		else if (text.GetCharAt(simpleSegment.Offset) == '\n')
		{
			newLineType = "\n";
		}
		else
		{
			newLineType = "\r";
		}
		return simpleSegment.Offset;
	}

	public static bool IsNewLine(string newLine)
	{
		if (!(newLine == "\r\n") && !(newLine == "\n"))
		{
			return newLine == "\r";
		}
		return true;
	}

	public static string NormalizeNewLines(string input, string newLine)
	{
		if (input == null)
		{
			return null;
		}
		if (!IsNewLine(newLine))
		{
			throw new ArgumentException("newLine must be one of the known newline sequences");
		}
		SimpleSegment simpleSegment = NewLineFinder.NextNewLine(input, 0);
		if (simpleSegment == SimpleSegment.Invalid)
		{
			return input;
		}
		StringBuilder stringBuilder = new StringBuilder(input.Length);
		int num = 0;
		do
		{
			stringBuilder.Append(input, num, simpleSegment.Offset - num);
			stringBuilder.Append(newLine);
			num = simpleSegment.EndOffset;
			simpleSegment = NewLineFinder.NextNewLine(input, num);
		}
		while (simpleSegment != SimpleSegment.Invalid);
		stringBuilder.Append(input, num, input.Length - num);
		return stringBuilder.ToString();
	}

	public static string GetNewLineFromDocument(IDocument document, int lineNumber)
	{
		IDocumentLine documentLine = document.GetLineByNumber(lineNumber);
		if (documentLine.DelimiterLength == 0)
		{
			documentLine = documentLine.PreviousLine;
			if (documentLine == null)
			{
				return Environment.NewLine;
			}
		}
		return document.GetText(documentLine.Offset + documentLine.Length, documentLine.DelimiterLength);
	}

	public static string GetControlCharacterName(char controlCharacter)
	{
		int num = controlCharacter;
		if (num < c0Table.Length)
		{
			return c0Table[num];
		}
		if (num >= 127 && num <= 159)
		{
			return delAndC1Table[num - 127];
		}
		return num.ToString("x4", CultureInfo.InvariantCulture);
	}

	public static ISegment GetWhitespaceAfter(ITextSource textSource, int offset)
	{
		if (textSource == null)
		{
			throw new ArgumentNullException("textSource");
		}
		int i;
		for (i = offset; i < textSource.TextLength; i++)
		{
			char charAt = textSource.GetCharAt(i);
			if (charAt != ' ' && charAt != '\t')
			{
				break;
			}
		}
		return new SimpleSegment(offset, i - offset);
	}

	public static ISegment GetWhitespaceBefore(ITextSource textSource, int offset)
	{
		if (textSource == null)
		{
			throw new ArgumentNullException("textSource");
		}
		int num;
		for (num = offset - 1; num >= 0; num--)
		{
			char charAt = textSource.GetCharAt(num);
			if (charAt != ' ' && charAt != '\t')
			{
				break;
			}
		}
		num++;
		return new SimpleSegment(num, offset - num);
	}

	public static ISegment GetLeadingWhitespace(TextDocument document, DocumentLine documentLine)
	{
		if (documentLine == null)
		{
			throw new ArgumentNullException("documentLine");
		}
		return GetWhitespaceAfter(document, documentLine.Offset);
	}

	public static ISegment GetTrailingWhitespace(TextDocument document, DocumentLine documentLine)
	{
		if (documentLine == null)
		{
			throw new ArgumentNullException("documentLine");
		}
		ISegment whitespaceBefore = GetWhitespaceBefore(document, documentLine.EndOffset);
		if (whitespaceBefore.Offset == documentLine.Offset)
		{
			return new SimpleSegment(documentLine.EndOffset, 0);
		}
		return whitespaceBefore;
	}

	public static ISegment GetSingleIndentationSegment(ITextSource textSource, int offset, int indentationSize)
	{
		if (textSource == null)
		{
			throw new ArgumentNullException("textSource");
		}
		int i;
		for (i = offset; i < textSource.TextLength; i++)
		{
			switch (textSource.GetCharAt(i))
			{
			case '\t':
				if (i == offset)
				{
					return new SimpleSegment(offset, 1);
				}
				break;
			case ' ':
				if (i - offset < indentationSize)
				{
					continue;
				}
				break;
			}
			break;
		}
		return new SimpleSegment(offset, i - offset);
	}

	public static CharacterClass GetCharacterClass(char c)
	{
		switch (c)
		{
		case '\n':
		case '\r':
			return CharacterClass.LineTerminator;
		case '_':
			return CharacterClass.IdentifierPart;
		default:
			return GetCharacterClass(char.GetUnicodeCategory(c));
		}
	}

	private static CharacterClass GetCharacterClass(char highSurrogate, char lowSurrogate)
	{
		if (char.IsSurrogatePair(highSurrogate, lowSurrogate))
		{
			return GetCharacterClass(char.GetUnicodeCategory(highSurrogate.ToString() + lowSurrogate, 0));
		}
		return CharacterClass.Other;
	}

	private static CharacterClass GetCharacterClass(UnicodeCategory c)
	{
		switch (c)
		{
		case UnicodeCategory.SpaceSeparator:
		case UnicodeCategory.LineSeparator:
		case UnicodeCategory.ParagraphSeparator:
		case UnicodeCategory.Control:
			return CharacterClass.Whitespace;
		case UnicodeCategory.UppercaseLetter:
		case UnicodeCategory.LowercaseLetter:
		case UnicodeCategory.TitlecaseLetter:
		case UnicodeCategory.ModifierLetter:
		case UnicodeCategory.OtherLetter:
		case UnicodeCategory.DecimalDigitNumber:
			return CharacterClass.IdentifierPart;
		case UnicodeCategory.NonSpacingMark:
		case UnicodeCategory.SpacingCombiningMark:
		case UnicodeCategory.EnclosingMark:
			return CharacterClass.CombiningMark;
		default:
			return CharacterClass.Other;
		}
	}

	public static int GetNextCaretPosition(ITextSource textSource, int offset, LogicalDirection direction, CaretPositioningMode mode)
	{
		if (textSource == null)
		{
			throw new ArgumentNullException("textSource");
		}
		if ((uint)mode > 5u)
		{
			throw new ArgumentException("Unsupported CaretPositioningMode: " + mode, "mode");
		}
		if (direction != LogicalDirection.Backward && direction != LogicalDirection.Forward)
		{
			throw new ArgumentException("Invalid LogicalDirection: " + direction, "direction");
		}
		int textLength = textSource.TextLength;
		if (textLength <= 0)
		{
			if (IsNormal(mode))
			{
				if (offset > 0 && direction == LogicalDirection.Backward)
				{
					return 0;
				}
				if (offset < 0 && direction == LogicalDirection.Forward)
				{
					return 0;
				}
			}
			return -1;
		}
		int num;
		while (true)
		{
			num = ((direction == LogicalDirection.Backward) ? (offset - 1) : (offset + 1));
			if (num < 0 || num > textLength)
			{
				return -1;
			}
			if (num == 0)
			{
				if (IsNormal(mode) || !char.IsWhiteSpace(textSource.GetCharAt(0)))
				{
					return num;
				}
			}
			else if (num == textLength)
			{
				if (mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol && (IsNormal(mode) || !char.IsWhiteSpace(textSource.GetCharAt(textLength - 1))))
				{
					return num;
				}
			}
			else
			{
				char charAt = textSource.GetCharAt(num - 1);
				char charAt2 = textSource.GetCharAt(num);
				if (!char.IsSurrogatePair(charAt, charAt2))
				{
					CharacterClass characterClass = GetCharacterClass(charAt);
					CharacterClass characterClass2 = GetCharacterClass(charAt2);
					if (char.IsLowSurrogate(charAt) && num >= 2)
					{
						characterClass = GetCharacterClass(textSource.GetCharAt(num - 2), charAt);
					}
					if (char.IsHighSurrogate(charAt2) && num + 1 < textLength)
					{
						characterClass2 = GetCharacterClass(charAt2, textSource.GetCharAt(num + 1));
					}
					if (StopBetweenCharacters(mode, characterClass, characterClass2))
					{
						break;
					}
				}
			}
			offset = num;
		}
		return num;
	}

	private static bool IsNormal(CaretPositioningMode mode)
	{
		if (mode != CaretPositioningMode.Normal)
		{
			return mode == CaretPositioningMode.EveryCodepoint;
		}
		return true;
	}

	private static bool StopBetweenCharacters(CaretPositioningMode mode, CharacterClass charBefore, CharacterClass charAfter)
	{
		if (mode == CaretPositioningMode.EveryCodepoint)
		{
			return true;
		}
		if (charAfter == CharacterClass.CombiningMark)
		{
			return false;
		}
		if (mode == CaretPositioningMode.Normal)
		{
			return true;
		}
		if (charBefore == charAfter)
		{
			if (charBefore == CharacterClass.Other && (mode == CaretPositioningMode.WordBorderOrSymbol || mode == CaretPositioningMode.WordStartOrSymbol))
			{
				return true;
			}
		}
		else if ((mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol) || (charAfter != CharacterClass.Whitespace && charAfter != CharacterClass.LineTerminator))
		{
			return true;
		}
		return false;
	}
}
