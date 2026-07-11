using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;

namespace Swordfish.NET.Collections;

public static class ConcurrentObservableExtensions
{
	private enum MapUnmap
	{
		Map,
		Unmap
	}

	public static void MapChildListToAggregate<T, K>(this ConcurrentObservableCollection<T> source, ConcurrentObservableCollection<K> dest, Func<T, ConcurrentObservableCollection<K>> selectChildList)
	{
		NotifyCollectionChangedEventHandler value = delegate(object? s, NotifyCollectionChangedEventArgs e)
		{
			foreach (T item in e?.OldItems?.OfType<T>() ?? Enumerable.Empty<T>())
			{
				selectChildList(item).UnmapChildToAggregate(dest);
			}
			foreach (T item2 in e?.NewItems?.OfType<T>() ?? Enumerable.Empty<T>())
			{
				selectChildList(item2).MapChildToAggregate(dest);
			}
		};
		using (source.FreezeUpdates())
		{
			source.CollectionChanged += value;
			foreach (T item3 in source)
			{
				selectChildList(item3).MapChildToAggregate(dest);
			}
		}
	}

	public static void MapChildToAggregate<T>(this ConcurrentObservableCollection<T> source, ConcurrentObservableCollection<T> dest)
	{
		source.ChildToAggregateMapUnmap(dest, MapUnmap.Map);
	}

	public static void UnmapChildToAggregate<T>(this ConcurrentObservableCollection<T> source, ConcurrentObservableCollection<T> dest)
	{
		source.ChildToAggregateMapUnmap(dest, MapUnmap.Unmap);
	}

	private static void ChildToAggregateMapUnmap<T>(this ConcurrentObservableCollection<T> source, ConcurrentObservableCollection<T> dest, MapUnmap mapUnmap)
	{
		NotifyCollectionChangedEventHandler value = delegate(object? s, NotifyCollectionChangedEventArgs e)
		{
			foreach (T item in e?.OldItems?.OfType<T>() ?? Enumerable.Empty<T>())
			{
				dest.Remove(item);
			}
			foreach (T item2 in e?.NewItems?.OfType<T>() ?? Enumerable.Empty<T>())
			{
				dest.Add(item2);
			}
		};
		using (source.FreezeUpdates())
		{
			switch (mapUnmap)
			{
			case MapUnmap.Map:
				dest.AddRange(source);
				source.CollectionChanged += value;
				break;
			case MapUnmap.Unmap:
				dest.RemoveRange(source);
				source.CollectionChanged -= value;
				break;
			}
		}
	}

	internal static ImmutableHashSet<T> AddRange<T>(this ImmutableHashSet<T> dest, IEnumerable<T> source)
	{
		ImmutableHashSet<T> immutableHashSet = dest;
		foreach (T item in source)
		{
			immutableHashSet = immutableHashSet.Add(item);
		}
		return immutableHashSet;
	}

	internal static ImmutableHashSet<T> RemoveRange<T>(this ImmutableHashSet<T> target, IEnumerable<T> source)
	{
		ImmutableHashSet<T> immutableHashSet = target;
		foreach (T item in source)
		{
			immutableHashSet = immutableHashSet.Remove(item);
		}
		return immutableHashSet;
	}
}
