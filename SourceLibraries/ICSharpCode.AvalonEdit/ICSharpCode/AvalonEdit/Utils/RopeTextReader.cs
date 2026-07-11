using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.AvalonEdit.Utils;

public sealed class RopeTextReader : TextReader
{
	private Stack<RopeNode<char>> stack = new Stack<RopeNode<char>>();

	private RopeNode<char> currentNode;

	private int indexInsideNode;

	public RopeTextReader(Rope<char> rope)
	{
		if (rope == null)
		{
			throw new ArgumentNullException("rope");
		}
		rope.root.Publish();
		if (rope.Length != 0)
		{
			currentNode = rope.root;
			GoToLeftMostLeaf();
		}
	}

	private void GoToLeftMostLeaf()
	{
		while (currentNode.contents == null)
		{
			if (currentNode.height == 0)
			{
				currentNode = currentNode.GetContentNode();
				continue;
			}
			stack.Push(currentNode.right);
			currentNode = currentNode.left;
		}
	}

	public override int Peek()
	{
		if (currentNode == null)
		{
			return -1;
		}
		return currentNode.contents[indexInsideNode];
	}

	public override int Read()
	{
		if (currentNode == null)
		{
			return -1;
		}
		char result = currentNode.contents[indexInsideNode++];
		if (indexInsideNode >= currentNode.length)
		{
			GoToNextNode();
		}
		return result;
	}

	private void GoToNextNode()
	{
		if (stack.Count == 0)
		{
			currentNode = null;
			return;
		}
		indexInsideNode = 0;
		currentNode = stack.Pop();
		GoToLeftMostLeaf();
	}

	public override int Read(char[] buffer, int index, int count)
	{
		if (currentNode == null)
		{
			return 0;
		}
		int num = currentNode.length - indexInsideNode;
		if (count < num)
		{
			Array.Copy(currentNode.contents, indexInsideNode, buffer, index, count);
			indexInsideNode += count;
			return count;
		}
		Array.Copy(currentNode.contents, indexInsideNode, buffer, index, num);
		GoToNextNode();
		return num;
	}
}
