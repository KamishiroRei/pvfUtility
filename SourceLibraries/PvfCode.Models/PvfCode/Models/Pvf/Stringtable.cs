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
	internal class dEOStg8lJCr53XY3219
	{
		public int bpn86uISfF;

		public byte[] U4W8yQrDQ6;

		[CompilerGenerated]
		private string Ps78wJVNG0;

		[CompilerGenerated]
		private int cHb8oQFvs9;

		[SpecialName]
		[CompilerGenerated]
		public string JEZ8f7BM5L()
		{
			return Ps78wJVNG0;
		}

		[SpecialName]
		[CompilerGenerated]
		public void fvJ8hOn2kH(string P_0)
		{
			Ps78wJVNG0 = P_0;
		}

		[SpecialName]
		[CompilerGenerated]
		public int t2b808KCZm()
		{
			return cHb8oQFvs9;
		}

		[SpecialName]
		[CompilerGenerated]
		public void lJD8s7p2oJ(int P_0)
		{
			cHb8oQFvs9 = P_0;
		}

		public dEOStg8lJCr53XY3219()
		{
		}
	}

	internal class kl82nT82GWkyXNqx1wJ
	{
		[CompilerGenerated]
		private PvfFile QK88qgBxsq;

		[CompilerGenerated]
		private int Kht8z0jM2Z;

		public PvfFile File
		{
			[CompilerGenerated]
			get
			{
				return QK88qgBxsq;
			}
			[CompilerGenerated]
			set
			{
				QK88qgBxsq = value;
			}
		}

		public int Index
		{
			[CompilerGenerated]
			get
			{
				return Kht8z0jM2Z;
			}
			[CompilerGenerated]
			set
			{
				Kht8z0jM2Z = value;
			}
		}

		public kl82nT82GWkyXNqx1wJ()
		{
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		public string QAJueEGxIY;

		public _003C_003Ec__DisplayClass35_0()
		{
		}

		internal bool xFNur4PlMj(string x)
		{
			return Regex.IsMatch(x, Regex.Escape(QAJueEGxIY), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass37_0
	{
		public Regex lcbuB0kYqP;

		public string Hp7u4x0WAG;

		public _003C_003Ec__DisplayClass37_0()
		{
		}

		internal bool WSsujUgd07(dEOStg8lJCr53XY3219 item)
		{
			return lcbuB0kYqP.IsMatch(item.JEZ8f7BM5L());
		}

		internal bool PwSuxQP0uD(dEOStg8lJCr53XY3219 item)
		{
			return LikeOperator.LikeString(item?.JEZ8f7BM5L(), Hp7u4x0WAG, CompareMethod.Binary);
		}

		internal bool tLaumcotBa(dEOStg8lJCr53XY3219 item)
		{
			if (item == null)
			{
				return false;
			}
			return item.JEZ8f7BM5L().IndexOf(Hp7u4x0WAG, StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal bool ymduSu11fJ(dEOStg8lJCr53XY3219 item)
		{
			if (item == null)
			{
				return false;
			}
			return item.JEZ8f7BM5L().IndexOf(Hp7u4x0WAG, StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass42_0
	{
		public ConcurrentBag<int> HCMuv0JAit;

		public _003C_003Ec__DisplayClass42_0()
		{
		}

		internal void TU6uChG36v(KeyValuePair<string, PvfFile> file)
		{
			file.Value.GetStringDatas(HCMuv0JAit);
		}
	}

	private Ilogger qO3L3oRnm4;

	private EncodingType rKoLHRQOAd;

	private Dictionary<string, dEOStg8lJCr53XY3219> XLiL78w9aV;

	private List<dEOStg8lJCr53XY3219?> xqBLcEarNI;

	[CompilerGenerated]
	private Dictionary<PvfFileType, int> NZELgav8gv;

	[CompilerGenerated]
	private int fvnLKYcvks;

	[CompilerGenerated]
	private bool CLiLYxBU1m;

	[CompilerGenerated]
	private int XpmLJdrws3;

	[CompilerGenerated]
	private HashSet<int> joLLdgLt5V;

	public int NameLabel
	{
		[CompilerGenerated]
		get
		{
			return fvnLKYcvks;
		}
		[CompilerGenerated]
		private set
		{
			fvnLKYcvks = value;
		}
	}

	public bool IsStringTableUpdated
	{
		[CompilerGenerated]
		get
		{
			return CLiLYxBU1m;
		}
		[CompilerGenerated]
		private set
		{
			CLiLYxBU1m = value;
		}
	}

	public int SetNameIndex
	{
		[CompilerGenerated]
		get
		{
			return XpmLJdrws3;
		}
		[CompilerGenerated]
		set
		{
			XpmLJdrws3 = value;
		}
	}

	public HashSet<int> NameLableOrSetNameLable
	{
		[CompilerGenerated]
		get
		{
			return joLLdgLt5V;
		}
		[CompilerGenerated]
		set
		{
			joLLdgLt5V = value;
		}
	}

	[SpecialName]
	private Ilogger jGMLEDTr60()
	{
		if (qO3L3oRnm4 == null)
		{
			qO3L3oRnm4 = AppSetting.Instance.GetIlogger();
		}
		return qO3L3oRnm4;
	}

	[SpecialName]
	[CompilerGenerated]
	private Dictionary<PvfFileType, int> dU8L8PBPHr()
	{
		return NZELgav8gv;
	}

	[SpecialName]
	[CompilerGenerated]
	private void ntPLuwJqJx(Dictionary<PvfFileType, int> P_0)
	{
		NZELgav8gv = P_0;
	}

	public void Clear()
	{
		IsStringTableUpdated = false;
		if (XLiL78w9aV != null)
		{
			XLiL78w9aV.Clear();
		}
		XLiL78w9aV = null;
		xqBLcEarNI = null;
		if (xqBLcEarNI != null)
		{
			xqBLcEarNI.Clear();
		}
		XLiL78w9aV = new Dictionary<string, dEOStg8lJCr53XY3219>();
		xqBLcEarNI = new List<dEOStg8lJCr53XY3219>();
	}

	public void Loadstringtable(byte[] stBytes, EncodingType encoding, PvfPack pvf)
	{
		NameLableOrSetNameLable = new HashSet<int>();
		rKoLHRQOAd = encoding;
		xqBLcEarNI = new List<dEOStg8lJCr53XY3219>();
		XLiL78w9aV = new Dictionary<string, dEOStg8lJCr53XY3219>();
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
			dEOStg8lJCr53XY3219 dEOStg8lJCr53XY3220 = new dEOStg8lJCr53XY3219
			{
				bpn86uISfF = i,
				U4W8yQrDQ6 = new byte[num4]
			};
			if (num4 > num2)
			{
				jGMLEDTr60().ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringTableBinError"), isError: true);
				return;
			}
			Buffer.BlockCopy(stBytes, num3 + 4, dEOStg8lJCr53XY3220.U4W8yQrDQ6, 0, num4);
			dEOStg8lJCr53XY3220.fvJ8hOn2kH(AppSetting.Instance.PvfConfig.StrTableAndStrViewConvertStrContent(Encoding.GetEncoding((int)rKoLHRQOAd).GetString(dEOStg8lJCr53XY3220.U4W8yQrDQ6).TrimEnd(new char[1])));
			lock (this)
			{
				xqBLcEarNI.Add(dEOStg8lJCr53XY3220);
			}
			if (!XLiL78w9aV.ContainsKey(dEOStg8lJCr53XY3220.JEZ8f7BM5L()))
			{
				XLiL78w9aV.TryAdd(dEOStg8lJCr53XY3220.JEZ8f7BM5L(), dEOStg8lJCr53XY3220);
			}
		}
		IsStringTableUpdated = false;
		BW8Lk2EMZB();
		jGMLEDTr60()?.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringTableLoadSuccess"), rKoLHRQOAd));
	}

	private void BW8Lk2EMZB()
	{
		ntPLuwJqJx(new Dictionary<PvfFileType, int>());
		NameLabel = GetStringTableId("[name]");
		dU8L8PBPHr().Add(PvfFileType.chr, GetStringTableId("[growtype name]"));
		dU8L8PBPHr().Add(PvfFileType.emo, GetStringTableId("[macro]"));
		dU8L8PBPHr().Add(PvfFileType.aic, GetStringTableId("[minimum info]"));
		dU8L8PBPHr().Add(PvfFileType.evt, GetStringTableId("[title]"));
		dU8L8PBPHr().Add(PvfFileType.map, GetStringTableId("[map name]"));
		dU8L8PBPHr().Add(PvfFileType.msn, GetStringTableId("[name_text]"));
		NameLableOrSetNameLable.Add(NameLabel);
		NameLableOrSetNameLable.Add(GetStringTableId("[set name]"));
	}

	public void InitDefault()
	{
		XLiL78w9aV = new Dictionary<string, dEOStg8lJCr53XY3219>();
		xqBLcEarNI = new List<dEOStg8lJCr53XY3219>();
	}

	public int GetNameLable(PvfFileType fileType)
	{
		if (dU8L8PBPHr().TryGetValue(fileType, out var value))
		{
			return value;
		}
		return NameLabel;
	}

	public bool IsAny()
	{
		if (dU8L8PBPHr() != null && XLiL78w9aV != null)
		{
			return xqBLcEarNI != null;
		}
		return false;
	}

	public string GetStringItem(int tableId, bool autoConvertStr = false)
	{
		if (tableId < 0 || tableId >= xqBLcEarNI.Count)
		{
			return string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_StringError"), tableId);
		}
		if (xqBLcEarNI[tableId] != null)
		{
			if (!autoConvertStr)
			{
				return xqBLcEarNI[tableId].JEZ8f7BM5L();
			}
			return AppSetting.Instance.PvfConfig.FileTextTraditionalConvertSimplifiedAutoMethods(xqBLcEarNI[tableId].JEZ8f7BM5L());
		}
		return string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_LocalStringTableDeleted"), tableId);
	}

	public int GetStringTableId(string str)
	{
		if (!XLiL78w9aV.TryGetValue(AppSetting.Instance.PvfConfig.StrTableAndStrViewConvertStrContent(str), out dEOStg8lJCr53XY3219 value))
		{
			return -1;
		}
		return value.bpn86uISfF;
	}

	public IEnumerable<string> SearchPanelGetKeywords(string keyword)
	{
		_003C_003Ec__DisplayClass35_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass35_0();
		CS_0024_003C_003E8__locals2.QAJueEGxIY = keyword;
		if (xqBLcEarNI == null || xqBLcEarNI.Count == 0)
		{
			return null;
		}
		return XLiL78w9aV.Keys.Where((string x) => Regex.IsMatch(x, Regex.Escape(CS_0024_003C_003E8__locals2.QAJueEGxIY), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)).Take(AppSetting.Instance.PublicSearchServiceOptions.TakeNumber);
	}

	public int AddStringItem(string str)
	{
		string text = AppSetting.Instance.PvfConfig.StrTableAndStrViewConvertStrContent(str);
		dEOStg8lJCr53XY3219 obj = new dEOStg8lJCr53XY3219();
		obj.bpn86uISfF = xqBLcEarNI.Count;
		obj.fvJ8hOn2kH(text);
		obj.U4W8yQrDQ6 = Encoding.GetEncoding((int)rKoLHRQOAd).GetBytes(text);
		dEOStg8lJCr53XY3219 dEOStg8lJCr53XY3220 = obj;
		lock (this)
		{
			xqBLcEarNI.Add(dEOStg8lJCr53XY3220);
			if (XLiL78w9aV.ContainsKey(dEOStg8lJCr53XY3220.JEZ8f7BM5L()))
			{
				XLiL78w9aV.Remove(dEOStg8lJCr53XY3220.JEZ8f7BM5L());
			}
			XLiL78w9aV.TryAdd(dEOStg8lJCr53XY3220.JEZ8f7BM5L(), dEOStg8lJCr53XY3220);
			IsStringTableUpdated = true;
		}
		return dEOStg8lJCr53XY3220.bpn86uISfF;
	}

	public void FindStringItem(HashSet<int> list, string keyword, bool startMatch, bool useLike, Regex regex)
	{
		_003C_003Ec__DisplayClass37_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass37_0();
		CS_0024_003C_003E8__locals7.lcbuB0kYqP = regex;
		CS_0024_003C_003E8__locals7.Hp7u4x0WAG = keyword;
		List<int> list2 = new List<int>();
		if (CS_0024_003C_003E8__locals7.lcbuB0kYqP != null)
		{
			list2.AddRange(from item in xqBLcEarNI
				where CS_0024_003C_003E8__locals7.lcbuB0kYqP.IsMatch(item.JEZ8f7BM5L())
				select item.bpn86uISfF);
		}
		else if (useLike)
		{
			list2.AddRange(from item in xqBLcEarNI
				where LikeOperator.LikeString(item?.JEZ8f7BM5L(), CS_0024_003C_003E8__locals7.Hp7u4x0WAG, CompareMethod.Binary)
				select item.bpn86uISfF);
		}
		else if (startMatch)
		{
			list2.AddRange(from item in xqBLcEarNI
				where item != null && item.JEZ8f7BM5L().IndexOf(CS_0024_003C_003E8__locals7.Hp7u4x0WAG, StringComparison.OrdinalIgnoreCase) == 0
				select item.bpn86uISfF);
		}
		else
		{
			list2.AddRange(from item in xqBLcEarNI
				where item != null && item.JEZ8f7BM5L().IndexOf(CS_0024_003C_003E8__locals7.Hp7u4x0WAG, StringComparison.OrdinalIgnoreCase) >= 0
				select item.bpn86uISfF);
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
		int num2 = xqBLcEarNI.Count * 4 + 4;
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(BitConverter.GetBytes((uint)xqBLcEarNI.Count), 0, 4);
		for (int i = 0; i < xqBLcEarNI.Count; i++)
		{
			memoryStream.Write(BitConverter.GetBytes((uint)(num2 + num)), 0, 4);
			if (xqBLcEarNI[i] != null)
			{
				num += xqBLcEarNI[i].U4W8yQrDQ6.Length;
			}
		}
		memoryStream.Write(BitConverter.GetBytes((uint)(num2 + num)), 0, 4);
		for (int j = 0; j < xqBLcEarNI.Count; j++)
		{
			if (xqBLcEarNI[j] == null)
			{
				memoryStream.Write(new byte[0], 0, 0);
			}
			else
			{
				memoryStream.Write(xqBLcEarNI[j].U4W8yQrDQ6, 0, xqBLcEarNI[j].U4W8yQrDQ6.Length);
			}
		}
		if (encode)
		{
			return yHLLLxR7Qi(memoryStream.ToArray(), num + num2 + 4);
		}
		return memoryStream.ToArray();
	}

	private byte[] yHLLLxR7Qi(byte[] P_0, int P_1)
	{
		Encode_Stringtable(P_0, P_1, 2478138381u);
		return P_0;
	}

	public Task<string> GetDocumentText()
	{
		if (xqBLcEarNI == null)
		{
			return Task.FromResult(string.Empty);
		}
		StringBuilder stringBuilder = new StringBuilder(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileHeader"));
		foreach (dEOStg8lJCr53XY3219 item in xqBLcEarNI)
		{
			if (item != null)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(5, 2, stringBuilder2);
				handler.AppendLiteral("[");
				handler.AppendFormatted(item.bpn86uISfF);
				handler.AppendLiteral("]\t`");
				handler.AppendFormatted(item.JEZ8f7BM5L().Replace("\r\n", "\\n"));
				handler.AppendLiteral("`");
				stringBuilder2.AppendLine(ref handler);
			}
		}
		return Task.FromResult(stringBuilder.ToString());
	}

	public Task LoadQuote(PvfPack pvf)
	{
		_003C_003Ec__DisplayClass42_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass42_0();
		if (pvf.FileList == null)
		{
			return Task.CompletedTask;
		}
		if (xqBLcEarNI != null)
		{
			Parallel.ForEach(xqBLcEarNI, delegate(dEOStg8lJCr53XY3219 item)
			{
				item.lJD8s7p2oJ(0);
			});
		}
		CS_0024_003C_003E8__locals3.HCMuv0JAit = new ConcurrentBag<int>();
		Parallel.ForEach(pvf.FileList.Where<KeyValuePair<string, PvfFile>>((KeyValuePair<string, PvfFile> it) => it.Value.IsScriptFile), delegate(KeyValuePair<string, PvfFile> file)
		{
			file.Value.GetStringDatas(CS_0024_003C_003E8__locals3.HCMuv0JAit);
		});
		foreach (int item in CS_0024_003C_003E8__locals3.HCMuv0JAit)
		{
			if (item >= 0 && item < xqBLcEarNI.Count && xqBLcEarNI[item] != null)
			{
				dEOStg8lJCr53XY3219? obj = xqBLcEarNI[item];
				int num = obj.t2b808KCZm();
				obj.lJD8s7p2oJ(num + 1);
			}
		}
		return Task.CompletedTask;
	}

	public int GetQuoteCount(int index)
	{
		if (index < 0 || index >= xqBLcEarNI.Count)
		{
			return 0;
		}
		if (xqBLcEarNI[index] == null)
		{
			return 0;
		}
		return xqBLcEarNI[index].t2b808KCZm();
	}

	public async Task DocumentSave(IDictionary<int, string> table, PvfPack pvf)
	{
		List<dEOStg8lJCr53XY3219> list = new List<dEOStg8lJCr53XY3219>();
		Dictionary<string, dEOStg8lJCr53XY3219> dictionary = new Dictionary<string, dEOStg8lJCr53XY3219>();
		int num = 0;
		foreach (KeyValuePair<int, string> item in table.OrderBy<KeyValuePair<int, string>, int>((KeyValuePair<int, string> it) => it.Key))
		{
			for (; item.Key > num; num++)
			{
				list.Add(null);
			}
			dEOStg8lJCr53XY3219 obj = new dEOStg8lJCr53XY3219();
			obj.bpn86uISfF = list.Count;
			obj.fvJ8hOn2kH(item.Value);
			obj.U4W8yQrDQ6 = Encoding.GetEncoding((int)rKoLHRQOAd).GetBytes(item.Value);
			dEOStg8lJCr53XY3219 dEOStg8lJCr53XY3220 = obj;
			list.Add(dEOStg8lJCr53XY3220);
			if (!dictionary.ContainsKey(item.Value))
			{
				dictionary.Add(item.Value, dEOStg8lJCr53XY3220);
			}
			num++;
		}
		XLiL78w9aV = dictionary;
		xqBLcEarNI = list;
		IsStringTableUpdated = true;
		await LoadQuote(pvf);
	}

	public IEnumerable<string> WebApiGetStringTab()
	{
		if (xqBLcEarNI == null)
		{
			return new List<string>();
		}
		return xqBLcEarNI.Select((dEOStg8lJCr53XY3219 it) => it.JEZ8f7BM5L());
	}

	public async Task DeletingInvalidReferences(PvfPack pvf)
	{
		if (xqBLcEarNI == null || !xqBLcEarNI.Any())
		{
			return;
		}
		await LoadQuote(pvf);
		if (!xqBLcEarNI.Any((dEOStg8lJCr53XY3219 it) => it.t2b808KCZm() == 0))
		{
			return;
		}
		bool flag = false;
		List<dEOStg8lJCr53XY3219> list = new List<dEOStg8lJCr53XY3219>();
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = 0;
		foreach (dEOStg8lJCr53XY3219 item in xqBLcEarNI)
		{
			if (item != null && item.t2b808KCZm() > 0)
			{
				if (item.bpn86uISfF != num)
				{
					dictionary.Add(item.bpn86uISfF, num);
					item.bpn86uISfF = num;
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
			Ilogger ilogger = jGMLEDTr60();
			if (ilogger != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("stringtable.bin文件共裁剪");
				defaultInterpolatedStringHandler.AppendFormatted(xqBLcEarNI.Count - dictionary.Count);
				defaultInterpolatedStringHandler.AppendLiteral("条 未被引用的字符串");
				ilogger.Success(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
		Dictionary<string, dEOStg8lJCr53XY3219> dictionary2 = new Dictionary<string, dEOStg8lJCr53XY3219>();
		if (!flag && dictionary.Count <= 0)
		{
			return;
		}
		foreach (dEOStg8lJCr53XY3219 item3 in list)
		{
			if (!dictionary2.ContainsKey(item3.JEZ8f7BM5L()))
			{
				dictionary2.Add(item3.JEZ8f7BM5L(), item3);
			}
		}
		XLiL78w9aV = dictionary2;
		xqBLcEarNI = list;
		IsStringTableUpdated = true;
		BW8Lk2EMZB();
	}

	public Stringtable()
	{
	}
}
