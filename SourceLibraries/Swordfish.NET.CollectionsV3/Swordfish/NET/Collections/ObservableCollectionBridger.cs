using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Swordfish.NET.Collections.Auxiliary;

namespace Swordfish.NET.Collections;

public class ObservableCollectionBridger : ObservableCollectionBridger<object, object>
{
	private ObservableCollectionBridger()
	{
	}
}
public class ObservableCollectionBridger<T1> : ObservableCollectionBridger<T1, T1>
{
	private ObservableCollectionBridger()
	{
	}
}
public class ObservableCollectionBridger<T1, T2>
{
	private enum UpdateSourceType
	{
		NotUpdating,
		FromCollection1,
		FromCollection2
	}

	private UpdateSourceType _updateSource;

	private Func<T1, T2> _converter1to2;

	private Func<T2, T1> _converter2to1;

	private ICollection<T1> _collection1;

	private ICollection<T2> _collection2;

	public static IDisposable Bridge<BT1>(ObservableCollection<BT1> collection1, ObservableCollection<BT1> collection2)
	{
		return ObservableCollectionBridger<T1, T2>.Bridge<BT1, BT1, ObservableCollection<BT1>, ObservableCollection<BT1>>(collection1, collection2, (Func<BT1, BT1>)((BT1 x) => x), (Func<BT1, BT1>)((BT1 x) => x));
	}

	public static IDisposable Bridge<BT1, BT2>(ObservableCollection<BT1> collection1, ObservableCollection<BT2> collection2, Func<BT1, BT2> converter1to2, Func<BT2, BT1> converter2to1)
	{
		return ObservableCollectionBridger<T1, T2>.Bridge<BT1, BT2, ObservableCollection<BT1>, ObservableCollection<BT2>>(collection1, collection2, converter1to2, converter2to1);
	}

	public static IDisposable Bridge<BT1, BT2, CT1, CT2>(CT1 collection1, CT2 collection2, Func<BT1, BT2> converter1to2, Func<BT2, BT1> converter2to1) where CT1 : ICollection<BT1>, INotifyCollectionChanged where CT2 : ICollection<BT2>, INotifyCollectionChanged
	{
		ObservableCollectionBridger<BT1, BT2> bridger = new ObservableCollectionBridger<BT1, BT2>();
		bridger._collection1 = collection1;
		bridger._collection2 = collection2;
		bridger._converter1to2 = converter1to2;
		bridger._converter2to1 = converter2to1;
		collection1.CollectionChanged += bridger.collection1_CollectionChanged;
		collection2.CollectionChanged += bridger.collection2_CollectionChanged;
		return new AnonDisposable(delegate
		{
			collection1.CollectionChanged -= bridger.collection1_CollectionChanged;
			collection2.CollectionChanged -= bridger.collection2_CollectionChanged;
		});
	}

	internal ObservableCollectionBridger()
	{
	}

	private void collection2_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (_updateSource != UpdateSourceType.NotUpdating)
		{
			return;
		}
		try
		{
			_updateSource = UpdateSourceType.FromCollection2;
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
			{
				foreach (T2 newItem in e.NewItems)
				{
					_collection1.Add(_converter2to1(newItem));
				}
				break;
			}
			case NotifyCollectionChangedAction.Remove:
			{
				foreach (T2 oldItem in e.OldItems)
				{
					_collection1.Remove(_converter2to1(oldItem));
				}
				break;
			}
			case NotifyCollectionChangedAction.Replace:
				foreach (T2 oldItem2 in e.OldItems)
				{
					_collection1.Remove(_converter2to1(oldItem2));
				}
				{
					foreach (T2 newItem2 in e.NewItems)
					{
						_collection1.Add(_converter2to1(newItem2));
					}
					break;
				}
			case NotifyCollectionChangedAction.Reset:
				_collection1.Clear();
				{
					foreach (T2 item in _collection2)
					{
						_collection1.Add(_converter2to1(item));
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

	private void collection1_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (_updateSource != UpdateSourceType.NotUpdating)
		{
			return;
		}
		try
		{
			_updateSource = UpdateSourceType.FromCollection1;
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
			{
				foreach (T1 newItem in e.NewItems)
				{
					_collection2.Add(_converter1to2(newItem));
				}
				break;
			}
			case NotifyCollectionChangedAction.Remove:
			{
				foreach (T1 oldItem in e.OldItems)
				{
					_collection2.Remove(_converter1to2(oldItem));
				}
				break;
			}
			case NotifyCollectionChangedAction.Replace:
				foreach (T1 oldItem2 in e.OldItems)
				{
					_collection2.Remove(_converter1to2(oldItem2));
				}
				{
					foreach (T1 newItem2 in e.NewItems)
					{
						_collection2.Add(_converter1to2(newItem2));
					}
					break;
				}
			case NotifyCollectionChangedAction.Reset:
				_collection1.Clear();
				{
					foreach (T1 item in _collection1)
					{
						_collection2.Add(_converter1to2(item));
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
