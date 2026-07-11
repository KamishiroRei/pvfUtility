using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
public sealed class Deque<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
	private T[] arr = Empty<T>.Array;

	private int size;

	private int head;

	private int tail;

	public int Count => size;

	public T this[int index]
	{
		get
		{
			ThrowUtil.CheckInRangeInclusive(index, "index", 0, size - 1);
			return arr[(head + index) % arr.Length];
		}
		set
		{
			ThrowUtil.CheckInRangeInclusive(index, "index", 0, size - 1);
			arr[(head + index) % arr.Length] = value;
		}
	}

	bool ICollection<T>.IsReadOnly => false;

	public void Clear()
	{
		arr = Empty<T>.Array;
		size = 0;
		head = 0;
		tail = 0;
	}

	public void PushBack(T item)
	{
		if (size == arr.Length)
		{
			SetCapacity(Math.Max(4, arr.Length * 2));
		}
		arr[tail++] = item;
		if (tail == arr.Length)
		{
			tail = 0;
		}
		size++;
	}

	public T PopBack()
	{
		if (size == 0)
		{
			throw new InvalidOperationException();
		}
		if (tail == 0)
		{
			tail = arr.Length - 1;
		}
		else
		{
			tail--;
		}
		T result = arr[tail];
		arr[tail] = default(T);
		size--;
		return result;
	}

	public void PushFront(T item)
	{
		if (size == arr.Length)
		{
			SetCapacity(Math.Max(4, arr.Length * 2));
		}
		if (head == 0)
		{
			head = arr.Length - 1;
		}
		else
		{
			head--;
		}
		arr[head] = item;
		size++;
	}

	public T PopFront()
	{
		if (size == 0)
		{
			throw new InvalidOperationException();
		}
		T result = arr[head];
		arr[head] = default(T);
		head++;
		if (head == arr.Length)
		{
			head = 0;
		}
		size--;
		return result;
	}

	private void SetCapacity(int capacity)
	{
		T[] array = new T[capacity];
		CopyTo(array, 0);
		head = 0;
		tail = ((size != capacity) ? size : 0);
		arr = array;
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (head < tail)
		{
			for (int i = head; i < tail; i++)
			{
				yield return arr[i];
			}
			yield break;
		}
		for (int i = head; i < arr.Length; i++)
		{
			yield return arr[i];
		}
		for (int i = 0; i < tail; i++)
		{
			yield return arr[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ICollection<T>.Add(T item)
	{
		PushBack(item);
	}

	public bool Contains(T item)
	{
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		using (IEnumerator<T> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				if (equalityComparer.Equals(item, current))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (head < tail)
		{
			Array.Copy(arr, head, array, arrayIndex, tail - head);
			return;
		}
		int num = arr.Length - head;
		Array.Copy(arr, head, array, arrayIndex, num);
		Array.Copy(arr, 0, array, arrayIndex + num, tail);
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}
}
