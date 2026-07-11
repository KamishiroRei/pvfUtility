using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace PvfCode.Views.Tools;

internal class WorldDropToolViewModel : ViewModelBase
{
	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	public int DeleteLevelMin
	{
		get => GetProperty(() => DeleteLevelMin);
		set => SetProperty(() => DeleteLevelMin, value);
	}

	public int DeleteLevelMax
	{
		get => GetProperty(() => DeleteLevelMax);
		set => SetProperty(() => DeleteLevelMax, value);
	}

	public string DeleteInputValue
	{
		get => GetProperty(() => DeleteInputValue);
		set => SetProperty(() => DeleteInputValue, value);
	}

	public WorldDropToolViewModel()
	{
		DeleteLevelMax = 200;
		DeleteLevelMin = 1;
	}

	[Command]
	public void OnDelete()
	{
		if (string.IsNullOrEmpty(DeleteInputValue))
		{
			AppCore.ShowMsg("请先输入要删除的代码");
			return;
		}

		List<int> itemCodes = ParseItemCodes();
		if (itemCodes.Count == 0)
		{
			throw new Exception("没有符合条件的代码！");
		}
		if (!Pvf.FileAny("etc/worlddrop.etc"))
		{
			throw new Exception("全局掉落文件不存在！");
		}
		if (!Pvf.GetFile("etc/worlddrop.etc").GetSectionIntArray(Pvf, "[world drop]", out List<int> items))
		{
			throw new Exception("全局掉落文件不存在:[world drop]节点");
		}

		Dictionary<int, List<int>> valuesByLevel = ParseWorldDropValues(items);
		Dictionary<int, List<KeyValuePair<int, int>>> dropsByLevel = new Dictionary<int, List<KeyValuePair<int, int>>>();
		foreach (KeyValuePair<int, List<int>> level in valuesByLevel)
		{
			if (level.Value.Count % 2 != 0)
			{
				throw new Exception($"等级：{level.Key}的代码数量异常无法分配为2个一组。请检查源数据！");
			}
			dropsByLevel.Add(level.Key, PairValues(level.Value));
		}

		foreach (KeyValuePair<int, List<KeyValuePair<int, int>>> level in dropsByLevel)
		{
			if (level.Key < DeleteLevelMin || level.Key > DeleteLevelMax)
			{
				continue;
			}

			foreach (int itemCode in itemCodes)
			{
				foreach (KeyValuePair<int, int> drop in level.Value.ToArray())
				{
					if (drop.Key == itemCode)
					{
						level.Value.Remove(drop);
					}
				}
			}
		}

		StringBuilder output = new StringBuilder("#PVF_File\r\n[world drop]\r\n");
		foreach (KeyValuePair<int, List<KeyValuePair<int, int>>> level in dropsByLevel.OrderBy(pair => pair.Key))
		{
			output.AppendLine($"{level.Key}\t0");
			foreach (KeyValuePair<int, int> drop in level.Value)
			{
				output.AppendLine($"{drop.Key}\t{drop.Value}");
			}
			output.AppendLine("-1");
		}

		Pvf.SaveFileText("etc/worlddrop.etc", output.ToString());
		AppCore.ShowMsg($"删除成功 总数：{itemCodes.Count}");
	}

	private List<int> ParseItemCodes()
	{
		string[] values = DeleteInputValue.Split(
			new[] { "\t", "\r\n", " " },
			StringSplitOptions.None);
		List<int> itemCodes = new List<int>();
		foreach (string value in values)
		{
			if (int.TryParse(value, out int itemCode))
			{
				itemCodes.Add(itemCode);
			}
			else
			{
				throw new Exception(value + "不是数字，请检查！");
			}
		}
		return itemCodes;
	}

	private static Dictionary<int, List<int>> ParseWorldDropValues(IEnumerable<int> items)
	{
		Dictionary<int, List<int>> valuesByLevel = new Dictionary<int, List<int>>();
		List<int> currentValues = new List<int>();
		int currentLevel = 0;
		int position = 0;
		foreach (int value in items)
		{
			if (value == -1)
			{
				currentValues = new List<int>();
				position = 0;
				continue;
			}

			position++;
			switch (position)
			{
			case 1:
				currentLevel = value;
				break;
			case 2:
				valuesByLevel.Add(currentLevel, currentValues);
				break;
			default:
				currentValues.Add(value);
				break;
			}
		}
		return valuesByLevel;
	}

	private static List<KeyValuePair<int, int>> PairValues(List<int> values)
	{
		List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();
		for (int index = 0; index < values.Count; index += 2)
		{
			pairs.Add(new KeyValuePair<int, int>(values[index], values[index + 1]));
		}
		return pairs;
	}
}
