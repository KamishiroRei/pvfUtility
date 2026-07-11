using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.AvalonEdit.Utils;

public sealed class CompressingTreeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private sealed class Node
	{
		internal Node left;

		internal Node right;

		internal Node parent;

		internal bool color;

		internal int count;

		internal int totalCount;

		internal T value;

		internal Node LeftMost
		{
			get
			{
				Node node = this;
				while (node.left != null)
				{
					node = node.left;
				}
				return node;
			}
		}

		internal Node RightMost
		{
			get
			{
				Node node = this;
				while (node.right != null)
				{
					node = node.right;
				}
				return node;
			}
		}

		internal Node Predecessor
		{
			get
			{
				if (left != null)
				{
					return left.RightMost;
				}
				Node node = this;
				Node node2;
				do
				{
					node2 = node;
					node = node.parent;
				}
				while (node != null && node.left == node2);
				return node;
			}
		}

		internal Node Successor
		{
			get
			{
				if (right != null)
				{
					return right.LeftMost;
				}
				Node node = this;
				Node node2;
				do
				{
					node2 = node;
					node = node.parent;
				}
				while (node != null && node.right == node2);
				return node;
			}
		}

		public Node(T value, int count)
		{
			this.value = value;
			this.count = count;
			totalCount = count;
		}

		public override string ToString()
		{
			string[] obj = new string[7]
			{
				"[TotalCount=",
				totalCount.ToString(),
				" Count=",
				count.ToString(),
				" Value=",
				null,
				null
			};
			T val = value;
			obj[5] = val?.ToString();
			obj[6] = "]";
			return string.Concat(obj);
		}
	}

	private readonly Func<T, T, bool> comparisonFunc;

	private Node root;

	internal const bool RED = true;

	internal const bool BLACK = false;

	public T this[int index]
	{
		get
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + (Count - 1));
			}
			return GetNode(ref index).value;
		}
		set
		{
			RemoveAt(index);
			Insert(index, value);
		}
	}

	public int Count
	{
		get
		{
			if (root != null)
			{
				return root.totalCount;
			}
			return 0;
		}
	}

	bool ICollection<T>.IsReadOnly => false;

	public CompressingTreeList(IEqualityComparer<T> equalityComparer)
	{
		if (equalityComparer == null)
		{
			throw new ArgumentNullException("equalityComparer");
		}
		comparisonFunc = equalityComparer.Equals;
	}

	public CompressingTreeList(Func<T, T, bool> comparisonFunc)
	{
		if (comparisonFunc == null)
		{
			throw new ArgumentNullException("comparisonFunc");
		}
		this.comparisonFunc = comparisonFunc;
	}

	public void InsertRange(int index, int count, T item)
	{
		if (index < 0 || index > Count)
		{
			throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + Count);
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", count, "Value must not be negative");
		}
		if (count == 0)
		{
			return;
		}
		if (Count + count < 0)
		{
			throw new OverflowException("Cannot insert elements: total number of elements must not exceed int.MaxValue.");
		}
		if (root == null)
		{
			root = new Node(item, count);
			return;
		}
		Node node = GetNode(ref index);
		if (comparisonFunc(node.value, item))
		{
			node.count += count;
			UpdateAugmentedData(node);
		}
		else if (index == node.count)
		{
			InsertAsRight(node, new Node(item, count));
		}
		else if (index == 0)
		{
			Node predecessor = node.Predecessor;
			if (predecessor != null && comparisonFunc(predecessor.value, item))
			{
				predecessor.count += count;
				UpdateAugmentedData(predecessor);
			}
			else
			{
				InsertBefore(node, new Node(item, count));
			}
		}
		else
		{
			node.count -= index;
			InsertBefore(node, new Node(node.value, index));
			InsertBefore(node, new Node(item, count));
			UpdateAugmentedData(node);
		}
	}

	private void InsertBefore(Node node, Node newNode)
	{
		if (node.left == null)
		{
			InsertAsLeft(node, newNode);
		}
		else
		{
			InsertAsRight(node.left.RightMost, newNode);
		}
	}

	public void RemoveRange(int index, int count)
	{
		if (index < 0 || index > Count)
		{
			throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + Count);
		}
		if (count < 0 || index + count > Count)
		{
			throw new ArgumentOutOfRangeException("count", count, "0 <= length, index(" + index + ")+count <= " + Count);
		}
		if (count == 0)
		{
			return;
		}
		Node node = GetNode(ref index);
		if (index + count < node.count)
		{
			node.count -= count;
			UpdateAugmentedData(node);
			return;
		}
		Node node2;
		if (index > 0)
		{
			count -= node.count - index;
			node.count = index;
			UpdateAugmentedData(node);
			node2 = node;
			node = node.Successor;
		}
		else
		{
			node2 = node.Predecessor;
		}
		while (node != null && count >= node.count)
		{
			count -= node.count;
			Node successor = node.Successor;
			RemoveNode(node);
			node = successor;
		}
		if (count > 0)
		{
			node.count -= count;
			UpdateAugmentedData(node);
		}
		if (node != null && node2 != null && comparisonFunc(node2.value, node.value))
		{
			node2.count += node.count;
			RemoveNode(node);
			UpdateAugmentedData(node2);
		}
	}

	public void SetRange(int index, int count, T item)
	{
		RemoveRange(index, count);
		InsertRange(index, count, item);
	}

	private Node GetNode(ref int index)
	{
		Node node = root;
		while (true)
		{
			if (node.left != null && index < node.left.totalCount)
			{
				node = node.left;
				continue;
			}
			if (node.left != null)
			{
				index -= node.left.totalCount;
			}
			if (index < node.count || node.right == null)
			{
				break;
			}
			index -= node.count;
			node = node.right;
		}
		return node;
	}

	private void UpdateAugmentedData(Node node)
	{
		int num = node.count;
		if (node.left != null)
		{
			num += node.left.totalCount;
		}
		if (node.right != null)
		{
			num += node.right.totalCount;
		}
		if (node.totalCount != num)
		{
			node.totalCount = num;
			if (node.parent != null)
			{
				UpdateAugmentedData(node.parent);
			}
		}
	}

	public int IndexOf(T item)
	{
		int num = 0;
		if (root != null)
		{
			for (Node node = root.LeftMost; node != null; node = node.Successor)
			{
				if (comparisonFunc(node.value, item))
				{
					return num;
				}
				num += node.count;
			}
		}
		return -1;
	}

	public int GetStartOfRun(int index)
	{
		if (index < 0 || index >= Count)
		{
			throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + (Count - 1));
		}
		int index2 = index;
		GetNode(ref index2);
		return index - index2;
	}

	public int GetEndOfRun(int index)
	{
		if (index < 0 || index >= Count)
		{
			throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + (Count - 1));
		}
		int index2 = index;
		int count = GetNode(ref index2).count;
		return index - index2 + count;
	}

	public void Transform(Func<T, T> converter)
	{
		if (root == null)
		{
			return;
		}
		Node node = null;
		for (Node node2 = root.LeftMost; node2 != null; node2 = node2.Successor)
		{
			node2.value = converter(node2.value);
			if (node != null && comparisonFunc(node.value, node2.value))
			{
				node2.count += node.count;
				UpdateAugmentedData(node2);
				RemoveNode(node);
			}
			node = node2;
		}
	}

	public void TransformRange(int index, int length, Func<T, T> converter)
	{
		if (root != null)
		{
			int num = index + length;
			int num2 = index;
			while (num2 < num)
			{
				int num3 = Math.Min(num, GetEndOfRun(num2));
				T arg = this[num2];
				T item = converter(arg);
				SetRange(num2, num3 - num2, item);
				num2 = num3;
			}
		}
	}

	public void Insert(int index, T item)
	{
		InsertRange(index, 1, item);
	}

	public void RemoveAt(int index)
	{
		RemoveRange(index, 1);
	}

	public void Add(T item)
	{
		InsertRange(Count, 1, item);
	}

	public void Clear()
	{
		root = null;
	}

	public bool Contains(T item)
	{
		return IndexOf(item) >= 0;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (array.Length < Count)
		{
			throw new ArgumentException("The array is too small", "array");
		}
		if (arrayIndex < 0 || arrayIndex + Count > array.Length)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Value must be between 0 and " + (array.Length - Count));
		}
		using IEnumerator<T> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			array[arrayIndex++] = current;
		}
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
		if (root == null)
		{
			yield break;
		}
		for (Node n = root.LeftMost; n != null; n = n.Successor)
		{
			for (int i = 0; i < n.count; i++)
			{
				yield return n.value;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private void InsertAsLeft(Node parentNode, Node newNode)
	{
		parentNode.left = newNode;
		newNode.parent = parentNode;
		newNode.color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void InsertAsRight(Node parentNode, Node newNode)
	{
		parentNode.right = newNode;
		newNode.parent = parentNode;
		newNode.color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void FixTreeOnInsert(Node node)
	{
		Node parent = node.parent;
		if (parent == null)
		{
			node.color = false;
		}
		else
		{
			if (!parent.color)
			{
				return;
			}
			Node parent2 = parent.parent;
			Node node2 = Sibling(parent);
			if (node2 != null && node2.color)
			{
				parent.color = false;
				node2.color = false;
				parent2.color = true;
				FixTreeOnInsert(parent2);
				return;
			}
			if (node == parent.right && parent == parent2.left)
			{
				RotateLeft(parent);
				node = node.left;
			}
			else if (node == parent.left && parent == parent2.right)
			{
				RotateRight(parent);
				node = node.right;
			}
			parent = node.parent;
			parent2 = parent.parent;
			parent.color = false;
			parent2.color = true;
			if (node == parent.left && parent == parent2.left)
			{
				RotateRight(parent2);
			}
			else
			{
				RotateLeft(parent2);
			}
		}
	}

	private void RemoveNode(Node removedNode)
	{
		if (removedNode.left != null && removedNode.right != null)
		{
			Node leftMost = removedNode.right.LeftMost;
			RemoveNode(leftMost);
			ReplaceNode(removedNode, leftMost);
			leftMost.left = removedNode.left;
			if (leftMost.left != null)
			{
				leftMost.left.parent = leftMost;
			}
			leftMost.right = removedNode.right;
			if (leftMost.right != null)
			{
				leftMost.right.parent = leftMost;
			}
			leftMost.color = removedNode.color;
			UpdateAugmentedData(leftMost);
			if (leftMost.parent != null)
			{
				UpdateAugmentedData(leftMost.parent);
			}
			return;
		}
		Node parent = removedNode.parent;
		Node node = removedNode.left ?? removedNode.right;
		ReplaceNode(removedNode, node);
		if (parent != null)
		{
			UpdateAugmentedData(parent);
		}
		if (!removedNode.color)
		{
			if (node != null && node.color)
			{
				node.color = false;
			}
			else
			{
				FixTreeOnDelete(node, parent);
			}
		}
	}

	private void FixTreeOnDelete(Node node, Node parentNode)
	{
		if (parentNode == null)
		{
			return;
		}
		Node node2 = Sibling(node, parentNode);
		if (node2.color)
		{
			parentNode.color = true;
			node2.color = false;
			if (node == parentNode.left)
			{
				RotateLeft(parentNode);
			}
			else
			{
				RotateRight(parentNode);
			}
			node2 = Sibling(node, parentNode);
		}
		if (!parentNode.color && !node2.color && !GetColor(node2.left) && !GetColor(node2.right))
		{
			node2.color = true;
			FixTreeOnDelete(parentNode, parentNode.parent);
			return;
		}
		if (parentNode.color && !node2.color && !GetColor(node2.left) && !GetColor(node2.right))
		{
			node2.color = true;
			parentNode.color = false;
			return;
		}
		if (node == parentNode.left && !node2.color && GetColor(node2.left) && !GetColor(node2.right))
		{
			node2.color = true;
			node2.left.color = false;
			RotateRight(node2);
		}
		else if (node == parentNode.right && !node2.color && GetColor(node2.right) && !GetColor(node2.left))
		{
			node2.color = true;
			node2.right.color = false;
			RotateLeft(node2);
		}
		node2 = Sibling(node, parentNode);
		node2.color = parentNode.color;
		parentNode.color = false;
		if (node == parentNode.left)
		{
			if (node2.right != null)
			{
				node2.right.color = false;
			}
			RotateLeft(parentNode);
		}
		else
		{
			if (node2.left != null)
			{
				node2.left.color = false;
			}
			RotateRight(parentNode);
		}
	}

	private void ReplaceNode(Node replacedNode, Node newNode)
	{
		if (replacedNode.parent == null)
		{
			root = newNode;
		}
		else if (replacedNode.parent.left == replacedNode)
		{
			replacedNode.parent.left = newNode;
		}
		else
		{
			replacedNode.parent.right = newNode;
		}
		if (newNode != null)
		{
			newNode.parent = replacedNode.parent;
		}
		replacedNode.parent = null;
	}

	private void RotateLeft(Node p)
	{
		Node right = p.right;
		ReplaceNode(p, right);
		p.right = right.left;
		if (p.right != null)
		{
			p.right.parent = p;
		}
		right.left = p;
		p.parent = right;
		UpdateAugmentedData(p);
		UpdateAugmentedData(right);
	}

	private void RotateRight(Node p)
	{
		Node left = p.left;
		ReplaceNode(p, left);
		p.left = left.right;
		if (p.left != null)
		{
			p.left.parent = p;
		}
		left.right = p;
		p.parent = left;
		UpdateAugmentedData(p);
		UpdateAugmentedData(left);
	}

	private static Node Sibling(Node node)
	{
		if (node == node.parent.left)
		{
			return node.parent.right;
		}
		return node.parent.left;
	}

	private static Node Sibling(Node node, Node parentNode)
	{
		if (node == parentNode.left)
		{
			return parentNode.right;
		}
		return parentNode.left;
	}

	private static bool GetColor(Node node)
	{
		return node?.color ?? false;
	}

	[Conditional("DATACONSISTENCYTEST")]
	internal void CheckProperties()
	{
	}

	internal string GetTreeAsString()
	{
		return "Not available in release build.";
	}
}
