using System.Collections.Generic;
using System.Diagnostics;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document;

internal sealed class TextAnchorTree
{
	private readonly TextDocument document;

	private readonly List<TextAnchorNode> nodesToDelete = new List<TextAnchorNode>();

	private TextAnchorNode root;

	internal const bool RED = true;

	internal const bool BLACK = false;

	public TextAnchorTree(TextDocument document)
	{
		this.document = document;
	}

	[Conditional("DEBUG")]
	private static void Log(string text)
	{
	}

	private void InsertText(int offset, int length, bool defaultAnchorMovementIsBeforeInsertion)
	{
		if (length == 0 || root == null || offset > root.totalLength)
		{
			return;
		}
		if (offset == root.totalLength)
		{
			PerformInsertText(FindActualBeginNode(root.RightMost), null, length, defaultAnchorMovementIsBeforeInsertion);
		}
		else
		{
			TextAnchorNode textAnchorNode = FindNode(ref offset);
			if (offset > 0)
			{
				textAnchorNode.length += length;
				UpdateAugmentedData(textAnchorNode);
			}
			else
			{
				PerformInsertText(FindActualBeginNode(textAnchorNode.Predecessor), textAnchorNode, length, defaultAnchorMovementIsBeforeInsertion);
			}
		}
		DeleteMarkedNodes();
	}

	private TextAnchorNode FindActualBeginNode(TextAnchorNode node)
	{
		while (node != null && node.length == 0)
		{
			node = node.Predecessor;
		}
		if (node == null)
		{
			node = root.LeftMost;
		}
		return node;
	}

	private void PerformInsertText(TextAnchorNode beginNode, TextAnchorNode endNode, int length, bool defaultAnchorMovementIsBeforeInsertion)
	{
		List<TextAnchorNode> list = new List<TextAnchorNode>();
		TextAnchorNode textAnchorNode;
		for (textAnchorNode = beginNode; textAnchorNode != endNode; textAnchorNode = textAnchorNode.Successor)
		{
			TextAnchor textAnchor = (TextAnchor)textAnchorNode.Target;
			if (textAnchor == null)
			{
				MarkNodeForDelete(textAnchorNode);
			}
			else if (defaultAnchorMovementIsBeforeInsertion ? (textAnchor.MovementType != AnchorMovementType.AfterInsertion) : (textAnchor.MovementType == AnchorMovementType.BeforeInsertion))
			{
				list.Add(textAnchorNode);
			}
		}
		textAnchorNode = beginNode;
		foreach (TextAnchorNode item in list)
		{
			SwapAnchors(item, textAnchorNode);
			textAnchorNode = textAnchorNode.Successor;
		}
		if (textAnchorNode != null)
		{
			textAnchorNode.length += length;
			UpdateAugmentedData(textAnchorNode);
		}
	}

	private void SwapAnchors(TextAnchorNode n1, TextAnchorNode n2)
	{
		if (n1 == n2)
		{
			return;
		}
		TextAnchor textAnchor = (TextAnchor)n1.Target;
		TextAnchor textAnchor2 = (TextAnchor)n2.Target;
		if (textAnchor != null || textAnchor2 != null)
		{
			n1.Target = textAnchor2;
			n2.Target = textAnchor;
			if (textAnchor == null)
			{
				nodesToDelete.Remove(n1);
				MarkNodeForDelete(n2);
				textAnchor2.node = n1;
			}
			else if (textAnchor2 == null)
			{
				nodesToDelete.Remove(n2);
				MarkNodeForDelete(n1);
				textAnchor.node = n2;
			}
			else
			{
				textAnchor.node = n2;
				textAnchor2.node = n1;
			}
		}
	}

