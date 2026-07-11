using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utools;

public static class BytesHelper
{
	public static Stream BytesToStream(this byte[] bytes)
	{
		return new MemoryStream(bytes);
	}

	public static bool EqualBytes(this byte[] bytes1, byte[] bytes2)
	{
		bool flag = bytes1 == null;
		bool flag2 = bytes2 == null;
		if (flag == flag2)
		{
			return true;
		}
		if (flag || flag2)
		{
			return false;
		}
		if (bytes1.Length != bytes2.Length)
		{
			return false;
		}
		for (int i = 0; i < bytes1.Length; i++)
		{
			if (bytes1[i] != bytes2[i])
			{
				return false;
			}
		}
		return true;
	}

	public static async Task<byte[]> TreamToBytes(this Stream stream)
	{
		byte[] bytes = new byte[stream.Length];
		await stream.ReadAsync(bytes.AsMemory(0, bytes.Length));
		stream.Close();
		await stream.DisposeAsync();
		return bytes;
	}

	public static byte[] ToByteArray(Stream input)
	{
		byte[] array = new byte[16384];
		using MemoryStream memoryStream = new MemoryStream();
		int count;
		while ((count = input.Read(array, 0, array.Length)) > 0)
		{
			memoryStream.Write(array, 0, count);
		}
		return memoryStream.ToArray();
	}

	public static byte[] CompressBytes(this byte[] bytes)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
		{
			gZipStream.Write(bytes, 0, bytes.Length);
		}
		return memoryStream.ToArray();
	}

	public static byte[] Decompress(this byte[] bytes)
	{
		using MemoryStream stream = new MemoryStream(bytes);
		using GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress);
		using MemoryStream memoryStream = new MemoryStream();
		gZipStream.CopyTo(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] StreamToBytes(Stream stream)
	{
		byte[] array = new byte[stream.Length];
		stream.Read(array, 0, array.Length);
		stream.Seek(0L, SeekOrigin.Begin);
		return array;
	}

	public static byte[] StringToBytes(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			return Encoding.UTF8.GetBytes(str);
		}
		return null;
	}

	public static Stream StringToStream(string str)
	{
		return StringToBytes(str).BytesToStream();
	}

	public static string BytesToString(byte[] bytes)
	{
		if (bytes == null)
		{
			return string.Empty;
		}
		return Encoding.Default.GetString(bytes);
	}

	public static string BytesToBase64Img(this byte[] bytes)
	{
		try
		{
			string text = Convert.ToBase64String(bytes);
			return "data:image/jpeg;base64," + text;
		}
		catch (Exception)
		{
			return "";
		}
	}

	public static List<byte[]> SplitAry(byte[] ary, int subSize)
	{
		int num = ((ary.Length % subSize == 0) ? (ary.Length / subSize) : (ary.Length / subSize + 1));
		List<byte[]> list = new List<byte[]>();
		for (int i = 0; i < num; i++)
		{
			int count = i * subSize;
			byte[] item = ary.Skip(count).Take(subSize).ToArray();
			list.Add(item);
		}
		return list;
	}

	public static int BytesToInt32(byte[] data)
	{
		if (data.Length < 4)
		{
			return 0;
		}
		int result = 0;
		if (data.Length >= 4)
		{
			byte[] array = new byte[4];
			Buffer.BlockCopy(data, 0, array, 0, 4);
			result = BitConverter.ToInt32(array, 0);
		}
		return result;
	}
}
