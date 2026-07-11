using System;
using System.Runtime.InteropServices;
using Ionic.Zlib;

namespace PvfCode.NPK.Utils.Lib;

public static class Zlib
{
	public static byte[] Compress(byte[] data)
	{
		int destLen = (int)((double)data.LongLength * 1.001 + 12.0);
		byte[] array = new byte[destLen];
		Compress(array, ref destLen, data, data.Length);
		byte[] array2 = new byte[destLen];
		Buffer.BlockCopy(array, 0, array2, 0, destLen);
		return array2;
	}

	public static byte[] Decompress(byte[] data, int size)
	{
		return ZlibStream.UncompressBuffer(data);
	}

	[DllImport("zlib64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "compress")]
	private static extern int Compress([In][Out] byte[] dest, ref int destLen, byte[] source, int sourceLen);

	[DllImport("zlib64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "uncompress")]
	private static extern int Decompress([In][Out] byte[] dest, ref int destLen, byte[] source, int sourceLen);
}
