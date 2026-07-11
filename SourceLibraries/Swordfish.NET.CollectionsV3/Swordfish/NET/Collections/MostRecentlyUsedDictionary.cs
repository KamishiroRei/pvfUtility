using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Swordfish.NET.Collections;

public class MostRecentlyUsedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	protected class DictionaryNode : DoubleLinkListDictionaryNode<TKey, TValue>
	{
		public DictionaryNode(TKey key, TValue value, DoubleLinkListDictionaryNode<TKey, TValue> next)
			: base(key, value, next)
		{
		}
	}

	protected Dictionary<TKey, DictionaryNode> _keyToIndex;

	protected DoubleLinkListDictionaryNode<TKey, TValue> _lastNode;

	protected DoubleLinkListDictionaryNode<TKey, TValue> _firstNode;

	public virtual TValue this[TKey key]
	{
		get
		{
			DictionaryNode dictionaryNode = _keyToIndex[key];
			Touch(dictionaryNode);
			return dictionaryNode.Value;
		}
		set
		{
			if (ContainsKey(key))
			{
				DictionaryNode dictionaryNode = _keyToIndex[key];
				Touch(dictionaryNode);
				dictionaryNode.Value = value;
			}
			else
			{
				Add(key, value);
			}
		}
	}

	public virtual TValue LeastRecentlyUsedValue
	{
		get
		{
			if (_lastNode != null)
			{
				return _lastNode.Value;
			}
			return default(TValue);
		}
	}

	public virtual KeyValuePair<TKey, TValue> LeastRecentlyUsedKeyValuePair
	{
		get
		{
			if (_lastNode != null)
			{
				return new KeyValuePair<TKey, TValue>(_lastNode.Key, _lastNode.Value);
			}
			return default(KeyValuePair<TKey, TValue>);
		}
	}

	public virtual int Count => _keyToIndex.Count;

	public MostRecentlyUsedDictionary()
	{
		_keyToIndex = new Dictionary<TKey, DictionaryNode>();
	}

	public MostRecentlyUsedDictionary(IDictionary<TKey, TValue> source)
		: this()
	{
		foreach (KeyValuePair<TKey, TValue> item in source)
		{
			Add(item.Key, item.Value);
		}
	}

	public MostRecentlyUsedDictionary(IEqualityComparer<TKey> equalityComparer)
		: this()
	{
		_keyToIndex = new Dictionary<TKey, DictionaryNode>(equalityComparer);
	}

	public MostRecentlyUsedDictionary(int capactity)
		: this()
	{
		_keyToIndex = new Dictionary<TKey, DictionaryNode>(capactity);
	}

	public MostRecentlyUsedDictionary(IDictionary<TKey, TValue> source, IEqualityComparer<TKey> equalityComparer)
		: this(equalityComparer)
	{
		foreach (KeyValuePair<TKey, TValue> item in source)
		{
			Add(item.Key, item.Value);
		}
	}

	public MostRecentlyUsedDictionary(int capacity, IEqualityComparer<TKey> equalityComparer)
		: this()
	{
		_keyToIndex = new Dictionary<TKey, DictionaryNode>(capacity, equalityComparer);
	}

	public virtual TValue Add(TKey key, TValue value)
	{
		DictionaryNode dictionaryNode = new DictionaryNode(key, value, _firstNode);
		_keyToIndex.Add(key, dictionaryNode);
		_firstNode = dictionaryNode;
		if (_lastNode == null)
		{
			_lastNode = dictionaryNode;
		}
		return value;
	}

	public virtual bool ContainsKey(TKey key)
	{
		return _keyToIndex.ContainsKey(key);
	}

	public virtual bool Remove(TKey key)
	{
		if (_keyToIndex.TryGetValue(key, out var value))
		{
			RemoveNode(value);
		}
		return _keyToIndex.Remove(key);
	}

	public virtual bool TryGetValue(TKey key, out TValue value)
	{
		if (_keyToIndex.TryGetValue(key, out var value2))
		{
			value = value2.Value;
			Touch(value2);
			return true;
		}
		value = default(TValue);
		return false;
	}

	private void RemoveNode(DoubleLinkListDictionaryNode<TKey, TValue> node)
	{
		if (node == _lastNode)
		{
			_lastNode = node.Previous;
		}
		else
		{
			node.Next.Previous = node.Previous;
		}
		if (node == _firstNode)
		{
			_firstNode = node.Next;
		}
		else
		{
			node.Previous.Next = node.Next;
		}
		node.Next = null;
		node.Previous = null;
	}

	private void Touch(DictionaryNode node)
	{
		if (_firstNode != node)
		{
			RemoveNode(node);
			node.Next = _firstNode;
			if (_firstNode != null)
			{
				_firstNode.Previous = node;
			}
			_firstNode = node;
		}
	}

	public virtual void TrimLeastRecentlyUsed()
	{
		if (_lastNode != null)
		{
			_keyToIndex.Remove(_lastNode.Key);
			RemoveNode(_lastNode);
		}
	}

	public virtual void Clear()
	{
		_keyToIndex.Clear();
		_lastNode = null;
	}

	public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return _keyToIndex.Values.Select((DictionaryNode x) => new KeyValuePair<TKey, TValue>(x.Key, x.Value)).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
