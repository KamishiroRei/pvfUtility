using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
public sealed class Rope<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICloneable
{
	internal struct RopeCacheEntry
	{
		internal readonly RopeNode<T> node;

		internal readonly int nodeStartIndex;

		internal RopeCacheEntry(RopeNode<T> node, int nodeStartOffset)
		{
			this.node = node;
			nodeStartIndex = nodeStartOffset;
		}

		internal bool IsInside(int offset)
		{
			if (offset >= nodeStartIndex)
			{
				return offset < nodeStartIndex + node.length;
			}
			return false;
		}
	}

	internal RopeNode<T> root;

	[NonSerialized]
	private volatile ImmutableStack<RopeCacheEntry> lastUsedNodeStack;

	public int Length => root.length;

	public int Count => root.length;

	public T this[int index]
	{
		get
		{
			if ((uint)index >= (uint)Length)
			{
				throw new ArgumentOutOfRangeException("index", index, "0 <= index < " + Length.ToString(CultureInfo.InvariantCulture));
			}
			RopeCacheEntry ropeCacheEntry = FindNodeUsingCache(index).PeekOrDefault();
			return ropeCacheEntry.node.contents[index - ropeCacheEntry.nodeStartIndex];
		}
		set
		{
			if (index < 0 || index >= Length)
			{
				throw new ArgumentOutOfRangeException("index", index, "0 <= index < " + Length.ToString(CultureInfo.InvariantCulture));
			}
			root = root.SetElement(index, value);
			OnChanged();
		}
	}

	bool ICollection<T>.IsReadOnly => false;

	internal Rope(RopeNode<T> root)
	{
		this.root = root;
	}

	public Rope()
	{
		root = RopeNode<T>.emptyRopeNode;
	}

