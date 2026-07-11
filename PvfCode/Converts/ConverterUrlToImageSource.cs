using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utools;

namespace PvfCode.Converts;

public class ConverterUrlToImageSource : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		string text = value.ToString();
		if (text.ToLower().Contains("http://") || text.ToLower().Contains("https://"))
		{
			return lDEaf7wYiQ(text);
		}
		return ImageSourceUtils.ConvertByteArrayToBitmapImage(ImageHelper.Base64ImageToBytes(text));
	}

	private ImageSource lDEaf7wYiQ(string? url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return null;
		}
		try
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(url, UriKind.Absolute);
			bitmapImage.DecodePixelWidth = 300;
			bitmapImage.EndInit();
			return bitmapImage;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}

	public ConverterUrlToImageSource()
	{
	}
}
