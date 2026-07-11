using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PvfCode.Models.Pvf;
using Utools;

namespace PvfCode.Services;

public static class ServiceItemCodeTable
{
	public static void Init(this PvfGroup pvf)
	{
		pvf.ListFileTable.CodeDic = new Dictionary<string, Dictionary<int, LstItem>>();
		pvf.ListFileTable.LstCountCode = new Dictionary<string, int>();
		List<PvfFile> list = pvf.FileList.Where<KeyValuePair<string, PvfFile>>(delegate(KeyValuePair<string, PvfFile> item)
		{
			KeyValuePair<string, PvfFile> keyValuePair = item;
			string key = keyValuePair.Key;
			keyValuePair = item;
			string text = PathsHelper.PathFix(keyValuePair.Value.DirectoryName);
			keyValuePair = item;
			return key == text + keyValuePair.Value.DirectoryName + ".lst";
		}).Select(delegate(KeyValuePair<string, PvfFile> item)
		{
			KeyValuePair<string, PvfFile> keyValuePair = item;
			return keyValuePair.Value;
		}).ToList();
		if (pvf.FileAny("n_quest/quest.lst"))
		{
			list.Add(pvf.GetFile("n_quest/quest.lst"));
		}
		if (pvf.FileAny("pvp_mission/mission.lst"))
		{
			list.Add(pvf.GetFile("pvp_mission/mission.lst"));
		}
		if (pvf.FileAny("etc/independentdrop.lst"))
		{
			list.Add(pvf.GetFile("etc/independentdrop.lst"));
		}
		foreach (PvfFile item in list)
		{
			l5AefSxFwx(item, pvf);
		}
		foreach (PvfFile item2 in n50eTZnvhd(pvf))
		{
			l5AefSxFwx(item2, pvf);
		}
	}

	internal static void l5AefSxFwx(PvfFile P_0, PvfGroup P_1)
	{
		if (P_0 == null || P_1 == null)
		{
			return;
		}
		Stringtable strtable = P_1.Strtable;
		if (strtable == null)
		{
			return;
		}
		string text = P_0.DirectoryName;
		string text2 = text;
		bool flag = P_0.IsSkillLst();
		if (flag)
		{
			text = "skill";
			text2 = LpueZvDmGI(P_1, P_0);
			if (text2 == null)
			{
				text2 = "skill";
			}
		}
		if (P_0.FileName == "etc/independentdrop.lst")
		{
			text2 = "independentdrop";
		}
		if (string.IsNullOrWhiteSpace(text2) || !P_0.FileName.Contains(text))
		{
			return;
		}
		if (P_1.ListFileTable.CodeDic.ContainsKey(text2))
		{
			P_1.ListFileTable.CodeDic[text2].Clear();
		}
		else
		{
			P_1.ListFileTable.CodeDic.Add(text2, new Dictionary<int, LstItem>());
		}
		if (!P_1.ListFileTable.LstFilePaths.ContainsKey(text2))
		{
			P_1.ListFileTable.LstFilePaths.Add(text2, P_0.FileName);
		}
		if (P_1.ListFileTable.LstCountCode.ContainsKey(P_0.FileName))
		{
			P_1.ListFileTable.LstCountCode.Remove(P_0.FileName);
		}
		Dictionary<int, LstItem> dictionary = P_1.ListFileTable.CodeDic[text2];
		int dataLen = P_0.DataLen;
		if (!P_0.IsScriptFile || dataLen < 12)
		{
			return;
		}
		string text3 = PathsHelper.PathFix(text);
		for (int i = 2; i < dataLen - 5; i += 10)
		{
			int num = BitConverter.ToInt32(P_0.Data, i + 1);
			string stringItem = strtable.GetStringItem(BitConverter.ToInt32(P_0.Data, i + 1 + 5));
			if (stringItem == null)
			{
				continue;
			}
			if (!dictionary.ContainsKey(num))
			{
				dictionary.Add(num, new LstItem(text, stringItem, num));
			}
			string key = text3 + stringItem.Replace('\\', '/').ToLower();
			if (P_1.FileList == null)
			{
				continue;
			}
			P_1.FileList.TryGetValue(key, out PvfFile value);
			if (value != null)
			{
				value.ItemCode = num;
				if (flag)
				{
					value.SkillLstItemPath = stringItem;
				}
			}
		}
		if (P_1.ListFileTable.LstCountCode.ContainsKey(P_0.FileName))
		{
			P_1.ListFileTable.LstCountCode.Remove(P_0.FileName);
		}
		P_1.ListFileTable.LstCountCode.Add(P_0.FileName, dictionary.Keys.ToList().Max());
	}

