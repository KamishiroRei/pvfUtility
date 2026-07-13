using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

#nullable enable

namespace PvfCode.ViewModels.DocumentFolder;

public sealed class PvfParsedSkillTreeNode
{
	public int Code { get; internal set; } = -1;

	public int? X { get; internal set; }

	public int? Y { get; internal set; }

	public bool IsCommon { get; internal set; }

	public List<int> NextSkills { get; } = new();

	public string SourceTagName { get; internal set; } = string.Empty;

	public int SourceOffset { get; internal set; }

	public int SourceLength { get; internal set; }
}

public sealed class PvfParsedSkillTreeGroup
{
	public string? Job { get; internal set; }

	public string? Branch { get; internal set; }

	public List<PvfParsedSkillTreeNode> Nodes { get; } = new();
}

public static class PvfSkillTreeParser
{
	private static readonly Regex TagRegex = new(
		@"^\s*\[([^\]]+)\]\s*(.*)$",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly Regex NumberRegex = new(
		@"-?\d+",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly Regex BacktickRegex = new(
		@"`([^`]*)`",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	public static IReadOnlyList<PvfParsedSkillTreeGroup> Parse(string text)
	{
		List<PvfParsedSkillTreeGroup> groups = new();
		PvfParsedSkillTreeGroup? group = null;
		PvfParsedSkillTreeNode? node = null;
		bool inCharacterJob = false;
		bool inSkillInfo = false;
		string currentTag = string.Empty;

		void FlushNode()
		{
			if (group != null && node != null && node.Code >= 0)
			{
				group.Nodes.Add(node);
			}
			node = null;
		}

		void FlushGroup()
		{
			FlushNode();
			if (group?.Nodes.Count > 0)
			{
				groups.Add(group);
			}
			group = null;
		}

		void ConsumeValue(string value)
		{
			if (group == null)
			{
				return;
			}
			string stripped = StripLineComment(value).Trim();
			if (stripped.Length == 0)
			{
				return;
			}
			string cleaned = LabelToken(stripped) ?? CleanValue(stripped) ?? stripped;
			if (!inSkillInfo)
			{
				if (group.Job == null && LooksLikeSkillTreeToken(cleaned))
				{
					group.Job = cleaned;
					return;
				}
				if (group.Job != null && group.Branch == null && LooksLikeSkillTreeToken(cleaned))
				{
					group.Branch = cleaned;
				}
				return;
			}
			if (node == null)
			{
				return;
			}
			List<int> numbers = NumbersFromLine(stripped);
			if (currentTag == "index" && numbers.Count > 0)
			{
				node.Code = numbers[0];
			}
			else if (currentTag == "icon pos" && numbers.Count >= 2)
			{
				node.X = numbers[0];
				node.Y = numbers[1];
			}
			else if (currentTag == "next skill" && numbers.Count > 0)
			{
				node.NextSkills.AddRange(numbers.FindAll(value => value >= 0));
			}
		}

		ForEachLine(text ?? string.Empty, (rawLine, lineOffset) =>
		{
			string line = rawLine.Trim();
			if (line.Length == 0 || line.StartsWith("//", StringComparison.Ordinal))
			{
				return;
			}
			Match tag = TagRegex.Match(rawLine);
			if (tag.Success)
			{
				string name = tag.Groups[1].Value.Trim().ToLowerInvariant();
				string inline = tag.Groups[2].Value.Trim();
				if (name == "character job")
				{
					FlushGroup();
					inCharacterJob = true;
					inSkillInfo = false;
					currentTag = string.Empty;
					group = new PvfParsedSkillTreeGroup();
					if (inline.Length > 0)
					{
						ConsumeValue(inline);
					}
					return;
				}
				if (name == "/character job")
				{
					FlushGroup();
					inCharacterJob = false;
					inSkillInfo = false;
					currentTag = string.Empty;
					return;
				}
				if (!inCharacterJob)
				{
					return;
				}
				if (name == "skill info" || name == "common skill")
				{
					FlushNode();
					inSkillInfo = true;
					currentTag = string.Empty;
					node = new PvfParsedSkillTreeNode
					{
						IsCommon = name == "common skill",
						SourceTagName = name,
						SourceOffset = lineOffset + tag.Groups[1].Index,
						SourceLength = tag.Groups[1].Length
					};
					if (inline.Length > 0)
					{
						ConsumeValue(inline);
					}
					return;
				}
				if (name == "/skill info" || name == "/common skill")
				{
					FlushNode();
					inSkillInfo = false;
					currentTag = string.Empty;
					return;
				}
				currentTag = name;
				if (inline.Length > 0)
				{
					ConsumeValue(inline);
				}
				return;
			}
			if (inCharacterJob)
			{
				ConsumeValue(line);
			}
		});
		FlushGroup();
		return groups;
	}

	private static void ForEachLine(string text, Action<string, int> consume)
	{
		int offset = 0;
		while (offset < text.Length)
		{
			int lineEnd = offset;
			while (lineEnd < text.Length && text[lineEnd] != '\r' && text[lineEnd] != '\n')
			{
				lineEnd++;
			}
			consume(text.Substring(offset, lineEnd - offset), offset);
			if (lineEnd < text.Length && text[lineEnd] == '\r')
			{
				lineEnd++;
			}
			if (lineEnd < text.Length && text[lineEnd] == '\n')
			{
				lineEnd++;
			}
			offset = lineEnd;
		}
	}

	private static string StripLineComment(string value)
	{
		int index = value.IndexOf("//", StringComparison.Ordinal);
		return index >= 0 ? value.Substring(0, index) : value;
	}

	private static string? CleanValue(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}
		string cleaned = value.Trim();
		Match backtick = BacktickRegex.Match(cleaned);
		if (backtick.Success)
		{
			cleaned = backtick.Groups[1].Value;
		}
		cleaned = cleaned.Trim('`', '"', '\'', ' ');
		return cleaned.Length == 0 ? null : cleaned;
	}

	private static string? LabelToken(string value)
	{
		string? cleaned = CleanValue(value);
		return cleaned?.Trim('[', ']').Trim();
	}

	private static bool LooksLikeSkillTreeToken(string? value)
	{
		return value != null && Regex.IsMatch(value.Trim(), @"^[a-z][a-z0-9 _-]*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
	}

	private static List<int> NumbersFromLine(string line)
	{
		List<int> values = new();
		foreach (Match match in NumberRegex.Matches(line))
		{
			if (int.TryParse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
			{
				values.Add(value);
			}
		}
		return values;
	}
}
