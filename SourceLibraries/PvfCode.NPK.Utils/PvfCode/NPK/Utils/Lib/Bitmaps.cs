using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PvfCode.NPK.Utils.Coder.GIF;
using PvfCode.NPK.Utils.Models.Enums;

namespace PvfCode.NPK.Utils.Lib;

public static class Bitmaps
{
	public static Rectangle Scan(this Bitmap bmp)
	{
		int num = bmp.Height;
		int num2 = 0;
		int num3 = bmp.Width;
		int num4 = 0;
		byte[] array = bmp.ToArray();
		for (int i = 0; i < bmp.Width; i++)
		{
			for (int j = 0; j < bmp.Height; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					if (array[(j * bmp.Width + i) * 4 + k] != 0)
					{
						num = ((num > j) ? j : num);
						num3 = ((num3 > i) ? i : num3);
						num2 = ((num2 < j) ? j : num2);
						num4 = ((num4 < i) ? i : num4);
						break;
					}
				}
			}
		}
		int width = Math.Abs(num4 - num3 + 1);
		int height = Math.Abs(num2 - num + 1);
		return new Rectangle(num3, num, width, height);
	}

	public static Bitmap Canvas(this Bitmap bmp, Rectangle rect)
	{
		Bitmap bitmap = new Bitmap(rect.Width, rect.Height);
		using Graphics graphics = Graphics.FromImage(bitmap);
		graphics.DrawImage(bmp, rect.X, rect.Y);
		return bitmap;
	}

	public static Bitmap Trim(this Bitmap bmp)
	{
		Rectangle srcRect = bmp.Scan();
		Bitmap bitmap = new Bitmap(srcRect.Width, srcRect.Height);
		Graphics graphics = Graphics.FromImage(bitmap);
		Rectangle destRect = new Rectangle(Point.Empty, srcRect.Size);
		graphics.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
		graphics.Dispose();
		return bitmap;
	}

	public static Bitmap LinearDodge(this Bitmap bmp)
	{
		byte[] array = bmp.ToArray();
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = Math.Max(array[i], Math.Max(array[i + 1], array[i + 2]));
			byte b2 = (byte)(255 - b);
			array[i + 3] = Math.Min(array[i + 3], b);
			array[i + 2] += b2;
			array[i + 1] += b2;
			array[i] += b2;
		}
		bmp = array.FromArray(bmp.Size);
		return bmp;
	}

	public static void LinearBrun(this byte[] bytes)
	{
		for (int i = 0; i < bytes.Length; i += 4)
		{
			byte b = Math.Max(bytes[i], Math.Max(bytes[i + 1], bytes[i + 2]));
			byte b2 = (byte)(255 - b);
			bytes[i + 3] = Math.Min(bytes[i + 3], b);
			bytes[i + 2] += b2;
			bytes[i + 1] += b2;
			bytes[i] += b2;
		}
	}

	public static Bitmap LinearBrun(this Bitmap bmp)
	{
		byte[] array = bmp.ToArray();
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = Math.Min(array[i], Math.Max(array[i + 1], array[i + 2]));
			array[i + 3] = Math.Max(array[i + 3], b);
			array[i + 2] = Math.Max((byte)0, (byte)(array[i + 2] - b));
			array[i + 1] = Math.Max((byte)0, (byte)(array[i + 1] - b));
			array[i] = Math.Max((byte)0, (byte)(array[i + 2] - b));
		}
		bmp = array.FromArray(bmp.Size);
		return bmp;
	}

	public static Bitmap Dye(this Bitmap bmp, System.Drawing.Color color)
	{
		byte[] array = bmp.ToArray();
		for (int i = 0; i < array.Length; i += 4)
		{
			byte val = array[i + 3];
			byte val2 = array[i + 2];
			byte val3 = array[i + 1];
			byte val4 = array[i];
			val = (byte)((double)(int)Math.Min(val, color.A) / ((double)(int)color.A + 1.0) * (double)(int)color.A);
			val2 = (byte)((double)(int)Math.Min(val2, color.R) / ((double)(int)color.R + 1.0) * (double)(int)color.R);
			val3 = (byte)((double)(int)Math.Min(val3, color.G) / ((double)(int)color.G + 1.0) * (double)(int)color.G);
			val4 = (byte)((double)(int)Math.Min(val4, color.B) / ((double)(int)color.B + 1.0) * (double)(int)color.B);
			array[i + 3] = val;
			array[i + 2] = val2;
			array[i + 1] = val3;
			array[i] = val4;
		}
		bmp = array.FromArray(bmp.Size);
		return bmp;
	}

	public static byte[] ImageDye(this byte[] data, System.Windows.Media.Color color)
	{
		for (int i = 0; i < data.Length; i += 4)
		{
			byte val = data[i + 3];
			byte val2 = data[i + 2];
			byte val3 = data[i + 1];
			byte val4 = data[i];
			val = (byte)((double)(int)Math.Min(val, color.A) / ((double)(int)color.A + 1.0) * (double)(int)color.A);
			val2 = (byte)((double)(int)Math.Min(val2, color.R) / ((double)(int)color.R + 1.0) * (double)(int)color.R);
			val3 = (byte)((double)(int)Math.Min(val3, color.G) / ((double)(int)color.G + 1.0) * (double)(int)color.G);
			val4 = (byte)((double)(int)Math.Min(val4, color.B) / ((double)(int)color.B + 1.0) * (double)(int)color.B);
			data[i + 3] = val;
			data[i + 2] = val2;
			data[i + 1] = val3;
			data[i] = val4;
		}
		return data;
	}

	public static Bitmap Star(this Bitmap bmp, decimal scale)
	{
		Size size = bmp.Size.Star(scale);
		Bitmap bitmap = new Bitmap(size.Width, size.Height);
		using Graphics graphics = Graphics.FromImage(bitmap);
		graphics.DrawImage(bmp, new Rectangle(Point.Empty, size));
		return bitmap;
	}

	public static byte[] ToArray(this Bitmap bmp)
	{
		bmp.ToArray(out byte[] data);
		return data;
	}

	public static void ToArray(this Bitmap bmp, out byte[] data)
	{
		data = new byte[bmp.Width * bmp.Height * 4];
		BitmapData bitmapData = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		Marshal.Copy(bitmapData.Scan0, data, 0, data.Length);
		bmp.UnlockBits(bitmapData);
	}

	public static byte[] ToArray(this Bitmap bmp, ColorBits type)
	{
		byte[] array = bmp.ToArray();
		MemoryStream memoryStream = new MemoryStream();
		for (int i = 0; i < array.Length; i += 4)
		{
			byte[] array2 = new byte[4];
			Array.Copy(array, i, array2, 0, 4);
			Colors.WriteColor(memoryStream, array2, type);
		}
		memoryStream.Close();
		return memoryStream.ToArray();
	}

	public static Bitmap FromArray(this byte[] data, Size size)
	{
		Bitmap bitmap = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
		bitmap.UnlockBits(bitmapData);
		return bitmap;
	}

	public static Bitmap FromArray(byte[] data, Size size, ColorBits bits)
	{
		MemoryStream memoryStream = new MemoryStream(data);
		data = new byte[size.Width * size.Height * 4];
		for (int i = 0; i < data.Length; i += 4)
		{
			Colors.ReadColor(memoryStream, bits, data, i);
		}
		memoryStream.Close();
		return data.FromArray(size);
	}

	public static byte[] FromArray2(byte[] data, Size size, ColorBits bits)
	{
		using MemoryStream stream = new MemoryStream(data);
		data = new byte[size.Width * size.Height * 4];
		for (int i = 0; i < data.Length; i += 4)
		{
			Colors.ReadColor(stream, bits, data, i);
		}
		return data;
	}

	public static Bitmap[] ReadGif(string path)
	{
		using FileStream stream = File.OpenRead(path);
		return ReadGif(stream);
	}

	public static Bitmap[] ReadGif(Stream stream)
	{
		GifDecoder gifDecoder = new GifDecoder();
		gifDecoder.Read(stream);
		int frameCount = gifDecoder.GetFrameCount();
		Bitmap[] array = new Bitmap[frameCount];
		for (int i = 0; i < frameCount; i++)
		{
			array[i] = new Bitmap(gifDecoder.GetFrame(i));
		}
		return array;
	}

	public static void WriteGif(string path, Image[] array, System.Drawing.Color transparent, int delay = 75)
	{
		AnimatedGifEncoder animatedGifEncoder = new AnimatedGifEncoder();
		animatedGifEncoder.Start();
		animatedGifEncoder.SetDelay(75);
		animatedGifEncoder.SetTransparent(transparent);
		foreach (Image im in array)
		{
			animatedGifEncoder.AddFrame(im);
		}
		animatedGifEncoder.Finish();
		animatedGifEncoder.Output(path);
	}

	public static ImageSource ToBitmapSourceA(this Bitmap bitmap)
	{
		return CreateBitmapSourceFromGdiBitmap(bitmap);
	}

	public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
	{
		if (bitmap == null)
		{
			return null;
		}
		Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
		BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		try
		{
			int bufferSize = rect.Width * rect.Height * 4;
			return BitmapSource.Create(bitmap.Width, bitmap.Height, bitmap.HorizontalResolution, bitmap.VerticalResolution, PixelFormats.Bgra32, null, bitmapData.Scan0, bufferSize, bitmapData.Stride);
		}
		finally
		{
			bitmap.UnlockBits(bitmapData);
		}
	}

	public static BitmapSource ByteArrayToBitmapSource2(this byte[] bytes, int width, int height)
	{
		System.Windows.Media.PixelFormat bgra = PixelFormats.Bgra32;
		int stride = width * bgra.BitsPerPixel / 8;
		return BitmapSource.Create(width, height, 0.0, 0.0, bgra, null, bytes, stride);
	}

	private static Bitmap Blur(Bitmap image, Rectangle rectangle, int blurSize)
	{
		Bitmap bitmap = new Bitmap(image.Width, image.Height);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
		}
		for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
		{
			for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				for (int k = i; k < i + blurSize && k < image.Width; k++)
				{
					for (int l = j; l < j + blurSize && l < image.Height; l++)
					{
						System.Drawing.Color pixel = bitmap.GetPixel(k, l);
						num += pixel.R;
						num2 += pixel.G;
						num3 += pixel.B;
						num4++;
					}
				}
				num /= num4;
				num2 /= num4;
				num3 /= num4;
				for (int m = i; m < i + blurSize && m < image.Width && m < rectangle.Width; m++)
				{
					for (int n = j; n < j + blurSize && n < image.Height && n < rectangle.Height; n++)
					{
						bitmap.SetPixel(m, n, System.Drawing.Color.FromArgb(num, num2, num3));
					}
				}
			}
		}
		return bitmap;
	}
}
