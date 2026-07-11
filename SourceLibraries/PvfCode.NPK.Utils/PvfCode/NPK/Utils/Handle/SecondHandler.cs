using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using PvfCode.NPK.Utils.Lib;
using PvfCode.NPK.Utils.Models;
using PvfCode.NPK.Utils.Models.Enums;
using Swordfish.NET.Collections.Auxiliary;
using Utools;
using Size = System.Drawing.Size;

namespace PvfCode.NPK.Utils.Handle;

public class SecondHandler : HandlerBase
{
	public SecondHandler(ImagePack album)
		: base(album)
	{
	}

	public override Bitmap ConvertToBitmap(ImgFile entity)
	{
		byte[] data = entity.Data;
		ColorBits type = entity.Type;
		int size = entity.Width * entity.Height * ((type == ColorBits.ARGB_8888) ? 4 : 2);
		if (entity.CompressMode == CompressMode.ZLIB)
		{
			data = Zlib.Decompress(data, size);
		}
		return Bitmaps.FromArray(data, entity.Size, type);
	}

	public override byte[] ConvertToByte2(ImgFile entity, Size size2)
	{
		byte[] data = entity.Data;
		ColorBits type = entity.Type;
		int size3 = entity.Width * entity.Height * ((type == ColorBits.ARGB_8888) ? 4 : 2);
		if (entity.CompressMode == CompressMode.ZLIB)
		{
			data = Zlib.Decompress(data, size3);
		}
		return Bitmaps.FromArray2(data, size2, type);
	}

	public override byte[] ConvertToByte(ImgFile entity)
	{
		if (entity.Type > ColorBits.LINK)
		{
			entity.Type -= 4;
		}
		if (entity.CompressMode > CompressMode.ZLIB)
		{
			entity.CompressMode = CompressMode.ZLIB;
		}
		return entity.Picture.ToArray(entity.Type);
	}

	public override void NewImage(int count, ColorBits type, int index)
	{
		if (count < 1)
		{
			return;
		}
		ImgFile[] array = new ImgFile[count];
		array[0] = new ImgFile(Pack)
		{
			Index = index
		};
		if (type != ColorBits.LINK)
		{
			array[0].Type = type;
		}
		for (int i = 1; i < count; i++)
		{
			array[i] = new ImgFile(Pack)
			{
				Type = type
			};
			if (type == ColorBits.LINK)
			{
				array[i].Target = array[0];
			}
			array[i].Index = index + i;
		}
		Pack.ImgList.InsertRange(index, array);
	}

	public override byte[] AdjustData()
	{
		using MemoryStream memoryStream = new MemoryStream();
		foreach (ImgFile img in Pack.ImgList)
		{
			memoryStream.WriteInt((int)img.Type);
			if (img.Type == ColorBits.LINK && img.Target != null)
			{
				memoryStream.WriteInt(img.Target.Index);
				continue;
			}
			memoryStream.WriteInt((int)img.CompressMode);
			memoryStream.WriteInt(img.Width);
			memoryStream.WriteInt(img.Height);
			memoryStream.WriteInt(img.Length);
			memoryStream.WriteInt(img.X);
			memoryStream.WriteInt(img.Y);
			memoryStream.WriteInt(img.FrameWidth);
			memoryStream.WriteInt(img.FrameHeight);
		}
		Pack.IndexLength = memoryStream.Length;
		foreach (ImgFile img2 in Pack.ImgList)
		{
			if (img2.Type != ColorBits.LINK)
			{
				memoryStream.Write(img2.Data);
			}
		}
		return memoryStream.ToArray();
	}

