using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PvfCode;

public enum PvfSkillKind
{
	Active,
	Passive,
	Common,
	Guild
}

public static class PvfSkillClassifier
{
	private sealed class CommonSkillCache
	{
		public PvfFile SourceFile { get; init; }

		public HashSet<int> Codes { get; init; }
	}

	private static readonly ConditionalWeakTable<PvfGroup, CommonSkillCache> CommonSkills = new();
	private static readonly SolidColorBrush ActiveBrush = CreateBrush("#FFE88C");
	private static readonly SolidColorBrush PassiveBrush = CreateBrush("#AEE080");
	private static readonly SolidColorBrush CommonBrush = CreateBrush("#0096FF");
	private static readonly SolidColorBrush GuildBrush = CreateBrush("#FFAFFF");

	public static PvfSkillKind? GetKind(PvfGroup pvf, PvfFile file, string itemName = null)
	{
		if (pvf == null || file?.FileType != PvfFileType.skl || !file.IsScriptFile)
		{
			return null;
		}
		string normalizedPath = file.FileName.Replace('\\', '/').ToLowerInvariant();
		string baseName = Path.GetFileNameWithoutExtension(normalizedPath);
		string displayName = itemName ?? pvf.GetItemName(file) ?? string.Empty;
		if (normalizedPath.Contains("guild", StringComparison.OrdinalIgnoreCase) ||
			baseName.Equals("statusup", StringComparison.OrdinalIgnoreCase) ||
			baseName.Equals("experienceup", StringComparison.OrdinalIgnoreCase) ||
			displayName.Contains("guild", StringComparison.OrdinalIgnoreCase) ||
			displayName.Contains("公会", StringComparison.Ordinal))
		{
			return PvfSkillKind.Guild;
		}
		if (file.ItemCode.HasValue && GetCommonSkillCodes(pvf).Contains(file.ItemCode.Value))
		{
			return PvfSkillKind.Common;
		}
		if (file.GetSectionIntValue("[skill class]", pvf, out int skillClass) && skillClass == 4)
		{
			return PvfSkillKind.Common;
		}
		if (file.GetSectionStringValue("[type]", pvf, out string type))
		{
			string normalizedType = type.Trim(' ', '\t', '\r', '\n', '`', '\'', '"', '[', ']').ToLowerInvariant();
			if (normalizedType == "active")
			{
				return PvfSkillKind.Active;
			}
			if (normalizedType == "passive")
			{
				return PvfSkillKind.Passive;
			}
		}
		return null;
	}

	public static string GetLabel(PvfSkillKind kind)
	{
		return kind switch
		{
			PvfSkillKind.Active => "主动",
			PvfSkillKind.Passive => "被动",
			PvfSkillKind.Common => "通用",
			PvfSkillKind.Guild => "公会",
			_ => string.Empty
		};
	}

	public static Brush GetBrush(PvfSkillKind kind)
	{
		return kind switch
		{
			PvfSkillKind.Active => ActiveBrush,
			PvfSkillKind.Passive => PassiveBrush,
			PvfSkillKind.Common => CommonBrush,
			PvfSkillKind.Guild => GuildBrush,
			_ => Brushes.Transparent
		};
	}

	private static HashSet<int> GetCommonSkillCodes(PvfGroup pvf)
	{
		PvfFile commonFile = pvf.GetFile("clientonly/commonskilllist.co");
		if (CommonSkills.TryGetValue(pvf, out CommonSkillCache cached) && ReferenceEquals(cached.SourceFile, commonFile))
		{
			return cached.Codes;
		}
		CommonSkills.Remove(pvf);
		HashSet<int> codes = new();
		if (commonFile != null && commonFile.GetSectionIntArray("[common skill]", pvf, out List<int> values))
		{
			codes.UnionWith(values);
		}
		CommonSkills.Add(pvf, new CommonSkillCache { SourceFile = commonFile, Codes = codes });
		return codes;
	}

	private static SolidColorBrush CreateBrush(string color)
	{
		SolidColorBrush brush = new((Color)ColorConverter.ConvertFromString(color));
		brush.Freeze();
		return brush;
	}
}

public static class PvfSkillTreeColorBehavior
{
	private static bool initialized;
	private static readonly Dictionary<Style, Style> SkillStyles = new();

	public static void Initialize()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		EventManager.RegisterClassHandler(typeof(TextBlock), FrameworkElement.LoadedEvent, new RoutedEventHandler(OnTextBlockLoaded), true);
	}

	private static void OnTextBlockLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is not TextBlock textBlock || textBlock.Name != "treeItemName")
		{
			return;
		}
		Style baseStyle = textBlock.Style;
		if (baseStyle == null || SkillStyles.ContainsValue(baseStyle))
		{
			return;
		}
		if (!SkillStyles.TryGetValue(baseStyle, out Style skillStyle))
		{
			skillStyle = CreateSkillStyle(baseStyle);
			SkillStyles[baseStyle] = skillStyle;
		}
		textBlock.Style = skillStyle;
	}

	private static Style CreateSkillStyle(Style baseStyle)
	{
		Style style = new(typeof(TextBlock), baseStyle);
		foreach (PvfSkillKind kind in Enum.GetValues<PvfSkillKind>())
		{
			DataTrigger trigger = new()
			{
				Binding = new Binding("Row.Value.SkillKind") { Mode = BindingMode.OneWay },
				Value = kind
			};
			trigger.Setters.Add(new Setter(TextBlock.ForegroundProperty, PvfSkillClassifier.GetBrush(kind)));
			style.Triggers.Add(trigger);
		}
		return style;
	}
}
