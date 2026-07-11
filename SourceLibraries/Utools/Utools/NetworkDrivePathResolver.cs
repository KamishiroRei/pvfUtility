using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Utools;

internal static class NetworkDrivePathResolver
{
	[DllImport("mpr.dll", CharSet = CharSet.Unicode, EntryPoint = "WNetGetConnection", SetLastError = true)]
	private static extern int WNetGetConnection([MarshalAs(UnmanagedType.LPTStr)] string localName, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName, ref int length);

	public static string ToUncPath(string path)
	{
		StringBuilder stringBuilder = new StringBuilder(512);
		int capacity = stringBuilder.Capacity;
		if (path.Length > 2 && path[1] == ':')
		{
			char driveLetter = path[0];
			if (((driveLetter >= 'a' && driveLetter <= 'z') || (driveLetter >= 'A' && driveLetter <= 'Z')) && WNetGetConnection(path.Substring(0, 2), stringBuilder, ref capacity) == 0)
			{
				string relativePath = Path.GetFullPath(path).Substring(Path.GetPathRoot(path).Length);
				return Path.Combine(stringBuilder.ToString().TrimEnd(), relativePath);
			}
		}
		return path;
	}
}
