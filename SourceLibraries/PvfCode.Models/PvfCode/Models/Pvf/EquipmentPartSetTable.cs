using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PvfCode.Models.Pvf;

public class EquipmentPartSetTable
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass10_0
	{
		public EquipmentPartSetTable HyH83kA7IE;

		public PvfPack io88H5Heig;

		public int u8O877C3mX;

		public int hhg8cXZJIM;

		public Dictionary<int, Dictionary<string, EquipmentPartSet>> OG98gJ4TEZ;

		public _003C_003Ec__DisplayClass10_0()
		{
		}

		internal void W0Q8DtgvdM(PvfFile file)
		{
			if (!HyH83kA7IE.MPDkSq9G8a(io88H5Heig, file, u8O877C3mX, hhg8cXZJIM, out int? num, out string equType) || !OG98gJ4TEZ.TryGetValue(num.Value, out Dictionary<string, EquipmentPartSet> value) || !value.TryGetValue(equType, out var value2))
			{
				return;
			}
			lock (HyH83kA7IE)
			{
				value2.ReferencesFiles.Add(new EquipmentPartSet.ReferencesRowViewModel(file));
				if (value2.ReferencesNumber == 0)
				{
					string itemName = io88H5Heig.GetItemName(file);
					if (!string.IsNullOrEmpty(itemName))
					{
						value2.SetItemName(itemName);
					}
				}
				value2.ReferencesNumber++;
			}
		}
	}

	[CompilerGenerated]
	private Dictionary<int, Dictionary<string, EquipmentPartSet>> HL5kB9TPGl;

	[CompilerGenerated]
	private Dictionary<string, Dictionary<string, EquipmentPartSet>> CHTk4I6q8d;

	public Dictionary<int, Dictionary<string, EquipmentPartSet>> Items
	{
		[CompilerGenerated]
		get
		{
			return HL5kB9TPGl;
		}
		[CompilerGenerated]
		set
		{
			HL5kB9TPGl = value;
		}
	}

	public Dictionary<string, Dictionary<string, EquipmentPartSet>> PathItems
	{
		[CompilerGenerated]
		get
		{
			return CHTk4I6q8d;
		}
		[CompilerGenerated]
		set
		{
			CHTk4I6q8d = value;
		}
	}

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
		_003C_003Ec__DisplayClass10_0 CS_0024_003C_003E8__locals18 = new _003C_003Ec__DisplayClass10_0();
		CS_0024_003C_003E8__locals18.HyH83kA7IE = this;
		CS_0024_003C_003E8__locals18.io88H5Heig = pack;
		CS_0024_003C_003E8__locals18.OG98gJ4TEZ = dic;
		Clear();
		CS_0024_003C_003E8__locals18.u8O877C3mX = CS_0024_003C_003E8__locals18.io88H5Heig.Strtable.GetStringTableId("[part set index]");
		CS_0024_003C_003E8__locals18.hhg8cXZJIM = CS_0024_003C_003E8__locals18.io88H5Heig.Strtable.GetStringTableId("[equipment type]");
		if (CS_0024_003C_003E8__locals18.u8O877C3mX == -1)
		{
			AppSetting.Instance.GetIlogger()?.Error("套装文件加载失败 字符串表中不存在标签：[part set index] ");
			return Task.CompletedTask;
		}
		Parallel.ForEach(from it in CS_0024_003C_003E8__locals18.io88H5Heig.FileList
			where it.Value.FilePathHeader == "equipment" && it.Value.IsScriptFile && it.Value.FileType == PvfFileType.equ
			select it.Value, delegate(PvfFile file)
		{
			if (CS_0024_003C_003E8__locals18.HyH83kA7IE.MPDkSq9G8a(CS_0024_003C_003E8__locals18.io88H5Heig, file, CS_0024_003C_003E8__locals18.u8O877C3mX, CS_0024_003C_003E8__locals18.hhg8cXZJIM, out int? num, out string equType) && CS_0024_003C_003E8__locals18.OG98gJ4TEZ.TryGetValue(num.Value, out Dictionary<string, EquipmentPartSet> value) && value.TryGetValue(equType, out var value2))
			{
				lock (CS_0024_003C_003E8__locals18.HyH83kA7IE)
				{
					value2.ReferencesFiles.Add(new EquipmentPartSet.ReferencesRowViewModel(file));
					if (value2.ReferencesNumber == 0)
					{
						string itemName = CS_0024_003C_003E8__locals18.io88H5Heig.GetItemName(file);
						if (!string.IsNullOrEmpty(itemName))
						{
							value2.SetItemName(itemName);
						}
					}
					value2.ReferencesNumber++;
				}
			}
		});
		Items = CS_0024_003C_003E8__locals18.OG98gJ4TEZ;
		AppSetting.Instance.GetIlogger().Success("套装数量：" + CS_0024_003C_003E8__locals18.OG98gJ4TEZ.Count);
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

	private bool MPDkSq9G8a(PvfPack P_0, PvfFile P_1, int P_2, int P_3, out int? P_4, out string? equType)
	{
		P_4 = null;
		equType = null;
		if (P_1 == null)
		{
			return false;
		}
		byte[] data = P_1.Data;
		int dataLen = P_1.DataLen;
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
			if (num2 == P_2 && i + 5 < dataLen && data[i + 5] == 2)
			{
				P_4 = BitConverter.ToInt32(data, i + 6);
				num++;
				if (equType != null && P_4.HasValue)
				{
					return true;
				}
			}
			else if (num2 == P_3 && i + 5 < dataLen && data[i + 5] == 7)
			{
				equType = P_0.Strtable.GetStringItem(BitConverter.ToInt32(data, i + 6));
				num++;
				if (equType != null && P_4.HasValue)
				{
					return true;
				}
			}
		}
		return false;
	}
}
