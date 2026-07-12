using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.ViewModels.DocumentFolder.PreviewControls;

namespace PvfCode.ViewModels.DocumentFolder;

public enum PvfPreviewTone
{
	Normal,
	Blue,
	Flavor,
	Set,
	Shop,
	Quest,
	Skill,
	Warning
}

public sealed class PvfPreviewTag
{
	private readonly List<string> values = new();

	public string Name { get; }

	public string DisplayName => $"[{Name}]  第 {LineNumber} 行";

	public string TagLabel => $"[{Name}]";

	public int LineNumber { get; }

	public int Offset { get; }

	public int Length { get; }

	public string ValuePreview => NormalizeValue(string.Join(" ", values));

	internal IReadOnlyList<string> Values => values;

	public PvfPreviewTag(string name, int lineNumber, int offset, int length)
	{
		Name = name;
		LineNumber = lineNumber;
		Offset = offset;
		Length = length;
	}

	internal void AddValue(string value)
	{
		if (!string.IsNullOrWhiteSpace(value))
		{
			values.Add(value.Trim());
		}
	}

	private static string NormalizeValue(string value)
	{
		string normalized = Regex.Replace(value, @"\s+", " ").Trim(' ', '\t', '\r', '\n', '`', '\'', '"');
		return normalized.Length > 220 ? normalized.Substring(0, 220) + "..." : normalized;
	}
}

public sealed class PvfPreviewField
{
	public string Label { get; }

	public string Value { get; }

	public PvfPreviewTag Tag { get; }

	public PvfPreviewTone Tone { get; }

	public PvfPreviewField(string label, string value, PvfPreviewTag tag = null, PvfPreviewTone tone = PvfPreviewTone.Normal)
	{
		Label = label;
		Value = value;
		Tag = tag;
		Tone = tone;
	}
}

public sealed class PvfPreviewLine
{
	public string Text { get; }

	public PvfPreviewTag Tag { get; }

	public PvfPreviewLine(string text, PvfPreviewTag tag)
	{
		Text = text;
		Tag = tag;
	}
}

public sealed class PvfPreviewEntry
{
	public int Code { get; }

	public int? Quantity { get; }

	public string Name { get; }

	public string Detail { get; }

	public PvfPreviewTag Tag { get; }

	public string DisplayName => Quantity.HasValue ? $"{Code}  {Name} x{Quantity}" : $"{Code}  {Name}";

	public PvfPreviewEntry(int code, int? quantity, string name, string detail, PvfPreviewTag tag)
	{
		Code = code;
		Quantity = quantity;
		Name = name;
		Detail = detail;
		Tag = tag;
	}
}

public sealed class PvfPreviewTableRow
{
	public IReadOnlyList<string> Cells { get; }

	public PvfPreviewTag Target { get; }

	public PvfPreviewTableRow(IReadOnlyList<string> cells, PvfPreviewTag target)
	{
		Cells = cells;
		Target = target;
	}
}

public sealed class PvfPreviewTable
{
	public string Caption { get; }

	public PvfPreviewTag Tag { get; }

	public List<string> Headers { get; } = new();

	public List<PvfPreviewTableRow> Rows { get; } = new();

	public PvfPreviewTable(string caption, PvfPreviewTag tag)
	{
		Caption = caption;
		Tag = tag;
	}
}

public sealed class PvfPreviewNode
{
	public int Code { get; }

	public string Name { get; }

	public double X { get; }

	public double Y { get; }

	public PvfPreviewTag Tag { get; }

	public PvfPreviewNode(int code, string name, double x, double y, PvfPreviewTag tag)
	{
		Code = code;
		Name = name;
		X = x;
		Y = y;
		Tag = tag;
	}
}

public sealed class PvfPreviewSection
{
	public string Title { get; }

	public PvfPreviewTone Tone { get; }

	public PvfPreviewTag Tag { get; internal set; }

	public List<PvfPreviewField> Fields { get; } = new();

	public List<PvfPreviewLine> Lines { get; } = new();

	public List<PvfPreviewEntry> Entries { get; } = new();

	public List<PvfPreviewTable> Tables { get; } = new();

	public PvfPreviewSection(string title, PvfPreviewTone tone = PvfPreviewTone.Normal, PvfPreviewTag tag = null)
	{
		Title = title;
		Tone = tone;
		Tag = tag;
	}
}

public sealed class PvfRichPreview
{
	public string Title { get; set; }

	public string Subtitle { get; set; }

	public string SourcePath { get; set; }

	public int? ItemCode { get; set; }

	public int? Rarity { get; set; }

	public PvfSkillKind? SkillKind { get; set; }

	public ImageSource Icon { get; set; }

	public string Message { get; set; }

	public List<string> Badges { get; } = new();

	public List<PvfPreviewSection> Sections { get; } = new();

	public List<PvfPreviewNode> SkillTreeNodes { get; } = new();
}

public sealed class PvfPreviewDocument : DocumentBase
{
	private sealed class SkillDataValue
	{
		public string Value { get; init; }

		public PvfPreviewTag Target { get; init; }
	}

	private sealed class SkillDataScene
	{
		public string Key { get; init; }

		public string Label { get; init; }

		public List<List<SkillDataValue>> LevelInfo { get; } = new();

		public List<SkillDataValue> StaticData { get; } = new();

		public List<string> LevelProperty { get; } = new();
	}

	public const string PreviewDocumentPath = "pvf-preview://current";

