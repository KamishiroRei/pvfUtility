using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swordfish.NET.Collections.Auxiliary;

namespace Swordfish.NET.Collections;

public class VirtualizingCollection<T> : ExtendedNotifyPropertyChanged, IList<VirtualizingCollectionDataWrapper<T>>, ICollection<VirtualizingCollectionDataWrapper<T>>, IEnumerable<VirtualizingCollectionDataWrapper<T>>, IEnumerable, IList, ICollection where T : class
{
	private readonly IVirtualizingCollectionItemsProvider<T> _itemsProvider;

	private readonly int _pageSize = 100;

	private readonly long _pageTimeout = 100000L;

	private int _count = -1;

	private Dictionary<int, VirtualizingCollectionDataPage<T>> _pages = new Dictionary<int, VirtualizingCollectionDataPage<T>>();

	private DateTime _lastPageCleanUp = DateTime.Now;

	public IVirtualizingCollectionItemsProvider<T> ItemsProvider => _itemsProvider;

	public int PageSize => _pageSize;

	public long PageTimeout => _pageTimeout;

	public int Count
	{
		get
		{
			if (_count == -1)
			{
				_count = 0;
				LoadCount();
			}
			return _count;
		}
		protected set
		{
			_count = value;
		}
	}

	public VirtualizingCollectionDataWrapper<T> this[int index]
	{
		get
		{
			try
			{
				int num = index / PageSize;
				int num2 = index % PageSize;
				RequestPage(num);
				if (num2 > PageSize / 2 && num < Count / PageSize)
				{
					RequestPage(num + 1);
				}
				if (num2 < PageSize / 2 && num > 0)
				{
					RequestPage(num - 1);
				}
				IList<VirtualizingCollectionDataWrapper<T>> items = _pages[num].Items;
				if (num2 < items.Count)
				{
					return items[num2];
				}
				return null;
			}
			finally
			{
				CleanUpPages();
			}
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public object SyncRoot => this;

	public bool IsSynchronized => false;

	public bool IsReadOnly => true;

	public bool IsFixedSize => false;

	public VirtualizingCollection(IVirtualizingCollectionItemsProvider<T> itemsProvider, int pageSize, int pageTimeout)
	{
		_itemsProvider = itemsProvider;
		_pageSize = pageSize;
		_pageTimeout = pageTimeout;
	}

	public VirtualizingCollection(IVirtualizingCollectionItemsProvider<T> itemsProvider, int pageSize)
	{
		_itemsProvider = itemsProvider;
		_pageSize = pageSize;
	}

	public VirtualizingCollection(IVirtualizingCollectionItemsProvider<T> itemsProvider)
	{
		_itemsProvider = itemsProvider;
	}

	public IEnumerator<VirtualizingCollectionDataWrapper<T>> GetEnumerator()
	{
		for (int i = 0; i < Count; i++)
		{
			yield return this[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Add(VirtualizingCollectionDataWrapper<T> item)
	{
		throw new NotSupportedException();
	}

	int IList.Add(object value)
	{
		throw new NotSupportedException();
	}

	bool IList.Contains(object value)
	{
		return Contains((VirtualizingCollectionDataWrapper<T>)value);
	}

	public bool Contains(VirtualizingCollectionDataWrapper<T> item)
	{
		foreach (VirtualizingCollectionDataPage<T> value in _pages.Values)
		{
			if (value.Items.Contains(item))
			{
				return true;
			}
		}
		return false;
	}

	public void Clear()
	{
		throw new NotSupportedException();
	}

	int IList.IndexOf(object value)
	{
		return IndexOf((VirtualizingCollectionDataWrapper<T>)value);
	}

	public int IndexOf(VirtualizingCollectionDataWrapper<T> item)
	{
		foreach (KeyValuePair<int, VirtualizingCollectionDataPage<T>> page in _pages)
		{
			int num = page.Value.Items.IndexOf(item);
			if (num != -1)
			{
				return PageSize * page.Key + num;
			}
		}
		return -1;
	}

	public void Insert(int index, VirtualizingCollectionDataWrapper<T> item)
	{
		throw new NotSupportedException();
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, (VirtualizingCollectionDataWrapper<T>)value);
	}

	public void RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	void IList.Remove(object value)
	{
		throw new NotSupportedException();
	}

	public bool Remove(VirtualizingCollectionDataWrapper<T> item)
	{
		throw new NotSupportedException();
	}

	public void CopyTo(VirtualizingCollectionDataWrapper<T>[] array, int arrayIndex)
	{
		throw new NotSupportedException();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		throw new NotSupportedException();
	}

	public void CleanUpPages()
	{
		if ((DateTime.Now - _lastPageCleanUp).TotalMilliseconds < (double)PageTimeout)
		{
			return;
		}
		int[] array = _pages.Keys.ToArray();
		foreach (int num in array)
		{
			if (num != 0 && (DateTime.Now - _pages[num].TouchTime).TotalMilliseconds > (double)PageTimeout)
			{
				bool flag = true;
				if (_pages.TryGetValue(num, out var value))
				{
					flag = !value.IsInUse;
				}
				if (flag)
				{
					_pages.Remove(num);
				}
			}
		}
		_lastPageCleanUp = DateTime.Now;
	}

	protected virtual void RequestPage(int pageIndex)
	{
		if (!_pages.ContainsKey(pageIndex))
		{
			int pageSize = PageSize;
			VirtualizingCollectionDataPage<T> value = new VirtualizingCollectionDataPage<T>(pageIndex * PageSize, pageSize);
			_pages.Add(pageIndex, value);
			LoadPage(pageIndex, pageSize);
		}
		else
		{
			_pages[pageIndex].TouchTime = DateTime.Now;
		}
	}

	protected virtual void PopulatePage(int pageIndex, IList<T> dataItems)
	{
		if (_pages.TryGetValue(pageIndex, out var value))
		{
			value.Populate(dataItems);
		}
	}

	protected void EmptyCache()
	{
		_pages = new Dictionary<int, VirtualizingCollectionDataPage<T>>();
	}

	protected void TrimIncompletePages(int minIndexToRemove)
	{
		while (_pages.Count > 0)
		{
			int num = _pages.Keys.Max();
			if (num >= minIndexToRemove)
			{
				if (num == 0)
				{
					EmptyCache();
					break;
				}
				if (_pages[num].Items.Count < PageSize)
				{
					_pages.Remove(num);
					continue;
				}
				break;
			}
			break;
		}
	}

	protected virtual void LoadCount()
	{
		Count = FetchCount();
	}

	protected virtual void LoadPage(int pageIndex, int pageLength)
	{
		int count = 0;
		PopulatePage(pageIndex, FetchPage(pageIndex, pageLength, out count));
		Count = count;
	}

	protected IList<T> FetchPage(int pageIndex, int pageLength, out int count)
	{
		return ItemsProvider.FetchRange(pageIndex * PageSize, pageLength, out count);
	}

	protected int FetchCount()
	{
		return ItemsProvider.FetchCount();
	}
}
