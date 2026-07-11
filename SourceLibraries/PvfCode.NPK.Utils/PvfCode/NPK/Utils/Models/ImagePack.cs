using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using PvfCode.NPK.Utils.Handle;
using PvfCode.NPK.Utils.Lib;
using PvfCode.NPK.Utils.Models.Enums;
using Swordfish.NET.Collections;
using Swordfish.NET.Collections.Auxiliary;
using Utools;

namespace PvfCode.NPK.Utils.Models;

public class ImagePack : NpkTreeBase
{
	private int _tabindex;

	public string Path { get; set; } = string.Empty;

	public string Name
	{
		get
		{
			return Path.GetSuffix();
		}
		set
		{
			Path = Path.Replace(Name, value);
		}
	}

	[LSIgnore]
	public HandlerBase Handler { get; set; }

	public byte[] Data { get; set; }

	public int Count
	{
		get
		{
			return GetProperty(() => Count);
		}
		set
		{
			SetProperty(() => Count, value);
		}
	}

	public int Length { get; set; }

	public int Offset { get; set; }

	public long IndexLength { get; set; }

	public ConcurrentObservableCollection<ImgFile> ImgList { get; set; }

	public ImgVersion Version { get; set; } = ImgVersion.Ver2;

	public List<System.Drawing.Color> CurrentTable
	{
		get
		{
			if (TableIndex > -1 && TableIndex < Tables.Count)
			{
				return Tables[TableIndex];
			}
			return new List<System.Drawing.Color>();
		}
	}

	public int TableIndex
	{
		get
		{
			return _tabindex;
		}
		set
		{
			if (_tabindex != value)
			{
				Refresh();
				_tabindex = Math.Min(value, Tables.Count - 1);
			}
		}
	}

	public List<List<System.Drawing.Color>> Tables { get; set; } = new List<List<System.Drawing.Color>>
	{
		new List<System.Drawing.Color>()
	};

	public ImagePack Target { get; set; }

	public ImgFile this[int index]
	{
		get
		{
			return ImgList[index];
		}
		set
		{
			if (index < ImgList.Count)
			{
				ImgList[index] = value;
			}
			else
			{
				ImgList.Add(value);
			}
		}
	}

	public ImagePack()
	{
		ImgList = new ConcurrentObservableCollection<ImgFile>();
	}

	public void Refresh()
	{
		Extensions.ForEach(ImgList, delegate(ImgFile e)
		{
			e.Picture = null;
		});
	}

	public bool InitHandle(Stream stream)
	{
		Handler = HandlerBase.CreateHandler(Version, this);
		if (Handler != null && stream != null)
		{
			return Handler.CreateFromStream(stream);
		}
		return false;
	}

	public bool IniHandle2(Stream stream, ConcurrentHashSet<int> code, ConcurrentDictionary<int, ImageSource> outDic)
	{
		Handler = HandlerBase.CreateHandler(Version, this);
		if (Handler != null && stream != null)
		{
			return Handler.CreateFromStream2(stream, code, outDic);
		}
		return false;
	}

	public byte[] ConvertToByte(ImgFile entity)
	{
		return Handler.ConvertToByte(entity);
	}

	public void Replace(ImagePack album)
	{
		album = album.Clone();
		Version = album.Version;
		Tables = album.Tables;
		TableIndex = album.TableIndex;
		Handler = album.Handler;
		Handler.Pack = this;
		ImgList.Clear();
		ImgList.AddRange(album.ImgList);
		AdjustIndex();
	}

	public void ConvertTo(ImgVersion version)
	{
		Handler.ConvertToVersion(version);
		Version = version;
		InitHandle(null);
	}

	public void AdjustIndex()
	{
		for (int i = 0; i < ImgList.Count; i++)
		{
			ImgList[i].Index = i;
			ImgList[i].Parent = this;
		}
	}

	public void Hide()
	{
		int count = ImgList.Count;
		ImgList.Clear();
		Tables = new List<List<System.Drawing.Color>>
		{
			new List<System.Drawing.Color>()
		};
		TableIndex = 0;
		ConvertTo(ImgVersion.Ver2);
		NewImage(count, ColorBits.LINK, -1);
	}

	public void HideImage(ImgFile img)
	{
		ImgFile value = new ImgFile(this);
		ImgList[img.Index] = value;
	}

	public ImagePack Clone()
	{
		Adjust();
		ImagePack imagePack = NpkCoder.ReadImg(Data, Path);
		imagePack.TableIndex = TableIndex;
		return imagePack;
	}

	public void Save(Stream stream)
	{
		Adjust();
		stream.Write(Data);
	}

	public void Save(string file)
	{
		using FileStream stream = new FileStream(file, FileMode.Create);
		Save(stream);
	}

	public bool Equals(ImagePack al)
	{
		return Path.Equals(al?.Path);
	}

	public void CreateFromStream(Stream stream)
	{
		Handler.CreateFromStream(stream);
	}

	public Bitmap ConvertToBitmap(ImgFile entity)
	{
		return Handler.ConvertToBitmap(entity);
	}

	public byte[] ConvertToByte2(ImgFile entity, Size size2)
	{
		return Handler.ConvertToByte2(entity, size2);
	}

	public void NewImage(int count, ColorBits type, int index)
	{
		Handler.NewImage(count, type, index);
		AdjustIndex();
	}

	public void Adjust()
	{
		if (Target == null)
		{
			AdjustIndex();
			Handler.Adjust();
		}
	}

	public IEnumerator<ImgFile> GetEnumerator()
	{
		return ImgList.GetEnumerator();
	}

	public void UpdateCount()
	{
		Count = ((ImgList != null) ? ImgList.Count : 0);
	}

	public void Delete(IList<ImgFile> files)
	{
		ImgList.RemoveRange(files);
		AdjustIndex();
		UpdateCount();
	}
}
