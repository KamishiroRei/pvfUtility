using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using PvfCode.Dot;
using PvfCode.NPK.Utils.Lib;
using PvfCode.NPK.Utils.Models;
using PvfCode.NPK.Utils.Models.Enums;
using Utools;

namespace PvfCode.NPK.Utils;

public static class NpkCoder
{
	public class UtImgFileTempWrapper
	{
		public UtImgFileTemp Value { get; }

		public UtImgFileTempWrapper(UtImgFileTemp value)
		{
			Value = value;
		}
	}

	public const string NPK_FlAG = "NeoplePack_Bill";

	public const string IMG_FLAG = "Neople Img File";

	public const string IMAGE_FLAG = "Neople Image File";

	public const string IMAGE_DIR = "ImagePacks2";

	public const string SOUND_DIR = "SoundPacks";

	private const string KEY_HEADER = "puchikon@neople dungeon and fighter ";

	public static Encoding Encoding = System.Text.Encoding.UTF8;

	private static byte[] key;

	private static int _fileNameIdCounter = 1;

	private static byte[] Key
	{
		get
		{
			if (key != null)
			{
				return key;
			}
			byte[] array = new byte[256];
			int bytes = Encoding.GetBytes("puchikon@neople dungeon and fighter ", 0, "puchikon@neople dungeon and fighter ".Length, array, 0);
			byte[] bytes2 = Encoding.GetBytes("DNF");
			for (int i = bytes; i < 255; i++)
			{
				array[i] = bytes2[i % 3];
			}
			array[255] = 0;
			return key = array;
		}
	}

	public static string ReadPath(this Stream stream)
	{
		byte[] array = new byte[256];
		int i;
		for (i = 0; i < 256; i++)
		{
			array[i] = (byte)(stream.ReadByte() ^ Key[i]);
			if (array[i] == 0)
			{
				break;
			}
		}
		stream.Seek(255 - i);
		return System.Text.Encoding.GetEncoding("gbk").GetString(array, 0, i);
	}

	public static async Task<string> ReadPathNewAsync(this Stream stream)
	{
		byte[] data = new byte[256];
		int i;
		for (i = 0; i < 256; i++)
		{
			int num = await stream.ReadByteAsync();
			if (num == -1)
			{
				break;
			}
			data[i] = (byte)(num ^ Key[i]);
			if (data[i] == 0)
			{
				break;
			}
		}
		stream.Seek(255 - i, SeekOrigin.Current);
		return System.Text.Encoding.GetEncoding("gbk").GetString(data, 0, i);
	}

