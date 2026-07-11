using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace Swordfish.NET.Collections.EditableBridges;

internal class EditableImmutableListBridge<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
{
	private IConcurrentObservableList<T> _destination;

	private ImmutableList<T> _source;

	private volatile Thread _guiCallerThread;

	public bool FreezeUpdates { get; set; }

	public T this[int index]
	{
		get
		{
			return _source[index];
		}
		set
		{
			Sync(() => ((IList<T>)_destination)[index] = value);
		}
	}

	public int Count => _source.Count;

	public bool IsReadOnly => false;

	bool IList.IsFixedSize => false;

	bool IList.IsReadOnly => false;

	int ICollection.Count => _source.Count;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => null;

	object IList.this[int index]
	{
		get
		{
			return ((IList)_source)[index];
		}
		set
		{
			Sync(() => ((IList)_destination)[index] = value);
		}
	}

	private EditableImmutableListBridge(ImmutableList<T> source, IConcurrentObservableList<T> destination)
	{
		_source = source;
		_destination = destination;
		Type typeFromHandle = typeof(EditableImmutableListBridge<T>);
		_ = typeFromHandle.IsGenericType;
		_ = typeFromHandle.GetGenericArguments().Length;
	}

	public static EditableImmutableListBridge<T> Empty(IConcurrentObservableList<T> destination)
	{
		return new EditableImmutableListBridge<T>(ImmutableList<T>.Empty, destination);
	}

	internal EditableImmutableListBridge<T> UpdateSource(ImmutableList<T> source)
	{
		EditableImmutableListBridge<T> result = ((_guiCallerThread != Thread.CurrentThread && !FreezeUpdates) ? new EditableImmutableListBridge<T>(source, _destination) : this);
		_guiCallerThread = null;
		return result;
	}

	private void Sync(Action action)
	{
		_guiCallerThread = Thread.CurrentThread;
		action();
		_source = _destination.ImmutableList;
		_guiCallerThread = null;
	}

	private TResult Sync<TResult>(Func<TResult> action)
	{
		_guiCallerThread = Thread.CurrentThread;
		TResult result = action();
		_source = _destination.ImmutableList;
		_guiCallerThread = null;
		return result;
	}

	public void Add(T item)
	{
		Sync(delegate
		{
			_destination.Add(item);
		});
	}

	public void Clear()
	{
		Sync(delegate
		{
			((ICollection<T>)_destination).Clear();
		});
	}

	public bool Contains(T item)
	{
		return ((ICollection<T>)_source).Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		((ICollection<T>)_source).CopyTo(array, arrayIndex);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _source.GetEnumerator();
	}

	public int IndexOf(T item)
	{
		return ((IList<T>)_source).IndexOf(item);
	}

	public void Insert(int index, T item)
	{
		Sync(delegate
		{
			_destination.Insert(index, item);
		});
	}

	public bool Remove(T item)
	{
		return Sync(() => _destination.Remove(item));
	}

	public void RemoveAt(int index)
	{
		Sync(delegate
		{
			((IList<T>)_destination).RemoveAt(index);
		});
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_source).GetEnumerator();
	}

	int IList.Add(object value)
	{
		return Sync(() => _destination.Add(value));
	}

	void IList.Clear()
	{
		Sync(delegate
		{
			((IList)_destination).Clear();
		});
	}

	bool IList.Contains(object value)
	{
		return ((IList)_source).Contains(value);
	}

	int IList.IndexOf(object value)
	{
		return ((IList)_source).IndexOf(value);
	}

	void IList.Insert(int index, object value)
	{
		Sync(delegate
		{
			_destination.Insert(index, value);
		});
	}

	void IList.Remove(object value)
	{
		Sync(delegate
		{
			_destination.Remove(value);
		});
	}

	void IList.RemoveAt(int index)
	{
		Sync(delegate
		{
			((IList)_destination).RemoveAt(index);
		});
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)_source).CopyTo(array, index);
	}
}
