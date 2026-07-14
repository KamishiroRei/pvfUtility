#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PvfCode.OfficialAnnotations;

public sealed class OfficialAnnotationCatalog
{
	private readonly Dictionary<string, string> filesByAlias;

	public string RootDirectory { get; }

	public IReadOnlyList<string> Files { get; }

	public OfficialAnnotationCatalog(string? rootDirectory = null)
	{
		RootDirectory = Path.GetFullPath(rootDirectory ?? Path.Combine(
			AppContext.BaseDirectory,
			"Resources",
			"OfficialAnnotationTranslation"));
		filesByAlias = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		if (!Directory.Exists(RootDirectory))
		{
			Files = Array.Empty<string>();
			return;
		}

		List<string> files = Directory.EnumerateFiles(RootDirectory, "*", SearchOption.AllDirectories)
			.Select(path => NormalizeRelativePath(Path.GetRelativePath(RootDirectory, path)))
			.OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
			.ToList();
		Files = files;

		foreach (string relativePath in files)
		{
			filesByAlias.TryAdd(relativePath, relativePath);
			if (relativePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
			{
				filesByAlias.TryAdd(relativePath[..^4], relativePath);
			}
		}
	}

	public bool TryRead(string requestedFile, out string? resolvedPath, out string? content)
	{
		resolvedPath = null;
		content = null;
		if (!TryNormalizeRequest(requestedFile, out string? normalizedRequest) || normalizedRequest == null ||
			!filesByAlias.TryGetValue(normalizedRequest, out string? relativePath) || relativePath == null)
		{
			return false;
		}

		string fullPath = Path.GetFullPath(Path.Combine(
			RootDirectory,
			relativePath.Replace('/', Path.DirectorySeparatorChar)));
		string rootWithSeparator = Path.TrimEndingDirectorySeparator(RootDirectory) + Path.DirectorySeparatorChar;
		if (!fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
		{
			return false;
		}

		resolvedPath = relativePath;
		content = File.ReadAllText(fullPath);
		return true;
	}

	private static bool TryNormalizeRequest(string requestedFile, out string? normalizedRequest)
	{
		normalizedRequest = null;
		if (string.IsNullOrWhiteSpace(requestedFile) || Path.IsPathRooted(requestedFile))
		{
			return false;
		}

		string candidate = NormalizeRelativePath(requestedFile.Trim());
		string[] segments = candidate.Split('/', StringSplitOptions.RemoveEmptyEntries);
		if (segments.Length == 0 || segments.Any(segment => segment is "." or ".." || segment.Contains(':')))
		{
			return false;
		}

		normalizedRequest = string.Join('/', segments);
		return true;
	}

	private static string NormalizeRelativePath(string path)
	{
		return path.Replace('\\', '/').TrimStart('/');
	}
}
