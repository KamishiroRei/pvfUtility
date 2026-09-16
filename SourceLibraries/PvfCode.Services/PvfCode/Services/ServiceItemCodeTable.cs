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
		List<PvfFile> lstFiles = pvf.FileList
			.Where(entry => entry.Key == PathsHelper.PathFix(entry.Value.DirectoryName) + entry.Value.DirectoryName + ".lst")
			.Select(entry => entry.Value)
			.ToList();
		if (pvf.FileAny("n_quest/quest.lst"))
		{
			lstFiles.Add(pvf.GetFile("n_quest/quest.lst"));
		}
		if (pvf.FileAny("pvp_mission/mission.lst"))
		{
			lstFiles.Add(pvf.GetFile("pvp_mission/mission.lst"));
		}
		if (pvf.FileAny("etc/independentdrop.lst"))
		{
			lstFiles.Add(pvf.GetFile("etc/independentdrop.lst"));
		}
		foreach (PvfFile lstFile in lstFiles)
		{
			LoadLstFile(lstFile, pvf);
		}
		foreach (PvfFile skillLstFile in GetSkillLstFiles(pvf))
		{
			LoadLstFile(skillLstFile, pvf);
		}
	}

	internal static void LoadLstFile(PvfFile file, PvfGroup pvf)
	{
		if (file == null || pvf == null)
		{
			return;
		}
		// 懒加载兼容（统一管线）：LST 文件内容按需取回经典视图后再扫描
		pvf.EnsureFileData(file.FileName);
		Stringtable strtable = pvf.Strtable;
		if (strtable == null)
		{
			return;
		}
		string directoryName = file.DirectoryName;
		string tableKey = directoryName;
		bool isSkillLst = file.IsSkillLst();
		if (isSkillLst)
		{
			directoryName = "skill";
			tableKey = GetSkillDirectory(pvf, file);
			if (tableKey == null)
			{
				tableKey = "skill";
			}
		}
		if (file.FileName == "etc/independentdrop.lst")
		{
			tableKey = "independentdrop";
		}
		if (string.IsNullOrWhiteSpace(tableKey) || !file.FileName.Contains(directoryName))
		{
			return;
		}
		if (pvf.ListFileTable.CodeDic.ContainsKey(tableKey))
		{
			pvf.ListFileTable.CodeDic[tableKey].Clear();
		}
		else
		{
			pvf.ListFileTable.CodeDic.Add(tableKey, new Dictionary<int, LstItem>());
		}
		if (!pvf.ListFileTable.LstFilePaths.ContainsKey(tableKey))
		{
			pvf.ListFileTable.LstFilePaths.Add(tableKey, file.FileName);
		}
		if (pvf.ListFileTable.LstCountCode.ContainsKey(file.FileName))
		{
			pvf.ListFileTable.LstCountCode.Remove(file.FileName);
		}
		Dictionary<int, LstItem> lstItems = pvf.ListFileTable.CodeDic[tableKey];
		int dataLen = file.DataLen;
		if (!file.IsScriptFile || dataLen < 12)
		{
			return;
		}
		string normalizedDirectory = PathsHelper.PathFix(directoryName);
		for (int i = 2; i < dataLen - 5; i += 10)
		{
			int itemCode = BitConverter.ToInt32(file.Data, i + 1);
			string itemPath = strtable.GetStringItem(BitConverter.ToInt32(file.Data, i + 1 + 5));
			if (itemPath == null)
			{
				continue;
			}
			if (!lstItems.ContainsKey(itemCode))
			{
				lstItems.Add(itemCode, new LstItem(directoryName, itemPath, itemCode));
			}
			string filePath = normalizedDirectory + itemPath.Replace('\\', '/').ToLower();
			if (pvf.FileList == null)
			{
				continue;
			}
			pvf.FileList.TryGetValue(filePath, out PvfFile referencedFile);
			if (referencedFile != null)
			{
				referencedFile.ItemCode = itemCode;
				if (isSkillLst)
				{
					referencedFile.SkillLstItemPath = itemPath;
				}
			}
		}
		if (pvf.ListFileTable.LstCountCode.ContainsKey(file.FileName))
		{
			pvf.ListFileTable.LstCountCode.Remove(file.FileName);
		}
		pvf.ListFileTable.LstCountCode.Add(file.FileName, lstItems.Keys.ToList().Max());
	}

	private static List<PvfFile> GetSkillLstFiles(PvfGroup pvf)
	{
		List<PvfFile> skillLstFiles = new List<PvfFile>();
		PvfFile file = pvf.GetFile("skill/skilllist.lst");
		if (file == null)
		{
			return skillLstFiles;
		}
		// 懒加载兼容（统一管线）：先取回经典视图再扫描
		pvf.EnsureFileData(file.FileName);
		int dataLen = file.DataLen;
		if (!file.IsScriptFile || dataLen < 12)
		{
			return skillLstFiles;
		}
		for (int i = 2; i < dataLen - 5; i += 10)
		{
			string filePath = ("skill/" + pvf.Strtable.GetStringItem(BitConverter.ToInt32(file.Data, i + 1 + 5))).Replace('\\', '/').ToLower();
			pvf.FileList.TryGetValue(filePath, out PvfFile skillLstFile);
			if (skillLstFile != null)
			{
				skillLstFiles.Add(skillLstFile);
			}
		}
		return skillLstFiles;
	}

	private static string GetSkillDirectory(PvfGroup pvf, PvfFile file)
	{
		int dataLen = file.DataLen;
		if (!file.IsScriptFile || dataLen < 12)
		{
			return null;
		}
		for (int i = 2; i < dataLen - 5; i += 10)
		{
			string skillPath = pvf.Strtable.GetStringItem(BitConverter.ToInt32(file.Data, i + 1 + 5));
			int separatorIndex = skillPath.IndexOf('/');
			if (separatorIndex != -1)
			{
				return ("skill/" + skillPath.Substring(0, separatorIndex)).ToLower();
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
		Dictionary<int, string> lstItems = new Dictionary<int, string>();
		foreach (PvfFile file in fileList)
		{
			if (file.ItemCode.HasValue)
			{
				pvf.ListFileTable.CodeDic.TryGetValue(file.GetLstPathHeader(), out Dictionary<int, LstItem> codeTable);
				if (codeTable != null && codeTable.TryGetValue(file.ItemCode.Value, out var lstItem) && !lstItems.ContainsKey(file.ItemCode.Value))
				{
					lstItems.Add(file.ItemCode.Value, lstItem.ItemPath);
				}
			}
		}
		return lstItems;
	}

	public static string FilesToLstItemsToString(PvfGroup pvf, IEnumerable<string> files, out int count)
	{
		count = 0;
		if (files == null)
		{
			return null;
		}
		return FormatLstItems(FilesToLstItems(pvf, files), out count);
	}

	public static string FilesToLstItemsToString(PvfGroup pvf, IEnumerable<PvfFile> files, out int count)
	{
		return FormatLstItems(FilesToLstItems(pvf, files), out count);
	}

	private static string FormatLstItems(Dictionary<int, string> lstItems, out int count)
	{
		if (lstItems == null)
		{
			count = 0;
			return null;
		}
		count = lstItems.Count;
		StringBuilder output = new StringBuilder();
		foreach (KeyValuePair<int, string> item in lstItems)
		{
			output.AppendLine(item.Key + "\t`" + item.Value + "`");
		}
		return output.ToString();
	}
}
