using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PvfCode.NPK.Utils.Lib;
using PvfCode.NPK.Utils.Models;
using PvfCode.NPK.Utils.Models.Enums;

namespace PvfCode.NPK.Utils.Handle;

public class FifthHandler(ImagePack album) : SecondHandler(album)
{
	private readonly Dictionary<int, TextureInfo> _map = new Dictionary<int, TextureInfo>();

	public readonly List<Texture> List = new List<Texture>();

	public override Bitmap ConvertToBitmap(ImgFile entity)
	{
		if (entity.Type < ColorBits.LINK && entity.Length > 0)
		{
			return base.ConvertToBitmap(entity);
		}
		if (!_map.ContainsKey(entity.Index))
		{
			return new Bitmap(1, 1);
		}
		TextureInfo textureInfo = _map[entity.Index];
		Bitmap bitmap = textureInfo.Texture.Pictrue.Clone(textureInfo.Rectangle, PixelFormat.Undefined);
		if (textureInfo.Top != 0)
		{
			bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
		}
		return bitmap;
	}

	public override byte[] ConvertToByte(ImgFile entity)
	{
		if (entity.Type < ColorBits.LINK && entity.Length > 0)
		{
			return base.ConvertToByte(entity);
		}
		if (entity.Width * entity.Height == 1)
		{
			entity.CompressMode = CompressMode.NONE;
			return base.ConvertToByte(entity);
		}
		if (entity.CompressMode == CompressMode.ZLIB)
		{
			entity.CompressMode = CompressMode.DDS_ZLIB;
		}
		Texture texture = Texture.CreateFromBitmap(entity);
		_map[entity.Index] = new TextureInfo
		{
			Texture = texture,
			RightDown = new Point(texture.Width, texture.Height)
		};
		return new byte[0];
	}

