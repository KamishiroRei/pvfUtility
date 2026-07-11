using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Swordfish.NET.Collections;

public class ObservableCollectionDeduplicatorFactory<K, T>
{
	private enum UpdateSourceType
	{
		NotUpdating,
		FromSource,
		FromDest
	}

	private UpdateSourceType _updateSource;

	private ObservableCollection<K> _source;

	private ObservableCollection<T> _aggregated;

	private Dictionary<T, List<K>> _bridge;

	private Func<K, T> _convert;

	public Func<K, T> ConvertTo
	{
		get
		{
			return _convert;
		}
		set
		{
			_convert = value;
		}
	}

	public Func<T, IEnumerable<K>> ConvertFrom => (T t) => (!_bridge.ContainsKey(t)) ? Enumerable.Empty<K>() : _bridge[t];

	public ObservableCollection<K> Source => _source;

	public ObservableCollection<T> AggregatedSource => _aggregated;

	public ObservableCollectionDeduplicatorFactory(ObservableCollection<K> source, Func<K, T> convert)
	{
		_convert = convert;
		_bridge = new Dictionary<T, List<K>>();
		_aggregated = new ObservableCollection<T>();
		_source = source;
		_source.CollectionChanged += Source_CollectionChanged;
		_aggregated.CollectionChanged += Aggregated_CollectionChanged;
	}

	public ObservableCollectionDeduplicator<K, T> CreateBidirectionalDeduplicator(ObservableCollection<K> source, ObservableCollection<T> dest)
	{
		return new ObservableCollectionDeduplicator<K, T>(source, dest, this);
	}

	public ObservableCollectionDeduplicator<K, T> CreateBidirectionalDeduplicator()
	{
		return CreateBidirectionalDeduplicator(new ObservableCollection<K>(), new ObservableCollection<T>());
	}

	public ObservableCollectionDeduplicator<K, T> CreateBidirectionalDeduplicator(ObservableCollection<K> source)
	{
		return CreateBidirectionalDeduplicator(source, new ObservableCollection<T>());
	}

	public ObservableCollectionDeduplicator<K, T> CreateBidirectionalDeduplicator(ObservableCollection<T> dest)
	{
		return CreateBidirectionalDeduplicator(new ObservableCollection<K>(), dest);
	}

	private void AddItem(K item)
	{
		T val = _convert(item);
		if (_bridge.ContainsKey(val))
		{
			_bridge[val].Add(item);
			return;
		}
		_bridge.Add(val, new List<K>());
		_bridge[val].Add(item);
		_aggregated.Add(val);
	}

	private void RemoveItem(K item)
	{
		T val = _convert(item);
		if (_bridge.ContainsKey(val))
		{
			_bridge[val].Remove(item);
			if (_bridge[val].Count < 1)
			{
				_bridge.Remove(val);
				_aggregated.Remove(val);
			}
		}
		else
		{
			_aggregated.Remove(val);
		}
	}

	private void AddRange(IEnumerable items)
	{
		foreach (K item in items)
		{
			AddItem(item);
		}
	}

	private void RemoveRange(IEnumerable items)
	{
		foreach (K item in items)
		{
			RemoveItem(item);
		}
	}

	private void Aggregated_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
				throw new InvalidOperationException("Can't handle adding items aggregated value with no original value");
			case NotifyCollectionChangedAction.Replace:
				throw new InvalidOperationException("Can't handle adding items aggregated value with no original value");
			case NotifyCollectionChangedAction.Remove:
			{
				foreach (T oldItem in e.OldItems)
				{
					if (!_bridge.ContainsKey(oldItem))
					{
						continue;
					}
					foreach (K item in _bridge[oldItem])
					{
						_source.Remove(item);
					}
					_aggregated.Remove(oldItem);
					_bridge.Remove(oldItem);
				}
				break;
			}
			case NotifyCollectionChangedAction.Reset:
				_source.Clear();
				if (_aggregated.Count > 0)
				{
					throw new InvalidOperationException("Can't handle adding items aggregated value with no original value");
				}
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
				AddRange(e.NewItems);
				break;
			case NotifyCollectionChangedAction.Remove:
				RemoveRange(e.OldItems);
				break;
			case NotifyCollectionChangedAction.Replace:
				RemoveRange(e.OldItems);
				AddRange(e.NewItems);
				break;
			case NotifyCollectionChangedAction.Reset:
				_aggregated.Clear();
				_bridge.Clear();
				AddRange(_source);
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