	public Rope(IEnumerable<T> input)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (input is Rope<T> rope)
		{
			rope.root.Publish();
			root = rope.root;
		}
		else if (input is string text)
		{
			((Rope<char>)(object)this).root = CharRope.InitFromString(text);
		}
		else
		{
			T[] array = ToArray(input);
			root = RopeNode<T>.CreateFromArray(array, 0, array.Length);
		}
	}

	public Rope(T[] array, int arrayIndex, int count)
	{
		VerifyArrayWithRange(array, arrayIndex, count);
		root = RopeNode<T>.CreateFromArray(array, arrayIndex, count);
	}

	public Rope(int length, Func<Rope<T>> initializer)
	{
		if (initializer == null)
		{
			throw new ArgumentNullException("initializer");
		}
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException("length", length, "Length must not be negative");
		}
		if (length == 0)
		{
			root = RopeNode<T>.emptyRopeNode;
		}
		else
		{
			root = new FunctionNode<T>(length, initializer);
		}
	}

	private static T[] ToArray(IEnumerable<T> input)
	{
		return (input as T[]) ?? input.ToArray();
	}

	public Rope<T> Clone()
	{
		root.Publish();
		return new Rope<T>(root);
	}

	object ICloneable.Clone()
	{
		return Clone();
	}

	public void Clear()
	{
		root = RopeNode<T>.emptyRopeNode;
		OnChanged();
	}

	public void InsertRange(int index, Rope<T> newElements)
	{
		if (index < 0 || index > Length)
		{
			throw new ArgumentOutOfRangeException("index", index, "0 <= index <= " + Length.ToString(CultureInfo.InvariantCulture));
		}
		if (newElements == null)
		{
			throw new ArgumentNullException("newElements");
		}
		newElements.root.Publish();
		root = root.Insert(index, newElements.root);
		OnChanged();
	}

	public void InsertRange(int index, IEnumerable<T> newElements)
	{
		if (newElements == null)
		{
			throw new ArgumentNullException("newElements");
		}
		if (newElements is Rope<T> newElements2)
		{
			InsertRange(index, newElements2);
			return;
		}
		T[] array = ToArray(newElements);
		InsertRange(index, array, 0, array.Length);
	}

	public void InsertRange(int index, T[] array, int arrayIndex, int count)
	{
		if (index < 0 || index > Length)
		{
			throw new ArgumentOutOfRangeException("index", index, "0 <= index <= " + Length.ToString(CultureInfo.InvariantCulture));
		}
		VerifyArrayWithRange(array, arrayIndex, count);
		if (count > 0)
		{
			root = root.Insert(index, array, arrayIndex, count);
			OnChanged();
		}
	}

	public void AddRange(IEnumerable<T> newElements)
	{
		InsertRange(Length, newElements);
	}

	public void AddRange(Rope<T> newElements)
	{
		InsertRange(Length, newElements);
	}

	public void AddRange(T[] array, int arrayIndex, int count)
	{
		InsertRange(Length, array, arrayIndex, count);
	}

	public void RemoveRange(int index, int count)
	{
		VerifyRange(index, count);
		if (count > 0)
		{
			root = root.RemoveRange(index, count);
			OnChanged();
		}
	}

	public void SetRange(int index, T[] array, int arrayIndex, int count)
	{
		VerifyRange(index, count);
		VerifyArrayWithRange(array, arrayIndex, count);
		if (count > 0)
		{
			root = root.StoreElements(index, array, arrayIndex, count);
			OnChanged();
		}
	}

	public Rope<T> GetRange(int index, int count)
	{
		VerifyRange(index, count);
		Rope<T> rope = Clone();
		int num = index + count;
		rope.RemoveRange(num, rope.Length - num);
		rope.RemoveRange(0, index);
		return rope;
	}

	public static Rope<T> Concat(Rope<T> left, Rope<T> right)
	{
		if (left == null)
		{
			throw new ArgumentNullException("left");
		}
		if (right == null)
		{
			throw new ArgumentNullException("right");
		}
		left.root.Publish();
		right.root.Publish();
		return new Rope<T>(RopeNode<T>.Concat(left.root, right.root));
	}

	public static Rope<T> Concat(params Rope<T>[] ropes)
	{
		if (ropes == null)
		{
			throw new ArgumentNullException("ropes");
		}
		Rope<T> rope = new Rope<T>();
		foreach (Rope<T> newElements in ropes)
		{
			rope.AddRange(newElements);
		}
		return rope;
	}

	internal void OnChanged()
	{
		lastUsedNodeStack = null;
	}

	internal ImmutableStack<RopeCacheEntry> FindNodeUsingCache(int index)
	{
		ImmutableStack<RopeCacheEntry> immutableStack = lastUsedNodeStack;
		ImmutableStack<RopeCacheEntry> immutableStack2 = immutableStack;
		if (immutableStack == null)
		{
			immutableStack = ImmutableStack<RopeCacheEntry>.Empty.Push(new RopeCacheEntry(root, 0));
		}
		while (!immutableStack.PeekOrDefault().IsInside(index))
		{
			immutableStack = immutableStack.Pop();
		}
		while (true)
		{
			RopeCacheEntry ropeCacheEntry = immutableStack.PeekOrDefault();
			if (ropeCacheEntry.node.height == 0)
			{
				if (ropeCacheEntry.node.contents == null)
				{
					ropeCacheEntry = new RopeCacheEntry(ropeCacheEntry.node.GetContentNode(), ropeCacheEntry.nodeStartIndex);
				}
				if (ropeCacheEntry.node.contents != null)
				{
					break;
				}
			}
			immutableStack = ((index - ropeCacheEntry.nodeStartIndex < ropeCacheEntry.node.left.length) ? immutableStack.Push(new RopeCacheEntry(ropeCacheEntry.node.left, ropeCacheEntry.nodeStartIndex)) : immutableStack.Push(new RopeCacheEntry(ropeCacheEntry.node.right, ropeCacheEntry.nodeStartIndex + ropeCacheEntry.node.left.length)));
		}
		if (immutableStack2 != immutableStack)
		{
			lastUsedNodeStack = immutableStack;
		}
		return immutableStack;
	}

	internal void VerifyRange(int startIndex, int length)
	{
		if (startIndex < 0 || startIndex > Length)
		{
			throw new ArgumentOutOfRangeException("startIndex", startIndex, "0 <= startIndex <= " + Length.ToString(CultureInfo.InvariantCulture));
		}
		if (length < 0 || startIndex + length > Length)
		{
			throw new ArgumentOutOfRangeException("length", length, "0 <= length, startIndex(" + startIndex + ")+length <= " + Length.ToString(CultureInfo.InvariantCulture));
		}
	}

	internal static void VerifyArrayWithRange(T[] array, int arrayIndex, int count)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0 || arrayIndex > array.Length)
		{
			throw new ArgumentOutOfRangeException("startIndex", arrayIndex, "0 <= arrayIndex <= " + array.Length.ToString(CultureInfo.InvariantCulture));
		}
		if (count < 0 || arrayIndex + count > array.Length)
		{
			throw new ArgumentOutOfRangeException("count", count, "0 <= length, arrayIndex(" + arrayIndex + ")+count <= " + array.Length.ToString(CultureInfo.InvariantCulture));
		}
	}

	public override string ToString()
	{
		if (this is Rope<char> rope)
		{
			return rope.ToString(0, Length);
		}
		StringBuilder stringBuilder = new StringBuilder();
		using (IEnumerator<T> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				if (stringBuilder.Length == 0)
				{
					stringBuilder.Append('{');
				}
				else
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(current.ToString());
			}
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	internal string GetTreeAsString()
	{
		return "Not available in release build.";
	}

	public int IndexOf(T item)
	{
		return IndexOf(item, 0, Length);
	}

	public int IndexOf(T item, int startIndex, int count)
	{
		VerifyRange(startIndex, count);
		while (count > 0)
		{
			RopeCacheEntry ropeCacheEntry = FindNodeUsingCache(startIndex).PeekOrDefault();
			T[] contents = ropeCacheEntry.node.contents;
			int num = startIndex - ropeCacheEntry.nodeStartIndex;
			int num2 = Math.Min(ropeCacheEntry.node.length, num + count);
			int num3 = Array.IndexOf(contents, item, num, num2 - num);
			if (num3 >= 0)
			{
				return ropeCacheEntry.nodeStartIndex + num3;
			}
			count -= num2 - num;
			startIndex = ropeCacheEntry.nodeStartIndex + num2;
		}
		return -1;
	}

	public int LastIndexOf(T item)
	{
		return LastIndexOf(item, 0, Length);
	}

	public int LastIndexOf(T item, int startIndex, int count)
	{
		VerifyRange(startIndex, count);
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		for (int num = startIndex + count - 1; num >= startIndex; num--)
		{
			if (equalityComparer.Equals(this[num], item))
			{
				return num;
			}
		}
		return -1;
	}

	public void Insert(int index, T item)
	{
		InsertRange(index, new T[1] { item }, 0, 1);
	}

	public void RemoveAt(int index)
	{
		RemoveRange(index, 1);
	}

	public void Add(T item)
	{
		InsertRange(Length, new T[1] { item }, 0, 1);
	}

	public bool Contains(T item)
	{
		return IndexOf(item) >= 0;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		CopyTo(0, array, arrayIndex, Length);
	}

	public void CopyTo(int index, T[] array, int arrayIndex, int count)
	{
		VerifyRange(index, count);
		VerifyArrayWithRange(array, arrayIndex, count);
		root.CopyTo(index, array, arrayIndex, count);
	}

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num >= 0)
		{
			RemoveAt(num);
			return true;
		}
		return false;
	}

	public IEnumerator<T> GetEnumerator()
	{
		root.Publish();
		return Enumerate(root);
	}

	public T[] ToArray()
	{
		T[] array = new T[Length];
		root.CopyTo(0, array, 0, array.Length);
		return array;
	}

	public T[] ToArray(int startIndex, int count)
	{
		VerifyRange(startIndex, count);
		T[] array = new T[count];
		CopyTo(startIndex, array, 0, count);
		return array;
	}

	private static IEnumerator<T> Enumerate(RopeNode<T> node)
	{
		Stack<RopeNode<T>> stack = new Stack<RopeNode<T>>();
		while (node != null)
		{
			while (node.contents == null)
			{
				if (node.height == 0)
				{
					node = node.GetContentNode();
					continue;
				}
				stack.Push(node.right);
				node = node.left;
			}
			for (int i = 0; i < node.length; i++)
			{
				yield return node.contents[i];
			}
			node = ((stack.Count <= 0) ? null : stack.Pop());
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
