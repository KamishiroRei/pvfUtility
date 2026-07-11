using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PvfCode.NPK.Utils.Models.Enums;

namespace PvfCode.NPK.Utils.Lib;

public static class Colors
{
	public const int Argb1555 = 14;

	public const int Argb4444 = 15;

	public const int Argb8888 = 16;

	public static void ReadColor(Stream stream, int bits, byte[] target, int offset)
	{
		byte[] buf;
		if (bits == 16)
		{
			stream.Read(4, out buf);
			buf.CopyTo(target, offset);
			return;
		}
		byte b = 0;
		byte b2 = 0;
		byte b3 = 0;
		byte b4 = 0;
		stream.Read(2, out buf);
		switch (bits)
		{
		case 14:
			b = (byte)(buf[1] >> 7);
			b2 = (byte)((buf[1] >> 2) & 0x1F);
			b3 = (byte)((buf[0] >> 5) | ((buf[1] & 3) << 3));
			b4 = (byte)(buf[0] & 0x1F);
			b *= 255;
			b2 = (byte)((b2 << 3) | (b2 >> 2));
			b3 = (byte)((b3 << 3) | (b3 >> 2));
			b4 = (byte)((b4 << 3) | (b4 >> 2));
			break;
		case 15:
			b = (byte)(buf[1] & 0xF0);
			b2 = (byte)((buf[1] & 0xF) << 4);
			b3 = (byte)(buf[0] & 0xF0);
			b4 = (byte)((buf[0] & 0xF) << 4);
			break;
		}
		target[offset] = b4;
		target[offset + 1] = b3;
		target[offset + 2] = b2;
		target[offset + 3] = b;
	}

	public static void ReadColor(Stream stream, ColorBits bits, byte[] target, int offset)
	{
		ReadColor(stream, (int)bits, target, offset);
	}

	public static byte[] ReadColor(Stream stream, int bits)
	{
		byte[] array = new byte[4];
		ReadColor(stream, bits, array, 0);
		return array;
	}

	public static byte[] ReadColor(Stream stream, ColorBits bits)
	{
		return ReadColor(stream, (int)bits);
	}

	public static void WriteColor(Stream stream, byte[] data, ColorBits bits)
	{
		if (bits == ColorBits.ARGB_8888)
		{
			stream.Write(data);
			return;
		}
		byte b = data[3];
		byte b2 = data[2];
		byte b3 = data[1];
		byte b4 = data[0];
		int num = 0;
		int num2 = 0;
		switch (bits)
		{
		case ColorBits.ARGB_1555:
			b >>= 7;
			b2 >>= 3;
			b3 >>= 3;
			b4 >>= 3;
			num = (byte)(((b3 & 7) << 5) | b4);
			num2 = (byte)((b << 7) | (b2 << 2) | (b3 >> 3));
			break;
		case ColorBits.ARGB_4444:
			num = b3 | (b4 >> 4);
			num2 = b | (b2 >> 4);
			break;
		}
		stream.WriteByte((byte)num);
		stream.WriteByte((byte)num2);
	}

	public static void WriteColor(this Stream stream, Color color, ColorBits bits)
	{
		byte[] data = new byte[4] { color.B, color.G, color.R, color.A };
		WriteColor(stream, data, bits);
	}

	public static IEnumerable<Color> ReadPalette(Stream stream, int count)
	{
		for (int i = 0; i < count; i++)
		{
			byte[] array = new byte[4];
			stream.Read(array);
			yield return Color.FromArgb(array[3], array[0], array[1], array[2]);
		}
	}

	public static void WritePalette(Stream stream, IEnumerable<Color> table)
	{
		foreach (Color item in table.ToList())
		{
			byte[] array = new byte[4] { item.R, item.G, item.B, item.A };
			stream.Write(array);
		}
	}

	public static string ToHexString(this Color color)
	{
		string text = color.ToArgb().ToString("x2");
		for (int i = text.Length; i < 8; i++)
		{
			text = "0" + text;
		}
		return "#" + text;
	}
}
