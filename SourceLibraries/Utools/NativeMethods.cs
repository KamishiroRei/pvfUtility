using System;
using System.Runtime.InteropServices;

public class NativeMethods
{
	public enum InfoLevel
	{
		UniversalName = 1,
		RemoteName
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct UNIVERSAL_NAME_INFO
	{
		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpUniversalName;
	}

	[DllImport("mpr.dll", CharSet = CharSet.Auto)]
	public static extern int WNetGetUniversalName(string lpLocalPath, InfoLevel dwInfoLevel, ref UNIVERSAL_NAME_INFO lpBuffer, ref int lpBufferSize);

	[DllImport("mpr.dll", CharSet = CharSet.Auto)]
	public static extern int WNetGetUniversalName(string lpLocalPath, InfoLevel dwInfoLevel, IntPtr lpBuffer, ref int lpBufferSize);

	public NativeMethods()
	{
	}
}
