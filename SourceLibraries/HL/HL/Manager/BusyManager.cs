using System;
using System.Collections.Generic;

namespace HL.Manager;

internal static class BusyManager
{
	public struct BusyLock : IDisposable
	{
		public static readonly BusyLock Failed = new BusyLock(null);

		private readonly List<object> objectList;

		public bool Success => objectList != null;

		internal BusyLock(List<object> objectList)
		{
			this.objectList = objectList;
		}

		public void Dispose()
		{
			if (objectList != null)
			{
				objectList.RemoveAt(objectList.Count - 1);
			}
		}
	}

	[ThreadStatic]
	private static List<object> _activeObjects;

	public static BusyLock Enter(object obj)
	{
		List<object> list = _activeObjects;
		if (list == null)
		{
			list = (_activeObjects = new List<object>());
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == obj)
			{
				return BusyLock.Failed;
			}
		}
		list.Add(obj);
		return new BusyLock(list);
	}
}