	public override bool CreateFromStream(Stream stream)
	{
		Dictionary<ImgFile, int> dictionary = new Dictionary<ImgFile, int>();
		long num = stream.Position + Pack.IndexLength;
		for (int i = 0; i < Pack.Count; i++)
		{
			ImgFile imgFile = new ImgFile(Pack);
			imgFile.Index = Pack.ImgList.Count;
			int num2 = stream.ReadInt();
			if (!ColorBitsHas.Contains(num2))
			{
				Pack.ImgList.Clear();
				Pack.Count = 0;
				throw new Exception("NPK已被加密 已忽略...");
			}
			imgFile.Type = (ColorBits)num2;
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
		}
		if (stream.Position < num)
		{
			Pack.ImgList.Clear();
			return false;
		}
		ImgFile[] array = Pack.ImgList.ToArray();
		foreach (ImgFile imgFile2 in array)
		{
			if (imgFile2.Type == ColorBits.LINK)
			{
				if (!dictionary.ContainsKey(imgFile2) || dictionary[imgFile2] >= Pack.ImgList.Count || dictionary[imgFile2] <= -1 || dictionary[imgFile2] == imgFile2.Index)
				{
					Pack.ImgList.Clear();
					return false;
				}
				imgFile2.Target = Pack.ImgList[dictionary[imgFile2]];
				imgFile2.Size = imgFile2.Target.Size;
				imgFile2.FrameSize = imgFile2.Target.FrameSize;
				imgFile2.Location = imgFile2.Target.Location;
			}
			else
			{
				if (imgFile2.CompressMode == CompressMode.NONE)
				{
					imgFile2.Length = imgFile2.Width * imgFile2.Height * ((imgFile2.Type == ColorBits.ARGB_8888) ? 4 : 2);
				}
				byte[] array2 = new byte[imgFile2.Length];
				stream.Read(array2);
				imgFile2.Data = array2;
			}
		}
		return true;
	}

	public override bool CreateFromStream2(Stream stream, ConcurrentHashSet<int> code, ConcurrentDictionary<int, ImageSource> outDic)
	{
		Dictionary<ImgFile, int> dictionary = new Dictionary<ImgFile, int>();
		long num = stream.Position + Pack.IndexLength;
		for (int i = 0; i < Pack.Count; i++)
		{
			int num2 = stream.ReadInt();
			if (!ColorBitsHas.Contains(num2))
			{
				Pack.ImgList.Clear();
				Pack.Count = 0;
				throw new Exception("NPK已被加密 已忽略...");
			}
			ColorBits colorBits = (ColorBits)num2;
			ImgFile imgFile = new ImgFile(Pack);
			imgFile.Index = i;
			imgFile.Type = colorBits;
			Pack.ImgList.Add(imgFile);
			if (!code.Contains(i))
			{
				if (colorBits == ColorBits.LINK)
				{
					dictionary.Add(imgFile, stream.ReadInt());
					continue;
				}
				imgFile.CompressMode = (CompressMode)stream.ReadInt();
				imgFile.Width = stream.ReadInt();
				imgFile.Height = stream.ReadInt();
				imgFile.Length = stream.ReadInt();
				stream.Position += 16L;
			}
			else if (imgFile.Type == ColorBits.LINK)
			{
				dictionary.Add(imgFile, stream.ReadInt());
			}
			else
			{
				imgFile.CompressMode = (CompressMode)stream.ReadInt();
				imgFile.Width = stream.ReadInt();
				imgFile.Height = stream.ReadInt();
				imgFile.Length = stream.ReadInt();
				stream.Position += 16L;
			}
		}
		if (stream.Position < num)
		{
			Pack.ImgList.Clear();
			return false;
		}
		ImgFile[] array = Pack.ImgList.ToArray();
		foreach (ImgFile imgFile2 in array)
		{
			if (imgFile2.Type == ColorBits.LINK)
			{
				if (dictionary.ContainsKey(imgFile2) && dictionary[imgFile2] < Pack.ImgList.Count && dictionary[imgFile2] > -1 && dictionary[imgFile2] != imgFile2.Index)
				{
					imgFile2.Target = Pack.ImgList[dictionary[imgFile2]];
					imgFile2.Size = imgFile2.Target.Size;
					imgFile2.FrameSize = imgFile2.Target.FrameSize;
					imgFile2.Location = imgFile2.Target.Location;
					continue;
				}
				Pack.ImgList.Clear();
				return false;
			}
			if (imgFile2.CompressMode == CompressMode.NONE)
			{
				imgFile2.Length = imgFile2.Width * imgFile2.Height * ((imgFile2.Type == ColorBits.ARGB_8888) ? 4 : 2);
			}
			if (!code.Contains(imgFile2.Index))
			{
				stream.Position += imgFile2.Length;
				continue;
			}
			byte[] array2 = new byte[imgFile2.Length];
			stream.Read(array2);
			imgFile2.Data = array2;
			ImageSource imageSouce = imgFile2.GetImageSouce();
			((Freezable)imageSouce).Freeze();
			outDic.TryAdd(imgFile2.Index, imageSouce);
		}
		return true;
	}

	public override void ConvertToVersion(ImgVersion version)
	{
		if (version == ImgVersion.Ver4 || version == ImgVersion.Ver6)
		{
			Extensions.ForEach(Pack.ImgList, delegate(ImgFile item)
			{
				item.Type = ColorBits.ARGB_1555;
			});
		}
	}
}
