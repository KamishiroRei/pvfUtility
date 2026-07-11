using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Swordfish.NET.Collections.EditableBridges;

namespace Swordfish.NET.Collections;

[Serializable]
public class ConcurrentObservableCollection<T> : ConcurrentObservableBase<T, IList<T>>, IConcurrentObservableList<T>, IConcurrentObservableBase<T>, INotifyPropertyChanged, INotifyCollectionChanged, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IEditableCollection, ISerializable
{
	private EditableImmutableListBridge<T> _editableCollectionView;

	private ConcurrentDictionary<Thread, ImmutableList<T>> _collectionAtlastCount = new ConcurrentDictionary<Thread, ImmutableList<T>>();

	public ImmutableList<T> ImmutableList => (ImmutableList<T>)_internalCollection;

	public IList<T> EditableCollectionView => _editableCollectionView;

	public virtual T this[int index]
	{
		get
		{
			return ImmutableList[index];
		}
		set
		{
			DoReadWriteNotify(() => ImmutableList[index], (T item) => ImmutableList.SetItem(index, value), (T item) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, item, index));
		}
	}

	int ICollection<T>.Count
	{
		get
		{
			ImmutableList<T> immutableList = ImmutableList;
			_collectionAtlastCount[Thread.CurrentThread] = immutableList;
			return immutableList.Count;
		}
	}

	public override int Count => ImmutableList.Count;

	public bool IsReadOnly => false;

	public override IList<T> CollectionView => _internalCollection;

	bool ICollection.IsSynchronized => ((ICollection)ImmutableList).IsSynchronized;

	object ICollection.SyncRoot => ((ICollection)ImmutableList).SyncRoot;

	bool IList.IsFixedSize => ((IList)ImmutableList).IsFixedSize;

	bool IList.IsReadOnly => ((IList)ImmutableList).IsReadOnly;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			this[index] = (T)value;
		}
	}

	public ConcurrentObservableCollection()
		: this(true)
	{
	}

	public ConcurrentObservableCollection(bool isMultithreaded)
		: base(isMultithreaded, (IList<T>)ImmutableList<T>.Empty)
	{
		_editableCollectionView = EditableImmutableListBridge<T>.Empty(this);
		base.PropertyChanged += delegate(object? s, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CollectionView")
			{
				RaisePropertyChanged("EditableCollectionView");
			}
		};
	}

	protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs changes)
	{
		_editableCollectionView = _editableCollectionView.UpdateSource((ImmutableList<T>)_internalCollection);
		base.OnCollectionChanged(changes);
	}

	protected virtual int IListAdd(T item)
	{
		return DoReadWriteNotify(() => ImmutableList.Count, (int index) => ImmutableList.Add(item), (int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
	}

	public T RemoveLast()
	{
		return DoReadWriteNotify(() => new
		{
			Index = ImmutableList.Count - 1,
			Item = ImmutableList.LastOrDefault()
		}, indexAndItem => (indexAndItem.Index >= 0) ? ImmutableList.RemoveAt(indexAndItem.Index) : ImmutableList, indexAndItem => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, indexAndItem.Item)).Item;
	}

	public virtual void AddRange(IList<T> items)
	{
		DoReadWriteNotify(() => ImmutableList.Count, (int index) => ImmutableList.AddRange(items), (int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, index));
	}

	public virtual void InsertRange(int index, IList<T> items)
	{
		DoWriteNotify(() => ImmutableList.InsertRange(index, items), () => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, index));
	}

	public void RemoveRange(int index, int count)
	{
		DoReadWriteNotify(() => ImmutableList.GetRange(index, count), (ImmutableList<T> items) => ImmutableList.RemoveRange(index, count), (ImmutableList<T> items) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index));
	}

	public void RemoveRange(IList<T> items)
	{
		DoWriteNotify(() => ImmutableList.RemoveRange(items), () => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)items));
	}

	public virtual void Reset(IList<T> items)
	{
		DoReadWriteNotify(() => ImmutableList.ToArray(), (T[] oldItems) => ImmutableList<T>.Empty.AddRange(items), (T[] oldItems) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, 0), (T[] oldItems) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, 0));
	}

	public T[] ToArray()
	{
		return ImmutableList.ToArray();
	}

	public List<T> ToList()
	{
		return ImmutableList.ToList();
	}

	public override string ToString()
	{
		return $"{{Items : {Count}}}";
	}

	public void BeginEditingItem()
	{
		_lock.EnterWriteLock();
		_editableCollectionView.FreezeUpdates = true;
		_lock.ExitWriteLock();
	}

	public void EndedEditingItem()
	{
		_lock.EnterWriteLock();
		_editableCollectionView.FreezeUpdates = false;
		_lock.ExitWriteLock();
	}

	public IEnumerator<T> GetEnumerator()
	{
		return ImmutableList.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ImmutableList.GetEnumerator();
	}

	public int IndexOf(T item)
	{
		return ImmutableList.IndexOf(item);
	}

	public virtual void Insert(int index, T item)
	{
		DoWriteNotify(() => ImmutableList.Insert(index, item), () => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
	}

	public void RemoveAt(int index)
	{
		DoReadWriteNotify(() => ImmutableList[index], (T item) => ImmutableList.RemoveAt(index), (T item) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
	}

	public void Add(T item)
	{
		IListAdd(item);
	}

	public void Clear()
	{
		DoReadWriteNotify(() => ImmutableList.ToArray(), (T[] items) => ImmutableList.Clear(), (T[] items) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, 0));
	}

	public bool Contains(T item)
	{
		return ImmutableList.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		ImmutableList<T> value = null;
		if (!_collectionAtlastCount.TryRemove(Thread.CurrentThread, out value))
		{
			value = ImmutableList;
		}
		int num = Math.Min(array.Length - arrayIndex, value.Count);
		for (int i = 0; i < num; i++)
		{
			array[i + arrayIndex] = value[i];
		}
	}

	public bool Remove(T item)
	{
		return DoReadWriteNotify(() => ImmutableList.IndexOf(item), (int index) => (index >= 0) ? ImmutableList.RemoveAt(index) : ImmutableList, (int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index)) >= 0;
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)ImmutableList).CopyTo(array, index);
	}

	int IList.Add(object value)
	{
		if (!(value is T item))
		{
			return -1;
		}
		return IListAdd(item);
	}

	bool IList.Contains(object value)
	{
		return ((IList)ImmutableList).Contains(value);
	}

	int IList.IndexOf(object value)
	{
		return ((IList)ImmutableList).IndexOf(value);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, (T)value);
	}

	void IList.Remove(object value)
	{
		Remove((T)value);
	}

	void IList.RemoveAt(int index)
	{
		RemoveAt(index);
	}

	protected override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("children", _internalCollection.ToArray());
	}

	protected ConcurrentObservableCollection(SerializationInfo information, StreamingContext context)
		: base(information, context)
	{
		_internalCollection = System.Collections.Immutable.ImmutableList.CreateRange((T[])information.GetValue("children", typeof(T[])));
	}
}
