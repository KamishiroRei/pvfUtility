#nullable enable

using System;
using System.Text.RegularExpressions;

namespace PvfCode.OfficialAnnotations;

public static class OfficialAnnotationLinks
{
	public const string Scheme = "pvf-official-annotation";

	private static readonly Regex OfficialExampleRegex = new(
		@"(?m)(?<prefix>^[^\r\n]*?官方示例\s*[:：]\s*)(?<file>[^\r\n\[\]]+?\.[A-Za-z0-9_]+)(?<suffix>\s*$)",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	public static event Action<string>? OpenRequested;

	public static string LinkifyOfficialExamples(string markdown)
	{
		if (string.IsNullOrWhiteSpace(markdown))
		{
			return markdown;
		}

		return OfficialExampleRegex.Replace(markdown, match =>
		{
			string fileName = match.Groups["file"].Value.Trim();
			return match.Groups["prefix"].Value +
				"[" + fileName + "](" + CreateUri(fileName).AbsoluteUri + ")" +
				match.Groups["suffix"].Value;
		});
	}

	public static Uri CreateUri(string fileName)
	{
		string normalized = (fileName ?? string.Empty).Replace('\\', '/').Trim();
		return new Uri(Scheme + "://open/" + Uri.EscapeDataString(normalized), UriKind.Absolute);
	}

	public static bool TryGetFileName(Uri uri, out string? fileName)
	{
		fileName = null;
		if (uri == null || !string.Equals(uri.Scheme, Scheme, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		fileName = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'));
		return !string.IsNullOrWhiteSpace(fileName);
	}

	public static void RequestOpen(string fileName)
	{
		if (!string.IsNullOrWhiteSpace(fileName))
		{
			OpenRequested?.Invoke(fileName);
		}
	}
}