	internal static string t3Te7YyP9f(PvfFile P_0, PvfGroup P_1)
	{
		string result = P_0.DirectoryName;
		if (P_0.IsSkillLst())
		{
			result = LpueZvDmGI(P_1, P_0);
		}
		return result;
	}

	private static List<PvfFile> n50eTZnvhd(PvfGroup P_0)
	{
		List<PvfFile> list = new List<PvfFile>();
		PvfFile file = P_0.GetFile("skill/skilllist.lst");
		if (file == null)
		{
			return list;
		}
		int dataLen = file.DataLen;
		if (!file.IsScriptFile || dataLen < 12)
		{
			return list;
		}
		for (int i = 2; i < dataLen - 5; i += 10)
		{
			string key = ("skill/" + P_0.Strtable.GetStringItem(BitConverter.ToInt32(file.Data, i + 1 + 5))).Replace('\\', '/').ToLower();
			P_0.FileList.TryGetValue(key, out PvfFile value);
			if (value != null)
			{
				list.Add(value);
			}
		}
		return list;
	}

	private static string LpueZvDmGI(PvfGroup P_0, PvfFile P_1)
	{
		int dataLen = P_1.DataLen;
		if (!P_1.IsScriptFile || dataLen < 12)
		{
			return null;
		}
		for (int i = 2; i < dataLen - 5; i += 10)
		{
			string stringItem = P_0.Strtable.GetStringItem(BitConverter.ToInt32(P_1.Data, i + 1 + 5));
			int num = stringItem.IndexOf('/');
			if (num != -1)
			{
				return ("skill/" + stringItem.Substring(0, num)).ToLower();
			}
		}
		return null;
	}

	public static Dictionary<int, string> FilesToLstItems(PvfGroup pvf, IEnumerable<string> fileList)
	{
		List<PvfFile> files = pvf.GetFiles(fileList);
		if (files == null)
		{
			return null;
		}
		return FilesToLstItems(pvf, files);
	}

	public static Dictionary<int, string> FilesToLstItems(PvfGroup pvf, IEnumerable<PvfFile> fileList)
	{
		if (fileList == null)
		{
			return null;
		}
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		foreach (PvfFile file in fileList)
		{
			if (file.ItemCode.HasValue)
			{
				pvf.ListFileTable.CodeDic.TryGetValue(file.GetLstPathHeader(), out Dictionary<int, LstItem> value);
				if (value != null && value.TryGetValue(file.ItemCode.Value, out var value2) && !dictionary.ContainsKey(file.ItemCode.Value))
				{
					dictionary.Add(file.ItemCode.Value, value2.ItemPath);
				}
			}
		}
		return dictionary;
	}

	public static string FilesToLstItemsToString(PvfGroup pvf, IEnumerable<string> files, out int count)
	{
		count = 0;
		if (files == null)
		{
			return null;
		}
		return FAnehQuUHj(FilesToLstItems(pvf, files), out count);
	}

	public static string FilesToLstItemsToString(PvfGroup pvf, IEnumerable<PvfFile> files, out int count)
	{
		return FAnehQuUHj(FilesToLstItems(pvf, files), out count);
	}

	private static string FAnehQuUHj(Dictionary<int, string> P_0, out int P_1)
	{
		if (P_0 == null)
		{
			P_1 = 0;
			return null;
		}
		P_1 = P_0.Count;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<int, string> item in P_0)
		{
			stringBuilder.AppendLine(item.Key + "\t`" + item.Value + "`");
		}
		return stringBuilder.ToString();
	}
}
