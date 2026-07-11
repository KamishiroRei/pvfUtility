using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Swordfish.NET.Collections;

public class CircularBuffer<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ICollection
{
	private int _capacity;

	private int _size;

	private int _head;

	private int _tail;

	private T[] _buffer;

	[NonSerialized]
	private object syncRoot;

	public bool AllowOverflow { get; set; }

	public int Capacity
	{
		get
		{
			return _capacity;
		}
		set
		{
			if (value != _capacity)
			{
				if (value < _size)
				{
					throw new ArgumentOutOfRangeException("value", "The new capacity must be greater than or equal to the buffer size.");
				}
				T[] array = new T[value];
				if (_size > 0)
				{
					CopyTo(array);
				}
				_buffer = array;
				_capacity = value;
			}
		}
	}

	public int Size => _size;

	int ICollection<T>.Count => Size;

	bool ICollection<T>.IsReadOnly => false;

	int ICollection.Count => Size;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
	{
		get
		{
			if (syncRoot == null)
			{
				Interlocked.CompareExchange(ref syncRoot, new object(), null);
			}
			return syncRoot;
		}
	}

	public CircularBuffer(int capacity)
		: this(capacity, false)
	{
	}

	public CircularBuffer(T[] initialBuffer, bool allowOverflow)
	{
		_capacity = initialBuffer.Length;
		_size = 0;
		_head = 0;
		_tail = 0;
		_buffer = initialBuffer;
		AllowOverflow = allowOverflow;
	}

	public CircularBuffer(int capacity, bool allowOverflow)
	{
		if (capacity < 0)
		{
			throw new ArgumentException("The buffer capacity must be greater than or equal to zero", "capacity");
		}
		_capacity = capacity;
		_size = 0;
		_head = 0;
		_tail = 0;
		_buffer = new T[capacity];
		AllowOverflow = allowOverflow;
	}

	public bool Contains(T item)
	{
		int num = _head;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		int num2 = 0;
		while (num2 < _size)
		{
			if (num == _capacity)
			{
				num = 0;
			}
			if (item == null && _buffer[num] == null)
			{
				return true;
			}
			if (_buffer[num] != null && equalityComparer.Equals(_buffer[num], item))
			{
				return true;
			}
			num2++;
			num++;
		}
		return false;
	}

	public void Clear()
	{
		_size = 0;
		_head = 0;
		_tail = 0;
	}

	public int Put(T[] src)
	{
		return Put(src, 0, src.Length);
	}

	public int Put(T[] src, int offset, int count)
	{
		if (!AllowOverflow && count > _capacity - _size)
		{
			throw new InvalidOperationException("The buffer does not have sufficient capacity to put new items.");
		}
		int num = offset;
		int num2 = 0;
		while (num2 < count)
		{
			if (_tail == _capacity)
			{
				_tail = 0;
			}
			_buffer[_tail] = src[num];
			num2++;
			_tail++;
			num++;
		}
		_size = Math.Min(_size + count, _capacity);
		return count;
	}

	public void Put(T item)
	{
		if (!AllowOverflow && _size == _capacity)
		{
			throw new InvalidOperationException("The buffer does not have sufficient capacity to put new items.");
		}
		_buffer[_tail] = item;
		if (++_tail == _capacity)
		{
			_tail = 0;
		}
		_size++;
	}

	public T Put()
	{
		if (!AllowOverflow && _size == _capacity)
		{
			throw new InvalidOperationException("The buffer does not have sufficient capacity to put new items.");
		}
		T result = _buffer[_tail];
		if (++_tail == _capacity)
		{
			_tail = 0;
		}
		_size++;
		return result;
	}

	public void Skip(int count)
	{
		_head += count;
		if (_head >= _capacity)
		{
			_head -= _capacity;
		}
	}

	public T[] Get(int count)
	{
		T[] array = new T[count];
		Get(array);
		return array;
	}

	public int Get(T[] dst)
	{
		return Get(dst, 0, dst.Length);
	}

	public int Get(T[] dst, int offset, int count)
	{
		int num = Math.Min(count, _size);
		int num2 = offset;
		int num3 = 0;
		while (num3 < num)
		{
			if (_head == _capacity)
			{
				_head = 0;
			}
			dst[num2] = _buffer[_head];
			num3++;
			_head++;
			num2++;
		}
		_size -= num;
		return num;
	}

	public int Get(ref T item)
	{
		if (_size == 0)
		{
			return 0;
		}
		item = _buffer[_head];
		if (++_head == _capacity)
		{
			_head = 0;
		}
		_size--;
		return 1;
	}

	public void CopyTo(T[] array)
	{
		CopyTo(array, 0);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		CopyTo(0, array, arrayIndex, _size);
	}

	public void CopyTo(int index, T[] array, int arrayIndex, int count)
	{
		if (count > _size)
		{
			throw new ArgumentOutOfRangeException("count", "The read count cannot be greater than the buffer size.");
		}
		int num = _head;
		int num2 = 0;
		while (num2 < count)
		{
			if (num == _capacity)
			{
				num = 0;
			}
			array[arrayIndex] = _buffer[num];
			num2++;
			num++;
			arrayIndex++;
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		int bufferIndex = _head;
		int i = 0;
		while (i < _size)
		{
			if (bufferIndex == _capacity)
			{
				bufferIndex = 0;
			}
			yield return _buffer[bufferIndex];
			i++;
			bufferIndex++;
		}
	}

	public T[] GetBuffer()
	{
		return _buffer;
	}

	public T[] ToArray()
	{
		T[] array = new T[_size];
		CopyTo(array);
		return array;
	}

	void ICollection<T>.Add(T item)
	{
		Put(item);
	}

	bool ICollection<T>.Remove(T item)
	{
		if (_size == 0)
		{
			return false;
		}
		Get(ref item);
		return true;
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		CopyTo((T[])array, arrayIndex);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
