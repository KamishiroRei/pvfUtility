using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Swordfish.NET.Collections;

public class ObservableSortedCollection<TKey> : ObservableCollection<TKey>
{
	private BinarySorter<TKey> _sorter;

	public ObservableSortedCollection(IComparer<TKey> comparer = null)
	{
		_sorter = new BinarySorter<TKey>(comparer);
	}

	protected override void MoveItem(int oldIndex, int newIndex)
	{
	}

	protected override void InsertItem(int index, TKey item)
	{
		int insertIndex = _sorter.GetInsertIndex(base.Count, item, (int mid) => base[mid]);
		base.InsertItem(insertIndex, item);
	}

	protected override void SetItem(int index, TKey item)
	{
		RemoveAt(index);
		InsertItem(index, item);
	}
}
