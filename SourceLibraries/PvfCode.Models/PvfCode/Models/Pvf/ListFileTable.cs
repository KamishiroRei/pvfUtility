using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.Mvvm.Native;

namespace PvfCode.Models.Pvf;

public class ListFileTable
{
	public string[] EquAndStkLstNames;

	private Dictionary<string, string> lstFilePaths;

	public Dictionary<string, Dictionary<int, LstItem>> CodeDic { get; set; }

	public Dictionary<string, int> LstCountCode { get; set; }

	public Dictionary<string, string> LstFilePaths
	{
		get
		{
			if (lstFilePaths == null)
			{
				lstFilePaths = new Dictionary<string, string>();
			}
			return lstFilePaths;
		}
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
		ConcurrentBag<string> filePaths = new ConcurrentBag<string>();
		if (!CodeDic.ContainsKey(key))
		{
			return filePaths;
		}
		Dictionary<int, LstItem> itemsByCode = CodeDic[key];
		Parallel.ForEach(itemCodes, code =>
		{
			if (itemsByCode.TryGetValue(code, out LstItem value))
			{
				filePaths.Add(value.FullPath);
			}
		});
		return filePaths;
	}

	public IEnumerable<string> ItemCodesToFilePathsAsync(string content, string key)
	{
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
		ConcurrentBag<int> itemCodes = new ConcurrentBag<int>();
		Parallel.ForEach(array, item =>
		{
			if (int.TryParse(item, out int itemCode))
			{
				itemCodes.Add(itemCode);
			}
		});
		return ItemCodesToFilePathsAsync(itemCodes, key);
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
		if (lstNames == null)
		{
			lstNames = EquAndStkLstNames;
		}
		List<string> filePaths = new List<string>();
		itemCodes.ForEach(itemCode =>
		{
			string text = ItemCodeConvertFilePath(lstNames, itemCode);
			if (!string.IsNullOrEmpty(text))
			{
				filePaths.Add(text);
			}
		});
		if (filePaths.Count <= 0)
		{
			return null;
		}
		return filePaths;
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
