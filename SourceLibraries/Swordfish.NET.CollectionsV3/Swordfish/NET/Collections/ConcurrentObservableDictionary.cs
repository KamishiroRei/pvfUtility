using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Swordfish.NET.Collections;

[Serializable]
public class ConcurrentObservableDictionary<TKey, TValue> : ConcurrentObservableBase<KeyValuePair<TKey, TValue>, ImmutableDictionaryListPair<TKey, TValue>>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary<TKey, TValue>, ICollection, ISerializable
{
	public override IList<KeyValuePair<TKey, TValue>> CollectionView => ListSelect.Create(_internalCollection.List, (ObservableDictionaryNode<TKey, TValue> node) => node.KeyValuePair);

	public IList<TKey> Keys => ListSelect.Create(_internalCollection.List, (ObservableDictionaryNode<TKey, TValue> node) => node.Key);

	ICollection<TKey> IDictionary<TKey, TValue>.Keys => ListSelect.Create(_internalCollection.List, (ObservableDictionaryNode<TKey, TValue> node) => node.Key);

	public IList<TValue> Values => ListSelect.Create(_internalCollection.List, (ObservableDictionaryNode<TKey, TValue> node) => node.Value);

	ICollection<TValue> IDictionary<TKey, TValue>.Values => ListSelect.Create(_internalCollection.List, (ObservableDictionaryNode<TKey, TValue> node) => node.Value);

	public override int Count => _internalCollection.Count;

	public virtual TValue this[TKey key]
	{
		get
		{
			return _internalCollection.Dictionary[key].Value;
		}
		set
		{
			KeyValuePair<TKey, TValue> pair = KeyValuePair.Create(key, value);
			DoTestReadWriteNotify(() => !_internalCollection.Dictionary.ContainsKey(key), () => _internalCollection.ItemAndIndexCount, (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => _internalCollection.Add(pair), (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair, itemAndIndex.Index), () => _internalCollection.GetItemAndIndex(key), (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => _internalCollection.ReplaceItem(itemAndIndex.Index, pair), (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, pair, itemAndIndex.Item, itemAndIndex.Index));
		}
	}

	public bool IsReadOnly => false;

	object ICollection.SyncRoot
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	bool ICollection.IsSynchronized
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public ConcurrentObservableDictionary()
		: this(true, (IEqualityComparer<TKey>)null)
	{
	}

	public ConcurrentObservableDictionary(bool isMultithreaded, IEqualityComparer<TKey> keyComparer = null)
		: base(isMultithreaded, (keyComparer == null) ? ImmutableDictionaryListPair<TKey, TValue>.Empty : ImmutableDictionaryListPair<TKey, TValue>.Empty.WithComparers(keyComparer))
	{
		base.PropertyChanged += delegate(object? s, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CollectionView")
			{
				RaisePropertyChanged("Keys", "Values");
			}
		};
	}

	public void Add(TKey key, TValue value)
	{
		Add(KeyValuePair.Create(key, value));
	}

	public virtual void Add(KeyValuePair<TKey, TValue> pair)
	{
		DoReadWriteNotify(() => _internalCollection.Count, (int index) => _internalCollection.Add(pair), (int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair, index));
	}

	public virtual void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
	{
		IList<KeyValuePair<TKey, TValue>> pairsList = pairs as IList<KeyValuePair<TKey, TValue>>;
		if (pairsList == null)
		{
			pairsList = pairs.ToList();
		}
		DoReadWriteNotify(() => _internalCollection.Count, (int index) => _internalCollection.AddRange(pairsList), (int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)pairsList, index));
	}

	public virtual bool TryAdd(TKey key, Func<TKey, TValue> getValue)
	{
		KeyValuePair<TKey, TValue>? newPair = null;
		if (DoTestReadWriteNotify(() => !_internalCollection.Dictionary.ContainsKey(key), () => _internalCollection.Count, delegate
		{
			ImmutableDictionaryListPair<TKey, TValue> internalCollection = _internalCollection;
			KeyValuePair<TKey, TValue>? keyValuePair = (newPair = KeyValuePair.Create(key, getValue(key)));
			return internalCollection.Add(keyValuePair.Value);
		}, (int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newPair, index)))
		{
			return true;
		}
		return false;
	}

	public bool TryAdd(TKey key, Func<TValue> getValue)
	{
		return TryAdd(key, (TKey keyIn) => getValue());
	}

	public virtual TValue RetrieveOrAdd(TKey key, Func<TKey, TValue> getValue)
	{
		ObservableDictionaryNode<TKey, TValue> internalNode = null;
		KeyValuePair<TKey, TValue>? newPair = null;
		if (DoTestReadWriteNotify(() => !_internalCollection.Dictionary.TryGetValue(key, out internalNode), () => _internalCollection.Count, delegate
		{
			ImmutableDictionaryListPair<TKey, TValue> internalCollection = _internalCollection;
			KeyValuePair<TKey, TValue>? keyValuePair = (newPair = KeyValuePair.Create(key, getValue(key)));
			return internalCollection.Add(keyValuePair.Value);
		}, (int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newPair, index)))
		{
			return newPair.Value.Value;
		}
		return internalNode.Value;
	}

	public TValue RetrieveOrAdd(TKey key, Func<TValue> getValue)
	{
		return RetrieveOrAdd(key, (TKey keyIn) => getValue());
	}

	public virtual void Insert(int index, KeyValuePair<TKey, TValue> pair)
	{
		DoWriteNotify(() => _internalCollection.Insert(index, pair), () => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair, index));
	}

