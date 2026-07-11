using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Utools;

[DebuggerDisplay("Count={Count}")]
public class ObservableConcurrentDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_0
	{
		public NotifyCollectionChangedEventHandler WrqKDafsyw;

		public ObservableConcurrentDictionary<TKey, TValue> lB0K1CFUAp;

		public PropertyChangedEventHandler VkRKsKsivE;

		public _003C_003Ec__DisplayClass9_0()
		{
		}

		internal void PLhKSss14S(object? s)
		{
			if (WrqKDafsyw != null)
			{
				WrqKDafsyw(lB0K1CFUAp, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			if (VkRKsKsivE != null)
			{
				VkRKsKsivE(lB0K1CFUAp, new PropertyChangedEventArgs("Count"));
				VkRKsKsivE(lB0K1CFUAp, new PropertyChangedEventArgs("Keys"));
				VkRKsKsivE(lB0K1CFUAp, new PropertyChangedEventArgs("Values"));
			}
		}
	}

	private readonly SynchronizationContext efM9aQCXok;

	private readonly ConcurrentDictionary<TKey, TValue> RP89m9nCEO;

	[CompilerGenerated]
	private NotifyCollectionChangedEventHandler I3l9l8xivq;

	[CompilerGenerated]
	private PropertyChangedEventHandler Atj9vpZEgB;

	int ICollection<KeyValuePair<TKey, TValue>>.Count => ((ICollection<KeyValuePair<TKey, TValue>>)RP89m9nCEO).Count;

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)RP89m9nCEO).IsReadOnly;

	public ICollection<TKey> Keys => RP89m9nCEO.Keys;

	public ICollection<TValue> Values => RP89m9nCEO.Values;

	public TValue this[TKey key]
	{
		get
		{
			return RP89m9nCEO[key];
		}
		set
		{
			Dvo9Uw6eRJ(key, value);
		}
	}

	public event NotifyCollectionChangedEventHandler CollectionChanged
	{
		[CompilerGenerated]
		add
		{
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = I3l9l8xivq;
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler2;
			do
			{
				notifyCollectionChangedEventHandler2 = notifyCollectionChangedEventHandler;
				NotifyCollectionChangedEventHandler value2 = (NotifyCollectionChangedEventHandler)Delegate.Combine(notifyCollectionChangedEventHandler2, value);
				notifyCollectionChangedEventHandler = Interlocked.CompareExchange(ref I3l9l8xivq, value2, notifyCollectionChangedEventHandler2);
			}
			while ((object)notifyCollectionChangedEventHandler != notifyCollectionChangedEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = I3l9l8xivq;
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler2;
			do
			{
				notifyCollectionChangedEventHandler2 = notifyCollectionChangedEventHandler;
				NotifyCollectionChangedEventHandler value2 = (NotifyCollectionChangedEventHandler)Delegate.Remove(notifyCollectionChangedEventHandler2, value);
				notifyCollectionChangedEventHandler = Interlocked.CompareExchange(ref I3l9l8xivq, value2, notifyCollectionChangedEventHandler2);
			}
			while ((object)notifyCollectionChangedEventHandler != notifyCollectionChangedEventHandler2);
		}
	}

	public event PropertyChangedEventHandler PropertyChanged
	{
		[CompilerGenerated]
		add
		{
			PropertyChangedEventHandler propertyChangedEventHandler = Atj9vpZEgB;
			PropertyChangedEventHandler propertyChangedEventHandler2;
			do
			{
				propertyChangedEventHandler2 = propertyChangedEventHandler;
				PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Combine(propertyChangedEventHandler2, value);
				propertyChangedEventHandler = Interlocked.CompareExchange(ref Atj9vpZEgB, value2, propertyChangedEventHandler2);
			}
			while ((object)propertyChangedEventHandler != propertyChangedEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			PropertyChangedEventHandler propertyChangedEventHandler = Atj9vpZEgB;
			PropertyChangedEventHandler propertyChangedEventHandler2;
			do
			{
				propertyChangedEventHandler2 = propertyChangedEventHandler;
				PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Remove(propertyChangedEventHandler2, value);
				propertyChangedEventHandler = Interlocked.CompareExchange(ref Atj9vpZEgB, value2, propertyChangedEventHandler2);
			}
			while ((object)propertyChangedEventHandler != propertyChangedEventHandler2);
		}
	}

	public ObservableConcurrentDictionary()
	{
		efM9aQCXok = AsyncOperationManager.SynchronizationContext;
		RP89m9nCEO = new ConcurrentDictionary<TKey, TValue>();
	}

	public void NotifyObserversOfChange()
	{
		_003C_003Ec__DisplayClass9_0 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass9_0();
		CS_0024_003C_003E8__locals15.lB0K1CFUAp = this;
		CS_0024_003C_003E8__locals15.WrqKDafsyw = I3l9l8xivq;
		CS_0024_003C_003E8__locals15.VkRKsKsivE = Atj9vpZEgB;
		if (CS_0024_003C_003E8__locals15.WrqKDafsyw == null && CS_0024_003C_003E8__locals15.VkRKsKsivE == null)
		{
			return;
		}
		efM9aQCXok.Post(delegate
		{
			if (CS_0024_003C_003E8__locals15.WrqKDafsyw != null)
			{
				CS_0024_003C_003E8__locals15.WrqKDafsyw(CS_0024_003C_003E8__locals15.lB0K1CFUAp, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			if (CS_0024_003C_003E8__locals15.VkRKsKsivE != null)
			{
				CS_0024_003C_003E8__locals15.VkRKsKsivE(CS_0024_003C_003E8__locals15.lB0K1CFUAp, new PropertyChangedEventArgs("Count"));
				CS_0024_003C_003E8__locals15.VkRKsKsivE(CS_0024_003C_003E8__locals15.lB0K1CFUAp, new PropertyChangedEventArgs("Keys"));
				CS_0024_003C_003E8__locals15.VkRKsKsivE(CS_0024_003C_003E8__locals15.lB0K1CFUAp, new PropertyChangedEventArgs("Values"));
			}
		}, null);
	}

	private bool vv49i9879l(KeyValuePair<TKey, TValue> P_0)
	{
		return H2192kyBm7(P_0.Key, P_0.Value);
	}

	private bool H2192kyBm7(TKey JmvKmE95veOvx3w2Fa9, TValue vL993D9XAtcu5sthjqY)
	{
		bool num = RP89m9nCEO.TryAdd(JmvKmE95veOvx3w2Fa9, vL993D9XAtcu5sthjqY);
		if (num)
		{
			NotifyObserversOfChange();
		}
		return num;
	}

	public bool AddTry(TKey key, TValue value)
	{
		return RP89m9nCEO.TryAdd(key, value);
	}

	public void Clear()
	{
		RP89m9nCEO.Clear();
		NotifyObserversOfChange();
	}

	private bool JbQ9WTc6Ih(TKey ADTqMh9CAlxkVcceWTS, out TValue P_1)
	{
		bool num = RP89m9nCEO.TryRemove(ADTqMh9CAlxkVcceWTS, out P_1);
		if (num)
		{
			NotifyObserversOfChange();
		}
		return num;
	}

	private void Dvo9Uw6eRJ(TKey cu9qsc9Lg03qJlxytHr, TValue EqTYsv9BJIb6nHvqHkO)
	{
		RP89m9nCEO[cu9qsc9Lg03qJlxytHr] = EqTYsv9BJIb6nHvqHkO;
		NotifyObserversOfChange();
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		vv49i9879l(item);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Clear()
	{
		((ICollection<KeyValuePair<TKey, TValue>>)RP89m9nCEO).Clear();
		NotifyObserversOfChange();
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		return ((ICollection<KeyValuePair<TKey, TValue>>)RP89m9nCEO).Contains(item);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		((ICollection<KeyValuePair<TKey, TValue>>)RP89m9nCEO).CopyTo(array, arrayIndex);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		TValue val;
		return JbQ9WTc6Ih(item.Key, out val);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return ((IEnumerable<KeyValuePair<TKey, TValue>>)RP89m9nCEO).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<KeyValuePair<TKey, TValue>>)RP89m9nCEO).GetEnumerator();
	}

	public void Add(TKey key, TValue value)
	{
		H2192kyBm7(key, value);
	}

	public bool ContainsKey(TKey key)
	{
		return RP89m9nCEO.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		TValue val;
		return JbQ9WTc6Ih(key, out val);
	}

	public bool RemoveTry(TKey key)
	{
		TValue value;
		return RP89m9nCEO.TryRemove(key, out value);
	}

	public bool Removes(List<TKey> keys)
	{
		foreach (TKey key in keys)
		{
			if (RP89m9nCEO.ContainsKey(key))
			{
				RP89m9nCEO.TryRemove(key, out var _);
			}
		}
		NotifyObserversOfChange();
		return true;
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return RP89m9nCEO.TryGetValue(key, out value);
	}
}
