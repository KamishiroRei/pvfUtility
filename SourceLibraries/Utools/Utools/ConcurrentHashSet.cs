using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace Utools;

public sealed class ConcurrentHashSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IDisposable
{
	private readonly ReaderWriterLockSlim Cce91xK3QK;

	private readonly HashSet<T> aMn9s4ZdKW;

	public int Count
	{
		get
		{
			Cce91xK3QK.EnterWriteLock();
			try
			{
				return aMn9s4ZdKW.Count;
			}
			finally
			{
				if (Cce91xK3QK.IsWriteLockHeld)
				{
					Cce91xK3QK.ExitWriteLock();
				}
			}
		}
	}

	public bool IsReadOnly => false;

	public ConcurrentHashSet()
	{
		Cce91xK3QK = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		aMn9s4ZdKW = new HashSet<T>();
	}

	public ConcurrentHashSet(IEqualityComparer<T> comparer)
	{
		Cce91xK3QK = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		aMn9s4ZdKW = new HashSet<T>();
		aMn9s4ZdKW = new HashSet<T>(comparer);
	}

	public ConcurrentHashSet(IEnumerable<T> collection)
	{
		Cce91xK3QK = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		aMn9s4ZdKW = new HashSet<T>();
		aMn9s4ZdKW = new HashSet<T>(collection);
	}

	public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
	{
		Cce91xK3QK = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		aMn9s4ZdKW = new HashSet<T>();
		aMn9s4ZdKW = new HashSet<T>(collection, comparer);
	}

	public ConcurrentHashSet(SerializationInfo info, StreamingContext context)
	{
		Cce91xK3QK = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		aMn9s4ZdKW = new HashSet<T>();
		aMn9s4ZdKW = new HashSet<T>();
		((ISerializable)aMn9s4ZdKW).GetObjectData(info, context);
	}

	public void OnDeserialization(object sender)
	{
		aMn9s4ZdKW.OnDeserialization(sender);
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		aMn9s4ZdKW.GetObjectData(info, context);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<T> GetEnumerator()
	{
		return aMn9s4ZdKW.GetEnumerator();
	}

	public void Add(T item)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			aMn9s4ZdKW.Add(item);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public void UnionWith(IEnumerable<T> other)
	{
		Cce91xK3QK.EnterWriteLock();
		Cce91xK3QK.EnterReadLock();
		try
		{
			aMn9s4ZdKW.UnionWith(other);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
			if (Cce91xK3QK.IsReadLockHeld)
			{
				Cce91xK3QK.ExitReadLock();
			}
		}
	}

	public void IntersectWith(IEnumerable<T> other)
	{
		Cce91xK3QK.EnterWriteLock();
		Cce91xK3QK.EnterReadLock();
		try
		{
			aMn9s4ZdKW.IntersectWith(other);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
			if (Cce91xK3QK.IsReadLockHeld)
			{
				Cce91xK3QK.ExitReadLock();
			}
		}
	}

	public void ExceptWith(IEnumerable<T> other)
	{
		Cce91xK3QK.EnterWriteLock();
		Cce91xK3QK.EnterReadLock();
		try
		{
			aMn9s4ZdKW.ExceptWith(other);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
			if (Cce91xK3QK.IsReadLockHeld)
			{
				Cce91xK3QK.ExitReadLock();
			}
		}
	}

	public void SymmetricExceptWith(IEnumerable<T> other)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			aMn9s4ZdKW.SymmetricExceptWith(other);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public bool IsSubsetOf(IEnumerable<T> other)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			return aMn9s4ZdKW.IsSubsetOf(other);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public bool IsSupersetOf(IEnumerable<T> other)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			return aMn9s4ZdKW.IsSupersetOf(other);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public bool IsProperSupersetOf(IEnumerable<T> other)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			return aMn9s4ZdKW.IsProperSupersetOf(other);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public bool IsProperSubsetOf(IEnumerable<T> other)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			return aMn9s4ZdKW.IsProperSubsetOf(other);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public bool Overlaps(IEnumerable<T> other)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			return aMn9s4ZdKW.Overlaps(other);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public bool SetEquals(IEnumerable<T> other)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			return aMn9s4ZdKW.SetEquals(other);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	bool ISet<T>.Add(T item)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			return aMn9s4ZdKW.Add(item);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public void Clear()
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			aMn9s4ZdKW.Clear();
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public bool Contains(T item)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			return aMn9s4ZdKW.Contains(item);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			aMn9s4ZdKW.CopyTo(array, arrayIndex);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public bool Remove(T item)
	{
		Cce91xK3QK.EnterWriteLock();
		try
		{
			return aMn9s4ZdKW.Remove(item);
		}
		finally
		{
			if (Cce91xK3QK.IsWriteLockHeld)
			{
				Cce91xK3QK.ExitWriteLock();
			}
		}
	}

	public void Dispose()
	{
		d7V9DR6Elr(true);
		GC.SuppressFinalize(this);
	}

	private void d7V9DR6Elr(bool P_0)
	{
		if (P_0 && Cce91xK3QK != null)
		{
			Cce91xK3QK.Dispose();
		}
	}

	~ConcurrentHashSet()
	{
		d7V9DR6Elr(false);
	}
}
