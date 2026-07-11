using System;
using System.Windows;

namespace ICSharpCode.AvalonEdit.Utils;

public abstract class WeakEventManagerBase<TManager, TEventSource> : WeakEventManager where TManager : WeakEventManagerBase<TManager, TEventSource>, new() where TEventSource : class
{
	protected static TManager CurrentManager
	{
		get
		{
			Type typeFromHandle = typeof(TManager);
			TManager val = (TManager)WeakEventManager.GetCurrentManager(typeFromHandle);
			if (val == null)
			{
				val = new TManager();
				WeakEventManager.SetCurrentManager(typeFromHandle, val);
			}
			return val;
		}
	}

	public static void AddListener(TEventSource source, IWeakEventListener listener)
	{
		CurrentManager.ProtectedAddListener(source, listener);
	}

	public static void RemoveListener(TEventSource source, IWeakEventListener listener)
	{
		CurrentManager.ProtectedRemoveListener(source, listener);
	}

	protected sealed override void StartListening(object source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		StartListening((TEventSource)source);
	}

	protected sealed override void StopListening(object source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		StopListening((TEventSource)source);
	}

	protected abstract void StartListening(TEventSource source);

	protected abstract void StopListening(TEventSource source);
}
