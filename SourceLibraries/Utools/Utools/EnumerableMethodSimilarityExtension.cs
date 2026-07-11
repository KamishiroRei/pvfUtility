using System;
using System.Collections.Generic;
using F23.StringSimilarity;

namespace Utools;

public static class EnumerableMethodSimilarityExtension
{
	public static SimilarityResultInfo<string> Similarity(this IEnumerable<string> source, string targetText)
	{
		return source.Similarity((string c) => c, targetText);
	}

	public static SimilarityResultInfo<TSource> Similarity<TSource>(this IEnumerable<TSource> source, Func<TSource, string> textSelector, string targetText)
	{
		if (source == null)
		{
			return null;
		}
		if (textSelector == null)
		{
			return null;
		}
		Levenshtein levenshtein = new Levenshtein();
		double? num = null;
		List<TSource> list = null;
		foreach (TSource item in source)
		{
			string text = textSelector(item);
			if (!string.IsNullOrEmpty(text))
			{
				double num2 = levenshtein.Distance(text, targetText);
				if (!num.HasValue)
				{
					num = num2;
					list = new List<TSource> { item };
				}
				else if (num2 < num.Value)
				{
					num = num2;
					list.Clear();
					list.Add(item);
				}
				else if (num2 == num.Value)
				{
					list.Add(item);
				}
			}
		}
		if (!num.HasValue)
		{
			return null;
		}
		return new SimilarityResultInfo<TSource>
		{
			SimilarityValue = num.Value,
			SimilarityTargetList = list
		};
	}
}
