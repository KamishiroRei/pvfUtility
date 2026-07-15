using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Collections.Pooled;
using PvfCode.Dot;
using PvfCode.Models.Pvf;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.SearchModel;
using PvfCode.Services.SearchModel.Enums;
using WinCopies.Collections;
using WinCopies.Util;

namespace PvfCode.Services.PvfRelease;

public class ServicePvfReleaseToClient : PvfReleaseBase
{
	private List<string> GetDropFilePaths()
	{
		return new List<string>
		{
			"etc/independent_drop.etc",
			"etc/independentdrop.lst",
			"etc/itemdropinfo_clearreward.etc",
			"etc/itemdropinfo_common.etc",
			"etc/itemdropinfo_control.etc",
			"etc/itemdropinfo_monseter.etc",
			"etc/itemdropinfo_monseter_extra.etc",
			"etc/itemdropinfo_monster_hell.etc",
			"etc/itemdropinfo_object.etc",
			"etc/worlddrop.etc "
		};
	}

	public ServicePvfReleaseToClient(PvfGroup pvf)
		: base(pvf)
	{
	}

	public override async Task<ResultData> Start(PvfReleaseClientOptions options)
	{
		ResultData status = new ResultData();
		if (options.SetDropFileBlank)
		{
			BlankDropFiles();
			status = await DeleteEquipmentCreationRate();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.AddGiftBomb)
		{
			status = await AddGiftBomb();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.ClearQuestMonsterRewardItem)
		{
			status = await ClearQuestMonsterRewardItem();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.DeleteQuestClearRewardItem)
		{
			status = await DeleteQuestClearRewardItem();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.DeleteQuestEnemyRewardItem)
		{
			status = await DeleteQuestEnemyRewardItem();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.DeleteMapDungeon)
		{
			status = await DeleteMapDungeon();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.SetMapMonsterCode1)
		{
			status = await SetMapMonsterCode1();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.SetMapAiCharacterCodeRandomApcId)
		{
			status = await SetMapAiCharacterCodeRandomApcId();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.SetLotteryRate1000)
		{
			status = await SetLotteryRate1000();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.DeleteMobCommonChampionDropItem)
		{
			status = await DeleteMobCommonChampionDropItem();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.DeleteMobItem)
		{
			status = await DeleteMobItem();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.DeletePackageSections)
		{
			status = await DeletePackageSections();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.ReplaceMoboxRandomList)
		{
			status = await ReplaceMoboxRandomList();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.DeletePetEggOutputIndex)
		{
			status = await DeletePetEggOutputIndex();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.ClearEtcRefillItem)
		{
			status = await ClearEtcRefillItem();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.DeleteDungeonMapSpecification)
		{
			status = await DeleteDungeonMapSpecification();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.ConfusePetCanItem)
		{
			status = await ConfusePetCanItem();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.SetEquEmancipateOutput)
		{
			status = await SetEquEmancipateOutput();
			if (status.IsError)
			{
				return status;
			}
		}
		if (options.SetPackageOutput)
		{
			status = await SetPackageOutput();
			if (status.IsError)
			{
				return status;
			}
		}
		await WriteBlankFiles();
		return status;
	}

	private void BlankDropFiles()
	{
		foreach (string item in GetDropFilePaths())
		{
			if (Pvf.FileList.TryGetValue(item, out PvfFile value) && value != null)
			{
				Pvf.SaveFileText(value, "#PVF_File");
			}
		}
	}

