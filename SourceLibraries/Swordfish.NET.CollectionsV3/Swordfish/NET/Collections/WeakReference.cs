using System;

namespace Swordfish.NET.Collections;

internal class WeakReference<T> : WeakReference where T : class
{
	public new T Target => (T)base.Target;

	public static WeakReference<T> Create(T target)
	{
		if (target == null)
		{
			return WeakNullReference<T>.Singleton;
		}
		return new WeakReference<T>(target);
	}

	protected WeakReference(T target)
		: base(target, trackResurrection: false)
	{
	}
}
