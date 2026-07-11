using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Swordfish.NET.Collections.Auxiliary;

namespace Swordfish.NET.Collections;

public class ImmutableDictionaryListPair<TKey, TValue>
{
	public class ItemAndIndex
	{
		public KeyValuePair<TKey, TValue> Item { get; }

		public int Index { get; }

		public ItemAndIndex(KeyValuePair<TKey, TValue> item, int index)
		{
			Item = item;
			Index = index;
		}
	}

	private BinarySorter<BigRationalOld> _indexFinder = new BinarySorter<BigRationalOld>();

	public int Count => List.Count;

	public ItemAndIndex ItemAndIndexCount => new ItemAndIndex(default(KeyValuePair<TKey, TValue>), List.Count);

	internal ImmutableDictionary<TKey, ObservableDictionaryNode<TKey, TValue>> Dictionary { get; }

	internal ImmutableList<ObservableDictionaryNode<TKey, TValue>> List { get; }

	public static ImmutableDictionaryListPair<TKey, TValue> Empty { get; } = new ImmutableDictionaryListPair<TKey, TValue>(ImmutableDictionary<TKey, ObservableDictionaryNode<TKey, TValue>>.Empty, ImmutableList<ObservableDictionaryNode<TKey, TValue>>.Empty);

	internal ImmutableDictionaryListPair(ImmutableDictionary<TKey, ObservableDictionaryNode<TKey, TValue>> dictionary, ImmutableList<ObservableDictionaryNode<TKey, TValue>> list)
	{
		Dictionary = dictionary;
		List = list;
	}

	public ImmutableDictionaryListPair<TKey, TValue> WithComparers(IEqualityComparer<TKey> keyComparer)
	{
		return new ImmutableDictionaryListPair<TKey, TValue>(Dictionary.WithComparers(keyComparer), List);
	}

	public ImmutableDictionaryListPair<TKey, TValue> Add(KeyValuePair<TKey, TValue> pair)
	{
		ObservableDictionaryNode<TKey, TValue> before = (List.Any() ? List[List.Count - 1] : null);
		ObservableDictionaryNode<TKey, TValue> value = new ObservableDictionaryNode<TKey, TValue>(pair, before);
		return new ImmutableDictionaryListPair<TKey, TValue>(Dictionary.Add(pair.Key, value), List.Add(value));
	}

	public ImmutableDictionaryListPair<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
	{
		ObservableDictionaryNode<TKey, TValue> endNode = (List.Any() ? List[List.Count - 1] : null);
		IEnumerable<ObservableDictionaryNode<TKey, TValue>> enumerable = pairs.SelectWithPreviousResult((KeyValuePair<TKey, TValue> firstItem) => new ObservableDictionaryNode<TKey, TValue>(firstItem, endNode), (ObservableDictionaryNode<TKey, TValue> previousNode, KeyValuePair<TKey, TValue> pair) => new ObservableDictionaryNode<TKey, TValue>(pair, previousNode));
		IEnumerable<KeyValuePair<TKey, ObservableDictionaryNode<TKey, TValue>>> pairs2 = enumerable.Select((ObservableDictionaryNode<TKey, TValue> node) => KeyValuePair.Create(node.Key, node));
		return new ImmutableDictionaryListPair<TKey, TValue>(Dictionary.AddRange(pairs2), List.AddRange(enumerable));
	}

	public ImmutableDictionaryListPair<TKey, TValue> Insert(int index, KeyValuePair<TKey, TValue> pair)
	{
		if (index == Count)
		{
			return Add(pair);
		}
		if (index > Count || index < 0)
		{
			throw new ArgumentOutOfRangeException("index", index, $"Needs to be greated than -1 and less than the count of {Count}");
		}
		BigRationalOld position = ((index > 0) ? ((List[index - 1].SortKey + List[index].SortKey) / 2) : (List[index].SortKey - BigRationalOld.One));
		ObservableDictionaryNode<TKey, TValue> observableDictionaryNode = new ObservableDictionaryNode<TKey, TValue>(pair, position);
		ImmutableList<ObservableDictionaryNode<TKey, TValue>> list = List.Insert(index, observableDictionaryNode);
		return new ImmutableDictionaryListPair<TKey, TValue>(Dictionary.Add(observableDictionaryNode.Key, observableDictionaryNode), list);
	}

