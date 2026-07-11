using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utools;

public static class ListHelper
{
	public static void SwapItem<T>(this IList<T> items, T target, T source)
	{
		int index = items.IndexOf(target);
		int index2 = items.IndexOf(source);
		T value = items[index];
		items[index] = items[index2];
		items[index2] = value;
	}

	public static void SwapItem<T>(this IList<T> source, T target, IList<T> list)
	{
		int num = source.IndexOf(target);
		for (int i = 0; i < list.Count; i++)
		{
			source.Insert(num + i, list[i]);
		}
		for (int j = 0; j < list.Count; j++)
		{
			source.Remove(list[j]);
		}
	}

	public static List<T> RandomSortList<T>(this List<T> list)
	{
		Random random = new Random();
		List<T> list2 = new List<T>();
		foreach (T item in list)
		{
			list2.Insert(random.Next(list2.Count), item);
		}
		return list2;
	}

	public static string GetParamSrc(Dictionary<string, string> paramsMap)
	{
		IOrderedEnumerable<KeyValuePair<string, string>> orderedEnumerable = paramsMap.OrderBy(delegate(KeyValuePair<string, string> objDic)
		{
			KeyValuePair<string, string> keyValuePair = objDic;
			return keyValuePair.Key;
		});
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> item in orderedEnumerable)
		{
			string key = item.Key;
			string value = item.Value;
			stringBuilder.Append(key + "=" + value + "&");
		}
		return stringBuilder.ToString().Substring(0, stringBuilder.ToString().Length - 1);
	}

	public static List<Tresult> MemberToList<TSource, Tresult>(this List<TSource> list, Func<TSource, Tresult> selector)
	{
		return list.Select(selector).ToList();
	}

	public static string ListToString<Tsoure>(this IEnumerable<Tsoure> list, string separator)
	{
		return string.Join(separator, list);
	}

	public static HashSet<T> ToHasSetNew<T>(this List<T> list)
	{
		return list?.ToHashSet();
	}

	public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
	{
		return (from x in source.Select((T x, int i) => new
			{
				Index = i,
				Value = x
			})
			group x by x.Index / chunkSize into x
			select x.Select(v => v.Value).ToList()).ToList();
	}
}
