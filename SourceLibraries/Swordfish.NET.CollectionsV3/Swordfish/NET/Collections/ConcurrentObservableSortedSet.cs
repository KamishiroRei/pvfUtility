using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace Swordfish.NET.Collections;

[Serializable]
public class ConcurrentObservableSortedSet<T> : ConcurrentObservableBase<T, ICollection<T>>, ICollection<T>, IEnumerable<T>, IEnumerable, ISet<T>, ICollection, ISerializable
{
	public override IList<T> CollectionView => (IList<T>)_internalCollection;

	public override int Count => _internalCollection.Count;

	public bool IsReadOnly => false;

	object ICollection.SyncRoot => ((ICollection)_internalCollection).SyncRoot;

	bool ICollection.IsSynchronized => ((ICollection)_internalCollection).IsSynchronized;

	public ConcurrentObservableSortedSet()
		: this(true)
	{
	}

	public ConcurrentObservableSortedSet(bool isMultithreaded)
		: base(isMultithreaded, (ICollection<T>)ImmutableSortedSet<T>.Empty)
	{
	}

	public bool Add(T value)
	{
		bool wasAdded = false;
		DoReadWriteNotify(() => _internalCollection.Count, delegate
		{
			ImmutableSortedSet<T> immutableSortedSet = ((ImmutableSortedSet<T>)_internalCollection).Add(value);
			wasAdded = immutableSortedSet != _internalCollection;
			return immutableSortedSet;
		}, (int index) => (!wasAdded) ? null : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
		return wasAdded;
	}

	public bool Remove(T value)
	{
		bool wasRemoved = false;
		DoReadWriteNotify(() => _internalCollection.Count, delegate
		{
			ImmutableSortedSet<T> immutableSortedSet = ((ImmutableSortedSet<T>)_internalCollection).Remove(value);
			wasRemoved = immutableSortedSet != _internalCollection;
			return immutableSortedSet;
		}, (int index) => (!wasRemoved) ? null : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
		return wasRemoved;
	}

	public override string ToString()
	{
		return $"{{Items : {Count}}}";
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _internalCollection.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _internalCollection.GetEnumerator();
	}

	public void Clear()
	{
		DoReadWriteNotify(() => _internalCollection.ToList(), (List<T> items) => ImmutableSortedSet<T>.Empty, (List<T> items) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, 0));
	}

	public bool Contains(T item)
	{
		return _internalCollection.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		((ICollection)_internalCollection).CopyTo(array, arrayIndex);
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		((ICollection)_internalCollection).CopyTo(array, arrayIndex);
	}

	void ICollection<T>.Add(T item)
	{
		Add(item);
	}

	void ISet<T>.UnionWith(IEnumerable<T> other)
	{
		((ISet<T>)_internalCollection).UnionWith(other);
	}

	void ISet<T>.IntersectWith(IEnumerable<T> other)
	{
		((ISet<T>)_internalCollection).IntersectWith(other);
	}

	void ISet<T>.ExceptWith(IEnumerable<T> other)
	{
		((ISet<T>)_internalCollection).ExceptWith(other);
	}

	void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
	{
		((ISet<T>)_internalCollection).SymmetricExceptWith(other);
	}

	bool ISet<T>.IsSubsetOf(IEnumerable<T> other)
	{
		return ((ISet<T>)_internalCollection).IsSubsetOf(other);
	}

	bool ISet<T>.IsSupersetOf(IEnumerable<T> other)
	{
		return ((ISet<T>)_internalCollection).IsSupersetOf(other);
	}

	bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other)
	{
		return ((ISet<T>)_internalCollection).IsProperSupersetOf(other);
	}

	bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other)
	{
		return ((ISet<T>)_internalCollection).IsProperSubsetOf(other);
	}

	bool ISet<T>.Overlaps(IEnumerable<T> other)
	{
		return ((ISet<T>)_internalCollection).Overlaps(other);
	}

	bool ISet<T>.SetEquals(IEnumerable<T> other)
	{
		return ((ISet<T>)_internalCollection).SetEquals(other);
	}

	protected override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		T[] value = _internalCollection.ToArray();
		info.AddValue("children", value);
	}

	protected ConcurrentObservableSortedSet(SerializationInfo information, StreamingContext context)
		: base(information, context)
	{
		_internalCollection = ImmutableSortedSet<T>.Empty;
		T[] array = (T[])information.GetValue("children", typeof(T[]));
		foreach (T value in array)
		{
			_internalCollection = ((ImmutableSortedSet<T>)_internalCollection).Add(value);
		}
	}
}
