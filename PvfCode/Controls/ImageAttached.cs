using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PvfCode.Controls;

public class ImageAttached
{
	public static readonly DependencyProperty Gray8Property;

	public static readonly DependencyProperty BitmapSourceBackupProperty;

	public static bool GetGray8(DependencyObject d)
	{
		return (bool)d.GetValue(Gray8Property);
	}

	public static void SetGray8(DependencyObject d, bool value)
	{
		d.SetValue(Gray8Property, (object)value);
	}

	private static void XUfasfJXY4(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		if (!(P_0 is Image { Source: not null } image))
		{
			return;
		}
		if ((bool)P_0.GetValue(Gray8Property))
		{
			BitmapSource bitmapSource = (image.Source as BitmapSource).CloneCurrentValue();
			P_0.SetValue(BitmapSourceBackupProperty, (object)bitmapSource);
			FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap();
			formatConvertedBitmap.BeginInit();
			formatConvertedBitmap.Source = image.Source as BitmapSource;
			formatConvertedBitmap.DestinationFormat = PixelFormats.Gray8;
			formatConvertedBitmap.EndInit();
			image.Source = formatConvertedBitmap;
		}
		else
		{
			object value = ((DependencyObject)image).GetValue(BitmapSourceBackupProperty);
			if (value != null && value is BitmapSource source)
			{
				image.Source = source;
			}
		}
	}

	public static BitmapSource GetBitmapSourceBackup(DependencyObject d)
	{
		return (BitmapSource)d.GetValue(BitmapSourceBackupProperty);
	}

	public static void SetBitmapSourceBackup(DependencyObject d, BitmapSource value)
	{
		d.SetValue(BitmapSourceBackupProperty, (object)value);
	}

	public ImageAttached()
	{
	}

	static ImageAttached()
	{
		Gray8Property = DependencyProperty.RegisterAttached("Gray8", typeof(bool), typeof(ImageAttached), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(XUfasfJXY4)));
		BitmapSourceBackupProperty = DependencyProperty.RegisterAttached("BitmapSourceBackup", typeof(BitmapSource), typeof(ImageAttached), (PropertyMetadata)(object)new FrameworkPropertyMetadata(null));
	}
}
