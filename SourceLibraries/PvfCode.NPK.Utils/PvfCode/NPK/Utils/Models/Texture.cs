using System.Drawing;
using PvfCode.NPK.Utils.Coder;
using PvfCode.NPK.Utils.Lib;
using PvfCode.NPK.Utils.Models.Enums;

namespace PvfCode.NPK.Utils.Models;

public class Texture
{
	private Bitmap _image;

	public int Index { get; set; }

	public int Width { get; set; } = 4;

	public int Height { get; set; } = 4;

	public int Length { get; set; }

	public int FullLength { get; set; }

	public byte[] Data { get; set; }

	public TextureVersion Version { get; set; } = TextureVersion.Dxt1;

	public ColorBits Type { get; set; } = ColorBits.DXT_1;

	public Bitmap Pictrue
	{
		get
		{
			if (_image != null)
			{
				return _image;
			}
			byte[] data = Zlib.Decompress(Data, FullLength);
			if (Type < ColorBits.LINK)
			{
				return Bitmaps.FromArray(data, new Size(Width, Height), Type);
			}
			data = DdsDecoder.Decode(data).DdsMipmaps[0].Data;
			return data.FromArray(new Size(Width, Height));
		}
		set
		{
			_image = value;
		}
	}

	public static Texture CreateFromBitmap(ImgFile sprite)
	{
		Bitmap picture = sprite.Picture;
		ColorBits colorBits = sprite.Type;
		if (colorBits > ColorBits.LINK)
		{
			colorBits -= 4;
		}
		byte[] array = picture.ToArray(colorBits);
		int fullLength = array.Length;
		int width = picture.Width;
		int height = picture.Height;
		array = Zlib.Compress(array);
		return new Texture
		{
			Data = array,
			FullLength = fullLength,
			Length = array.Length,
			Width = width,
			Height = height,
			Type = colorBits
		};
	}
}