	public override byte[] AdjustData()
	{
		List.Clear();
		foreach (TextureInfo value in _map.Values)
		{
			Texture texture = value.Texture;
			if (!List.Contains(texture))
			{
				texture.Index = List.Count;
				List.Add(texture);
			}
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteInt(Pack.CurrentTable.Count);
		Colors.WritePalette(memoryStream, Pack.CurrentTable);
		foreach (Texture item in List)
		{
			memoryStream.WriteInt((int)item.Version);
			memoryStream.WriteInt((int)item.Type);
			memoryStream.WriteInt(item.Index);
			memoryStream.WriteInt(item.Length);
			memoryStream.WriteInt(item.FullLength);
			memoryStream.WriteInt(item.Width);
			memoryStream.WriteInt(item.Height);
		}
		List<ImgFile> list = new List<ImgFile>();
		long length = memoryStream.Length;
		foreach (ImgFile img in Pack.ImgList)
		{
			memoryStream.WriteInt((int)img.Type);
			if (img.Type == ColorBits.LINK)
			{
				memoryStream.WriteInt(img.Target.Index);
				continue;
			}
			memoryStream.WriteInt((int)img.CompressMode);
			memoryStream.WriteInt(img.Size.Width);
			memoryStream.WriteInt(img.Size.Height);
			memoryStream.WriteInt(img.Length);
			memoryStream.WriteInt(img.Location.X);
			memoryStream.WriteInt(img.Location.Y);
			memoryStream.WriteInt(img.FrameSize.Width);
			memoryStream.WriteInt(img.FrameSize.Height);
			if (img.Type < ColorBits.LINK && img.Length != 0)
			{
				list.Add(img);
				continue;
			}
			TextureInfo textureInfo = _map[img.Index];
			memoryStream.WriteInt(textureInfo.Unknown);
			memoryStream.WriteInt(textureInfo.Texture.Index);
			memoryStream.WriteInt(textureInfo.LeftUp.X);
			memoryStream.WriteInt(textureInfo.LeftUp.Y);
			memoryStream.WriteInt(textureInfo.RightDown.X);
			memoryStream.WriteInt(textureInfo.RightDown.Y);
			memoryStream.WriteInt(textureInfo.Top);
		}
		Pack.IndexLength = memoryStream.Length - length;
		foreach (Texture item2 in List)
		{
			memoryStream.Write(item2.Data);
		}
		foreach (ImgFile item3 in list)
		{
			memoryStream.Write(item3.Data);
		}
		memoryStream.Close();
		byte[] array = memoryStream.ToArray();
		Pack.Length = array.Length + 40;
		memoryStream = new MemoryStream();
		memoryStream.WriteInt(List.Count);
		memoryStream.WriteInt(Pack.Length);
		memoryStream.Write(array);
		memoryStream.Close();
		return memoryStream.ToArray();
	}

	public override bool CreateFromStream(Stream stream)
	{
		int num = stream.ReadInt();
		Pack.Length = stream.ReadInt();
		int count = stream.ReadInt();
		List<Color> item = new List<Color>(Colors.ReadPalette(stream, count));
		Pack.Tables = new List<List<Color>> { item };
		List<Texture> list = new List<Texture>();
		for (int i = 0; i < num; i++)
		{
			Texture item2 = new Texture
			{
				Version = (TextureVersion)stream.ReadInt(),
				Type = (ColorBits)stream.ReadInt(),
				Index = stream.ReadInt(),
				Length = stream.ReadInt(),
				FullLength = stream.ReadInt(),
				Width = stream.ReadInt(),
				Height = stream.ReadInt()
			};
			list.Add(item2);
		}
		Dictionary<ImgFile, int> dictionary = new Dictionary<ImgFile, int>();
		List<ImgFile> list2 = new List<ImgFile>();
		for (int j = 0; j < Pack.Count; j++)
		{
			ImgFile imgFile = new ImgFile(Pack);
			imgFile.Index = Pack.ImgList.Count;
			imgFile.Type = (ColorBits)stream.ReadInt();
			Pack.ImgList.Add(imgFile);
			if (imgFile.Type == ColorBits.LINK)
			{
				dictionary.Add(imgFile, stream.ReadInt());
				continue;
			}
			imgFile.CompressMode = (CompressMode)stream.ReadInt();
			imgFile.Width = stream.ReadInt();
			imgFile.Height = stream.ReadInt();
			imgFile.Length = stream.ReadInt();
			imgFile.X = stream.ReadInt();
			imgFile.Y = stream.ReadInt();
			imgFile.FrameWidth = stream.ReadInt();
			imgFile.FrameHeight = stream.ReadInt();
			if (imgFile.Type < ColorBits.LINK && imgFile.Length != 0)
			{
				list2.Add(imgFile);
				continue;
			}
			int unknown = stream.ReadInt();
			int index = stream.ReadInt();
			Texture texture = list[index];
			Point leftUp = new Point(stream.ReadInt(), stream.ReadInt());
			Point rightDown = new Point(stream.ReadInt(), stream.ReadInt());
			int top = stream.ReadInt();
			TextureInfo value = new TextureInfo
			{
				Unknown = unknown,
				Texture = texture,
				LeftUp = leftUp,
				RightDown = rightDown,
				Top = top
			};
			_map.Add(imgFile.Index, value);
		}
		foreach (ImgFile key in dictionary.Keys)
		{
			key.Target = Pack.ImgList[dictionary[key]];
		}
		foreach (Texture item3 in list)
		{
			byte[] array = new byte[item3.Length];
			stream.Read(array);
			item3.Data = array;
		}
		foreach (ImgFile item4 in list2)
		{
			byte[] array2 = new byte[item4.Length];
			stream.Read(array2);
			item4.Data = array2;
		}
		return true;
	}

	public override void ConvertToVersion(ImgVersion version)
	{
		foreach (ImgFile img in Pack.ImgList)
		{
			img.Load();
			if (version <= ImgVersion.Ver2)
			{
				if (img.Type == ColorBits.DXT_1)
				{
					img.Type = ColorBits.ARGB_1555;
				}
				if (img.Type == ColorBits.DXT_5)
				{
					img.Type = ColorBits.ARGB_8888;
				}
			}
			else if (version == ImgVersion.Ver4)
			{
				img.Type = ColorBits.ARGB_1555;
			}
			if (img.CompressMode > CompressMode.ZLIB)
			{
				img.CompressMode = CompressMode.ZLIB;
			}
		}
	}
}
