namespace Swordfish.NET.Collections;

internal sealed class WeakKeyReference<T> : WeakReference<T> where T : class
{
	public readonly int HashCode;

	public WeakKeyReference(T key, WeakKeyComparer<T> comparer)
		: base(key)
	{
		HashCode = comparer.GetHashCode(key);
	}
}
