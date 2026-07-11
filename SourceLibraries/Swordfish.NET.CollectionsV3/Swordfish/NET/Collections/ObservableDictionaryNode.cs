using System.Collections.Generic;
using Swordfish.NET.Collections.Auxiliary;

namespace Swordfish.NET.Collections;

internal class ObservableDictionaryNode<TKey, TValue>
{
	public KeyValuePair<TKey, TValue> KeyValuePair { get; }

	public TKey Key => KeyValuePair.Key;

	public TValue Value => KeyValuePair.Value;

	internal BigRationalOld SortKey { get; }

	public ObservableDictionaryNode(KeyValuePair<TKey, TValue> pair, BigRationalOld position)
	{
		KeyValuePair = pair;
		SortKey = position;
	}

	public ObservableDictionaryNode(KeyValuePair<TKey, TValue> pair, ObservableDictionaryNode<TKey, TValue> before)
		: this(pair, (before != null) ? (before.SortKey + BigRationalOld.One) : BigRationalOld.Zero)
	{
	}
}
