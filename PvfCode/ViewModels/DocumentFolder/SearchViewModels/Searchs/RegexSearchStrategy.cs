using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.SearchViewModels.Searchs;

public class RegexSearchStrategy : ISearchStrategy, IEquatable<ISearchStrategy>
{
	private readonly Regex NYOAgi44Jk;

	private readonly bool YMpA6aWpY2;

	public RegexSearchStrategy(Regex searchPattern, bool matchWholeWords)
	{
		if (searchPattern == null)
		{
			throw new ArgumentNullException("searchPattern");
		}
		NYOAgi44Jk = searchPattern;
		YMpA6aWpY2 = matchWholeWords;
	}

	public IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length)
	{
		int endOffset = offset + length;
		foreach (Match item in NYOAgi44Jk.Matches(document.Text))
		{
			int num = item.Length + item.Index;
			if (offset <= item.Index && endOffset >= num && (!YMpA6aWpY2 || (YrVAaBlsEj(document, item.Index) && YrVAaBlsEj(document, num))))
			{
				yield return new SearchResult
				{
					StartOffset = item.Index,
					Length = item.Length,
					Data = item
				};
			}
		}
	}

	private static bool YrVAaBlsEj(ITextSource P_0, int P_1)
	{
		return TextUtilities.GetNextCaretPosition(P_0, P_1 - 1, LogicalDirection.Forward, CaretPositioningMode.WordBorder) == P_1;
	}

	public ISearchResult FindNext(ITextSource document, int offset, int length)
	{
		return FindAll(document, offset, length).FirstOrDefault();
	}

	public bool Equals(ISearchStrategy other)
	{
		if (other is RegexSearchStrategy regexSearchStrategy && regexSearchStrategy.NYOAgi44Jk.ToString() == NYOAgi44Jk.ToString() && regexSearchStrategy.NYOAgi44Jk.Options == NYOAgi44Jk.Options)
		{
			return regexSearchStrategy.NYOAgi44Jk.RightToLeft == NYOAgi44Jk.RightToLeft;
		}
		return false;
	}

	public int ReplaceAll(IDocument document, SearchConfig config)
	{
		int num = 0;
		if (string.IsNullOrEmpty(document.Text))
		{
			return num;
		}
		int num2 = 0;
		string text = Transform(config.ReplaceKeyword);
		foreach (Match item in NYOAgi44Jk.Matches(document.Text))
		{
			num++;
			if (config.RegularExpression)
			{
				string text2 = NYOAgi44Jk.Replace(item.Value, Transform(config.ReplaceKeyword));
				document.Replace(num2 + item.Index, item.Length, text2);
				num2 += text2.Length - item.Length;
			}
			else
			{
				document.Replace(num2 + item.Index, item.Length, text);
				num2 += text.Length - item.Length;
			}
		}
		return num;
	}

	public string ReplaceNext(IDocument document, int startOffset, int length, string findKeyWord, string replaceText, bool useRegularExpression)
	{
		if (useRegularExpression)
		{
			string text = document.GetText(startOffset, length);
			string text2 = NYOAgi44Jk.Replace(text, Transform(replaceText));
			document.Replace(startOffset, length, text2);
			return text2;
		}
		replaceText = Transform(replaceText);
		document.Replace(startOffset, length, replaceText);
		return replaceText;
	}

	public static string Transform(string data)
	{
		return data.Replace("\\r\\n", Environment.NewLine).Replace("\\r", '\r'.ToString()).Replace("\\n", '\n'.ToString())
			.Replace("\\t", '\t'.ToString())
			.Replace("\\0", '\0'.ToString());
	}
}
