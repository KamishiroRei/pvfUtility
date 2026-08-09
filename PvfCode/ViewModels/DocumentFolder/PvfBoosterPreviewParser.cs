#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PvfCode.ViewModels.DocumentFolder;

public readonly record struct PvfPreviewSourceSpan(int LineNumber, int Offset, int Length);

public sealed class PvfParsedBoosterItem
{
	public int Code { get; }

	public int Quantity { get; }

	public int? Weight { get; }

	public string? DefaultItemName => Weight.HasValue
		? null
		: Code switch
		{
			0 => "金币",
			1 => "复活币",
			_ => null
		};

	public PvfPreviewSourceSpan Source { get; }

	public PvfParsedBoosterItem(
		int code,
		int quantity,
		int? weight,
		PvfPreviewSourceSpan source)
	{
		Code = code;
		Quantity = quantity;
		Weight = weight;
		Source = source;
	}
}

public sealed class PvfParsedBoosterGroup
{
	public string ItemType { get; }

	public int GainCount { get; }

	public IReadOnlyList<PvfParsedBoosterItem> Items { get; }

	public PvfPreviewSourceSpan Source { get; }

	public PvfParsedBoosterGroup(
		string itemType,
		int gainCount,
		IReadOnlyList<PvfParsedBoosterItem> items,
		PvfPreviewSourceSpan source)
	{
		ItemType = itemType;
		GainCount = gainCount;
		Items = items;
		Source = source;
	}
}

public static class PvfBoosterPreviewParser
{
	private static readonly Regex TagLineRegex = new(
		@"^\s*\[\s*(?<close>/)?\s*(?<name>[^\]\r\n]+?)\s*\]\s*(?<value>.*)$",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly Regex NumberRegex = new(
		@"-?\d+",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly HashSet<string> ItemTypes = new(StringComparer.OrdinalIgnoreCase)
	{
		"avatar",
		"special avatar",
		"creature",
		"etc",
		"equipment",
		"cera",
		"stackable",
		"emblem"
	};

	public static IReadOnlyList<PvfParsedBoosterGroup> Parse(string? text)
	{
		List<PvfParsedBoosterGroup> groups = new();
		if (string.IsNullOrEmpty(text))
		{
			return groups;
		}

		bool insideBoosterInfo = false;
		GroupBuilder? currentGroup = null;
		int lineNumber = 1;
		int offset = 0;
		foreach (string rawWithEnding in Regex.Split(text, "(?<=\\n)"))
		{
			string rawLine = rawWithEnding.TrimEnd('\r', '\n');
			Match tagMatch = TagLineRegex.Match(rawLine);
			if (tagMatch.Success)
			{
				string name = NormalizeTagName(tagMatch.Groups["name"].Value);
				bool isClosingTag = tagMatch.Groups["close"].Success;
				if (isClosingTag)
				{
					if (currentGroup != null && string.Equals(name, currentGroup.ItemType, StringComparison.OrdinalIgnoreCase))
					{
						AddGroup(groups, currentGroup);
						currentGroup = null;
					}
					if (string.Equals(name, "booster info", StringComparison.OrdinalIgnoreCase))
					{
						if (currentGroup != null)
						{
							AddGroup(groups, currentGroup);
							currentGroup = null;
						}
						insideBoosterInfo = false;
					}
				}
				else if (string.Equals(name, "booster info", StringComparison.OrdinalIgnoreCase))
				{
					insideBoosterInfo = true;
				}
				else if (insideBoosterInfo && ItemTypes.Contains(name))
				{
					if (currentGroup != null)
					{
						AddGroup(groups, currentGroup);
					}
					PvfPreviewSourceSpan source = new(lineNumber, offset, Math.Max(1, rawLine.Length));
					currentGroup = new GroupBuilder(name, source);
					currentGroup.AddLine(tagMatch.Groups["value"].Value, source);
				}
			}
			else if (insideBoosterInfo && currentGroup != null)
			{
				currentGroup.AddLine(rawLine, new PvfPreviewSourceSpan(lineNumber, offset, Math.Max(1, rawLine.Length)));
			}

			offset += rawWithEnding.Length;
			lineNumber++;
		}

		if (currentGroup != null)
		{
			AddGroup(groups, currentGroup);
		}
		return groups;
	}

	private static void AddGroup(List<PvfParsedBoosterGroup> groups, GroupBuilder builder)
	{
		if (builder.Items.Count == 0)
		{
			return;
		}
		groups.Add(new PvfParsedBoosterGroup(
			builder.ItemType,
			builder.GainCount,
			builder.Items,
			builder.Source));
	}

	private static string NormalizeTagName(string value)
	{
		return Regex.Replace(value.Trim(), @"\s+", " ").ToLowerInvariant();
	}

	private static List<int> ParseNumbers(string value)
	{
		int commentIndex = value.IndexOf("//", StringComparison.Ordinal);
		string data = commentIndex >= 0 ? value.Substring(0, commentIndex) : value;
		List<int> values = new();
		foreach (Match match in NumberRegex.Matches(data))
		{
			if (int.TryParse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int number))
			{
				values.Add(number);
			}
		}
		return values;
	}

	private sealed class GroupBuilder
	{
		private bool hasGainCount;

		public string ItemType { get; }

		public int GainCount { get; private set; }

		public List<PvfParsedBoosterItem> Items { get; } = new();

		public PvfPreviewSourceSpan Source { get; }

		public GroupBuilder(string itemType, PvfPreviewSourceSpan source)
		{
			ItemType = itemType;
			Source = source;
		}

		public void AddLine(string value, PvfPreviewSourceSpan source)
		{
			List<int> numbers = ParseNumbers(value);
			if (numbers.Count == 0)
			{
				return;
			}
			if (!hasGainCount)
			{
				GainCount = numbers[0];
				hasGainCount = true;
				numbers.RemoveAt(0);
			}
			if (numbers.Count < 2)
			{
				return;
			}

			// Preview rows use the first value as ID, an optional second value as weight, and the last as quantity.
			Items.Add(new PvfParsedBoosterItem(
				numbers[0],
				numbers[^1],
				numbers.Count >= 3 ? numbers[1] : null,
				source));
		}
	}
}
