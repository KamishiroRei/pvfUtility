using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class HeightTreeNode
{
	internal readonly DocumentLine documentLine;

	internal HeightTreeLineNode lineNode;

	internal HeightTreeNode left;

	internal HeightTreeNode right;

	internal HeightTreeNode parent;

	internal bool color;

	internal int totalCount;

	internal double totalHeight;

	internal List<CollapsedLineSection> collapsedSections;

	internal HeightTreeNode LeftMost
	{
		get
		{
			HeightTreeNode heightTreeNode = this;
			while (heightTreeNode.left != null)
			{
				heightTreeNode = heightTreeNode.left;
			}
			return heightTreeNode;
		}
	}

	internal HeightTreeNode RightMost
	{
		get
		{
			HeightTreeNode heightTreeNode = this;
			while (heightTreeNode.right != null)
			{
				heightTreeNode = heightTreeNode.right;
			}
			return heightTreeNode;
		}
	}

	internal HeightTreeNode Successor
	{
		get
		{
			if (right != null)
			{
				return right.LeftMost;
			}
			HeightTreeNode heightTreeNode = this;
			HeightTreeNode heightTreeNode2;
			do
			{
				heightTreeNode2 = heightTreeNode;
				heightTreeNode = heightTreeNode.parent;
			}
			while (heightTreeNode != null && heightTreeNode.right == heightTreeNode2);
			return heightTreeNode;
		}
	}

	internal bool IsDirectlyCollapsed => collapsedSections != null;

	internal HeightTreeNode()
	{
	}

	internal HeightTreeNode(DocumentLine documentLine, double height)
	{
		this.documentLine = documentLine;
		totalCount = 1;
		lineNode = new HeightTreeLineNode(height);
		totalHeight = height;
	}

	internal void AddDirectlyCollapsed(CollapsedLineSection section)
	{
		if (collapsedSections == null)
		{
			collapsedSections = new List<CollapsedLineSection>();
			totalHeight = 0.0;
		}
		collapsedSections.Add(section);
	}

	internal void RemoveDirectlyCollapsed(CollapsedLineSection section)
	{
		collapsedSections.Remove(section);
		if (collapsedSections.Count == 0)
		{
			collapsedSections = null;
			totalHeight = lineNode.TotalHeight;
			if (left != null)
			{
				totalHeight += left.totalHeight;
			}
			if (right != null)
			{
				totalHeight += right.totalHeight;
			}
		}
	}
}
