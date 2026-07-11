using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SettingsModel.ExtensionMethods;

internal static class SecureStringExtensionMethod
{
	public static string ConvertToUnsecureString(this SecureString securePassword)
	{
		if (securePassword == null)
		{
			throw new ArgumentNullException("securePassword");
		}
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
			return Marshal.PtrToStringUni(intPtr);
		}
		finally
		{
			Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
		}
	}

	public static SecureString ConvertToSecureString(this string password)
	{
		if (password == null)
		{
			throw new ArgumentNullException("password");
		}
		SecureString secureString = new SecureString();
		foreach (char c in password)
		{
			secureString.AppendChar(c);
		}
		secureString.MakeReadOnly();
		return secureString;
	}
}
