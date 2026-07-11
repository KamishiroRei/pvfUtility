using System;
using System.Drawing;
using System.IO;
using PvfCode.NPK.Utils.Lib;
using PvfCode.NPK.Utils.Models;

namespace PvfCode.NPK.Utils.Handle;

public class OtherHandler(ImagePack album) : HandlerBase(album)
{
	private byte[] _data = new byte[0];

	public override byte[] AdjustData()
	{
		return _data;
	}

	public override Bitmap ConvertToBitmap(ImgFile entity)
	{
		return null;
	}

	public override byte[] ConvertToByte(ImgFile entity)
	{
		return new byte[0];
	}

	public override byte[] ConvertToByte2(ImgFile entity, Size size2)
	{
		throw new NotImplementedException();
	}

	public override bool CreateFromStream(Stream stream)
	{
		stream.Read((int)Pack.IndexLength, out _data);
		Pack.Data = _data;
		return true;
	}
}