	private static readonly Regex TagLineRegex = new(
		@"^(?<indent>\s*)\[\s*(?<close>/)?\s*(?<name>[^\]\r\n]+?)\s*\]\s*(?<value>.*)$",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly Regex NumberRegex = new(@"-?\d+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly Regex NumericTokenRegex = new(@"-?\d+(?:\.\d+)?", RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly Regex ValueTokenRegex = new(
		"`(?<backtick>[^`]*)`|\"(?<double>[^\"]*)\"|'(?<single>[^']*)'|(?<plain>[^\\s]+)",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly Regex SkillTreePathRegex = new(
		@"^(clientonly/skilltree/.+_(sp|tp)\.co|clientonly/skillshoptree(sp|tp)index\.co|etc/pvpskilltree/.+\.etc)$",
		RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	private static readonly HashSet<string> BlockValueTags = new(StringComparer.OrdinalIgnoreCase)
	{
		"a condition item", "b condition item", "booster random", "command", "common skill",
		"consume item", "dungeon info", "enemy reward item", "etc", "executable states", "int data",
		"level info", "level property", "material", "monster reward item", "need material", "output",
		"package data", "piece set ability", "pre required skill", "purchase cost", "random list",
		"result item", "reward int data", "reward selection int data", "sell item", "set ability",
		"set item", "skill fitness growtype", "skill fitness second growtype", "skill info", "skill levelup",
		"skill under cooltime effect", "skill under cooltime effect each", "special level up",
		"special purchase cost", "spending item", "static data", "string data", "usable job"
	};

	private static readonly HashSet<string> KnownPreviewTags = new(BlockValueTags, StringComparer.OrdinalIgnoreCase)
	{
		"attach type", "basic explain", "basic explain ex", "cash", "casting time", "command key explain",
		"complete npc index", "consume mp", "cool time", "detail explain", "equipment magical attack",
		"equipment magical defense", "equipment physical attack", "equipment physical defense", "equipment type",
		"explain", "explain ex", "flavor text", "fullset basic explain", "fullset detail explain", "grade",
		"growtype maximum level", "icon", "icon pos", "index", "item group name", "job", "job message",
		"maximum level", "message", "minimum level", "name", "name2", "next skill", "npc", "npc index",
		"parameter basic explain", "parameter detail explain", "price", "rarity", "relation quest",
		"required level", "required level range", "reward type", "set name", "skill class",
		"skill command advantage", "stack limit", "stackable type", "start cool time", "tab name", "type",
		"use effect explain", "value", "weapon effect type", "weight", "durability", "weapon shop",
		"character job", "part set index"
	};

	private static readonly Dictionary<string, string> EquipmentStats = new(StringComparer.OrdinalIgnoreCase)
	{
		["equipment physical attack"] = "物理攻击力", ["equipment magical attack"] = "魔法攻击力",
		["equipment physical defense"] = "物理防御力", ["equipment magical defense"] = "魔法防御力",
		["separate attack"] = "独立攻击力", ["physical attack"] = "力量", ["magical attack"] = "智力",
		["physical defense"] = "体力", ["magical defense"] = "精神"
	};

	private static readonly Dictionary<string, string> EquipmentMagicStats = new(StringComparer.OrdinalIgnoreCase)
	{
		["physical critical hit"] = "物理暴击率", ["magical critical hit"] = "魔法暴击率",
		["attack speed"] = "攻击速度", ["cast speed"] = "施放速度", ["move speed"] = "移动速度",
		["jump power"] = "跳跃力", ["hit recovery"] = "硬直", ["room list move speed rate"] = "城镇移动速度",
		["stuck"] = "命中率", ["stuck resistance"] = "回避率", ["hp max"] = "HP 最大值",
		["mp max"] = "MP 最大值", ["hp regen speed"] = "HP 回复速度", ["mp regen speed"] = "MP 回复速度",
		["all elemental resistance"] = "所有属性抗性", ["all elemental attack"] = "所有属性强化",
		["fire attack"] = "火属性强化", ["water attack"] = "冰属性强化", ["ice attack"] = "冰属性强化",
		["light attack"] = "光属性强化", ["dark attack"] = "暗属性强化", ["inventory limit"] = "负重上限",
		["slow resistance"] = "减速抗性", ["freeze resistance"] = "冰冻抗性", ["poison resistance"] = "中毒抗性",
		["stun resistance"] = "眩晕抗性", ["curse resistance"] = "诅咒抗性", ["blind resistance"] = "失明抗性",
		["lightning resistance"] = "感电抗性", ["stone resistance"] = "石化抗性", ["sleep resistance"] = "睡眠抗性",
		["bleeding resistance"] = "出血抗性", ["confuse resistance"] = "混乱抗性", ["hold resistance"] = "束缚抗性",
		["burn resistance"] = "灼伤抗性", ["weapon break resistance"] = "武器破坏抗性",
		["armor break resistance"] = "防具破坏抗性", ["piercing resistance"] = "贯通抗性"
	};

	private static readonly Dictionary<string, string> JobLabels = new(StringComparer.OrdinalIgnoreCase)
	{
		["all"] = "所有职业", ["swordman"] = "鬼剑士(男)", ["at swordman"] = "鬼剑士(女)",
		["atswordman"] = "鬼剑士(女)", ["fighter"] = "格斗家(女)", ["at fighter"] = "格斗家(男)",
		["atfighter"] = "格斗家(男)", ["gunner"] = "神枪手(男)", ["at gunner"] = "神枪手(女)",
		["atgunner"] = "神枪手(女)", ["mage"] = "魔法师(女)", ["at mage"] = "魔法师(男)",
		["atmage"] = "魔法师(男)", ["priest"] = "圣职者", ["thief"] = "暗夜使者",
		["weaponmaster"] = "剑魂", ["soulbringer"] = "鬼泣", ["berserker"] = "狂战士", ["asura"] = "阿修罗",
		["ranger"] = "漫游枪手", ["launcher"] = "枪炮师", ["mechanic"] = "机械师", ["spitfire"] = "弹药专家",
		["elementalmaster"] = "元素师", ["summoner"] = "召唤师", ["battlemage"] = "战斗法师", ["witch"] = "魔道学者",
		["nenmaster"] = "气功师", ["striker"] = "散打", ["streetfighter"] = "街霸", ["grappler"] = "柔道家",
		["crusader"] = "圣骑士", ["infighter"] = "蓝拳圣使", ["exorcist"] = "驱魔师", ["avenger"] = "复仇者",
		["rogue"] = "刺客", ["necromancer"] = "死灵术士"
	};

	private readonly Dictionary<string, List<PvfPreviewTag>> tagsByName = new(StringComparer.OrdinalIgnoreCase);
	private TextDocument sourceTextDocument;
	private PvfFileDocument sourceDocument;
	private TextEditorPreviewViewModelAni aniPreviewViewModel;
	private PvfRichPreview richPreview;
	private string aniPreviewStatus;
	private PvfPreviewTag selectedTag;
	private int aniLoadAttempts;
	private bool refreshQueued;

	public ObservableCollection<PvfPreviewTag> Tags { get; } = new();

	public PvfFileDocument SourceDocument => sourceDocument;

	public PvfRichPreview RichPreview
	{
		get => richPreview;
		private set
		{
			richPreview = value;
			RaisePropertyChanged(nameof(RichPreview));
		}
	}

	public TextEditorPreviewViewModelAni AniPreviewViewModel
	{
		get => aniPreviewViewModel;
		private set
		{
			aniPreviewViewModel = value;
			RaisePropertyChanged(nameof(AniPreviewViewModel));
		}
	}

	public bool IsAniPreview => sourceDocument?.File?.FileType == PvfFileType.ani;

	public string AniPreviewStatus
	{
		get => aniPreviewStatus;
		private set
		{
			aniPreviewStatus = value;
			RaisePropertyChanged(nameof(AniPreviewStatus));
		}
	}

	public PvfPreviewTag SelectedTag
	{
		get => selectedTag;
		private set
		{
			selectedTag = value;
			RaisePropertyChanged(nameof(SelectedTag));
		}
	}

	public string SourcePath => sourceDocument?.FullPath ?? string.Empty;

	public override string FileName => sourceDocument == null ? "预览" : $"预览: {sourceDocument.FileName}";

	public PvfPreviewDocument(PvfFileDocument source)
		: base(PreviewDocumentPath)
	{
		DocumentType = PvfFileDocumentType.预览;
		SetSource(source);
	}

	public static bool Supports(PvfFile file)
	{
		if (file == null || (file.FileType != PvfFileType.ani && !file.IsScriptFile))
		{
			return false;
		}
		return file.FileType switch
		{
			PvfFileType.ani or PvfFileType.als or PvfFileType.equ or PvfFileType.stk or
			PvfFileType.shp or PvfFileType.qst or PvfFileType.skl => true,
			PvfFileType.co or PvfFileType.etc => SkillTreePathRegex.IsMatch(file.FileName.Replace('\\', '/')),
			_ => false
		};
	}

	public void SetSource(PvfFileDocument source)
	{
		if (source == null || !Supports(source.File) || ReferenceEquals(sourceDocument, source))
		{
			return;
		}
		UnsubscribeSource();
		sourceDocument = source;
		sourceTextDocument = source.Document;
		Icon = source.Icon;
		source.PreviewContentChanged += OnSourcePreviewContentChanged;
		if (sourceTextDocument != null)
		{
			sourceTextDocument.TextChanged += OnSourceTextChanged;
		}
		RaisePropertyChanged(nameof(SourceDocument));
		RaisePropertyChanged(nameof(SourcePath));
		RaisePropertyChanged(nameof(FileName));
		RaisePropertyChanged(nameof(IsAniPreview));
		RefreshPreview();
	}

	public void RefreshPreview()
	{
		refreshQueued = false;
		if (sourceDocument?.File == null)
		{
			return;
		}
		ParseTags(sourceTextDocument?.Text ?? string.Empty);
		RichPreview = BuildRichPreview(sourceDocument.File);
		if (IsAniPreview)
		{
			aniLoadAttempts = 0;
			LoadAniPreview();
		}
		else
		{
			AniPreviewViewModel = null;
			AniPreviewStatus = string.Empty;
		}
	}

	public void JumpToTag(PvfPreviewTag tag)
	{
		if (tag != null)
		{
			SelectedTag = tag;
			sourceDocument?.NavigateToTag(tag.Offset, tag.Length);
		}
	}

	private void ParseTags(string text)
	{
		Tags.Clear();
		tagsByName.Clear();
		PvfPreviewTag current = null;
		int offset = 0;
		string[] lines = Regex.Split(text, "(?<=\\n)");
		foreach (string rawWithEnding in lines)
		{
			string rawLine = rawWithEnding.TrimEnd('\r', '\n');
			string trimmed = rawLine.Trim();
			Match match = TagLineRegex.Match(rawLine);
			if (match.Success)
			{
				if (match.Groups["close"].Success)
				{
					current = null;
					offset += rawWithEnding.Length;
					continue;
				}
				string name = match.Groups["name"].Value.Trim().ToLowerInvariant();
				Group nameGroup = match.Groups["name"];
				PvfPreviewTag occurrence = new(name, GetLineNumber(offset), offset + nameGroup.Index, nameGroup.Length);
				Tags.Add(occurrence);
				if (!tagsByName.TryGetValue(name, out List<PvfPreviewTag> occurrences))
				{
					occurrences = new List<PvfPreviewTag>();
					tagsByName[name] = occurrences;
				}
				occurrences.Add(occurrence);
				bool isTopLevel = match.Groups["indent"].Length == 0;
				if (current != null && ShouldKeepTagLineInCurrent(current.Name, name, isTopLevel))
				{
					current.AddValue(trimmed);
				}
				else
				{
					current = occurrence;
					current.AddValue(match.Groups["value"].Value);
				}
			}
			else if (current != null && trimmed.Length > 0 && !trimmed.StartsWith("//", StringComparison.Ordinal))
			{
				current.AddValue(trimmed);
			}
			offset += rawWithEnding.Length;
		}
		SelectedTag = Tags.FirstOrDefault();
	}

	private int GetLineNumber(int offset)
	{
		if (sourceTextDocument == null || sourceTextDocument.TextLength == 0)
		{
			return 1;
		}
		return sourceTextDocument.GetLineByOffset(Math.Min(offset, sourceTextDocument.TextLength - 1)).LineNumber;
	}

	private static bool ShouldKeepTagLineInCurrent(string current, string nextName, bool isTopLevel)
	{
		return !isTopLevel && (BlockValueTags.Contains(current) || !KnownPreviewTags.Contains(nextName));
	}

	private PvfRichPreview BuildRichPreview(PvfFile file)
	{
		PvfRichPreview preview = new()
		{
			Title = FirstText("name") ?? FirstText("name2") ?? FirstText("set name") ?? Path.GetFileNameWithoutExtension(file.ShortName),
			Subtitle = GetPreviewKind(file),
			SourcePath = file.FileName,
			ItemCode = file.ItemCode,
			Rarity = FirstNumber("rarity"),
			Icon = sourceDocument.Icon
		};
		preview.Badges.Add(file.Extension.TrimStart('.').ToUpperInvariant());
		if (preview.Rarity.HasValue)
		{
			preview.Badges.Add($"稀有度 {preview.Rarity.Value}");
		}
		switch (file.FileType)
		{
			case PvfFileType.equ:
				if (HasAnyTag("set name", "set item", "set ability", "piece set ability", "fullset basic explain"))
				{
					BuildEquipmentSet(preview);
				}
				else
				{
					BuildEquipment(preview);
				}
				break;
			case PvfFileType.stk:
				BuildStackable(preview);
				break;
			case PvfFileType.shp:
				BuildShop(preview);
				break;
			case PvfFileType.qst:
				BuildQuest(preview);
				break;
			case PvfFileType.skl:
				BuildSkill(preview);
				break;
			case PvfFileType.ani:
				BuildAni(preview);
				break;
			case PvfFileType.als:
				BuildAls(preview);
				break;
			case PvfFileType.co:
			case PvfFileType.etc:
				BuildSkillTree(preview);
				break;
		}
		if (preview.Sections.Count == 0 && preview.SkillTreeNodes.Count == 0)
		{
			BuildGeneric(preview);
		}
		return preview;
	}

	private void BuildEquipment(PvfRichPreview preview)
	{
		PvfPreviewSection info = AddSection(preview, "装备信息", PvfPreviewTone.Normal, "equipment type");
		AddCode(info, "道具ID", preview.ItemCode);
		AddField(info, "类型", LabelToken(FirstText("equipment type")), "equipment type");
		AddField(info, "物品组", FirstText("item group name"), "item group name");
		AddField(info, "等级限制", LevelText(FirstNumber("minimum level")), "minimum level");
		AddField(info, "耐久度", NumberText(FirstNumber("durability")), "durability");
		AddField(info, "重量", WeightText(FirstNumber("weight")), "weight");
		AddField(info, "交易", TradeText(FirstText("attach type")), "attach type");
		AddField(info, "出售价格", PriceText(FirstNumber("value"), 5), "value");
		AddField(info, "价格", PriceText(FirstNumber("price")), "price");
		RemoveEmpty(preview, info);

		List<string> jobs = TagLines("usable job").Select(LabelToken).Where(value => !string.IsNullOrEmpty(value))
			.Select(value => JobLabels.TryGetValue(value, out string label) ? label : value).ToList();
		if (jobs.Count > 0)
		{
			PvfPreviewSection section = AddSection(preview, "可使用职业", PvfPreviewTone.Normal, "usable job");
			section.Lines.Add(new PvfPreviewLine(string.Join("、", jobs), FindTag("usable job")));
		}
		AddStatSection(preview, "基础属性", EquipmentStats, false, PvfPreviewTone.Normal);
		AddStatSection(preview, "特殊属性", EquipmentMagicStats, true, PvfPreviewTone.Blue);
		AddEntrySection(preview, "材料/条件", PvfPreviewTone.Normal, false, "need material", "material", "condition item", "a condition item", "b condition item");
		AddTextSection(preview, "装备说明", PvfPreviewTone.Blue, "basic explain", "detail explain", "explain");
		AddTextSection(preview, "风味文本", PvfPreviewTone.Flavor, "flavor text");
	}

	private void BuildEquipmentSet(PvfRichPreview preview)
	{
		preview.Subtitle = "装备套装";
		AddEntrySection(preview, "套装部件", PvfPreviewTone.Set, true, "set item");
		AddTextSection(preview, "套装属性", PvfPreviewTone.Set, "set ability");
		AddTextSection(preview, "件数属性", PvfPreviewTone.Set, "piece set ability");
		AddTextSection(preview, "全套说明", PvfPreviewTone.Blue, "fullset basic explain", "fullset detail explain");
		AddTextSection(preview, "参数说明", PvfPreviewTone.Blue, "parameter basic explain", "parameter detail explain");
	}

	private void BuildStackable(PvfRichPreview preview)
	{
		PvfPreviewSection info = AddSection(preview, "道具信息", PvfPreviewTone.Normal, "stackable type");
		AddCode(info, "ID", preview.ItemCode);
		AddField(info, "类型", LabelToken(FirstText("stackable type")), "stackable type");
		AddField(info, "堆叠上限", NumberText(FirstNumber("stack limit")), "stack limit");
		AddField(info, "等级限制", LevelText(FirstNumber("minimum level")), "minimum level");
		AddField(info, "交易", TradeText(FirstText("attach type")), "attach type");
		AddField(info, "出售价格", PriceText(FirstNumber("value"), 5), "value");
		AddField(info, "价格", PriceText(FirstNumber("price")), "price");
		RemoveEmpty(preview, info);
		AddTextSection(preview, "道具说明", PvfPreviewTone.Blue, "explain", "basic explain", "detail explain", "use effect explain");
		AddEntrySection(preview, "礼包内容", PvfPreviewTone.Shop, true, "package data");
		AddEntrySection(preview, "随机/产出内容", PvfPreviewTone.Shop, false, "random list", "booster random", "etc", "output", "result item");
		AddEntrySection(preview, "材料/条件", PvfPreviewTone.Normal, false, "need material", "material", "condition item", "a condition item", "b condition item");
		AddTextSection(preview, "附魔/特殊数据", PvfPreviewTone.Blue, "enchant", "monster card id", "string data", "stat change", "stat change duration");
		AddTextSection(preview, "风味文本", PvfPreviewTone.Flavor, "flavor text");
	}

	private void BuildShop(PvfRichPreview preview)
	{
		PvfPreviewSection info = AddSection(preview, "商店信息", PvfPreviewTone.Shop, "message");
		AddCode(info, "代码", preview.ItemCode);
		AddField(info, "消息", FirstText("message"), "message");
		AddField(info, "商店类型", HasAnyTag("weapon shop") ? "武器商店" : null, "weapon shop");
		AddField(info, "NPC", FirstText("npc"), "npc");
		RemoveEmpty(preview, info);
		AddTextSection(preview, "标签页", PvfPreviewTone.Shop, "tab name");
		AddEntrySection(preview, "出售商品", PvfPreviewTone.Shop, false, "sell item");
		AddEntrySection(preview, "兑换/消耗材料", PvfPreviewTone.Shop, false, "spending item", "need material");
	}

	private void BuildQuest(PvfRichPreview preview)
	{
		PvfPreviewSection info = AddSection(preview, "任务信息", PvfPreviewTone.Quest, "type");
		AddCode(info, "代码", preview.ItemCode);
		AddField(info, "类型", LabelToken(FirstText("type")), "type");
		AddField(info, "奖励类型", LabelToken(FirstText("reward type")), "reward type");
		AddField(info, "任务品级", LabelToken(FirstText("grade")), "grade");
		AddField(info, "职业", LabelToken(FirstText("job")), "job");
		AddField(info, "接取 NPC", NumberText(FirstNumber("npc index")), "npc index");
		AddField(info, "完成 NPC", NumberText(FirstNumber("complete npc index")), "complete npc index");
		AddField(info, "前置任务", NumberText(FirstNumber("pre required quest")), "pre required quest");
		AddField(info, "关联任务", NumberText(FirstNumber("relation quest")), "relation quest");
		RemoveEmpty(preview, info);
		AddTextSection(preview, "任务说明", PvfPreviewTone.Quest, "explain", "basic explain", "detail explain", "depend message", "job message");
		AddTextSection(preview, "完成条件", PvfPreviewTone.Quest, "int data", "dungeon info", "monster reward item", "enemy reward item", "clear reward item");
		AddEntrySection(preview, "任务奖励", PvfPreviewTone.Quest, true, "reward int data", "reward selection int data");
	}

	private void BuildSkill(PvfRichPreview preview)
	{
		preview.SkillKind = PvfSkillClassifier.GetKind(AppCore.ViewModelBase.PVF, sourceDocument.File, preview.Title);
		if (preview.SkillKind.HasValue)
		{
			preview.Badges.Add($"{PvfSkillClassifier.GetLabel(preview.SkillKind.Value)}技能");
		}
		PvfPreviewSection info = AddSection(preview, "技能信息", PvfPreviewTone.Skill, "type");
		AddCode(info, "代码", preview.ItemCode);
		AddField(info, "类型", preview.SkillKind.HasValue ? PvfSkillClassifier.GetLabel(preview.SkillKind.Value) : LabelToken(FirstText("type")), "type");
		AddField(info, "技能类", FirstText("skill class"), "skill class");
		AddField(info, "学习等级", LevelText(FirstNumber("required level")), "required level");
		AddField(info, "等级间隔", NumberText(FirstNumber("required level range")), "required level range");
		AddField(info, "最高等级", NumberText(FirstNumber("maximum level")), "maximum level");
		AddField(info, "冷却时间", TimeText(FirstNumber("cool time"), 1000), "cool time");
		AddField(info, "开始冷却", TimeText(FirstNumber("start cool time"), 1000), "start cool time");
		AddField(info, "MP 消耗", RangeText(NumbersFromLines(TagLines("consume mp"))), "consume mp");
		AddField(info, "施法时间", TimeText(FirstNumber("casting time"), 100), "casting time");
		AddField(info, "伤害类型", LabelToken(FirstText("weapon effect type")), "weapon effect type");
		RemoveEmpty(preview, info);
		AddTextSection(preview, "技能说明", PvfPreviewTone.Skill, "basic explain", "explain", "basic explain ex", "explain ex");
		AddTextSection(preview, "技能属性", PvfPreviewTone.Blue, "level property", "special level up", "static data");
		AddTextSection(preview, "前置/消耗", PvfPreviewTone.Skill, "pre required skill", "consume item", "purchase cost", "special purchase cost");
		AddTextSection(preview, "指令", PvfPreviewTone.Skill, "command", "command key explain", "skill command advantage");
		AddSkillDataTables(preview, sourceTextDocument?.Text ?? string.Empty);
		AddTextSection(preview, "特殊效果", PvfPreviewTone.Blue, "skill under cooltime effect", "skill under cooltime effect each");
	}

	private void AddSkillDataTables(PvfRichPreview preview, string text)
	{
		List<SkillDataScene> scenes = ParseSkillDataScenes(text);
		PvfPreviewSection section = new("动态/静态数据", PvfPreviewTone.Blue, FindTag("level info") ?? FindTag("static data"));
		foreach (SkillDataScene scene in scenes)
		{
			(Dictionary<int, string> levelLabels, Dictionary<int, string> staticLabels) = BuildSkillDataLabels(scene.LevelProperty);
			List<List<SkillDataValue>> levelRows = NormalizeLevelInfoRows(scene.LevelInfo);
			if (levelRows.Count > 0)
			{
				PvfPreviewTable table = new($"{scene.Label} - [level info]", FindTag("level info"));
				table.Headers.Add("等级");
				int columnCount = levelRows.Max(row => row.Count);
				for (int column = 0; column < columnCount; column++)
				{
					table.Headers.Add(levelLabels.TryGetValue(column, out string label) ? label : $"动态#{column}");
				}
				for (int rowIndex = 0; rowIndex < levelRows.Count; rowIndex++)
				{
					List<SkillDataValue> row = levelRows[rowIndex];
					List<string> cells = new() { $"Lv.{rowIndex + 1}" };
					for (int column = 0; column < row.Count; column++)
					{
						SkillDataValue previous = rowIndex > 0 && column < levelRows[rowIndex - 1].Count ? levelRows[rowIndex - 1][column] : null;
						cells.Add(FormatLevelInfoCell(row[column].Value, previous?.Value));
					}
					table.Rows.Add(new PvfPreviewTableRow(cells, row.FirstOrDefault()?.Target));
				}
				section.Tables.Add(table);
			}
			if (scene.StaticData.Count > 0)
			{
				PvfPreviewTable table = new($"{scene.Label} - [static data]", FindTag("static data"));
				table.Headers.AddRange(new[] { "索引", "含义", "值" });
				for (int index = 0; index < scene.StaticData.Count; index++)
				{
					SkillDataValue value = scene.StaticData[index];
					string label = staticLabels.TryGetValue(index, out string knownLabel) ? knownLabel : $"静态#{index}";
					table.Rows.Add(new PvfPreviewTableRow(new[] { index.ToString(CultureInfo.InvariantCulture), label, value.Value }, value.Target));
				}
				section.Tables.Add(table);
			}
		}
		if (section.Tables.Count > 0)
		{
			preview.Sections.Add(section);
		}
	}

	private List<SkillDataScene> ParseSkillDataScenes(string text)
	{
		Dictionary<string, SkillDataScene> scenes = new(StringComparer.OrdinalIgnoreCase);
		SkillDataScene GetScene(string key)
		{
			string normalized = string.IsNullOrEmpty(key) ? "default" : key;
			if (!scenes.TryGetValue(normalized, out SkillDataScene scene))
			{
				scene = new SkillDataScene { Key = normalized, Label = SkillSceneLabel(normalized) };
				scenes[normalized] = scene;
			}
			return scene;
		}

		GetScene("default");
		List<string> sceneStack = new() { "default" };
		string currentBlock = string.Empty;
		bool inBlockComment = false;
		int lineNumber = 1;
		int lineStartOffset = 0;
		foreach (string rawWithEnding in Regex.Split(text, "(?<=\\n)"))
		{
			string rawLine = rawWithEnding.TrimEnd('\r', '\n');
			string lineText = RemoveBlockComments(rawLine, ref inBlockComment);
			string trimmed = StripLineComment(lineText).Trim();
			if (trimmed.Length > 0)
			{
				Match tag = TagLineRegex.Match(lineText);
				if (tag.Success)
				{
					string name = tag.Groups["name"].Value.Trim().ToLowerInvariant();
					if (tag.Groups["close"].Success)
					{
						if (currentBlock.Equals(name, StringComparison.OrdinalIgnoreCase))
						{
							currentBlock = string.Empty;
						}
						if (IsSkillScene(name) && sceneStack.Count > 1)
						{
							sceneStack.RemoveAt(sceneStack.Count - 1);
						}
					}
					else if (IsSkillScene(name))
					{
						sceneStack.Add(name);
						GetScene(name);
						currentBlock = string.Empty;
					}
					else if (name is "level info" or "static data" or "level property")
					{
						currentBlock = name;
						string inline = StripLineComment(tag.Groups["value"].Value).Trim();
						if (inline.Length > 0)
						{
							AppendSkillDataLine(GetScene(sceneStack[^1]), currentBlock, inline, lineStartOffset + tag.Groups["value"].Index, lineNumber);
						}
					}
					else if (currentBlock.Length > 0)
					{
						AppendSkillDataLine(GetScene(sceneStack[^1]), currentBlock, trimmed, lineStartOffset + Math.Max(0, lineText.IndexOf(trimmed, StringComparison.Ordinal)), lineNumber);
					}
				}
				else if (currentBlock.Length > 0)
				{
					AppendSkillDataLine(GetScene(sceneStack[^1]), currentBlock, trimmed, lineStartOffset + Math.Max(0, lineText.IndexOf(trimmed, StringComparison.Ordinal)), lineNumber);
				}
			}
			lineStartOffset += rawWithEnding.Length;
			lineNumber++;
		}
		return scenes.Values.Where(scene => scene.LevelInfo.Count > 0 || scene.StaticData.Count > 0).ToList();
	}

	private static string RemoveBlockComments(string line, ref bool inBlockComment)
	{
		string result = line;
		if (inBlockComment)
		{
			int end = result.IndexOf("*/", StringComparison.Ordinal);
			if (end < 0)
			{
				return string.Empty;
			}
			result = result.Substring(end + 2);
			inBlockComment = false;
		}
		while (true)
		{
			int start = result.IndexOf("/*", StringComparison.Ordinal);
			if (start < 0)
			{
				return result;
			}
			int end = result.IndexOf("*/", start + 2, StringComparison.Ordinal);
			if (end < 0)
			{
				inBlockComment = true;
				return result.Substring(0, start);
			}
			result = result.Substring(0, start) + result.Substring(end + 2);
		}
	}

	private void AppendSkillDataLine(SkillDataScene scene, string block, string line, int absoluteOffset, int lineNumber)
	{
		if (block == "level property")
		{
			scene.LevelProperty.Add(line);
			return;
		}
		List<SkillDataValue> values = ParseSkillDataValues(line, absoluteOffset, lineNumber, block);
		if (values.Count == 0)
		{
			return;
		}
		if (block == "level info")
		{
			scene.LevelInfo.Add(values);
		}
		else if (block == "static data")
		{
			scene.StaticData.AddRange(values);
		}
	}

	private static List<SkillDataValue> ParseSkillDataValues(string line, int absoluteOffset, int lineNumber, string tagName)
	{
		List<SkillDataValue> values = new();
		foreach (Match match in ValueTokenRegex.Matches(line))
		{
			string value = match.Groups["backtick"].Success ? match.Groups["backtick"].Value :
				match.Groups["double"].Success ? match.Groups["double"].Value :
				match.Groups["single"].Success ? match.Groups["single"].Value : match.Groups["plain"].Value;
			if (value.Length == 0)
			{
				continue;
			}
			PvfPreviewTag target = new(tagName, lineNumber, absoluteOffset + match.Index, Math.Max(1, match.Length));
			target.AddValue(value);
			values.Add(new SkillDataValue { Value = value, Target = target });
		}
		return values;
	}

	private static List<List<SkillDataValue>> NormalizeLevelInfoRows(List<List<SkillDataValue>> rawRows)
	{
		if (rawRows.Count == 0 || rawRows[0].Count == 0 || !int.TryParse(rawRows[0][0].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int columnCount) || columnCount <= 0)
		{
			return rawRows;
		}
		List<SkillDataValue> packed = rawRows[0].Skip(1).Concat(rawRows.Skip(1).SelectMany(row => row)).ToList();
		List<List<SkillDataValue>> rows = new();
		for (int index = 0; index < packed.Count; index += columnCount)
		{
			rows.Add(packed.Skip(index).Take(columnCount).ToList());
		}
		return rows;
	}

	private static (Dictionary<int, string> Level, Dictionary<int, string> Static) BuildSkillDataLabels(IEnumerable<string> lines)
	{
		Dictionary<int, string> level = new();
		Dictionary<int, string> stat = new();
		string pendingLabel = string.Empty;
		foreach (string line in lines)
		{
			Match text = Regex.Match(line, @"`([^`]*)`");
			if (text.Success)
			{
				pendingLabel = Regex.Replace(text.Groups[1].Value, @"<[^>]+>", string.Empty).Trim(' ', ':', '：');
			}
			List<double> numbers = NumericTokenRegex.Matches(line).Select(match => double.Parse(match.Value, CultureInfo.InvariantCulture)).ToList();
			for (int index = 0; index + 2 < numbers.Count; index += 3)
			{
				double source = numbers[index];
				double target = numbers[index + 1];
				if (target < 0 || target != Math.Truncate(target))
				{
					continue;
				}
				string label = string.IsNullOrEmpty(pendingLabel) ? (source < 0 ? $"动态#{(int)target}" : $"静态#{(int)source}") : pendingLabel;
				if (source < 0)
				{
					level[(int)target] = label;
				}
				else if (source == Math.Truncate(source))
				{
					stat[(int)source] = label;
					stat.TryAdd((int)target, label);
				}
			}
		}
		return (level, stat);
	}

	private static string FormatLevelInfoCell(string current, string previous)
	{
		if (previous == null || !double.TryParse(current, NumberStyles.Float, CultureInfo.InvariantCulture, out double currentValue) ||
			!double.TryParse(previous, NumberStyles.Float, CultureInfo.InvariantCulture, out double previousValue))
		{
			return current;
		}
		double delta = currentValue - previousValue;
		string deltaText = delta >= 0 ? $"+{delta:0.##}" : delta.ToString("0.##", CultureInfo.InvariantCulture);
		if (previousValue == 0)
		{
			return $"{current}（{deltaText}）";
		}
		double percent = delta / previousValue * 100;
		string percentText = percent >= 0 ? $"+{percent:0.##}%" : $"{percent:0.##}%";
		return $"{current}（{deltaText}，{percentText}）";
	}

	private static bool IsSkillScene(string name) => name is "dungeon" or "pvp" or "death tower" or "warroom";

	private static string SkillSceneLabel(string key)
	{
		return key switch
		{
			"default" => "默认/通用",
			"dungeon" => "地下城",
			"pvp" => "决斗场",
			"death tower" => "死亡之塔",
			"warroom" => "战争房间",
			_ => key
		};
	}

	private static string StripLineComment(string value)
	{
		int index = value.IndexOf("//", StringComparison.Ordinal);
		return index >= 0 ? value.Substring(0, index) : value;
	}

	private void BuildAni(PvfRichPreview preview)
	{
		preview.Badges.Add("画面预览");
		int frameCount = Tags.Count(tag => tag.Name.StartsWith("frame", StringComparison.OrdinalIgnoreCase));
		List<string> imageReferences = Tags.Where(tag => tag.Name.Equals("image", StringComparison.OrdinalIgnoreCase) || tag.Name.Equals("img", StringComparison.OrdinalIgnoreCase))
			.SelectMany(tag => tag.Values).Select(CleanValue).Where(value => !string.IsNullOrEmpty(value)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
		PvfPreviewSection info = AddSection(preview, "动画信息", PvfPreviewTone.Normal, Tags.FirstOrDefault()?.Name);
		info.Fields.Add(new PvfPreviewField("帧标签", frameCount.ToString(CultureInfo.InvariantCulture), Tags.FirstOrDefault(tag => tag.Name.StartsWith("frame", StringComparison.OrdinalIgnoreCase))));
		info.Fields.Add(new PvfPreviewField("图片引用", imageReferences.Count.ToString(CultureInfo.InvariantCulture), FindTag("image") ?? FindTag("img")));
		foreach (string reference in imageReferences.Take(12))
		{
			info.Lines.Add(new PvfPreviewLine(reference, FindTag("image") ?? FindTag("img")));
		}
	}

	private void BuildAls(PvfRichPreview preview)
	{
		preview.Badges.Add("动画分层");
		AddTextSection(preview, "动画引用", PvfPreviewTone.Blue, "ani", "animation", "path", "filename");
		AddTextSection(preview, "图层/应用范围", PvfPreviewTone.Normal, "layer", "use", "scope", "apply");
		if (preview.Sections.Count == 0)
		{
			BuildGeneric(preview);
		}
	}

	private void BuildSkillTree(PvfRichPreview preview)
	{
		preview.Badges.Add(preview.SourcePath.EndsWith("_tp.co", StringComparison.OrdinalIgnoreCase) ? "TP 技能树" : "SP 技能树");
		PvfPreviewSection info = AddSection(preview, "技能树信息", PvfPreviewTone.Skill, "character job");
		string job = LabelToken(FirstText("character job"));
		AddField(info, "职业", JobLabels.TryGetValue(job ?? string.Empty, out string jobLabel) ? jobLabel : job, "character job");
		AddField(info, "节点数", tagsByName.TryGetValue("skill info", out List<PvfPreviewTag> skillTags) ? skillTags.Count.ToString(CultureInfo.InvariantCulture) : "0", "skill info");
		RemoveEmpty(preview, info);
		if (skillTags == null)
		{
			preview.Message = "没有解析到可绘制的技能树节点。";
			return;
		}
		foreach (PvfPreviewTag tag in skillTags)
		{
			string block = string.Join(" ", tag.Values);
			int? code = ExtractTaggedNumber(block, "index");
			Match pos = Regex.Match(block, @"\[\s*icon pos\s*\]\s*(-?\d+)\s+(-?\d+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			if (!code.HasValue || !pos.Success)
			{
				continue;
			}
			double x = double.Parse(pos.Groups[1].Value, CultureInfo.InvariantCulture);
			double y = double.Parse(pos.Groups[2].Value, CultureInfo.InvariantCulture);
			preview.SkillTreeNodes.Add(new PvfPreviewNode(code.Value, ResolveItemName(code.Value), x, y, tag));
		}
		if (preview.SkillTreeNodes.Count == 0)
		{
			preview.Message = "已找到技能树 TAG，但没有解析到带 [index] 与 [icon pos] 的节点。";
		}
	}

	private void BuildGeneric(PvfRichPreview preview)
	{
		PvfPreviewSection section = AddSection(preview, "结构化内容", PvfPreviewTone.Normal, Tags.FirstOrDefault()?.Name);
		foreach (PvfPreviewTag tag in Tags.Take(80))
		{
			string value = tag.ValuePreview;
			if (!string.IsNullOrEmpty(value))
			{
				section.Fields.Add(new PvfPreviewField(tag.TagLabel, value, tag));
			}
		}
		if (section.Fields.Count == 0)
		{
			preview.Message = "没有解析到可显示的 TAG 内容。";
		}
	}

	private void AddStatSection(PvfRichPreview preview, string title, Dictionary<string, string> labels, bool signed, PvfPreviewTone tone)
	{
		PvfPreviewSection section = new(title, tone);
		foreach ((string tagName, string label) in labels)
		{
			int? value = FirstNumber(tagName);
			if (value.HasValue)
			{
				section.Fields.Add(new PvfPreviewField(label, value.Value > 0 || !signed ? $"+{value.Value}" : value.Value.ToString(CultureInfo.InvariantCulture), FindTag(tagName), signed ? PvfPreviewTone.Blue : PvfPreviewTone.Normal));
				section.Tag ??= FindTag(tagName);
			}
		}
		if (section.Fields.Count > 0)
		{
			preview.Sections.Add(section);
		}
	}

	private void AddTextSection(PvfRichPreview preview, string title, PvfPreviewTone tone, params string[] tagNames)
	{
		PvfPreviewSection section = new(title, tone, tagNames.Select(FindTag).FirstOrDefault(tag => tag != null));
		foreach (string tagName in tagNames)
		{
			PvfPreviewTag tag = FindTag(tagName);
			foreach (string line in TagLines(tagName).SelectMany(value => value.Replace("\\n", "\n").Split('\n')).Select(value => value.Trim()).Where(value => value.Length > 0).Take(80))
			{
				section.Lines.Add(new PvfPreviewLine(line, tag));
			}
		}
		if (section.Lines.Count > 0)
		{
			preview.Sections.Add(section);
		}
	}

	private void AddEntrySection(PvfRichPreview preview, string title, PvfPreviewTone tone, bool paired, params string[] tagNames)
	{
		PvfPreviewSection section = new(title, tone, tagNames.Select(FindTag).FirstOrDefault(tag => tag != null));
		foreach (string tagName in tagNames)
		{
			PvfPreviewTag tag = FindTag(tagName);
			foreach (string line in TagLines(tagName))
			{
				List<int> numbers = NumbersFromLines(new[] { line });
				if (paired)
				{
					for (int index = 0; index < numbers.Count; index += 2)
					{
						AddEntry(section, numbers[index], index + 1 < numbers.Count ? numbers[index + 1] : null, line, tag);
					}
				}
				else
				{
					int code = numbers.FirstOrDefault(value => value >= 0);
					if (numbers.Count > 0 && code >= 0)
					{
						int codeIndex = numbers.IndexOf(code);
						int? quantity = codeIndex + 1 < numbers.Count ? numbers[codeIndex + 1] : null;
						AddEntry(section, code, quantity, line, tag);
					}
				}
			}
		}
		if (section.Entries.Count > 0)
		{
			preview.Sections.Add(section);
		}
	}

	private void AddEntry(PvfPreviewSection section, int code, int? quantity, string detail, PvfPreviewTag tag)
	{
		if (code < 0 || section.Entries.Any(entry => entry.Code == code && entry.Quantity == quantity))
		{
			return;
		}
		section.Entries.Add(new PvfPreviewEntry(code, quantity, ResolveItemName(code), CleanValue(detail), tag));
	}

	private string ResolveItemName(int code)
	{
		try
		{
			var pvf = AppCore.ViewModelBase.PVF;
			string path = pvf?.ListFileTable?.ItemCodeConvertFilePath(code);
			PvfFile file = string.IsNullOrEmpty(path) ? null : pvf.GetFile(path);
			return pvf?.GetItemName(file) ?? (string.IsNullOrEmpty(path) ? "未解析" : Path.GetFileNameWithoutExtension(path));
		}
		catch
		{
			return "未解析";
		}
	}

	private PvfPreviewSection AddSection(PvfRichPreview preview, string title, PvfPreviewTone tone, string tagName)
	{
		PvfPreviewSection section = new(title, tone, FindTag(tagName));
		preview.Sections.Add(section);
		return section;
	}

	private static void AddCode(PvfPreviewSection section, string label, int? code)
	{
		if (code.HasValue)
		{
			section.Fields.Add(new PvfPreviewField(label, code.Value.ToString(CultureInfo.InvariantCulture)));
		}
	}

	private void AddField(PvfPreviewSection section, string label, string value, string tagName, PvfPreviewTone tone = PvfPreviewTone.Normal)
	{
		if (!string.IsNullOrWhiteSpace(value))
		{
			section.Fields.Add(new PvfPreviewField(label, value, FindTag(tagName), tone));
		}
	}

	private static void RemoveEmpty(PvfRichPreview preview, PvfPreviewSection section)
	{
		if (section.Fields.Count == 0 && section.Lines.Count == 0 && section.Entries.Count == 0 && section.Tables.Count == 0)
		{
			preview.Sections.Remove(section);
		}
	}

	private bool HasAnyTag(params string[] names) => names.Any(name => tagsByName.ContainsKey(name));

	private PvfPreviewTag FindTag(string name)
	{
		return name != null && tagsByName.TryGetValue(name, out List<PvfPreviewTag> occurrences) ? occurrences.FirstOrDefault() : null;
	}

	private List<string> TagLines(params string[] names)
	{
		List<string> values = new();
		foreach (string name in names)
		{
			if (tagsByName.TryGetValue(name, out List<PvfPreviewTag> occurrences))
			{
				values.AddRange(occurrences.SelectMany(tag => tag.Values).Select(CleanValue).Where(value => !string.IsNullOrEmpty(value)));
			}
		}
		return values;
	}

	private string FirstText(string name) => TagLines(name).FirstOrDefault();

	private int? FirstNumber(string name)
	{
		List<int> values = NumbersFromLines(TagLines(name));
		return values.Count > 0 ? values[0] : null;
	}

	private static List<int> NumbersFromLines(IEnumerable<string> lines)
	{
		List<int> values = new();
		foreach (string line in lines)
		{
			foreach (Match match in NumberRegex.Matches(line))
			{
				if (int.TryParse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
				{
					values.Add(value);
				}
			}
		}
		return values;
	}

	private static int? ExtractTaggedNumber(string text, string name)
	{
		Match match = Regex.Match(text, $@"\[\s*{Regex.Escape(name)}\s*\]\s*(-?\d+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		return match.Success && int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) ? value : null;
	}

	private static string CleanValue(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}
		string text = value.Trim();
		Match link = Regex.Match(text, @"`([^`]*)`");
		if (link.Success)
		{
			text = link.Groups[1].Value;
		}
		return text.Trim('`', '"', '\'', ' ');
	}

	private static string LabelToken(string value) => CleanValue(value)?.Trim('[', ']').Trim();

	private static string NumberText(int? value) => value?.ToString(CultureInfo.InvariantCulture);

	private static string LevelText(int? value) => value.HasValue ? $"Lv.{value.Value}" : null;

	private static string WeightText(int? value) => value.HasValue ? $"{(value.Value / 1000d).ToString("0.##", CultureInfo.InvariantCulture)}kg" : null;

	private static string PriceText(int? value, int divisor = 1) => value.HasValue ? $"{value.Value / divisor} 金币" : null;

	private static string TimeText(int? value, int divisor) => value.HasValue ? $"{(value.Value / (double)divisor).ToString("0.##", CultureInfo.InvariantCulture)} 秒" : null;

	private static string RangeText(List<int> values) => values.Count switch
	{
		0 => null,
		1 => values[0].ToString(CultureInfo.InvariantCulture),
		_ => $"{values[0]} - {values[^1]}"
	};

	private static string TradeText(string value)
	{
		string token = CleanValue(value);
		if (string.IsNullOrEmpty(token))
		{
			return null;
		}
		string normalized = token.StartsWith("[", StringComparison.Ordinal) ? token.ToLowerInvariant() : $"[{token.ToLowerInvariant()}]";
		return normalized switch
		{
			"[trade]" => "无法交易",
			"[free]" => "自由交易",
			"[sealing]" => "封装",
			"[trade delete]" => "无法交易/删除",
			"[account]" => "账号绑定",
			"[sealing trade]" => "封装且不可交易",
			_ => token
		};
	}

	private static string GetPreviewKind(PvfFile file)
	{
		return file.FileType switch
		{
			PvfFileType.equ => "装备",
			PvfFileType.stk => "道具",
			PvfFileType.shp => "NPC 商店",
			PvfFileType.qst => "任务",
			PvfFileType.skl => "技能",
			PvfFileType.ani => "ANI 动画",
			PvfFileType.als => "ALS 动画层",
			PvfFileType.co or PvfFileType.etc => "技能树",
			_ => "结构化预览"
		};
	}

	private void LoadAniPreview()
	{
		if (sourceDocument?.TextEditorPreviewViewModelBase is TextEditorPreviewViewModelAni aniPreview)
		{
			AniPreviewViewModel = aniPreview;
			AniPreviewStatus = string.Empty;
			aniPreview.LoadDataForPreviewDocument(sourceDocument.Document?.Text ?? string.Empty);
			return;
		}
		AniPreviewStatus = "正在加载 ANI 预览...";
		if (aniLoadAttempts++ >= 12 || Application.Current == null)
		{
			AniPreviewStatus = "ANI 预览需要先加载编辑器和 NPK/IMG 资源。";
			return;
		}
		Application.Current.Dispatcher.BeginInvoke((Action)LoadAniPreview, DispatcherPriority.Background);
	}

	private void OnSourceTextChanged(object sender, EventArgs e)
	{
		if (refreshQueued || Application.Current == null)
		{
			return;
		}
		refreshQueued = true;
		Application.Current.Dispatcher.BeginInvoke((Action)RefreshPreview, DispatcherPriority.Background);
	}

	private void OnSourcePreviewContentChanged(object sender, EventArgs e) => RefreshPreview();

	private void UnsubscribeSource()
	{
		if (sourceDocument != null)
		{
			sourceDocument.PreviewContentChanged -= OnSourcePreviewContentChanged;
		}
		if (sourceTextDocument != null)
		{
			sourceTextDocument.TextChanged -= OnSourceTextChanged;
		}
	}

	public override void Dispose()
	{
		UnsubscribeSource();
		Tags.Clear();
		tagsByName.Clear();
		sourceTextDocument = null;
		sourceDocument = null;
		RichPreview = null;
		AniPreviewViewModel = null;
		SelectedTag = null;
	}
}
