using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace SettingsModel.Models.XML.Converters;

internal class SecureStringHandler : IAlternativeDataTypeHandler
{
	private static byte[] entropy = Encoding.Unicode.GetBytes("Salt Is Usually Not A Password");

	public Type SourceDataType => typeof(SecureString);

	public Type TargetDataType => typeof(string);

	public object Convert(object objectInput)
	{
		if (!(objectInput is SecureString input))
		{
			return null;
		}
		byte[] array = null;
		try
		{
			array = ProtectedData.Protect(Encoding.Unicode.GetBytes(ToInsecureString(input)), entropy, DataProtectionScope.CurrentUser);
			return System.Convert.ToBase64String(array);
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = 0;
				}
			}
		}
	}

	public object ConvertBack(object objectEncryptedData)
	{
		if (!(objectEncryptedData is string s))
		{
			return null;
		}
		byte[] array = null;
		try
		{
			array = ProtectedData.Unprotect(System.Convert.FromBase64String(s), entropy, DataProtectionScope.CurrentUser);
			return ToSecureString(Encoding.Unicode.GetString(array));
		}
		catch
		{
			return new SecureString();
		}
		finally
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = 0;
				}
			}
		}
	}

	private SecureString ToSecureString(string input)
	{
		SecureString secureString = new SecureString();
		foreach (char c in input)
		{
			secureString.AppendChar(c);
		}
		secureString.MakeReadOnly();
		return secureString;
	}

	private string ToInsecureString(SecureString input)
	{
		_ = string.Empty;
		IntPtr intPtr = Marshal.SecureStringToBSTR(input);
		try
		{
			return Marshal.PtrToStringBSTR(intPtr);
		}
		finally
		{
			Marshal.ZeroFreeBSTR(intPtr);
		}
	}
}
