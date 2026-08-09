using PvfCode.ViewModels.DocumentFolder;

const string previewText = """
#PVF_File
[stackable type] `[booster]`
[booster info]
    [etc]
        2
        3037 1000 5 // weighted candidate: item, relative weight, quantity 999 is comment text
        0 50000 // guaranteed gold: item, quantity
        1 3 // guaranteed life tokens: item, quantity
    [/etc]
    [stackable] 1
        3340 250 2
    [/stackable]
[/booster info]
""";

IReadOnlyList<PvfParsedBoosterGroup> groups = PvfBoosterPreviewParser.Parse(previewText);

AssertEqual(2, groups.Count, "booster child groups");
AssertEqual("etc", groups[0].ItemType, "first group type");
AssertEqual(2, groups[0].GainCount, "group draw count");
AssertEqual(3, groups[0].Items.Count, "weighted and guaranteed rows are retained");
AssertItem(groups[0].Items[0], 3037, 5, 1000, "weighted row uses the final number as quantity");
AssertItem(groups[0].Items[1], 0, 50000, null, "gold without a weight is guaranteed");
AssertItem(groups[0].Items[2], 1, 3, null, "life tokens without a weight are guaranteed");
AssertEqual("金币", groups[0].Items[1].DefaultItemName, "gold pseudo-code display name");
AssertEqual("复活币", groups[0].Items[2].DefaultItemName, "life-token pseudo-code display name");
AssertItem(groups[1].Items.Single(), 3340, 2, 250, "inline draw count does not become an item code");

const string extendedRowText = """
[booster info]
    [avatar]
        1
        39000 1000 1 0 7
    [/avatar]
[/booster info]
""";

IReadOnlyList<PvfParsedBoosterGroup> extendedRowGroups = PvfBoosterPreviewParser.Parse(extendedRowText);
AssertEqual(1, extendedRowGroups.Count, "extended reward row group");
AssertItem(extendedRowGroups[0].Items.Single(), 39000, 7, 1000, "the final column remains quantity when control columns are present");

Console.WriteLine("Gift-box preview regression tests passed.");

static void AssertItem(PvfParsedBoosterItem item, int code, int quantity, int? weight, string scenario)
{
	AssertEqual(code, item.Code, $"{scenario}: code");
	AssertEqual(quantity, item.Quantity, $"{scenario}: quantity");
	AssertEqual(weight, item.Weight, $"{scenario}: weight");
}

static void AssertEqual<T>(T expected, T actual, string scenario)
{
	if (!EqualityComparer<T>.Default.Equals(expected, actual))
	{
		throw new InvalidOperationException($"{scenario}: expected {expected}, got {actual}.");
	}
}
