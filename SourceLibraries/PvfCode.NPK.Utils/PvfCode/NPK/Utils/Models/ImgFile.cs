using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm;
using PvfCode.NPK.Utils.Lib;
using PvfCode.NPK.Utils.Models.Enums;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace PvfCode.NPK.Utils.Models;

public class ImgFile : ViewModelBase
{
	[LSIgnore]
	private Bitmap _image;

	[LSIgnore]
	public Size FrameSize = Size.Empty;

	public CompressMode CompressMode = CompressMode.NONE;

	[LSIgnore]
	public byte[] Data = new byte[2];

	[LSIgnore]
	public int Length = 2;

	[LSIgnore]
	public Point Location;

	[LSIgnore]
	public ImagePack Parent;

	[LSIgnore]
	public Size Size = new Size(1, 1);

	[LSIgnore]
	public ImgFile Target;

	private ImageSource imageSource;

	public int Index
	{
		get
		{
			return GetProperty(() => Index);
		}
		set
		{
			SetProperty(() => Index, value);
		}
	}

	public ColorBits Type { get; set; } = ColorBits.ARGB_1555;

	[LSIgnore]
	public Bitmap Picture
	{
		get
		{
			if (Type == ColorBits.LINK)
			{
				return Target?.Picture;
			}
			if (IsOpen)
			{
				return _image;
			}
			return _image = Parent.ConvertToBitmap(this);
		}
		set
		{
			_image = value;
			if (value != null)
			{
				Size = value.Size;
			}
		}
	}

	public ImageSource ImageSource
	{
		get
		{
			if (imageSource == null)
			{
				imageSource = GetImageSouce();
				((Freezable)imageSource).Freeze();
			}
			return imageSource;
		}
	}

	public bool AllowImageDyeing
	{
		get
		{
			return GetProperty(() => AllowImageDyeing);
		}
		set
		{
			SetProperty(() => AllowImageDyeing, value);
		}
	}

	private System.Windows.Media.Color? ImageDyeingColor { get; set; }

	[LSIgnore]
	public bool IsOpen => _image != null;

	public int X
	{
		get
		{
			return Location.X;
		}
		set
		{
			Location.X = value;
			RaisePropertyChanged("X");
		}
	}

	public int Y
	{
		get
		{
			return Location.Y;
		}
		set
		{
			Location.Y = value;
			RaisePropertyChanged("Y");
		}
	}

	public int Width
	{
		get
		{
			return Size.Width;
		}
		set
		{
			Size.Width = value;
		}
	}

	public int Height
	{
		get
		{
			return Size.Height;
		}
		set
		{
			Size.Height = value;
		}
	}

	public int FrameWidth
	{
		get
		{
			return FrameSize.Width;
		}
		set
		{
			FrameSize = new Size(value, FrameHeight);
		}
	}

	public int FrameHeight
	{
		get
		{
			return FrameSize.Height;
		}
		set
		{
			FrameSize = new Size(FrameWidth, value);
		}
	}

	[LSIgnore]
	public ImgVersion Version => Parent.Version;

	[LSIgnore]
	public bool Hidden
	{
		get
		{
			if (Width * Height == 1)
			{
				return CompressMode == CompressMode.NONE;
			}
			return false;
		}
	}

	public ImgFile()
	{
	}

	public ImgFile(ImagePack parent)
	{
		Parent = parent;
	}

	public byte[]? GetImageBytes(Size? size = null)
	{
		if (!size.HasValue)
		{
			size = Size;
		}
		if (Type == ColorBits.LINK)
		{
			return Target?.GetImageBytes(size);
		}
		return Parent.ConvertToByte2(this, size.Value);
	}

	public void AniImageDye(System.Windows.Media.Color? color)
	{
		AllowImageDyeing = color.HasValue;
		ImageDyeingColor = color;
		byte[] imageBytes = GetImageBytes(Size);
		if (color.HasValue)
		{
			imageSource = imageBytes.ImageDye(ImageDyeingColor.Value).ByteArrayToBitmapSource2(Size.Width, Size.Height);
		}
		else
		{
			imageSource = GetImageSouce();
		}
		ImageSource obj = imageSource;
		if (obj != null)
		{
			((Freezable)obj).Freeze();
		}
		RaisePropertyChanged("ImageSource");
	}

