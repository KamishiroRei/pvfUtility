using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace Swordfish.NET.Collections;

[Serializable]
public class ConcurrentObservableSortedDictionary<TKey, TValue> : ConcurrentObservableDictionary<TKey, TValue>, ISerializable
{
	private BinarySorter<TKey> _sorter;

	public override TValue this[TKey key]
	{
		get
		{
			return base[key];
		}
		set
		{
			KeyValuePair<TKey, TValue> pair = KeyValuePair.Create(key, value);
			DoTestReadWriteNotify(() => !_internalCollection.Dictionary.ContainsKey(key), delegate
			{
				int insertIndex = _sorter.GetInsertIndex(_internalCollection.Count, pair.Key, (int i) => _internalCollection.List[i].Key);
				return new ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex(pair, insertIndex);
			}, (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => _internalCollection.Insert(itemAndIndex.Index, pair), (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair, itemAndIndex.Index), () => _internalCollection.GetItemAndIndex(key), (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => _internalCollection.ReplaceItem(itemAndIndex.Index, pair), (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, pair, itemAndIndex.Item, itemAndIndex.Index));
		}
	}

	public ConcurrentObservableSortedDictionary()
		: this(true, (IComparer<TKey>)null)
	{
	}

	public ConcurrentObservableSortedDictionary(bool isMultithreaded)
		: this(isMultithreaded, (IComparer<TKey>)null)
	{
	}

	public ConcurrentObservableSortedDictionary(IComparer<TKey> comparer)
		: this(true, comparer)
	{
	}

	public ConcurrentObservableSortedDictionary(bool isMultithreaded, IComparer<TKey> comparer)
		: base(isMultithreaded, (IEqualityComparer<TKey>)null)
	{
		_sorter = new BinarySorter<TKey>(comparer);
	}

	public override void Add(KeyValuePair<TKey, TValue> pair)
	{
		DoReadWriteNotify(() => _sorter.GetInsertIndex(_internalCollection.Count, pair.Key, (int i) => _internalCollection.List[i].Key), (int index) => _internalCollection.Insert(index, pair), (int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair, index));
	}

	public override void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
	{
		IList<KeyValuePair<TKey, TValue>> pairsList = pairs as IList<KeyValuePair<TKey, TValue>>;
		if (pairsList == null)
		{
			pairsList = pairs.ToList();
		}
		Func<int, ImmutableDictionaryListPair<TKey, TValue>> write = delegate
		{
			ImmutableDictionaryListPair<TKey, TValue> updatedCollection = _internalCollection;
			foreach (KeyValuePair<TKey, TValue> item in pairsList)
			{
				int insertIndex = _sorter.GetInsertIndex(updatedCollection.Count, item.Key, (int i) => updatedCollection.List[i].Key);
				updatedCollection = updatedCollection.Insert(insertIndex, item);
			}
			return updatedCollection;
		};
		DoReadWriteNotify(() => 0, write, (int nothing) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)pairsList));
	}

	public override void Insert(int index, KeyValuePair<TKey, TValue> pair)
	{
		Add(pair);
	}

	public override TValue RetrieveOrAdd(TKey key, Func<TKey, TValue> getValue)
	{
		ObservableDictionaryNode<TKey, TValue> internalNode = null;
		KeyValuePair<TKey, TValue>? newPair = null;
		if (DoTestReadWriteNotify(() => !_internalCollection.Dictionary.TryGetValue(key, out internalNode), () => _sorter.GetInsertIndex(_internalCollection.Count, key, (int i) => _internalCollection.List[i].Key), delegate(int index)
		{
			ImmutableDictionaryListPair<TKey, TValue> internalCollection = _internalCollection;
			KeyValuePair<TKey, TValue>? keyValuePair = (newPair = KeyValuePair.Create(key, getValue(key)));
			return internalCollection.Insert(index, keyValuePair.Value);
		}, (int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newPair, index)))
		{
			return newPair.Value.Value;
		}
		return internalNode.Value;
	}

	protected override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}

	protected ConcurrentObservableSortedDictionary(SerializationInfo information, StreamingContext context)
		: base(information, context)
	{
	}
}
