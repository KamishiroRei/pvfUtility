using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

namespace Swordfish.NET.Collections;

public class VirtualizingCollectionAsync<T> : VirtualizingCollection<T>, INotifyCollectionChanged where T : class
{
	private readonly SynchronizationContext _synchronizationContext;

	private bool _isLoading;

	private bool _isInitializing;

	protected SynchronizationContext SynchronizationContext => _synchronizationContext;

	public bool IsLoading
	{
		get
		{
			return _isLoading;
		}
		set
		{
			SetProperty(ref _isLoading, value, "IsLoading");
		}
	}

	public bool IsInitializing
	{
		get
		{
			return _isInitializing;
		}
		set
		{
			SetProperty(ref _isInitializing, value, "IsInitializing");
		}
	}

	public event NotifyCollectionChangedEventHandler CollectionChanged;

	public VirtualizingCollectionAsync(IVirtualizingCollectionItemsProvider<T> itemsProvider, int pageSize, int pageTimeout)
		: base(itemsProvider, pageSize, pageTimeout)
	{
		_synchronizationContext = SynchronizationContext.Current;
	}

	protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		this.CollectionChanged?.Invoke(this, e);
	}

	private void FireCollectionReset()
	{
		NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
		OnCollectionChanged(e);
	}

	protected override void LoadCount()
	{
		if (base.Count == 0)
		{
			IsInitializing = true;
		}
		ThreadPool.QueueUserWorkItem(LoadCountWork);
	}

	private void LoadCountWork(object args)
	{
		int num = FetchCount();
		SynchronizationContext.Send(LoadCountCompleted, num);
	}

	protected virtual void LoadCountCompleted(object args)
	{
		int newCount = (int)args;
		TakeNewCount(newCount);
		IsInitializing = false;
	}

	private void TakeNewCount(int newCount)
	{
		if (newCount != base.Count)
		{
			int count = base.Count;
			base.Count = newCount;
			int minIndexToRemove = (count - 1) / base.PageSize;
			TrimIncompletePages(minIndexToRemove);
			FireCollectionReset();
		}
	}

	protected override void LoadPage(int pageIndex, int pageLength)
	{
		IsLoading = true;
		ThreadPool.QueueUserWorkItem(LoadPageWork, new int[2] { pageIndex, pageLength });
	}

	private void LoadPageWork(object state)
	{
		int[] obj = (int[])state;
		int num = obj[0];
		int pageLength = obj[1];
		int count = 0;
		IList<T> list = FetchPage(num, pageLength, out count);
		SynchronizationContext.Send(LoadPageCompleted, new object[3] { num, list, count });
	}

	private void LoadPageCompleted(object state)
	{
		object[] obj = (object[])state;
		int pageIndex = (int)obj[0];
		IList<T> dataItems = (IList<T>)obj[1];
		int newCount = (int)obj[2];
		TakeNewCount(newCount);
		PopulatePage(pageIndex, dataItems);
		IsLoading = false;
	}
}
