using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Services.PreviewPvfFileFolder.Stackable;
using PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;
using PvfCode.Services.PvfParsingNew;

namespace PvfCode.Services;

public class ServiceStackable
{
	private readonly PvfGroup r3ne9f5cWs;

	private readonly PvfFile Bleeqw5kG4;

	public readonly ScriptFileParserNew ScriptParserNew;

	public ServiceStackable(PvfGroup pvf, PvfFile file)
	{
		r3ne9f5cWs = pvf;
		Bleeqw5kG4 = file;
		ScriptParserNew = new ScriptFileParserNew(file, pvf);
		ScriptParserNew.PraseStructureMain();
	}

	private bool pCned5xYTK()
	{
		if (Bleeqw5kG4 == null || Bleeqw5kG4.Data == null || Bleeqw5kG4.DataLen < 7 || ScriptParserNew.Sections == null || ScriptParserNew.Sections.Count == 0)
		{
			return false;
		}
		return true;
	}

	public bool GetBoosterInfo(out BoosterInfo? boosterInfo)
	{
		boosterInfo = null;
		if (!pCned5xYTK())
		{
			return false;
		}
		SectionBase sectionBase = ScriptParserNew.Sections.Where((SectionBase it) => it.GetSectionName() == "[booster info]").FirstOrDefault();
		if (sectionBase == null || sectionBase.Children.Count == 0)
		{
			return false;
		}
		boosterInfo = new BoosterInfo(r3ne9f5cWs, Bleeqw5kG4);
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		StringBuilder stringBuilder = new StringBuilder();
		for (int num = 1; num < sectionBase.Children.Count - 1; num++)
		{
			SectionBase sectionBase2 = sectionBase.Children[num];
			if (sectionBase2 is PvfSection)
			{
				StringBuilder stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler;
				if (!BoosterInfo.StrConvertBoosterType(sectionBase2.GetSectionName(), out var type))
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder3 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(33, 2, stringBuilder2);
					handler.AppendLiteral("无法识别的 [booster info] 子类型：");
					handler.AppendFormatted(sectionBase2.GetSectionName());
					handler.AppendLiteral(" file://");
					handler.AppendFormatted(Bleeqw5kG4.FileName);
					stringBuilder3.AppendLine(ref handler);
					continue;
				}
				if (sectionBase2.Children.Count < 5)
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder4 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(15, 2, stringBuilder2);
					handler.AppendLiteral("数据格式错误：");
					handler.AppendFormatted(sectionBase2.GetSectionName());
					handler.AppendLiteral(" file://");
					handler.AppendFormatted(Bleeqw5kG4.FileName);
					stringBuilder4.AppendLine(ref handler);
				}
				string error = null;
				BoosterInfo.BoosterInfoItemRoot result = null;
				if (type.Value switch
				{
					BoosterType.Special_Avatar => BoosterInfo.Avatar.Create(type.Value, sectionBase2.Children.GetRange(1, sectionBase2.HasEndSection() ? (sectionBase2.Children.Count - 2) : (sectionBase2.Children.Count - 1)), out result, out error), 
					BoosterType.Avatar => BoosterInfo.Avatar.Create(type.Value, sectionBase2.Children.GetRange(1, sectionBase2.HasEndSection() ? (sectionBase2.Children.Count - 2) : (sectionBase2.Children.Count - 1)), out result, out error), 
					_ => BoosterInfo.BoosterInfoItemBase.Create(type.Value, sectionBase2.Children.GetRange(1, sectionBase2.HasEndSection() ? (sectionBase2.Children.Count - 2) : (sectionBase2.Children.Count - 1)), out result, out error), 
				})
				{
					if (result != null)
					{
						boosterInfo.Items.Add(result);
					}
					continue;
				}
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder5 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(15, 3, stringBuilder2);
				handler.AppendLiteral("数据错误：");
				handler.AppendFormatted(sectionBase2.GetSectionName());
				handler.AppendLiteral(" file://");
				handler.AppendFormatted(Bleeqw5kG4.FileName);
				handler.AppendLiteral("  ");
				handler.AppendFormatted(error);
				stringBuilder5.AppendLine(ref handler);
			}
			else
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder6 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(36, 1, stringBuilder2);
				handler.AppendLiteral("[booster info] 标签内不支持非标签直接内容 file://");
				handler.AppendFormatted(Bleeqw5kG4.FileName);
				stringBuilder6.AppendLine(ref handler);
			}
		}
		if (stringBuilder.Length > 0)
		{
			ilogger.Error(stringBuilder.ToString());
		}
		return boosterInfo.Items.Count > 0;
	}

	public bool GetBoosterSelectionInfo(out BoosterInfo? boosterInfo)
	{
		boosterInfo = null;
		if (!pCned5xYTK())
		{
			return false;
		}

		PvfSection? categoryNumSection = ScriptParserNew.Sections.OfType<PvfSection>().FirstOrDefault(it => it.GetSectionName() == "[booster category num]");
		PvfSection? categoryNameSection = ScriptParserNew.Sections.OfType<PvfSection>().FirstOrDefault(it => it.GetSectionName() == "[booster category name]");
		List<PvfSection> categorySections = ScriptParserNew.Sections.OfType<PvfSection>().Where(it => it.GetSectionName() == "[booster select category]").ToList();
		if (categoryNumSection == null || categoryNameSection == null || categorySections.Count == 0)
		{
			return false;
		}

		List<int> categoryNumbers = GetDirectIntValues(categoryNumSection);
		if (categoryNumbers.Count < 2 || categoryNumbers[0] is < 1 or > 2 || categoryNumbers[1] <= 0)
		{
			return false;
		}

		int menuLevel = categoryNumbers[0];
		int primaryCount = categoryNumbers[1];
		int secondaryCount = menuLevel == 2 && categoryNumbers.Count > 2 ? categoryNumbers[2] : 0;
		if (menuLevel == 2 && secondaryCount <= 0)
		{
			return false;
		}

		List<string> categoryNames = GetDirectTextValues(categoryNameSection);
		boosterInfo = new BoosterInfo(r3ne9f5cWs, Bleeqw5kG4)
		{
			SelectionMenuLevel = menuLevel,
			PrimaryCategoryCount = primaryCount,
			SecondaryCategoryCount = secondaryCount,
			SelectionPrompt = categoryNames.ElementAtOrDefault(0),
			SecondarySelectionPrompt = menuLevel == 2 ? categoryNames.ElementAtOrDefault(primaryCount + 1) : null
		};

		List<string> primaryNames = categoryNames.Skip(1).Take(primaryCount).ToList();
		List<string> secondaryNames = menuLevel == 2
			? categoryNames.Skip(primaryCount + 2).Take(primaryCount * secondaryCount).ToList()
			: new List<string>();
		List<SelectionCategoryBlock> categoryBlocks = GetSelectionCategoryBlocks(categorySections);
		bool primaryIndexesAreOneBased = UsesOneBasedIndexes(categoryBlocks.Select(block => block.PrimaryIndex), primaryCount);
		bool secondaryIndexesAreOneBased = menuLevel == 2 && UsesOneBasedIndexes(categoryBlocks.Select(block => block.SecondaryIndex), secondaryCount);
		StringBuilder errors = new StringBuilder();
		foreach (SelectionCategoryBlock block in categoryBlocks)
		{
			int primaryIndex = NormalizeCategoryIndex(block.PrimaryIndex, primaryCount, primaryIndexesAreOneBased);
			int secondaryIndex = menuLevel == 2 ? NormalizeCategoryIndex(block.SecondaryIndex, secondaryCount, secondaryIndexesAreOneBased) : 0;
			string primaryName = GetCategoryName(primaryNames, primaryIndex, block.PrimaryIndex, "一级");
			string secondaryName = menuLevel == 2
				? GetSecondaryCategoryName(secondaryNames, primaryIndex, secondaryIndex, secondaryCount, block.SecondaryIndex)
				: string.Empty;

			foreach (PvfSection itemSection in block.ItemSections)
			{
				if (!BoosterInfo.StrConvertBoosterType(itemSection.GetSectionName(), out BoosterType? boosterType) || !boosterType.HasValue)
				{
					continue;
				}
				List<int> values = GetDirectIntValues(itemSection);
				int groupLength = GetSelectionItemGroupLength(boosterType.Value, values.Count);
				string typeName = BoosterInfo.BoosterTypeToName(boosterType.Value);
				string title = menuLevel == 2
					? $"{primaryName} > {secondaryName} - {typeName}"
					: $"{primaryName} - {typeName}";
				if (BoosterInfo.CreateSelectionItemRoot(boosterType.Value, values, groupLength, title, block.PrimaryIndex, block.SecondaryIndex, out BoosterInfo.BoosterInfoItemRoot? result, out string? error) && result != null)
				{
					boosterInfo.Items.Add(result);
				}
				if (!string.IsNullOrEmpty(error))
				{
					errors.AppendLine($"{itemSection.GetSectionName()} {title}: {error} file://{Bleeqw5kG4.FileName}");
				}
			}
		}

		if (errors.Length > 0)
		{
			AppSetting.Instance.GetIlogger()?.Error(errors.ToString());
		}
		return boosterInfo.Items.Count > 0;
	}

	private sealed class SelectionCategoryBlock
	{
		public int PrimaryIndex { get; init; }

		public int SecondaryIndex { get; init; }

		public List<PvfSection> ItemSections { get; } = new List<PvfSection>();
	}

	private static List<SelectionCategoryBlock> GetSelectionCategoryBlocks(IEnumerable<PvfSection> categorySections)
	{
		List<SelectionCategoryBlock> blocks = new List<SelectionCategoryBlock>();
		foreach (PvfSection categorySection in categorySections)
		{
			SelectionCategoryBlock? current = null;
			List<int> position = new List<int>(2);
			for (int i = 1; i < categorySection.Children.Count; i++)
			{
				SectionBase child = categorySection.Children[i];
				if (child is PvfSection itemSection)
				{
					current?.ItemSections.Add(itemSection);
					continue;
				}
				if (child.Item?.Type != ScriptType.Int)
				{
					continue;
				}
				position.Add(child.Item.Data);
				if (position.Count == 2)
				{
					current = new SelectionCategoryBlock
					{
						PrimaryIndex = position[0],
						SecondaryIndex = position[1]
					};
					blocks.Add(current);
					position.Clear();
				}
			}
		}
		return blocks;
	}

	private List<string> GetDirectTextValues(PvfSection section)
	{
		List<string> values = new List<string>();
		for (int i = 1; i < section.Children.Count; i++)
		{
			SectionBase child = section.Children[i];
			if (child is PvfSection || child.Item == null || child.Item.Type == ScriptType.Section || child.Item.Type == ScriptType.StringLink)
			{
				continue;
			}
			ScriptItem? linkedString = null;
			if (child.Item.Type == ScriptType.StringLinkIndex && i + 1 < section.Children.Count)
			{
				linkedString = section.Children[++i].Item;
			}
			values.Add(child.Item.GetItemTextNotChar(r3ne9f5cWs, linkedString));
		}
		return values;
	}

	private static List<int> GetDirectIntValues(PvfSection section)
	{
		return section.Children
			.Where(child => child is not PvfSection && child.Item?.Type == ScriptType.Int)
			.Select(child => child.Item.Data)
			.ToList();
	}

	private static bool UsesOneBasedIndexes(IEnumerable<int> values, int count)
	{
		List<int> indexes = values.ToList();
		return indexes.Count > 0 && !indexes.Contains(0) && indexes.All(value => value >= 1 && value <= count);
	}

	private static int NormalizeCategoryIndex(int value, int count, bool oneBased)
	{
		int normalized = oneBased ? value - 1 : value;
		return normalized >= 0 && normalized < count ? normalized : -1;
	}

	private static string GetCategoryName(IReadOnlyList<string> names, int normalizedIndex, int rawIndex, string levelName)
	{
		return normalizedIndex >= 0 && normalizedIndex < names.Count && !string.IsNullOrWhiteSpace(names[normalizedIndex])
			? names[normalizedIndex]
			: $"{levelName}选项 {rawIndex}";
	}

	private static string GetSecondaryCategoryName(IReadOnlyList<string> names, int primaryIndex, int secondaryIndex, int secondaryCount, int rawIndex)
	{
		int flatIndex = primaryIndex >= 0 && secondaryIndex >= 0 ? primaryIndex * secondaryCount + secondaryIndex : -1;
		return flatIndex >= 0 && flatIndex < names.Count && !string.IsNullOrWhiteSpace(names[flatIndex])
			? names[flatIndex]
			: $"二级选项 {rawIndex}";
	}

	private static int GetSelectionItemGroupLength(BoosterType boosterType, int valueCount)
	{
		if (boosterType is BoosterType.Avatar or BoosterType.Special_Avatar)
		{
			return valueCount % 4 == 0 ? 4 : 2;
		}
		return 2;
	}

	public bool GetBoosterInfoBytes()
	{
		return false;
	}

	public bool FindSectionIndex(int sectionId, out int indexOut)
	{
		indexOut = -1;
		for (int i = 2; i < Bleeqw5kG4.DataLen - 4; i += 5)
		{
			if (Bleeqw5kG4.Data[i] == 5 && BitConverter.ToInt32(Bleeqw5kG4.Data, i + 1) == sectionId)
			{
				indexOut = i;
				return true;
			}
		}
		return false;
	}

	public bool GetRecipe(out RecipeViewModel? recipe)
	{
		recipe = null;
		if (!pCned5xYTK())
		{
			return false;
		}
		SectionBase sectionBase = ScriptParserNew.Sections.Where((SectionBase it) => it.GetSectionName() == "[int data]").FirstOrDefault();
		if (sectionBase == null || sectionBase.Children.Count == 0)
		{
			return false;
		}
		ScriptParserNew.GetSectionIntArray(sectionBase, out List<int> arr);
		ResultData<RecipeViewModel> resultData = RecipeViewModel.Create(arr, r3ne9f5cWs);
		if (resultData.IsError)
		{
			AppSetting.Instance.GetIlogger()?.Error("设计图文件：" + Bleeqw5kG4.FileName + " " + resultData.Msg);
			return false;
		}
		recipe = resultData.Data;
		return true;
	}

	public bool GetEnchantCardInfo(out EnchantCardInfo? card)
	{
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		card = null;
		if (!pCned5xYTK())
		{
			return false;
		}
		SectionBase sectionBase = ScriptParserNew.Sections.Where((SectionBase it) => it.GetSectionName() == "[int data]").FirstOrDefault();
		if (sectionBase == null || sectionBase.Children.Count == 0)
		{
			return false;
		}
		ScriptParserNew.GetSectionIntArray(sectionBase, out List<int> arr);
		if (arr.Count == 1)
		{
			arr.Add(0);
		}
		if (arr.Count != 2)
		{
			ilogger.Error("附魔卡片数据错误 [int data] 长度不等于2 file://" + Bleeqw5kG4.FileName);
			return false;
		}
		card = new EnchantCardInfo(Bleeqw5kG4, r3ne9f5cWs, ScriptParserNew);
		card.BackImgIndex = arr[0];
		card.MonsterId = arr[1];
		SectionBase sectionBase2 = ScriptParserNew.Sections.Where((SectionBase it) => it.GetSectionName() == "[string data]").FirstOrDefault();
		if (sectionBase2 == null || sectionBase2.Children.Count == 0)
		{
			ilogger.Error("附魔卡片数据错误 [string data] 长度不能为0");
			return false;
		}
		ScriptParserNew.GetSectionStringArray(sectionBase2, out List<string> arr2);
		if (arr2.Count < 1)
		{
			ilogger.Error("附魔卡片数据错误 [string data] 长度不能为0");
			return false;
		}
		card.BackImgPath = arr2[0];
		if (arr2.Count > 1)
		{
			card.EquipParts = arr2.GetRange(1, arr2.Count - 1);
		}
		return true;
	}

	public bool GetUsable_cera_package(out Usable_cera_package? usable_cera_package)
	{
		AppSetting.Instance.GetIlogger();
		usable_cera_package = null;
		if (!pCned5xYTK())
		{
			return false;
		}
		SectionBase sectionBase = ScriptParserNew.Sections.Where((SectionBase it) => it.GetSectionName() == "[package data]").FirstOrDefault();
		if (sectionBase == null || sectionBase.Children.Count == 0)
		{
			return false;
		}
		ScriptParserNew.GetSectionIntArray(sectionBase, out List<int> arr);
		if (arr == null || !arr.Any())
		{
			return false;
		}
		usable_cera_package = new Usable_cera_package(arr, Bleeqw5kG4, r3ne9f5cWs);
		return true;
	}
}
