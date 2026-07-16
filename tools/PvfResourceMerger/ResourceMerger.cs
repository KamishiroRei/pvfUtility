using System.Collections;
using System.Reflection;
using System.Resources;

namespace PvfResourceMerger;

public sealed record ResourceMergeResult(
	int BaseResourceCount,
	int OverlayResourceCount,
	int ReplacementCount,
	int OutputResourceCount,
	IReadOnlyList<string> ReplacementKeys);

public sealed record ResourceAuditResult(
	int BaseResourceCount,
	int OverlayResourceCount,
	int OutputResourceCount,
	int BamlResourceCount,
	IReadOnlyList<string> ReplacementKeys,
	IReadOnlyList<string> ChangedKeys,
	string AssemblyResourceName);

public sealed class ResourceMergeException : Exception
{
	public ResourceMergeException(string message)
		: base(message)
	{
	}

	public ResourceMergeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}

public static class ResourceMerger
{
	public static ResourceMergeResult Merge(
		string basePath,
		string overlayPath,
		string outputPath,
		string replacementManifestPath)
	{
		string normalizedBasePath = ValidateInputPath(basePath, "base resource file");
		string normalizedOverlayPath = ValidateInputPath(overlayPath, "overlay resource file");
		string normalizedManifestPath = ValidateInputPath(replacementManifestPath, "replacement manifest");
		string normalizedOutputPath = ValidateOutputPath(outputPath);

		RejectOutputInputCollision(normalizedOutputPath, normalizedBasePath, "base resource file");
		RejectOutputInputCollision(normalizedOutputPath, normalizedOverlayPath, "overlay resource file");
		RejectOutputInputCollision(normalizedOutputPath, normalizedManifestPath, "replacement manifest");

		SortedDictionary<string, RawResource> baseResources = ReadResources(normalizedBasePath, "base");
		SortedDictionary<string, RawResource> overlayResources = ReadResources(normalizedOverlayPath, "overlay");
		SortedSet<string> replacementKeys = ReadReplacementManifest(normalizedManifestPath);

		ValidateReplacementSet(baseResources, overlayResources, replacementKeys);

		SortedDictionary<string, RawResource> mergedResources = new(StringComparer.Ordinal);
		foreach ((string key, RawResource resource) in baseResources)
		{
			mergedResources.Add(key, resource);
		}

		foreach (string key in replacementKeys)
		{
			mergedResources[key] = overlayResources[key];
		}

		if (mergedResources.Count == 0)
		{
			throw new ResourceMergeException("The merged resource set is empty; refusing to create an empty output.");
		}

		WriteResources(normalizedOutputPath, mergedResources);

		return new ResourceMergeResult(
			baseResources.Count,
			overlayResources.Count,
			replacementKeys.Count,
			mergedResources.Count,
			replacementKeys.ToArray());
	}

