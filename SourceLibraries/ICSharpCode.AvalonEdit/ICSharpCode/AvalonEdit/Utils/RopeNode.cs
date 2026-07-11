using System;
using System.Diagnostics;

namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
internal class RopeNode<T>
{
	internal const int NodeSize = 256;

	internal static readonly RopeNode<T> emptyRopeNode = new RopeNode<T>
	{
		isShared = true,
		contents = new T[256]
	};

	internal RopeNode<T> left;

	internal RopeNode<T> right;

	internal volatile bool isShared;

	internal int length;

	internal byte height;

	internal T[] contents;

	internal int Balance => right.height - left.height;

	[Conditional("DATACONSISTENCYTEST")]
	internal void CheckInvariants()
	{
		if (height == 0)
		{
			_ = contents;
		}
		else
		{
			_ = isShared;
		}
	}

	internal RopeNode<T> Clone()
	{
		if (height == 0)
		{
			if (contents == null)
			{
				return GetContentNode().Clone();
			}
			T[] array = new T[256];
			contents.CopyTo(array, 0);
			return new RopeNode<T>
			{
				length = length,
				contents = array
			};
		}
		return new RopeNode<T>
		{
			left = left,
			right = right,
			length = length,
			height = height
		};
	}

	internal RopeNode<T> CloneIfShared()
	{
		if (isShared)
		{
			return Clone();
		}
		return this;
	}

	internal void Publish()
	{
		if (!isShared)
		{
			if (left != null)
			{
				left.Publish();
			}
			if (right != null)
			{
				right.Publish();
			}
			isShared = true;
		}
	}

	internal static RopeNode<T> CreateFromArray(T[] arr, int index, int length)
	{
		if (length == 0)
		{
			return emptyRopeNode;
		}
		return CreateNodes(length).StoreElements(0, arr, index, length);
	}

	internal static RopeNode<T> CreateNodes(int totalLength)
	{
		return CreateNodes((totalLength + 256 - 1) / 256, totalLength);
	}

	private static RopeNode<T> CreateNodes(int leafCount, int totalLength)
	{
		RopeNode<T> ropeNode = new RopeNode<T>();
		ropeNode.length = totalLength;
		if (leafCount == 1)
		{
			ropeNode.contents = new T[256];
		}
		else
		{
			int num = leafCount / 2;
			int num2 = leafCount - num;
			int num3 = num2 * 256;
			ropeNode.left = CreateNodes(num2, num3);
			ropeNode.right = CreateNodes(num, totalLength - num3);
			ropeNode.height = (byte)(1 + Math.Max(ropeNode.left.height, ropeNode.right.height));
		}
		return ropeNode;
	}

	internal void Rebalance()
	{
		if (left == null)
		{
			return;
		}
		while (Math.Abs(Balance) > 1)
		{
			if (Balance > 1)
			{
				if (right.Balance < 0)
				{
					right = right.CloneIfShared();
					right.RotateRight();
				}
				RotateLeft();
				left.Rebalance();
			}
			else if (Balance < -1)
			{
				if (left.Balance > 0)
				{
					left = left.CloneIfShared();
					left.RotateLeft();
				}
				RotateRight();
				right.Rebalance();
			}
		}
		height = (byte)(1 + Math.Max(left.height, right.height));
	}

	private void RotateLeft()
	{
		RopeNode<T> ropeNode = left;
		RopeNode<T> ropeNode2 = right.left;
		RopeNode<T> ropeNode3 = right.right;
		left = (right.isShared ? new RopeNode<T>() : right);
		left.left = ropeNode;
		left.right = ropeNode2;
		left.length = ropeNode.length + ropeNode2.length;
		left.height = (byte)(1 + Math.Max(ropeNode.height, ropeNode2.height));
		right = ropeNode3;
		left.MergeIfPossible();
	}

	private void RotateRight()
	{
		RopeNode<T> ropeNode = left.left;
		RopeNode<T> ropeNode2 = left.right;
		RopeNode<T> ropeNode3 = right;
		right = (left.isShared ? new RopeNode<T>() : left);
		right.left = ropeNode2;
		right.right = ropeNode3;
		right.length = ropeNode2.length + ropeNode3.length;
		right.height = (byte)(1 + Math.Max(ropeNode2.height, ropeNode3.height));
		left = ropeNode;
		right.MergeIfPossible();
	}

	private void MergeIfPossible()
	{
		if (length <= 256)
		{
			height = 0;
			int num = left.length;
			if (left.isShared)
			{
				contents = new T[256];
				left.CopyTo(0, contents, 0, num);
			}
			else
			{
				contents = left.contents;
			}
			left = null;
			right.CopyTo(0, contents, num, right.length);
			right = null;
		}
	}

	internal RopeNode<T> StoreElements(int index, T[] array, int arrayIndex, int count)
	{
		RopeNode<T> ropeNode = CloneIfShared();
		if (ropeNode.height == 0)
		{
			Array.Copy(array, arrayIndex, ropeNode.contents, index, count);
		}
		else
		{
			if (index + count <= ropeNode.left.length)
			{
				ropeNode.left = ropeNode.left.StoreElements(index, array, arrayIndex, count);
			}
			else if (index >= left.length)
			{
				ropeNode.right = ropeNode.right.StoreElements(index - ropeNode.left.length, array, arrayIndex, count);
			}
			else
			{
				int num = ropeNode.left.length - index;
				ropeNode.left = ropeNode.left.StoreElements(index, array, arrayIndex, num);
				ropeNode.right = ropeNode.right.StoreElements(0, array, arrayIndex + num, count - num);
			}
			ropeNode.Rebalance();
		}
		return ropeNode;
	}

