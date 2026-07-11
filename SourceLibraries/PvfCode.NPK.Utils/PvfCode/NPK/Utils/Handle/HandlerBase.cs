using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media;
using PvfCode.NPK.Utils.Lib;
using PvfCode.NPK.Utils.Models;
using PvfCode.NPK.Utils.Models.Enums;
using Utools;

namespace PvfCode.NPK.Utils.Handle;

public abstract class HandlerBase
{
	public HashSet<int> ColorBitsHas = new HashSet<int> { 0, 14, 15, 16, 17, 18, 19, 20 };

	private static readonly Dictionary<ImgVersion, Type> Dic;

	[LSIgnore]
	public ImagePack Pack;

	public static List<ImgVersion> Versions => Dic.Keys.ToList();

	public ImgVersion Version { get; } = ImgVersion.Ver2;

	static HandlerBase()
	{
		Dic = new Dictionary<ImgVersion, Type>();
		Regisity();
	}

	public HandlerBase(ImagePack album)
	{
		Pack = album;
	}

	public static HandlerBase CreateHandler(ImgVersion version, ImagePack album)
	{
		return Dic[version].CreateInstance(album) as HandlerBase;
	}

	public abstract bool CreateFromStream(Stream stream);

	public virtual bool CreateFromStream2(Stream stream, ConcurrentHashSet<int> code, ConcurrentDictionary<int, ImageSource> outDic)
	{
		return false;
	}

	public abstract Bitmap ConvertToBitmap(ImgFile entity);

	public abstract byte[] ConvertToByte2(ImgFile entity, Size size2);

	public abstract byte[] ConvertToByte(ImgFile entity);

	public virtual void NewImage(int count, ColorBits type, int index)
	{
	}

	public void Adjust()
	{
		foreach (ImgFile img in Pack.ImgList)
		{
			img.Adjust();
		}
		Pack.Count = Pack.ImgList.Count;
		MemoryStream memoryStream = new MemoryStream();
		byte[] array = AdjustData();
		if (Pack.Version > ImgVersion.Ver1)
		{
			memoryStream.WriteString("Neople Img File");
			memoryStream.WriteLong(Pack.IndexLength);
			memoryStream.WriteInt((int)Pack.Version);
			memoryStream.WriteInt(Pack.Count);
		}
		memoryStream.Write(array);
		memoryStream.Close();
		Pack.Data = memoryStream.ToArray();
		Pack.Length = Pack.Data.Length;
	}

	public static void Regisity(ImgVersion version, Type type)
	{
		if (Dic.ContainsKey(version))
		{
			Dic.Remove(version);
		}
		Dic.Add(version, type);
	}

	public static void Regisity()
	{
		Regisity(ImgVersion.Other, typeof(OtherHandler));
		Regisity(ImgVersion.Ver1, typeof(FirstHandler));
		Regisity(ImgVersion.Ver2, typeof(SecondHandler));
		Regisity(ImgVersion.Ver4, typeof(FourthHandler));
		Regisity(ImgVersion.Ver5, typeof(FifthHandler));
		Regisity(ImgVersion.Ver6, typeof(SixthHandler));
	}

	public virtual void ConvertToVersion(ImgVersion version)
	{
	}

	public virtual byte[] AdjustData()
	{
		return new byte[0];
	}
}
