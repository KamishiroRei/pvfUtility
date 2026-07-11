using System;

namespace ICSharpCode.AvalonEdit.Document;

internal sealed class TextAnchorNode : WeakReference
{
	internal TextAnchorNode left;

	internal TextAnchorNode right;

	internal TextAnchorNode parent;

	internal bool color;

	internal int length;

	internal int totalLength;

	internal TextAnchorNode LeftMost
	{
		get
		{
			TextAnchorNode textAnchorNode = this;
			while (textAnchorNode.left != null)
			{
				textAnchorNode = textAnchorNode.left;
			}
			return textAnchorNode;
		}
	}

	internal TextAnchorNode RightMost
	{
		get
		{
			TextAnchorNode textAnchorNode = this;
			while (textAnchorNode.right != null)
			{
				textAnchorNode = textAnchorNode.right;
			}
			return textAnchorNode;
		}
	}

	internal TextAnchorNode Successor
	{
		get
		{
			if (right != null)
			{
				return right.LeftMost;
			}
			TextAnchorNode textAnchorNode = this;
			TextAnchorNode textAnchorNode2;
			do
			{
				textAnchorNode2 = textAnchorNode;
				textAnchorNode = textAnchorNode.parent;
			}
			while (textAnchorNode != null && textAnchorNode.right == textAnchorNode2);
			return textAnchorNode;
		}
	}

	internal TextAnchorNode Predecessor
	{
		get
		{
			if (left != null)
			{
				return left.RightMost;
			}
			TextAnchorNode textAnchorNode = this;
			TextAnchorNode textAnchorNode2;
			do
			{
				textAnchorNode2 = textAnchorNode;
				textAnchorNode = textAnchorNode.parent;
			}
			while (textAnchorNode != null && textAnchorNode.left == textAnchorNode2);
			return textAnchorNode;
		}
	}

	public TextAnchorNode(TextAnchor anchor)
		: base(anchor)
	{
	}

	public override string ToString()
	{
		return "[TextAnchorNode Length=" + length + " TotalLength=" + totalLength + " Target=" + Target?.ToString() + "]";
	}
}
