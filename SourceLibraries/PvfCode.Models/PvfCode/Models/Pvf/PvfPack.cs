using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PvfCode.Models.Pvf.Enums;
using Utools;

namespace PvfCode.Models.Pvf;

public abstract class PvfPack : ModelBase
{
	public delegate void DelegatePvfIsOpenChanged(bool pvfIsOpen);

	private bool pvfIsOpen;

	private string pvfPackFilePath;

	public bool PvfIsOpen
	{
		get
		{
			return pvfIsOpen;
		}
		set
		{
			pvfIsOpen = value;
			DoNotify(nameof(PvfIsOpen));
			PvfIsOpenChanged?.Invoke(value);
		}
	}

	public uint _fileTreeChecksum { get; set; }

	public int _fileTreeLength { get; set; }

	public int _guidLen { get; set; }

	public int FileVersion { get; set; }

	public byte[] Guid { get; set; }

	public EquipmentPartSetTable EquipmentPartSetTable { get; set; }

	public string PvfPackFilePath
	{
		get
		{
			return pvfPackFilePath;
		}
		set
		{
			pvfPackFilePath = value;
			DoNotify(nameof(PvfPackFilePath));
			DoNotify(nameof(PvfPackDir));
			DoNotify(nameof(PvfPackDefaultExtractDir));
		}
	}

	public string PvfPackDir
	{
		get
		{
			if (string.IsNullOrEmpty(PvfPackFilePath))
			{
				return null;
			}
			return Path.GetDirectoryName(PvfPackFilePath);
		}
	}

	public string PvfPackDefaultExtractDir
	{
		get
		{
			if (string.IsNullOrEmpty(PvfPackFilePath))
			{
				return null;
			}
			return Path.Combine(Path.GetDirectoryName(PvfPackFilePath), "script");
		}
	}

	public Dictionary<string, PvfFile> FileList { get; set; }

	/// <summary>自最近一次成功写盘后是否存在未保存变更（内容/结构修改、导入、删除、重命名等）。
	/// 自动备份据此跳过无变更周期，避免无意义的整包重建。</summary>
	public bool HasUnsavedChanges { get; set; }

	public Stringtable Strtable { get; }

	public StringView Strview { get; }

	public ListFileTable ListFileTable { get; }

	public EncodingType OverAllEncodingType
	{
		get
		{
			return AppSetting.Instance.PvfConfig.DefaultEncoding;
		}
		set
		{
			AppSetting.Instance.PvfConfig.DefaultEncoding = value;
			DoNotify(nameof(OverAllEncodingType));
			if (PvfIsOpen && Strtable != null)
			{
				PvfFile stringTableFile = GetFile("stringtable.bin");
				if (stringTableFile == null)
				{
					// 110/NKPI 虚拟串表由名称池生成、文本不做编码转换，无需重建
					return;
				}
				if (Strtable.IsStringTableUpdated)
				{
					stringTableFile.WriteFileData(Strtable.CreateStringTable());
				}
				Strtable?.Loadstringtable(stringTableFile.Data, value, this);
				Strview?.InitStringData(GetFile("n_string.lst"), this, value);
			}
		}
	}

	public event DelegatePvfIsOpenChanged PvfIsOpenChanged;

	public PvfPack()
	{
		Strtable = new Stringtable();
		Strview = new StringView();
		ListFileTable = new ListFileTable();
		EquipmentPartSetTable = new EquipmentPartSetTable();
	}

	public int? GetItemCode(string filePath)
	{
		FileList.TryGetValue(filePath, out PvfFile value);
		return GetItemCode(value);
	}

	public int? GetItemCode(PvfFile? file)
	{
		return file?.ItemCode;
	}

	public HashSet<int> GetItemCodes(IEnumerable<string> fileList)
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (string file in fileList)
		{
			FileList.TryGetValue(file, out PvfFile value);
			if (value != null && value.ItemCode.HasValue)
			{
				hashSet.Add(value.ItemCode.Value);
			}
		}
		return hashSet.ToHashSet();
	}

	public IEnumerable<string> GetFiles(string path, PvfFileType? fileType)
	{
		if (!fileType.HasValue)
		{
			return GetFiles(path);
		}
		if (string.IsNullOrEmpty(path))
		{
			return FileList.Keys;
		}
		string path2 = PathsHelper.PathFix(path);
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, PvfFile> file in FileList)
		{
			if (file.Value.FileType == fileType.Value && PathsHelper.IsPathMatch(file.Key, path2))
			{
				list.Add(file.Key);
			}
		}
		return list;
	}

	public IEnumerable<string> GetFiles(PvfFileType fileType)
	{
		if (FileList == null)
		{
			return new List<string>();
		}
		return from it in FileList
			where it.Value.FileType == fileType
			select it.Key;
	}

	public IEnumerable<string> GetFiles(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return FileList.Keys;
		}
		string fixedPath = PathsHelper.PathFix(path);
		return FileList.Where(item => PathsHelper.IsPathMatch(item.Key, fixedPath)).Select(item => item.Key);
	}

	public PvfFile[] GetFileObjs(string path)
	{
		string fixedPath = PathsHelper.PathFix(path);
		return FileList.Where(item => PathsHelper.IsPathMatch(item.Key, fixedPath)).Select(item => item.Value).ToArray();
	}

	public PvfFile GetFile(string filePath)
	{
		if (filePath == null)
		{
			return null;
		}
		if (!FileList.TryGetValue(filePath.Replace('\\', '/').ToLower(), out PvfFile value))
		{
			return null;
		}
		return value;
	}

	public List<PvfFile> GetFiles(IEnumerable<string> paths)
	{
		List<PvfFile> list = new List<PvfFile>();
		foreach (string path in paths)
		{
			if (FileList.TryGetValue(path, out PvfFile value))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public bool FileAny(string filePath)
	{
		return FileList.ContainsKey(filePath);
	}

	public abstract bool SaveFileText(string filePath, string fileText, EncodingType? encoding = null);

	public abstract bool SaveFileText(PvfFile file, string fileText, EncodingType? encoding = null);

	public abstract string GetItemName(string filePath);

	public abstract string GetItemName(PvfFile file);
}
