using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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

	public static void Initialize()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		EventManager.RegisterClassHandler(typeof(TextBlock), FrameworkElement.LoadedEvent, new RoutedEventHandler(OnTextBlockLoaded));
	}

	private static void OnTextBlockLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is not TextBlock textBlock || textBlock.Name != "treeItemName")
		{
			return;
		}
		textBlock.DataContextChanged -= OnDataContextChanged;
		textBlock.DataContextChanged += OnDataContextChanged;
		ApplyColor(textBlock);
	}

	private static void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		if (sender is TextBlock textBlock)
		{
			ApplyColor(textBlock);
		}
	}

	private static void ApplyColor(TextBlock textBlock)
	{
		PvfTreeFileBase node = FindTreeNode(textBlock.DataContext);
		PvfSkillKind? kind = node?.SkillKind;
		if (kind.HasValue)
		{
			textBlock.Foreground = PvfSkillClassifier.GetBrush(kind.Value);
		}
		else
		{
			textBlock.ClearValue(TextBlock.ForegroundProperty);
		}
	}

	private static PvfTreeFileBase FindTreeNode(object value)
	{
		for (int depth = 0; value != null && depth < 4; depth++)
		{
			if (value is PvfTreeFileBase node)
			{
				return node;
			}
			Type type = value.GetType();
			PropertyInfo property = type.GetProperty(depth == 0 ? "Row" : "Value", BindingFlags.Instance | BindingFlags.Public) ??
				type.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public) ??
				type.GetProperty("Row", BindingFlags.Instance | BindingFlags.Public);
			value = property?.GetValue(value);
		}
		return null;
	}
}
