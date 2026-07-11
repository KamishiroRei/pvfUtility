namespace Swordfish.NET.Collections;

public class DoubleLinkListDictionaryNode<TKey, TValue>
{
	public DoubleLinkListDictionaryNode<TKey, TValue> Next;

	public DoubleLinkListDictionaryNode<TKey, TValue> Previous;

	public TKey Key;

	public TValue Value;

	public DoubleLinkListDictionaryNode(TKey key, TValue value, DoubleLinkListDictionaryNode<TKey, TValue> next)
	{
		Next = next;
		Key = key;
		Value = value;
		if (Next != null)
		{
			Next.Previous = this;
		}
	}

	public override string ToString()
	{
		return $"{Key}, {Value}";
	}
}
