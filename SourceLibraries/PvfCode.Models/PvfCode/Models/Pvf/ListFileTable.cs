using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevExpress.Mvvm.Native;
using PvfCode.LoggerBase;

namespace PvfCode.Models.Pvf;

public class ListFileTable
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass19_0
	{
		public Dictionary<int, LstItem> qsI8rChwTb;

		public ConcurrentBag<string> OCb8eveinb;

		public _003C_003Ec__DisplayClass19_0()
		{
		}

		internal void m1v8O3vSsa(int code)
		{
			if (qsI8rChwTb.TryGetValue(code, out LstItem value))
			{
				OCb8eveinb.Add(value.FullPath);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass20_0
	{
		public ConcurrentBag<int> W6b8xgGWUQ;

		public _003C_003Ec__DisplayClass20_0()
		{
		}

		internal void xmx8jlabMV(string item)
		{
			if (int.TryParse(item, out var result))
			{
				W6b8xgGWUQ.Add(result);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass25_0
	{
		public ListFileTable bKK8Sa0iu3;

		public IEnumerable<string> R7o8BEo1wn;

		public List<string> IMV84dNHcC;

		public _003C_003Ec__DisplayClass25_0()
		{
		}

		internal void DsD8m7E9u4(int itemCode)
		{
			string text = bKK8Sa0iu3.ItemCodeConvertFilePath(R7o8BEo1wn, itemCode);
			if (!string.IsNullOrEmpty(text))
			{
				IMV84dNHcC.Add(text);
			}
		}
	}

	private Ilogger m1kklH3VZj;

	public string[] EquAndStkLstNames;

	[CompilerGenerated]
	private Dictionary<string, Dictionary<int, LstItem>> Yeukfp6uye;

	[CompilerGenerated]
	private Dictionary<string, int> cv9kheKrEZ;

	private Dictionary<string, string> jcWkTfuNxm;

	public Dictionary<string, Dictionary<int, LstItem>> CodeDic
	{
		[CompilerGenerated]
		get
		{
			return Yeukfp6uye;
		}
		[CompilerGenerated]
		set
		{
			Yeukfp6uye = value;
		}
	}

	public Dictionary<string, int> LstCountCode
	{
		[CompilerGenerated]
		get
		{
			return cv9kheKrEZ;
		}
		[CompilerGenerated]
		set
		{
			cv9kheKrEZ = value;
		}
	}

	public Dictionary<string, string> LstFilePaths
	{
		get
		{
			if (jcWkTfuNxm == null)
			{
				jcWkTfuNxm = new Dictionary<string, string>();
			}
			return jcWkTfuNxm;
		}
	}

	[SpecialName]
	private Ilogger EcekItxqxX()
	{
		if (m1kklH3VZj == null)
		{
			m1kklH3VZj = AppSetting.Instance.GetService<Ilogger>();
		}
		return m1kklH3VZj;
	}

	public ListFileTable()
	{
		CodeDic = new Dictionary<string, Dictionary<int, LstItem>>();
		LstCountCode = new Dictionary<string, int>();
		EquAndStkLstNames = new string[2]
		{
			"equipment",
			"stackable"
		};
	}

	public int GetLstNumMax(string filePath)
	{
		if (LstCountCode.TryGetValue(filePath, out var value))
		{
			LstCountCode.Remove(filePath);
			LstCountCode.Add(filePath, value + 1);
			return value;
		}
		LstCountCode.Add(filePath, 100000);
		return 100000;
	}

	public IEnumerable<string> GetLstFileList(IEnumerable<string> lstNames)
	{
		List<string> list = new List<string>();
		foreach (string lstName in lstNames)
		{
			if (!CodeDic.TryGetValue(lstName, out Dictionary<int, LstItem> value))
			{
				continue;
			}
			foreach (KeyValuePair<int, LstItem> item in value)
			{
				list.Add(item.Value.FullPath);
			}
		}
		return list;
	}

	public void SaveLstNumMax(string filePath, int value)
	{
		if (LstCountCode.TryGetValue(filePath, out var _))
		{
			LstCountCode.Remove(filePath);
			LstCountCode.Add(filePath, value);
		}
	}

	public IEnumerable<string> ItemCodesToFilePathsAsync(IEnumerable<int> itemCodes, string key)
	{
		_003C_003Ec__DisplayClass19_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass19_0();
		CS_0024_003C_003E8__locals6.OCb8eveinb = new ConcurrentBag<string>();
		if (!CodeDic.ContainsKey(key))
		{
			return CS_0024_003C_003E8__locals6.OCb8eveinb;
		}
		CS_0024_003C_003E8__locals6.qsI8rChwTb = CodeDic[key];
		Parallel.ForEach(itemCodes, delegate(int code)
		{
			if (CS_0024_003C_003E8__locals6.qsI8rChwTb.TryGetValue(code, out LstItem value))
			{
				CS_0024_003C_003E8__locals6.OCb8eveinb.Add(value.FullPath);
			}
		});
		return CS_0024_003C_003E8__locals6.OCb8eveinb;
	}

	public IEnumerable<string> ItemCodesToFilePathsAsync(string content, string key)
	{
		_003C_003Ec__DisplayClass20_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass20_0();
		ConcurrentBag<string> result = new ConcurrentBag<string>();
		if (string.IsNullOrEmpty(content))
		{
			return result;
		}
		string[] separator = new string[3]
		{
			"\t",
			" ",
			"\r\n"
		};
		string[] array = content.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (array == null)
		{
			return result;
		}
		CS_0024_003C_003E8__locals3.W6b8xgGWUQ = new ConcurrentBag<int>();
		Parallel.ForEach(array, delegate(string item)
		{
			if (int.TryParse(item, out var result2))
			{
				CS_0024_003C_003E8__locals3.W6b8xgGWUQ.Add(result2);
			}
		});
		return ItemCodesToFilePathsAsync(CS_0024_003C_003E8__locals3.W6b8xgGWUQ, key);
	}

	public void Clear()
	{
		CodeDic = null;
		LstCountCode = null;
		LstFilePaths.Clear();
	}

	public string? ItemCodeConvertFilePath(string lstName, int itemCode)
	{
		if (CodeDic.TryGetValue(lstName, out Dictionary<int, LstItem> value) && value.TryGetValue(itemCode, out var value2))
		{
			return value2.FullPath;
		}
		return null;
	}

	public string? ItemCodeConvertFilePath(IEnumerable<string> lstNames, int itemCode)
	{
		foreach (string lstName in lstNames)
		{
			if (CodeDic.TryGetValue(lstName, out Dictionary<int, LstItem> value) && value.TryGetValue(itemCode, out var value2))
			{
				return value2.FullPath;
			}
		}
		return null;
	}

	public LstItem? GetLstItem(IEnumerable<string> lstNames, int itemCode)
	{
		foreach (string lstName in lstNames)
		{
			if (CodeDic.TryGetValue(lstName, out Dictionary<int, LstItem> value) && value.TryGetValue(itemCode, out var value2))
			{
				return value2;
			}
		}
		return null;
	}

	public List<string>? ItemCodeConvertFilePath(IEnumerable<int> itemCodes, IEnumerable<string>? lstNames = null)
	{
		_003C_003Ec__DisplayClass25_0 CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass25_0();
		CS_0024_003C_003E8__locals10.bKK8Sa0iu3 = this;
		CS_0024_003C_003E8__locals10.R7o8BEo1wn = lstNames;
		if (CS_0024_003C_003E8__locals10.R7o8BEo1wn == null)
		{
			CS_0024_003C_003E8__locals10.R7o8BEo1wn = EquAndStkLstNames;
		}
		CS_0024_003C_003E8__locals10.IMV84dNHcC = new List<string>();
		itemCodes.ForEach(delegate(int itemCode)
		{
			string text = CS_0024_003C_003E8__locals10.bKK8Sa0iu3.ItemCodeConvertFilePath(CS_0024_003C_003E8__locals10.R7o8BEo1wn, itemCode);
			if (!string.IsNullOrEmpty(text))
			{
				CS_0024_003C_003E8__locals10.IMV84dNHcC.Add(text);
			}
		});
		if (CS_0024_003C_003E8__locals10.IMV84dNHcC.Count <= 0)
		{
			return null;
		}
		return CS_0024_003C_003E8__locals10.IMV84dNHcC;
	}

	public string? ItemCodeConvertFilePath(int itemCode, IEnumerable<string>? lstNames = null)
	{
		if (lstNames == null)
		{
			lstNames = EquAndStkLstNames;
		}
		return ItemCodeConvertFilePath(lstNames, itemCode);
	}

	public PvfFile? ItemCodeConvertPvfFile(PvfPack pvf, int itemCode, IEnumerable<string>? lstNames = null)
	{
		string text = ItemCodeConvertFilePath(itemCode, lstNames);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		pvf.FileList.TryGetValue(text, out PvfFile value);
		return value;
	}
}
