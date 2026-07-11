using System;
using System.Collections.Generic;

namespace Swordfish.NET.Collections;

public interface IVirtualizingCollectionItemsProvider<T>
{
	event EventHandler CountChanged;

	int FetchCount();

	IList<T> FetchRange(int startIndex, int pageCount, out int overallCount);
}
