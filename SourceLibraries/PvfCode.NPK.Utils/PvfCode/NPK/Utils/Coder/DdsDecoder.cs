using System;
using System.IO;
using PvfCode.NPK.Utils.Lib;

namespace PvfCode.NPK.Utils.Coder;

public class DdsDecoder
{
	private const int Dxt1 = 827611204;

	private const int Dxt3 = 861165636;

	private const int Dxt5 = 894720068;

	private const int DdsMagic = 542327876;

	private const int DdsMipmapCount = 131072;

	public static DdsTexture Decode(byte[] data)
	{
		using MemoryStream stream = new MemoryStream(data);
		return Decode(stream);
	}

	public static DdsTexture Decode(Stream stream)
	{
		DdsTexture ddsTexture = new DdsTexture();
		if (stream.ReadInt() != 542327876)
		{
			throw new Exception("Invalid magic number in DDS header");
		}
		ddsTexture.Length = stream.ReadInt();
		ddsTexture.Flags = stream.ReadInt();
		ddsTexture.Width = stream.ReadInt();
		ddsTexture.Height = stream.ReadInt();
		ddsTexture.Pitch = stream.ReadInt();
		ddsTexture.Depth = stream.ReadInt();
		int val = stream.ReadInt();
		ddsTexture.Reverse = stream.Read(11);
		stream.Seek(37L);
		if ((stream.ReadInt() & 0x20000) != 0)
		{
			ddsTexture.Count = Math.Max(1, val);
		}
		int num = stream.ReadInt();
		int num2 = 0;
		switch (num)
		{
		case 827611204:
			num2 = 8;
			ddsTexture.Format = DdsFormat.RgbS3TcDxt1Format;
			break;
		case 861165636:
			num2 = 16;
			ddsTexture.Format = DdsFormat.RgbaS3TcDxt3Format;
			break;
		case 894720068:
			num2 = 16;
			ddsTexture.Format = DdsFormat.RgbaS3TcDxt5Format;
			break;
		}
		int num3 = ddsTexture.Length + 4;
		stream.Seek(num3, SeekOrigin.Begin);
		int num4 = ddsTexture.Width;
		int num5 = ddsTexture.Height;
		int length = num4 * num5 / 16 * num2;
		ddsTexture.DdsMipmaps = new DdsMipmap[ddsTexture.Count];
		for (int i = 0; i < ddsTexture.Count; i++)
		{
			stream.Read(length, out byte[] buf);
			switch (num)
			{
			case 827611204:
				buf = DecodeDxt1(buf, num4, num5);
				break;
			case 861165636:
				buf = DecodeDxt3(buf, num4, num5);
				break;
			case 894720068:
				buf = DecodeDxt5(buf, num4, num5);
				break;
			}
			DdsMipmap ddsMipmap = new DdsMipmap
			{
				Width = num4,
				Height = num5,
				Data = buf
			};
			ddsTexture.DdsMipmaps[i] = ddsMipmap;
			num4 = Math.Max(num4 >> 1, 1);
			num5 = Math.Max(num5 >> 1, 1);
		}
		return ddsTexture;
	}

	public static byte[] DecodeDxt1(byte[] data, int width, int height)
	{
		MemoryStream memoryStream = new MemoryStream(data);
		byte[] array = new byte[width * height * 4];
		for (int i = 0; i < width; i += 4)
		{
			for (int j = 0; j < height; j += 4)
			{
				ushort num = memoryStream.ReadUShort();
				ushort num2 = memoryStream.ReadUShort();
				byte[][] array2 = new byte[4][]
				{
					DecodeRgb565(num),
					DecodeRgb565(num2),
					new byte[4],
					new byte[4]
				};
				if (num > num2)
				{
					array2[2][0] = (byte)((array2[0][0] * 2 + array2[1][0]) / 3);
					array2[2][1] = (byte)((array2[0][1] * 2 + array2[1][1]) / 3);
					array2[2][2] = (byte)((array2[0][2] * 2 + array2[1][2]) / 3);
					array2[2][3] = byte.MaxValue;
					array2[3][0] = (byte)((array2[0][0] + array2[1][0] * 2) / 3);
					array2[3][1] = (byte)((array2[0][1] + array2[1][1] * 2) / 3);
					array2[3][2] = (byte)((array2[0][2] + array2[1][2] * 2) / 3);
					array2[3][3] = byte.MaxValue;
				}
				else
				{
					array2[2][0] = (byte)((array2[0][0] + array2[1][0]) / 2);
					array2[2][1] = (byte)((array2[0][1] + array2[1][1]) / 2);
					array2[2][2] = (byte)((array2[0][2] + array2[1][2]) / 2);
					array2[2][3] = (byte)((array2[0][3] + array2[1][3]) / 2);
				}
				int num3 = memoryStream.ReadInt();
				int num4 = 0;
				while (num4 < 16)
				{
					int num5 = num3 & 3;
					int num6 = 4 * (height * (i + num4 / 4) + j + num4 % 4);
					array[num6] = array2[num5][0];
					array[num6 + 1] = array2[num5][1];
					array[num6 + 2] = array2[num5][2];
					array[num6 + 3] = array2[num5][3];
					num4++;
					num3 >>= 2;
				}
			}
		}
		memoryStream.Close();
		return array;
	}

