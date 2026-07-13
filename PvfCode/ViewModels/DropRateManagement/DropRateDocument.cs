#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PvfCode.ViewModels.DropRateManagement;

internal sealed class DropRateDocument
{
	public const string SectionName = "[basis of rarity dicision]";
	public const int Scale = 1_000_000;

	private static readonly Regex IntegerRegex = new(@"(?<![\w.])-?\d+(?![\w.])", RegexOptions.Compiled);

	public IReadOnlyList<DropRateColumn> Columns { get; }

	private DropRateDocument(IReadOnlyList<DropRateColumn> columns)
	{
		Columns = columns;
	}

	public static bool TryParseHell(string text, out DropRateDocument? document, out string error)
	{
		document = null;
		if (!TryReadSectionValues(text, out List<int> values, out error))
		{
			return false;
		}
		if (values.Count == 0 || values[0] != 2)
		{
			error = $"{SectionName} 的第一个数字必须是难度数量 2。";
			return false;
		}
		if (values.Count != 13)
		{
			error = $"{SectionName} 应包含难度数量和两组各 6 个数字，实际读取到 {values.Count} 个数字。";
			return false;
		}

		List<DropRateColumn> columns = new(2);
		for (int index = 0; index < 2; index++)
		{
			if (!DropRateColumn.TryCreate(values.Skip(1 + index * 6).Take(6).ToArray(), out DropRateColumn? column, out error))
			{
				error = $"第 {index + 1} 个深渊难度的数据无效：{error}";
				return false;
			}
			columns.Add(column!);
		}

		document = new DropRateDocument(columns);
		return true;
	}

	public static bool TryParseClearReward(string text, out DropRateDocument? document, out string error)
	{
		document = null;
		if (!TryReadSectionValues(text, out List<int> values, out error))
		{
			return false;
		}
		if (values.Count != 6)
		{
			error = $"{SectionName} 应包含 6 个数字，实际读取到 {values.Count} 个数字。";
			return false;
		}
		if (!DropRateColumn.TryCreate(values, out DropRateColumn? column, out error))
		{
			return false;
		}

		document = new DropRateDocument(new[] { column! });
		return true;
	}

	public static string ReplaceHellSection(string text, IReadOnlyList<int[]> columns)
	{
		if (columns.Count != 2 || columns.Any(column => column.Length != 6))
		{
			throw new ArgumentException("深渊爆率必须提供两组各 6 个累加值。", nameof(columns));
		}

		return ReplaceSection(text, new[]
		{
			"2",
			string.Join("\t", columns[0]),
			string.Join("\t", columns[1])
		});
	}

	public static string ReplaceClearRewardSection(string text, int[] column)
	{
		if (column.Length != 6)
		{
			throw new ArgumentException("翻牌爆率必须提供 6 个累加值。", nameof(column));
		}

		return ReplaceSection(text, new[] { string.Join("\t", column) });
	}

	private static bool TryReadSectionValues(string text, out List<int> values, out string error)
	{
		values = new List<int>();
		if (!TryFindSection(text, out _, out int bodyStart, out int bodyEnd))
		{
			error = $"未找到 {SectionName} 节。";
			return false;
		}

		string body = text.Substring(bodyStart, bodyEnd - bodyStart);
		foreach (string line in body.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None))
		{
			string valuePart = line.Split(new[] { "//" }, 2, StringSplitOptions.None)[0];
			foreach (Match match in IntegerRegex.Matches(valuePart))
			{
				if (!int.TryParse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
				{
					error = $"无法读取数字 {match.Value}。";
					return false;
				}
				values.Add(value);
			}
		}

		error = string.Empty;
		return true;
	}

	private static string ReplaceSection(string text, IReadOnlyList<string> lines)
	{
		if (!TryFindSection(text, out int headerEnd, out _, out int bodyEnd))
		{
			throw new InvalidOperationException($"未找到 {SectionName} 节。");
		}

		string newline = text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
		string replacement = newline + string.Join(newline, lines) + newline;
		return text[..headerEnd] + replacement + text[bodyEnd..];
	}

	private static bool TryFindSection(string text, out int headerEnd, out int bodyStart, out int bodyEnd)
	{
		Regex headerRegex = new(
			@"(?im)^[^\S\r\n]*" + Regex.Escape(SectionName) + @"[^\S\r\n]*(?=\r?$)",
			RegexOptions.CultureInvariant);
		Match header = headerRegex.Match(text);
		if (!header.Success)
		{
			headerEnd = bodyStart = bodyEnd = 0;
			return false;
		}

		headerEnd = header.Index + header.Length;
		bodyStart = headerEnd;
		Regex nextHeaderRegex = new(
			@"(?m)^[^\S\r\n]*\[[^\]\r\n]+\][^\S\r\n]*(?=\r?$)",
			RegexOptions.CultureInvariant);
		Match nextHeader = nextHeaderRegex.Match(text, headerEnd);
		bodyEnd = nextHeader.Success ? nextHeader.Index : text.Length;
		return true;
	}
}

