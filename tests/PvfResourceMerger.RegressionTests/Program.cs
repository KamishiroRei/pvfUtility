using PvfResourceMerger;
using System.Collections;
using System.Resources;
using System.Security.Cryptography;

List<(string Name, Action Test)> tests =
[
	("declared replacement succeeds and preserves untouched raw data", DeclaredReplacementSucceeds),
	("missing overlay replacement fails", MissingOverlayReplacementFails),
	("replacement absent from base fails", ReplacementAbsentFromBaseFails),
	("undeclared overlay conflict fails", UndeclaredOverlayConflictFails),
	("unexpected overlay entry fails", UnexpectedOverlayEntryFails),
	("duplicate manifest key fails", DuplicateManifestKeyFails),
	("empty merged output fails", EmptyMergedOutputFails),
	("repeated merges are byte deterministic", RepeatedMergesAreDeterministic),
	("audit verifies only declared resources changed", AuditVerifiesDeclaredChanges),
	("audit rejects an undeclared output mutation", AuditRejectsUndeclaredMutation),
	("CLI merges through the documented options", CliMergesThroughDocumentedOptions),
	("CLI validates required arguments", CliValidatesRequiredArguments)
];

foreach ((string name, Action test) in tests)
{
	test();
	Console.WriteLine($"PASS: {name}");
}

Console.WriteLine($"PvfResourceMerger regression tests passed ({tests.Count} scenarios).");

static void DeclaredReplacementSucceeds()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string outputPath = directory.PathFor("nested", "merged.resources");
	string manifestPath = directory.PathFor("replacements.txt");

	WriteResources(basePath,
		("z-untouched", new byte[] { 1, 2, 3, 4 }),
		("app.baml", new byte[] { 10, 20, 30 }),
		("a-untouched", "legacy text"));
	WriteResources(overlayPath, ("app.baml", new byte[] { 99, 88, 77, 66 }));
	File.WriteAllText(manifestPath, "app.baml\n");

	IReadOnlyDictionary<string, RawResource> baseRaw = ReadRawResources(basePath);
	IReadOnlyDictionary<string, RawResource> overlayRaw = ReadRawResources(overlayPath);
	ResourceMergeResult result = ResourceMerger.Merge(basePath, overlayPath, outputPath, manifestPath);
	IReadOnlyDictionary<string, RawResource> outputRaw = ReadRawResources(outputPath);

	AssertEqual(3, result.BaseResourceCount, "base resource count");
	AssertEqual(1, result.OverlayResourceCount, "overlay resource count");
	AssertEqual(1, result.ReplacementCount, "replacement count");
	AssertEqual(3, result.OutputResourceCount, "output resource count");
	AssertSequenceEqual(["app.baml"], result.ReplacementKeys, "reported replacement keys");
	AssertRawEqual(baseRaw["a-untouched"], outputRaw["a-untouched"], "untouched string resource");
	AssertRawEqual(baseRaw["z-untouched"], outputRaw["z-untouched"], "untouched byte resource");
	AssertRawEqual(overlayRaw["app.baml"], outputRaw["app.baml"], "replacement resource");
}

static void MissingOverlayReplacementFails()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string manifestPath = directory.PathFor("replacements.txt");
	WriteResources(basePath, ("app.baml", new byte[] { 1 }));
	WriteResources(overlayPath);
	File.WriteAllText(manifestPath, "app.baml\n");

	AssertMergeFails(
		() => ResourceMerger.Merge(basePath, overlayPath, directory.PathFor("out.resources"), manifestPath),
		"missing from the overlay");
}

static void ReplacementAbsentFromBaseFails()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string manifestPath = directory.PathFor("replacements.txt");
	WriteResources(basePath, ("existing.baml", new byte[] { 1 }));
	WriteResources(overlayPath, ("new.baml", new byte[] { 2 }));
	File.WriteAllText(manifestPath, "new.baml\n");

	AssertMergeFails(
		() => ResourceMerger.Merge(basePath, overlayPath, directory.PathFor("out.resources"), manifestPath),
		"does not exist in the base");
}

static void UndeclaredOverlayConflictFails()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string manifestPath = directory.PathFor("replacements.txt");
	WriteResources(basePath, ("app.baml", new byte[] { 1 }));
	WriteResources(overlayPath, ("app.baml", new byte[] { 2 }));
	File.WriteAllText(manifestPath, string.Empty);

	AssertMergeFails(
		() => ResourceMerger.Merge(basePath, overlayPath, directory.PathFor("out.resources"), manifestPath),
		"undeclared duplicate");
}

