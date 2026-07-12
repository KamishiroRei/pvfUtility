#if RECOVERED_LEGACY_PVFCODE_DOT
using System.Reflection;
using PvfCode.Dot.Desktop;

namespace PvfCode.Compatibility;

internal static class PvfCommentDtoCompatibility
{
	private static readonly PropertyInfo TitleProperty = typeof(PvfCommentDto).GetProperty("Title", BindingFlags.Instance | BindingFlags.Public);
	private static readonly PropertyInfo OfficialDescriptionProperty = typeof(PvfCommentDto).GetProperty("OfficialDescription", BindingFlags.Instance | BindingFlags.Public);

	public static string GetTitle(PvfCommentDto comment)
	{
		return GetString(comment, TitleProperty);
	}

	public static void SetTitle(PvfCommentDto comment, string value)
	{
		SetString(comment, TitleProperty, value);
	}

	public static string GetOfficialDescription(PvfCommentDto comment)
	{
		return GetString(comment, OfficialDescriptionProperty);
	}

	public static void SetOfficialDescription(PvfCommentDto comment, string value)
	{
		SetString(comment, OfficialDescriptionProperty, value);
	}

	private static string GetString(PvfCommentDto comment, PropertyInfo property)
	{
		return comment == null || property == null ? string.Empty : property.GetValue(comment) as string ?? string.Empty;
	}

	private static void SetString(PvfCommentDto comment, PropertyInfo property, string value)
	{
		if (comment != null && property?.CanWrite == true)
		{
			property.SetValue(comment, value);
		}
	}
}
#endif
