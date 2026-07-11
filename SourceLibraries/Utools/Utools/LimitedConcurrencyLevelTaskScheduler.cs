using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Utools;

public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
{
	[ThreadStatic]
	private static bool vquwrjew4;

	private readonly LinkedList<Task> QSKeCjTZR;

	private readonly int wZ6HKZWOV;

	private int Gxif7q560;

	public sealed override int MaximumConcurrencyLevel => wZ6HKZWOV;

	public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
	{
		QSKeCjTZR = new LinkedList<Task>();
		if (maxDegreeOfParallelism < 1)
		{
			throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
		}
		wZ6HKZWOV = maxDegreeOfParallelism;
	}

	protected sealed override void QueueTask(Task task)
	{
		lock (QSKeCjTZR)
		{
			QSKeCjTZR.AddLast(task);
			if (Gxif7q560 < wZ6HKZWOV)
			{
				Gxif7q560++;
				Rh73XvLkZ();
			}
		}
	}

	private void Rh73XvLkZ()
	{
		ThreadPool.UnsafeQueueUserWorkItem(delegate
		{
			vquwrjew4 = true;
			try
			{
				while (true)
				{
					Task value;
					lock (QSKeCjTZR)
					{
						if (QSKeCjTZR.Count == 0)
						{
							Gxif7q560--;
							break;
						}
						value = QSKeCjTZR.First.Value;
						QSKeCjTZR.RemoveFirst();
					}
					TryExecuteTask(value);
				}
			}
			finally
			{
				vquwrjew4 = false;
			}
		}, null);
	}

	protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
	{
		if (!vquwrjew4)
		{
			return false;
		}
		if (taskWasPreviouslyQueued)
		{
			if (TryDequeue(task))
			{
				return TryExecuteTask(task);
			}
			return false;
		}
		return TryExecuteTask(task);
	}

	protected sealed override bool TryDequeue(Task task)
	{
		lock (QSKeCjTZR)
		{
			return QSKeCjTZR.Remove(task);
		}
	}

	protected sealed override IEnumerable<Task> GetScheduledTasks()
	{
		bool lockTaken = false;
		try
		{
			Monitor.TryEnter(QSKeCjTZR, ref lockTaken);
			if (lockTaken)
			{
				return QSKeCjTZR;
			}
			throw new NotSupportedException();
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(QSKeCjTZR);
			}
		}
	}

	[CompilerGenerated]
	private void qFIEZGrgt(object? P_0)
	{
		vquwrjew4 = true;
		try
		{
			while (true)
			{
				Task value;
				lock (QSKeCjTZR)
				{
					if (QSKeCjTZR.Count == 0)
					{
						Gxif7q560--;
						break;
					}
					value = QSKeCjTZR.First.Value;
					QSKeCjTZR.RemoveFirst();
				}
				TryExecuteTask(value);
			}
		}
		finally
		{
			vquwrjew4 = false;
		}
	}
}