	public void HandleTextChange(OffsetChangeMapEntry entry, DelayedEvents delayedEvents)
	{
		if (entry.RemovalLength == 0)
		{
			InsertText(entry.Offset, entry.InsertionLength, entry.DefaultAnchorMovementIsBeforeInsertion);
			return;
		}
		int offset = entry.Offset;
		int num = entry.RemovalLength;
		if (root == null || offset >= root.totalLength)
		{
			return;
		}
		TextAnchorNode textAnchorNode = FindNode(ref offset);
		TextAnchorNode textAnchorNode2 = null;
		while (textAnchorNode != null && offset + num > textAnchorNode.length)
		{
			TextAnchor textAnchor = (TextAnchor)textAnchorNode.Target;
			if (textAnchor != null && (textAnchor.SurviveDeletion || entry.RemovalNeverCausesAnchorDeletion))
			{
				if (textAnchorNode2 == null)
				{
					textAnchorNode2 = textAnchorNode;
				}
				num -= textAnchorNode.length - offset;
				textAnchorNode.length = offset;
				offset = 0;
				UpdateAugmentedData(textAnchorNode);
				textAnchorNode = textAnchorNode.Successor;
			}
			else
			{
				TextAnchorNode successor = textAnchorNode.Successor;
				num -= textAnchorNode.length;
				RemoveNode(textAnchorNode);
				nodesToDelete.Remove(textAnchorNode);
				textAnchor?.OnDeleted(delayedEvents);
				textAnchorNode = successor;
			}
		}
		if (textAnchorNode != null)
		{
			textAnchorNode.length -= num;
		}
		if (entry.InsertionLength > 0)
		{
			if (textAnchorNode2 != null)
			{
				PerformInsertText(textAnchorNode2, textAnchorNode, entry.InsertionLength, entry.DefaultAnchorMovementIsBeforeInsertion);
			}
			else if (textAnchorNode != null)
			{
				textAnchorNode.length += entry.InsertionLength;
			}
		}
		if (textAnchorNode != null)
		{
			UpdateAugmentedData(textAnchorNode);
		}
		DeleteMarkedNodes();
	}

	private void MarkNodeForDelete(TextAnchorNode node)
	{
		if (!nodesToDelete.Contains(node))
		{
			nodesToDelete.Add(node);
		}
	}

	private void DeleteMarkedNodes()
	{
		while (nodesToDelete.Count > 0)
		{
			int index = nodesToDelete.Count - 1;
			TextAnchorNode textAnchorNode = nodesToDelete[index];
			TextAnchorNode successor = textAnchorNode.Successor;
			if (successor != null)
			{
				successor.length += textAnchorNode.length;
			}
			RemoveNode(textAnchorNode);
			if (successor != null)
			{
				UpdateAugmentedData(successor);
			}
			nodesToDelete.RemoveAt(index);
		}
	}

	private TextAnchorNode FindNode(ref int offset)
	{
		TextAnchorNode textAnchorNode = root;
		while (true)
		{
			if (textAnchorNode.left != null)
			{
				if (offset < textAnchorNode.left.totalLength)
				{
					textAnchorNode = textAnchorNode.left;
					continue;
				}
				offset -= textAnchorNode.left.totalLength;
			}
			if (!textAnchorNode.IsAlive)
			{
				MarkNodeForDelete(textAnchorNode);
			}
			if (offset < textAnchorNode.length)
			{
				return textAnchorNode;
			}
			offset -= textAnchorNode.length;
			if (textAnchorNode.right == null)
			{
				break;
			}
			textAnchorNode = textAnchorNode.right;
		}
		return null;
	}

	private void UpdateAugmentedData(TextAnchorNode n)
	{
		if (!n.IsAlive)
		{
			MarkNodeForDelete(n);
		}
		int num = n.length;
		if (n.left != null)
		{
			num += n.left.totalLength;
		}
		if (n.right != null)
		{
			num += n.right.totalLength;
		}
		if (n.totalLength != num)
		{
			n.totalLength = num;
			if (n.parent != null)
			{
				UpdateAugmentedData(n.parent);
			}
		}
	}

	public TextAnchor CreateAnchor(int offset)
	{
		TextAnchor textAnchor = new TextAnchor(document);
		textAnchor.node = new TextAnchorNode(textAnchor);
		if (root == null)
		{
			root = textAnchor.node;
			root.totalLength = (root.length = offset);
		}
		else if (offset >= root.totalLength)
		{
			textAnchor.node.totalLength = (textAnchor.node.length = offset - root.totalLength);
			InsertAsRight(root.RightMost, textAnchor.node);
		}
		else
		{
			TextAnchorNode textAnchorNode = FindNode(ref offset);
			textAnchor.node.totalLength = (textAnchor.node.length = offset);
			textAnchorNode.length -= offset;
			InsertBefore(textAnchorNode, textAnchor.node);
		}
		DeleteMarkedNodes();
		return textAnchor;
	}

	private void InsertBefore(TextAnchorNode node, TextAnchorNode newNode)
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