	public void RefreshImage()
	{
		imageSource = GetImageSouce();
		RaisePropertyChanged("ImageSource");
	}

	public ImageSource GetImageSouce(Size? size = null)
	{
		if (!size.HasValue)
		{
			size = Size;
		}
		return GetImageBytes(size.Value).ByteArrayToBitmapSource2(size.Value.Width, size.Value.Height);
	}

	public void Load()
	{
		_image = Parent.ConvertToBitmap(this);
	}

	public void ReplaceImage(ColorBits type, bool isAdjust, Bitmap bmp)
	{
		if (bmp != null)
		{
			Picture = bmp;
			Target = null;
			Type = ((type == ColorBits.UNKNOWN) ? Type : type);
			if (type == ColorBits.UNKNOWN)
			{
				type = ((Type == ColorBits.LINK) ? ColorBits.ARGB_1555 : ((Version == ImgVersion.Ver5 || Type <= ColorBits.LINK) ? Type : (Type - 4)));
			}
			Type = type;
			if (isAdjust)
			{
				X += bmp.Width - Size.Width;
				Y += bmp.Height - Size.Height;
			}
			Size = bmp.Size;
			if (FrameHeight < bmp.Height)
			{
				FrameHeight = bmp.Height;
			}
			if (FrameWidth < bmp.Width)
			{
				FrameWidth = bmp.Width;
			}
			if (Width * Height > 1)
			{
				CompressMode = CompressMode.ZLIB;
			}
		}
	}

	public void TrimImage()
	{
		if (Type != ColorBits.LINK && CompressMode != CompressMode.NONE && Picture != null)
		{
			Rectangle srcRect = Picture.Scan();
			Bitmap bitmap = new Bitmap(srcRect.Width, srcRect.Height);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.DrawImage(destRect: new Rectangle(Point.Empty, srcRect.Size), image: Picture, srcRect: srcRect, srcUnit: GraphicsUnit.Pixel);
			graphics.Dispose();
			Size = srcRect.Size;
			Location = Location.Add(srcRect.Location);
			Picture = bitmap;
		}
	}

	public void CanvasImage(Rectangle target)
	{
		if (Type != ColorBits.LINK)
		{
			Picture = Picture.Canvas(target.Add(new Rectangle(Location, Size.Empty)));
			Size = target.Size;
			Location = target.Location;
		}
	}

	public virtual void Adjust()
	{
		if (Type == ColorBits.LINK)
		{
			Length = 0;
		}
		else if (IsOpen)
		{
			Data = Parent.ConvertToByte(this);
			if (Data.Length != 0 && CompressMode >= CompressMode.ZLIB)
			{
				Data = Zlib.Compress(Data);
			}
			Length = Data.Length;
		}
	}

	public bool Equals(ImgFile entity)
	{
		if (entity != null && Parent.Equals(entity.Parent))
		{
			return Index == entity.Index;
		}
		return false;
	}

	public ImgFile Clone(ImagePack album)
	{
		return new ImgFile(album)
		{
			Picture = Picture,
			CompressMode = CompressMode,
			Type = Type,
			Location = Location,
			FrameSize = FrameSize,
			Target = Target
		};
	}

	public void Hide()
	{
		CompressMode = CompressMode.NONE;
		Data = new byte[0];
		imageSource = null;
		RaisePropertyChanged("ImageSource");
		RaisePropertyChanged("Hidden");
	}

	public bool ImgSaveFileAs(string filePath)
	{
		if (Data != null && Data.Length != 0 && ImageSource != null)
		{
			using FileStream stream = new FileStream(filePath, FileMode.Create);
			PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
			pngBitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)ImageSource));
			pngBitmapEncoder.Save(stream);
		}
		return false;
	}
}