	private static void WritePath(this Stream stream, string str)
	{
		byte[] array = new byte[256];
		Encoding.GetBytes(str, 0, str.Length, array, 0);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] ^= Key[i];
		}
		stream.Write(array);
	}

	public static Bitmap ReadImage(Stream stream, ImgFile entity)
	{
		byte[] array = new byte[entity.Width * entity.Height * 4];
		for (int i = 0; i < array.Length; i += 4)
		{
			ColorBits colorBits = entity.Type;
			if (entity.Version == ImgVersion.Ver4 && colorBits == ColorBits.ARGB_1555)
			{
				colorBits = ColorBits.ARGB_8888;
			}
			PvfCode.NPK.Utils.Lib.Colors.ReadColor(stream, colorBits, array, i);
		}
		return array.FromArray(entity.Size);
	}

	private static byte[] CompileHash(byte[] data)
	{
		if (data.Length == 0)
		{
			return new byte[0];
		}
		try
		{
			using HMACSHA256 hMACSHA = new HMACSHA256();
			return hMACSHA.ComputeHash(data, 0, data.Length / 17 * 17);
		}
		catch
		{
			throw new FipsException();
		}
	}

	public static List<ImagePack> ReadInfo(Stream stream)
	{
		string text = stream.ReadString();
		List<ImagePack> list = new List<ImagePack>();
		if (text != "NeoplePack_Bill")
		{
			return list;
		}
		int num = stream.ReadInt();
		for (int i = 0; i < num; i++)
		{
			ImagePack item = new ImagePack
			{
				Offset = stream.ReadInt(),
				Length = stream.ReadInt(),
				Path = stream.ReadPath()
			};
			list.Add(item);
		}
		return list;
	}

	public static List<ImagePack> ReadNpk(Stream stream, string file)
	{
		List<ImagePack> list = new List<ImagePack>();
		if (stream.ReadString() == "NeoplePack_Bill")
		{
			stream.Seek(0L, SeekOrigin.Begin);
			list.AddRange(ReadInfo(stream));
			if (list.Count > 0)
			{
				stream.Seek(32L);
			}
		}
		else
		{
			ImagePack imagePack = new ImagePack();
			if (file != null)
			{
				imagePack.Path = file.GetSuffix();
			}
			list.Add(imagePack);
		}
		for (int i = 0; i < list.Count; i++)
		{
			long length = ((i < list.Count - 1) ? list[i + 1].Offset : stream.Length);
			ReadImg(stream, list[i], length);
		}
		return list;
	}

	public static List<ImagePack> ReadNpk(Stream stream)
	{
		return ReadNpk(stream, null);
	}

	public static List<ImagePack> ReadNpk(string filePath)
	{
		using FileStream stream = File.OpenRead(filePath);
		return ReadNpk(stream);
	}

	public static async Task<ResultData<Dictionary<string, UtImgFile>, string, Dictionary<int, string>>> ParallelReadNpkList2(string dirName)
	{
		_fileNameIdCounter = 1;
		StringBuilder buiError = new StringBuilder();
		await Task.Delay(1);
		ConcurrentDictionary<string, UtImgFileTemp> concurrentDictionary = new ConcurrentDictionary<string, UtImgFileTemp>();
		List<string> list = new List<string>(Directory.EnumerateFiles(dirName, "*.npk", SearchOption.TopDirectoryOnly));
		ConcurrentDictionary<int, string> concurrentDictionary2 = new ConcurrentDictionary<int, string>();
		foreach (string item in list)
		{
			ReadNpkInfo2(item, concurrentDictionary, buiError, concurrentDictionary2);
		}
		return new ResultData<Dictionary<string, UtImgFile>, string, Dictionary<int, string>>
		{
			Data = ((IEnumerable<KeyValuePair<string, UtImgFileTemp>>)concurrentDictionary).ToDictionary((Func<KeyValuePair<string, UtImgFileTemp>, string>)((KeyValuePair<string, UtImgFileTemp> it) => it.Key), (Func<KeyValuePair<string, UtImgFileTemp>, UtImgFile>)((KeyValuePair<string, UtImgFileTemp> it) => it.Value)),
			Data2 = buiError.ToString(),
			Data3 = concurrentDictionary2.ToDictionary((KeyValuePair<int, string> it) => it.Key, (KeyValuePair<int, string> it) => it.Value)
		};
	}

	private static void ReadNpkInfo2(string filePath, ConcurrentDictionary<string, UtImgFileTemp> dic, StringBuilder buiError, ConcurrentDictionary<int, string> npkFilePathDic)
	{
		if (!File.Exists(filePath))
		{
			return;
		}
		string fileName = Path.GetFileName(filePath);
		int fileNameId = Interlocked.Increment(ref _fileNameIdCounter);
		npkFilePathDic.TryAdd(fileNameId, fileName);
		try
		{
			using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
			if (!(fileStream.ReadString(System.Text.Encoding.Default) == "NeoplePack_Bill"))
			{
				return;
			}
			fileStream.Seek(0L, SeekOrigin.Begin);
			if (!(fileStream.ReadString(System.Text.Encoding.Default) == "NeoplePack_Bill"))
			{
				return;
			}
			int num = fileStream.ReadInt();
			if (num <= 0)
			{
				return;
			}
			(int, string)[] array = new(int, string)[num];
			for (int i = 0; i < num; i++)
			{
				int item = fileStream.ReadInt();
				fileStream.Seek(4L);
				string item2 = fileStream.ReadPath();
				array[i] = (item, item2);
			}
			fileStream.Seek(32L);
			_ = new byte[16];
			(int, string)[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				var (num2, text) = array2[j];
				fileStream.Seek(num2, SeekOrigin.Begin);
				string text2 = fileStream.ReadString();
				if (text2 == "Neople Img File" || text2 == "Neople Image File")
				{
					if (!dic.ContainsKey(text))
					{
						dic.TryAdd(text, new UtImgFileTemp(num2, fileNameId));
					}
					fileStream.Seek(12L);
					dic[text].Count = fileStream.ReadInt();
				}
			}
		}
		catch (IOException ex) when (ex.InnerException == null || ex.InnerException is ArgumentException)
		{
			if (ex.Message.Contains("because it is being used by another process"))
			{
				lock (buiError)
				{
					buiError.AppendLine("文件被其他进程占用无法读取：" + filePath);
					return;
				}
			}
		}
		catch (Exception ex2)
		{
			lock (buiError)
			{
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(12, 2, buiError);
				handler.AppendLiteral("读取文件 ");
				handler.AppendFormatted(filePath);
				handler.AppendLiteral(" 时出现错误：");
				handler.AppendFormatted(ex2.Message);
				buiError.AppendLine(ref handler);
			}
		}
	}

	public static ConcurrentDictionary<string, ConcurrentDictionary<int, ImageSource>> ReadNpkTreeIcon(ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentHashSet<int>>> tsource)
	{
		ConcurrentDictionary<string, ConcurrentDictionary<int, ImageSource>> DIC = new ConcurrentDictionary<string, ConcurrentDictionary<int, ImageSource>>();
		TaskFactory taskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(10));
		List<Task> list = new List<Task>();
		foreach (KeyValuePair<string, ConcurrentDictionary<string, ConcurrentHashSet<int>>> item in tsource)
		{
			list.Add(taskFactory.StartNew(delegate
			{
				ReadNpkInfoFrom(item.Key, item.Value, DIC);
			}));
		}
		Task.WaitAll(list.ToArray());
		return DIC;
	}

	private static void ReadNpkInfoFrom(string filePath, ConcurrentDictionary<string, ConcurrentHashSet<int>> li, ConcurrentDictionary<string, ConcurrentDictionary<int, ImageSource>> DIC)
	{
		try
		{
			using Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			if (stream.ReadString() == "NeoplePack_Bill")
			{
				stream.Seek(0L, SeekOrigin.Begin);
				if (ReadInfoFrom(stream, li, DIC))
				{
					stream.Seek(32L);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public static bool ReadInfoFrom(Stream stream, ConcurrentDictionary<string, ConcurrentHashSet<int>> list, ConcurrentDictionary<string, ConcurrentDictionary<int, ImageSource>> DIC)
	{
		if (stream.ReadString() != "NeoplePack_Bill")
		{
			return false;
		}
		int num = stream.ReadInt();
		ConcurrentHashSet<string> concurrentHashSet = new ConcurrentHashSet<string>();
		for (int i = 0; i < num; i++)
		{
			ImagePack imagePack = new ImagePack
			{
				Offset = stream.ReadInt(),
				Length = stream.ReadInt(),
				Path = stream.ReadPath()
			};
			if (!list.ContainsKey(imagePack.Path) || concurrentHashSet.Contains(imagePack.Path))
			{
				continue;
			}
			long position = stream.Position;
			concurrentHashSet.Add(imagePack.Path);
			stream.Seek(imagePack.Offset, SeekOrigin.Begin);
			string text = stream.ReadString();
			if (text == "Neople Img File")
			{
				imagePack.IndexLength = stream.ReadLong();
				imagePack.Version = (ImgVersion)stream.ReadInt();
				imagePack.Count = stream.ReadInt();
				ConcurrentDictionary<int, ImageSource> concurrentDictionary = new ConcurrentDictionary<int, ImageSource>();
				imagePack.IniHandle2(stream, list[imagePack.Path], concurrentDictionary);
				if (concurrentDictionary.Count > 0)
				{
					DIC.TryAdd(imagePack.Path, concurrentDictionary);
				}
			}
			else if (text == "Neople Image File")
			{
				ConcurrentDictionary<int, ImageSource> concurrentDictionary2 = new ConcurrentDictionary<int, ImageSource>();
				imagePack.Version = ImgVersion.Ver1;
				imagePack.IniHandle2(stream, list[imagePack.Path], concurrentDictionary2);
				if (concurrentDictionary2.Count > 0)
				{
					DIC.TryAdd(imagePack.Path, concurrentDictionary2);
				}
			}
			stream.Position = position;
		}
		return true;
	}

	public static void ReadImg(Stream stream, ImagePack pack, long length)
	{
		stream.Seek(pack.Offset, SeekOrigin.Begin);
		string text = stream.ReadString();
		if (text == "Neople Img File")
		{
			pack.IndexLength = stream.ReadLong();
			pack.Version = (ImgVersion)stream.ReadInt();
			pack.Count = stream.ReadInt();
			pack.InitHandle(stream);
			return;
		}
		if (text == "Neople Image File")
		{
			pack.Version = ImgVersion.Ver1;
		}
		else
		{
			if (length < 0)
			{
				length = stream.Length;
			}
			pack.Version = ImgVersion.Other;
			stream.Seek(pack.Offset, SeekOrigin.Begin);
			if (pack.Name.ToLower().EndsWith(".ogg"))
			{
				pack.Version = ImgVersion.Other;
				pack.IndexLength = length - stream.Position;
			}
		}
		pack.InitHandle(stream);
	}

	public static void ReadImg(Stream stream, ImagePack album)
	{
		ReadImg(stream, album, -1L);
	}

	public static ImagePack ReadImg(Stream stream, string path)
	{
		return ReadImg(stream, path, -1L);
	}

	public static ImagePack ReadImg(Stream stream, string path, long length)
	{
		ImagePack imagePack = new ImagePack
		{
			Path = path
		};
		ReadImg(stream, imagePack, length);
		return imagePack;
	}

	public static void ReadImg(byte[] data, ImagePack album)
	{
		ReadImg(data, album, -1L);
	}

	public static ImagePack ReadImg(byte[] data, string path)
	{
		return ReadImg(data, path, -1L);
	}

	public static void ReadImg(byte[] data, ImagePack album, long length)
	{
		using MemoryStream stream = new MemoryStream(data);
		ReadImg(stream, album, length);
	}

	public static ImagePack ReadImg(byte[] data, string path, long length)
	{
		using MemoryStream stream = new MemoryStream(data);
		return ReadImg(stream, path, length);
	}

	public static void WriteNpk(Stream stream, List<ImagePack> List)
	{
		int num = 52 + List.Count * 264;
		int num2 = 0;
		for (int i = 0; i < List.Count; i++)
		{
			List[i].Adjust();
			if (i > 0)
			{
				if (List[i].Target != null)
				{
					continue;
				}
				num += num2;
			}
			List[i].Offset = num;
			num2 = List[i].Length;
		}
		List.ForEach(delegate(ImagePack e)
		{
			if (e.Target != null)
			{
				e.Offset = e.Target.Offset;
				e.Length = e.Target.Length;
			}
		});
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteString("NeoplePack_Bill");
		memoryStream.WriteInt(List.Count);
		foreach (ImagePack item in List)
		{
			memoryStream.WriteInt(item.Offset);
			memoryStream.WriteInt(item.Length);
			memoryStream.WritePath(item.Path);
		}
		memoryStream.Close();
		byte[] array = memoryStream.ToArray();
		stream.Write(array);
		stream.Write(CompileHash(array));
		foreach (ImagePack item2 in List)
		{
			if (item2.Target == null)
			{
				stream.Write(item2.Data);
			}
		}
	}

	public static List<ImagePack> Find(IEnumerable<ImagePack> Items, params string[] args)
	{
		return Find(Items, allCheck: false, args);
	}

	public static List<ImagePack> Find(IEnumerable<ImagePack> Items, bool allCheck, params string[] args)
	{
		return new List<ImagePack>(Items.Where(delegate(ImagePack item)
		{
			if (!allCheck && args.Length == 0)
			{
				return true;
			}
			return (!allCheck || args[0].Equals(item.Name)) && args.All((string arg) => item.Path.Contains(arg));
		}));
	}

	public static string GetFilePath(this ImagePack file)
	{
		string text = file.Path;
		int num = text.LastIndexOf("/");
		if (num > -1)
		{
			text = text.Substring(0, num);
		}
		text = text.Replace("/", "_");
		return text + ".NPK";
	}

	public static List<ImagePack> Load(string file)
	{
		return Load(onlyPath: false, file);
	}

	public static List<ImagePack> Load(bool onlyPath, string file)
	{
		List<ImagePack> result = new List<ImagePack>();
		if (Directory.Exists(file))
		{
			return Load(onlyPath, Directory.GetFiles(file));
		}
		if (!File.Exists(file))
		{
			return result;
		}
		using FileStream stream = File.OpenRead(file);
		if (onlyPath)
		{
			return ReadInfo(stream);
		}
		return ReadNpk(stream, file);
	}

	public static List<ImagePack> Load(bool onlyPath, params string[] files)
	{
		List<ImagePack> list = new List<ImagePack>();
		foreach (string file in files)
		{
			list.AddRange(Load(onlyPath, file));
		}
		return list;
	}

	public static List<ImagePack> Load(params string[] files)
	{
		return Load(onlyPath: false, files);
	}

	public static ImagePack LoadWithPath(string file, string name)
	{
		using FileStream stream = File.OpenRead(file);
		return LoadWithPath(stream, name);
	}

	public static ImagePack LoadWithPath(Stream stream, string name)
	{
		List<ImagePack> list = LoadWithPathArray(stream, name);
		if (list.Count > 0)
		{
			return list[0];
		}
		return null;
	}

	public static List<ImagePack> LoadWithPathArray(string file, params string[] args)
	{
		using FileStream stream = File.OpenRead(file);
		return LoadWithPathArray(stream, args);
	}

	public static List<ImagePack> LoadWithPathArray(Stream stream, params string[] paths)
	{
		return LoadAll(stream, (ImagePack e) => Enumerable.Contains(paths, e.Path));
	}

	public static List<ImagePack> LoadAll(string file, Predicate<ImagePack> predicate)
	{
		using FileStream stream = File.OpenRead(file);
		return LoadAll(stream, predicate);
	}

	public static List<ImagePack> LoadAll(Stream stream, Predicate<ImagePack> predicate)
	{
		List<ImagePack> list = ReadInfo(stream);
		list = list.FindAll(predicate);
		foreach (ImagePack item in list)
		{
			stream.Seek(item.Offset, SeekOrigin.Begin);
			ReadImg(stream, item, stream.Length);
		}
		return list;
	}

	public static void Save(string file, List<ImagePack> list)
	{
		using FileStream stream = File.Open(file, FileMode.Create);
		WriteNpk(stream, list);
	}

	public static void SaveToDirectory(string dir, IEnumerable<ImagePack> array)
	{
		foreach (ImagePack item in array)
		{
			item.Save(dir + "/" + item.Name);
		}
	}

	public static void Compare(string gamePath, Action<ImagePack, ImagePack> restore, params ImagePack[] array)
	{
		Compare(gamePath, "ImagePacks2", restore, array);
	}

	public static void Compare(string gamePath, string dir, Action<ImagePack, ImagePack> restore, params ImagePack[] array)
	{
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		ImagePack[] array2 = array;
		foreach (ImagePack imagePack in array2)
		{
			string filePath = imagePack.GetFilePath();
			filePath = $"{gamePath}/{dir}/{filePath}";
			if (!dictionary.ContainsKey(filePath))
			{
				dictionary.Add(filePath, new List<string>());
			}
			dictionary[filePath].Add(imagePack.Path);
		}
		List<ImagePack> list = new List<ImagePack>();
		foreach (string key in dictionary.Keys)
		{
			list.AddRange(LoadWithPathArray(key, dictionary[key].ToArray()));
		}
		array2 = array;
		foreach (ImagePack imagePack2 in array2)
		{
			foreach (ImagePack item in list)
			{
				if (imagePack2.Path.Equals(item.Path))
				{
					restore(item, imagePack2);
				}
			}
		}
	}
}
