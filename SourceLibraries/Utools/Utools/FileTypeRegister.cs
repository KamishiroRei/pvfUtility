using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Utools;

public class FileTypeRegister
{
	public static void RegisterFileType(FileTypeRegInfo regInfo)
	{
		RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(regInfo.ExtendName);
		string text = regInfo.ExtendName.Substring(1, regInfo.ExtendName.Length - 1).ToUpper() + "_FileType";
		registryKey.SetValue("", text);
		registryKey.Close();
		RegistryKey registryKey2 = Registry.ClassesRoot.CreateSubKey(text);
		registryKey2.SetValue("", regInfo.Description);
		registryKey2.CreateSubKey("DefaultIcon").SetValue("", regInfo.IconPath);
		registryKey2.CreateSubKey("Shell").CreateSubKey("Open").CreateSubKey("Command")
			.SetValue("", regInfo.ExePath + " %1");
		registryKey2.Close();
	}

	public static bool UpdateFileTypeRegInfo(FileTypeRegInfo regInfo)
	{
		string extendName = regInfo.ExtendName;
		string name = extendName.Substring(1, extendName.Length - 1).ToUpper() + "_FileType";
		RegistryKey? registryKey = Registry.ClassesRoot.OpenSubKey(name, writable: true);
		registryKey.SetValue("", regInfo.Description);
		registryKey.OpenSubKey("DefaultIcon", writable: true).SetValue("", regInfo.IconPath);
		(registryKey.OpenSubKey("Shell").OpenSubKey("Open")?.OpenSubKey("Command", writable: true))?.SetValue("", regInfo.ExePath + " %1");
		registryKey.Close();
		return true;
	}

	public static FileTypeRegInfo GetFileTypeRegInfo(string extendName)
	{
		if (!FileTypeRegistered(extendName))
		{
			return null;
		}
		FileTypeRegInfo fileTypeRegInfo = new FileTypeRegInfo(extendName);
		string name = extendName.Substring(1, extendName.Length - 1).ToUpper() + "_FileType";
		RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(name);
		fileTypeRegInfo.Description = registryKey.GetValue("").ToString();
		RegistryKey registryKey2 = registryKey.OpenSubKey("DefaultIcon");
		fileTypeRegInfo.IconPath = registryKey2.GetValue("").ToString();
		string text = registryKey.OpenSubKey("Shell").OpenSubKey("Open").OpenSubKey("Command")
			.GetValue("")
			.ToString();
		fileTypeRegInfo.ExePath = text.Substring(0, text.Length - 3);
		return fileTypeRegInfo;
	}

	public static bool FileTypeRegistered(string extendName)
	{
		if (Registry.ClassesRoot.OpenSubKey(extendName) != null)
		{
			return true;
		}
		return false;
	}

	[DllImport("shell32.dll")]
	public static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

	private void oPkVxo8RXb()
	{
		if (Registry.GetValue("HKEY_CLASSES_ROOT\\MyApp", string.Empty, string.Empty) == null)
		{
			Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\MyApp", "", "My File Type");
			Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\MyApp", "FriendlyTypeName", "My Friendly Type Name");
			Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\MyApp\\shell\\open\\command", "", "path\\to\\my\\app \"%1\"");
			Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\.ext", "", "MyApp");
			SHChangeNotify(134217728, 8192, IntPtr.Zero, IntPtr.Zero);
		}
	}

	public FileTypeRegister()
	{
	}
}
