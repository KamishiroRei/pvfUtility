using System;
using System.Diagnostics;

namespace Swordfish.NET.Collections.Auxiliary;

internal static class Utils
{
	[DebuggerStepThrough]
	public static void RequireNotNullOrEmpty(string stringParameter, string parameterName)
	{
		if (stringParameter == null)
		{
			throw new ArgumentNullException(parameterName);
		}
		if (stringParameter.Length == 0)
		{
			throw new ArgumentOutOfRangeException(parameterName);
		}
	}

	[DebuggerStepThrough]
	public static void RequireNotNull(object obj, string parameterName)
	{
		if (obj == null)
		{
			throw new ArgumentNullException(parameterName);
		}
	}

	[DebuggerStepThrough]
	public static void RequireArgument(bool truth, string parameterName)
	{
		RequireNotNullOrEmpty(parameterName, "parameterName");
		if (!truth)
		{
			throw new ArgumentException(parameterName);
		}
	}

	[DebuggerStepThrough]
	public static void RequireArgument(bool truth, string paramName, string message)
	{
		RequireNotNullOrEmpty(paramName, "paramName");
		RequireNotNullOrEmpty(message, "message");
		if (!truth)
		{
			throw new ArgumentException(message, paramName);
		}
	}

	[DebuggerStepThrough]
	public static void RequireArgumentRange(bool truth, string parameterName)
	{
		RequireNotNullOrEmpty(parameterName, "parameterName");
		if (!truth)
		{
			throw new ArgumentOutOfRangeException(parameterName);
		}
	}

	[DebuggerStepThrough]
	public static void RequireArgumentRange(bool truth, string paramName, string message)
	{
		RequireNotNullOrEmpty(paramName, "paramName");
		RequireNotNullOrEmpty(message, "message");
		if (!truth)
		{
			throw new ArgumentOutOfRangeException(message, paramName);
		}
	}
}
