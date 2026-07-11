using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Utools;

public class ImageHelper
{
	public static PixelFormat[] pixelFormats;

	public static byte[] Base64ImageToBytes(string baseImageUrl)
	{
		if (string.IsNullOrEmpty(baseImageUrl))
		{
			return null;
		}
		return Convert.FromBase64String(baseImageUrl.Replace("data:image/jpeg;base64,", string.Empty).Replace("data:image/bmp;base64,", string.Empty).Replace("data:image/jpg;base64,", string.Empty)
			.Replace("data:image/png;base64,", string.Empty)
			.Replace("data:image/gif;base64,", string.Empty));
	}

	public static string BytesToBase64Image(byte[] imageBytes)
	{
		if (imageBytes == null)
		{
			return string.Empty;
		}
		string text = Convert.ToBase64String(imageBytes);
		return "data:image/jpeg;base64," + text;
	}

	public static byte[] BitmapToBytes(Bitmap bitmap)
	{
		MemoryStream memoryStream = new MemoryStream();
		bitmap.Save(memoryStream, ImageFormat.Bmp);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		byte[] array = new byte[memoryStream.Length];
		memoryStream.Read(array, 0, array.Length);
		memoryStream.Dispose();
		return array;
	}

	public ImageHelper()
	{
	}

	static ImageHelper()
	{
		pixelFormats = new PixelFormat[4]
		{
			PixelFormat.Format8bppIndexed,
			PixelFormat.Format16bppArgb1555,
			PixelFormat.Format32bppArgb,
			PixelFormat.Format64bppArgb
		};
	}
}
