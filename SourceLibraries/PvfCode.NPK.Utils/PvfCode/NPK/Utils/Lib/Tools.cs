using System;
using System.Reflection;

namespace PvfCode.NPK.Utils.Lib;

internal static class Tools
{
	public static object CreateInstance(this Type type, params object[] args)
	{
		return type.Assembly.CreateInstance(type.FullName ?? throw new InvalidOperationException(), ignoreCase: true, BindingFlags.Default, null, args, null, null);
	}
}
