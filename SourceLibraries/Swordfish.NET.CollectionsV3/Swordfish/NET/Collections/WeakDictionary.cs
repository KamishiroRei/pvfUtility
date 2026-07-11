using System;
using System.Collections.Generic;

namespace Swordfish.NET.Collections;

public sealed class WeakDictionary<TKey, TValue> : BaseDictionary<TKey, TValue> where TKey : class where TValue : class
{
	private Dictionary<object, WeakReference<TValue>> dictionary;

	private WeakKeyComparer<TKey> comparer;

	public override int Count => dictionary.Count;

	public WeakDictionary()
		: this(0, (IEqualityComparer<TKey>)null)
	{
	}

	public WeakDictionary(int capacity)
		: this(capacity, (IEqualityComparer<TKey>)null)
	{
	}

	public WeakDictionary(IEqualityComparer<TKey> comparer)
		: this(0, comparer)
	{
	}

	public WeakDictionary(int capacity, IEqualityComparer<TKey> comparer)
	{
		this.comparer = new WeakKeyComparer<TKey>(comparer);
		dictionary = new Dictionary<object, WeakReference<TValue>>(capacity, this.comparer);
	}

	public override void Add(TKey key, TValue value)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		WeakReference<TKey> key2 = new WeakKeyReference<TKey>(key, comparer);
		WeakReference<TValue> value2 = WeakReference<TValue>.Create(value);
		dictionary.Add(key2, value2);
	}

	public override bool ContainsKey(TKey key)
	{
		return dictionary.ContainsKey(key);
	}

	public override bool Remove(TKey key)
	{
		return dictionary.Remove(key);
	}

	public override bool TryGetValue(TKey key, out TValue value)
	{
		if (dictionary.TryGetValue(key, out var value2))
		{
			value = value2.Target;
			return value2.IsAlive;
		}
		value = null;
		return false;
	}

	protected override void SetValue(TKey key, TValue value)
	{
		WeakReference<TKey> key2 = new WeakKeyReference<TKey>(key, comparer);
		dictionary[key2] = WeakReference<TValue>.Create(value);
	}

	public override void Clear()
	{
		dictionary.Clear();
	}

	public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		foreach (KeyValuePair<object, WeakReference<TValue>> item in dictionary)
		{
			WeakReference<TKey> obj = (WeakReference<TKey>)item.Key;
			WeakReference<TValue> value = item.Value;
			TKey target = obj.Target;
			TValue target2 = value.Target;
			if (obj.IsAlive && value.IsAlive)
			{
				yield return new KeyValuePair<TKey, TValue>(target, target2);
			}
		}
	}

	public void RemoveCollectedEntries()
	{
		List<object> list = null;
		foreach (KeyValuePair<object, WeakReference<TValue>> item in dictionary)
		{
			WeakReference<TKey> weakReference = (WeakReference<TKey>)item.Key;
			WeakReference<TValue> value = item.Value;
			if (!weakReference.IsAlive || !value.IsAlive)
			{
				if (list == null)
				{
					list = new List<object>();
				}
				list.Add(weakReference);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (object item2 in list)
		{
			dictionary.Remove(item2);
		}
	}
}
