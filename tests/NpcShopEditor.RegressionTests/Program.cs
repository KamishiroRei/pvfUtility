using PvfCode.ViewModels.NpcShopEditor;
using System.Globalization;
using System.Text;
using PvfCode;
using PvfCode.Services;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var newline = Environment.NewLine;

AssertEqual(
	$"[price]{newline}1200{newline}",
    NpcShopPurchaseSectionFormatter.Format("1200", null, null),
    "price-only formatting");

AssertFalse(
	NpcShopPurchaseSectionFormatter.TryValidatePrice("12.5", out _),
	"decimal price must be rejected");
AssertTrue(
	NpcShopPurchaseSectionFormatter.TryNormalizePrice(" 0012 ", out string? normalizedPrice),
	"integer price must be normalizable");
AssertEqual("12", normalizedPrice!, "integer price normalization");
AssertFalse(
	NpcShopPurchaseSectionFormatter.TryNormalizePrice("12.5", out _),
	"decimal price must not be normalized for display");

AssertEqual(
    $"[price]{newline}1200{newline}" +
    $"[need material]{newline}3037\t4{newline}",
    NpcShopPurchaseSectionFormatter.Format("1200", "3037", "4"),
    "price and need-material formatting");

AssertEqual(
	$"[need material]{newline}3037\t4{newline}",
	NpcShopPurchaseSectionFormatter.FormatNeedMaterial("3037", "4"),
	"need-material output uses the verified unclosed root-section format");

AssertFalse(
	NpcShopPurchaseSectionFormatter.Format("1200", "3037", "4").Contains("[/", StringComparison.Ordinal),
	"purchase root fields must not gain closing tags");

AssertEqual(
	$"[need material]{newline}3037\t4{newline}3040\t2{newline}",
    NpcShopPurchaseSectionFormatter.Format(null, "3037", "4", [3040, 2]),
    "additional need-material pairs are preserved");

AssertEqual(
    string.Empty,
    NpcShopPurchaseSectionFormatter.Format(null, null, null),
    "clearing purchase fields removes both sections");

AssertFalse(
    NpcShopPurchaseSectionFormatter.TryValidate(null, "3037", null, out _),
    "need-material id without count must be rejected");

AssertFalse(
    NpcShopPurchaseSectionFormatter.TryValidate(null, null, "4", out _),
    "need-material count without id must be rejected");

AssertFalse(
	NpcShopPurchaseSectionFormatter.TryValidate("not-a-number", null, null, out _),
	"non-numeric price must be rejected");

AssertFalse(
	NpcShopPurchaseSectionFormatter.TryValidate("2147483648", null, null, out _),
	"integer price outside Int32 must match compiler rejection");

AssertFalse(
	NpcShopPurchaseSectionFormatter.TryValidate("1e3", null, null, out _),
	"scientific notation must match compiler rejection");

AssertFalse(
    NpcShopPurchaseSectionFormatter.TryValidate(null, "not-an-id", "4", out _),
    "non-integer need-material values must be rejected");

AssertFalse(
    NpcShopPurchaseSectionFormatter.TryValidateNeedMaterial("3037", "4", [3040], out _),
    "incomplete additional need-material pair must be rejected");

AssertFallbackPrice("1000", "200", "integer value fallback");
AssertFallbackPrice("1001", "200", "integer value fallback truncates remainder");
AssertFalse(
	NpcShopPurchaseSectionFormatter.TryFormatValueFallbackPrice("12.5", out _),
	"decimal value fallback must be rejected");
AssertFalse(
	NpcShopPurchaseSectionFormatter.TryFormatValueFallbackPrice("not-a-number", out _),
	"invalid value fallback must be rejected");

CultureInfo originalCulture = CultureInfo.CurrentCulture;
try
{
	AssertCompilerFloat("de-DE", 12.5f);
	AssertCompilerFloat("fr-FR", 12.5f);

	PvfGroup group = CreateCompilerGroup();
	ScriptFileCompilerOl compiler = new ScriptFileCompilerOl(group);
	(bool invalidSuccess, _) = compiler.EncryptScriptText("#PVF_File\n[price]\nnot-a-number", readOnly: false, notShowError: true);
	AssertFalse(invalidSuccess, "script compiler must reject an invalid numeric token");
	PvfFile invalidFile = new PvfFile("stackable/compiler-regression.stk");
	AssertTrue(
		compiler.Compile(invalidFile, "#PVF_File\n[price]\nnot-a-number") == null,
		"production compiler must return null for an invalid numeric token");
	AssertFalse(
		group.SaveFileText(invalidFile, "#PVF_File\n[price]\nnot-a-number"),
		"SaveFileText must report an invalid numeric token as failure");
}
finally
{
	CultureInfo.CurrentCulture = originalCulture;
}

Console.WriteLine("NpcShopEditor purchase-data regression tests passed.");

static void AssertEqual(string expected, string actual, string scenario)
{
    if (!string.Equals(expected, actual, StringComparison.Ordinal))
    {
        throw new InvalidOperationException(
            $"{scenario} failed.{Environment.NewLine}Expected:{Environment.NewLine}{expected}{Environment.NewLine}Actual:{Environment.NewLine}{actual}");
    }
}

static void AssertFalse(bool actual, string scenario)
{
    if (actual)
    {
        throw new InvalidOperationException($"{scenario} failed: expected false, got true.");
    }
}

static void AssertTrue(bool actual, string scenario)
{
	if (!actual)
	{
		throw new InvalidOperationException($"{scenario} failed: expected true, got false.");
	}
}

static void AssertCompilerFloat(string cultureName, float expected)
{
	CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
	PvfGroup group = CreateCompilerGroup();
	ScriptFileCompilerOl compiler = new ScriptFileCompilerOl(group);
	(bool success, byte[] data) = compiler.EncryptScriptText("#PVF_File\n[price]\n12.5", readOnly: false, notShowError: true);
	if (!success || data.Length != 10 || data[5] != 4)
	{
		throw new InvalidOperationException($"script compiler float token failed under {cultureName}.");
	}
	float actual = BitConverter.ToSingle(data, 6);
	if (actual != expected)
	{
		throw new InvalidOperationException($"script compiler float value under {cultureName}: expected {expected}, got {actual}.");
	}
}

static void AssertFallbackPrice(string value, string expected, string scenario)
{
	if (!NpcShopPurchaseSectionFormatter.TryFormatValueFallbackPrice(value, out string? actual))
	{
		throw new InvalidOperationException($"{scenario} failed: value could not be formatted.");
	}
	AssertEqual(expected, actual!, scenario);
}

static PvfGroup CreateCompilerGroup()
{
	PvfGroup group = new PvfGroup();
	group.Strtable.InitDefault();
	return group;
}
