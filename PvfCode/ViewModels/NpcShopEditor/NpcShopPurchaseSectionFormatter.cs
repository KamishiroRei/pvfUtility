using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PvfCode.ViewModels.NpcShopEditor;

internal static class NpcShopPurchaseSectionFormatter
{
	public static bool TryValidate(string? price, string? needMaterialItemCode, string? needMaterialCount, out string? error)
	{
		return TryValidatePrice(price, out error) &&
			TryValidateNeedMaterial(needMaterialItemCode, needMaterialCount, null, out error);
	}

	public static bool TryValidatePrice(string? price, out string? error)
	{
		string? value = Normalize(price);
		if (value == null)
		{
			error = null;
			return true;
		}
		bool valid = TryNormalizePrice(value, out _);
		if (valid)
		{
			error = null;
			return true;
		}
		error = "金币价格必须是整数，留空表示移除price。";
		return false;
	}

	public static bool TryNormalizePrice(string? price, out string? normalizedPrice)
	{
		string? value = Normalize(price);
		if (value == null || !TryParseIntegerPrice(value, out int numericValue))
		{
			normalizedPrice = null;
			return false;
		}

		normalizedPrice = numericValue.ToString(CultureInfo.InvariantCulture);
		return true;
	}

	public static bool TryFormatValueFallbackPrice(string? value, out string? fallbackPrice)
	{
		string? normalizedValue = Normalize(value);
		if (normalizedValue == null)
		{
			fallbackPrice = null;
			return false;
		}

		if (!TryParseIntegerPrice(normalizedValue, out int numericValue))
		{
			fallbackPrice = null;
			return false;
		}

		fallbackPrice = (numericValue / 5).ToString(CultureInfo.InvariantCulture);
		return true;
	}

	public static bool TryValidateNeedMaterial(
		string? needMaterialItemCode,
		string? needMaterialCount,
		IReadOnlyList<int>? additionalNeedMaterialValues,
		out string? error)
	{
		string? itemCode = Normalize(needMaterialItemCode);
		string? count = Normalize(needMaterialCount);
		if ((itemCode == null) != (count == null))
		{
			error = "所需材料ID和所需材料数量必须同时填写，或同时留空。";
			return false;
		}
		if (itemCode != null &&
			(!int.TryParse(itemCode, NumberStyles.Integer, CultureInfo.InvariantCulture, out _) ||
			 !int.TryParse(count, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)))
		{
			error = "所需材料ID和数量必须是整数。";
			return false;
		}
		if (itemCode != null && additionalNeedMaterialValues != null && additionalNeedMaterialValues.Count % 2 != 0)
		{
			error = "原need material包含不完整的材料ID/数量对，无法安全保存。";
			return false;
		}
		error = null;
		return true;
	}

	public static string Format(
		string? price,
		string? needMaterialItemCode,
		string? needMaterialCount,
		IReadOnlyList<int>? additionalNeedMaterialValues = null)
	{
		if (!TryValidate(price, needMaterialItemCode, needMaterialCount, out string? error) ||
			!TryValidateNeedMaterial(needMaterialItemCode, needMaterialCount, additionalNeedMaterialValues, out error))
		{
			throw new InvalidOperationException(error);
		}
		return FormatPrice(price) + FormatNeedMaterial(needMaterialItemCode, needMaterialCount, additionalNeedMaterialValues);
	}

	public static string FormatPrice(string? price)
	{
		if (!TryValidatePrice(price, out string? error))
		{
			throw new InvalidOperationException(error);
		}
		string? value = Normalize(price);
		return value == null ? string.Empty : FormatSection("[price]", value);
	}

	public static string FormatNeedMaterial(
		string? needMaterialItemCode,
		string? needMaterialCount,
		IReadOnlyList<int>? additionalNeedMaterialValues = null)
	{
		if (!TryValidateNeedMaterial(needMaterialItemCode, needMaterialCount, additionalNeedMaterialValues, out string? error))
		{
			throw new InvalidOperationException(error);
		}
		string? itemCode = Normalize(needMaterialItemCode);
		if (itemCode == null)
		{
			return string.Empty;
		}
		StringBuilder materialValues = new StringBuilder($"{itemCode}\t{Normalize(needMaterialCount)}");
		if (additionalNeedMaterialValues != null)
		{
			for (int index = 0; index < additionalNeedMaterialValues.Count; index += 2)
			{
				materialValues.AppendLine();
				materialValues.Append($"{additionalNeedMaterialValues[index]}\t{additionalNeedMaterialValues[index + 1]}");
			}
		}
		return FormatSection("[need material]", materialValues.ToString());
	}

	private static string FormatSection(string sectionName, string value)
	{
		StringBuilder output = new StringBuilder();
		output.AppendLine(sectionName);
		output.AppendLine(value);
		return output.ToString();
	}

	private static string? Normalize(string? value)
	{
		return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
	}

	private static bool TryParseIntegerPrice(string value, out int numericValue)
	{
		return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericValue);
	}
}
