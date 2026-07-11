using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Swordfish.NET.Collections.Auxiliary;

public static class Extensions
{
	public delegate void ForEachAction<TSource>(TSource a);

	private static Lazy<Regex> _numberFinder = new Lazy<Regex>(() => new Regex("(?<!\\.\\d*)\\d+", RegexOptions.Compiled));

	public static void Initialize<T>(this T[] list, T value)
	{
		for (int i = 0; i < list.Length; i++)
		{
			list[i] = value;
		}
	}

	public static void Initialize<T>(this IList<T> list, T value)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = value;
		}
	}

	public static bool ContainsKeysAll<T1, T2>(this Dictionary<T1, T2> dictionary, params T1[] keys)
	{
		foreach (T1 key in keys)
		{
			if (!dictionary.ContainsKey(key))
			{
				return false;
			}
		}
		return true;
	}

	public static bool ContainsKeysAny<T1, T2>(this Dictionary<T1, T2> dictionary, params T1[] keys)
	{
		foreach (T1 key in keys)
		{
			if (dictionary.ContainsKey(key))
			{
				return true;
			}
		}
		return false;
	}

	public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
	{
		if (batchSize == 0)
		{
			yield break;
		}
		List<T> list = new List<T>();
		int num = 0;
		foreach (T item in source)
		{
			if (num >= batchSize)
			{
				yield return list;
				num = 0;
				list = new List<T>();
			}
			list.Add(item);
			num++;
		}
		yield return list;
	}

	public static IEnumerable<IEnumerable<T>> BatchWithOverlap<T>(this IEnumerable<T> source, int batchSize, int overlapSize)
	{
		if (batchSize <= 0)
		{
			yield break;
		}
		if (overlapSize >= batchSize)
		{
			throw new ArgumentException("Overlap needs to be less than batch size", "overlapSize");
		}
		List<T> batch = new List<T>();
		foreach (T item in source)
		{
			if (batch.Count >= batchSize)
			{
				yield return batch;
				batch = batch.Skip(batchSize - overlapSize).ToList();
			}
			batch.Add(item);
		}
		yield return batch;
	}

	public static bool AllUnique<T>(this IList<T> source) where T : IEquatable<T>
	{
		Utils.RequireNotNull(source, "source");
		EqualityComparer<T> comparer = EqualityComparer<T>.Default;
		return source.TrueForAllItemsToEachOtherItem((T a, T b) => !comparer.Equals(a, b));
	}

	public static bool TrueForAllItemsToEachOtherItem<T>(this IList<T> source, Func<T, T, bool> compare)
	{
		Utils.RequireNotNull(source, "source");
		Utils.RequireNotNull(compare, "compare");
		for (int i = 0; i < source.Count; i++)
		{
			for (int j = i + 1; j < source.Count; j++)
			{
				if (!compare(source[i], source[j]))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool TrueForAllAdjacentPairs<T>(this IList<T> source, Func<T, T, bool> compare)
	{
		Utils.RequireNotNull(source, "source");
		Utils.RequireNotNull(compare, "compare");
		for (int i = 0; i < source.Count - 1; i++)
		{
			if (!compare(source[i], source[i + 1]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
	{
		Utils.RequireNotNull(source, "source");
		if (source is ICollection<TSource>)
		{
			return ((ICollection<TSource>)source).Count == 0;
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		return !enumerator.MoveNext();
	}

	public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		Utils.RequireNotNull(source, "source");
		Utils.RequireNotNull(predicate, "predicate");
		int num = 0;
		foreach (TSource item in source)
		{
			if (predicate(item))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static ReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(this IEnumerable<TSource> source)
	{
		Utils.RequireNotNull(source, "source");
		return new ReadOnlyCollection<TSource>(source.ToArray());
	}

	public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, ForEachAction<TSource> action)
	{
		Utils.RequireNotNull(source, "source");
		Utils.RequireNotNull(action, "action");
		foreach (TSource item in source)
		{
			action(item);
		}
		return source;
	}

	public static IEnumerable<TSource> ForEachNotNull<TSource>(this IEnumerable<TSource> source, ForEachAction<TSource> action)
	{
		Utils.RequireNotNull(source, "source");
		Utils.RequireNotNull(action, "action");
		foreach (TSource item in source)
		{
			if (item != null)
			{
				action(item);
			}
		}
		return source;
	}

	public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource> source)
	{
		return source ?? Enumerable.Empty<TSource>();
	}

	public static IEnumerable<TSource> SelectRecursive<TSource>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TSource>> recursiveSelector)
	{
		Utils.RequireNotNull(source, "source");
		Utils.RequireNotNull(recursiveSelector, "recursiveSelector");
		Stack<IEnumerator<TSource>> stack = new Stack<IEnumerator<TSource>>();
		stack.Push(source.GetEnumerator());
		try
		{
			while (stack.Count > 0)
			{
				if (stack.Peek().MoveNext())
				{
					TSource current = stack.Peek().Current;
					yield return current;
					stack.Push(recursiveSelector(current).GetEnumerator());
				}
				else
				{
					stack.Pop().Dispose();
				}
			}
		}
		finally
		{
			while (stack.Count > 0)
			{
				stack.Pop().Dispose();
			}
		}
	}

	public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f)
	{
		return e.SelectMany((T c) => f(c).Flatten(f)).Concat(e);
	}

	public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T next)
	{
		return enumerable.Concat(ToIEnumerable(next));
	}

	public static IEnumerable<T> ToIEnumerable<T>(T next)
	{
		yield return next;
	}

	public static IEnumerable<T> EndWith<T>(this IEnumerable<T> enumerable, Action action)
	{
		if (enumerable == null)
		{
			throw new ArgumentNullException("enumerable");
		}
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		foreach (T item in enumerable)
		{
			yield return item;
		}
		action();
	}

	public static IEnumerable<T> BeginWith<T>(this IEnumerable<T> enumerable, Action action)
	{
		if (enumerable == null)
		{
			throw new ArgumentNullException("enumerable");
		}
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		action();
		foreach (T item in enumerable)
		{
			yield return item;
		}
	}

	public static void InitializeArray<T>(this Array array, Func<T> factory)
	{
		int[] recursiveIndicies = new int[0];
		Func<int[], T> objectFromIndicies = (int[] indices) => factory();
		InitializeArray(array, objectFromIndicies, recursiveIndicies);
	}

	public static void InitializeArray<T>(this Array array, Func<int[], T> objectFromIndicies)
	{
		int[] recursiveIndicies = new int[0];
		InitializeArray(array, objectFromIndicies, recursiveIndicies);
	}

	private static void InitializeArray<T>(Array array, Func<int[], T> objectFromIndicies, int[] recursiveIndicies)
	{
		if (recursiveIndicies.Length < array.Rank)
		{
			int lowerBound = array.GetLowerBound(recursiveIndicies.Length);
			int count = array.GetUpperBound(recursiveIndicies.Length) - lowerBound + 1;
			Enumerable.Range(lowerBound, count).ForEach(delegate(int x)
			{
				InitializeArray(array, objectFromIndicies, recursiveIndicies.Concat(x).ToArray());
			});
		}
		else
		{
			array.SetValue(objectFromIndicies(recursiveIndicies), recursiveIndicies);
		}
	}

	public static TValue GetValueOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : class
	{
		if (!dictionary.TryGetValue(key, out var value))
		{
			return null;
		}
		return value;
	}

	public static TValue GetValueOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
	{
		return dictionary.GetValueOrCreate(key, () => new TValue());
	}

	public static TValue GetValueOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
	{
		if (!dictionary.TryGetValue(key, out var value))
		{
			return valueFactory();
		}
		return value;
	}

	public static IOrderedEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector, int maxDigits = 7)
	{
		return source.OrderBy<T, string>((T i) => AlphaNumericReplacer(selector(i), maxDigits), StringComparer.Ordinal);
	}

	public static OrderedParallelQuery<T> OrderByAlphaNumeric<T>(this ParallelQuery<T> source, Func<T, string> selector, int maxDigits = 7)
	{
		return source.OrderBy<T, string>((T i) => AlphaNumericReplacer(selector(i), maxDigits), StringComparer.Ordinal);
	}

	private static string AlphaNumericReplacer(string input, int maxDigits)
	{
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Match item in _numberFinder.Value.Matches(input))
		{
			if (item.Index > num)
			{
				stringBuilder.Append(input.Substring(num, item.Index - num));
			}
			num = item.Index + item.Length;
			string text = item.Value.PadLeft(maxDigits, '0');
			if (item.Index > 0 && input[item.Index - 1] == '-')
			{
				char[] array = text.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (char)(57 - array[i] + 48);
				}
				stringBuilder.Append(array);
				int num2 = 0;
				if (num < input.Length && input[num] == '.')
				{
					stringBuilder.Append('.');
					num++;
					while (num < input.Length && input[num] >= '0' && input[num] <= '9')
					{
						stringBuilder.Append((char)(57 - input[num] + 48));
						num++;
						num2++;
					}
					if (num2 < maxDigits)
					{
						stringBuilder.Append('9', maxDigits - num2);
					}
				}
			}
			else
			{
				stringBuilder.Append(text);
			}
		}
		if (num < input.Length)
		{
			stringBuilder.Append(input.Substring(num));
		}
		return stringBuilder.ToString();
	}

	public static IEnumerable Ensure(this IEnumerable source)
	{
		return source ?? Enumerable.Empty<object>();
	}

	public static IEnumerable<T> Ensure<T>(this IEnumerable<T> source)
	{
		return source ?? Enumerable.Empty<T>();
	}

	public static void AddRange<T>(this IList<T> collection, IEnumerable<T> source)
	{
		if (collection is List<T> list)
		{
			list.AddRange(source);
			return;
		}
		source.ForEach(delegate(T x)
		{
			collection.Add(x);
		});
	}

	public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> collection, IEnumerable<KeyValuePair<TKey, TValue>> source)
	{
		source.ForEach(delegate(KeyValuePair<TKey, TValue> x)
		{
			collection.Add(x.Key, x.Value);
		});
	}

	public static IEnumerable<T> GetLatestConsumingEnumerable<T>(this BlockingCollection<T> collection)
	{
		foreach (T item2 in collection.GetConsumingEnumerable())
		{
			T val = item2;
			T item;
			while (collection.TryTake(out item))
			{
				val = item;
			}
			yield return val;
		}
	}

	public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> projection)
	{
		using IEnumerator<TSource> iterator = source.GetEnumerator();
		if (iterator.MoveNext())
		{
			TSource current = iterator.Current;
			while (iterator.MoveNext())
			{
				yield return projection(current, iterator.Current);
				current = iterator.Current;
			}
			yield break;
		}
	}

	public static IEnumerable<TResult> SelectWithPreviousResult<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> firstItem, Func<TResult, TSource, TResult> projection)
	{
		using IEnumerator<TSource> iterator = source.GetEnumerator();
		if (iterator.MoveNext())
		{
			TSource current = iterator.Current;
			TResult previousResult = firstItem(current);
			yield return previousResult;
			while (iterator.MoveNext())
			{
				previousResult = projection(previousResult, iterator.Current);
				yield return previousResult;
			}
			yield break;
		}
	}
}
