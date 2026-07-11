using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PvfCode;

public class ImageSourceUtils
{
	public static byte[] ConvertBitmapSourceToByteArray(BitmapEncoder encoder, ImageSource imageSource)
	{
		byte[] result = null;
		if (imageSource is BitmapSource source)
		{
			encoder.Frames.Add(BitmapFrame.Create(source));
			using MemoryStream memoryStream = new MemoryStream();
			encoder.Save(memoryStream);
			result = memoryStream.ToArray();
		}
		return result;
	}

	public static byte[] ConvertBitmapSourceToByteArray(BitmapSource image)
	{
		BitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
		bitmapEncoder.Frames.Add(BitmapFrame.Create(image));
		using MemoryStream memoryStream = new MemoryStream();
		bitmapEncoder.Save(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] ConvertBitmapSourceToByteArray(ImageSource imageSource)
	{
		if (imageSource == null)
		{
			return null;
		}
		BitmapSource source = imageSource as BitmapSource;
		BitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
		bitmapEncoder.Frames.Add(BitmapFrame.Create(source));
		using MemoryStream memoryStream = new MemoryStream();
		bitmapEncoder.Save(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] ConvertBitmapSourceToByteArray(Uri uri)
	{
		BitmapImage source = new BitmapImage(uri);
		BitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
		bitmapEncoder.Frames.Add(BitmapFrame.Create(source));
		using MemoryStream memoryStream = new MemoryStream();
		bitmapEncoder.Save(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] ConvertBitmapSourceToByteArray(string filepath)
	{
		BitmapImage source = new BitmapImage(new Uri(filepath));
		BitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
		bitmapEncoder.Frames.Add(BitmapFrame.Create(source));
		using MemoryStream memoryStream = new MemoryStream();
		bitmapEncoder.Save(memoryStream);
		return memoryStream.ToArray();
	}

	public static BitmapImage ConvertByteArrayToBitmapImage(byte[] bytes)
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(bytes);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = memoryStream;
			bitmapImage.EndInit();
			return bitmapImage;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public ImageSourceUtils()
	{
	}
}
