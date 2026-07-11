using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Swordfish.NET.Collections;

public static class ListSelect
{
	public static ListSelect<TSource, TResult> Create<TSource, TResult>(IList<TSource> sourceList, Func<TSource, TResult> select)
	{
		return new ListSelect<TSource, TResult>(sourceList, select);
	}
}
public class ListSelect<TSource, TResult> : IList<TResult>, ICollection<TResult>, IEnumerable<TResult>, IEnumerable, IList, ICollection
{
	private IList<TSource> _sourceList;

	private Func<TSource, TResult> _select;

	TResult IList<TResult>.this[int index]
	{
		get
		{
			return _select(_sourceList[index]);
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	int ICollection<TResult>.Count => _sourceList.Count;

	bool ICollection<TResult>.IsReadOnly => true;

	bool IList.IsFixedSize => true;

	bool IList.IsReadOnly => true;

	int ICollection.Count => _sourceList.Count;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => null;

	object IList.this[int index]
	{
		get
		{
			return _select(_sourceList[index]);
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public ListSelect(IList<TSource> sourceList, Func<TSource, TResult> select)
	{
		_sourceList = sourceList;
		_select = select;
	}

	public void Add(TResult item)
	{
		throw new NotImplementedException();
	}

	public void Clear()
	{
		throw new NotImplementedException();
	}

	public bool Contains(TResult item)
	{
		return (from x in _sourceList
			select _select(x) into x
			where x.Equals(item)
			select x).Any();
	}

	public void CopyTo(TResult[] array, int arrayIndex)
	{
		int count = _sourceList.Count;
		int num = arrayIndex;
		for (int i = 0; i < count; i++)
		{
			array[num] = _select(_sourceList[i]);
			num++;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _sourceList.Select((TSource x) => _select(x)).GetEnumerator();
	}

	public IEnumerator<TResult> GetEnumerator()
	{
		return _sourceList.Select((TSource x) => _select(x)).GetEnumerator();
	}

	public int IndexOf(TResult item)
	{
		int num = 0;
		using (IEnumerator<TResult> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				TResult current = enumerator.Current;
				if (item.Equals(current))
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	void IList<TResult>.Insert(int index, TResult item)
	{
		throw new NotImplementedException();
	}

	bool ICollection<TResult>.Remove(TResult item)
	{
		throw new NotImplementedException();
	}

	void IList<TResult>.RemoveAt(int index)
	{
		throw new NotImplementedException();
	}

	int IList.Add(object value)
	{
		throw new NotImplementedException();
	}

	bool IList.Contains(object value)
	{
		return Contains((TResult)value);
	}

	int IList.IndexOf(object value)
	{
		return IndexOf((TResult)value);
	}

	void IList.Insert(int index, object value)
	{
		throw new NotImplementedException();
	}

	void IList.Remove(object value)
	{
		throw new NotImplementedException();
	}

	void IList.RemoveAt(int index)
	{
		throw new NotImplementedException();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		throw new NotImplementedException();
	}
}
