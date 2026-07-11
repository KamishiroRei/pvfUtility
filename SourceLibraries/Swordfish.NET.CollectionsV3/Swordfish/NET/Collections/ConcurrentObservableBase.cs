using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;
using Swordfish.NET.Collections.Auxiliary;

namespace Swordfish.NET.Collections;

[Serializable]
public abstract class ConcurrentObservableBase<T, TInternalCollection> : ExtendedNotifyPropertyChanged, IConcurrentObservableBase<T>, INotifyPropertyChanged, INotifyCollectionChanged, ISerializable where TInternalCollection : class
{
	protected ReaderWriterLockSlim _lock;

	protected TInternalCollection _internalCollection;

	private ThrottledAction _viewChanged;

	public abstract IList<T> CollectionView { get; }

	public abstract int Count { get; }

	public bool AllowDirectBindingToView { get; set; }

	private event NotifyCollectionChangedEventHandler _collectionChanged;

	public event NotifyCollectionChangedEventHandler CollectionChanged
	{
		add
		{
			if (!AllowDirectBindingToView)
			{
				string text = value.Target?.GetType().FullName;
				if (text == "System.Windows.Data.CollectionView" || text == "System.Windows.Data.ListCollectionView")
				{
					string value2 = "";
					throw new ApplicationException($"Collection type={typeof(T).Name}, don't bind directly to {"ConcurrentObservableCollection"}, instead bind to {"ConcurrentObservableCollection"}.CollectionView. {value2}");
				}
			}
			_collectionChanged += value;
		}
		remove
		{
			_collectionChanged -= value;
		}
	}

	protected ConcurrentObservableBase(bool isMultithreaded, TInternalCollection initialCollection)
	{
		_lock = (isMultithreaded ? new ReaderWriterLockSlim() : null);
		_internalCollection = initialCollection;
		_viewChanged = new ThrottledAction(delegate
		{
			RaisePropertyChanged("CollectionView", "Count");
		}, TimeSpan.FromMilliseconds(20.0));
	}

	public IDisposable FreezeUpdates()
	{
		_lock?.EnterReadLock();
		return new AnonDisposable(delegate
		{
			_lock?.ExitReadLock();
		});
	}

	protected void DoWriteNotify(Func<TInternalCollection> write, Func<NotifyCollectionChangedEventArgs> change)
	{
		DoReadWriteNotify(() => 0, (int n) => write(), (int n) => change());
	}

	private TRead BodyReadWriteNotify<TRead>(TRead readValue, Func<TRead, TInternalCollection> write, params Func<TRead, NotifyCollectionChangedEventArgs>[] changes)
	{
		_lock?.EnterWriteLock();
		_internalCollection = write(readValue);
		for (int i = 0; i < changes.Length; i++)
		{
			NotifyCollectionChangedEventArgs e = changes[i](readValue);
			if (e != null)
			{
				OnCollectionChanged(e);
			}
		}
		_lock?.ExitWriteLock();
		_lock?.ExitUpgradeableReadLock();
		_viewChanged.InvokeAction();
		return readValue;
	}

	protected TRead DoReadWriteNotify<TRead>(Func<TRead> read, Func<TRead, TInternalCollection> write, params Func<TRead, NotifyCollectionChangedEventArgs>[] changes)
	{
		_lock?.EnterUpgradeableReadLock();
		TRead readValue = read();
		return BodyReadWriteNotify(readValue, write, changes);
	}

	protected bool DoTestReadWriteNotify<TRead>(Func<bool> test, Func<TRead> read, Func<TRead, TInternalCollection> write, Func<TRead, NotifyCollectionChangedEventArgs> change)
	{
		_lock?.EnterUpgradeableReadLock();
		bool flag = test();
		if (flag)
		{
			TRead readValue = read();
			BodyReadWriteNotify(readValue, write, change);
		}
		else
		{
			_lock?.ExitUpgradeableReadLock();
		}
		return flag;
	}

	protected bool DoTestReadWriteNotify<TRead>(Func<bool> test, Func<TRead> readTrue, Func<TRead, TInternalCollection> writeTrue, Func<TRead, NotifyCollectionChangedEventArgs> changeTrue, Func<TRead> readFalse, Func<TRead, TInternalCollection> writeFalse, Func<TRead, NotifyCollectionChangedEventArgs> changeFalse)
	{
		_lock?.EnterUpgradeableReadLock();
		bool flag = test();
		Func<TRead> func = (flag ? readTrue : readFalse);
		Func<TRead, TInternalCollection> write = (flag ? writeTrue : writeFalse);
		Func<TRead, NotifyCollectionChangedEventArgs> func2 = (flag ? changeTrue : changeFalse);
		TRead readValue = func();
		BodyReadWriteNotify(readValue, write, func2);
		return flag;
	}

	protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs changes)
	{
		try
		{
			this._collectionChanged?.Invoke(this, changes);
		}
		catch (Exception)
		{
		}
	}

	protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("isMultithreaded", _lock != null);
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		GetObjectData(info, context);
	}

	protected ConcurrentObservableBase(SerializationInfo information, StreamingContext context)
		: this(information.GetBoolean("isMultithreaded"), (TInternalCollection)null)
	{
	}
}
