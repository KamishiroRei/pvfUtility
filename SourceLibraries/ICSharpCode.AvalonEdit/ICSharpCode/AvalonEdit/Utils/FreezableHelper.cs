using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ICSharpCode.AvalonEdit.Utils;

internal static class FreezableHelper
{
	public static void ThrowIfFrozen(IFreezable freezable)
	{
		if (freezable.IsFrozen)
		{
			throw new InvalidOperationException("Cannot mutate frozen " + freezable.GetType().Name);
		}
	}

	public static IList<T> FreezeListAndElements<T>(IList<T> list)
	{
		if (list != null)
		{
			foreach (T item in list)
			{
				Freeze(item);
			}
		}
		return FreezeList(list);
	}

	public static IList<T> FreezeList<T>(IList<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return Empty<T>.Array;
		}
		if (list.IsReadOnly)
		{
			return list;
		}
		return new ReadOnlyCollection<T>(list.ToArray());
	}

	public static void Freeze(object item)
	{
		if (item is IFreezable freezable)
		{
			freezable.Freeze();
		}
	}

	public static T FreezeAndReturn<T>(T item) where T : IFreezable
	{
		item.Freeze();
		return item;
	}

	public static T GetFrozenClone<T>(T item) where T : IFreezable, ICloneable
	{
		if (!item.IsFrozen)
		{
			item = (T)item.Clone();
			item.Freeze();
		}
		return item;
	}
}
