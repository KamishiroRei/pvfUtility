using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PvfCode.ViewModels.DocumentFolder;

internal sealed class PvfParsedEquipmentSetAbility
{
	public int? RequiredPieces { get; init; }

	public string Explanation { get; init; } = string.Empty;

	public string DetailExplanation { get; init; } = string.Empty;

	public int ExplanationSourceOffset { get; init; }

	public int ExplanationSourceLength { get; init; }

	public int DetailExplanationSourceOffset { get; init; }

	public int DetailExplanationSourceLength { get; init; }

	public string AdditionalEffect { get; init; } = string.Empty;

	public int SourceOffset { get; init; }

	public List<PvfParsedSkillDataUpRow> SkillDataRows { get; } = new();
}

internal sealed class PvfParsedSkillDataUpRow
{
	public string Job { get; init; } = string.Empty;

	public int SkillCode { get; init; }

	public string Scope { get; init; } = string.Empty;

	public string DataType { get; init; } = string.Empty;

	public int DataIndex { get; init; }

	public string Operator { get; init; } = string.Empty;

	public int Value { get; init; }

	public int SourceOffset { get; init; }

	public int SourceLength { get; init; }

	public int SourceLineNumber { get; init; }
}

internal static class PvfEquipmentSetSkillDataParser
{
	private static readonly Regex PieceSetAbilityRegex = new(
		@"(?ms)^[ \t]*\[piece set ability\][^\r\n]*(?:\r?\n|$)(?<body>.*?)^[ \t]*\[/piece set ability\][^\r\n]*(?:\r?\n|$)",
		RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	private static readonly Regex SkillDataUpRegex = new(
		@"(?ms)^[ \t]*\[skill data up\][^\r\n]*(?:\r?\n|$)(?<body>.*?)^[ \t]*\[/skill data up\][^\r\n]*(?:\r?\n|$)",
		RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	private static readonly Regex ExplanationRegex = new(
		@"(?ims)^[ \t]*\[(?<kind>parameter basic explain|parameter detail explain)\][^\r\n]*(?:\r?\n|$)(?<body>.*?)(?=^[ \t]*\[(?:parameter basic explain|parameter detail explain|/parameter basic explain|/parameter detail explain|/piece set ability)\][^\r\n]*(?:\r?\n|$)|\z)",
		RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	private static readonly Regex PieceCountRegex = new(
		@"\A(?:[ \t]*\r?\n)*[ \t]*(?<count>\d+)[ \t]*(?://[^\r\n]*)?(?:\r?\n|$)",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly Regex ValueTokenRegex = new(
		"`(?<backtick>[^`]*)`|\"(?<double>[^\"]*)\"|'(?<single>[^']*)'|(?<plain>[^\\s]+)",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	public static IReadOnlyList<PvfParsedEquipmentSetAbility> Parse(string text)
	{
		List<PvfParsedEquipmentSetAbility> abilities = new();
		if (string.IsNullOrWhiteSpace(text))
		{
			return abilities;
		}

		foreach (Match abilityMatch in PieceSetAbilityRegex.Matches(text))
		{
			string body = abilityMatch.Groups["body"].Value;
			Match countMatch = PieceCountRegex.Match(body);
			int? requiredPieces = countMatch.Success && int.TryParse(
				countMatch.Groups["count"].Value,
				NumberStyles.Integer,
				CultureInfo.InvariantCulture,
				out int parsedCount)
				? parsedCount
				: null;
			Match basicExplanationMatch = ExplanationRegex.Matches(body)
				.Cast<Match>()
				.FirstOrDefault(match => string.Equals(match.Groups["kind"].Value, "parameter basic explain", StringComparison.OrdinalIgnoreCase));
			Match detailExplanationMatch = ExplanationRegex.Matches(body)
				.Cast<Match>()
				.FirstOrDefault(match => string.Equals(match.Groups["kind"].Value, "parameter detail explain", StringComparison.OrdinalIgnoreCase));
			PvfParsedEquipmentSetAbility ability = new()
			{
				RequiredPieces = requiredPieces,
				Explanation = basicExplanationMatch != null ? CleanBlockValue(basicExplanationMatch.Groups["body"].Value) : string.Empty,
				DetailExplanation = detailExplanationMatch != null ? CleanBlockValue(detailExplanationMatch.Groups["body"].Value) : string.Empty,
				ExplanationSourceOffset = basicExplanationMatch != null ? abilityMatch.Groups["body"].Index + basicExplanationMatch.Index : 0,
				ExplanationSourceLength = basicExplanationMatch != null ? TagHeaderLength(basicExplanationMatch) : 0,
				DetailExplanationSourceOffset = detailExplanationMatch != null ? abilityMatch.Groups["body"].Index + detailExplanationMatch.Index : 0,
				DetailExplanationSourceLength = detailExplanationMatch != null ? TagHeaderLength(detailExplanationMatch) : 0,
				AdditionalEffect = ExtractAdditionalEffect(body, countMatch),
				SourceOffset = abilityMatch.Index
			};

			foreach (Match blockMatch in SkillDataUpRegex.Matches(body))
			{
				int blockOffset = abilityMatch.Groups["body"].Index + blockMatch.Groups["body"].Index;
				ParseRows(text, blockMatch.Groups["body"].Value, blockOffset, ability.SkillDataRows);
			}
			abilities.Add(ability);
		}
		return abilities;
	}

	private static string ExtractAdditionalEffect(string body, Match countMatch)
	{
		string result = body;
		if (countMatch.Success)
		{
			result = result.Remove(countMatch.Index, countMatch.Length);
		}
		result = SkillDataUpRegex.Replace(result, string.Empty);
		result = ExplanationRegex.Replace(result, string.Empty);
		return result.Trim();
	}

	private static int TagHeaderLength(Match match)
	{
		int newline = match.Value.IndexOfAny(new[] { '\r', '\n' });
		return newline >= 0 ? Math.Max(1, newline) : Math.Max(1, match.Length);
	}

	private static void ParseRows(string source, string block, int blockOffset, List<PvfParsedSkillDataUpRow> rows)
	{
		int relativeOffset = 0;
		foreach (string lineWithEnding in Regex.Split(block, "(?<=\\n)"))
		{
			string line = StripLineComment(lineWithEnding.TrimEnd('\r', '\n'));
			List<string> values = ParseValues(line);
			if (values.Count == 7 &&
				int.TryParse(values[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int skillCode) &&
				int.TryParse(values[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out int dataIndex) &&
				int.TryParse(values[6], NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
			{
				int sourceOffset = blockOffset + relativeOffset;
				rows.Add(new PvfParsedSkillDataUpRow
				{
					Job = NormalizeBracketToken(values[0]),
					SkillCode = skillCode,
					Scope = NormalizeBracketToken(values[2]),
					DataType = NormalizeBracketToken(values[3]),
					DataIndex = dataIndex,
					Operator = values[5].Trim(),
					Value = value,
					SourceOffset = sourceOffset,
					SourceLength = Math.Max(1, line.Length),
					SourceLineNumber = GetLineNumber(source, sourceOffset)
				});
			}
			relativeOffset += lineWithEnding.Length;
		}
	}

	private static List<string> ParseValues(string line)
	{
		List<string> values = new();
		foreach (Match match in ValueTokenRegex.Matches(line))
		{
			string value = match.Groups["backtick"].Success ? match.Groups["backtick"].Value :
				match.Groups["double"].Success ? match.Groups["double"].Value :
				match.Groups["single"].Success ? match.Groups["single"].Value : match.Groups["plain"].Value;
			if (value.Length > 0)
			{
				values.Add(value);
			}
		}
		return values;
	}

	private static string StripLineComment(string line)
	{
		int index = line.IndexOf("//", StringComparison.Ordinal);
		return index >= 0 ? line.Substring(0, index) : line;
	}

	private static string NormalizeBracketToken(string value)
	{
		return value.Trim().Trim('[', ']').Trim().ToLowerInvariant();
	}

	private static string CleanBlockValue(string value)
	{
		string result = value.Trim();
		if (result.Length >= 2 && ((result[0] == '`' && result[^1] == '`') ||
			(result[0] == '"' && result[^1] == '"') || (result[0] == '\'' && result[^1] == '\'')))
		{
			result = result.Substring(1, result.Length - 2);
		}
		return result.Trim();
	}

	private static int GetLineNumber(string source, int offset)
	{
		int lineNumber = 1;
		for (int index = 0; index < offset && index < source.Length; index++)
		{
			if (source[index] == '\n')
			{
				lineNumber++;
			}
		}
		return lineNumber;
	}
}
