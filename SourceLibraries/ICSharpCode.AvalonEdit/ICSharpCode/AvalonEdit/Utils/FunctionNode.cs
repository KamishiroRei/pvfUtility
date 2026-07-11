using System;

namespace ICSharpCode.AvalonEdit.Utils;

internal sealed class FunctionNode<T> : RopeNode<T>
{
	private Func<Rope<T>> initializer;

	private RopeNode<T> cachedResults;

	public FunctionNode(int length, Func<Rope<T>> initializer)
	{
		base.length = length;
		this.initializer = initializer;
		isShared = true;
	}

	internal override RopeNode<T> GetContentNode()
	{
		lock (this)
		{
			if (cachedResults == null)
			{
				if (initializer == null)
				{
					throw new InvalidOperationException("Trying to load this node recursively; or: a previous call to a rope initializer failed.");
				}
				Func<Rope<T>> func = initializer;
				initializer = null;
				RopeNode<T> root = (func() ?? throw new InvalidOperationException("Rope initializer returned null.")).root;
				root.Publish();
				if (root.length != length)
				{
					throw new InvalidOperationException("Rope initializer returned rope with incorrect length.");
				}
				if (root.height == 0 && root.contents == null)
				{
					cachedResults = root.GetContentNode();
				}
				else
				{
					cachedResults = root;
				}
			}
			return cachedResults;
		}
	}
}
