using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PvfCode.NPK.Utils.Lib;
using PvfCode.NPK.Utils.Models;
using PvfCode.NPK.Utils.Models.Enums;

namespace PvfCode.NPK.Utils.Handle;

public class FourthHandler : SecondHandler
{
	public FourthHandler(ImagePack album)
		: base(album)
	{
	}

	public override Bitmap ConvertToBitmap(ImgFile entity)
	{
		byte[] data = entity.Data;
		int size = entity.Width * entity.Height;
		if (entity.Type == ColorBits.ARGB_1555 && entity.CompressMode == CompressMode.ZLIB)
		{
			data = Zlib.Decompress(data, size);
			List<Color> currentTable = Pack.CurrentTable;
			if (currentTable.Count > 0)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					byte[] array = data;
					for (int i = 0; i < array.Length; i++)
					{
						int index = array[i] % currentTable.Count;
						memoryStream.WriteColor(currentTable[index], ColorBits.ARGB_8888);
					}
					data = memoryStream.ToArray();
				}
				return data.FromArray(entity.Size);
			}
		}
		return base.ConvertToBitmap(entity);
	}

	public override byte[] ConvertToByte(ImgFile entity)
	{
		if (entity.CompressMode == CompressMode.NONE)
		{
			return base.ConvertToByte(entity);
		}
		using MemoryStream memoryStream = new MemoryStream();
		byte[] array = entity.Picture.ToArray();
		List<Color> currentTable = Pack.CurrentTable;
		for (int i = 0; i < array.Length; i += 4)
		{
			Color item = Color.FromArgb(array[i + 3], array[i + 2], array[i + 1], array[i]);
			if (!currentTable.Contains(item))
			{
				currentTable.Add(item);
			}
			memoryStream.WriteByte((byte)currentTable.IndexOf(item));
		}
		array = memoryStream.ToArray();
		if (array.Length < 2)
		{
			array = new byte[2];
		}
		return array;
	}

	public override byte[] AdjustData()
	{
		List<Color> currentTable = Pack.CurrentTable;
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteInt(currentTable.Count);
		Colors.WritePalette(memoryStream, currentTable);
		memoryStream.Write(base.AdjustData());
		return memoryStream.ToArray();
	}

	public override void ConvertToVersion(ImgVersion version)
	{
		if (version > ImgVersion.Ver2 && version != ImgVersion.Ver5)
		{
			return;
		}
		foreach (ImgFile img in Pack.ImgList)
		{
			if (img.Type != ColorBits.LINK)
			{
				img.Type = ColorBits.ARGB_8888;
			}
		}
	}

	public override bool CreateFromStream(Stream stream)
	{
		int count = stream.ReadInt();
		List<Color> item = new List<Color>(Colors.ReadPalette(stream, count));
		Pack.Tables = new List<List<Color>>();
		Pack.Tables.Add(item);
		return base.CreateFromStream(stream);
	}
}
