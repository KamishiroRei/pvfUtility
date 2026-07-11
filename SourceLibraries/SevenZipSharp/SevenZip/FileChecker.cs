using System;
using System.IO;

namespace SevenZip;

internal static class FileChecker
{
	private const int SIGNATURE_SIZE = 21;

	private const int SFX_SCAN_LENGTH = 262144;

	private static bool SpecialDetect(Stream stream, int offset, InArchiveFormat expectedFormat)
	{
		if (stream.Length > offset + 21)
		{
			byte[] array = new byte[21];
			int num = 21;
			int num2 = 0;
			stream.Seek(offset, SeekOrigin.Begin);
			while (num > 0)
			{
				int num3 = stream.Read(array, num2, num);
				num -= num3;
				num2 += num3;
			}
			string text = BitConverter.ToString(array);
			foreach (string key in Formats.InSignatureFormats.Keys)
			{
				if (Formats.InSignatureFormats[key] == expectedFormat && text.StartsWith(key, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static InArchiveFormat CheckSignature(Stream stream, out int offset, out bool isExecutable)
	{
		offset = 0;
		if (!stream.CanRead)
		{
			throw new ArgumentException("The stream must be readable.");
		}
		if (stream.Length < 21)
		{
			throw new ArgumentException("The stream is invalid.");
		}
		byte[] array = new byte[21];
		int num = 21;
		int num2 = 0;
		stream.Seek(0L, SeekOrigin.Begin);
		while (num > 0)
		{
			int num3 = stream.Read(array, num2, num);
			num -= num3;
			num2 += num3;
		}
		string text = BitConverter.ToString(array);
		InArchiveFormat inArchiveFormat = InArchiveFormat.XZ;
		isExecutable = false;
		foreach (string key in Formats.InSignatureFormats.Keys)
		{
			if (text.StartsWith(key, StringComparison.OrdinalIgnoreCase) || (text.Substring(6).StartsWith(key, StringComparison.OrdinalIgnoreCase) && Formats.InSignatureFormats[key] == InArchiveFormat.Lzh))
			{
				if (Formats.InSignatureFormats[key] != InArchiveFormat.PE)
				{
					return Formats.InSignatureFormats[key];
				}
				inArchiveFormat = InArchiveFormat.PE;
				isExecutable = true;
			}
		}
		if (text.StartsWith("D0-CF-11-E0-A1-B1-1A-E1", StringComparison.OrdinalIgnoreCase))
		{
			inArchiveFormat = InArchiveFormat.Cab;
		}
		try
		{
			SpecialDetect(stream, 257, InArchiveFormat.Tar);
		}
		catch (ArgumentException)
		{
		}
		if (SpecialDetect(stream, 32769, InArchiveFormat.Iso))
		{
			return InArchiveFormat.Iso;
		}
		if (SpecialDetect(stream, 34817, InArchiveFormat.Iso))
		{
			return InArchiveFormat.Iso;
		}
		if (SpecialDetect(stream, 36865, InArchiveFormat.Iso))
		{
			return InArchiveFormat.Iso;
		}
		if (SpecialDetect(stream, 36865, InArchiveFormat.Iso))
		{
			return InArchiveFormat.Iso;
		}
		if (SpecialDetect(stream, 1024, InArchiveFormat.Hfs))
		{
			return InArchiveFormat.Hfs;
		}
		if (stream.Length >= 1024)
		{
			stream.Seek(-1024L, SeekOrigin.End);
			byte[] array2 = new byte[1024];
			stream.Read(array2, 0, 1024);
			bool flag = true;
			for (int i = 0; i < 1024; i++)
			{
				flag = flag && array2[i] == 0;
			}
			if (flag)
			{
				return InArchiveFormat.Tar;
			}
		}
		if (inArchiveFormat != InArchiveFormat.XZ)
		{
			long num4 = Math.Min(stream.Length, 262144L);
			array = new byte[num4];
			num = (int)num4;
			num2 = 0;
			stream.Seek(0L, SeekOrigin.Begin);
			while (num > 0)
			{
				int num5 = stream.Read(array, num2, num);
				num -= num5;
				num2 += num5;
			}
			text = BitConverter.ToString(array);
			InArchiveFormat[] array3 = new InArchiveFormat[6]
			{
				InArchiveFormat.Zip,
				InArchiveFormat.SevenZip,
				InArchiveFormat.Rar4,
				InArchiveFormat.Rar,
				InArchiveFormat.Cab,
				InArchiveFormat.Arj
			};
			foreach (InArchiveFormat inArchiveFormat2 in array3)
			{
				int num6 = text.IndexOf(Formats.InSignatureFormatsReversed[inArchiveFormat2]);
				if (num6 > -1)
				{
					offset = num6 / 3;
					return inArchiveFormat2;
				}
			}
			if (inArchiveFormat == InArchiveFormat.PE)
			{
				return InArchiveFormat.PE;
			}
		}
		throw new ArgumentException("The stream is invalid or no corresponding signature was found.");
	}

	public static InArchiveFormat CheckSignature(string fileName, out int offset, out bool isExecutable)
	{
		using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		try
		{
			return CheckSignature(stream, out offset, out isExecutable);
		}
		catch (ArgumentException)
		{
			offset = 0;
			isExecutable = false;
			return Formats.FormatByFileName(fileName, reportErrors: true);
		}
	}
}
