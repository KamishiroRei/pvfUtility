using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Utools;

public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
{
	private int itemIndex;

	public new KeyCollection Keys => base.Keys;

	public new ValueCollection Values => base.Values;

	public new int Count => base.Count;

	public new TValue this[TKey key]
	{
		get
		{
			return GetValue(key);
		}
		set
		{
			SetValue(key, value);
		}
	}

	public TValue this[int index]
	{
		get
		{
			return GetValueAt(index);
		}
		set
		{
			SetValueAt(index, value);
		}
	}

	public event NotifyCollectionChangedEventHandler CollectionChanged;

	public event PropertyChangedEventHandler PropertyChanged;

	public ObservableDictionary()
	{
	}

	public new void Add(TKey key, TValue value)
	{
		base.Add(key, value);
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, FindItem(key), itemIndex));
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
		KeyValuePair<TKey, TValue> keyValuePair = FindItem(key);
		if (base.Remove(key))
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, keyValuePair, itemIndex));
			OnPropertyChanged("Keys");
			OnPropertyChanged("Values");
			OnPropertyChanged("Count");
			return true;
		}
		return false;
	}

	protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		if (CollectionChanged != null)
		{
			CollectionChanged(this, e);
		}
	}

	protected void OnPropertyChanged(string propertyName)
	{
		if (PropertyChanged != null)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	private TValue GetValueAt(int index)
	{
		for (int i = 0; i < Count; i++)
		{
			if (i == index)
			{
				return this.ElementAt(i).Value;
			}
		}
		return default(TValue);
	}

	private void SetValueAt(int index, TValue value)
	{
		try
		{
			SetValue(this.ElementAtOrDefault(index).Key, value);
		}
		catch (Exception)
		{
		}
	}

	private TValue GetValue(TKey key)
	{
		if (ContainsKey(key))
		{
			return base[key];
		}
		return default(TValue);
	}

	private void SetValue(TKey key, TValue value)
	{
		if (ContainsKey(key))
		{
			KeyValuePair<TKey, TValue> keyValuePair = FindItem(key);
			int oldItemIndex = itemIndex;
			base[key] = value;
			KeyValuePair<TKey, TValue> keyValuePair2 = FindItem(key);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, keyValuePair2, keyValuePair, oldItemIndex));
			OnPropertyChanged("Values");
			OnPropertyChanged("Item[]");
		}
		else
		{
			Add(key, value);
		}
	}

	private KeyValuePair<TKey, TValue> FindItem(TKey key)
	{
		itemIndex = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, TValue> current = enumerator.Current;
				if (current.Key.Equals(key))
				{
					return current;
				}
				itemIndex++;
			}
		}
		return default(KeyValuePair<TKey, TValue>);
	}

	private int IndexOfKey(TKey key)
	{
		int num = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Key.Equals(key))
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}
}
