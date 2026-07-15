using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Utools;

public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
{
	[ThreadStatic]
	private static bool currentThreadIsProcessingItems;

	private readonly LinkedList<Task> tasks;

	private readonly int maxDegreeOfParallelism;

	private int delegatesQueuedOrRunning;

	public sealed override int MaximumConcurrencyLevel => maxDegreeOfParallelism;

	public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
	{
		tasks = new LinkedList<Task>();
		if (maxDegreeOfParallelism < 1)
		{
			throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
		}
		this.maxDegreeOfParallelism = maxDegreeOfParallelism;
	}

	protected sealed override void QueueTask(Task task)
	{
		lock (tasks)
		{
			tasks.AddLast(task);
			if (delegatesQueuedOrRunning < maxDegreeOfParallelism)
			{
				delegatesQueuedOrRunning++;
				NotifyThreadPoolOfPendingWork();
			}
		}
	}

	private void NotifyThreadPoolOfPendingWork()
	{
		ThreadPool.UnsafeQueueUserWorkItem(delegate
		{
			currentThreadIsProcessingItems = true;
			try
			{
				while (true)
				{
					Task value;
					lock (tasks)
					{
						if (tasks.Count == 0)
						{
							delegatesQueuedOrRunning--;
							break;
						}
						value = tasks.First.Value;
						tasks.RemoveFirst();
					}
					TryExecuteTask(value);
				}
			}
			finally
			{
				currentThreadIsProcessingItems = false;
			}
		}, null);
	}

	protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
	{
		if (!currentThreadIsProcessingItems)
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
		lock (tasks)
		{
			return tasks.Remove(task);
		}
	}

	protected sealed override IEnumerable<Task> GetScheduledTasks()
	{
		bool lockTaken = false;
		try
		{
			Monitor.TryEnter(tasks, ref lockTaken);
			if (lockTaken)
			{
				return tasks;
			}
			throw new NotSupportedException();
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(tasks);
			}
		}
	}
}