	private async Task<ResultData> DeleteEquipmentCreationRate()
	{
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[creation rate]",
			SearchFolder = "equipment",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
			{
				continue;
			}
			IEnumerable<SectionBase> enumerable = scriptFileParserNew.Sections.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[creation rate]");
			if (enumerable != null)
			{
				SectionBase[] array = enumerable.ToArray();
				foreach (SectionBase item in array)
				{
					scriptFileParserNew.Sections.Remove(item);
				}
				Pvf.SaveFileText(value, scriptFileParserNew.GetText());
			}
		}
		return re;
	}

	private async Task<ResultData> DeletePetEggOutputIndex()
	{
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[output index]",
			SearchFolder = "equipment",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
			{
				continue;
			}
			IEnumerable<SectionBase> enumerable = scriptFileParserNew.Sections.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[output index]");
			if (enumerable != null)
			{
				SectionBase[] array = enumerable.ToArray();
				foreach (SectionBase item in array)
				{
					scriptFileParserNew.Sections.Remove(item);
				}
				Pvf.SaveFileText(value, scriptFileParserNew.GetText());
			}
		}
		return re;
	}

	private async Task<ResultData> SetEquEmancipateOutput()
	{
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/emancipate]",
			SearchFolder = "equipment",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
			{
				continue;
			}
			SectionBase sectionBase = scriptFileParserNew.Sections.FirstOrDefault((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[emancipate]" && pvfSection.HasEndSection());
			if (sectionBase == null)
			{
				continue;
			}
			IEnumerable<SectionBase> enumerable = sectionBase.Children.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[output]" && pvfSection.HasEndSection());
			if (enumerable == null)
			{
				continue;
			}
			foreach (SectionBase item in enumerable)
			{
				if (item.Children != null && item.Children.Any() && item.Children.Count == 4 && item.Children[1].Item.Type == ScriptType.Int)
				{
					item.Children[1].Item.Data = 3037;
				}
			}
			Pvf.SaveFileText(value, scriptFileParserNew.GetText());
		}
		return re;
	}

	private async Task<ResultData> AddGiftBomb()
	{
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/booster info]",
			SearchFolder = "stackable",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		try
		{
			foreach (string datum in resultData.Data)
			{
				if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
				{
					continue;
				}
				ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
				scriptFileParserNew.PraseStructureMain();
				if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
				{
					continue;
				}
				bool flag = false;
				SectionBase sectionBase = scriptFileParserNew.Sections.FirstOrDefault((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[booster info]" && pvfSection.HasEndSection());
				if (sectionBase == null || sectionBase.Children == null)
				{
					continue;
				}
				IEnumerable<SectionBase> enumerable = sectionBase.Children.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.HasEndSection());
				if (enumerable == null)
				{
					continue;
				}
				foreach (SectionBase item in enumerable)
				{
					if (item.Children.Count >= 3 && !(item.GetSectionName() == "[avatar]"))
					{
						item.Children.RemoveRange(2, item.Children.Count - 3);
						item.Children.Insert(2, new SectionBase
						{
							Item = new ScriptItem
							{
								Data = 3037,
								Type = ScriptType.Int
							}
						});
						item.Children.Insert(3, new SectionBase
						{
							Item = new ScriptItem
							{
								Data = 1000,
								Type = ScriptType.Int
							}
						});
						item.Children.Insert(4, new SectionBase
						{
							Item = new ScriptItem
							{
								Data = 1,
								Type = ScriptType.Int
							}
						});
						flag = true;
					}
				}
				if (flag)
				{
					Pvf.SaveFileText(value, scriptFileParserNew.GetText());
				}
			}
		}
		catch (Exception ex)
		{
			re.Msg = "SetPackageFile Error:" + ex.Message;
		}
		return re;
	}

	private async Task<ResultData> DeletePackageSections()
	{
		Dictionary<string, bool> deleteSections = new Dictionary<string, bool>
		{
			{
				"[success rate]",
				false
			},
			{
				"[maintain ability]",
				false
			},
			{
				"[broadcast type]",
				false
			},
			{
				"[fail output]",
				true
			}
		};
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[success rate]",
			SearchFolder = "stackable",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
			{
				continue;
			}
			bool flag = false;
			foreach (KeyValuePair<string, bool> item in deleteSections)
			{
				if (RemoveSections(scriptFileParserNew.Sections, item.Key, item.Value))
				{
					flag = true;
				}
			}
			if (flag)
			{
				Pvf.SaveFileText(value, scriptFileParserNew.GetText());
			}
		}
		return new ResultData();
	}

	private async Task<ResultData> ReplaceMoboxRandomList()
	{
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/random list]",
			SearchFolder = "stackable",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
			{
				continue;
			}
			SectionBase sectionBase = scriptFileParserNew.Sections.FirstOrDefault((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[random]" && pvfSection.HasEndSection());
			if (sectionBase == null || sectionBase.Children == null)
			{
				continue;
			}
			IEnumerable<SectionBase> enumerable = sectionBase.Children.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[random list]" && pvfSection.HasEndSection());
			if (enumerable == null || !enumerable.Any())
			{
				continue;
			}
			foreach (SectionBase item in enumerable)
			{
				if (item.Children != null && item.Children.Count > 6)
				{
					item.Children.RemoveRange(5, item.Children.Count - 6);
					item.Children.Insert(5, new SectionBase
					{
						Item = new ScriptItem
						{
							Data = 3037,
							Type = ScriptType.Int
						}
					});
					item.Children.Insert(6, new SectionBase
					{
						Item = new ScriptItem
						{
							Data = 2000,
							Type = ScriptType.Int
						}
					});
					item.Children.Insert(7, new SectionBase
					{
						Item = new ScriptItem
						{
							Data = 1,
							Type = ScriptType.Int
						}
					});
					item.Children.Insert(8, new SectionBase
					{
						Item = new ScriptItem
						{
							Data = 0,
							Type = ScriptType.Int
						}
					});
					Pvf.SaveFileText(value, scriptFileParserNew.GetText());
				}
			}
		}
		return new ResultData();
	}

	private async Task<ResultData> ConfusePetCanItem()
	{
		string equLstFilePath = "equipment/equipment.lst";
		string stackableLstFilePath = "stackable/stackable.lst";
		ResultData<Dictionary<int, LstItem>> lstDicTable = Pvf.GetLstDicTable(equLstFilePath);
		if (lstDicTable.IsError)
		{
			return new ResultData();
		}
		Dictionary<int, LstItem> equOlDic = lstDicTable.Data;
		ResultData<Dictionary<int, LstItem>> lstDicTable2 = Pvf.GetLstDicTable(stackableLstFilePath);
		if (lstDicTable.IsError)
		{
			return new ResultData();
		}
		Dictionary<int, LstItem> stackableOlDic = lstDicTable2.Data;
		Dictionary<int, string> stackableNewTable = new Dictionary<int, string>();
		Dictionary<int, string> equNewTable = new Dictionary<int, string>();
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[upgradable legacy]",
			SearchFolder = "stackable",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
			{
				continue;
			}
			SectionBase sectionBase = scriptFileParserNew.Sections.FirstOrDefault((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[int data]" && pvfSection.HasEndSection());
			if (sectionBase == null)
			{
				continue;
			}
			int num = 0;
			bool flag = false;
			for (int num2 = 3; num2 < sectionBase.Children.Count - 1; num2++)
			{
				num++;
				switch (num)
				{
				case 1:
				{
					ScriptItem item = sectionBase.Children[num2].Item;
					if (item.Type != ScriptType.Int)
					{
						break;
					}
					int data = item.Data;
					bool flag2 = equOlDic.ContainsKey(data);
					if (!dictionary.TryGetValue(data, out var value2))
					{
						value2 = new Random(Guid.NewGuid().GetHashCode()).Next(0, equOlDic.Count + 1000);
						while (stackableOlDic.ContainsKey(value2) || equOlDic.ContainsKey(value2))
						{
							value2 = new Random(Guid.NewGuid().GetHashCode()).Next(0, equOlDic.Count + 10000);
						}
						value2 = CopyItemWithNewCode(flag2 ? equNewTable : stackableNewTable, flag2 ? equOlDic : stackableOlDic, data, value2, flag2, true);
						if (value2 != -1)
						{
							dictionary.Add(data, value2);
						}
					}
					if (value2 != -1)
					{
						item.Data = value2;
						if (!flag)
						{
							flag = true;
						}
					}
					break;
				}
				case 3:
					num = 0;
					break;
				}
			}
			if (flag)
			{
				Pvf.SaveFileText(value, scriptFileParserNew.GetText());
			}
		}
		if (equNewTable.Count > 0)
		{
			InsertLstItems(equLstFilePath, equNewTable);
		}
		if (stackableNewTable.Count > 0)
		{
			InsertLstItems(stackableLstFilePath, stackableNewTable);
		}
		return re;
	}

	private async Task<ResultData> SetPackageOutput()
	{
		string equLstFilePath = "equipment/equipment.lst";
		string stackableLstFilePath = "stackable/stackable.lst";
		ResultData<Dictionary<int, LstItem>> lstDicTable = Pvf.GetLstDicTable(equLstFilePath);
		if (lstDicTable.IsError)
		{
			return new ResultData();
		}
		Dictionary<int, LstItem> equOlDic = lstDicTable.Data;
		ResultData<Dictionary<int, LstItem>> lstDicTable2 = Pvf.GetLstDicTable(stackableLstFilePath);
		if (lstDicTable.IsError)
		{
			return new ResultData();
		}
		Dictionary<int, LstItem> stackableOlDic = lstDicTable2.Data;
		Dictionary<int, string> stackableNewTable = new Dictionary<int, string>();
		Dictionary<int, string> equNewTable = new Dictionary<int, string>();
		Dictionary<int, int> newItemCodeDIc = new Dictionary<int, int>();
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			ScriptContent = "[stackable type]\r\n\t`[recipe]`",
			SearchFolder = "stackable",
			Type = SearchType.ScriptContent,
			ScriptContentSearchMode = ScriptContentSearchMode.二进制
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
			{
				continue;
			}
			SectionBase sectionBase = scriptFileParserNew.Sections.FirstOrDefault((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[int data]" && pvfSection.HasEndSection());
			if (sectionBase == null || sectionBase.Children.Count <= 5)
			{
				continue;
			}
			SectionBase sectionBase2 = sectionBase.Children[sectionBase.Children.Count - 2];
			if (sectionBase2.Item.Type == ScriptType.Int && sectionBase2.Item.Data != 0)
			{
				continue;
			}
			sectionBase.Children[sectionBase.Children.Count - 3].Item.Data = new Random(Guid.NewGuid().GetHashCode()).Next(1, 2000);
			SectionBase sectionBase3 = sectionBase.Children[sectionBase.Children.Count - 4];
			int data = sectionBase3.Item.Data;
			bool flag = equOlDic.ContainsKey(data);
			if (!newItemCodeDIc.TryGetValue(data, out var value2))
			{
				value2 = new Random(Guid.NewGuid().GetHashCode()).Next(0, equOlDic.Count + 1000);
				while (stackableOlDic.ContainsKey(value2) || equOlDic.ContainsKey(value2))
				{
					value2 = new Random(Guid.NewGuid().GetHashCode()).Next(0, equOlDic.Count + 10000);
				}
				value2 = CopyItemWithNewCode(flag ? equNewTable : stackableNewTable, flag ? equOlDic : stackableOlDic, data, value2, flag, true);
				if (value2 != -1)
				{
					newItemCodeDIc.Add(data, value2);
				}
			}
			if (value2 != -1)
			{
				sectionBase3.Item.Data = value2;
			}
			Pvf.SaveFileText(value, scriptFileParserNew.GetText());
		}
		if (equNewTable.Count > 0)
		{
			InsertLstItems(equLstFilePath, equNewTable);
		}
		if (stackableNewTable.Count > 0)
		{
			InsertLstItems(stackableLstFilePath, stackableNewTable);
		}
		return re;
	}

	private void InsertLstItems(string filePath, Dictionary<int, string> items)
	{
		PvfFile file = Pvf.GetFile(filePath);
		if (file == null)
		{
			Pvf.FileList.Add(filePath, new PvfFile(filePath));
			AppSetting.Instance.GetIlogger()?.TreeListAddFiles(new PooledList<string> { filePath });
		}
		List<string> list = Pvf.GetFileText(filePath).Split("\r\n").ToList();
		int length = (file.FilePathHeader + "/").Length;
		foreach (KeyValuePair<int, string> item in items)
		{
			int index = new Random(Guid.NewGuid().GetHashCode()).Next(0, list.Count - 1);
			list.Insert(index, $"{item.Key}\t`{item.Value.Substring(length, item.Value.Length - length)}`");
		}
		Pvf.SaveFileText(filePath, "#PVF_File\r\n" + string.Join("\r\n", list));
	}

	private int CopyItemWithNewCode(Dictionary<int, string> newItems, Dictionary<int, LstItem> oldItems, int oldItemCode, int newItemCode, bool isEquipment, bool cloneFile)
	{
		if (!oldItems.TryGetValue(oldItemCode, out LstItem value) || value == null)
		{
			return -1;
		}
		string fullPath = value.FullPath;
		PvfFile file = Pvf.GetFile(fullPath);
		if (file == null)
		{
			return -1;
		}
		string itemName = Pvf.GetItemName(fullPath);
		string value2 = (isEquipment ? ".equ" : ".stk");
		string text = Path.Combine(Path.GetDirectoryName(fullPath), $"{newItemCode}{value2}").Replace("\\", "/");
		int num = 0;
		while (Pvf.FileAny(text))
		{
			num++;
			string? directoryName = Path.GetDirectoryName(fullPath);
			text = Path.Combine(directoryName, $"{newItemCode}_{num}{value2}").Replace("\\", "/");
		}
		oldItems.Add(newItemCode, null);
		if (cloneFile)
		{
			PvfFile pvfFile = (PvfFile)file.Clone();
			pvfFile.Rename(text);
			Pvf.FileList.Add(text, pvfFile);
		}
		else
		{
			Pvf.FileList.Add(text, new PvfFile(text));
			int rarity = 0;
			file?.GetRarity((PvfPack)Pvf, out rarity);
			Pvf.SaveFileText(text, $"#PVF_File\r\n[name]\r\n`{itemName}`\r\n[rarity]\r\n{rarity}");
		}
		newItems.Add(newItemCode, text);
		return newItemCode;
	}

	private async Task<ResultData> ClearQuestMonsterRewardItem()
	{
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/monster reward item]",
			SearchFolder = "n_quest",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0 || !scriptFileParserNew.Sections.Any((SectionBase it) => it is PvfSection && it.GetSectionName() == "[dungeon info]" && it.HasEndSection()))
			{
				continue;
			}
			IEnumerable<SectionBase> enumerable = scriptFileParserNew.Sections.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[monster reward item]" && it.HasEndSection());
			if (enumerable == null)
			{
				continue;
			}
			foreach (SectionBase item in enumerable)
			{
				if (item.Children.Count >= 3)
				{
					item.Children.RemoveRange(1, item.Children.Count - 2);
				}
			}
			Pvf.SaveFileText(value, scriptFileParserNew.GetText());
		}
		return re;
	}

	private async Task<ResultData> DeleteQuestClearRewardItem()
	{
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/clear reward item]",
			SearchFolder = "n_quest",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections != null && scriptFileParserNew.Sections.Count > 0)
			{
				IEnumerable<SectionBase> enumerable = scriptFileParserNew.Sections.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[clear reward item]" && it.HasEndSection());
				if (enumerable != null)
				{
					ClearSectionContents(enumerable, "[clear reward item]", true);
					Pvf.SaveFileText(value, scriptFileParserNew.GetText());
				}
			}
		}
		return re;
	}

	private async Task<ResultData> DeleteQuestEnemyRewardItem()
	{
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/enemy reward item]",
			SearchFolder = "n_quest",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections != null && scriptFileParserNew.Sections.Count > 0)
			{
				IEnumerable<SectionBase> enumerable = scriptFileParserNew.Sections.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[enemy reward item]" && it.HasEndSection());
				if (enumerable != null)
				{
					scriptFileParserNew.Sections.RemoveRangeIfContains(enumerable.ToArray());
					Pvf.SaveFileText(value, scriptFileParserNew.GetText());
				}
			}
		}
		return re;
	}

	private async Task<ResultData> DeleteMapDungeon()
	{
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/dungeon]",
			SearchFolder = "map",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
			{
				continue;
			}
			IEnumerable<SectionBase> enumerable = scriptFileParserNew.Sections.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[dungeon]" && it.HasEndSection());
			if (enumerable == null)
			{
				continue;
			}
			foreach (SectionBase item in enumerable)
			{
				if (item.Children.Count >= 3)
				{
					item.Children.RemoveRange(1, item.Children.Count - 2);
				}
			}
			Pvf.SaveFileText(value, scriptFileParserNew.GetText());
		}
		return re;
	}

	private async Task<ResultData> SetMapMonsterCode1()
	{
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/monster]",
			SearchFolder = "map",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (!Pvf.FileList.TryGetValue(datum, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
			{
				continue;
			}
			IEnumerable<SectionBase> enumerable = scriptFileParserNew.Sections.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[monster]" && it.HasEndSection());
			if (enumerable == null)
			{
				continue;
			}
			foreach (SectionBase item in enumerable)
			{
				if (item.Children == null || !item.Children.Any())
				{
					continue;
				}
				int num = 0;
				for (int num2 = 1; num2 < item.Children.Count - 2; num2++)
				{
					num++;
					SectionBase sectionBase = item.Children[num2];
					if (num == 1 && sectionBase.Item.Type == ScriptType.Int)
					{
						sectionBase.Item.Data = 1;
					}
					if (num == 10)
					{
						num = 0;
					}
				}
			}
			Pvf.SaveFileText(value, scriptFileParserNew.GetText());
		}
		return re;
	}

	private async Task<ResultData> SetMapAiCharacterCodeRandomApcId()
	{
		ResultData re = new ResultData();
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/ai character]",
			SearchFolder = "map",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		if (Pvf.ListFileTable.CodeDic.TryGetValue("aicharacter", out Dictionary<int, LstItem> value))
		{
			if (value != null && value.Count > 0)
			{
				int data = value.Keys.OrderBy((int u) => Guid.NewGuid()).First();
				foreach (string datum in resultData.Data)
				{
					if (!Pvf.FileList.TryGetValue(datum, out PvfFile value2) || value2 == null)
					{
						continue;
					}
					ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value2, Pvf);
					scriptFileParserNew.PraseStructureMain();
					if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
					{
						continue;
					}
					IEnumerable<SectionBase> enumerable = scriptFileParserNew.Sections.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[ai character]" && it.HasEndSection());
					if (enumerable == null)
					{
						continue;
					}
					foreach (SectionBase item in enumerable)
					{
						if (item.Children == null || !item.Children.Any())
						{
							continue;
						}
						int num = 0;
						for (int num2 = 1; num2 < item.Children.Count - 2; num2++)
						{
							num++;
							SectionBase sectionBase = item.Children[num2];
							if (num == 1 && sectionBase.Item.Type == ScriptType.Int)
							{
								sectionBase.Item.Data = data;
							}
							if (num == 8)
							{
								num = 0;
							}
						}
					}
					Pvf.SaveFileText(value2, scriptFileParserNew.GetText());
				}
				return re;
			}
			return re;
		}
		return re;
	}

	private HashSet<string> GetLotteryFilePaths()
	{
		return new HashSet<string>
		{
			"etc/pcroom.vm",
			"etc/pcroom3.vm",
			"etc/pcroom4.vm"
		};
	}

	private Task<ResultData> SetLotteryRate1000()
	{
		foreach (string item in GetLotteryFilePaths())
		{
			if (!Pvf.FileList.TryGetValue(item, out PvfFile value) || value == null)
			{
				continue;
			}
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count <= 0)
			{
				continue;
			}
			IEnumerable<SectionBase> enumerable = scriptFileParserNew.Sections[0].Children.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == "[output]");
			if (enumerable == null)
			{
				continue;
			}
			foreach (SectionBase item2 in enumerable)
			{
				if (item2.Children == null || !item2.Children.Any())
				{
					continue;
				}
				int num = 0;
				for (int num2 = 1; num2 < item2.Children.Count - 2; num2++)
				{
					num++;
					SectionBase sectionBase = item2.Children[num2];
					if (num == 2 && sectionBase.Item.Type == ScriptType.Int)
					{
						sectionBase.Item.Data = 1000;
					}
					if (num == 4)
					{
						num = 0;
					}
				}
			}
			Pvf.SaveFileText(value, scriptFileParserNew.GetText());
		}
		return Task.FromResult(new ResultData());
	}

	private async Task<ResultData> DeleteMobItem()
	{
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/item]",
			SearchFolder = "monster",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (Pvf.FileList.TryGetValue(datum, out PvfFile value) && value != null)
			{
				ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
				scriptFileParserNew.PraseStructureMain();
				if (scriptFileParserNew.Sections != null && scriptFileParserNew.Sections.Count > 0 && RemoveSections(scriptFileParserNew.Sections, "[item]", true))
				{
					Pvf.SaveFileText(value, scriptFileParserNew.GetText());
				}
			}
		}
		return new ResultData();
	}

	private async Task<ResultData> DeleteMobCommonChampionDropItem()
	{
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = "[/common champion drop item]",
			SearchFolder = "monster",
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (Pvf.FileList.TryGetValue(datum, out PvfFile value) && value != null)
			{
				ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
				scriptFileParserNew.PraseStructureMain();
				if (scriptFileParserNew.Sections != null && scriptFileParserNew.Sections.Count > 0 && RemoveSections(scriptFileParserNew.Sections, "[common champion drop item]", true))
				{
					Pvf.SaveFileText(value, scriptFileParserNew.GetText());
				}
			}
		}
		return new ResultData();
	}

	private Task<ResultData> ClearEtcRefillItem()
	{
		ResultData result = new ResultData();
		if (Pvf.FileList.TryGetValue("etc/chn_server_limititemusageinfo.etc", out PvfFile value) && value != null)
		{
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
			scriptFileParserNew.PraseStructureMain();
			if (scriptFileParserNew.Sections != null && scriptFileParserNew.Sections.Count > 0)
			{
				ClearSectionContents(scriptFileParserNew.Sections, "[refill item]", true);
				Pvf.SaveFileText(value, scriptFileParserNew.GetText());
			}
		}
		return Task.FromResult(result);
	}

	private async Task<ResultData> DeleteDungeonMapSpecification()
	{
		ResultData resultData = await SearchAndRemoveSections("[/map specification]", "dungeon", "[map specification]");
		if (resultData.IsError)
		{
			return resultData;
		}
		resultData = await SearchAndRemoveSections("[/special passive object item]", "dungeon", "[special passive object item]");
		if (resultData.IsError)
		{
			return resultData;
		}
		resultData = await SearchAndRemoveSections("[/start map]", "dungeon", "[start map]");
		if (resultData.IsError)
		{
			return resultData;
		}
		resultData = await SearchAndRemoveSections("[/boss map]", "dungeon", "[boss map]");
		if (resultData.IsError)
		{
			return resultData;
		}
		return new ResultData();
	}

	private async Task<ResultData> SearchAndRemoveSections(string keyword, string searchFolder, string sectionName, bool hasEnding = true)
	{
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = keyword,
			SearchFolder = searchFolder,
			Type = SearchType.Strings
		}, Pvf).Search();
		if (resultData.IsError)
		{
			return resultData;
		}
		foreach (string datum in resultData.Data)
		{
			if (Pvf.FileList.TryGetValue(datum, out PvfFile value) && value != null)
			{
				ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, Pvf);
				scriptFileParserNew.PraseStructureMain();
				if (scriptFileParserNew.Sections != null && scriptFileParserNew.Sections.Count > 0 && RemoveSections(scriptFileParserNew.Sections, sectionName, hasEnding))
				{
					Pvf.SaveFileText(value, scriptFileParserNew.GetText());
				}
			}
		}
		return new ResultData();
	}

	private bool RemoveSections(IList<SectionBase> sections, string sectionName, bool hasEnding)
	{
		if (sections == null)
		{
			return false;
		}
		IEnumerable<SectionBase> enumerable = sections.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == sectionName && pvfSection.HasEndSection() == hasEnding);
		bool result = false;
		if (enumerable != null)
		{
			sections.RemoveRangeIfContains(enumerable.ToArray());
			result = true;
		}
		return result;
	}

	private bool ClearSectionContents(IEnumerable<SectionBase> sections, string sectionName, bool hasEnding)
	{
		if (sections == null)
		{
			return false;
		}
		IEnumerable<SectionBase> enumerable = sections.Where((SectionBase it) => it is PvfSection pvfSection && pvfSection.GetSectionName() == sectionName && pvfSection.HasEndSection() == hasEnding);
		bool result = false;
		if (enumerable != null)
		{
			foreach (SectionBase item in enumerable)
			{
				if (item.Children != null && item.Children.Count >= 3)
				{
					item.Children.RemoveRange(1, item.Children.Count - 2);
					result = true;
				}
			}
		}
		return result;
	}

	public override Task<ResultData> Start()
	{
		return null;
	}
}
