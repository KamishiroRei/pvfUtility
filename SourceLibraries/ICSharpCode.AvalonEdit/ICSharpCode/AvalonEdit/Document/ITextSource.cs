using System;
using System.IO;

namespace ICSharpCode.AvalonEdit.Document;

public interface ITextSource
{
	ITextSourceVersion Version { get; }

	int TextLength { get; }

	string Text { get; }

	ITextSource CreateSnapshot();

	ITextSource CreateSnapshot(int offset, int length);

	TextReader CreateReader();

	TextReader CreateReader(int offset, int length);

	char GetCharAt(int offset);

	string GetText(int offset, int length);

	string GetText(ISegment segment);

	void WriteTextTo(TextWriter writer);

	void WriteTextTo(TextWriter writer, int offset, int length);

	int IndexOf(char c, int startIndex, int count);

	int IndexOfAny(char[] anyOf, int startIndex, int count);

	int IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType);

	int LastIndexOf(char c, int startIndex, int count);

	int LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType);
}
