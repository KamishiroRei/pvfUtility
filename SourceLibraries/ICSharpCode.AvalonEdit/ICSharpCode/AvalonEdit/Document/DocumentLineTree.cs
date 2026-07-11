using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Document;

internal sealed class DocumentLineTree : IList<DocumentLine>, ICollection<DocumentLine>, IEnumerable<DocumentLine>, IEnumerable
{
	private readonly TextDocument document;

	private DocumentLine root;

	internal const bool RED = true;

	internal const bool BLACK = false;

	public int LineCount => root.nodeTotalCount;

	DocumentLine IList<DocumentLine>.this[int index]
	{
		get
		{
			document.VerifyAccess();
			return GetByNumber(1 + index);
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	int ICollection<DocumentLine>.Count
	{
		get
		{
			document.VerifyAccess();
			return LineCount;
		}
	}

	bool ICollection<DocumentLine>.IsReadOnly => true;

	public DocumentLineTree(TextDocument document)
	{
		this.document = document;
		DocumentLine documentLine = new DocumentLine(document);
		root = documentLine.InitLineNode();
	}

	internal static void UpdateAfterChildrenChange(DocumentLine node)
	{
		int num = 1;
		int num2 = node.TotalLength;
		if (node.left != null)
		{
			num += node.left.nodeTotalCount;
			num2 += node.left.nodeTotalLength;
		}
		if (node.right != null)
		{
			num += node.right.nodeTotalCount;
			num2 += node.right.nodeTotalLength;
		}
		if (num != node.nodeTotalCount || num2 != node.nodeTotalLength)
		{
			node.nodeTotalCount = num;
			node.nodeTotalLength = num2;
			if (node.parent != null)
			{
				UpdateAfterChildrenChange(node.parent);
			}
		}
	}

	private static void UpdateAfterRotateLeft(DocumentLine node)
	{
		UpdateAfterChildrenChange(node);
	}

	private static void UpdateAfterRotateRight(DocumentLine node)
	{
		UpdateAfterChildrenChange(node);
	}

	public void RebuildTree(List<DocumentLine> documentLines)
	{
		DocumentLine[] array = new DocumentLine[documentLines.Count];
		for (int i = 0; i < documentLines.Count; i++)
		{
			DocumentLine documentLine = documentLines[i].InitLineNode();
			array[i] = documentLine;
		}
		int treeHeight = GetTreeHeight(array.Length);
		root = BuildTree(array, 0, array.Length, treeHeight);
		root.color = false;
	}

	internal static int GetTreeHeight(int size)
	{
		if (size == 0)
		{
			return 0;
		}
		return GetTreeHeight(size / 2) + 1;
	}

	private DocumentLine BuildTree(DocumentLine[] nodes, int start, int end, int subtreeHeight)
	{
		if (start == end)
		{
			return null;
		}
		int num = (start + end) / 2;
		DocumentLine documentLine = nodes[num];
		documentLine.left = BuildTree(nodes, start, num, subtreeHeight - 1);
		documentLine.right = BuildTree(nodes, num + 1, end, subtreeHeight - 1);
		if (documentLine.left != null)
		{
			documentLine.left.parent = documentLine;
		}
		if (documentLine.right != null)
		{
			documentLine.right.parent = documentLine;
		}
		if (subtreeHeight == 1)
		{
			documentLine.color = true;
		}
		UpdateAfterChildrenChange(documentLine);
		return documentLine;
	}

	private DocumentLine GetNodeByIndex(int index)
	{
		DocumentLine documentLine = root;
		while (true)
		{
			if (documentLine.left != null && index < documentLine.left.nodeTotalCount)
			{
				documentLine = documentLine.left;
				continue;
			}
			if (documentLine.left != null)
			{
				index -= documentLine.left.nodeTotalCount;
			}
			if (index == 0)
			{
				break;
			}
			index--;
			documentLine = documentLine.right;
		}
		return documentLine;
	}

	internal static int GetIndexFromNode(DocumentLine node)
	{
		int num = ((node.left != null) ? node.left.nodeTotalCount : 0);
		while (node.parent != null)
		{
			if (node == node.parent.right)
			{
				if (node.parent.left != null)
				{
					num += node.parent.left.nodeTotalCount;
				}
				num++;
			}
			node = node.parent;
		}
		return num;
	}

	private DocumentLine GetNodeByOffset(int offset)
	{
		if (offset == root.nodeTotalLength)
		{
			return root.RightMost;
		}
		DocumentLine documentLine = root;
		while (true)
		{
			if (documentLine.left != null && offset < documentLine.left.nodeTotalLength)
			{
				documentLine = documentLine.left;
				continue;
			}
			if (documentLine.left != null)
			{
				offset -= documentLine.left.nodeTotalLength;
			}
			offset -= documentLine.TotalLength;
			if (offset < 0)
			{
				break;
			}
			documentLine = documentLine.right;
		}
		return documentLine;
	}

	internal static int GetOffsetFromNode(DocumentLine node)
	{
		int num = ((node.left != null) ? node.left.nodeTotalLength : 0);
		while (node.parent != null)
		{
			if (node == node.parent.right)
			{
				if (node.parent.left != null)
				{
					num += node.parent.left.nodeTotalLength;
				}
				num += node.parent.TotalLength;
			}
			node = node.parent;
		}
		return num;
	}

	public DocumentLine GetByNumber(int number)
	{
		return GetNodeByIndex(number - 1);
	}

	public DocumentLine GetByOffset(int offset)
	{
		return GetNodeByOffset(offset);
	}

	public void RemoveLine(DocumentLine line)
	{
		RemoveNode(line);
		line.isDeleted = true;
	}

	public DocumentLine InsertLineAfter(DocumentLine line, int totalLength)
	{
		DocumentLine documentLine = new DocumentLine(document);
		documentLine.TotalLength = totalLength;
		InsertAfter(line, documentLine);
		return documentLine;
	}

	private void InsertAfter(DocumentLine node, DocumentLine newLine)
	{
		DocumentLine newNode = newLine.InitLineNode();
		if (node.right == null)
		{
			InsertAsRight(node, newNode);
		}
		else
		{
			InsertAsLeft(node.right.LeftMost, newNode);
		}
	}

	private void InsertAsLeft(DocumentLine parentNode, DocumentLine newNode)
	{
		parentNode.left = newNode;
		newNode.parent = parentNode;
		newNode.color = true;
		UpdateAfterChildrenChange(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void InsertAsRight(DocumentLine parentNode, DocumentLine newNode)
	{
		parentNode.right = newNode;
		newNode.parent = parentNode;
		newNode.color = true;
		UpdateAfterChildrenChange(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void FixTreeOnInsert(DocumentLine node)
	{
		DocumentLine parent = node.parent;
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
			DocumentLine parent2 = parent.parent;
			DocumentLine documentLine = Sibling(parent);
			if (documentLine != null && documentLine.color)
			{
				parent.color = false;
				documentLine.color = false;
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

	private void RemoveNode(DocumentLine removedNode)
	{
		if (removedNode.left != null && removedNode.right != null)
		{
			DocumentLine leftMost = removedNode.right.LeftMost;
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
			UpdateAfterChildrenChange(leftMost);
			if (leftMost.parent != null)
			{
				UpdateAfterChildrenChange(leftMost.parent);
			}
			return;
		}
		DocumentLine parent = removedNode.parent;
		DocumentLine documentLine = removedNode.left ?? removedNode.right;
		ReplaceNode(removedNode, documentLine);
		if (parent != null)
		{
			UpdateAfterChildrenChange(parent);
		}
		if (!removedNode.color)
		{
			if (documentLine != null && documentLine.color)
			{
				documentLine.color = false;
			}
			else
			{
				FixTreeOnDelete(documentLine, parent);
			}
		}
	}

	private void FixTreeOnDelete(DocumentLine node, DocumentLine parentNode)
	{
		if (parentNode == null)
		{
			return;
		}
		DocumentLine documentLine = Sibling(node, parentNode);
		if (documentLine.color)
		{
			parentNode.color = true;
			documentLine.color = false;
			if (node == parentNode.left)
			{
				RotateLeft(parentNode);
			}
			else
			{
				RotateRight(parentNode);
			}
			documentLine = Sibling(node, parentNode);
		}
		if (!parentNode.color && !documentLine.color && !GetColor(documentLine.left) && !GetColor(documentLine.right))
		{
			documentLine.color = true;
			FixTreeOnDelete(parentNode, parentNode.parent);
			return;
		}
		if (parentNode.color && !documentLine.color && !GetColor(documentLine.left) && !GetColor(documentLine.right))
		{
			documentLine.color = true;
			parentNode.color = false;
			return;
		}
		if (node == parentNode.left && !documentLine.color && GetColor(documentLine.left) && !GetColor(documentLine.right))
		{
			documentLine.color = true;
			documentLine.left.color = false;
			RotateRight(documentLine);
		}
		else if (node == parentNode.right && !documentLine.color && GetColor(documentLine.right) && !GetColor(documentLine.left))
		{
			documentLine.color = true;
			documentLine.right.color = false;
			RotateLeft(documentLine);
		}
		documentLine = Sibling(node, parentNode);
		documentLine.color = parentNode.color;
		parentNode.color = false;
		if (node == parentNode.left)
		{
			if (documentLine.right != null)
			{
				documentLine.right.color = false;
			}
			RotateLeft(parentNode);
		}
		else
		{
			if (documentLine.left != null)
			{
				documentLine.left.color = false;
			}
			RotateRight(parentNode);
		}
	}

	private void ReplaceNode(DocumentLine replacedNode, DocumentLine newNode)
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

	private void RotateLeft(DocumentLine p)
	{
		DocumentLine right = p.right;
		ReplaceNode(p, right);
		p.right = right.left;
		if (p.right != null)
		{
			p.right.parent = p;
		}
		right.left = p;
		p.parent = right;
		UpdateAfterRotateLeft(p);
	}

	private void RotateRight(DocumentLine p)
	{
		DocumentLine left = p.left;
		ReplaceNode(p, left);
		p.left = left.right;
		if (p.left != null)
		{
			p.left.parent = p;
		}
		left.right = p;
		p.parent = left;
		UpdateAfterRotateRight(p);
	}

	private static DocumentLine Sibling(DocumentLine node)
	{
		if (node == node.parent.left)
		{
			return node.parent.right;
		}
		return node.parent.left;
	}

	private static DocumentLine Sibling(DocumentLine node, DocumentLine parentNode)
	{
		if (node == parentNode.left)
		{
			return parentNode.right;
		}
		return parentNode.left;
	}

	private static bool GetColor(DocumentLine node)
	{
		return node?.color ?? false;
	}

	int IList<DocumentLine>.IndexOf(DocumentLine item)
	{
		document.VerifyAccess();
		if (item == null || item.IsDeleted)
		{
			return -1;
		}
		int num = item.LineNumber - 1;
		if (num < LineCount && GetNodeByIndex(num) == item)
		{
			return num;
		}
		return -1;
	}

	void IList<DocumentLine>.Insert(int index, DocumentLine item)
	{
		throw new NotSupportedException();
	}

	void IList<DocumentLine>.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	void ICollection<DocumentLine>.Add(DocumentLine item)
	{
		throw new NotSupportedException();
	}

	void ICollection<DocumentLine>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<DocumentLine>.Contains(DocumentLine item)
	{
		return ((IList<DocumentLine>)this).IndexOf(item) >= 0;
	}

	void ICollection<DocumentLine>.CopyTo(DocumentLine[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (array.Length < LineCount)
		{
			throw new ArgumentException("The array is too small", "array");
		}
		if (arrayIndex < 0 || arrayIndex + LineCount > array.Length)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Value must be between 0 and " + (array.Length - LineCount));
		}
		using IEnumerator<DocumentLine> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			DocumentLine current = enumerator.Current;
			array[arrayIndex++] = current;
		}
	}

	bool ICollection<DocumentLine>.Remove(DocumentLine item)
	{
		throw new NotSupportedException();
	}

	public IEnumerator<DocumentLine> GetEnumerator()
	{
		document.VerifyAccess();
		return Enumerate();
	}

	private IEnumerator<DocumentLine> Enumerate()
	{
		document.VerifyAccess();
		for (DocumentLine line = root.LeftMost; line != null; line = line.NextLine)
		{
			yield return line;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
