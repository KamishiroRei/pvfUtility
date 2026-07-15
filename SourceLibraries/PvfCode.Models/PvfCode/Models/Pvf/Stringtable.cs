using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Models.Pvf;

public class Stringtable
{
	internal class StringTableEntry
	{
		public int Index;

		public byte[] Bytes;

		public string Text { get; set; }

		public int QuoteCount { get; set; }
	}

	private Ilogger logger;

	private EncodingType encoding;

	private Dictionary<string, StringTableEntry> entriesByText;

	private List<StringTableEntry?> entries;

	private Dictionary<PvfFileType, int> nameLabelByFileType;

	public int NameLabel { get; private set; }

	public bool IsStringTableUpdated { get; private set; }

	public int SetNameIndex { get; set; }

	public HashSet<int> NameLableOrSetNameLable { get; set; }

	private Ilogger GetLogger()
	{
		if (logger == null)
		{
			logger = AppSetting.Instance.GetIlogger();
		}
		return logger;
	}

	public void Clear()
	{
		IsStringTableUpdated = false;
		if (entriesByText != null)
		{
			entriesByText.Clear();
		}
		entriesByText = null;
		entries = null;
		if (entries != null)
		{
			entries.Clear();
		}
		entriesByText = new Dictionary<string, StringTableEntry>();
		entries = new List<StringTableEntry>();
	}

	public void Loadstringtable(byte[] stBytes, EncodingType encoding, PvfPack pvf)
	{
		NameLableOrSetNameLable = new HashSet<int>();
		this.encoding = encoding;
		entries = new List<StringTableEntry>();
		entriesByText = new Dictionary<string, StringTableEntry>();
		if (stBytes == null)
		{
			AppSetting.Instance.GetIlogger()?.Error("stringtable.bin文件 是Null");
			return;
		}
		int num = BitConverter.ToInt32(stBytes, 0);
		int num2 = stBytes.Length;
		for (int i = 0; i < num; i++)
		{
			int num3 = BitConverter.ToInt32(stBytes, i * 4 + 4);
			int num4 = BitConverter.ToInt32(stBytes, i * 4 + 8) - num3;
			StringTableEntry entry = new StringTableEntry
			{
				Index = i,
				Bytes = new byte[num4]
			};
			if (num4 > num2)
			{
				GetLogger().ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringTableBinError"), isError: true);
				return;
			}
			Buffer.BlockCopy(stBytes, num3 + 4, entry.Bytes, 0, num4);
			entry.Text = AppSetting.Instance.PvfConfig.StrTableAndStrViewConvertStrContent(Encoding.GetEncoding((int)this.encoding).GetString(entry.Bytes).TrimEnd(new char[1]));
			lock (this)
			{
				entries.Add(entry);
			}
			if (!entriesByText.ContainsKey(entry.Text))
			{
				entriesByText.TryAdd(entry.Text, entry);
			}
		}
		IsStringTableUpdated = false;
		InitializeNameLabels();
		GetLogger()?.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringTableLoadSuccess"), this.encoding));
	}

	private void InitializeNameLabels()
	{
		nameLabelByFileType = new Dictionary<PvfFileType, int>();
		NameLabel = GetStringTableId("[name]");
		nameLabelByFileType.Add(PvfFileType.chr, GetStringTableId("[growtype name]"));
		nameLabelByFileType.Add(PvfFileType.emo, GetStringTableId("[macro]"));
		nameLabelByFileType.Add(PvfFileType.aic, GetStringTableId("[minimum info]"));
		nameLabelByFileType.Add(PvfFileType.evt, GetStringTableId("[title]"));
		nameLabelByFileType.Add(PvfFileType.map, GetStringTableId("[map name]"));
		nameLabelByFileType.Add(PvfFileType.msn, GetStringTableId("[name_text]"));
		NameLableOrSetNameLable.Add(NameLabel);
		NameLableOrSetNameLable.Add(GetStringTableId("[set name]"));
	}

	public void InitDefault()
	{
		entriesByText = new Dictionary<string, StringTableEntry>();
		entries = new List<StringTableEntry>();
	}

	public int GetNameLable(PvfFileType fileType)
	{
		if (nameLabelByFileType.TryGetValue(fileType, out var value))
		{
			return value;
		}
		return NameLabel;
	}

	public bool IsAny()
	{
		if (nameLabelByFileType != null && entriesByText != null)
		{
			return entries != null;
		}
		return false;
	}