	public static ResourceAuditResult Audit(
		string basePath,
		string overlayPath,
		string outputPath,
		string replacementManifestPath,
		int expectedResourceCount,
		int? expectedBamlResourceCount = null,
		string? assemblyPath = null,
		string assemblyResourceName = "pvfUtility.g.resources")
	{
		string normalizedBasePath = ValidateInputPath(basePath, "base resource file");
		string normalizedOverlayPath = ValidateInputPath(overlayPath, "overlay resource file");
		string normalizedOutputPath = ValidateInputPath(outputPath, "merged output resource file");
		string normalizedManifestPath = ValidateInputPath(replacementManifestPath, "replacement manifest");
		if (expectedResourceCount <= 0)
		{
			throw new ResourceMergeException("The expected resource count must be greater than zero.");
		}
		if (expectedBamlResourceCount is <= 0)
		{
			throw new ResourceMergeException("The expected BAML resource count must be greater than zero when specified.");
		}

		SortedDictionary<string, RawResource> baseResources = ReadResources(normalizedBasePath, "base");
		SortedDictionary<string, RawResource> overlayResources = ReadResources(normalizedOverlayPath, "overlay");
		SortedDictionary<string, RawResource> outputResources = ReadResources(normalizedOutputPath, "merged output");
		SortedSet<string> replacementKeys = ReadReplacementManifest(normalizedManifestPath);
		ValidateReplacementSet(baseResources, overlayResources, replacementKeys);

		if (baseResources.Count != expectedResourceCount)
		{
			throw new ResourceMergeException(
				$"The base resource file contains {baseResources.Count} entries; expected {expectedResourceCount}.");
		}

		if (outputResources.Count != expectedResourceCount)
		{
			throw new ResourceMergeException(
				$"The merged output resource file contains {outputResources.Count} entries; expected {expectedResourceCount}.");
		}

		int baseBamlResourceCount = baseResources.Keys.Count(
			key => key.EndsWith(".baml", StringComparison.OrdinalIgnoreCase));
		int outputBamlResourceCount = outputResources.Keys.Count(
			key => key.EndsWith(".baml", StringComparison.OrdinalIgnoreCase));
		if (expectedBamlResourceCount is int expectedBamlCount &&
			(baseBamlResourceCount != expectedBamlCount || outputBamlResourceCount != expectedBamlCount))
		{
			throw new ResourceMergeException(
				$"The base/merged BAML counts are {baseBamlResourceCount}/{outputBamlResourceCount}; expected {expectedBamlCount}/{expectedBamlCount}.");
		}

		List<string> changedKeys = [];
		foreach ((string key, RawResource baseResource) in baseResources)
		{
			if (!outputResources.TryGetValue(key, out RawResource? outputResource))
			{
				throw new ResourceMergeException($"The merged output is missing base resource '{key}'.");
			}

			if (replacementKeys.Contains(key))
			{
				if (!RawResourceEquals(overlayResources[key], outputResource))
				{
					throw new ResourceMergeException(
						$"Replacement resource '{key}' in the merged output does not match the generated overlay.");
				}

				if (RawResourceEquals(baseResource, outputResource))
				{
					throw new ResourceMergeException(
						$"Replacement resource '{key}' is byte-identical to the legacy resource; the migration did not replace it.");
				}

				changedKeys.Add(key);
			}
			else if (!RawResourceEquals(baseResource, outputResource))
			{
				throw new ResourceMergeException(
					$"Undeclared resource '{key}' differs between the legacy and merged containers.");
			}
		}

		foreach (string outputKey in outputResources.Keys)
		{
			if (!baseResources.ContainsKey(outputKey))
			{
				throw new ResourceMergeException($"The merged output contains unexpected resource '{outputKey}'.");
			}
		}

		if (!string.IsNullOrWhiteSpace(assemblyPath))
		{
			ValidateAssemblyResource(
				ValidateInputPath(assemblyPath, "application assembly"),
				normalizedOutputPath,
				assemblyResourceName);
		}

		return new ResourceAuditResult(
			baseResources.Count,
			overlayResources.Count,
			outputResources.Count,
			outputBamlResourceCount,
			replacementKeys.ToArray(),
			changedKeys,
			assemblyResourceName);
	}

