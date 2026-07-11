using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Utools;

public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
{
	private int HWG7V8P3QH;

	[CompilerGenerated]
	private NotifyCollectionChangedEventHandler KFR79ydYLf;

	[CompilerGenerated]
	private PropertyChangedEventHandler AVu77Ldd0e;

	public new KeyCollection Keys => base.Keys;

	public new ValueCollection Values => base.Values;

	public new int Count => base.Count;

	public new TValue this[TKey key]
	{
		get
		{
			return gSi9d3lmGD(key);
		}
		set
		{
			RsR9u9YL0g(key, value);
		}
	}

	public TValue this[int index]
	{
		get
		{
			return VE49Inr7HQ(index);
		}
		set
		{
			Xxx9nxRIE8(index, value);
		}
	}

	public event NotifyCollectionChangedEventHandler CollectionChanged
	{
		[CompilerGenerated]
		add
		{
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = KFR79ydYLf;
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler2;
			do
			{
				notifyCollectionChangedEventHandler2 = notifyCollectionChangedEventHandler;
				NotifyCollectionChangedEventHandler value2 = (NotifyCollectionChangedEventHandler)Delegate.Combine(notifyCollectionChangedEventHandler2, value);
				notifyCollectionChangedEventHandler = Interlocked.CompareExchange(ref KFR79ydYLf, value2, notifyCollectionChangedEventHandler2);
			}
			while ((object)notifyCollectionChangedEventHandler != notifyCollectionChangedEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = KFR79ydYLf;
			NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler2;
			do
			{
				notifyCollectionChangedEventHandler2 = notifyCollectionChangedEventHandler;
				NotifyCollectionChangedEventHandler value2 = (NotifyCollectionChangedEventHandler)Delegate.Remove(notifyCollectionChangedEventHandler2, value);
				notifyCollectionChangedEventHandler = Interlocked.CompareExchange(ref KFR79ydYLf, value2, notifyCollectionChangedEventHandler2);
			}
			while ((object)notifyCollectionChangedEventHandler != notifyCollectionChangedEventHandler2);
		}
	}

	public event PropertyChangedEventHandler PropertyChanged
	{
		[CompilerGenerated]
		add
		{
			PropertyChangedEventHandler propertyChangedEventHandler = AVu77Ldd0e;
			PropertyChangedEventHandler propertyChangedEventHandler2;
			do
			{
				propertyChangedEventHandler2 = propertyChangedEventHandler;
				PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Combine(propertyChangedEventHandler2, value);
				propertyChangedEventHandler = Interlocked.CompareExchange(ref AVu77Ldd0e, value2, propertyChangedEventHandler2);
			}
			while ((object)propertyChangedEventHandler != propertyChangedEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			PropertyChangedEventHandler propertyChangedEventHandler = AVu77Ldd0e;
			PropertyChangedEventHandler propertyChangedEventHandler2;
			do
			{
				propertyChangedEventHandler2 = propertyChangedEventHandler;
				PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Remove(propertyChangedEventHandler2, value);
				propertyChangedEventHandler = Interlocked.CompareExchange(ref AVu77Ldd0e, value2, propertyChangedEventHandler2);
			}
			while ((object)propertyChangedEventHandler != propertyChangedEventHandler2);
		}
	}

	public ObservableDictionary()
	{
	}

	public new void Add(TKey key, TValue value)
	{
		base.Add(key, value);
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, wdI9JWKB5L(key), HWG7V8P3QH));
		OnPropertyChanged("Keys");
		OnPropertyChanged("Values");
		OnPropertyChanged("Count");
	}

	public new void Clear()
	{
		base.Clear();
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		OnPropertyChanged("Keys");
		OnPropertyChanged("Values");
		OnPropertyChanged("Count");
	}

	public new bool Remove(TKey key)
	{
		KeyValuePair<TKey, TValue> keyValuePair = wdI9JWKB5L(key);
		if (base.Remove(key))
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, keyValuePair, HWG7V8P3QH));
			OnPropertyChanged("Keys");
			OnPropertyChanged("Values");
			OnPropertyChanged("Count");
			return true;
		}
		return false;
	}

	protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		if (KFR79ydYLf != null)
		{
			KFR79ydYLf(this, e);
		}
	}

	protected void OnPropertyChanged(string propertyName)
	{
		if (AVu77Ldd0e != null)
		{
			AVu77Ldd0e(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	private TValue VE49Inr7HQ(int P_0)
	{
		for (int i = 0; i < Count; i++)
		{
			if (i == P_0)
			{
				return this.ElementAt(i).Value;
			}
		}
		return default(TValue);
	}

	private void Xxx9nxRIE8(int P_0, TValue SPJyLq9O3QPNe0cZkcR)
	{
		try
		{
			RsR9u9YL0g(this.ElementAtOrDefault(P_0).Key, SPJyLq9O3QPNe0cZkcR);
		}
		catch (Exception)
		{
		}
	}

	private TValue gSi9d3lmGD(TKey M1HDCy98i3UwAwGwVai)
	{
		if (ContainsKey(M1HDCy98i3UwAwGwVai))
		{
			return base[M1HDCy98i3UwAwGwVai];
		}
		return default(TValue);
	}

	private void RsR9u9YL0g(TKey pOMhZv9hvCPPWbhDTAj, TValue wuiLSR945F6N801VCNF)
	{
		if (ContainsKey(pOMhZv9hvCPPWbhDTAj))
		{
			KeyValuePair<TKey, TValue> keyValuePair = wdI9JWKB5L(pOMhZv9hvCPPWbhDTAj);
			int hWG7V8P3QH = HWG7V8P3QH;
			base[pOMhZv9hvCPPWbhDTAj] = wuiLSR945F6N801VCNF;
			KeyValuePair<TKey, TValue> keyValuePair2 = wdI9JWKB5L(pOMhZv9hvCPPWbhDTAj);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, keyValuePair2, keyValuePair, hWG7V8P3QH));
			OnPropertyChanged("Values");
			OnPropertyChanged("Item[]");
		}
		else
		{
			Add(pOMhZv9hvCPPWbhDTAj, wuiLSR945F6N801VCNF);
		}
	}

	private KeyValuePair<TKey, TValue> wdI9JWKB5L(TKey MlsAvG90GgGKnuTFO0n)
	{
		HWG7V8P3QH = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, TValue> current = enumerator.Current;
				if (current.Key.Equals(MlsAvG90GgGKnuTFO0n))
				{
					return current;
				}
				HWG7V8P3QH++;
			}
		}
		return default(KeyValuePair<TKey, TValue>);
	}

	private int EVg9zW6IDt(TKey MwdOdj7Yd25wikENeGp)
	{
		int num = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Key.Equals(MwdOdj7Yd25wikENeGp))
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}
}
