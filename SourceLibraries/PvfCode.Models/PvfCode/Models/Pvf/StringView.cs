using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

		[CompilerGenerated]
		private Dictionary<string, StrListFileData> kOdubdgHms;

		public Dictionary<string, StrListFileData> StringLst
		{
			[CompilerGenerated]
			get
			{
				return kOdubdgHms;
			}
			[CompilerGenerated]
			set
			{
				kOdubdgHms = value;
			}
		}

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
		[CompilerGenerated]
		private string IAquVoDSTF;

		[CompilerGenerated]
		private int udeuPPjcaR;

		public string Data
		{
			[CompilerGenerated]
			get
			{
				return IAquVoDSTF;
			}
			[CompilerGenerated]
			set
			{
				IAquVoDSTF = value;
			}
		}

		public int Quote
		{
			[CompilerGenerated]
			get
			{
				return udeuPPjcaR;
			}
			[CompilerGenerated]
			set
			{
				udeuPPjcaR = value;
			}
		}

		public StrListFileData(string data)
		{
			Data = data;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_0
	{
		public Regex WKKu9o1Xl4;

		public string hLQuRAOJD7;

		public Func<KeyValuePair<string, StrListFileData>, bool> J3QuWj4ak5;

		public Func<KeyValuePair<string, StrListFileData>, bool> ECWuqHSBMZ;

		public Func<KeyValuePair<string, StrListFileData>, bool> SxFuzAtwXq;

		public Func<KeyValuePair<string, StrListFileData>, bool> b7a5ARiLXN;

		public _003C_003Ec__DisplayClass11_0()
		{
		}

		internal bool MFtuwdnAwE(KeyValuePair<string, StrListFileData> item)
		{
			return WKKu9o1Xl4.IsMatch(hLQuRAOJD7);
		}

		internal bool XcjuocA15f(KeyValuePair<string, StrListFileData> item)
		{
			KeyValuePair<string, StrListFileData> keyValuePair = item;
			return LikeOperator.LikeString(keyValuePair.Value.Data, hLQuRAOJD7, CompareMethod.Binary);
		}

		internal bool irru2AWRhX(KeyValuePair<string, StrListFileData> item)
		{
			KeyValuePair<string, StrListFileData> keyValuePair = item;
			return keyValuePair.Value.Data.IndexOf(hLQuRAOJD7, StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal bool cY9utKJe7V(KeyValuePair<string, StrListFileData> item)
		{
			KeyValuePair<string, StrListFileData> keyValuePair = item;
			return keyValuePair.Value.Data.IndexOf(hLQuRAOJD7, StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass16_0
	{
		public string qcS5kSTSY3;

		public _003C_003Ec__DisplayClass16_0()
		{
		}

		internal bool mmY5nVk38F(StrListFile it)
		{
			return it.StrFileName.ToLower() == qcS5kSTSY3;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_0
	{
		public ConcurrentBag<KeyValuePair<int, int>> E6Z5E7afhM;

		public PvfPack V6k5Zy1Hgk;

		public _003C_003Ec__DisplayClass18_0()
		{
		}

		internal void oSd5LVwLlV(KeyValuePair<string, PvfFile> file)
		{
			file.Value.GetStringViewQuote(E6Z5E7afhM, V6k5Zy1Hgk.Strtable);
		}
	}

	private StrListFile[] twdLjWing9;

	[SpecialName]
	private Ilogger AFXLrVMT12()
	{
		return AppSetting.Instance.GetService<Ilogger>();
	}

	public StrListFile[] Get_pvfstrlist()
	{
		return twdLjWing9;
	}

	public void Clear()
	{
		twdLjWing9 = null;
	}

	public void InitDefault()
	{
		twdLjWing9 = new StrListFile[0];
	}

	public void InitStringData(PvfFile file, PvfPack pvf, EncodingType type)
	{
		if (!file.IsScriptFile)
		{
			return;
		}
		int dataLen = file.DataLen;
		twdLjWing9 = new StrListFile[(dataLen - 2) / 10];
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
				AFXLrVMT12().Warning(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotFound"), item));
			}
			wa6L1IZjSX(text, item.Value, item.Key, pvf);
		}
		AFXLrVMT12()?.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkLoadSuccess"), (dataLen - 2) / 10));
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		pooledDictionary.Dispose();
		stopwatch.Stop();
		_ = stopwatch.Elapsed;
	}

	private void wa6L1IZjSX(string P_0, int P_1, string P_2, PvfPack P_3)
	{
		try
		{
			twdLjWing9[P_1] = new StrListFile(P_2);
			char[] separator = new char[2] { '\r', '\n' };
			Dictionary<string, StrListFileData> dictionary = new Dictionary<string, StrListFileData>();
			string[] array = P_0.Split(separator, StringSplitOptions.RemoveEmptyEntries);
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
			twdLjWing9[P_1].StringLst = dictionary;
		}
		catch (Exception ex)
		{
			AFXLrVMT12().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkLoadError"), P_2, ex.Message, P_1));
		}
	}

	public void ReloadstrFile(string fileName, string fileData, PvfPack pvf)
	{
		StrListFile[] array = twdLjWing9;
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
							AFXLrVMT12().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkLoadError2"), fileName, dataFromFormat));
						}
					}
				}
				strListFile.StringLst = dictionary;
				InitStringViewQuote(pvf, dictionary, clearQuote: false, Array.IndexOf(twdLjWing9, strListFile));
			}
			catch (Exception ex)
			{
				AFXLrVMT12().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkLoadError3"), fileName, ex.Message));
			}
			AFXLrVMT12().Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkReloadSuccess"), fileName));
			break;
		}
	}

	public string GetStrText(int strid, string strname, bool autoConvertStr = false)
	{
		try
		{
			if (strid < 0 || strid >= twdLjWing9.Length)
			{
				return string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_StrIndexError"), strid);
			}
			StrListFile strListFile = twdLjWing9[strid];
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
			AFXLrVMT12().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkReadError"), strid, strname, ex.Message));
			return "";
		}
	}

	public int GetQuoteCount(int strId, string strName)
	{
		if (strId < twdLjWing9.Length)
		{
			int result = 0;
			if (twdLjWing9[strId].StringLst.TryGetValue(strName, out StrListFileData value))
			{
				result = value.Quote;
			}
			return result;
		}
		return 0;
	}

	public void SearchstrInFiles(HashSet<int> nums, Stringtable stringtable, string keyWord, bool startMatch, bool useLike, Regex regex)
	{
		_003C_003Ec__DisplayClass11_0 CS_0024_003C_003E8__locals8 = new _003C_003Ec__DisplayClass11_0();
		CS_0024_003C_003E8__locals8.WKKu9o1Xl4 = regex;
		CS_0024_003C_003E8__locals8.hLQuRAOJD7 = keyWord;
		for (int i = 0; i < twdLjWing9.Length; i++)
		{
			List<string> list = new List<string>();
			if (twdLjWing9[i] == null)
			{
				continue;
			}
			if (CS_0024_003C_003E8__locals8.WKKu9o1Xl4 != null)
			{
				list.AddRange(twdLjWing9[i].StringLst.Where<KeyValuePair<string, StrListFileData>>((KeyValuePair<string, StrListFileData> item) => CS_0024_003C_003E8__locals8.WKKu9o1Xl4.IsMatch(CS_0024_003C_003E8__locals8.hLQuRAOJD7)).Select(delegate(KeyValuePair<string, StrListFileData> item)
				{
					KeyValuePair<string, StrListFileData> keyValuePair = item;
					return keyValuePair.Key;
				}));
			}
			else if (useLike)
			{
				list.AddRange(twdLjWing9[i].StringLst.Where<KeyValuePair<string, StrListFileData>>(delegate(KeyValuePair<string, StrListFileData> item)
				{
					KeyValuePair<string, StrListFileData> keyValuePair = item;
					return LikeOperator.LikeString(keyValuePair.Value.Data, CS_0024_003C_003E8__locals8.hLQuRAOJD7, CompareMethod.Binary);
				}).Select(delegate(KeyValuePair<string, StrListFileData> item)
				{
					KeyValuePair<string, StrListFileData> keyValuePair = item;
					return keyValuePair.Key;
				}));
			}
			else if (startMatch)
			{
				list.AddRange(twdLjWing9[i].StringLst.Where<KeyValuePair<string, StrListFileData>>(delegate(KeyValuePair<string, StrListFileData> item)
				{
					KeyValuePair<string, StrListFileData> keyValuePair = item;
					return keyValuePair.Value.Data.IndexOf(CS_0024_003C_003E8__locals8.hLQuRAOJD7, StringComparison.OrdinalIgnoreCase) == 0;
				}).Select(delegate(KeyValuePair<string, StrListFileData> item)
				{
					KeyValuePair<string, StrListFileData> keyValuePair = item;
					return keyValuePair.Key;
				}));
			}
			else
			{
				list.AddRange(twdLjWing9[i].StringLst.Where<KeyValuePair<string, StrListFileData>>(delegate(KeyValuePair<string, StrListFileData> item)
				{
					KeyValuePair<string, StrListFileData> keyValuePair = item;
					return keyValuePair.Value.Data.IndexOf(CS_0024_003C_003E8__locals8.hLQuRAOJD7, StringComparison.OrdinalIgnoreCase) >= 0;
				}).Select(delegate(KeyValuePair<string, StrListFileData> item)
				{
					KeyValuePair<string, StrListFileData> keyValuePair = item;
					return keyValuePair.Key;
				}));
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
		if (twdLjWing9 == null || !twdLjWing9.Any())
		{
			return;
		}
		if (clearQuote)
		{
			StrListFile[] array = twdLjWing9;
			foreach (StrListFile strListFile in array)
			{
				if (strListFile != null && strListFile.StringLst != null)
				{
					h6dLGHIfyD(strListFile.StringLst);
				}
			}
		}
		BnsLOXQ8p2(pvf);
	}

	public void InitStringViewQuote(PvfPack pvf, Dictionary<string, StrListFileData> dic, bool clearQuote, int viewId)
	{
		if (dic == null || dic.Count == 0)
		{
			return;
		}
		if (clearQuote)
		{
			h6dLGHIfyD(dic);
		}
		ConcurrentBag<KeyValuePair<int, int>> concurrentBag = BnsLOXQ8p2(pvf);
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
			if (item2.Key >= 0 && item2.Key < twdLjWing9.Length && item2.Key == viewId && dictionary.TryGetValue(item2.Value, out var value))
			{
				value.Quote++;
			}
		}
	}

	public Task InitStringViewQuote(PvfPack pvf, string strFilePath)
	{
		_003C_003Ec__DisplayClass16_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass16_0();
		CS_0024_003C_003E8__locals2.qcS5kSTSY3 = strFilePath;
		if (twdLjWing9 == null || !twdLjWing9.Any())
		{
			return Task.CompletedTask;
		}
		StrListFile strListFile = twdLjWing9.FirstOrDefault((StrListFile it) => it.StrFileName.ToLower() == CS_0024_003C_003E8__locals2.qcS5kSTSY3);
		InitStringViewQuote(pvf, strListFile?.StringLst, clearQuote: true, Array.IndexOf(twdLjWing9, strListFile));
		return Task.CompletedTask;
	}

	private void h6dLGHIfyD(Dictionary<string, StrListFileData> P_0)
	{
		foreach (KeyValuePair<string, StrListFileData> item in P_0)
		{
			item.Value.Quote = 0;
		}
	}

	private ConcurrentBag<KeyValuePair<int, int>> BnsLOXQ8p2(PvfPack P_0)
	{
		_003C_003Ec__DisplayClass18_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass18_0();
		CS_0024_003C_003E8__locals6.V6k5Zy1Hgk = P_0;
		CS_0024_003C_003E8__locals6.E6Z5E7afhM = new ConcurrentBag<KeyValuePair<int, int>>();
		Parallel.ForEach(CS_0024_003C_003E8__locals6.V6k5Zy1Hgk.FileList.Where<KeyValuePair<string, PvfFile>>((KeyValuePair<string, PvfFile> it) => it.Value.IsScriptFile), delegate(KeyValuePair<string, PvfFile> file)
		{
			file.Value.GetStringViewQuote(CS_0024_003C_003E8__locals6.E6Z5E7afhM, CS_0024_003C_003E8__locals6.V6k5Zy1Hgk.Strtable);
		});
		return CS_0024_003C_003E8__locals6.E6Z5E7afhM;
	}

	public Task TrimmableStringView(PvfPack pvf)
	{
		InitStringViewQuote(pvf, clearQuote: true);
		if (twdLjWing9 == null || !twdLjWing9.Any())
		{
			return Task.CompletedTask;
		}
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		StrListFile[] array = twdLjWing9;
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
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
					defaultInterpolatedStringHandler.AppendLiteral("文件：file://");
					defaultInterpolatedStringHandler.AppendFormatted(strListFile.StrFileName);
					defaultInterpolatedStringHandler.AppendLiteral(" 裁剪未使用的字符串共：");
					defaultInterpolatedStringHandler.AppendFormatted(count - strListFile.StringLst.Count);
					defaultInterpolatedStringHandler.AppendLiteral("条数据");
					ilogger.Success(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
		}
		return Task.CompletedTask;
	}

	public StringView()
	{
	}
}
