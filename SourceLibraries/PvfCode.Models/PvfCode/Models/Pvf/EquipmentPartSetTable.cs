using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PvfCode.Models.Pvf;

public class EquipmentPartSetTable
{
	public Dictionary<int, Dictionary<string, EquipmentPartSet>> Items { get; set; }

	public Dictionary<string, Dictionary<string, EquipmentPartSet>> PathItems { get; set; }

	public EquipmentPartSetTable()
	{
		Clear();
	}

	public void Clear()
	{
		Items = new Dictionary<int, Dictionary<string, EquipmentPartSet>>();
		PathItems = new Dictionary<string, Dictionary<string, EquipmentPartSet>>();
	}

	public Task Init(PvfPack pack, Dictionary<int, Dictionary<string, EquipmentPartSet>> dic)
	{
		Clear();
		int partSetIndexId = pack.Strtable.GetStringTableId("[part set index]");
		int equipmentTypeId = pack.Strtable.GetStringTableId("[equipment type]");
		if (partSetIndexId == -1)
		{
			AppSetting.Instance.GetIlogger()?.Error("套装文件加载失败 字符串表中不存在标签：[part set index] ");
			return Task.CompletedTask;
		}
		Parallel.ForEach(from item in pack.FileList
			where item.Value.FilePathHeader == "equipment" && item.Value.IsScriptFile && item.Value.FileType == PvfFileType.equ
			select item.Value, file =>
		{
			if (TryGetPartSetInfo(pack, file, partSetIndexId, equipmentTypeId, out int? partSetIndex, out string equipmentType) && dic.TryGetValue(partSetIndex.Value, out Dictionary<string, EquipmentPartSet> partSets) && partSets.TryGetValue(equipmentType, out var partSet))
			{
				lock (this)
				{
					partSet.ReferencesFiles.Add(new EquipmentPartSet.ReferencesRowViewModel(file));
					if (partSet.ReferencesNumber == 0)
					{
						string itemName = pack.GetItemName(file);
						if (!string.IsNullOrEmpty(itemName))
						{
							partSet.SetItemName(itemName);
						}
					}
					partSet.ReferencesNumber++;
				}
			}
		});
		Items = dic;
		AppSetting.Instance.GetIlogger().Success("套装数量：" + dic.Count);
		foreach (KeyValuePair<int, Dictionary<string, EquipmentPartSet>> item in Items)
		{
			string text = item.Value.Values.FirstOrDefault()?.ParFile?.FileName;
			if (text != null && !PathItems.ContainsKey(text))
			{
				PathItems.Add(text, item.Value);
			}
		}
		return Task.CompletedTask;
	}

	private static bool TryGetPartSetInfo(PvfPack pack, PvfFile file, int partSetIndexId, int equipmentTypeId, out int? partSetIndex, out string? equipmentType)
	{
		partSetIndex = null;
		equipmentType = null;
		if (file == null)
		{
			return false;
		}
		byte[] data = file.Data;
		int dataLen = file.DataLen;
		if (data == null || dataLen < 7)
		{
			return false;
		}
		int num = 0;
		for (int i = 2; i < dataLen - 4; i += 5)
		{
			if (data[i] != 5)
			{
				continue;
			}
			int num2 = BitConverter.ToInt32(data, i + 1);
			if (num2 == partSetIndexId && i + 5 < dataLen && data[i + 5] == 2)
			{
				partSetIndex = BitConverter.ToInt32(data, i + 6);
				num++;
				if (equipmentType != null && partSetIndex.HasValue)
				{
					return true;
				}
			}
			else if (num2 == equipmentTypeId && i + 5 < dataLen && data[i + 5] == 7)
			{
				equipmentType = pack.Strtable.GetStringItem(BitConverter.ToInt32(data, i + 6));
				num++;
				if (equipmentType != null && partSetIndex.HasValue)
				{
					return true;
				}
			}
		}
		return false;
	}
}
