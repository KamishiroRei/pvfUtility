using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using PvfCode.Models.Pvf.Enums;
using Utools;

namespace PvfCode.Models.Pvf;

public abstract class PvfPack : ModelBase
{
	public delegate void DelegatePvfIsOpenChanged(bool pvfIsOpen);

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass61_0
	{
		public PvfFileType bVQ8i5nNTg;

		public _003C_003Ec__DisplayClass61_0()
		{
		}

		internal bool qHu8NZaM1L(KeyValuePair<string, PvfFile> it)
		{
			return it.Value.FileType == bVQ8i5nNTg;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass62_0
	{
		public string SGr8aWC24W;

		public _003C_003Ec__DisplayClass62_0()
		{
		}

		internal bool VkO8MP255q(KeyValuePair<string, PvfFile> item)
		{
			KeyValuePair<string, PvfFile> keyValuePair = item;
			return PathsHelper.IsPathMatch(keyValuePair.Key, SGr8aWC24W);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass63_0
	{
		public string svQ8UiMZc5;

		public _003C_003Ec__DisplayClass63_0()
		{
		}

		internal bool NXl8ItJ7Zf(KeyValuePair<string, PvfFile> item)
		{
			KeyValuePair<string, PvfFile> keyValuePair = item;
			return PathsHelper.IsPathMatch(keyValuePair.Key, svQ8UiMZc5);
		}
	}

	[CompilerGenerated]
	private DelegatePvfIsOpenChanged Qmfk6XqkRy;

	private bool CHykyv0NKF;

	[CompilerGenerated]
	private uint L5Zkw9D5H0;

	[CompilerGenerated]
	private int FrjkoA0hwL;

	[CompilerGenerated]
	private int DAuk28OIXJ;

	[CompilerGenerated]
	private int IwNktFiSaq;

	[CompilerGenerated]
	private byte[] jo8k9wkjl1;

	[CompilerGenerated]
	private EquipmentPartSetTable LslkRgUB3f;

	private string m5dkWejnIy;

	[CompilerGenerated]
	private Dictionary<string, PvfFile> txGkq5LLem;

	[CompilerGenerated]
	private readonly Stringtable FPDkz9cTKh;

	[CompilerGenerated]
	private readonly StringView QZgLAUTQVL;

	[CompilerGenerated]
	private readonly ListFileTable KArLneOoUp;

	public bool PvfIsOpen
	{
		get
		{
			return CHykyv0NKF;
		}
		set
		{
			CHykyv0NKF = value;
			DoNotify("PvfIsOpen");
			if (Qmfk6XqkRy != null)
			{
				Qmfk6XqkRy(value);
			}
		}
	}

	public uint _fileTreeChecksum
	{
		[CompilerGenerated]
		get
		{
			return L5Zkw9D5H0;
		}
		[CompilerGenerated]
		set
		{
			L5Zkw9D5H0 = value;
		}
	}

	public int _fileTreeLength
	{
		[CompilerGenerated]
		get
		{
			return FrjkoA0hwL;
		}
		[CompilerGenerated]
		set
		{
			FrjkoA0hwL = value;
		}
	}

	public int _guidLen
	{
		[CompilerGenerated]
		get
		{
			return DAuk28OIXJ;
		}
		[CompilerGenerated]
		set
		{
			DAuk28OIXJ = value;
		}
	}

	public int FileVersion
	{
		[CompilerGenerated]
		get
		{
			return IwNktFiSaq;
		}
		[CompilerGenerated]
		set
		{
			IwNktFiSaq = value;
		}
	}

	public byte[] Guid
	{
		[CompilerGenerated]
		get
		{
			return jo8k9wkjl1;
		}
		[CompilerGenerated]
		set
		{
			jo8k9wkjl1 = value;
		}
	}

	public EquipmentPartSetTable EquipmentPartSetTable
	{
		[CompilerGenerated]
		get
		{
			return LslkRgUB3f;
		}
		[CompilerGenerated]
		set
		{
			LslkRgUB3f = value;
		}
	}

	public string PvfPackFilePath
	{
		get
		{
			return m5dkWejnIy;
		}
		set
		{
			m5dkWejnIy = value;
			DoNotify("PvfPackFilePath");
			DoNotify("PvfPackDir");
			DoNotify("PvfPackDefaultExtractDir");
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

	public Dictionary<string, PvfFile> FileList
	{
		[CompilerGenerated]
		get
		{
			return txGkq5LLem;
		}
		[CompilerGenerated]
		set
		{
			txGkq5LLem = value;
		}
	}

	public Stringtable Strtable
	{
		[CompilerGenerated]
		get
		{
			return FPDkz9cTKh;
		}
	}

	public StringView Strview
	{
		[CompilerGenerated]
		get
		{
			return QZgLAUTQVL;
		}
	}

	public ListFileTable ListFileTable
	{
		[CompilerGenerated]
		get
		{
			return KArLneOoUp;
		}
	}

	public EncodingType OverAllEncodingType
	{
		get
		{
			return AppSetting.Instance.PvfConfig.DefaultEncoding;
		}
		set
		{
			AppSetting.Instance.PvfConfig.DefaultEncoding = value;
			DoNotify("OverAllEncodingType");
			if (PvfIsOpen && Strtable != null)
			{
				if (Strtable.IsStringTableUpdated)
				{
					GetFile("stringtable.bin")?.WriteFileData(Strtable.CreateStringTable());
				}
				Strtable?.Loadstringtable(GetFile("stringtable.bin").Data, value, this);
				Strview?.InitStringData(GetFile("n_string.lst"), this, value);
			}
		}
	}

	public event DelegatePvfIsOpenChanged PvfIsOpenChanged
	{
		[CompilerGenerated]
		add
		{
			DelegatePvfIsOpenChanged delegatePvfIsOpenChanged = Qmfk6XqkRy;
			DelegatePvfIsOpenChanged delegatePvfIsOpenChanged2;
			do
			{
				delegatePvfIsOpenChanged2 = delegatePvfIsOpenChanged;
				DelegatePvfIsOpenChanged value2 = (DelegatePvfIsOpenChanged)Delegate.Combine(delegatePvfIsOpenChanged2, value);
				delegatePvfIsOpenChanged = Interlocked.CompareExchange(ref Qmfk6XqkRy, value2, delegatePvfIsOpenChanged2);
			}
			while ((object)delegatePvfIsOpenChanged != delegatePvfIsOpenChanged2);
		}
		[CompilerGenerated]
		remove
		{
			DelegatePvfIsOpenChanged delegatePvfIsOpenChanged = Qmfk6XqkRy;
			DelegatePvfIsOpenChanged delegatePvfIsOpenChanged2;
			do
			{
				delegatePvfIsOpenChanged2 = delegatePvfIsOpenChanged;
				DelegatePvfIsOpenChanged value2 = (DelegatePvfIsOpenChanged)Delegate.Remove(delegatePvfIsOpenChanged2, value);
				delegatePvfIsOpenChanged = Interlocked.CompareExchange(ref Qmfk6XqkRy, value2, delegatePvfIsOpenChanged2);
			}
			while ((object)delegatePvfIsOpenChanged != delegatePvfIsOpenChanged2);
		}
	}

	public PvfPack()
	{
		FPDkz9cTKh = new Stringtable();
		QZgLAUTQVL = new StringView();
		KArLneOoUp = new ListFileTable();
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
		_003C_003Ec__DisplayClass61_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass61_0();
		CS_0024_003C_003E8__locals2.bVQ8i5nNTg = fileType;
		if (FileList == null)
		{
			return new List<string>();
		}
		return from it in FileList
			where it.Value.FileType == CS_0024_003C_003E8__locals2.bVQ8i5nNTg
			select it.Key;
	}

	public IEnumerable<string> GetFiles(string path)
	{
		_003C_003Ec__DisplayClass62_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass62_0();
		if (string.IsNullOrEmpty(path))
		{
			return FileList.Keys;
		}
		CS_0024_003C_003E8__locals2.SGr8aWC24W = PathsHelper.PathFix(path);
		return FileList.Where<KeyValuePair<string, PvfFile>>(delegate(KeyValuePair<string, PvfFile> item)
		{
			KeyValuePair<string, PvfFile> keyValuePair = item;
			return PathsHelper.IsPathMatch(keyValuePair.Key, CS_0024_003C_003E8__locals2.SGr8aWC24W);
		}).Select(delegate(KeyValuePair<string, PvfFile> item)
		{
			KeyValuePair<string, PvfFile> keyValuePair = item;
			return keyValuePair.Key;
		});
	}

	public PvfFile[] GetFileObjs(string path)
	{
		_003C_003Ec__DisplayClass63_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass63_0();
		CS_0024_003C_003E8__locals2.svQ8UiMZc5 = PathsHelper.PathFix(path);
		return FileList.Where<KeyValuePair<string, PvfFile>>(delegate(KeyValuePair<string, PvfFile> item)
		{
			KeyValuePair<string, PvfFile> keyValuePair = item;
			return PathsHelper.IsPathMatch(keyValuePair.Key, CS_0024_003C_003E8__locals2.svQ8UiMZc5);
		}).Select(delegate(KeyValuePair<string, PvfFile> item)
		{
			KeyValuePair<string, PvfFile> keyValuePair = item;
			return keyValuePair.Value;
		}).ToArray();
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
