using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SevenZip;

internal class CallbackBase : MarshalByRefObject
{
	private readonly string _password;

	private readonly bool _reportErrors;

	private readonly List<Exception> _exceptions = new List<Exception>();

	public string Password => _password;

	public bool Canceled { get; set; }

	public bool ReportErrors => _reportErrors;

	public ReadOnlyCollection<Exception> Exceptions => new ReadOnlyCollection<Exception>(_exceptions);

	public bool HasExceptions => _exceptions.Count > 0;

	protected CallbackBase()
	{
		_password = "";
		_reportErrors = true;
	}

	protected CallbackBase(string password)
	{
		if (string.IsNullOrEmpty(password))
		{
			throw new SevenZipException("Empty password was specified.");
		}
		_password = password;
		_reportErrors = true;
	}

	public void AddException(Exception e)
	{
		_exceptions.Add(e);
	}

	public void ClearExceptions()
	{
		_exceptions.Clear();
	}

	public bool ThrowException(CallbackBase handler, params Exception[] e)
	{
		if (_reportErrors && (handler == null || !handler.Canceled))
		{
			throw e[0];
		}
		return false;
	}

	public bool ThrowException()
	{
		if (HasExceptions && _reportErrors)
		{
			throw _exceptions[0];
		}
		return true;
	}

	public void ThrowUserException()
	{
		if (HasExceptions)
		{
			throw new SevenZipException("The extraction was successful butsome exceptions were thrown in your events. Check UserExceptions for details.");
		}
	}
}
