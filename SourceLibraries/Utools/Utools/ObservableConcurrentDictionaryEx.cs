using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Collections.Pooled;

namespace Utools;

[DebuggerDisplay("Count={Count}")]
public class ObservableConcurrentDictionaryEx<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
{
	private readonly SynchronizationContext synchronizationContext;

	private readonly PooledDictionary<TKey, TValue> dictionary;

	int ICollection<KeyValuePair<TKey, TValue>>.Count => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Count;

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly;

	public ICollection<TKey> Keys => dictionary.Keys;

	public ICollection<TValue> Values => dictionary.Values;

	public TValue this[TKey key]
	{
		get
		{
			return dictionary[key];
		}
		set
		{
			SetValueAndNotify(key, value);
		}
	}

	public event NotifyCollectionChangedEventHandler CollectionChanged;

	public event PropertyChangedEventHandler PropertyChanged;

	~ObservableConcurrentDictionaryEx()
	{
		dictionary?.Dispose();
	}

	public void Dispose()
	{
		dictionary?.Dispose();
	}

	public ObservableConcurrentDictionaryEx()
	{
		synchronizationContext = AsyncOperationManager.SynchronizationContext;
		dictionary = new PooledDictionary<TKey, TValue>();
	}

	public ObservableConcurrentDictionaryEx(int count)
	{
		synchronizationContext = AsyncOperationManager.SynchronizationContext;
		dictionary = new PooledDictionary<TKey, TValue>(count);
	}

	public void NotifyObserversOfChange()
	{
		NotifyCollectionChangedEventHandler collectionChanged = CollectionChanged;
		PropertyChangedEventHandler propertyChanged = PropertyChanged;
		if (collectionChanged == null && propertyChanged == null)
		{
			return;
		}
		synchronizationContext.Post(delegate
		{
			if (collectionChanged != null)
			{
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs("Count"));
				propertyChanged(this, new PropertyChangedEventArgs("Keys"));
				propertyChanged(this, new PropertyChangedEventArgs("Values"));
			}
		}, null);
	}

	public void DoNotify(string properName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(properName));
	}

	public void NotifyObserversOfChangeRemove()
	{
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
	}

	protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		if (CollectionChanged != null)
		{
			CollectionChanged(this, e);
		}
	}

	private bool AddAndNotify(KeyValuePair<TKey, TValue> item)
	{
		return AddAndNotify(item.Key, item.Value);
	}

	private bool AddAndNotify(TKey key, TValue value)
	{
		bool num = dictionary.TryAdd(key, value);
		if (num)
		{
			NotifyObserversOfChange();
		}
		return num;
	}

	public bool AddTry(TKey key, TValue value)
	{
		return dictionary.TryAdd(key, value);
	}

	public void Clear()
	{
		dictionary.Clear();
		NotifyObserversOfChange();
	}

	private bool RemoveAndNotify(TKey key, out TValue value)
	{
		bool num = dictionary.Remove(key, out value);
		if (num)
		{
			NotifyObserversOfChange();
		}
		return num;
	}

	private void SetValueAndNotify(TKey key, TValue value)
	{
		dictionary[key] = value;
		NotifyObserversOfChange();
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		AddAndNotify(item);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Clear()
	{
		((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Clear();
		NotifyObserversOfChange();
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		TValue val;
		return RemoveAndNotify(item.Key, out val);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return ((IEnumerable<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
	}

	public void Add(TKey key, TValue value)
	{
		AddAndNotify(key, value);
	}

	public void Add(KeyValuePair<TKey, TValue> keyValuePair)
	{
		dictionary.AddRange(keyValuePair);
	}

	public bool ContainsKey(TKey key)
	{
		return dictionary.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		TValue val;
		return RemoveAndNotify(key, out val);
	}

	public bool RemoveTry(TKey key)
	{
		lock (this)
		{
			TValue value;
			return dictionary.Remove(key, out value);
		}
	}

	public bool Removes(List<TKey> keys)
	{
		foreach (TKey key in keys)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary.Remove(key, out var _);
			}
		}
		NotifyObserversOfChange();
		return true;
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return dictionary.TryGetValue(key, out value);
	}
}