	private static string ValidateInputPath(string path, string description)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			throw new ResourceMergeException($"The {description} path is required.");
		}

		string fullPath = Path.GetFullPath(path);
		if (!File.Exists(fullPath))
		{
			throw new ResourceMergeException($"The {description} does not exist: {fullPath}");
		}

		return fullPath;
	}

	private static string ValidateOutputPath(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			throw new ResourceMergeException("The output resource file path is required.");
		}

		return Path.GetFullPath(path);
	}

	private static void RejectOutputInputCollision(string outputPath, string inputPath, string description)
	{
		if (string.Equals(outputPath, inputPath, StringComparison.OrdinalIgnoreCase))
		{
			throw new ResourceMergeException($"The output path must not overwrite the {description}: {outputPath}");
		}
	}

	private static SortedDictionary<string, RawResource> ReadResources(string path, string description)
	{
		SortedDictionary<string, RawResource> resources = new(StringComparer.Ordinal);
		try
		{
			using ResourceReader reader = new(path);
			IDictionaryEnumerator enumerator = reader.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Key is not string key)
				{
					throw new ResourceMergeException($"The {description} resource file contains a non-string key.");
				}

				reader.GetResourceData(key, out string resourceTypeName, out byte[] resourceData);
				if (!resources.TryAdd(key, new RawResource(resourceTypeName, resourceData)))
				{
					throw new ResourceMergeException($"The {description} resource file contains the duplicate key '{key}'.");
				}
			}
		}
		catch (ResourceMergeException)
		{
			throw;
		}
		catch (Exception exception) when (exception is ArgumentException or BadImageFormatException or IOException or InvalidOperationException)
		{
			throw new ResourceMergeException($"Could not read the {description} resource file '{path}': {exception.Message}", exception);
		}

		return resources;
	}

	private static SortedSet<string> ReadReplacementManifest(string path)
	{
		SortedSet<string> replacementKeys = new(StringComparer.Ordinal);
		try
		{
			int lineNumber = 0;
			foreach (string line in File.ReadLines(path))
			{
				lineNumber++;
				string key = line.Trim();
				if (key.Length == 0)
				{
					continue;
				}

				if (!replacementKeys.Add(key))
				{
					throw new ResourceMergeException($"The replacement manifest contains duplicate key '{key}' at line {lineNumber}.");
				}
			}
		}
		catch (ResourceMergeException)
		{
			throw;
		}
		catch (IOException exception)
		{
			throw new ResourceMergeException($"Could not read replacement manifest '{path}': {exception.Message}", exception);
		}

		return replacementKeys;
	}

	private static void ValidateReplacementSet(
		IReadOnlyDictionary<string, RawResource> baseResources,
		IReadOnlyDictionary<string, RawResource> overlayResources,
		IReadOnlySet<string> replacementKeys)
	{
		foreach (string key in replacementKeys.Order(StringComparer.Ordinal))
		{
			if (!baseResources.ContainsKey(key))
			{
				throw new ResourceMergeException($"Replacement key '{key}' does not exist in the base resource file.");
			}

			if (!overlayResources.ContainsKey(key))
			{
				throw new ResourceMergeException($"Replacement key '{key}' is missing from the overlay resource file.");
			}
		}

		foreach (string key in overlayResources.Keys)
		{
			if (!replacementKeys.Contains(key))
			{
				string conflictDescription = baseResources.ContainsKey(key)
					? "an undeclared duplicate of a base resource"
					: "not declared in the replacement manifest";
				throw new ResourceMergeException($"Overlay key '{key}' is {conflictDescription}.");
			}
		}
	}

	private static void WriteResources(string outputPath, IReadOnlyDictionary<string, RawResource> resources)
	{
		string? outputDirectory = Path.GetDirectoryName(outputPath);
		if (!string.IsNullOrEmpty(outputDirectory))
		{
			Directory.CreateDirectory(outputDirectory);
		}

		string temporaryPath = Path.Combine(
			outputDirectory ?? Directory.GetCurrentDirectory(),
			$".{Path.GetFileName(outputPath)}.{Guid.NewGuid():N}.tmp");

		try
		{
			using (ResourceWriter writer = new(temporaryPath))
			{
				foreach ((string key, RawResource resource) in resources.OrderBy(pair => pair.Key, StringComparer.Ordinal))
				{
					writer.AddResourceData(key, resource.TypeName, resource.Data);
				}
				writer.Generate();
			}

			FileInfo temporaryFile = new(temporaryPath);
			if (!temporaryFile.Exists || temporaryFile.Length == 0)
			{
				throw new ResourceMergeException("The resource writer produced an empty output file.");
			}

			File.Move(temporaryPath, outputPath, overwrite: true);
		}
		catch (ResourceMergeException)
		{
			throw;
		}
		catch (Exception exception) when (exception is ArgumentException or IOException or InvalidOperationException)
		{
			throw new ResourceMergeException($"Could not write merged resource file '{outputPath}': {exception.Message}", exception);
		}
		finally
		{
			if (File.Exists(temporaryPath))
			{
				File.Delete(temporaryPath);
			}
		}
	}

	private static bool RawResourceEquals(RawResource left, RawResource right)
	{
		return string.Equals(left.TypeName, right.TypeName, StringComparison.Ordinal) &&
			left.Data.AsSpan().SequenceEqual(right.Data);
	}

	private static void ValidateAssemblyResource(
		string assemblyPath,
		string expectedResourcePath,
		string resourceName)
	{
		if (string.IsNullOrWhiteSpace(resourceName))
		{
			throw new ResourceMergeException("The assembly resource name is required.");
		}

		try
		{
			Assembly assembly = Assembly.LoadFile(assemblyPath);
			string[] matchingNames = assembly.GetManifestResourceNames()
				.Where(name => string.Equals(name, resourceName, StringComparison.Ordinal))
				.ToArray();
			if (matchingNames.Length != 1)
			{
				throw new ResourceMergeException(
					$"The application assembly must contain exactly one '{resourceName}' resource; found {matchingNames.Length}.");
			}

			using Stream stream = assembly.GetManifestResourceStream(resourceName)
				?? throw new ResourceMergeException(
					$"The application assembly resource '{resourceName}' could not be opened.");
			using MemoryStream embeddedResource = new();
			stream.CopyTo(embeddedResource);
			byte[] expectedResource = File.ReadAllBytes(expectedResourcePath);
			if (!embeddedResource.GetBuffer().AsSpan(0, checked((int)embeddedResource.Length)).SequenceEqual(expectedResource))
			{
				throw new ResourceMergeException(
					$"The application assembly resource '{resourceName}' does not match the audited merged container.");
			}
		}
		catch (ResourceMergeException)
		{
			throw;
		}
		catch (Exception exception) when (exception is BadImageFormatException or IOException or InvalidOperationException)
		{
			throw new ResourceMergeException(
				$"Could not audit application assembly '{assemblyPath}': {exception.Message}",
				exception);
		}
	}

	private sealed record RawResource(string TypeName, byte[] Data);
}

