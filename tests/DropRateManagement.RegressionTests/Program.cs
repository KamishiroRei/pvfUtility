using PvfCode.ViewModels.DropRateManagement;
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

const string section = DropRateDocument.SectionName;
string hellText = $"#PVF_File\r\n{section}\r\n2\r\n210000\t610000\t755000\t931000\t1000000\t1000001\r\n100000\t500000\t700000\t900000\t1000000\t1234567\r\n[next section]\r\n99\r\n";
string clearText = $"#PVF_File\n{section}\n210000 610000 755000 931000 1000000 1000001\n[next section]\n42\n";

AssertTrue(DropRateDocument.TryParseHell(hellText, out DropRateDocument? hell, out string hellError), hellError);
AssertEqual(2, hell!.Columns.Count, "hell column count");
AssertEqual(176000, hell.Columns[0].Rates[3], "artifact rate is cumulative pink minus purple");
AssertEqual(69000, hell.Columns[0].Rates[4], "epic rate is cumulative epic minus pink");
AssertEqual(1000001, hell.Columns[0].Placeholder, "hell placeholder is preserved");
AssertFalse(
	DropRateDocument.TryParseClearReward($"#PVF_File\n{section}\n210000 610000 755000 931000 1000000 -1\n", out _, out _),
	"negative placeholders are rejected");

AssertTrue(DropRateDocument.TryParseClearReward(clearText, out DropRateDocument? clear, out string clearError), clearError);
AssertEqual(210000, clear!.Columns[0].Rates[0], "normal rate");
AssertEqual("17.6", DropRateColumn.FormatPercentage(clear.Columns[0].Rates[3]), "artifact percentage display");
AssertEqual("6.9", DropRateColumn.FormatPercentage(clear.Columns[0].Rates[4]), "epic percentage display");

string placeholderClearText = $"#PVF_File\n{section}\n210000 610000 755000 1000000 1000001 1000002\n";
AssertTrue(
	DropRateDocument.TryParseClearReward(placeholderClearText, out DropRateDocument? placeholderClear, out string placeholderError),
	placeholderError);
AssertEqual(245000, placeholderClear!.Columns[0].Rates[3], "artifact reaches the one-million cumulative threshold");
AssertEqual(0, placeholderClear.Columns[0].Rates[4], "epic placeholder means a zero-percent epic rate");
AssertTrue(
	placeholderClear.Columns[0].TryBuildCumulative(new[] { "40", "14.5", "24.5", "0" }, out int[] placeholderRebuilt, out string placeholderRebuildError),
	placeholderRebuildError);
AssertSequenceEqual(
	new[] { 210000, 610000, 755000, 1000000, 1000001, 1000002 },
	placeholderRebuilt,
	"trailing placeholders are preserved when their rarity rates remain zero");

AssertTrue(
	DropRateColumn.TryBuildCumulative(new[] { "40", "14.5", "17.6", "6.9" }, 1000001, out int[] rebuilt, out string rebuildError),
	rebuildError);
AssertSequenceEqual(new[] { 210000, 610000, 755000, 931000, 1000000, 1000001 }, rebuilt, "percentage to cumulative conversion");

AssertFalse(
	DropRateColumn.TryBuildCumulative(new[] { "40", "14.5", "-1", "6.9" }, 1000001, out _, out _),
	"negative rates are rejected");
AssertFalse(
	DropRateColumn.TryBuildCumulative(new[] { "40", "40", "20", "1" }, 1000001, out _, out _),
	"negative calculated normal rate is rejected");

string replacedHell = DropRateDocument.ReplaceHellSection(hellText, new[]
{
	rebuilt,
	new[] { 100000, 500000, 700000, 900000, 1000000, 1234567 }
});
AssertTrue(replacedHell.Contains("[next section]\r\n99", StringComparison.Ordinal), "hell replacement preserves following sections");
AssertTrue(replacedHell.Contains("1000001", StringComparison.Ordinal), "hell replacement preserves placeholder");
AssertFalse(replacedHell.Contains($"{section}\r\n\r\n", StringComparison.Ordinal), "hell replacement does not add a blank line after the section header");

string replacedClear = DropRateDocument.ReplaceClearRewardSection(clearText, rebuilt);
AssertTrue(replacedClear.Contains("[next section]\n42", StringComparison.Ordinal), "clear reward replacement preserves following sections");

Console.WriteLine("Drop-rate management regression tests passed.");

static void AssertTrue(bool actual, string scenario)
{
	if (!actual)
	{
		throw new InvalidOperationException($"{scenario}: expected true, got false.");
	}
}

static void AssertFalse(bool actual, string scenario)
{
	if (actual)
	{
		throw new InvalidOperationException($"{scenario}: expected false, got true.");
	}
}

static void AssertEqual<T>(T expected, T actual, string scenario) where T : notnull
{
	if (!EqualityComparer<T>.Default.Equals(expected, actual))
	{
		throw new InvalidOperationException($"{scenario}: expected {expected}, got {actual}.");
	}
}

static void AssertSequenceEqual(IReadOnlyList<int> expected, IReadOnlyList<int> actual, string scenario)
{
	if (!expected.SequenceEqual(actual))
	{
		throw new InvalidOperationException($"{scenario}: expected [{string.Join(", ", expected)}], got [{string.Join(", ", actual)}].");
	}
}