	public static byte[] DecodeDxt3(byte[] data, int width, int height)
	{
		MemoryStream memoryStream = new MemoryStream(data);
		byte[] array = new byte[width * height * 4];
		for (int i = 0; i < width; i += 4)
		{
			for (int j = 0; j < height; j += 4)
			{
				ushort[] array2 = new ushort[4];
				ushort color = memoryStream.ReadUShort();
				ushort color2 = memoryStream.ReadUShort();
				int num = memoryStream.ReadInt();
				array2[0] = memoryStream.ReadUShort();
				array2[1] = memoryStream.ReadUShort();
				array2[2] = memoryStream.ReadUShort();
				array2[3] = memoryStream.ReadUShort();
				byte[] array3 = DecodeRgb565(color);
				byte[] array4 = DecodeRgb565(color2);
				byte[] array5 = new byte[4];
				byte[] array6 = new byte[4];
				array5[0] = (byte)((array3[0] * 2 + array4[0]) / 3);
				array5[1] = (byte)((array3[1] * 2 + array4[1]) / 3);
				array5[2] = (byte)((array3[2] * 2 + array4[2]) / 3);
				array6[0] = (byte)((array3[0] + array4[0] * 2) / 3);
				array6[1] = (byte)((array3[1] + array4[1] * 2) / 3);
				array6[2] = (byte)((array3[2] + array4[2] * 2) / 3);
				byte[][] array7 = new byte[4][] { array3, array4, array5, array6 };
				int num2 = 0;
				while (num2 < 16)
				{
					int num3 = num & 3;
					int num4 = 4 * (height * (i + num2 / 4) + j + num2 % 4);
					byte b = (byte)(array2[num2 / 4] & 0xF);
					array[num4] = array7[num3][0];
					array[num4 + 1] = array7[num3][1];
					array[num4 + 2] = array7[num3][2];
					array[num4 + 3] = (byte)(b | (b << 4));
					num2++;
					num >>= 2;
				}
			}
		}
		memoryStream.Close();
		return array;
	}

	public static byte[] DecodeDxt5(byte[] data, int width, int height)
	{
		MemoryStream memoryStream = new MemoryStream(data);
		byte[] array = new byte[width * height * 4];
		for (int i = 0; i < width; i += 4)
		{
			for (int j = 0; j < height; j += 4)
			{
				byte[] array2 = new byte[8]
				{
					(byte)memoryStream.ReadByte(),
					(byte)memoryStream.ReadByte(),
					0,
					0,
					0,
					0,
					0,
					0
				};
				if (array2[0] > array2[1])
				{
					array2[2] = (byte)((6 * array2[0] + array2[1]) / 7);
					array2[3] = (byte)((5 * array2[0] + 2 * array2[1]) / 7);
					array2[4] = (byte)((4 * array2[0] + 3 * array2[1]) / 7);
					array2[5] = (byte)((3 * array2[0] + 4 * array2[1]) / 7);
					array2[6] = (byte)((2 * array2[0] + 5 * array2[1]) / 7);
					array2[7] = (byte)((array2[0] + 6 * array2[1]) / 7);
				}
				else
				{
					array2[2] = (byte)((4 * array2[0] + array2[1]) / 5);
					array2[3] = (byte)((3 * array2[0] + 2 * array2[1]) / 5);
					array2[4] = (byte)((2 * array2[0] + 3 * array2[1]) / 5);
					array2[5] = (byte)((array2[0] + 4 * array2[1]) / 5);
					array2[6] = 0;
					array2[7] = byte.MaxValue;
				}
				ulong num = Read6Byte(memoryStream);
				ushort color = memoryStream.ReadUShort();
				ushort color2 = memoryStream.ReadUShort();
				byte[][] array3 = new byte[4][]
				{
					DecodeRgb565(color),
					DecodeRgb565(color2),
					new byte[4],
					new byte[4]
				};
				for (int k = 0; k < 3; k++)
				{
					array3[2][k] = (byte)((array3[0][k] * 2 + array3[1][k]) / 3);
					array3[3][k] = (byte)((array3[0][k] + array3[1][k] * 2) / 3);
				}
				int num2 = memoryStream.ReadInt();
				int num3 = 0;
				while (num3 < 16)
				{
					int num4 = num2 & 3;
					int num5 = (height * (i + num3 / 4) + j + num3 % 4) * 4;
					byte b = array2[num & 7];
					array[num5] = array3[num4][0];
					array[num5 + 1] = array3[num4][1];
					array[num5 + 2] = array3[num4][2];
					array[num5 + 3] = b;
					num3++;
					num2 >>= 2;
					num >>= 3;
				}
			}
		}
		memoryStream.Close();
		return array;
	}

	public static byte[] DecodeRgb565(ushort color)
	{
		byte b = (byte)(color & 0x1F);
		byte b2 = (byte)((color >> 5) & 0x3F);
		byte b3 = (byte)((color >> 11) & 0x1F);
		byte b4 = byte.MaxValue;
		b = (byte)((b << 3) | (b >> 2));
		b2 = (byte)((b2 << 2) | (b2 >> 4));
		b3 = (byte)((b3 << 3) | (b3 >> 2));
		return new byte[4] { b, b2, b3, b4 };
	}

	public static ulong Read6Byte(Stream ms)
	{
		byte[] array = new byte[8];
		ms.Read(array, 0, 6);
		return BitConverter.ToUInt64(array, 0);
	}
}
