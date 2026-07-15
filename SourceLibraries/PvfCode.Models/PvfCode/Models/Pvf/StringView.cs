using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Collections.Pooled;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Models.Pvf;

public class StringView
{
	public class StrListFile
	{
		public readonly string StrFileName;

		public Dictionary<string, StrListFileData> StringLst { get; set; }

		public StrListFile(string strFileName)
		{
			StrFileName = strFileName;
			StringLst = new Dictionary<string, StrListFileData>();
		}

		public string ToText()
		{
			if (StringLst == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, StrListFileData> item in StringLst)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
				handler.AppendFormatted(item.Key);
				handler.AppendLiteral(">");
				handler.AppendFormatted(item.Value.Data);
				stringBuilder2.AppendLine(ref handler);
			}
			return stringBuilder.ToString();
		}
	}

	public class StrListFileData
	{
		public string Data { get; set; }

		public int Quote { get; set; }

		public StrListFileData(string data)
		{
			Data = data;
		}
	}

	private StrListFile[] stringListFiles;

	private Ilogger GetLogger()
	{
		return AppSetting.Instance.GetService<Ilogger>();
	}

	public StrListFile[] Get_pvfstrlist()
	{
		return stringListFiles;
	}

	public void Clear()
	{
		stringListFiles = null;
	}

	public void InitDefault()
	{
		stringListFiles = new StrListFile[0];
	}

	public void InitStringData(PvfFile file, PvfPack pvf, EncodingType type)
	{
		if (!file.IsScriptFile)
		{
			return;
		}
		int dataLen = file.DataLen;
		stringListFiles = new StrListFile[(dataLen - 2) / 10];
		PooledDictionary<string, int> pooledDictionary = new PooledDictionary<string, int>();
		int num = 0;
		for (int i = 2; i < dataLen; i += 10)
		{
			string stringItem = pvf.Strtable.GetStringItem(BitConverter.ToInt32(file.Data, i + 1 + 5));
			if (pooledDictionary.ContainsKey(stringItem))
			{
				AppSetting.Instance.GetIlogger()?.Error("n_string.lst 文件中存在重复的kor.str文件名:" + stringItem);
			}
			else
			{
				pooledDictionary.Add(stringItem, num);
			}
			num++;
		}
		foreach (KeyValuePair<string, int> item in pooledDictionary)
		{
			string text = string.Empty;
			if (pvf.FileList.TryGetValue(item.Key.ToLower(), out PvfFile value))
			{
				if (value.Data != null)
				{
					text = Encoding.GetEncoding((int)type).GetString(value.Data).TrimEnd(new char[1]);
				}
			}
			else
			{
				GetLogger().Warning(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotFound"), item));
			}
			LoadStringListFile(text, item.Value, item.Key, pvf);
		}
		GetLogger()?.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkLoadSuccess"), (dataLen - 2) / 10));
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		pooledDictionary.Dispose();
		stopwatch.Stop();
		_ = stopwatch.Elapsed;
	}

	private void LoadStringListFile(string content, int index, string fileName, PvfPack pvf)
	{
		try
		{
			stringListFiles[index] = new StrListFile(fileName);
			char[] separator = new char[2] { '\r', '\n' };
			Dictionary<string, StrListFileData> dictionary = new Dictionary<string, StrListFileData>();
			string[] array = content.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				if (text.IndexOf('>') > 0 && (text.Length <= 2 || text[0] != '/' || text[1] != '/'))
				{
					string dataFromFormat = DataHelper.GetDataFromFormat(text, "", ">");
					string dataFromFormat2 = DataHelper.GetDataFromFormat(text, ">", "");
					if (dataFromFormat.Length > 0)
					{
						dictionary.Add(dataFromFormat, new StrListFileData(AppSetting.Instance.PvfConfig.StrTableAndStrViewConvertStrContent(dataFromFormat2)));
					}
				}
			}
			stringListFiles[index].StringLst = dictionary;
		}
		catch (Exception ex)
		{
			GetLogger().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkLoadError"), fileName, ex.Message, index));
		}
	}

	public void ReloadstrFile(string fileName, string fileData, PvfPack pvf)
	{
		StrListFile[] array = stringListFiles;
		foreach (StrListFile strListFile in array)
		{
			if (strListFile == null || strListFile.StrFileName.Replace('\\', '/').ToLower() != fileName)
			{
				continue;
			}
			strListFile.StringLst.Clear();
			try
			{
				char[] separator = new char[2] { '\r', '\n' };
				Dictionary<string, StrListFileData> dictionary = new Dictionary<string, StrListFileData>();
				string[] array2 = fileData.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array2)
				{
					if (text.IndexOf('>') <= 0 || (text.Length > 2 && text[0] == '/' && text[1] == '/'))
					{
						continue;
					}
					string dataFromFormat = DataHelper.GetDataFromFormat(text, "", ">");
					string dataFromFormat2 = DataHelper.GetDataFromFormat(text, ">", "");
					if (dataFromFormat.Length > 0)
					{
						if (!dictionary.ContainsKey(dataFromFormat))
						{
							dictionary.Add(dataFromFormat, new StrListFileData(AppSetting.Instance.PvfConfig.StrTableAndStrViewConvertStrContent(dataFromFormat2)));
						}
						else
						{
							GetLogger().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkLoadError2"), fileName, dataFromFormat));
						}
					}
				}
				strListFile.StringLst = dictionary;
				InitStringViewQuote(pvf, dictionary, clearQuote: false, Array.IndexOf(stringListFiles, strListFile));
			}
			catch (Exception ex)
			{
				GetLogger().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkLoadError3"), fileName, ex.Message));
			}
			GetLogger().Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkReloadSuccess"), fileName));
			break;
		}
	}

	public string GetStrText(int strid, string strname, bool autoConvertStr = false)
	{
		try
		{
			if (strid < 0 || strid >= stringListFiles.Length)
			{
				return string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_StrIndexError"), strid);
			}
			StrListFile strListFile = stringListFiles[strid];
			if (strListFile == null)
			{
				return string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_StringLinkFileNotExist"), strid, strname);
			}
			if (strListFile.StringLst.TryGetValue(strname, out StrListFileData value))
			{
				return autoConvertStr ? AppSetting.Instance.PvfConfig.FileTextTraditionalConvertSimplifiedAutoMethods(value.Data) : value.Data;
			}
			return string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_CouldnotFindStr"), strname);
		}
		catch (Exception ex)
		{
			GetLogger().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkReadError"), strid, strname, ex.Message));
			return "";
		}
	}

	public int GetQuoteCount(int strId, string strName)
	{
		if (strId < stringListFiles.Length)
		{
			int result = 0;
			if (stringListFiles[strId].StringLst.TryGetValue(strName, out StrListFileData value))
			{
				result = value.Quote;
			}
			return result;
		}
		return 0;
	}

	public void SearchstrInFiles(HashSet<int> nums, Stringtable stringtable, string keyWord, bool startMatch, bool useLike, Regex regex)
	{
		for (int i = 0; i < stringListFiles.Length; i++)
		{
			List<string> list = new List<string>();
			if (stringListFiles[i] == null)
			{
				continue;
			}
			if (regex != null)
			{
				list.AddRange(stringListFiles[i].StringLst.Where(item => regex.IsMatch(item.Value.Data)).Select(item => item.Key));
			}
			else if (useLike)
			{
				list.AddRange(stringListFiles[i].StringLst.Where(item => LikeOperator.LikeString(item.Value.Data, keyWord, CompareMethod.Binary)).Select(item => item.Key));
			}
			else if (startMatch)
			{
				list.AddRange(stringListFiles[i].StringLst.Where(item => item.Value.Data.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) == 0).Select(item => item.Key));
			}
			else
			{
				list.AddRange(stringListFiles[i].StringLst.Where(item => item.Value.Data.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0).Select(item => item.Key));
			}
			foreach (int item in from num in list.Select(stringtable.GetStringTableId)
				where num != -1
				select num)
			{
				nums.Add(item + i * 16777216);
			}
		}
	}

	public void InitStringViewQuote(PvfPack pvf, bool clearQuote)
	{
		if (stringListFiles == null || !stringListFiles.Any())
		{
			return;
		}
		if (clearQuote)
		{
			StrListFile[] array = stringListFiles;
			foreach (StrListFile strListFile in array)
			{
				if (strListFile != null && strListFile.StringLst != null)
				{
					ClearQuoteCounts(strListFile.StringLst);
				}
			}
		}
		CollectStringViewReferences(pvf);
	}

	public void InitStringViewQuote(PvfPack pvf, Dictionary<string, StrListFileData> dic, bool clearQuote, int viewId)
	{
		if (dic == null || dic.Count == 0)
		{
			return;
		}
		if (clearQuote)
		{
			ClearQuoteCounts(dic);
		}
		ConcurrentBag<KeyValuePair<int, int>> concurrentBag = CollectStringViewReferences(pvf);
		Dictionary<int, StrListFileData> dictionary = new Dictionary<int, StrListFileData>();
		foreach (KeyValuePair<string, StrListFileData> item in dic)
		{
			int stringTableId = pvf.Strtable.GetStringTableId(item.Key);
			if (stringTableId != -1 && !dictionary.ContainsKey(stringTableId))
			{
				dictionary.Add(stringTableId, item.Value);
			}
		}
		foreach (KeyValuePair<int, int> item2 in concurrentBag)
		{
			if (item2.Key >= 0 && item2.Key < stringListFiles.Length && item2.Key == viewId && dictionary.TryGetValue(item2.Value, out var value))
			{
				value.Quote++;
			}
		}
	}

	public Task InitStringViewQuote(PvfPack pvf, string strFilePath)
	{
		if (stringListFiles == null || !stringListFiles.Any())
		{
			return Task.CompletedTask;
		}
		StrListFile strListFile = stringListFiles.FirstOrDefault(item => item.StrFileName.ToLower() == strFilePath);
		InitStringViewQuote(pvf, strListFile?.StringLst, clearQuote: true, Array.IndexOf(stringListFiles, strListFile));
		return Task.CompletedTask;
	}

	private static void ClearQuoteCounts(Dictionary<string, StrListFileData> stringEntries)
	{
		foreach (KeyValuePair<string, StrListFileData> item in stringEntries)
		{
			item.Value.Quote = 0;
		}
	}

	private static ConcurrentBag<KeyValuePair<int, int>> CollectStringViewReferences(PvfPack pvf)
	{
		ConcurrentBag<KeyValuePair<int, int>> references = new ConcurrentBag<KeyValuePair<int, int>>();
		Parallel.ForEach(pvf.FileList.Where(item => item.Value.IsScriptFile), file =>
		{
			file.Value.GetStringViewQuote(references, pvf.Strtable);
		});
		return references;
	}

	public Task TrimmableStringView(PvfPack pvf)
	{
		InitStringViewQuote(pvf, clearQuote: true);
		if (stringListFiles == null || !stringListFiles.Any())
		{
			return Task.CompletedTask;
		}
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		StrListFile[] array = stringListFiles;
		foreach (StrListFile strListFile in array)
		{
			if (strListFile != null && strListFile.StringLst != null && strListFile.StringLst.Any<KeyValuePair<string, StrListFileData>>((KeyValuePair<string, StrListFileData> it) => it.Value.Quote == 0))
			{
				int count = strListFile.StringLst.Count;
				KeyValuePair<string, StrListFileData>[] array2 = strListFile.StringLst.Where<KeyValuePair<string, StrListFileData>>((KeyValuePair<string, StrListFileData> it) => it.Value.Quote == 0).ToArray();
				foreach (KeyValuePair<string, StrListFileData> keyValuePair in array2)
				{
					strListFile.StringLst[keyValuePair.Key].Data = string.Empty;
				}
				pvf.SaveFileText(strListFile.StrFileName.ToLower(), strListFile.ToText());
				if (ilogger != null)
				{
					ilogger.Success($"文件：file://{strListFile.StrFileName} 裁剪未使用的字符串共：{count - strListFile.StringLst.Count}条数据");
				}
			}
		}
		return Task.CompletedTask;
	}

	public StringView()
	{
	}
}
