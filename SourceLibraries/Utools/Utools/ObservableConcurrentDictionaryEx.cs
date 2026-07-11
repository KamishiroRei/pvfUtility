using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Collections.Pooled;

namespace Utools;

[DebuggerDisplay("Count={Count}")]
public class ObservableConcurrentDictionaryEx<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass12_0
	{
		public NotifyCollectionChangedEventHandler YkAK28TCTP;

		public ObservableConcurrentDictionaryEx<TKey, TValue> jpfK5pPUft;

		public PropertyChangedEventHandler bAdKXwVwTe;

		public _003C_003Ec__DisplayClass12_0()
		{
		}

		internal void ScfKihorPA(object? s)
		{
			if (YkAK28TCTP != null)
			{
				YkAK28TCTP(jpfK5pPUft, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			if (bAdKXwVwTe != null)
			{
				bAdKXwVwTe(jpfK5pPUft, new PropertyChangedEventArgs("Count"));
				bAdKXwVwTe(jpfK5pPUft, new PropertyChangedEventArgs("Keys"));
				bAdKXwVwTe(jpfK5pPUft, new PropertyChangedEventArgs("Values"));
			}
		}
	}

	private readonly SynchronizationContext mP49wdyb8f;

	private readonly PooledDictionary<TKey, TValue> YG49expDOf;

	[CompilerGenerated]
	private NotifyCollectionChangedEventHandler C0n9HK8Y1n;

	[CompilerGenerated]
	private PropertyChangedEventHandler yED9fd5vbH;

	int ICollection<KeyValuePair<TKey, TValue>>.Count => ((ICollection<KeyValuePair<TKey, TValue>>)YG49expDOf).Count;

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)YG49expDOf).IsReadOnly;

	public ICollection<TKey> Keys => YG49expDOf.Keys;

	public ICollection<TValue> Values => YG49expDOf.Values;

	public TValue this[TKey key]
	{
		get
		{
			return YG49expDOf[key];
		}
		set
		{
			cwv9x5H2oc(key, value);
		}
	}

	public event NotifyCollectionChangedEventHandler CollectionChanged
	{
		[CompilerGenerated]
		add
		{
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = C0n9HK8Y1n;
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler2;
			do
			{
				notifyCollectionChangedEventHandler2 = notifyCollectionChangedEventHandler;
				NotifyCollectionChangedEventHandler value2 = (NotifyCollectionChangedEventHandler)Delegate.Combine(notifyCollectionChangedEventHandler2, value);
				notifyCollectionChangedEventHandler = Interlocked.CompareExchange(ref C0n9HK8Y1n, value2, notifyCollectionChangedEventHandler2);
			}
			while ((object)notifyCollectionChangedEventHandler != notifyCollectionChangedEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = C0n9HK8Y1n;
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler2;
			do
			{
				notifyCollectionChangedEventHandler2 = notifyCollectionChangedEventHandler;
				NotifyCollectionChangedEventHandler value2 = (NotifyCollectionChangedEventHandler)Delegate.Remove(notifyCollectionChangedEventHandler2, value);
				notifyCollectionChangedEventHandler = Interlocked.CompareExchange(ref C0n9HK8Y1n, value2, notifyCollectionChangedEventHandler2);
			}
			while ((object)notifyCollectionChangedEventHandler != notifyCollectionChangedEventHandler2);
		}
	}

	public event PropertyChangedEventHandler PropertyChanged
	{
		[CompilerGenerated]
		add
		{
			PropertyChangedEventHandler propertyChangedEventHandler = yED9fd5vbH;
			PropertyChangedEventHandler propertyChangedEventHandler2;
			do
			{
				propertyChangedEventHandler2 = propertyChangedEventHandler;
				PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Combine(propertyChangedEventHandler2, value);
				propertyChangedEventHandler = Interlocked.CompareExchange(ref yED9fd5vbH, value2, propertyChangedEventHandler2);
			}
			while ((object)propertyChangedEventHandler != propertyChangedEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			PropertyChangedEventHandler propertyChangedEventHandler = yED9fd5vbH;
			PropertyChangedEventHandler propertyChangedEventHandler2;
			do
			{
				propertyChangedEventHandler2 = propertyChangedEventHandler;
				PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Remove(propertyChangedEventHandler2, value);
				propertyChangedEventHandler = Interlocked.CompareExchange(ref yED9fd5vbH, value2, propertyChangedEventHandler2);
			}
			while ((object)propertyChangedEventHandler != propertyChangedEventHandler2);
		}
	}

	~ObservableConcurrentDictionaryEx()
	{
		YG49expDOf?.Dispose();
	}

	public void Dispose()
	{
		YG49expDOf?.Dispose();
	}

	public ObservableConcurrentDictionaryEx()
	{
		mP49wdyb8f = AsyncOperationManager.SynchronizationContext;
		YG49expDOf = new PooledDictionary<TKey, TValue>();
	}

	public ObservableConcurrentDictionaryEx(int count)
	{
		mP49wdyb8f = AsyncOperationManager.SynchronizationContext;
		YG49expDOf = new PooledDictionary<TKey, TValue>(count);
	}

	public void NotifyObserversOfChange()
	{
		_003C_003Ec__DisplayClass12_0 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass12_0();
		CS_0024_003C_003E8__locals15.jpfK5pPUft = this;
		CS_0024_003C_003E8__locals15.YkAK28TCTP = C0n9HK8Y1n;
		CS_0024_003C_003E8__locals15.bAdKXwVwTe = yED9fd5vbH;
		if (CS_0024_003C_003E8__locals15.YkAK28TCTP == null && CS_0024_003C_003E8__locals15.bAdKXwVwTe == null)
		{
			return;
		}
		mP49wdyb8f.Post(delegate
		{
			if (CS_0024_003C_003E8__locals15.YkAK28TCTP != null)
			{
				CS_0024_003C_003E8__locals15.YkAK28TCTP(CS_0024_003C_003E8__locals15.jpfK5pPUft, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			if (CS_0024_003C_003E8__locals15.bAdKXwVwTe != null)
			{
				CS_0024_003C_003E8__locals15.bAdKXwVwTe(CS_0024_003C_003E8__locals15.jpfK5pPUft, new PropertyChangedEventArgs("Count"));
				CS_0024_003C_003E8__locals15.bAdKXwVwTe(CS_0024_003C_003E8__locals15.jpfK5pPUft, new PropertyChangedEventArgs("Keys"));
				CS_0024_003C_003E8__locals15.bAdKXwVwTe(CS_0024_003C_003E8__locals15.jpfK5pPUft, new PropertyChangedEventArgs("Values"));
			}
		}, null);
	}

	public void DoNotify(string properName = "")
	{
		yED9fd5vbH?.Invoke(this, new PropertyChangedEventArgs(properName));
	}

	public void NotifyObserversOfChangeRemove()
	{
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
	}

	protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		if (C0n9HK8Y1n != null)
		{
			C0n9HK8Y1n(this, e);
		}
	}

	private bool NXb9NrVb4H(KeyValuePair<TKey, TValue> P_0)
	{
		return Mmg9TKd7YI(P_0.Key, P_0.Value);
	}

	private bool Mmg9TKd7YI(TKey FxhWXf9kouV1en6T70L, TValue MUjgr59cLXrTrOiwN5Q)
	{
		bool num = YG49expDOf.TryAdd(FxhWXf9kouV1en6T70L, MUjgr59cLXrTrOiwN5Q);
		if (num)
		{
			NotifyObserversOfChange();
		}
		return num;
	}

	public bool AddTry(TKey key, TValue value)
	{
		return YG49expDOf.TryAdd(key, value);
	}

	public void Clear()
	{
		YG49expDOf.Clear();
		NotifyObserversOfChange();
	}

	private bool dJY9RYbesZ(TKey D3ZRZh9gxwBlG8dqgW4, out TValue P_1)
	{
		bool num = YG49expDOf.Remove(D3ZRZh9gxwBlG8dqgW4, out P_1);
		if (num)
		{
			NotifyObserversOfChange();
		}
		return num;
	}

	private void cwv9x5H2oc(TKey xCqHwD93vScKsw165ag, TValue yokPaG9EJAHpJ1N0to7)
	{
		YG49expDOf[xCqHwD93vScKsw165ag] = yokPaG9EJAHpJ1N0to7;
		NotifyObserversOfChange();
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		NXb9NrVb4H(item);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Clear()
	{
		((ICollection<KeyValuePair<TKey, TValue>>)YG49expDOf).Clear();
		NotifyObserversOfChange();
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		return ((ICollection<KeyValuePair<TKey, TValue>>)YG49expDOf).Contains(item);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		((ICollection<KeyValuePair<TKey, TValue>>)YG49expDOf).CopyTo(array, arrayIndex);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		TValue val;
		return dJY9RYbesZ(item.Key, out val);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return ((IEnumerable<KeyValuePair<TKey, TValue>>)YG49expDOf).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<KeyValuePair<TKey, TValue>>)YG49expDOf).GetEnumerator();
	}

	public void Add(TKey key, TValue value)
	{
		Mmg9TKd7YI(key, value);
	}

	public void Add(KeyValuePair<TKey, TValue> keyValuePair)
	{
		YG49expDOf.AddRange(keyValuePair);
	}

	public bool ContainsKey(TKey key)
	{
		return YG49expDOf.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		TValue val;
		return dJY9RYbesZ(key, out val);
	}

	public bool RemoveTry(TKey key)
	{
		lock (this)
		{
			TValue value;
			return YG49expDOf.Remove(key, out value);
		}
	}

	public bool Removes(List<TKey> keys)
	{
		foreach (TKey key in keys)
		{
			if (YG49expDOf.ContainsKey(key))
			{
				YG49expDOf.Remove(key, out var _);
			}
		}
		NotifyObserversOfChange();
		return true;
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return YG49expDOf.TryGetValue(key, out value);
	}
}
