using System.Collections.Generic;

namespace Swordfish.NET.Collections;

internal sealed class WeakKeyComparer<T> : IEqualityComparer<object> where T : class
{
	private IEqualityComparer<T> comparer;

	internal WeakKeyComparer(IEqualityComparer<T> comparer)
	{
		if (comparer == null)
		{
			comparer = EqualityComparer<T>.Default;
		}
		this.comparer = comparer;
	}

	public int GetHashCode(object obj)
	{
		if (obj is WeakKeyReference<T> weakKeyReference)
		{
			return weakKeyReference.HashCode;
		}
		return comparer.GetHashCode((T)obj);
	}

	public new bool Equals(object x, object y)
	{
		bool isDead;
		T target = GetTarget(x, out isDead);
		bool isDead2;
		T target2 = GetTarget(y, out isDead2);
		if (isDead)
		{
			if (!isDead2)
			{
				return false;
			}
			return x == y;
		}
		if (isDead2)
		{
			return false;
		}
		return comparer.Equals(target, target2);
	}

	private static T GetTarget(object obj, out bool isDead)
	{
		T result;
		if (obj is WeakKeyReference<T> weakKeyReference)
		{
			result = weakKeyReference.Target;
			isDead = !weakKeyReference.IsAlive;
		}
		else
		{
			result = (T)obj;
			isDead = false;
		}
		return result;
	}
}
