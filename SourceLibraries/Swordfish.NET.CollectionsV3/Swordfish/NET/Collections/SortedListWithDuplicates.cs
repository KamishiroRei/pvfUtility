using System.Collections;
using System.Collections.Generic;

namespace Swordfish.NET.Collections;

public class SortedListWithDuplicates<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private IList<T> _list;

	private BinarySorter<T> _sorter;

	public T this[int index]
	{
		get
		{
			return _list[index];
		}
		set
		{
			RemoveAt(index);
			Add(value);
		}
	}

	public int Count => _list.Count;

	public bool IsReadOnly => false;

	public SortedListWithDuplicates(IComparer<T> comparer = null, IList<T> baseList = null)
	{
		_sorter = new BinarySorter<T>(comparer);
		_list = baseList ?? new List<T>();
	}

	public int IndexOf(T item)
	{
		return _list.IndexOf(item);
	}

	public void Insert(int index, T item)
	{
		Add(item);
	}

	public void RemoveAt(int index)
	{
		_list.RemoveAt(index);
	}

	public void Add(T item)
	{
		int insertIndex = _sorter.GetInsertIndex(_list.Count, item, (int i) => _list[i]);
		_list.Insert(insertIndex, item);
	}

	public void Clear()
	{
		_list.Clear();
	}

	public bool Contains(T item)
	{
		return _list.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		_list.CopyTo(array, arrayIndex);
	}

	public bool Remove(T item)
	{
		return _list.Remove(item);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_list).GetEnumerator();
	}
}
