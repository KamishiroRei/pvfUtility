using System;
using System.IO;
using System.Text;

namespace ICSharpCode.AvalonEdit.Utils;

public static class FileReader
{
	private static readonly Encoding UTF8NoBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

	public static bool IsUnicode(Encoding encoding)
	{
		if (encoding == null)
		{
			throw new ArgumentNullException("encoding");
		}
		int codePage = encoding.CodePage;
		if ((uint)(codePage - 1200) <= 1u || (uint)(codePage - 12000) <= 1u || (uint)(codePage - 65000) <= 1u)
		{
			return true;
		}
		return false;
	}

	private static bool IsASCIICompatible(Encoding encoding)
	{
		byte[] bytes = encoding.GetBytes("Az");
		if (bytes.Length == 2 && bytes[0] == 65)
		{
			return bytes[1] == 122;
		}
		return false;
	}

	private static Encoding RemoveBOM(Encoding encoding)
	{
		if (encoding.CodePage == 65001)
		{
			return UTF8NoBOM;
		}
		return encoding;
	}

	public static string ReadFileContent(Stream stream, Encoding defaultEncoding)
	{
		using StreamReader streamReader = OpenStream(stream, defaultEncoding);
		return streamReader.ReadToEnd();
	}

	public static string ReadFileContent(string fileName, Encoding defaultEncoding)
	{
		using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
		return ReadFileContent(stream, defaultEncoding);
	}

	public static StreamReader OpenFile(string fileName, Encoding defaultEncoding)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
		try
		{
			return OpenStream(fileStream, defaultEncoding);
		}
		catch
		{
			fileStream.Dispose();
			throw;
		}
	}

	public static StreamReader OpenStream(Stream stream, Encoding defaultEncoding)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (stream.Position != 0L)
		{
			throw new ArgumentException("stream is not positioned at beginning.", "stream");
		}
		if (defaultEncoding == null)
		{
			throw new ArgumentNullException("defaultEncoding");
		}
		if (stream.Length >= 2)
		{
			int num = stream.ReadByte();
			int num2 = stream.ReadByte();
			switch ((num << 8) | num2)
			{
			case 0:
			case 61371:
			case 65279:
			case 65534:
				stream.Position = 0L;
				return new StreamReader(stream);
			default:
				return AutoDetect(stream, (byte)num, (byte)num2, defaultEncoding);
			}
		}
		if (defaultEncoding != null)
		{
			return new StreamReader(stream, defaultEncoding);
		}
		return new StreamReader(stream);
	}

	private static StreamReader AutoDetect(Stream fs, byte firstByte, byte secondByte, Encoding defaultEncoding)
	{
		int num = (int)Math.Min(fs.Length, 500000L);
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < num; i++)
		{
			byte b = i switch
			{
				0 => firstByte, 
				1 => secondByte, 
				_ => (byte)fs.ReadByte(), 
			};
			if (b < 128)
			{
				if (num2 == 3)
				{
					num2 = 1;
					break;
				}
			}
			else if (b < 192)
			{
				if (num2 != 3)
				{
					num2 = 1;
					break;
				}
				num3--;
				if (num3 < 0)
				{
					num2 = 1;
					break;
				}
				if (num3 == 0)
				{
					num2 = 2;
				}
			}
			else
			{
				if (b < 194 || b >= 245)
				{
					num2 = 1;
					break;
				}
				if (num2 != 2 && num2 != 0)
				{
					num2 = 1;
					break;
				}
				num2 = 3;
				num3 = ((b < 224) ? 1 : ((b >= 240) ? 3 : 2));
			}
		}
		fs.Position = 0L;
		switch (num2)
		{
		case 0:
			return new StreamReader(fs, IsASCIICompatible(defaultEncoding) ? RemoveBOM(defaultEncoding) : Encoding.ASCII);
		case 1:
			if (IsUnicode(defaultEncoding))
			{
				defaultEncoding = Encoding.Default;
			}
			return new StreamReader(fs, RemoveBOM(defaultEncoding));
		default:
			return new StreamReader(fs, UTF8NoBOM);
		}
	}
}
