using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Swordfish.NET.Collections;

public class ObservableCollectionDeduplicator<K, T>
{
	private enum UpdateSourceType
	{
		NotUpdating,
		FromSource,
		FromDest
	}

	private UpdateSourceType _updateSource;

	private Dictionary<T, int> _itemCount = new Dictionary<T, int>();

	private ObservableCollection<K> _source;

	private ObservableCollection<T> _deduplicated;

	private ObservableCollectionDeduplicatorFactory<K, T> _factory;

	public ObservableCollection<K> Source => _source;

	public ObservableCollection<T> Deduplicated => _deduplicated;

	public ObservableCollectionDeduplicator(ObservableCollection<K> source, ObservableCollection<T> dest, ObservableCollectionDeduplicatorFactory<K, T> factory)
	{
		_factory = factory;
		_source = source;
		_deduplicated = dest;
		AddRangeSourceToDest(_source);
		_source.CollectionChanged += Source_CollectionChanged;
		_deduplicated.CollectionChanged += DeduplicatedDest_CollectionChanged;
		_factory.Source.CollectionChanged += MasterSource_CollectionChanged;
	}

	private void MasterSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Replace:
		{
			foreach (K oldItem in e.OldItems)
			{
				if (_source.Contains(oldItem))
				{
					_source.Remove(oldItem);
				}
			}
			break;
		}
		case NotifyCollectionChangedAction.Remove:
		{
			foreach (K oldItem2 in e.OldItems)
			{
				if (_source.Contains(oldItem2))
				{
					_source.Remove(oldItem2);
				}
			}
			break;
		}
		case NotifyCollectionChangedAction.Reset:
			_source.Clear();
			_deduplicated.Clear();
			break;
		case NotifyCollectionChangedAction.Add:
		case NotifyCollectionChangedAction.Move:
			break;
		}
	}

	private void AddRangeDestToSource(IList items)
	{
		foreach (T item in items)
		{
			foreach (K item2 in _factory.ConvertFrom(item))
			{
				_source.Add(item2);
			}
		}
	}

	private void RemoveRangeDestToSource(IList items)
	{
		foreach (T item in items)
		{
			foreach (K item2 in _factory.ConvertFrom(item))
			{
				_source.Remove(item2);
			}
		}
	}

	private void AddRangeSourceToDest(IList items)
	{
		foreach (K item in items)
		{
			T val = _factory.ConvertTo(item);
			if (!_itemCount.ContainsKey(val))
			{
				_deduplicated.Add(val);
				_itemCount[val] = 1;
			}
			else
			{
				_itemCount[val]++;
			}
		}
	}

	private void RemoveRangeSourceToDest(IList items)
	{
		foreach (K item in items)
		{
			T val = _factory.ConvertTo(item);
			if (_itemCount.ContainsKey(val))
			{
				_itemCount[val]--;
				if (_itemCount[val] < 1)
				{
					_itemCount.Remove(val);
					_deduplicated.Remove(val);
				}
			}
			else
			{
				_deduplicated.Remove(val);
			}
		}
	}

	private void DeduplicatedDest_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (_updateSource != UpdateSourceType.NotUpdating)
		{
			return;
		}
		try
		{
			_updateSource = UpdateSourceType.FromDest;
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				AddRangeDestToSource(e.NewItems);
				break;
			case NotifyCollectionChangedAction.Remove:
				RemoveRangeDestToSource(e.OldItems);
				break;
			case NotifyCollectionChangedAction.Replace:
				RemoveRangeDestToSource(e.OldItems);
				AddRangeDestToSource(e.NewItems);
				break;
			case NotifyCollectionChangedAction.Reset:
				_source.Clear();
				_itemCount.Clear();
				AddRangeDestToSource(_deduplicated);
				break;
			case NotifyCollectionChangedAction.Move:
				break;
			}
		}
		finally
		{
			_updateSource = UpdateSourceType.NotUpdating;
		}
	}

	private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (_updateSource != UpdateSourceType.NotUpdating)
		{
			return;
		}
		try
		{
			_updateSource = UpdateSourceType.FromSource;
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				AddRangeSourceToDest(e.NewItems);
				break;
			case NotifyCollectionChangedAction.Remove:
				RemoveRangeSourceToDest(e.OldItems);
				break;
			case NotifyCollectionChangedAction.Replace:
				RemoveRangeSourceToDest(e.OldItems);
				AddRangeSourceToDest(e.NewItems);
				break;
			case NotifyCollectionChangedAction.Reset:
				_deduplicated.Clear();
				_itemCount.Clear();
				AddRangeSourceToDest(_source);
				break;
			case NotifyCollectionChangedAction.Move:
				break;
			}
		}
		finally
		{
			_updateSource = UpdateSourceType.NotUpdating;
		}
	}
}
