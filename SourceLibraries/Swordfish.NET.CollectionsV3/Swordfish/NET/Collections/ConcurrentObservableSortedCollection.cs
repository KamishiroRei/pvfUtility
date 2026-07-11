using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;

namespace Swordfish.NET.Collections;

public class ConcurrentObservableSortedCollection<T> : ConcurrentObservableCollection<T>
{
	private BinarySorter<T> _sorter;

	public override T this[int index]
	{
		get
		{
			return base[index];
		}
		set
		{
			RemoveAt(index);
			Add(value);
		}
	}

	public ConcurrentObservableSortedCollection()
		: this(true, (IComparer<T>)null)
	{
	}

	public ConcurrentObservableSortedCollection(bool isMultithreaded)
		: this(isMultithreaded, (IComparer<T>)null)
	{
	}

	public ConcurrentObservableSortedCollection(IComparer<T> comparer)
		: this(true, comparer)
	{
	}

	public ConcurrentObservableSortedCollection(bool isMultithreaded, IComparer<T> comparer)
		: base(isMultithreaded)
	{
		_sorter = new BinarySorter<T>(comparer);
	}

	protected override int IListAdd(T item)
	{
		return DoReadWriteNotify(() => _sorter.GetInsertIndex(base.ImmutableList.Count, item, (int i) => _internalCollection[i]), (int index) => base.ImmutableList.Insert(index, item), (int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
	}

	public override void AddRange(IList<T> items)
	{
		Func<int, IList<T>> write = delegate
		{
			ImmutableList<T> updatedCollection = base.ImmutableList;
			foreach (T item in items)
			{
				int insertIndex = _sorter.GetInsertIndex(updatedCollection.Count, item, (int i) => updatedCollection[i]);
				updatedCollection = updatedCollection.Insert(insertIndex, item);
			}
			return updatedCollection;
		};
		DoReadWriteNotify(() => 0, write, (int nothing) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList()));
	}

	public override void Reset(IList<T> items)
	{
		DoReadWriteNotify(() => base.ImmutableList.ToArray(), (T[] oldItems) => ImmutableList<T>.Empty.AddRange(items.OrderBy((T x) => x, _sorter).ToList()), (T[] oldItems) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, 0), (T[] oldItems) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, 0));
	}

	public override void Insert(int index, T item)
	{
		Add(item);
	}

	public override void InsertRange(int index, IList<T> items)
	{
		AddRange(items);
	}
}