	internal void CopyTo(int index, T[] array, int arrayIndex, int count)
	{
		if (height == 0)
		{
			if (contents == null)
			{
				GetContentNode().CopyTo(index, array, arrayIndex, count);
			}
			else
			{
				Array.Copy(contents, index, array, arrayIndex, count);
			}
		}
		else if (index + count <= left.length)
		{
			left.CopyTo(index, array, arrayIndex, count);
		}
		else if (index >= left.length)
		{
			right.CopyTo(index - left.length, array, arrayIndex, count);
		}
		else
		{
			int num = left.length - index;
			left.CopyTo(index, array, arrayIndex, num);
			right.CopyTo(0, array, arrayIndex + num, count - num);
		}
	}

	internal RopeNode<T> SetElement(int offset, T value)
	{
		RopeNode<T> ropeNode = CloneIfShared();
		if (ropeNode.height == 0)
		{
			ropeNode.contents[offset] = value;
		}
		else
		{
			if (offset < ropeNode.left.length)
			{
				ropeNode.left = ropeNode.left.SetElement(offset, value);
			}
			else
			{
				ropeNode.right = ropeNode.right.SetElement(offset - ropeNode.left.length, value);
			}
			ropeNode.Rebalance();
		}
		return ropeNode;
	}

	internal static RopeNode<T> Concat(RopeNode<T> left, RopeNode<T> right)
	{
		if (left.length == 0)
		{
			return right;
		}
		if (right.length == 0)
		{
			return left;
		}
		if (left.length + right.length <= 256)
		{
			left = left.CloneIfShared();
			right.CopyTo(0, left.contents, left.length, right.length);
			left.length += right.length;
			return left;
		}
		RopeNode<T> ropeNode = new RopeNode<T>();
		ropeNode.left = left;
		ropeNode.right = right;
		ropeNode.length = left.length + right.length;
		ropeNode.Rebalance();
		return ropeNode;
	}

	private RopeNode<T> SplitAfter(int offset)
	{
		RopeNode<T> ropeNode = new RopeNode<T>();
		ropeNode.contents = new T[256];
		ropeNode.length = length - offset;
		Array.Copy(contents, offset, ropeNode.contents, 0, ropeNode.length);
		length = offset;
		return ropeNode;
	}

	internal RopeNode<T> Insert(int offset, RopeNode<T> newElements)
	{
		if (offset == 0)
		{
			return Concat(newElements, this);
		}
		if (offset == length)
		{
			return Concat(this, newElements);
		}
		RopeNode<T> ropeNode = CloneIfShared();
		if (ropeNode.height == 0)
		{
			return Concat(right: ropeNode.SplitAfter(offset), left: Concat(ropeNode, newElements));
		}
		if (offset < ropeNode.left.length)
		{
			ropeNode.left = ropeNode.left.Insert(offset, newElements);
		}
		else
		{
			ropeNode.right = ropeNode.right.Insert(offset - ropeNode.left.length, newElements);
		}
		ropeNode.length += newElements.length;
		ropeNode.Rebalance();
		return ropeNode;
	}

	internal RopeNode<T> Insert(int offset, T[] array, int arrayIndex, int count)
	{
		if (length + count < 256)
		{
			RopeNode<T> ropeNode = CloneIfShared();
			int num = ropeNode.length - offset;
			T[] array2 = ropeNode.contents;
			for (int num2 = num; num2 >= 0; num2--)
			{
				array2[num2 + offset + count] = array2[num2 + offset];
			}
			Array.Copy(array, arrayIndex, array2, offset, count);
			ropeNode.length += count;
			return ropeNode;
		}
		if (height == 0)
		{
			return Insert(offset, CreateFromArray(array, arrayIndex, count));
		}
		RopeNode<T> ropeNode2 = CloneIfShared();
		if (offset < ropeNode2.left.length)
		{
			ropeNode2.left = ropeNode2.left.Insert(offset, array, arrayIndex, count);
		}
		else
		{
			ropeNode2.right = ropeNode2.right.Insert(offset - ropeNode2.left.length, array, arrayIndex, count);
		}
		ropeNode2.length += count;
		ropeNode2.Rebalance();
		return ropeNode2;
	}

	internal RopeNode<T> RemoveRange(int index, int count)
	{
		if (index == 0 && count == length)
		{
			return emptyRopeNode;
		}
		int num = index + count;
		RopeNode<T> ropeNode = CloneIfShared();
		if (ropeNode.height == 0)
		{
			int num2 = ropeNode.length - num;
			for (int i = 0; i < num2; i++)
			{
				ropeNode.contents[index + i] = ropeNode.contents[num + i];
			}
			ropeNode.length -= count;
		}
		else
		{
			if (num <= ropeNode.left.length)
			{
				ropeNode.left = ropeNode.left.RemoveRange(index, count);
			}
			else if (index >= ropeNode.left.length)
			{
				ropeNode.right = ropeNode.right.RemoveRange(index - ropeNode.left.length, count);
			}
			else
			{
				int num3 = ropeNode.left.length - index;
				ropeNode.left = ropeNode.left.RemoveRange(index, num3);
				ropeNode.right = ropeNode.right.RemoveRange(0, count - num3);
			}
			if (ropeNode.left.length == 0)
			{
				return ropeNode.right;
			}
			if (ropeNode.right.length == 0)
			{
				return ropeNode.left;
			}
			ropeNode.length -= count;
			ropeNode.MergeIfPossible();
			ropeNode.Rebalance();
		}
		return ropeNode;
	}

	internal virtual RopeNode<T> GetContentNode()
	{
		throw new InvalidOperationException("Called GetContentNode() on non-FunctionNode.");
	}
}
