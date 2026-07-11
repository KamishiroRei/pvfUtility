using System;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Utools;

public static class PathsHelper
{
	public static string AppPath => AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

	public static void DirectoryExistCheck(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName) && directoryName != null)
		{
			Directory.CreateDirectory(directoryName);
		}
	}

	public static bool IsPathMatch(string filename, string path)
	{
		if (path.Length <= 0)
		{
			return true;
		}
		if (filename.Length < path.Length)
		{
			return false;
		}
		return filename.Substring(0, path.Length) == path;
	}

	public static bool IsPathMatchEx(string filename, string path)
	{
		if (path.Length <= 0)
		{
			return true;
		}
		if (filename.Length >= path.Length)
		{
			return LikeOperator.LikeString(filename, path, CompareMethod.Binary);
		}
		return false;
	}

	public static string PathFix(string path)
	{
		if (path == null)
		{
			return string.Empty;
		}
		path = path.ToLower().Replace('\\', '/');
		if (path.Length < 1)
		{
			return path;
		}
		if (path[path.Length - 1] != '/')
		{
			path += "/";
		}
		return path;
	}

	public static string PathFixWin(string path)
	{
		if (path.Length < 1)
		{
			return path;
		}
		if (path[path.Length - 1] != '\\')
		{
			path += "\\";
		}
		return path;
	}

	public static string PathFixEx(string path)
	{
		if (path.Length < 1)
		{
			return path.Replace('\\', '/');
		}
		path = path.Replace('\\', '/');
		if (path.IndexOf('*') < 0)
		{
			path += "/*";
		}
		if (path.Substring(path.Length - 1, 1) == "/")
		{
			path += "*";
		}
		return path;
	}

	public static string LefNotFix(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return path;
		}
		if (path[0] == '/')
		{
			return path.Substring(1, path.Length - 1);
		}
		return path;
	}

	public static string RightNotFix(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return path;
		}
		if (path[path.Length - 1] == '/')
		{
			return path.Substring(0, path.Length - 1);
		}
		return path;
	}
}