	private void InsertAsLeft(TextAnchorNode parentNode, TextAnchorNode newNode)
	{
		parentNode.left = newNode;
		newNode.parent = parentNode;
		newNode.color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void InsertAsRight(TextAnchorNode parentNode, TextAnchorNode newNode)
	{
		parentNode.right = newNode;
		newNode.parent = parentNode;
		newNode.color = true;
		UpdateAugmentedData(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void FixTreeOnInsert(TextAnchorNode node)
	{
		TextAnchorNode parent = node.parent;
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
			TextAnchorNode parent2 = parent.parent;
			TextAnchorNode textAnchorNode = Sibling(parent);
			if (textAnchorNode != null && textAnchorNode.color)
			{
				parent.color = false;
				textAnchorNode.color = false;
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

	private void RemoveNode(TextAnchorNode removedNode)
	{
		if (removedNode.left != null && removedNode.right != null)
		{
			TextAnchorNode leftMost = removedNode.right.LeftMost;
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
		TextAnchorNode parent = removedNode.parent;
		TextAnchorNode textAnchorNode = removedNode.left ?? removedNode.right;
		ReplaceNode(removedNode, textAnchorNode);
		if (parent != null)
		{
			UpdateAugmentedData(parent);
		}
		if (!removedNode.color)
		{
			if (textAnchorNode != null && textAnchorNode.color)
			{
				textAnchorNode.color = false;
			}
			else
			{
				FixTreeOnDelete(textAnchorNode, parent);
			}
		}
	}

	private void FixTreeOnDelete(TextAnchorNode node, TextAnchorNode parentNode)
	{
		if (parentNode == null)
		{
			return;
		}
		TextAnchorNode textAnchorNode = Sibling(node, parentNode);
		if (textAnchorNode.color)
		{
			parentNode.color = true;
			textAnchorNode.color = false;
			if (node == parentNode.left)
			{
				RotateLeft(parentNode);
			}
			else
			{
				RotateRight(parentNode);
			}
			textAnchorNode = Sibling(node, parentNode);
		}
		if (!parentNode.color && !textAnchorNode.color && !GetColor(textAnchorNode.left) && !GetColor(textAnchorNode.right))
		{
			textAnchorNode.color = true;
			FixTreeOnDelete(parentNode, parentNode.parent);
			return;
		}
		if (parentNode.color && !textAnchorNode.color && !GetColor(textAnchorNode.left) && !GetColor(textAnchorNode.right))
		{
			textAnchorNode.color = true;
			parentNode.color = false;
			return;
		}
		if (node == parentNode.left && !textAnchorNode.color && GetColor(textAnchorNode.left) && !GetColor(textAnchorNode.right))
		{
			textAnchorNode.color = true;
			textAnchorNode.left.color = false;
			RotateRight(textAnchorNode);
		}
		else if (node == parentNode.right && !textAnchorNode.color && GetColor(textAnchorNode.right) && !GetColor(textAnchorNode.left))
		{
			textAnchorNode.color = true;
			textAnchorNode.right.color = false;
			RotateLeft(textAnchorNode);
		}
		textAnchorNode = Sibling(node, parentNode);
		textAnchorNode.color = parentNode.color;
		parentNode.color = false;
		if (node == parentNode.left)
		{
			if (textAnchorNode.right != null)
			{
				textAnchorNode.right.color = false;
			}
			RotateLeft(parentNode);
		}
		else
		{
			if (textAnchorNode.left != null)
			{
				textAnchorNode.left.color = false;
			}
			RotateRight(parentNode);
		}
	}

	private void ReplaceNode(TextAnchorNode replacedNode, TextAnchorNode newNode)
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

	private void RotateLeft(TextAnchorNode p)
	{
		TextAnchorNode right = p.right;
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

	private void RotateRight(TextAnchorNode p)
	{
		TextAnchorNode left = p.left;
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

	private static TextAnchorNode Sibling(TextAnchorNode node)
	{
		if (node == node.parent.left)
		{
			return node.parent.right;
		}
		return node.parent.left;
	}

	private static TextAnchorNode Sibling(TextAnchorNode node, TextAnchorNode parentNode)
	{
		if (node == parentNode.left)
		{
			return parentNode.right;
		}
		return parentNode.left;
	}

	private static bool GetColor(TextAnchorNode node)
	{
		return node?.color ?? false;
	}

	[Conditional("DATACONSISTENCYTEST")]
	internal void CheckProperties()
	{
	}
}
