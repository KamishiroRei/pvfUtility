using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using PvfCode.Images;
using PvfCode.Models.Images;

namespace PvfCode;

public class Res : IRes
{
	private static Res instance;

	private CharacJobAvatar characJobAvatar;

	private ImageSource? imageSealingMark;

	private ImageSource? itemImageSourceSelected;

	private ImageSource? readNull;

	public static Res Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new Res();
			}
			return instance;
		}
	}

	public CharacJobAvatar CharacJobAvatar
	{
		get
		{
			if (characJobAvatar == null)
			{
				characJobAvatar = new CharacJobAvatar();
			}
			return characJobAvatar;
		}
		set
		{
			characJobAvatar = value;
		}
	}

	public ImageSource ImageSealingMark
	{
		get
		{
			if (imageSealingMark == null)
			{
				WriteableBitmap writeableBitmap = new WriteableBitmap(new FormatConvertedBitmap(new BitmapImage(new Uri("pack://application:,,,/pvfUtility;component/images/pngs/npcshop/imagemark.png", UriKind.RelativeOrAbsolute)), PixelFormats.Bgra32, null, 0.0));
				int pixelWidth = writeableBitmap.PixelWidth;
				int pixelHeight = writeableBitmap.PixelHeight;
				byte[] array = new byte[pixelWidth * pixelHeight * 4];
				writeableBitmap.CopyPixels(array, pixelWidth * 4, 0);
				byte b = 50;
				for (int i = 0; i < array.Length; i += 4)
				{
					byte b2 = array[i];
					byte b3 = array[i + 1];
					byte b4 = array[i + 2];
					if ((byte)(0.2126 * (double)(int)b4 + 0.7152 * (double)(int)b3 + 0.0722 * (double)(int)b2) < b)
					{
						array[i + 3] = 20;
					}
					else
					{
						array[i + 3] = byte.MaxValue;
					}
				}
				writeableBitmap.WritePixels(new Int32Rect(0, 0, pixelWidth, pixelHeight), array, pixelWidth * 4, 0);
				imageSealingMark = writeableBitmap;
				((Freezable)imageSealingMark).Freeze();
			}
			return imageSealingMark;
		}
	}

	public ImageSource ItemImageSourceSelected
	{
		get
		{
			itemImageSourceSelected = null;
			if (itemImageSourceSelected == null)
			{
				WriteableBitmap writeableBitmap = new WriteableBitmap(new FormatConvertedBitmap(new BitmapImage(new Uri("pack://application:,,,/pvfUtility;component/images/pngs/npcshop/selecteditem.png", UriKind.RelativeOrAbsolute)), PixelFormats.Bgra32, null, 0.0));
				int pixelWidth = writeableBitmap.PixelWidth;
				int pixelHeight = writeableBitmap.PixelHeight;
				byte[] array = new byte[pixelWidth * pixelHeight * 4];
				writeableBitmap.CopyPixels(array, pixelWidth * 4, 0);
				byte b = 50;
				for (int i = 0; i < array.Length; i += 4)
				{
					byte b2 = array[i];
					byte b3 = array[i + 1];
					byte b4 = array[i + 2];
					if ((byte)(0.2126 * (double)(int)b4 + 0.7152 * (double)(int)b3 + 0.0722 * (double)(int)b2) < b)
					{
						array[i + 3] = 20;
					}
					else
					{
						array[i + 3] = byte.MaxValue;
					}
				}
				writeableBitmap.WritePixels(new Int32Rect(0, 0, pixelWidth, pixelHeight), array, pixelWidth * 4, 0);
				itemImageSourceSelected = writeableBitmap;
				((Freezable)itemImageSourceSelected).Freeze();
			}
			return itemImageSourceSelected;
		}
	}

	public ImageSource ChatGPTICON => new BitmapImage(new Uri("pack://application:,,,/pvfUtility;component/images/pngs/chatgpt.png", UriKind.RelativeOrAbsolute));

	public ImageSource OpenFolder_16x => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/openfolder_16x.svg");

	public ImageSource Save_16x => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/save_16x.svg");

	public ImageSource ClosePvf => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/closepvf.svg");

	public ImageSource ResetLayoutOptions => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/resetlayoutoptions.svg");

	public ImageSource VisualStudioBlendLogo2015Pre_16x => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/visualstudioblendlogo2015pre_16x.svg");

	public ImageSource ErrorIcon => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/erroricon.svg");

	public ResTreeFileTypes TreeFiles { get; set; }

	public ResSearch Search { get; set; }

	public ResEditor Editor { get; set; }

	public ImageSource HigSectionIcon => (ImageSource)Application.Current.TryFindResource("HigSectionIcon");

	public ImageSource MethodSealed => (ImageSource)Application.Current.TryFindResource("MethodSealed");

	public ImageSource String => (ImageSource)Application.Current.TryFindResource("StringIcon");

	public ImageSource MacroIcon => (ImageSource)Application.Current.TryFindResource("MacroIcon");

	public ImageSource FindinFiles_16x => (ImageSource)Application.Current.TryFindResource("FindinFiles_16x");

	public ImageSource CollapseGroup_16x => (ImageSource)Application.Current.TryFindResource("CollapseGroup_16x");

	public ImageSource ClearWindowContent => (ImageSource)Application.Current.TryFindResource("ClearWindowContent");

	public ImageSource ReadNull
	{
		get
		{
			if (readNull == null)
			{
				readNull = new BitmapImage(new Uri("pack://application:,,,/pvfUtility;component/images/pngs/npcshop/rednull.png", UriKind.RelativeOrAbsolute));
				((Freezable)readNull).Freeze();
			}
			return readNull;
		}
	}

	public ImageSource GetSvgImage(string path)
	{
		return (ImageSource)new SvgImageSourceExtension
		{
			Uri = new Uri(path)
		}.ProvideValue(null);
	}

	public Res()
	{
		TreeFiles = new ResTreeFileTypes();
		Search = new ResSearch();
		Editor = new ResEditor();
	}
}
