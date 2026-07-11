using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Utools;

public static class FileHelper
{
	public static string CountSize(long Size)
	{
		string result = "";
		long num = Size;
		if ((double)num < 1024.0)
		{
			result = num.ToString("F2") + "K";
		}
		else if ((double)num >= 1024.0 && num < 1048576)
		{
			result = ((double)num / 1024.0).ToString("F2") + "KB";
		}
		else if (num >= 1048576 && num < 1073741824)
		{
			result = ((double)num / 1024.0 / 1024.0).ToString("F2") + "MB";
		}
		else if (num >= 1073741824)
		{
			result = ((double)num / 1024.0 / 1024.0 / 1024.0).ToString("F2") + "GB";
		}
		return result;
	}

	public static string CountSize(object obj)
	{
		try
		{
			return CountSize(Convert.ToInt64(obj));
		}
		catch (Exception)
		{
			return "/";
		}
	}

	public static bool CheckDir(string Dir_path)
	{
		try
		{
			if (!Directory.Exists(Dir_path))
			{
				Directory.CreateDirectory(Dir_path);
				return true;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	[DllImport("shell32.dll", SetLastError = true)]
	public static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In][MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

	[DllImport("shell32.dll", SetLastError = true)]
	public static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, out IntPtr pidl, uint sfgaoIn, out uint psfgaoOut);

	public static void OpenFolderAndSelectItem(string folderPath, string file)
	{
		SHParseDisplayName(folderPath, IntPtr.Zero, out var pidl, 0u, out var psfgaoOut);
		if (!(pidl == IntPtr.Zero))
		{
			SHParseDisplayName(Path.Combine(folderPath, file), IntPtr.Zero, out var pidl2, 0u, out psfgaoOut);
			IntPtr[] array = ((!(pidl2 == IntPtr.Zero)) ? new IntPtr[1] { pidl2 } : new IntPtr[0]);
			SHOpenFolderAndSelectItems(pidl, (uint)array.Length, array, 0u);
			Marshal.FreeCoTaskMem(pidl);
			if (pidl2 != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(pidl2);
			}
		}
	}

	public static void OpenFolderAndSelectFile(string fileFullName)
	{
		OpenFolderAndSelectItem(Path.GetDirectoryName(fileFullName), Path.GetFileName(fileFullName));
	}

	public static void OpenFile(string filePathAndName, bool isWaitFileClose = true)
	{
		Process process = new Process();
		ProcessStartInfo startInfo = new ProcessStartInfo(filePathAndName);
		process.StartInfo = startInfo;
		process.StartInfo.UseShellExecute = true;
		try
		{
			process.Start();
			if (isWaitFileClose)
			{
				process.WaitForExit();
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			process?.Close();
		}
	}

	public static bool IsFileInUse(string fileName)
	{
		bool result = true;
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
			result = false;
		}
		catch
		{
		}
		finally
		{
			fileStream?.Close();
		}
		return result;
	}
}
