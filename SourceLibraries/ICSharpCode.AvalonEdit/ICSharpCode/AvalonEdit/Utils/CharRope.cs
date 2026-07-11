using System;
using System.IO;

namespace ICSharpCode.AvalonEdit.Utils;

public static class CharRope
{
	public static Rope<char> Create(string text)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		return new Rope<char>(InitFromString(text));
	}

	public static string ToString(this Rope<char> rope, int startIndex, int length)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		if (length == 0)
		{
			return string.Empty;
		}
		char[] array = new char[length];
		rope.CopyTo(startIndex, array, 0, length);
		return new string(array);
	}

	public static void WriteTo(this Rope<char> rope, TextWriter output, int startIndex, int length)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		if (output == null)
		{
			throw new ArgumentNullException("output");
		}
		rope.VerifyRange(startIndex, length);
		rope.root.WriteTo(startIndex, output, length);
	}

	public static void AddText(this Rope<char> rope, string text)
	{
		rope.InsertText(rope.Length, text);
	}

	public static void InsertText(this Rope<char> rope, int index, string text)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		rope.InsertRange(index, text.ToCharArray(), 0, text.Length);
	}

	internal static RopeNode<char> InitFromString(string text)
	{
		if (text.Length == 0)
		{
			return RopeNode<char>.emptyRopeNode;
		}
		RopeNode<char> ropeNode = RopeNode<char>.CreateNodes(text.Length);
		FillNode(ropeNode, text, 0);
		return ropeNode;
	}

	private static void FillNode(RopeNode<char> node, string text, int start)
	{
		if (node.contents != null)
		{
			text.CopyTo(start, node.contents, 0, node.length);
			return;
		}
		FillNode(node.left, text, start);
		FillNode(node.right, text, start + node.left.length);
	}

	internal static void WriteTo(this RopeNode<char> node, int index, TextWriter output, int count)
	{
		if (node.height == 0)
		{
			if (node.contents == null)
			{
				node.GetContentNode().WriteTo(index, output, count);
			}
			else
			{
				output.Write(node.contents, index, count);
			}
		}
		else if (index + count <= node.left.length)
		{
			node.left.WriteTo(index, output, count);
		}
		else if (index >= node.left.length)
		{
			node.right.WriteTo(index - node.left.length, output, count);
		}
		else
		{
			int num = node.left.length - index;
			node.left.WriteTo(index, output, num);
			node.right.WriteTo(0, output, count - num);
		}
	}

	public static int IndexOfAny(this Rope<char> rope, char[] anyOf, int startIndex, int length)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		if (anyOf == null)
		{
			throw new ArgumentNullException("anyOf");
		}
		rope.VerifyRange(startIndex, length);
		while (length > 0)
		{
			Rope<char>.RopeCacheEntry ropeCacheEntry = rope.FindNodeUsingCache(startIndex).PeekOrDefault();
			char[] contents = ropeCacheEntry.node.contents;
			int num = startIndex - ropeCacheEntry.nodeStartIndex;
			int num2 = Math.Min(ropeCacheEntry.node.length, num + length);
			for (int i = startIndex - ropeCacheEntry.nodeStartIndex; i < num2; i++)
			{
				char c = contents[i];
				foreach (char c2 in anyOf)
				{
					if (c == c2)
					{
						return ropeCacheEntry.nodeStartIndex + i;
					}
				}
			}
			length -= num2 - num;
			startIndex = ropeCacheEntry.nodeStartIndex + num2;
		}
		return -1;
	}

	public static int IndexOf(this Rope<char> rope, string searchText, int startIndex, int length, StringComparison comparisonType)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		if (searchText == null)
		{
			throw new ArgumentNullException("searchText");
		}
		rope.VerifyRange(startIndex, length);
		int num = rope.ToString(startIndex, length).IndexOf(searchText, comparisonType);
		if (num < 0)
		{
			return -1;
		}
		return num + startIndex;
	}

	public static int LastIndexOf(this Rope<char> rope, string searchText, int startIndex, int length, StringComparison comparisonType)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		if (searchText == null)
		{
			throw new ArgumentNullException("searchText");
		}
		rope.VerifyRange(startIndex, length);
		int num = rope.ToString(startIndex, length).LastIndexOf(searchText, comparisonType);
		if (num < 0)
		{
			return -1;
		}
		return num + startIndex;
	}
}