	public IList<KeyValuePair<TKey, TValue>> GetRange(int index, int count)
	{
		return ListSelect.Create(List.GetRange(index, count), (ObservableDictionaryNode<TKey, TValue> s) => s.KeyValuePair);
	}

	public IList<KeyValuePair<TKey, TValue>> GetRange(IList<TKey> keys)
	{
		return (from key in keys
			where Dictionary.ContainsKey(key)
			select Dictionary[key] into node
			select node.KeyValuePair).ToList();
	}

	public ImmutableDictionaryListPair<TKey, TValue> Remove(TKey key)
	{
		if (Dictionary.TryGetValue(key, out var value))
		{
			int index = GetIndex(value);
			return new ImmutableDictionaryListPair<TKey, TValue>(Dictionary.Remove(key), List.RemoveAt(index));
		}
		return this;
	}

	public ImmutableDictionaryListPair<TKey, TValue> RemoveAt(int index)
	{
		ObservableDictionaryNode<TKey, TValue> observableDictionaryNode = List[index];
		return new ImmutableDictionaryListPair<TKey, TValue>(Dictionary.Remove(observableDictionaryNode.Key), List.RemoveAt(index));
	}

	public ImmutableDictionaryListPair<TKey, TValue> RemoveRange(int index, int count)
	{
		IEnumerable<TKey> keys = from x in List.GetRange(index, count)
			select x.Key;
		return new ImmutableDictionaryListPair<TKey, TValue>(Dictionary.RemoveRange(keys), List.RemoveRange(index, count));
	}

	public ImmutableDictionaryListPair<TKey, TValue> RemoveRange(IList<TKey> keys)
	{
		IEnumerable<ObservableDictionaryNode<TKey, TValue>> enumerable = from key in keys
			where Dictionary.ContainsKey(key)
			select Dictionary[key];
		if (enumerable.Any())
		{
			return new ImmutableDictionaryListPair<TKey, TValue>(Dictionary.RemoveRange(keys), List.RemoveRange(enumerable));
		}
		return this;
	}

	public KeyValuePair<TKey, TValue> GetItem(int index)
	{
		return List[index].KeyValuePair;
	}

	public ItemAndIndex GetItemAndIndex(TKey key)
	{
		if (Dictionary.ContainsKey(key))
		{
			ObservableDictionaryNode<TKey, TValue> observableDictionaryNode = Dictionary[key];
			int index = GetIndex(observableDictionaryNode);
			return new ItemAndIndex(observableDictionaryNode.KeyValuePair, index);
		}
		return null;
	}

	private int GetIndex(ObservableDictionaryNode<TKey, TValue> node)
	{
		int matchIndex = _indexFinder.GetMatchIndex(List.Count, node.SortKey, (int index) => List[index].SortKey);
		if (matchIndex < 0 && !node.Equals(List[matchIndex]))
		{
			throw new InvalidOperationException("Theres a bug in the code that finds the dictionary index");
		}
		return matchIndex;
	}

	public ImmutableDictionaryListPair<TKey, TValue> ReplaceItem(int index, KeyValuePair<TKey, TValue> pair)
	{
		ObservableDictionaryNode<TKey, TValue> observableDictionaryNode = new ObservableDictionaryNode<TKey, TValue>(pair, List[index].SortKey);
		return new ImmutableDictionaryListPair<TKey, TValue>(Dictionary.SetItem(observableDictionaryNode.Key, observableDictionaryNode), List.SetItem(index, observableDictionaryNode));
	}
}
