using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Swordfish.NET.Collections;

public class ObservableCollectionAggregator<T> : ObservableCollection<T>
{
	private enum UpdateSourceType
	{
		NotUpdating,
		FromSource,
		FromDest
	}

	private UpdateSourceType _updateSource;

	private List<ObservableCollection<T>> _sourceCollections;

	private Dictionary<T, List<ObservableCollection<T>>> _itemToSourceCollectionOwners;

	public ObservableCollectionAggregator(params ObservableCollection<T>[] sourceCollections)
	{
		_sourceCollections = new List<ObservableCollection<T>>(sourceCollections);
		_itemToSourceCollectionOwners = new Dictionary<T, List<ObservableCollection<T>>>();
		foreach (ObservableCollection<T> sourceCollection in _sourceCollections)
		{
			sourceCollection.CollectionChanged += sourceCollection_CollectionChanged;
		}
		CollectionChanged += Aggregated_CollectionChanged;
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
				throw new ArgumentException("Can't add an item without an owner source");
			case NotifyCollectionChangedAction.Replace:
				throw new ArgumentException("Can't add an item without an owner source");
			case NotifyCollectionChangedAction.Remove:
			{
				foreach (T oldItem in e.OldItems)
				{
					if (!_itemToSourceCollectionOwners.TryGetValue(oldItem, out var value))
					{
						continue;
					}
					foreach (ObservableCollection<T> item in value)
					{
						while (item.Remove(oldItem))
						{
						}
					}
					_itemToSourceCollectionOwners.Remove(oldItem);
				}
				break;
			}
			case NotifyCollectionChangedAction.Reset:
				if (base.Count > 0)
				{
					throw new ArgumentException("Can't add an item without an owner source");
				}
				{
					foreach (ObservableCollection<T> sourceCollection in _sourceCollections)
					{
						sourceCollection.Clear();
					}
					break;
				}
			case NotifyCollectionChangedAction.Move:
				break;
			}
		}
		finally
		{
			_updateSource = UpdateSourceType.NotUpdating;
		}
	}

	private void AddRange(IEnumerable items, ObservableCollection<T> source)
	{
		foreach (T item in items)
		{
			if (_itemToSourceCollectionOwners.TryGetValue(item, out var value))
			{
				if (!value.Contains(source))
				{
					value.Add(source);
				}
			}
			else
			{
				value = new List<ObservableCollection<T>>();
				_itemToSourceCollectionOwners[item] = value;
				value.Add(source);
				Add(item);
			}
		}
	}

	private void RemoveRange(IEnumerable items, ObservableCollection<T> source)
	{
		foreach (T item in items)
		{
			if (_itemToSourceCollectionOwners.TryGetValue(item, out var value))
			{
				value.Remove(source);
				if (value.Count < 1)
				{
					Remove(item);
					_itemToSourceCollectionOwners.Remove(item);
				}
			}
		}
	}

	private void sourceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
				AddRange(e.NewItems, (ObservableCollection<T>)sender);
				break;
			case NotifyCollectionChangedAction.Remove:
				RemoveRange(e.OldItems, (ObservableCollection<T>)sender);
				break;
			case NotifyCollectionChangedAction.Replace:
				RemoveRange(e.OldItems, (ObservableCollection<T>)sender);
				AddRange(e.NewItems, (ObservableCollection<T>)sender);
				break;
			case NotifyCollectionChangedAction.Reset:
				Clear();
				_itemToSourceCollectionOwners.Clear();
				{
					foreach (ObservableCollection<T> sourceCollection in _sourceCollections)
					{
						AddRange(sourceCollection, sourceCollection);
					}
					break;
				}
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