	public string GetStringItem(int tableId, bool autoConvertStr = false)
	{
		if (tableId < 0 || tableId >= entries.Count)
		{
			return string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_StringError"), tableId);
		}
		if (entries[tableId] != null)
		{
			if (!autoConvertStr)
			{
				return entries[tableId].Text;
			}
			return AppSetting.Instance.PvfConfig.FileTextTraditionalConvertSimplifiedAutoMethods(entries[tableId].Text);
		}
		return string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_LocalStringTableDeleted"), tableId);
	}

	public int GetStringTableId(string str)
	{
		if (!entriesByText.TryGetValue(AppSetting.Instance.PvfConfig.StrTableAndStrViewConvertStrContent(str), out StringTableEntry value))
		{
			return -1;
		}
		return value.Index;
	}

	public IEnumerable<string> SearchPanelGetKeywords(string keyword)
	{
		if (entries == null || entries.Count == 0)
		{
			return null;
		}
		return entriesByText.Keys.Where(text => Regex.IsMatch(text, Regex.Escape(keyword), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)).Take(AppSetting.Instance.PublicSearchServiceOptions.TakeNumber);
	}

	public int AddStringItem(string str)
	{
		string text = AppSetting.Instance.PvfConfig.StrTableAndStrViewConvertStrContent(str);
		StringTableEntry entry = new StringTableEntry
		{
			Index = entries.Count,
			Text = text,
			Bytes = Encoding.GetEncoding((int)encoding).GetBytes(text)
		};
		lock (this)
		{
			entries.Add(entry);
			if (entriesByText.ContainsKey(entry.Text))
			{
				entriesByText.Remove(entry.Text);
			}
			entriesByText.TryAdd(entry.Text, entry);
			IsStringTableUpdated = true;
		}
		return entry.Index;
	}

	public void FindStringItem(HashSet<int> list, string keyword, bool startMatch, bool useLike, Regex regex)
	{
		List<int> list2 = new List<int>();
		if (regex != null)
		{
			list2.AddRange(from item in entries
				where regex.IsMatch(item.Text)
				select item.Index);
		}
		else if (useLike)
		{
			list2.AddRange(from item in entries
				where LikeOperator.LikeString(item?.Text, keyword, CompareMethod.Binary)
				select item.Index);
		}
		else if (startMatch)
		{
			list2.AddRange(from item in entries
				where item != null && item.Text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == 0
				select item.Index);
		}
		else
		{
			list2.AddRange(from item in entries
				where item != null && item.Text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
				select item.Index);
		}
		foreach (int item in list2)
		{
			list.Add(item);
		}
	}

	public void Encode_Stringtable(byte[] buff, int bufflength, uint key)
	{
		uint num = key;
		for (int i = 0; i < bufflength; i += 4)
		{
			Array.Copy(BitConverter.GetBytes(num ^= BitConverter.ToUInt32(buff, i)), 0, buff, i, 4);
		}
	}

	public byte[] CreateStringTable(bool encode = false)
	{
		if (GetStringTableId("此StringTable由 pvfUtility 2022 或更高版本生成.") < 0)
		{
			AddStringItem("此StringTable由 pvfUtility 2022 或更高版本生成.");
		}
		int num = 0;
		int num2 = entries.Count * 4 + 4;
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(BitConverter.GetBytes((uint)entries.Count), 0, 4);
		for (int i = 0; i < entries.Count; i++)
		{
			memoryStream.Write(BitConverter.GetBytes((uint)(num2 + num)), 0, 4);
			if (entries[i] != null)
			{
				num += entries[i].Bytes.Length;
			}
		}
		memoryStream.Write(BitConverter.GetBytes((uint)(num2 + num)), 0, 4);
		for (int j = 0; j < entries.Count; j++)
		{
			if (entries[j] == null)
			{
				memoryStream.Write(new byte[0], 0, 0);
			}
			else
			{
				memoryStream.Write(entries[j].Bytes, 0, entries[j].Bytes.Length);
			}
		}
		if (encode)
		{
			return EncodeStringTable(memoryStream.ToArray(), num + num2 + 4);
		}
		return memoryStream.ToArray();
	}

	private byte[] EncodeStringTable(byte[] buffer, int length)
	{
		Encode_Stringtable(buffer, length, 2478138381u);
		return buffer;
	}

