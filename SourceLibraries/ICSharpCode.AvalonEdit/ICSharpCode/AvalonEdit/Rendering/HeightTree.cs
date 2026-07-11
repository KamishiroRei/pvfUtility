using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class HeightTree : ILineTracker, IDisposable
{
	private enum UpdateAfterChildrenChangeRecursionMode
	{
		None,
		IfRequired,
		WholeBranch
	}

	private readonly TextDocument document;

	private HeightTreeNode root;

	private WeakLineTracker weakLineTracker;

	private double defaultLineHeight;

	private bool inRemoval;

	private List<HeightTreeNode> nodesToCheckForMerging;

	private const bool RED = true;

	private const bool BLACK = false;

	public bool IsDisposed => root == null;

	public double DefaultLineHeight
	{
		get
		{
			return defaultLineHeight;
		}
		set
		{
			double num = defaultLineHeight;
			if (num == value)
			{
				return;
			}
			defaultLineHeight = value;
			foreach (HeightTreeNode allNode in AllNodes)
			{
				if (allNode.lineNode.height == num)
				{
					allNode.lineNode.height = value;
					UpdateAugmentedData(allNode, UpdateAfterChildrenChangeRecursionMode.IfRequired);
				}
			}
		}
	}

	public int LineCount => root.totalCount;

	public double TotalHeight => root.totalHeight;

	private IEnumerable<HeightTreeNode> AllNodes
	{
		get
		{
			if (root != null)
			{
				for (HeightTreeNode node = root.LeftMost; node != null; node = node.Successor)
				{
					yield return node;
				}
			}
		}
	}

	public HeightTree(TextDocument document, double defaultLineHeight)
	{
		this.document = document;
		weakLineTracker = WeakLineTracker.Register(document, this);
		DefaultLineHeight = defaultLineHeight;
		RebuildDocument();
	}

	public void Dispose()
	{
		if (weakLineTracker != null)
		{
			weakLineTracker.Deregister();
		}
		root = null;
		weakLineTracker = null;
	}

	private HeightTreeNode GetNode(DocumentLine ls)
	{
		return GetNodeByIndex(ls.LineNumber - 1);
	}

	void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
	{
	}

	void ILineTracker.SetLineLength(DocumentLine ls, int newTotalLength)
	{
	}

	public void RebuildDocument()
	{
		foreach (CollapsedLineSection allCollapsedSection in GetAllCollapsedSections())
		{
			allCollapsedSection.Start = null;
			allCollapsedSection.End = null;
		}
		HeightTreeNode[] array = new HeightTreeNode[document.LineCount];
		int num = 0;
		foreach (DocumentLine line in document.Lines)
		{
			array[num++] = new HeightTreeNode(line, defaultLineHeight);
		}
		int treeHeight = DocumentLineTree.GetTreeHeight(array.Length);
		root = BuildTree(array, 0, array.Length, treeHeight);
		root.color = false;
	}

	private HeightTreeNode BuildTree(HeightTreeNode[] nodes, int start, int end, int subtreeHeight)
	{
		if (start == end)
		{
			return null;
		}
		int num = (start + end) / 2;
		HeightTreeNode heightTreeNode = nodes[num];
		heightTreeNode.left = BuildTree(nodes, start, num, subtreeHeight - 1);
		heightTreeNode.right = BuildTree(nodes, num + 1, end, subtreeHeight - 1);
		if (heightTreeNode.left != null)
		{
			heightTreeNode.left.parent = heightTreeNode;
		}
		if (heightTreeNode.right != null)
		{
			heightTreeNode.right.parent = heightTreeNode;
		}
		if (subtreeHeight == 1)
		{
			heightTreeNode.color = true;
		}
		UpdateAugmentedData(heightTreeNode, UpdateAfterChildrenChangeRecursionMode.None);
		return heightTreeNode;
	}

	void ILineTracker.BeforeRemoveLine(DocumentLine line)
	{
		HeightTreeNode node = GetNode(line);
		if (node.lineNode.collapsedSections != null)
		{
			CollapsedLineSection[] array = node.lineNode.collapsedSections.ToArray();
			foreach (CollapsedLineSection collapsedLineSection in array)
			{
				if (collapsedLineSection.Start == line && collapsedLineSection.End == line)
				{
					collapsedLineSection.Start = null;
					collapsedLineSection.End = null;
				}
				else if (collapsedLineSection.Start == line)
				{
					Uncollapse(collapsedLineSection);
					collapsedLineSection.Start = line.NextLine;
					AddCollapsedSection(collapsedLineSection, collapsedLineSection.End.LineNumber - collapsedLineSection.Start.LineNumber + 1);
				}
				else if (collapsedLineSection.End == line)
				{
					Uncollapse(collapsedLineSection);
					collapsedLineSection.End = line.PreviousLine;
					AddCollapsedSection(collapsedLineSection, collapsedLineSection.End.LineNumber - collapsedLineSection.Start.LineNumber + 1);
				}
			}
		}
		BeginRemoval();
		RemoveNode(node);
		node.lineNode.collapsedSections = null;
		EndRemoval();
	}

	void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
	{
		InsertAfter(GetNode(insertionPos), newLine);
	}

	private HeightTreeNode InsertAfter(HeightTreeNode node, DocumentLine newLine)
	{
		HeightTreeNode heightTreeNode = new HeightTreeNode(newLine, defaultLineHeight);
		if (node.right == null)
		{
			if (node.lineNode.collapsedSections != null)
			{
				foreach (CollapsedLineSection collapsedSection in node.lineNode.collapsedSections)
				{
					if (collapsedSection.End != node.documentLine)
					{
						heightTreeNode.AddDirectlyCollapsed(collapsedSection);
					}
				}
			}
			InsertAsRight(node, heightTreeNode);
		}
		else
		{
			node = node.right.LeftMost;
			if (node.lineNode.collapsedSections != null)
			{
				foreach (CollapsedLineSection collapsedSection2 in node.lineNode.collapsedSections)
				{
					if (collapsedSection2.Start != node.documentLine)
					{
						heightTreeNode.AddDirectlyCollapsed(collapsedSection2);
					}
				}
			}
			InsertAsLeft(node, heightTreeNode);
		}
		return heightTreeNode;
	}

	private static void UpdateAfterChildrenChange(HeightTreeNode node)
	{
		UpdateAugmentedData(node, UpdateAfterChildrenChangeRecursionMode.IfRequired);
	}

	private static void UpdateAugmentedData(HeightTreeNode node, UpdateAfterChildrenChangeRecursionMode mode)
	{
		int num = 1;
		double num2 = node.lineNode.TotalHeight;
		if (node.left != null)
		{
			num += node.left.totalCount;
			num2 += node.left.totalHeight;
		}
		if (node.right != null)
		{
			num += node.right.totalCount;
			num2 += node.right.totalHeight;
		}
		if (node.IsDirectlyCollapsed)
		{
			num2 = 0.0;
		}
		if (num != node.totalCount || !num2.IsClose(node.totalHeight) || mode == UpdateAfterChildrenChangeRecursionMode.WholeBranch)
		{
			node.totalCount = num;
			node.totalHeight = num2;
			if (node.parent != null && mode != UpdateAfterChildrenChangeRecursionMode.None)
			{
				UpdateAugmentedData(node.parent, mode);
			}
		}
	}

	private void UpdateAfterRotateLeft(HeightTreeNode node)
	{
		List<CollapsedLineSection> collapsedSections = node.parent.collapsedSections;
		List<CollapsedLineSection> collapsedSections2 = node.collapsedSections;
		node.parent.collapsedSections = collapsedSections2;
		node.collapsedSections = null;
		if (collapsedSections != null)
		{
			foreach (CollapsedLineSection item in collapsedSections)
			{
				if (node.parent.right != null)
				{
					node.parent.right.AddDirectlyCollapsed(item);
				}
				node.parent.lineNode.AddDirectlyCollapsed(item);
				if (node.right != null)
				{
					node.right.AddDirectlyCollapsed(item);
				}
			}
		}
		MergeCollapsedSectionsIfPossible(node);
		UpdateAfterChildrenChange(node);
	}

	private void UpdateAfterRotateRight(HeightTreeNode node)
	{
		List<CollapsedLineSection> collapsedSections = node.parent.collapsedSections;
		List<CollapsedLineSection> collapsedSections2 = node.collapsedSections;
		node.parent.collapsedSections = collapsedSections2;
		node.collapsedSections = null;
		if (collapsedSections != null)
		{
			foreach (CollapsedLineSection item in collapsedSections)
			{
				if (node.parent.left != null)
				{
					node.parent.left.AddDirectlyCollapsed(item);
				}
				node.parent.lineNode.AddDirectlyCollapsed(item);
				if (node.left != null)
				{
					node.left.AddDirectlyCollapsed(item);
				}
			}
		}
		MergeCollapsedSectionsIfPossible(node);
		UpdateAfterChildrenChange(node);
	}

	private void BeforeNodeRemove(HeightTreeNode removedNode)
	{
		List<CollapsedLineSection> collapsedSections = removedNode.collapsedSections;
		if (collapsedSections != null)
		{
			HeightTreeNode heightTreeNode = removedNode.left ?? removedNode.right;
			if (heightTreeNode != null)
			{
				foreach (CollapsedLineSection item in collapsedSections)
				{
					heightTreeNode.AddDirectlyCollapsed(item);
				}
			}
		}
		if (removedNode.parent != null)
		{
			MergeCollapsedSectionsIfPossible(removedNode.parent);
		}
	}

	private void BeforeNodeReplace(HeightTreeNode removedNode, HeightTreeNode newNode, HeightTreeNode newNodeOldParent)
	{
		while (newNodeOldParent != removedNode)
		{
			if (newNodeOldParent.collapsedSections != null)
			{
				foreach (CollapsedLineSection collapsedSection in newNodeOldParent.collapsedSections)
				{
					newNode.lineNode.AddDirectlyCollapsed(collapsedSection);
				}
			}
			newNodeOldParent = newNodeOldParent.parent;
		}
		if (newNode.collapsedSections != null)
		{
			foreach (CollapsedLineSection collapsedSection2 in newNode.collapsedSections)
			{
				newNode.lineNode.AddDirectlyCollapsed(collapsedSection2);
			}
		}
		newNode.collapsedSections = removedNode.collapsedSections;
		MergeCollapsedSectionsIfPossible(newNode);
	}

	private void BeginRemoval()
	{
		if (nodesToCheckForMerging == null)
		{
			nodesToCheckForMerging = new List<HeightTreeNode>();
		}
		inRemoval = true;
	}

	private void EndRemoval()
	{
		inRemoval = false;
		foreach (HeightTreeNode item in nodesToCheckForMerging)
		{
			MergeCollapsedSectionsIfPossible(item);
		}
		nodesToCheckForMerging.Clear();
	}

	private void MergeCollapsedSectionsIfPossible(HeightTreeNode node)
	{
		if (inRemoval)
		{
			nodesToCheckForMerging.Add(node);
			return;
		}
		bool flag = false;
		List<CollapsedLineSection> collapsedSections = node.lineNode.collapsedSections;
		if (collapsedSections != null)
		{
			for (int num = collapsedSections.Count - 1; num >= 0; num--)
			{
				CollapsedLineSection collapsedLineSection = collapsedSections[num];
				if (collapsedLineSection.Start != node.documentLine && collapsedLineSection.End != node.documentLine && (node.left == null || (node.left.collapsedSections != null && node.left.collapsedSections.Contains(collapsedLineSection))) && (node.right == null || (node.right.collapsedSections != null && node.right.collapsedSections.Contains(collapsedLineSection))))
				{
					if (node.left != null)
					{
						node.left.RemoveDirectlyCollapsed(collapsedLineSection);
					}
					if (node.right != null)
					{
						node.right.RemoveDirectlyCollapsed(collapsedLineSection);
					}
					collapsedSections.RemoveAt(num);
					node.AddDirectlyCollapsed(collapsedLineSection);
					flag = true;
				}
			}
			if (collapsedSections.Count == 0)
			{
				node.lineNode.collapsedSections = null;
			}
		}
		if (flag && node.parent != null)
		{
			MergeCollapsedSectionsIfPossible(node.parent);
		}
	}

	private HeightTreeNode GetNodeByIndex(int index)
	{
		HeightTreeNode heightTreeNode = root;
		while (true)
		{
			if (heightTreeNode.left != null && index < heightTreeNode.left.totalCount)
			{
				heightTreeNode = heightTreeNode.left;
				continue;
			}
			if (heightTreeNode.left != null)
			{
				index -= heightTreeNode.left.totalCount;
			}
			if (index == 0)
			{
				break;
			}
			index--;
			heightTreeNode = heightTreeNode.right;
		}
		return heightTreeNode;
	}

	private HeightTreeNode GetNodeByVisualPosition(double position)
	{
		HeightTreeNode heightTreeNode = root;
		while (true)
		{
			double num = position;
			if (heightTreeNode.left != null)
			{
				num -= heightTreeNode.left.totalHeight;
				if (num < 0.0)
				{
					heightTreeNode = heightTreeNode.left;
					continue;
				}
			}
			double num2 = num - heightTreeNode.lineNode.TotalHeight;
			if (num2 < 0.0)
			{
				return heightTreeNode;
			}
			if (heightTreeNode.right == null || heightTreeNode.right.totalHeight == 0.0)
			{
				if (heightTreeNode.lineNode.TotalHeight > 0.0 || heightTreeNode.left == null)
				{
					break;
				}
				heightTreeNode = heightTreeNode.left;
			}
			else
			{
				position = num2;
				heightTreeNode = heightTreeNode.right;
			}
		}
		return heightTreeNode;
	}

	private static double GetVisualPositionFromNode(HeightTreeNode node)
	{
		double num = ((node.left != null) ? node.left.totalHeight : 0.0);
		while (node.parent != null)
		{
			if (node.IsDirectlyCollapsed)
			{
				num = 0.0;
			}
			if (node == node.parent.right)
			{
				if (node.parent.left != null)
				{
					num += node.parent.left.totalHeight;
				}
				num += node.parent.lineNode.TotalHeight;
			}
			node = node.parent;
		}
		return num;
	}

	public DocumentLine GetLineByNumber(int number)
	{
		return GetNodeByIndex(number - 1).documentLine;
	}

	public DocumentLine GetLineByVisualPosition(double position)
	{
		return GetNodeByVisualPosition(position).documentLine;
	}

	public double GetVisualPosition(DocumentLine line)
	{
		return GetVisualPositionFromNode(GetNode(line));
	}

	public double GetHeight(DocumentLine line)
	{
		return GetNode(line).lineNode.height;
	}

	public void SetHeight(DocumentLine line, double val)
	{
		HeightTreeNode node = GetNode(line);
		node.lineNode.height = val;
		UpdateAfterChildrenChange(node);
	}

	public bool GetIsCollapsed(int lineNumber)
	{
		HeightTreeNode nodeByIndex = GetNodeByIndex(lineNumber - 1);
		if (!nodeByIndex.lineNode.IsDirectlyCollapsed)
		{
			return GetIsCollapedFromNode(nodeByIndex);
		}
		return true;
	}

	public CollapsedLineSection CollapseText(DocumentLine start, DocumentLine end)
	{
		if (!document.Lines.Contains(start))
		{
			throw new ArgumentException("Line is not part of this document", "start");
		}
		if (!document.Lines.Contains(end))
		{
			throw new ArgumentException("Line is not part of this document", "end");
		}
		int num = end.LineNumber - start.LineNumber + 1;
		if (num < 0)
		{
			throw new ArgumentException("start must be a line before end");
		}
		CollapsedLineSection collapsedLineSection = new CollapsedLineSection(this, start, end);
		AddCollapsedSection(collapsedLineSection, num);
		return collapsedLineSection;
	}

	internal IEnumerable<CollapsedLineSection> GetAllCollapsedSections()
	{
		List<CollapsedLineSection> emptyCSList = new List<CollapsedLineSection>();
		return AllNodes.SelectMany((HeightTreeNode node) => (node.lineNode.collapsedSections ?? emptyCSList).Concat(node.collapsedSections ?? emptyCSList)).Distinct();
	}

	private void InsertAsLeft(HeightTreeNode parentNode, HeightTreeNode newNode)
	{
		parentNode.left = newNode;
		newNode.parent = parentNode;
		newNode.color = true;
		UpdateAfterChildrenChange(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void InsertAsRight(HeightTreeNode parentNode, HeightTreeNode newNode)
	{
		parentNode.right = newNode;
		newNode.parent = parentNode;
		newNode.color = true;
		UpdateAfterChildrenChange(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void FixTreeOnInsert(HeightTreeNode node)
	{
		HeightTreeNode parent = node.parent;
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
			HeightTreeNode parent2 = parent.parent;
			HeightTreeNode heightTreeNode = Sibling(parent);
			if (heightTreeNode != null && heightTreeNode.color)
			{
				parent.color = false;
				heightTreeNode.color = false;
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

	private void RemoveNode(HeightTreeNode removedNode)
	{
		if (removedNode.left != null && removedNode.right != null)
		{
			HeightTreeNode leftMost = removedNode.right.LeftMost;
			HeightTreeNode parent = leftMost.parent;
			RemoveNode(leftMost);
			BeforeNodeReplace(removedNode, leftMost, parent);
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
		HeightTreeNode parent2 = removedNode.parent;
		HeightTreeNode heightTreeNode = removedNode.left ?? removedNode.right;
		BeforeNodeRemove(removedNode);
		ReplaceNode(removedNode, heightTreeNode);
		if (parent2 != null)
		{
			UpdateAfterChildrenChange(parent2);
		}
		if (!removedNode.color)
		{
			if (heightTreeNode != null && heightTreeNode.color)
			{
				heightTreeNode.color = false;
			}
			else
			{
				FixTreeOnDelete(heightTreeNode, parent2);
			}
		}
	}

	private void FixTreeOnDelete(HeightTreeNode node, HeightTreeNode parentNode)
	{
		if (parentNode == null)
		{
			return;
		}
		HeightTreeNode heightTreeNode = Sibling(node, parentNode);
		if (heightTreeNode.color)
		{
			parentNode.color = true;
			heightTreeNode.color = false;
			if (node == parentNode.left)
			{
				RotateLeft(parentNode);
			}
			else
			{
				RotateRight(parentNode);
			}
			heightTreeNode = Sibling(node, parentNode);
		}
		if (!parentNode.color && !heightTreeNode.color && !GetColor(heightTreeNode.left) && !GetColor(heightTreeNode.right))
		{
			heightTreeNode.color = true;
			FixTreeOnDelete(parentNode, parentNode.parent);
			return;
		}
		if (parentNode.color && !heightTreeNode.color && !GetColor(heightTreeNode.left) && !GetColor(heightTreeNode.right))
		{
			heightTreeNode.color = true;
			parentNode.color = false;
			return;
		}
		if (node == parentNode.left && !heightTreeNode.color && GetColor(heightTreeNode.left) && !GetColor(heightTreeNode.right))
		{
			heightTreeNode.color = true;
			heightTreeNode.left.color = false;
			RotateRight(heightTreeNode);
		}
		else if (node == parentNode.right && !heightTreeNode.color && GetColor(heightTreeNode.right) && !GetColor(heightTreeNode.left))
		{
			heightTreeNode.color = true;
			heightTreeNode.right.color = false;
			RotateLeft(heightTreeNode);
		}
		heightTreeNode = Sibling(node, parentNode);
		heightTreeNode.color = parentNode.color;
		parentNode.color = false;
		if (node == parentNode.left)
		{
			if (heightTreeNode.right != null)
			{
				heightTreeNode.right.color = false;
			}
			RotateLeft(parentNode);
		}
		else
		{
			if (heightTreeNode.left != null)
			{
				heightTreeNode.left.color = false;
			}
			RotateRight(parentNode);
		}
	}

	private void ReplaceNode(HeightTreeNode replacedNode, HeightTreeNode newNode)
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

	private void RotateLeft(HeightTreeNode p)
	{
		HeightTreeNode right = p.right;
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

	private void RotateRight(HeightTreeNode p)
	{
		HeightTreeNode left = p.left;
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

	private static HeightTreeNode Sibling(HeightTreeNode node)
	{
		if (node == node.parent.left)
		{
			return node.parent.right;
		}
		return node.parent.left;
	}

	private static HeightTreeNode Sibling(HeightTreeNode node, HeightTreeNode parentNode)
	{
		if (node == parentNode.left)
		{
			return parentNode.right;
		}
		return parentNode.left;
	}

	private static bool GetColor(HeightTreeNode node)
	{
		return node?.color ?? false;
	}

	private static bool GetIsCollapedFromNode(HeightTreeNode node)
	{
		while (node != null)
		{
			if (node.IsDirectlyCollapsed)
			{
				return true;
			}
			node = node.parent;
		}
		return false;
	}

	internal void AddCollapsedSection(CollapsedLineSection section, int sectionLength)
	{
		AddRemoveCollapsedSection(section, sectionLength, add: true);
	}

	private void AddRemoveCollapsedSection(CollapsedLineSection section, int sectionLength, bool add)
	{
		HeightTreeNode heightTreeNode = GetNode(section.Start);
		while (true)
		{
			if (add)
			{
				heightTreeNode.lineNode.AddDirectlyCollapsed(section);
			}
			else
			{
				heightTreeNode.lineNode.RemoveDirectlyCollapsed(section);
			}
			sectionLength--;
			if (sectionLength == 0)
			{
				break;
			}
			if (heightTreeNode.right != null)
			{
				if (heightTreeNode.right.totalCount >= sectionLength)
				{
					AddRemoveCollapsedSectionDown(section, heightTreeNode.right, sectionLength, add);
					break;
				}
				if (add)
				{
					heightTreeNode.right.AddDirectlyCollapsed(section);
				}
				else
				{
					heightTreeNode.right.RemoveDirectlyCollapsed(section);
				}
				sectionLength -= heightTreeNode.right.totalCount;
			}
			HeightTreeNode parent = heightTreeNode.parent;
			while (parent.right == heightTreeNode)
			{
				heightTreeNode = parent;
				parent = heightTreeNode.parent;
			}
			heightTreeNode = parent;
		}
		UpdateAugmentedData(GetNode(section.Start), UpdateAfterChildrenChangeRecursionMode.WholeBranch);
		UpdateAugmentedData(GetNode(section.End), UpdateAfterChildrenChangeRecursionMode.WholeBranch);
	}

	private static void AddRemoveCollapsedSectionDown(CollapsedLineSection section, HeightTreeNode node, int sectionLength, bool add)
	{
		while (true)
		{
			if (node.left != null)
			{
				if (node.left.totalCount >= sectionLength)
				{
					node = node.left;
					continue;
				}
				if (add)
				{
					node.left.AddDirectlyCollapsed(section);
				}
				else
				{
					node.left.RemoveDirectlyCollapsed(section);
				}
				sectionLength -= node.left.totalCount;
			}
			if (add)
			{
				node.lineNode.AddDirectlyCollapsed(section);
			}
			else
			{
				node.lineNode.RemoveDirectlyCollapsed(section);
			}
			sectionLength--;
			if (sectionLength != 0)
			{
				node = node.right;
				continue;
			}
			break;
		}
	}

	public void Uncollapse(CollapsedLineSection section)
	{
		int sectionLength = section.End.LineNumber - section.Start.LineNumber + 1;
		AddRemoveCollapsedSection(section, sectionLength, add: false);
	}
}
