using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Utools;

public static class IEnumerableExtensions
{

	public static IEnumerable<TFirst> IntersectBy<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> condition)
	{
		return first.Where(f => second.Any(s => condition(f, s)));
	}

	public static IEnumerable<TFirst> ExceptBy<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> condition)
	{
		return first.Where(f => !second.Any(s => condition(f, s)));
	}

	public static void AddRange<T>(this ICollection<T> @this, params T[] values)
	{
		foreach (T item in values)
		{
			@this.Add(item);
		}
	}

	public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> values)
	{
		foreach (T value in values)
		{
			@this.Add(value);
		}
	}

	public static void AddRange<T>(this ConcurrentBag<T> @this, params T[] values)
	{
		foreach (T item in values)
		{
			@this.Add(item);
		}
	}

	public static void AddRange<T>(this ConcurrentQueue<T> @this, params T[] values)
	{
		foreach (T item in values)
		{
			@this.Enqueue(item);
		}
	}

	public static void AddRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
	{
		foreach (T val in values)
		{
			if (predicate(val))
			{
				@this.Add(val);
			}
		}
	}

	public static void AddRangeIf<T>(this ConcurrentBag<T> @this, Func<T, bool> predicate, params T[] values)
	{
		foreach (T val in values)
		{
			if (predicate(val))
			{
				@this.Add(val);
			}
		}
	}

	public static void AddRangeIf<T>(this ConcurrentQueue<T> @this, Func<T, bool> predicate, params T[] values)
	{
		foreach (T val in values)
		{
			if (predicate(val))
			{
				@this.Enqueue(val);
			}
		}
	}

	public static void AddRangeIfNotContains<T>(this ICollection<T> @this, params T[] values)
	{
		foreach (T item in values)
		{
			if (!@this.Contains(item))
			{
				@this.Add(item);
			}
		}
	}

	public static void RemoveWhere<T>(this ICollection<T> @this, Func<T, bool> where)
	{
		foreach (T item in @this.Where(where).ToList())
		{
			@this.Remove(item);
		}
	}

	public static void InsertAfter<T>(this IList<T> list, Func<T, bool> condition, T value)
	{
		foreach (var item in from p in list.Select((T item, int index) => new { item, index })
			where condition(p.item)
			orderby p.index descending
			select p)
		{
			if (item.index + 1 == list.Count)
			{
				list.Add(value);
			}
			else
			{
				list.Insert(item.index + 1, value);
			}
		}
	}

	public static void InsertAfter<T>(this IList<T> list, int index, T value)
	{
		foreach (var item in from p in list.Select((T v, int i) => new
			{
				Value = v,
				Index = i
			})
			where p.Index == index
			orderby p.Index descending
			select p)
		{
			if (item.Index + 1 == list.Count)
			{
				list.Add(value);
			}
			else
			{
				list.Insert(item.Index + 1, value);
			}
		}
	}

	public static HashSet<TResult> ToHashSet<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
	{
		HashSet<TResult> hashSet = new HashSet<TResult>();
		hashSet.UnionWith(source.Select(selector));
		return hashSet;
	}

	public static void ForEach<T>(this IEnumerable<T> objs, Action<T> action)
	{
		foreach (T obj in objs)
		{
			action(obj);
		}
	}

	public static async Task ForeachAsync<T>(this IEnumerable<T> source, Func<T, Task> action, int maxParallelCount, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (Debugger.IsAttached)
		{
			foreach (T item in source)
			{
				await action(item);
			}
			return;
		}
		List<Task> list = new List<Task>();
		foreach (T item2 in source)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			list.Add(action(item2));
			if (list.Count >= maxParallelCount)
			{
				await Task.WhenAll(list);
				list.Clear();
			}
		}
		await Task.WhenAll(list);
	}

	public static Task ForeachAsync<T>(this IEnumerable<T> source, Func<T, Task> action, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.ForeachAsync(action, source.Count(), cancellationToken);
	}

	public static Task<TResult[]> SelectAsync<T, TResult>(this IEnumerable<T> source, Func<T, Task<TResult>> selector)
	{
		return Task.WhenAll(source.Select(selector));
	}

	public static Task<TResult[]> SelectAsync<T, TResult>(this IEnumerable<T> source, Func<T, int, Task<TResult>> selector)
	{
		return Task.WhenAll(source.Select(selector));
	}

	public static async Task<List<TResult>> SelectAsync<T, TResult>(this IEnumerable<T> source, Func<T, Task<TResult>> selector, int maxParallelCount)
	{
		List<TResult> results = new List<TResult>();
		List<Task<TResult>> tasks = new List<Task<TResult>>();
		List<TResult> list;
		foreach (T item2 in source)
		{
			Task<TResult> item = selector(item2);
			tasks.Add(item);
			if (tasks.Count >= maxParallelCount)
			{
				list = results;
				list.AddRange(await Task.WhenAll(tasks));
				tasks.Clear();
			}
		}
		list = results;
		list.AddRange(await Task.WhenAll(tasks));
		return results;
	}

	public static async Task<List<TResult>> SelectAsync<T, TResult>(this IEnumerable<T> source, Func<T, int, Task<TResult>> selector, int maxParallelCount)
	{
		List<TResult> results = new List<TResult>();
		List<Task<TResult>> tasks = new List<Task<TResult>>();
		int index = 0;
		List<TResult> list;
		foreach (T item2 in source)
		{
			Task<TResult> item = selector(item2, index++);
			tasks.Add(item);
			if (tasks.Count >= maxParallelCount)
			{
				list = results;
				list.AddRange(await Task.WhenAll(tasks));
				tasks.Clear();
			}
		}
		list = results;
		list.AddRange(await Task.WhenAll(tasks));
		return results;
	}

	public static async Task ForAsync<T>(this IEnumerable<T> source, Func<T, int, Task> selector, int maxParallelCount, CancellationToken cancellationToken = default(CancellationToken))
	{
		int index = 0;
		if (Debugger.IsAttached)
		{
			foreach (T item in source)
			{
				await selector(item, index);
				index++;
			}
			return;
		}
		List<Task> list = new List<Task>();
		foreach (T item2 in source)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			list.Add(selector(item2, index));
			Interlocked.Add(ref index, 1);
			if (list.Count >= maxParallelCount)
			{
				await Task.WhenAll(list);
				list.Clear();
			}
		}
		await Task.WhenAll(list);
	}

	public static Task ForAsync<T>(this IEnumerable<T> source, Func<T, int, Task> selector, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.ForAsync(selector, source.Count(), cancellationToken);
	}

	public static TResult MaxOrDefault<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
	{
		return source.Select(selector).DefaultIfEmpty().Max();
	}

	public static TResult MaxOrDefault<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, TResult defaultValue)
	{
		return source.Select(selector).DefaultIfEmpty(defaultValue).Max();
	}

	public static TSource MaxOrDefault<TSource>(this IQueryable<TSource> source)
	{
		return source.DefaultIfEmpty().Max();
	}

	public static TSource MaxOrDefault<TSource>(this IQueryable<TSource> source, TSource defaultValue)
	{
		return source.DefaultIfEmpty(defaultValue).Max();
	}

	public static TResult MaxOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, TResult defaultValue)
	{
		return source.Select(selector).DefaultIfEmpty(defaultValue).Max();
	}

	public static TResult MaxOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		return source.Select(selector).DefaultIfEmpty().Max();
	}

	public static TSource MaxOrDefault<TSource>(this IEnumerable<TSource> source)
	{
		return source.DefaultIfEmpty().Max();
	}

	public static TSource MaxOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
	{
		return source.DefaultIfEmpty(defaultValue).Max();
	}

	public static TResult MinOrDefault<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
	{
		return source.Select(selector).DefaultIfEmpty().Min();
	}

	public static TResult MinOrDefault<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, TResult defaultValue)
	{
		return source.Select(selector).DefaultIfEmpty(defaultValue).Min();
	}

	public static TSource MinOrDefault<TSource>(this IQueryable<TSource> source)
	{
		return source.DefaultIfEmpty().Min();
	}

	public static TSource MinOrDefault<TSource>(this IQueryable<TSource> source, TSource defaultValue)
	{
		return source.DefaultIfEmpty(defaultValue).Min();
	}

	public static TResult MinOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		return source.Select(selector).DefaultIfEmpty().Min();
	}

	public static TResult MinOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, TResult defaultValue)
	{
		return source.Select(selector).DefaultIfEmpty(defaultValue).Min();
	}

	public static TSource MinOrDefault<TSource>(this IEnumerable<TSource> source)
	{
		return source.DefaultIfEmpty().Min();
	}

	public static TSource MinOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
	{
		return source.DefaultIfEmpty(defaultValue).Min();
	}

	public static TResult StandardDeviation<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector) where TResult : IConvertible
	{
		return source.Select(t => selector(t).ConvertTo<double>()).StandardDeviation().ConvertTo<TResult>();
	}

	public static T StandardDeviation<T>(this IEnumerable<T> source) where T : IConvertible
	{
		return source.Select((T t) => t.ConvertTo<double>()).StandardDeviation().ConvertTo<T>();
	}

	public static double StandardDeviation(this IEnumerable<double> source)
	{
		double result = 0.0;
		int num = source.Count();
		if (num > 1)
		{
			double average = source.Average();
			result = Math.Sqrt(source.Sum(d => (d - average) * (d - average)) / num);
		}
		return result;
	}

	public static IOrderedEnumerable<T> OrderByRandom<T>(this IEnumerable<T> source)
	{
		return source.OrderBy((T _) => Guid.NewGuid());
	}

	public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> condition)
	{
		if (first is ICollection<T> collection && second is ICollection<T> collection2)
		{
			if (collection.Count != collection2.Count)
			{
				return false;
			}
			if (collection is IList<T> list && collection2 is IList<T> list2)
			{
				int count = collection.Count;
				for (int i = 0; i < count; i++)
				{
					if (!condition(list[i], list2[i]))
					{
						return false;
					}
				}
				return true;
			}
		}
		using IEnumerator<T> enumerator = first.GetEnumerator();
		using IEnumerator<T> enumerator2 = second.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!enumerator2.MoveNext() || !condition(enumerator.Current, enumerator2.Current))
			{
				return false;
			}
		}
		return !enumerator2.MoveNext();
	}

	public static bool SequenceEqual<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, bool> condition)
	{
		if (first is ICollection<T1> collection && second is ICollection<T2> collection2)
		{
			if (collection.Count != collection2.Count)
			{
				return false;
			}
			if (collection is IList<T1> list && collection2 is IList<T2> list2)
			{
				int count = collection.Count;
				for (int i = 0; i < count; i++)
				{
					if (!condition(list[i], list2[i]))
					{
						return false;
					}
				}
				return true;
			}
		}
		using IEnumerator<T1> enumerator = first.GetEnumerator();
		using IEnumerator<T2> enumerator2 = second.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!enumerator2.MoveNext() || !condition(enumerator.Current, enumerator2.Current))
			{
				return false;
			}
		}
		return !enumerator2.MoveNext();
	}

	public static (List<T1> adds, List<T2> remove, List<T1> updates) CompareChanges<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, bool> condition)
	{
		List<T1> item = first.ExceptBy(second, condition).ToList();
		List<T2> item2 = second.ExceptBy(first, (s, f) => condition(f, s)).ToList();
		List<T1> item3 = first.IntersectBy(second, condition).ToList();
		return (adds: item, remove: item2, updates: item3);
	}

	public static (List<T1> adds, List<T2> remove, List<(T1 first, T2 second)> updates) CompareChangesPlus<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, bool> condition)
	{
		List<T1> item = first.ExceptBy(second, condition).ToList();
		List<T2> item2 = second.ExceptBy(first, (s, f) => condition(f, s)).ToList();
		List<(T1, T2)> item3 = first.IntersectBy(second, condition)
			.Select(t1 => (t1, second.FirstOrDefault(t2 => condition(t1, t2))))
			.ToList();
		return (adds: item, remove: item2, updates: item3);
	}

	public static List<T> AsNotNull<T>(this List<T> list)
	{
		return list ?? new List<T>();
	}

	public static IEnumerable<T> AsNotNull<T>(this IEnumerable<T> list)
	{
		return list ?? new List<T>();
	}
}
