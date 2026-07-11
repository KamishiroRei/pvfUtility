using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using PvfCode.Images;
using PvfCode.Models.Images;

namespace PvfCode;

public class Res : IRes
{
	private static Res t0dlt03dM7;

	private CharacJobAvatar pevlb6LMoW;

	private ImageSource? U8klIxPlPJ;

	private ImageSource? p0xlEImTe8;

	[CompilerGenerated]
	private ResTreeFileTypes sOhlOwrxKx;

	[CompilerGenerated]
	private ResSearch gaqlKkFJG3;

	[CompilerGenerated]
	private ResEditor eCkl9C4luQ;

	private ImageSource? qdZlPIBnaX;

	public static Res Instance
	{
		get
		{
			if (t0dlt03dM7 == null)
			{
				t0dlt03dM7 = new Res();
			}
			return t0dlt03dM7;
		}
	}

	public CharacJobAvatar CharacJobAvatar
	{
		get
		{
			if (pevlb6LMoW == null)
			{
				pevlb6LMoW = new CharacJobAvatar();
			}
			return pevlb6LMoW;
		}
		set
		{
			pevlb6LMoW = value;
		}
	}

	public ImageSource ImageSealingMark
	{
		get
		{
			if (U8klIxPlPJ == null)
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
					_ = array[i + 3];
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
				U8klIxPlPJ = writeableBitmap;
				((Freezable)U8klIxPlPJ).Freeze();
			}
			return U8klIxPlPJ;
		}
	}

	public ImageSource ItemImageSourceSelected
	{
		get
		{
			p0xlEImTe8 = null;
			if (p0xlEImTe8 == null)
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
					_ = array[i + 3];
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
				p0xlEImTe8 = writeableBitmap;
				((Freezable)p0xlEImTe8).Freeze();
			}
			return p0xlEImTe8;
		}
	}

	public ImageSource ChatGPTICON => new BitmapImage(new Uri("pack://application:,,,/pvfUtility;component/images/pngs/chatgpt.png", UriKind.RelativeOrAbsolute));

	public ImageSource OpenFolder_16x => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/openfolder_16x.svg");

	public ImageSource Save_16x => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/save_16x.svg");

	public ImageSource ClosePvf => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/closepvf.svg");

	public ImageSource ResetLayoutOptions => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/resetlayoutoptions.svg");

	public ImageSource VisualStudioBlendLogo2015Pre_16x => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/visualstudioblendlogo2015pre_16x.svg");

	public ImageSource ErrorIcon => GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/erroricon.svg");

	public ResTreeFileTypes TreeFiles
	{
		[CompilerGenerated]
		get
		{
			return sOhlOwrxKx;
		}
		[CompilerGenerated]
		set
		{
			sOhlOwrxKx = value;
		}
	}

	public ResSearch Search
	{
		[CompilerGenerated]
		get
		{
			return gaqlKkFJG3;
		}
		[CompilerGenerated]
		set
		{
			gaqlKkFJG3 = value;
		}
	}

	public ResEditor Editor
	{
		[CompilerGenerated]
		get
		{
			return eCkl9C4luQ;
		}
		[CompilerGenerated]
		set
		{
			eCkl9C4luQ = value;
		}
	}

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
			if (qdZlPIBnaX == null)
			{
				qdZlPIBnaX = new BitmapImage(new Uri("pack://application:,,,/pvfUtility;component/images/pngs/npcshop/rednull.png", UriKind.RelativeOrAbsolute));
				((Freezable)qdZlPIBnaX).Freeze();
			}
			return qdZlPIBnaX;
		}
	}

	private ImageSource nlYlemf92d(string P_0)
	{
		return new BitmapImage(new Uri(P_0, UriKind.Relative));
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
		sOhlOwrxKx = new ResTreeFileTypes();
		gaqlKkFJG3 = new ResSearch();
		eCkl9C4luQ = new ResEditor();
	}
}