	public virtual KeyValuePair<TKey, TValue> RemoveAt(int index)
	{
		KeyValuePair<TKey, TValue> localRemovedItem = default(KeyValuePair<TKey, TValue>);
		DoReadWriteNotify(() => _internalCollection.GetItem(index), (KeyValuePair<TKey, TValue> item) => _internalCollection.RemoveAt(index), delegate(KeyValuePair<TKey, TValue> item)
		{
			localRemovedItem = item;
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
		});
		return localRemovedItem;
	}

	public bool Remove(TKey key)
	{
		return DoReadWriteNotify(() => _internalCollection.GetItemAndIndex(key), (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => _internalCollection.Remove(key), (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => (itemAndIndex == null) ? null : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemAndIndex.Item, itemAndIndex.Index)) != null;
	}

	public bool Remove(KeyValuePair<TKey, TValue> pair)
	{
		return DoTestReadWriteNotify(delegate
		{
			ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex = _internalCollection.GetItemAndIndex(pair.Key);
			return itemAndIndex != null && itemAndIndex.Item.Value.Equals(pair.Value);
		}, () => _internalCollection.GetItemAndIndex(pair.Key), (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => _internalCollection.Remove(pair.Key), (ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemAndIndex.Item, itemAndIndex.Index));
	}

	public IList<KeyValuePair<TKey, TValue>> RemoveRange(int index, int count)
	{
		IList<KeyValuePair<TKey, TValue>> localRemovedItems = null;
		DoReadWriteNotify(() => _internalCollection.GetRange(index, count), (IList<KeyValuePair<TKey, TValue>> items) => _internalCollection.RemoveRange(index, count), delegate(IList<KeyValuePair<TKey, TValue>> items)
		{
			localRemovedItems = items;
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)items, index);
		});
		return localRemovedItems;
	}

	public void RemoveRange(IEnumerable<TKey> keys)
	{
		IList<TKey> keysList = keys as IList<TKey>;
		if (keysList == null)
		{
			keysList = keys.ToList();
		}
		DoReadWriteNotify(() => _internalCollection.GetRange(keysList), (IList<KeyValuePair<TKey, TValue>> items) => _internalCollection.RemoveRange(keysList), (IList<KeyValuePair<TKey, TValue>> items) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)items));
	}

	public KeyValuePair<TKey, TValue> GetItem(int index)
	{
		return _internalCollection.GetItem(index);
	}

	public override string ToString()
	{
		return $"{{Items : {Count}}}";
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return _internalCollection.List.Select((ObservableDictionaryNode<TKey, TValue> x) => x.KeyValuePair).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _internalCollection.List.Select((ObservableDictionaryNode<TKey, TValue> x) => x.KeyValuePair).GetEnumerator();
	}

	public bool ContainsKey(TKey key)
	{
		return _internalCollection.Dictionary.ContainsKey(key);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex = _internalCollection.GetItemAndIndex(key);
		if (itemAndIndex != null)
		{
			value = itemAndIndex.Item.Value;
			return true;
		}
		value = default(TValue);
		return false;
	}

	public void Clear()
	{
		DoReadWriteNotify(() => ListSelect.Create(_internalCollection.List, (ObservableDictionaryNode<TKey, TValue> x) => x.KeyValuePair), (ListSelect<ObservableDictionaryNode<TKey, TValue>, KeyValuePair<TKey, TValue>> items) => ImmutableDictionaryListPair<TKey, TValue>.Empty, (ListSelect<ObservableDictionaryNode<TKey, TValue>, KeyValuePair<TKey, TValue>> items) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, 0));
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		ImmutableDictionaryListPair<TKey, TValue>.ItemAndIndex itemAndIndex = _internalCollection.GetItemAndIndex(item.Key);
		if (itemAndIndex != null)
		{
			return itemAndIndex.Item.Value.Equals(item.Value);
		}
		return false;
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		ListSelect.Create(_internalCollection.List, (ObservableDictionaryNode<TKey, TValue> node) => node.KeyValuePair).CopyTo(array, arrayIndex);
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		((ICollection)ListSelect.Create(_internalCollection.List, (ObservableDictionaryNode<TKey, TValue> node) => node.KeyValuePair)).CopyTo(array, arrayIndex);
	}

	protected override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		ImmutableDictionaryListPair<TKey, TValue> internalCollection = _internalCollection;
		KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[internalCollection.Count];
		for (int i = 0; i < internalCollection.Count; i++)
		{
			array[i] = internalCollection.GetItem(i);
		}
		info.AddValue("children", array);
	}

	protected ConcurrentObservableDictionary(SerializationInfo information, StreamingContext context)
		: base(information, context)
	{
		KeyValuePair<TKey, TValue>[] pairs = (KeyValuePair<TKey, TValue>[])information.GetValue("children", typeof(KeyValuePair<TKey, TValue>[]));
		_internalCollection = ImmutableDictionaryListPair<TKey, TValue>.Empty.AddRange(pairs);
	}
}
