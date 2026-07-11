using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using PvfCode.NPK.Utils.Lib;
using PvfCode.NPK.Utils.Models;
using PvfCode.NPK.Utils.Models.Enums;
using Utools;

namespace PvfCode.NPK.Utils.Handle;

public class FirstHandler : SecondHandler
{
	public FirstHandler(ImagePack album)
		: base(album)
	{
	}

	public override byte[] AdjustData()
	{
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteString("Neople Image File");
		memoryStream.Write(new byte[6]);
		memoryStream.WriteInt((int)Pack.Version);
		memoryStream.WriteInt(Pack.Count);
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
			memoryStream.Write(img.Data);
		}
		Pack.IndexLength = memoryStream.Length;
		return memoryStream.ToArray();
	}

	public override bool CreateFromStream(Stream stream)
	{
		Pack.IndexLength = stream.ReadInt();
		stream.Seek(2L);
		Pack.Version = (ImgVersion)stream.ReadInt();
		Pack.Count = stream.ReadInt();
		Dictionary<ImgFile, int> dictionary = new Dictionary<ImgFile, int>();
		for (int i = 0; i < Pack.Count; i++)
		{
			int num = stream.ReadInt();
			if (!ColorBitsHas.Contains(num))
			{
				Pack.ImgList.Clear();
				Pack.Count = 0;
				throw new Exception("NPK已被加密 已忽略...");
			}
			ImgFile imgFile = new ImgFile(Pack)
			{
				Index = Pack.ImgList.Count,
				Type = (ColorBits)num
			};
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
			if (imgFile.CompressMode == CompressMode.NONE)
			{
				imgFile.Length = imgFile.Size.Width * imgFile.Size.Height * ((imgFile.Type == ColorBits.ARGB_8888) ? 4 : 2);
			}
			byte[] array = new byte[imgFile.Length];
			stream.Read(array);
			imgFile.Data = array;
		}
		foreach (ImgFile img in Pack.ImgList)
		{
			if (img.Type == ColorBits.LINK)
			{
				if (!dictionary.ContainsKey(img) || dictionary[img] <= -1 || dictionary[img] >= Pack.ImgList.Count || dictionary[img] == img.Index)
				{
					Pack.ImgList.Clear();
					return false;
				}
				img.Target = Pack.ImgList[dictionary[img]];
				img.Size = img.Target.Size;
				img.FrameSize = img.Target.FrameSize;
				img.Location = img.Target.Location;
			}
		}
		return true;
	}

	public override bool CreateFromStream2(Stream stream, ConcurrentHashSet<int> code, ConcurrentDictionary<int, ImageSource> outDic)
	{
		Pack.IndexLength = stream.ReadInt();
		stream.Seek(2L);
		Pack.Version = (ImgVersion)stream.ReadInt();
		Pack.Count = stream.ReadInt();
		Dictionary<ImgFile, int> dictionary = new Dictionary<ImgFile, int>();
		for (int i = 0; i < Pack.Count; i++)
		{
			int num = stream.ReadInt();
			if (!ColorBitsHas.Contains(num))
			{
				Pack.ImgList.Clear();
				Pack.Count = 0;
				throw new Exception("NPK已被加密 已忽略...");
			}
			ImgFile imgFile = new ImgFile(Pack)
			{
				Index = Pack.ImgList.Count,
				Type = (ColorBits)num
			};
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
			if (imgFile.CompressMode == CompressMode.NONE)
			{
				imgFile.Length = imgFile.Size.Width * imgFile.Size.Height * ((imgFile.Type == ColorBits.ARGB_8888) ? 4 : 2);
			}
			byte[] array = new byte[imgFile.Length];
			stream.Read(array);
			imgFile.Data = array;
		}
		ImgFile[] array2 = Pack.ImgList.ToArray();
		foreach (ImgFile imgFile2 in array2)
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
				if (!code.Contains(imgFile2.Index))
				{
					stream.Position += imgFile2.Length;
					continue;
				}
				ImageSource imageSouce = imgFile2.GetImageSouce();
				((Freezable)imageSouce).Freeze();
				outDic.TryAdd(imgFile2.Index, imageSouce);
			}
		}
		return true;
	}
}
