using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.AvalonEdit.Utils;

internal sealed class ObserveAddRemoveCollection<T> : Collection<T>
{
	private readonly Action<T> onAdd;

	private readonly Action<T> onRemove;

	public ObserveAddRemoveCollection(Action<T> onAdd, Action<T> onRemove)
	{
		if (onAdd == null)
		{
			throw new ArgumentNullException("onAdd");
		}
		if (onRemove == null)
		{
			throw new ArgumentNullException("onRemove");
		}
		this.onAdd = onAdd;
		this.onRemove = onRemove;
	}

	protected override void ClearItems()
	{
		if (onRemove != null)
		{
			using IEnumerator<T> enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				onRemove(current);
			}
		}
		base.ClearItems();
	}

	protected override void InsertItem(int index, T item)
	{
		if (onAdd != null)
		{
			onAdd(item);
		}
		base.InsertItem(index, item);
	}

	protected override void RemoveItem(int index)
	{
		if (onRemove != null)
		{
			onRemove(base[index]);
		}
		base.RemoveItem(index);
	}

	protected override void SetItem(int index, T item)
	{
		if (onRemove != null)
		{
			onRemove(base[index]);
		}
		try
		{
			if (onAdd != null)
			{
				onAdd(item);
			}
		}
		catch
		{
			RemoveAt(index);
			throw;
		}
		base.SetItem(index, item);
	}
}
