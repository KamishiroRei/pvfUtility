namespace ICSharpCode.AvalonEdit.Document;

internal static class NewLineFinder
{
	private static readonly char[] newline = new char[2] { '\r', '\n' };

	internal static readonly string[] NewlineStrings = new string[3] { "\r\n", "\r", "\n" };

	internal static SimpleSegment NextNewLine(string text, int offset)
	{
		int num = text.IndexOfAny(newline, offset);
		if (num >= 0)
		{
			if (text[num] == '\r' && num + 1 < text.Length && text[num + 1] == '\n')
			{
				return new SimpleSegment(num, 2);
			}
			return new SimpleSegment(num, 1);
		}
		return SimpleSegment.Invalid;
	}

	internal static SimpleSegment NextNewLine(ITextSource text, int offset)
	{
		int textLength = text.TextLength;
		int num = text.IndexOfAny(newline, offset, textLength - offset);
		if (num >= 0)
		{
			if (text.GetCharAt(num) == '\r' && num + 1 < textLength && text.GetCharAt(num + 1) == '\n')
			{
				return new SimpleSegment(num, 2);
			}
			return new SimpleSegment(num, 1);
		}
		return SimpleSegment.Invalid;
	}
}
