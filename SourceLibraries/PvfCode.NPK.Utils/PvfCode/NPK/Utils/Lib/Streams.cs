using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvfCode.NPK.Utils.Lib;

public static class Streams
{
	public static int Read(this Stream stream, byte[] buf)
	{
		return stream.Read(buf, 0, buf.Length);
	}

	public static int Read(this Stream stream, int length, out byte[] buf)
	{
		buf = new byte[length];
		return stream.Read(buf, 0, length);
	}

	public static byte[] Read(this Stream stream, int length)
	{
		byte[] array = new byte[length];
		stream.Read(array, 0, length);
		return array;
	}

	public static void Write(this Stream stream, byte[] buf)
	{
		stream.Write(buf, 0, buf.Length);
	}

	public static void Seek(this Stream stream, long offset)
	{
		stream.Seek(offset, SeekOrigin.Current);
	}

	public static int ReadInt(this Stream stream)
	{
		stream.Read(4, out byte[] buf);
		return BitConverter.ToInt32(buf, 0);
	}

	public static byte[] ReadRange(this Stream stream, params int[] lengths)
	{
		byte[] array = new byte[lengths.Sum()];
		int num = 0;
		foreach (int num2 in lengths)
		{
			int num3 = stream.Read(array, num, num2);
			if (num3 != num2)
			{
				throw new EndOfStreamException($"Expected to read {num2} bytes, but only read {num3} bytes.");
			}
			num += num2;
		}
		return array;
	}

	public static async Task<int> ReadIntAsync(this Stream stream)
	{
		byte[] buf = new byte[4];
		await stream.ReadAsync(buf);
		return BitConverter.ToInt32(buf, 0);
	}

	public static async Task<long> ReadLongAsync(this Stream stream)
	{
		byte[] buffer = new byte[8];
		int num;
		for (int bytesRead = 0; bytesRead < 8; bytesRead += num)
		{
			num = await stream.ReadAsync(buffer.AsMemory(bytesRead, 8 - bytesRead)).ConfigureAwait(continueOnCapturedContext: false);
			if (num == 0)
			{
				throw new EndOfStreamException("Stream ended prematurely");
			}
		}
		return BitConverter.ToInt64(buffer, 0);
	}

	public static uint ReadUInt(this Stream stream)
	{
		stream.Read(4, out byte[] buf);
		return BitConverter.ToUInt32(buf, 0);
	}

	public static void WriteInt(this Stream stream, int data)
	{
		stream.Write(BitConverter.GetBytes(data));
	}

	public static void WriteUInt(this Stream stream, uint data)
	{
		stream.Write(BitConverter.GetBytes(data));
	}

	public static short ReadShort(this Stream stream)
	{
		byte[] array = new byte[2];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToInt16(array, 0);
	}

	public static ushort ReadUShort(this Stream stream)
	{
		byte[] array = new byte[2];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToUInt16(array, 0);
	}

	public static void WriteShort(this Stream stream, short s)
	{
		stream.Write(BitConverter.GetBytes(s));
	}

	public static long ReadLong(this Stream stream)
	{
		byte[] array = new byte[8];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToInt64(array, 0);
	}

	public static void WriteLong(this Stream stream, long l)
	{
		stream.Write(BitConverter.GetBytes(l));
	}

	public static string ReadString(this Stream stream)
	{
		return stream.ReadString(Encoding.Default);
	}

	public static void WriteString(this Stream stream, string str)
	{
		stream.WriteString(str, Encoding.Default, split: true);
	}

	public static string ReadString(this Stream stream, Encoding encoding)
	{
		MemoryStream memoryStream = new MemoryStream();
		int num;
		while ((num = stream.ReadByte()) != 0 && num != -1)
		{
			memoryStream.WriteByte((byte)num);
		}
		memoryStream.Close();
		return encoding.GetString(memoryStream.ToArray());
	}

	public static string ReadString2(this Stream stream, Encoding encoding)
	{
		using BinaryReader binaryReader = new BinaryReader(stream, encoding, leaveOpen: true);
		StringBuilder stringBuilder = new StringBuilder();
		char value;
		while ((value = binaryReader.ReadChar()) != 0)
		{
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	public static async Task<int> ReadByteAsync(this Stream stream)
	{
		byte[] buffer = new byte[1];
		return (await stream.ReadAsync(buffer, 0, 1) > 0) ? buffer[0] : (-1);
	}

	public static async Task<string> ReadNullTerminatedStringAsync(this Stream stream, Encoding encoding)
	{
		byte[] buffer = new byte[1024];
		StringBuilder result = new StringBuilder();
		while (true)
		{
			int num = await stream.ReadAsync(buffer, 0, buffer.Length);
			if (num == 0)
			{
				break;
			}
			int num2 = Array.IndexOf(buffer, (byte)0);
			if (num2 >= 0)
			{
				result.Append(encoding.GetString(buffer, 0, num2));
				break;
			}
			result.Append(encoding.GetString(buffer, 0, num));
		}
		return result.ToString();
	}

	public static void WriteString(this Stream stream, string str, Encoding encoding)
	{
		stream.WriteString(str, encoding, split: true);
	}

	public static void WriteString(this Stream stream, string str, Encoding encoding, bool split)
	{
		stream.Write(encoding.GetBytes(str));
		if (split)
		{
			stream.WriteByte(0);
		}
	}

	public static byte[] ReadToEnd(this Stream stream)
	{
		byte[] array = new byte[stream.Length - stream.Position];
		stream.Read(array, 0, array.Length);
		return array;
	}

	public static void ReadToEnd(this Stream stream, out byte[] buf)
	{
		buf = new byte[stream.Length - stream.Position];
		stream.Read(buf, 0, buf.Length);
	}
}
