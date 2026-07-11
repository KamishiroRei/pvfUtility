using System;
using System.Runtime.InteropServices;

namespace PvfCode.Models.Pvf;

public class ByteHelper
{
	public const ushort COMPRESSION_FORMAT_LZNT1 = 2;

	public const ushort COMPRESSION_ENGINE_MAXIMUM = 256;

	[DllImport("ntdll.dll")]
	public static extern uint RtlGetCompressionWorkSpaceSize(ushort dCompressionFormat, out uint dNeededBufferSize, out uint dUnknown);

	[DllImport("ntdll.dll")]
	public static extern uint RtlCompressBuffer(ushort dCompressionFormat, byte[] dSourceBuffer, int dSourceBufferLength, byte[] dDestinationBuffer, int dDestinationBufferLength, uint dUnknown, out int dDestinationSize, IntPtr dWorkspaceBuffer);

	[DllImport("ntdll.dll")]
	public static extern uint RtlDecompressBuffer(ushort dCompressionFormat, byte[] dDestinationBuffer, int dDestinationBufferLength, byte[] dSourceBuffer, int dSourceBufferLength, out uint dDestinationSize);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr LocalAlloc(int uFlags, IntPtr sizetdwBytes);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern IntPtr LocalFree(IntPtr hMem);

	public static byte[] Decompress(byte[] buffer, int Length)
	{
		byte[] array = new byte[Length * 6];
		uint dNeededBufferSize = 0u;
		uint dUnknown = 0u;
		if (RtlGetCompressionWorkSpaceSize(2, out dNeededBufferSize, out dUnknown) != 0)
		{
			return null;
		}
		if (RtlDecompressBuffer(2, array, array.Length, buffer, Length, out dUnknown) != 0)
		{
			return null;
		}
		Array.Resize(ref array, (int)dUnknown);
		return array;
	}

	public static byte[] Compress(byte[] buffer, int Length)
	{
		byte[] array = new byte[Length * 6];
		uint dNeededBufferSize = 0u;
		uint dUnknown = 0u;
		if (RtlGetCompressionWorkSpaceSize(2, out dNeededBufferSize, out dUnknown) != 0)
		{
			return null;
		}
		int dDestinationSize = 0;
		IntPtr intPtr = LocalAlloc(0, new IntPtr(dNeededBufferSize));
		if (RtlCompressBuffer(2, buffer, Length, array, array.Length, 0u, out dDestinationSize, intPtr) != 0)
		{
			return null;
		}
		LocalFree(intPtr);
		Array.Resize(ref array, (dDestinationSize + 3) & -4);
		return array;
	}

	public ByteHelper()
	{
	}
}