public static class PvfResourceMergerCli
{
	private static readonly string[] MergeRequiredOptions =
	[
		"--base",
		"--overlay",
		"--output",
		"--replacement-manifest"
	];

	private static readonly string[] AuditRequiredOptions =
	[
		"--base",
		"--overlay",
		"--output",
		"--replacement-manifest",
		"--expected-count",
		"--expected-baml-count",
		"--assembly",
		"--resource-name"
	];

	public static int Run(string[] args, TextWriter standardOutput, TextWriter standardError)
	{
		try
		{
			if (args.Length > 0 && string.Equals(args[0], "audit", StringComparison.Ordinal))
			{
				return RunAudit(args[1..], standardOutput);
			}

			IReadOnlyDictionary<string, string> options = ParseOptions(args, MergeRequiredOptions);
			ResourceMergeResult result = ResourceMerger.Merge(
				options["--base"],
				options["--overlay"],
				options["--output"],
				options["--replacement-manifest"]);

			standardOutput.WriteLine($"Base resources: {result.BaseResourceCount}");
			standardOutput.WriteLine($"Overlay resources: {result.OverlayResourceCount}");
			standardOutput.WriteLine($"Replacement resources: {result.ReplacementCount}");
			foreach (string key in result.ReplacementKeys)
			{
				standardOutput.WriteLine($"Replaced: {key}");
			}
			standardOutput.WriteLine($"Output resources: {result.OutputResourceCount}");
			return 0;
		}
		catch (ArgumentException exception)
		{
			standardError.WriteLine($"Argument error: {exception.Message}");
			WriteUsage(standardError);
			return 2;
		}
		catch (ResourceMergeException exception)
		{
			standardError.WriteLine($"Resource merge failed: {exception.Message}");
			return 1;
		}
	}

	private static int RunAudit(string[] args, TextWriter standardOutput)
	{
		IReadOnlyDictionary<string, string> options = ParseOptions(args, AuditRequiredOptions);
		if (!int.TryParse(options["--expected-count"], out int expectedCount) || expectedCount <= 0)
		{
			throw new ArgumentException("Option '--expected-count' must be a positive integer.");
		}
		if (!int.TryParse(options["--expected-baml-count"], out int expectedBamlCount) || expectedBamlCount <= 0)
		{
			throw new ArgumentException("Option '--expected-baml-count' must be a positive integer.");
		}

		ResourceAuditResult result = ResourceMerger.Audit(
			options["--base"],
			options["--overlay"],
			options["--output"],
			options["--replacement-manifest"],
			expectedCount,
			expectedBamlCount,
			options["--assembly"],
			options["--resource-name"]);
		standardOutput.WriteLine($"Audited resources: {result.OutputResourceCount}");
		standardOutput.WriteLine($"Audited BAML resources: {result.BamlResourceCount}");
		foreach (string key in result.ChangedKeys)
		{
			standardOutput.WriteLine($"Verified replacement: {key}");
		}
		standardOutput.WriteLine($"Verified assembly resource: {result.AssemblyResourceName}");
		return 0;
	}

	private static IReadOnlyDictionary<string, string> ParseOptions(
		string[] args,
		IReadOnlyCollection<string> requiredOptions)
	{
		Dictionary<string, string> options = new(StringComparer.Ordinal);
		for (int index = 0; index < args.Length; index++)
		{
			string option = args[index];
			if (!requiredOptions.Contains(option, StringComparer.Ordinal))
			{
				throw new ArgumentException($"Unknown option '{option}'.");
			}

			if (!options.TryAdd(option, string.Empty))
			{
				throw new ArgumentException($"Option '{option}' was specified more than once.");
			}

			if (++index >= args.Length || args[index].StartsWith("--", StringComparison.Ordinal))
			{
				throw new ArgumentException($"Option '{option}' requires a value.");
			}

			options[option] = args[index];
		}

		foreach (string requiredOption in requiredOptions)
		{
			if (!options.ContainsKey(requiredOption))
			{
				throw new ArgumentException($"Required option '{requiredOption}' was not specified.");
			}
		}

		return options;
	}

	private static void WriteUsage(TextWriter writer)
	{
		writer.WriteLine("Usage: PvfResourceMerger --base <file> --overlay <file> --output <file> --replacement-manifest <file>");
		writer.WriteLine("       PvfResourceMerger audit --base <file> --overlay <file> --output <file> --replacement-manifest <file> --expected-count <count> --expected-baml-count <count> --assembly <file> --resource-name <name>");
	}
}
