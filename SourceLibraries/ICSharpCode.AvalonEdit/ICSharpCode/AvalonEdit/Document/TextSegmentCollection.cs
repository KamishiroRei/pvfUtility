using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document;

public sealed class TextSegmentCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ISegmentTree, IWeakEventListener where T : TextSegment
{
	private int count;

	private TextSegment root;

	private bool isConnectedToDocument;

	internal const bool RED = true;

	internal const bool BLACK = false;

	public T FirstSegment
	{
		get
		{
			if (root != null)
			{
				return (T)root.LeftMost;
			}
			return null;
		}
	}

	public T LastSegment
	{
		get
		{
			if (root != null)
			{
				return (T)root.RightMost;
			}
			return null;
		}
	}

	public int Count => count;

	bool ICollection<T>.IsReadOnly => false;

	public TextSegmentCollection()
	{
	}

	public TextSegmentCollection(TextDocument textDocument)
	{
		if (textDocument == null)
		{
			throw new ArgumentNullException("textDocument");
		}
		textDocument.VerifyAccess();
		isConnectedToDocument = true;
		WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument>.AddListener(textDocument, this);
	}

	bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(TextDocumentWeakEventManager.Changed))
		{
			OnDocumentChanged((DocumentChangeEventArgs)e);
			return true;
		}
		return false;
	}

	public void UpdateOffsets(DocumentChangeEventArgs e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		if (isConnectedToDocument)
		{
			throw new InvalidOperationException("This TextSegmentCollection will automatically update offsets; do not call UpdateOffsets manually!");
		}
		OnDocumentChanged(e);
	}

	private void OnDocumentChanged(DocumentChangeEventArgs e)
	{
		OffsetChangeMap offsetChangeMapOrNull = e.OffsetChangeMapOrNull;
		if (offsetChangeMapOrNull != null)
		{
			foreach (OffsetChangeMapEntry item in offsetChangeMapOrNull)
			{
				UpdateOffsetsInternal(item);
			}
			return;
		}
		UpdateOffsetsInternal(e.CreateSingleChangeMapEntry());
	}

	public void UpdateOffsets(OffsetChangeMapEntry change)
	{
		if (isConnectedToDocument)
		{
			throw new InvalidOperationException("This TextSegmentCollection will automatically update offsets; do not call UpdateOffsets manually!");
		}
		UpdateOffsetsInternal(change);
	}

	private void UpdateOffsetsInternal(OffsetChangeMapEntry change)
	{
		if (change.RemovalLength == 0)
		{
			InsertText(change.Offset, change.InsertionLength);
		}
		else
		{
			ReplaceText(change);
		}
	}

	private void InsertText(int offset, int length)
	{
		if (length == 0)
		{
			return;
		}
		foreach (T item in FindSegmentsContaining(offset))
		{
			if (item.StartOffset < offset && offset < item.EndOffset)
			{
				item.Length += length;
			}
		}
		TextSegment textSegment = FindFirstSegmentWithStartAfter(offset);
		if (textSegment != null)
		{
			textSegment.nodeLength += length;
			UpdateAugmentedData(textSegment);
		}
	}

	private void ReplaceText(OffsetChangeMapEntry change)
	{
		int offset = change.Offset;
		foreach (T item in FindOverlappingSegments(offset, change.RemovalLength))
		{
			if (item.StartOffset <= offset)
			{
				if (item.EndOffset >= offset + change.RemovalLength)
				{
					item.Length += change.InsertionLength - change.RemovalLength;
				}
				else
				{
					item.Length = offset - item.StartOffset;
				}
			}
			else
			{
				int val = item.EndOffset - (offset + change.RemovalLength);
				RemoveSegment(item);
				item.StartOffset = offset + change.RemovalLength;
				item.Length = Math.Max(0, val);
				AddSegment(item);
			}
		}
		TextSegment textSegment = FindFirstSegmentWithStartAfter(offset + 1);
		if (textSegment != null)
		{
			textSegment.nodeLength += change.InsertionLength - change.RemovalLength;
			UpdateAugmentedData(textSegment);
		}
	}

	public void Add(T item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (item.ownerTree != null)
		{
			throw new ArgumentException("The segment is already added to a SegmentCollection.");
		}
		AddSegment(item);
	}

	void ISegmentTree.Add(TextSegment s)
	{
		AddSegment(s);
	}

	private void AddSegment(TextSegment node)
	{
		int offset = node.StartOffset;
		node.distanceToMaxEnd = node.segmentLength;
		if (root == null)
		{
			root = node;
			node.totalNodeLength = node.nodeLength;
		}
		else if (offset >= root.totalNodeLength)
		{
			node.nodeLength = (node.totalNodeLength = offset - root.totalNodeLength);
			InsertAsRight(root.RightMost, node);
		}
		else
		{
			TextSegment textSegment = FindNode(ref offset);
			node.totalNodeLength = (node.nodeLength = offset);
			textSegment.nodeLength -= offset;
			InsertBefore(textSegment, node);
		}
		node.ownerTree = this;
		count++;
	}

	private void InsertBefore(TextSegment node, TextSegment newNode)
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

	public T GetNextSegment(T segment)
	{
		if (!Contains(segment))
		{
			throw new ArgumentException("segment is not inside the segment tree");
		}
		return (T)segment.Successor;
	}

	public T GetPreviousSegment(T segment)
	{
		if (!Contains(segment))
		{
			throw new ArgumentException("segment is not inside the segment tree");
		}
		return (T)segment.Predecessor;
	}

	public T FindFirstSegmentWithStartAfter(int startOffset)
	{
		if (root == null)
		{
			return null;
		}
		if (startOffset <= 0)
		{
			return (T)root.LeftMost;
		}
		TextSegment textSegment = FindNode(ref startOffset);
		while (startOffset == 0)
		{
			TextSegment textSegment2 = ((textSegment == null) ? root.RightMost : textSegment.Predecessor);
			startOffset += textSegment2.nodeLength;
			textSegment = textSegment2;
		}
		return (T)textSegment;
	}

	private TextSegment FindNode(ref int offset)
	{
		TextSegment textSegment = root;
		while (true)
		{
			if (textSegment.left != null)
			{
				if (offset < textSegment.left.totalNodeLength)
				{
					textSegment = textSegment.left;
					continue;
				}
				offset -= textSegment.left.totalNodeLength;
			}
			if (offset < textSegment.nodeLength)
			{
				return textSegment;
			}
			offset -= textSegment.nodeLength;
			if (textSegment.right == null)
			{
				break;
			}
			textSegment = textSegment.right;
		}
		return null;
	}

	public ReadOnlyCollection<T> FindSegmentsContaining(int offset)
	{
		return FindOverlappingSegments(offset, 0);
	}

	public ReadOnlyCollection<T> FindOverlappingSegments(ISegment segment)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		return FindOverlappingSegments(segment.Offset, segment.Length);
	}

	public ReadOnlyCollection<T> FindOverlappingSegments(int offset, int length)
	{
		ThrowUtil.CheckNotNegative(length, "length");
		List<T> list = new List<T>();
		if (root != null)
		{
			FindOverlappingSegments(list, root, offset, offset + length);
		}
		return list.AsReadOnly();
	}

	private void FindOverlappingSegments(List<T> results, TextSegment node, int low, int high)
	{
		if (high < 0)
		{
			return;
		}
		int num = low - node.nodeLength;
		int num2 = high - node.nodeLength;
		if (node.left != null)
		{
			num -= node.left.totalNodeLength;
			num2 -= node.left.totalNodeLength;
		}
		if (node.distanceToMaxEnd < num)
		{
			return;
		}
		if (node.left != null)
		{
			FindOverlappingSegments(results, node.left, low, high);
		}
		if (num2 >= 0)
		{
			if (num <= node.segmentLength)
			{
				results.Add((T)node);
			}
			if (node.right != null)
			{
				FindOverlappingSegments(results, node.right, num, num2);
			}
		}
	}

	private void UpdateAugmentedData(TextSegment node)
	{
		int num = node.nodeLength;
		int num2 = node.segmentLength;
		if (node.left != null)
		{
			num += node.left.totalNodeLength;
			int num3 = node.left.distanceToMaxEnd;
			if (node.left.right != null)
			{
				num3 -= node.left.right.totalNodeLength;
			}
			num3 -= node.nodeLength;
			if (num3 > num2)
			{
				num2 = num3;
			}
		}
		if (node.right != null)
		{
			num += node.right.totalNodeLength;
			int distanceToMaxEnd = node.right.distanceToMaxEnd;
			distanceToMaxEnd += node.right.nodeLength;
			if (node.right.left != null)
			{
				distanceToMaxEnd += node.right.left.totalNodeLength;
			}
			if (distanceToMaxEnd > num2)
			{
				num2 = distanceToMaxEnd;
			}
		}
		if (node.totalNodeLength != num || node.distanceToMaxEnd != num2)
		{
			node.totalNodeLength = num;
			node.distanceToMaxEnd = num2;
			if (node.parent != null)
			{
				UpdateAugmentedData(node.parent);
			}
		}
	}

	void ISegmentTree.UpdateAugmentedData(TextSegment node)
	{
		UpdateAugmentedData(node);
	}

	public bool Remove(T item)
	{
		if (!Contains(item))
		{
			return false;
		}
		RemoveSegment(item);
		return true;
	}

	void ISegmentTree.Remove(TextSegment s)
	{
		RemoveSegment(s);
	}

	private void RemoveSegment(TextSegment s)
	{
		int startOffset = s.StartOffset;
		TextSegment successor = s.Successor;
		if (successor != null)
		{
			successor.nodeLength += s.nodeLength;
		}
		RemoveNode(s);
		if (successor != null)
		{
			UpdateAugmentedData(successor);
		}
		Disconnect(s, startOffset);
	}

	private void Disconnect(TextSegment s, int offset)
	{
		s.left = (s.right = (s.parent = null));
		s.ownerTree = null;
		s.nodeLength = offset;
		count--;
	}

	public void Clear()
	{
		T[] array = this.ToArray();
		root = null;
		int num = 0;
		T[] array2 = array;
		foreach (TextSegment textSegment in array2)
		{
			num += textSegment.nodeLength;
			Disconnect(textSegment, num);
		}
	}

	[Conditional("DATACONSISTENCYTEST")]
	internal void CheckProperties()
	{
	}

	internal string GetTreeAsString()
	{
		return "Not available in release build.";
	}

	private void InsertAsLeft(TextSegment parentNode, TextSegment newNode)
	{
		parentNode.left = newNode;
		newNode.parent = parentNode;
		newNode.color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void InsertAsRight(TextSegment parentNode, TextSegment newNode)
	{
		parentNode.right = newNode;
		newNode.parent = parentNode;
		newNode.color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void FixTreeOnInsert(TextSegment node)
	{
		TextSegment parent = node.parent;
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
			TextSegment parent2 = parent.parent;
			TextSegment textSegment = Sibling(parent);
			if (textSegment != null && textSegment.color)
			{
				parent.color = false;
				textSegment.color = false;
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

	private void RemoveNode(TextSegment removedNode)
	{
		if (removedNode.left != null && removedNode.right != null)
		{
			TextSegment leftMost = removedNode.right.LeftMost;
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
		TextSegment parent = removedNode.parent;
		TextSegment textSegment = removedNode.left ?? removedNode.right;
		ReplaceNode(removedNode, textSegment);
		if (parent != null)
		{
			UpdateAugmentedData(parent);
		}
		if (!removedNode.color)
		{
			if (textSegment != null && textSegment.color)
			{
				textSegment.color = false;
			}
			else
			{
				FixTreeOnDelete(textSegment, parent);
			}
		}
	}

	private void FixTreeOnDelete(TextSegment node, TextSegment parentNode)
	{
		if (parentNode == null)
		{
			return;
		}
		TextSegment textSegment = Sibling(node, parentNode);
		if (textSegment.color)
		{
			parentNode.color = true;
			textSegment.color = false;
			if (node == parentNode.left)
			{
				RotateLeft(parentNode);
			}
			else
			{
				RotateRight(parentNode);
			}
			textSegment = Sibling(node, parentNode);
		}
		if (!parentNode.color && !textSegment.color && !GetColor(textSegment.left) && !GetColor(textSegment.right))
		{
			textSegment.color = true;
			FixTreeOnDelete(parentNode, parentNode.parent);
			return;
		}
		if (parentNode.color && !textSegment.color && !GetColor(textSegment.left) && !GetColor(textSegment.right))
		{
			textSegment.color = true;
			parentNode.color = false;
			return;
		}
		if (node == parentNode.left && !textSegment.color && GetColor(textSegment.left) && !GetColor(textSegment.right))
		{
			textSegment.color = true;
			textSegment.left.color = false;
			RotateRight(textSegment);
		}
		else if (node == parentNode.right && !textSegment.color && GetColor(textSegment.right) && !GetColor(textSegment.left))
		{
			textSegment.color = true;
			textSegment.right.color = false;
			RotateLeft(textSegment);
		}
		textSegment = Sibling(node, parentNode);
		textSegment.color = parentNode.color;
		parentNode.color = false;
		if (node == parentNode.left)
		{
			if (textSegment.right != null)
			{
				textSegment.right.color = false;
			}
			RotateLeft(parentNode);
		}
		else
		{
			if (textSegment.left != null)
			{
				textSegment.left.color = false;
			}
			RotateRight(parentNode);
		}
	}

	private void ReplaceNode(TextSegment replacedNode, TextSegment newNode)
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

	private void RotateLeft(TextSegment p)
	{
		TextSegment right = p.right;
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

	private void RotateRight(TextSegment p)
	{
		TextSegment left = p.left;
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

	private static TextSegment Sibling(TextSegment node)
	{
		if (node == node.parent.left)
		{
			return node.parent.right;
		}
		return node.parent.left;
	}

	private static TextSegment Sibling(TextSegment node, TextSegment parentNode)
	{
		if (node == parentNode.left)
		{
			return parentNode.right;
		}
		return parentNode.left;
	}

	private static bool GetColor(TextSegment node)
	{
		return node?.color ?? false;
	}

	public bool Contains(T item)
	{
		if (item != null)
		{
			return item.ownerTree == this;
		}
		return false;
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
		if (arrayIndex < 0 || arrayIndex + count > array.Length)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Value must be between 0 and " + (array.Length - count));
		}
		using IEnumerator<T> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			array[arrayIndex++] = current;
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (root != null)
		{
			for (TextSegment current = root.LeftMost; current != null; current = current.Successor)
			{
				yield return (T)current;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
