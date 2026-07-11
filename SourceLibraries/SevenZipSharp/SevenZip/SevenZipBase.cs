using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;

namespace SevenZip;

public abstract class SevenZipBase : MarshalByRefObject
{
	private delegate void EventHandlerDelegate<T>(EventHandler<T> handler, T e) where T : EventArgs;

	private readonly bool _reportErrors;

	private readonly int _uniqueID;

	private static readonly List<int> Identifiers = new List<int>();

	protected internal bool NeedsToBeRecreated;

	private readonly List<Exception> _exceptions = new List<Exception>();

	internal SynchronizationContext Context { get; set; }

	public EventSynchronizationStrategy EventSynchronization { get; set; }

	public int UniqueID => _uniqueID;

	public string Password { get; protected set; }

	internal bool ReportErrors => _reportErrors;

	internal ReadOnlyCollection<Exception> Exceptions => new ReadOnlyCollection<Exception>(_exceptions);

	internal bool HasExceptions => _exceptions.Count > 0;

	[CLSCompliant(false)]
	public static LibraryFeature CurrentLibraryFeatures => SevenZipLibraryManager.CurrentLibraryFeatures;

	internal virtual void SaveContext()
	{
		Context = SynchronizationContext.Current;
		NeedsToBeRecreated = true;
	}

	internal virtual void ReleaseContext()
	{
		Context = null;
		NeedsToBeRecreated = true;
		GC.SuppressFinalize(this);
	}

	internal void OnEvent<T>(EventHandler<T> handler, T e, bool synchronous) where T : EventArgs
	{
		try
		{
			if (handler == null)
			{
				return;
			}
			switch (EventSynchronization)
			{
			case EventSynchronizationStrategy.AlwaysAsynchronous:
				synchronous = false;
				break;
			case EventSynchronizationStrategy.AlwaysSynchronous:
				synchronous = true;
				break;
			}
			if (Context == null)
			{
				handler(this, e);
				return;
			}
			SendOrPostCallback d = delegate(object? obj)
			{
				object[] array = (object[])obj;
				((EventHandler<T>)array[0])(array[1], (T)array[2]);
			};
			if (synchronous)
			{
				Context.Send(d, new object[3] { handler, this, e });
			}
			else
			{
				Context.Post(d, new object[3] { handler, this, e });
			}
		}
		catch (Exception e2)
		{
			AddException(e2);
		}
	}

	private static int GetUniqueID()
	{
		lock (Identifiers)
		{
			Random random = new Random(DateTime.Now.Millisecond);
			int num;
			do
			{
				num = random.Next(int.MaxValue);
			}
			while (Identifiers.Contains(num));
			Identifiers.Add(num);
			return num;
		}
	}

	protected SevenZipBase(string password = "")
	{
		Password = password;
		_reportErrors = true;
		_uniqueID = GetUniqueID();
	}

	~SevenZipBase()
	{
		lock (Identifiers)
		{
			Identifiers.Remove(_uniqueID);
		}
	}

	internal void AddException(Exception e)
	{
		_exceptions.Add(e);
	}

	internal void ClearExceptions()
	{
		_exceptions.Clear();
	}

	internal bool ThrowException(CallbackBase handler, params Exception[] e)
	{
		if (_reportErrors && (handler == null || !handler.Canceled))
		{
			throw e[0];
		}
		return false;
	}

	internal void ThrowUserException()
	{
		if (HasExceptions)
		{
			throw new SevenZipException("The extraction was successful butsome exceptions were thrown in your events. Check UserExceptions for details.");
		}
	}

	internal void CheckedExecute(int hresult, string message, CallbackBase handler)
	{
		if (hresult == 0 && !handler.HasExceptions)
		{
			return;
		}
		if (!handler.HasExceptions)
		{
			if (hresult < -2000000000)
			{
				SevenZipException ex = hresult switch
				{
					-2146233067 => new SevenZipException("Operation is not supported. (0x80131515: E_NOTSUPPORTED)"), 
					-2147024784 => new SevenZipException("There is not enough space on the disk. (0x80070070: ERROR_DISK_FULL)"), 
					-2147024864 => new SevenZipException("The file is being used by another process. (0x80070020: ERROR_SHARING_VIOLATION)"), 
					-2147024882 => new SevenZipException("There is not enough memory (RAM). (0x8007000E: E_OUTOFMEMORY)"), 
					-2147024809 => new SevenZipException("Invalid arguments provided. (0x80070057: E_INVALIDARG)"), 
					-2147467263 => new SevenZipException("Functionality not implemented. (0x80004001: E_NOTIMPL)"), 
					-2147024891 => new SevenZipException("Access is denied. (0x80070005: E_ACCESSDENIED)"), 
					_ => new SevenZipException($"Execution has failed due to an internal SevenZipSharp issue (0x{hresult:x} / {hresult}).\n" + "Please report it to https://github.com/squid-box/SevenZipSharp/issues/, include the release number, 7z version used, and attach the archive."), 
				};
				ThrowException(handler, ex);
			}
			else
			{
				ThrowException(handler, new SevenZipException(message + hresult.ToString(CultureInfo.InvariantCulture) + "."));
			}
		}
		else
		{
			ThrowException(handler, handler.Exceptions[0]);
		}
	}

	public static void SetLibraryPath(string libraryPath)
	{
		SevenZipLibraryManager.SetLibraryPath(libraryPath);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SevenZipBase sevenZipBase))
		{
			return false;
		}
		return _uniqueID == sevenZipBase._uniqueID;
	}

	public override int GetHashCode()
	{
		return _uniqueID;
	}

	public override string ToString()
	{
		string value = "SevenZipBase";
		if (this is SevenZipExtractor)
		{
			value = "SevenZipExtractor";
		}
		if (this is SevenZipCompressor)
		{
			value = "SevenZipCompressor";
		}
		return $"{value} [{_uniqueID}]";
	}
}
