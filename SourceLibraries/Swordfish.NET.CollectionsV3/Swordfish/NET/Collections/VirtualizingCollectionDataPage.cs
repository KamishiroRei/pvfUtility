using System;
using System.Collections.Generic;
using System.Linq;

namespace Swordfish.NET.Collections;

public class VirtualizingCollectionDataPage<T> where T : class
{
	public IList<VirtualizingCollectionDataWrapper<T>> Items { get; set; }

	public DateTime TouchTime { get; set; }

	public bool IsInUse => Items.Any((VirtualizingCollectionDataWrapper<T> wrapper) => wrapper.IsInUse);

	public VirtualizingCollectionDataPage(int firstIndex, int pageLength)
	{
		Items = new List<VirtualizingCollectionDataWrapper<T>>(pageLength);
		for (int i = 0; i < pageLength; i++)
		{
			Items.Add(new VirtualizingCollectionDataWrapper<T>(firstIndex + i));
		}
		TouchTime = DateTime.Now;
	}

	public void Populate(IList<T> newItems)
	{
		int num = 0;
		int i;
		for (i = 0; i < newItems.Count && i < Items.Count; i++)
		{
			Items[i].Data = newItems[i];
			num = Items[i].Index;
		}
		for (; i < newItems.Count; i++)
		{
			num++;
			Items.Add(new VirtualizingCollectionDataWrapper<T>(num)
			{
				Data = newItems[i]
			});
		}
		while (i < Items.Count)
		{
			Items.RemoveAt(Items.Count - 1);
		}
	}
}