static void UnexpectedOverlayEntryFails()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string manifestPath = directory.PathFor("replacements.txt");
	WriteResources(basePath, ("app.baml", new byte[] { 1 }));
	WriteResources(overlayPath, ("new-resource.baml", new byte[] { 2 }));
	File.WriteAllText(manifestPath, string.Empty);

	AssertMergeFails(
		() => ResourceMerger.Merge(basePath, overlayPath, directory.PathFor("out.resources"), manifestPath),
		"not declared in the replacement manifest");
}

static void DuplicateManifestKeyFails()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string manifestPath = directory.PathFor("replacements.txt");
	WriteResources(basePath, ("app.baml", new byte[] { 1 }));
	WriteResources(overlayPath, ("app.baml", new byte[] { 2 }));
	File.WriteAllText(manifestPath, "app.baml\napp.baml\n");

	AssertMergeFails(
		() => ResourceMerger.Merge(basePath, overlayPath, directory.PathFor("out.resources"), manifestPath),
		"duplicate key");
}

static void EmptyMergedOutputFails()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string manifestPath = directory.PathFor("replacements.txt");
	WriteResources(basePath);
	WriteResources(overlayPath);
	File.WriteAllText(manifestPath, string.Empty);

	AssertMergeFails(
		() => ResourceMerger.Merge(basePath, overlayPath, directory.PathFor("out.resources"), manifestPath),
		"merged resource set is empty");
}

static void RepeatedMergesAreDeterministic()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string manifestPath = directory.PathFor("replacements.txt");
	string firstOutputPath = directory.PathFor("first.resources");
	string secondOutputPath = directory.PathFor("second.resources");
	WriteResources(basePath,
		("middle", new byte[] { 3 }),
		("z-last", "last"),
		("a-first", new byte[] { 1 }),
		("replace-me", new byte[] { 4 }));
	WriteResources(overlayPath, ("replace-me", new byte[] { 8, 9 }));
	File.WriteAllText(manifestPath, "replace-me\n");

	ResourceMerger.Merge(basePath, overlayPath, firstOutputPath, manifestPath);
	ResourceMerger.Merge(basePath, overlayPath, secondOutputPath, manifestPath);

	byte[] firstOutput = File.ReadAllBytes(firstOutputPath);
	byte[] secondOutput = File.ReadAllBytes(secondOutputPath);
	AssertTrue(firstOutput.SequenceEqual(secondOutput),
		$"deterministic bytes (first SHA-256 {Hash(firstOutput)}, second SHA-256 {Hash(secondOutput)})");
}

static void AuditVerifiesDeclaredChanges()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string outputPath = directory.PathFor("merged.resources");
	string manifestPath = directory.PathFor("replacements.txt");
	WriteResources(basePath,
		("app.baml", new byte[] { 1 }),
		("views/editor.baml", new byte[] { 2 }),
		("untouched.baml", new byte[] { 3 }));
	WriteResources(overlayPath,
		("app.baml", new byte[] { 10 }),
		("views/editor.baml", new byte[] { 20 }));
	File.WriteAllText(manifestPath, "app.baml\nviews/editor.baml\n");
	ResourceMerger.Merge(basePath, overlayPath, outputPath, manifestPath);

	ResourceAuditResult result = ResourceMerger.Audit(
		basePath,
		overlayPath,
		outputPath,
		manifestPath,
		expectedResourceCount: 3);

	AssertEqual(3, result.OutputResourceCount, "audited output resource count");
	AssertSequenceEqual(
		["app.baml", "views/editor.baml"],
		result.ChangedKeys,
		"audited replacement keys");
}

static void AuditRejectsUndeclaredMutation()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string outputPath = directory.PathFor("merged.resources");
	string manifestPath = directory.PathFor("replacements.txt");
	WriteResources(basePath,
		("app.baml", new byte[] { 1 }),
		("untouched.baml", new byte[] { 2 }));
	WriteResources(overlayPath, ("app.baml", new byte[] { 10 }));
	WriteResources(outputPath,
		("app.baml", new byte[] { 10 }),
		("untouched.baml", new byte[] { 99 }));
	File.WriteAllText(manifestPath, "app.baml\n");

	AssertMergeFails(
		() => ResourceMerger.Audit(
			basePath,
			overlayPath,
			outputPath,
			manifestPath,
			expectedResourceCount: 2),
		"Undeclared resource");
}

static void CliValidatesRequiredArguments()
{
	using StringWriter standardOutput = new();
	using StringWriter standardError = new();
	int exitCode = PvfResourceMergerCli.Run([], standardOutput, standardError);
	AssertEqual(2, exitCode, "CLI argument error exit code");
	AssertTrue(standardError.ToString().Contains("--base", StringComparison.Ordinal), "CLI names missing required option");
	AssertTrue(standardError.ToString().Contains("Usage:", StringComparison.Ordinal), "CLI prints usage");
}