	public Task<string> GetDocumentText()
	{
		if (entries == null)
		{
			return Task.FromResult(string.Empty);
		}
		StringBuilder stringBuilder = new StringBuilder(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileHeader"));
		foreach (StringTableEntry item in entries)
		{
			if (item != null)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(5, 2, stringBuilder2);
				handler.AppendLiteral("[");
				handler.AppendFormatted(item.Index);
				handler.AppendLiteral("]\t`");
				handler.AppendFormatted(item.Text.Replace("\r\n", "\\n"));
				handler.AppendLiteral("`");
				stringBuilder2.AppendLine(ref handler);
			}
		}
		return Task.FromResult(stringBuilder.ToString());
	}

	public Task LoadQuote(PvfPack pvf)
	{
		if (pvf.FileList == null)
		{
			return Task.CompletedTask;
		}
		if (entries != null)
		{
			Parallel.ForEach(entries, item =>
			{
				item.QuoteCount = 0;
			});
		}
		ConcurrentBag<int> stringIds = new ConcurrentBag<int>();
		Parallel.ForEach(pvf.FileList.Where(item => item.Value.IsScriptFile), file =>
		{
			file.Value.GetStringDatas(stringIds);
		});
		foreach (int item in stringIds)
		{
			if (item >= 0 && item < entries.Count && entries[item] != null)
			{
				entries[item].QuoteCount++;
			}
		}
		return Task.CompletedTask;
	}

	public int GetQuoteCount(int index)
	{
		if (index < 0 || index >= entries.Count)
		{
			return 0;
		}
		if (entries[index] == null)
		{
			return 0;
		}
		return entries[index].QuoteCount;
	}

	public async Task DocumentSave(IDictionary<int, string> table, PvfPack pvf)
	{
		List<StringTableEntry> list = new List<StringTableEntry>();
		Dictionary<string, StringTableEntry> dictionary = new Dictionary<string, StringTableEntry>();
		int num = 0;
		foreach (KeyValuePair<int, string> item in table.OrderBy<KeyValuePair<int, string>, int>((KeyValuePair<int, string> it) => it.Key))
		{
			for (; item.Key > num; num++)
			{
				list.Add(null);
			}
			StringTableEntry entry = new StringTableEntry
			{
				Index = list.Count,
				Text = item.Value,
				Bytes = Encoding.GetEncoding((int)encoding).GetBytes(item.Value)
			};
			list.Add(entry);
			if (!dictionary.ContainsKey(item.Value))
			{
				dictionary.Add(item.Value, entry);
			}
			num++;
		}
		entriesByText = dictionary;
		entries = list;
		IsStringTableUpdated = true;
		await LoadQuote(pvf);
	}

	public IEnumerable<string> WebApiGetStringTab()
	{
		if (entries == null)
		{
			return new List<string>();
		}
		return entries.Select(item => item.Text);
	}

	public async Task DeletingInvalidReferences(PvfPack pvf)
	{
		if (entries == null || !entries.Any())
		{
			return;
		}
		await LoadQuote(pvf);
		if (!entries.Any(item => item.QuoteCount == 0))
		{
			return;
		}
		bool flag = false;
		List<StringTableEntry> list = new List<StringTableEntry>();
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = 0;
		foreach (StringTableEntry item in entries)
		{
			if (item != null && item.QuoteCount > 0)
			{
				if (item.Index != num)
				{
					dictionary.Add(item.Index, num);
					item.Index = num;
				}
				list.Add(item);
				num++;
			}
			else if (!flag)
			{
				flag = true;
			}
		}
		if (dictionary.Count > 0)
		{
			foreach (PvfFile item2 in from it in pvf.FileList
				where it.Value.IsScriptFile
				select it.Value)
			{
				item2?.ReconstructNameReferenceData(dictionary);
			}
			Ilogger ilogger = GetLogger();
			if (ilogger != null)
			{
				ilogger.Success($"stringtable.bin文件共裁剪{entries.Count - dictionary.Count}条 未被引用的字符串");
			}
		}
		Dictionary<string, StringTableEntry> dictionary2 = new Dictionary<string, StringTableEntry>();
		if (!flag && dictionary.Count <= 0)
		{
			return;
		}
		foreach (StringTableEntry item3 in list)
		{
			if (!dictionary2.ContainsKey(item3.Text))
			{
				dictionary2.Add(item3.Text, item3);
			}
		}
		entriesByText = dictionary2;
		entries = list;
		IsStringTableUpdated = true;
		InitializeNameLabels();
	}

	public Stringtable()
	{
	}
}