internal sealed class DropRateColumn
{
	public IReadOnlyList<int> Rates { get; }

	public int Placeholder => originalCumulativeValues[5];

	private readonly int[] originalCumulativeValues;

	private DropRateColumn(IReadOnlyList<int> rates, int[] originalCumulativeValues)
	{
		Rates = rates;
		this.originalCumulativeValues = originalCumulativeValues;
	}

	public static bool TryCreate(IReadOnlyList<int> cumulativeValues, out DropRateColumn? column, out string error)
	{
		column = null;
		if (cumulativeValues.Count != 6)
		{
			error = "每组必须包含 6 个累加值。";
			return false;
		}

		int[] rates = new int[5];
		int previous = 0;
		bool reachedScale = false;
		for (int index = 0; index < rates.Length; index++)
		{
			int cumulative = cumulativeValues[index];
			if (cumulative < 0)
			{
				error = $"第 {index + 1} 个数字不能为负数。";
				return false;
			}
			if (reachedScale)
			{
				if (cumulative < DropRateDocument.Scale)
				{
					error = $"第 {index + 1} 个数字位于 100% 累加值之后，不能小于 {DropRateDocument.Scale}。";
					return false;
				}
				rates[index] = 0;
				continue;
			}
			if (cumulative < previous)
			{
				error = $"第 {index + 1} 个累加值小于前一个值。";
				return false;
			}
			if (cumulative > DropRateDocument.Scale)
			{
				error = $"第 {index + 1} 个累加值不能超过 {DropRateDocument.Scale}。";
				return false;
			}
			rates[index] = cumulative - previous;
			previous = cumulative;
			reachedScale = cumulative == DropRateDocument.Scale;
		}
		if (!reachedScale)
		{
			error = $"前 5 个数字中必须包含累加值 {DropRateDocument.Scale}。";
			return false;
		}
		if (cumulativeValues[5] < 0)
		{
			error = "占位值不能为负数。";
			return false;
		}

		column = new DropRateColumn(rates, cumulativeValues.ToArray());
		error = string.Empty;
		return true;
	}

	public bool TryBuildCumulative(
		IReadOnlyList<string> editablePercentages,
		out int[] cumulativeValues,
		out string error)
	{
		if (!TryBuildCumulative(editablePercentages, Placeholder, out cumulativeValues, out error))
		{
			return false;
		}

		int[] calculatedValues = cumulativeValues.ToArray();
		for (int index = 1; index < 5; index++)
		{
			if (originalCumulativeValues[index] > DropRateDocument.Scale &&
				calculatedValues[index - 1] == DropRateDocument.Scale &&
				calculatedValues[index] == DropRateDocument.Scale)
			{
				cumulativeValues[index] = originalCumulativeValues[index];
			}
		}
		return true;
	}

	public static bool TryBuildCumulative(
		IReadOnlyList<string> editablePercentages,
		int placeholder,
		out int[] cumulativeValues,
		out string error)
	{
		cumulativeValues = Array.Empty<int>();
		if (editablePercentages.Count != 4)
		{
			error = "必须提供高级、稀有、神器和史诗四项爆率。";
			return false;
		}

		int[] editableRates = new int[4];
		long editableTotal = 0;
		for (int index = 0; index < editablePercentages.Count; index++)
		{
			if (!TryParsePercentage(editablePercentages[index], out decimal percentage))
			{
				error = $"第 {index + 2} 个稀有度爆率不是有效数字。";
				return false;
			}
			if (percentage < 0)
			{
				error = "爆率不能为负数。";
				return false;
			}

			decimal scaled = decimal.Round(percentage * 10_000m, 0, MidpointRounding.AwayFromZero);
			if (scaled > DropRateDocument.Scale)
			{
				error = "单项爆率不能超过 100%。";
				return false;
			}
			editableRates[index] = decimal.ToInt32(scaled);
			editableTotal += editableRates[index];
		}

		int normalRate = checked((int)(DropRateDocument.Scale - editableTotal));
		if (normalRate < 0)
		{
			error = "其他稀有度爆率之和不能超过 100%，否则普通爆率会为负数。";
			return false;
		}

		cumulativeValues = new int[6];
		cumulativeValues[0] = normalRate;
		for (int index = 0; index < editableRates.Length; index++)
		{
			cumulativeValues[index + 1] = cumulativeValues[index] + editableRates[index];
		}
		cumulativeValues[5] = placeholder;
		error = string.Empty;
		return true;
	}

	public static string FormatPercentage(int rate)
	{
		return (rate / 10_000m).ToString("0.####", CultureInfo.CurrentCulture);
	}

	private static bool TryParsePercentage(string value, out decimal percentage)
	{
		return decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out percentage) ||
			decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out percentage);
	}
}
