using System;
using System.Collections;
using System.Collections.Generic;

namespace Swordfish.NET.Collections;

internal class BinarySorter<TKey> : IComparer<TKey>
{
	private IComparer<TKey> _comparer;

	private bool _defaultCompareFailed;

	public BinarySorter(IComparer<TKey> comparer = null)
	{
		_comparer = comparer;
	}

	public int GetInsertIndex(int count, TKey key, Func<int, TKey> indexToKey)
	{
		return BinarySearchForIndex(0, count - 1, key, indexToKey);
	}

	public int GetMatchIndex(int count, TKey key, Func<int, TKey> indexToKey)
	{
		return BinarySearchForMatch(0, count - 1, key, indexToKey);
	}

	public int Compare(TKey key1, TKey key2)
	{
		if (_comparer != null)
		{
			return _comparer.Compare(key1, key2);
		}
		if (!_defaultCompareFailed)
		{
			try
			{
				return Comparer.Default.Compare(key1, key2);
			}
			catch (Exception)
			{
				_defaultCompareFailed = true;
			}
		}
		return string.Compare(key1.ToString(), key2.ToString(), StringComparison.InvariantCultureIgnoreCase);
	}

	private int BinarySearchForIndex(int low, int high, TKey key, Func<int, TKey> indexToKey)
	{
		while (high >= low)
		{
			int num = low + (high - low >> 1);
			int num2 = Compare(indexToKey(num), key);
			if (num2 == 0)
			{
				return num;
			}
			if (num2 < 0)
			{
				low = num + 1;
			}
			else
			{
				high = num - 1;
			}
		}
		return low;
	}

	private int BinarySearchForMatch(int low, int high, TKey key, Func<int, TKey> indexToKey)
	{
		while (high >= low)
		{
			int num = low + (high - low >> 1);
			int num2 = Compare(indexToKey(num), key);
			if (num2 == 0)
			{
				return num;
			}
			if (num2 < 0)
			{
				low = num + 1;
			}
			else
			{
				high = num - 1;
			}
		}
		return -1;
	}
}