static void CliMergesThroughDocumentedOptions()
{
	using TestDirectory directory = new();
	string basePath = directory.PathFor("base.resources");
	string overlayPath = directory.PathFor("overlay.resources");
	string outputPath = directory.PathFor("merged.resources");
	string manifestPath = directory.PathFor("replacements.txt");
	WriteResources(basePath, ("app.baml", new byte[] { 1 }), ("untouched", "legacy"));
	WriteResources(overlayPath, ("app.baml", new byte[] { 2 }));
	File.WriteAllText(manifestPath, "app.baml\n");

	using StringWriter standardOutput = new();
	using StringWriter standardError = new();
	int exitCode = PvfResourceMergerCli.Run(
	[
		"--overlay", overlayPath,
		"--replacement-manifest", manifestPath,
		"--base", basePath,
		"--output", outputPath
	], standardOutput, standardError);

	AssertEqual(0, exitCode, "CLI success exit code");
	AssertTrue(File.Exists(outputPath), "CLI creates output file");
	AssertTrue(standardOutput.ToString().Contains("Replaced: app.baml", StringComparison.Ordinal), "CLI audits replacement key");
	AssertEqual(string.Empty, standardError.ToString(), "CLI success error output");
}

static void WriteResources(string path, params (string Key, object Value)[] resources)
{
	using ResourceWriter writer = new(path);
	foreach ((string key, object value) in resources)
	{
		writer.AddResource(key, value);
	}
	writer.Generate();
}

static IReadOnlyDictionary<string, RawResource> ReadRawResources(string path)
{
	Dictionary<string, RawResource> resources = new(StringComparer.Ordinal);
	using ResourceReader reader = new(path);
	IDictionaryEnumerator enumerator = reader.GetEnumerator();
	while (enumerator.MoveNext())
	{
		string key = (string)enumerator.Key;
		reader.GetResourceData(key, out string typeName, out byte[] data);
		resources.Add(key, new RawResource(typeName, data));
	}
	return resources;
}

static void AssertMergeFails(Action action, string expectedMessageFragment)
{
	try
	{
		action();
	}
	catch (ResourceMergeException exception)
	{
		AssertTrue(
			exception.Message.Contains(expectedMessageFragment, StringComparison.OrdinalIgnoreCase),
			$"failure message contains '{expectedMessageFragment}', actual: '{exception.Message}'");
		return;
	}

	throw new InvalidOperationException($"Expected ResourceMergeException containing '{expectedMessageFragment}'.");
}

static void AssertRawEqual(RawResource expected, RawResource actual, string scenario)
{
	AssertEqual(expected.TypeName, actual.TypeName, $"{scenario} type name");
	AssertTrue(expected.Data.SequenceEqual(actual.Data), $"{scenario} raw bytes");
}

static void AssertEqual<T>(T expected, T actual, string scenario)
	where T : IEquatable<T>
{
	if (!expected.Equals(actual))
	{
		throw new InvalidOperationException($"{scenario}: expected '{expected}', got '{actual}'.");
	}
}

static void AssertSequenceEqual(IReadOnlyList<string> expected, IReadOnlyList<string> actual, string scenario)
{
	if (!expected.SequenceEqual(actual, StringComparer.Ordinal))
	{
		throw new InvalidOperationException($"{scenario}: expected [{string.Join(", ", expected)}], got [{string.Join(", ", actual)}].");
	}
}

static void AssertTrue(bool condition, string scenario)
{
	if (!condition)
	{
		throw new InvalidOperationException($"Assertion failed: {scenario}.");
	}
}

static string Hash(byte[] data) => Convert.ToHexString(SHA256.HashData(data));

internal sealed record RawResource(string TypeName, byte[] Data);

internal sealed class TestDirectory : IDisposable
{
	private readonly string _path = System.IO.Path.Combine(
		System.IO.Path.GetTempPath(),
		"PvfResourceMerger-tests-" + Guid.NewGuid().ToString("N"));

	public TestDirectory()
	{
		Directory.CreateDirectory(_path);
	}

	public string PathFor(params string[] parts)
	{
		string path = parts.Aggregate(_path, System.IO.Path.Combine);
		string? directory = System.IO.Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}
		return path;
	}

	public void Dispose()
	{
		if (Directory.Exists(_path))
		{
			Directory.Delete(_path, recursive: true);
		}
	}
}
