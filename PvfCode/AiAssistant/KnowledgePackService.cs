using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PvfCode.AiAssistant;

public sealed record KnowledgeSearchResult(string Path, string Content, int Score, bool Truncated);

public sealed class KnowledgePackService
{
	private const int MaximumDocumentChars = 12000;

	private readonly string _rootDirectory;

	private readonly string _rootPrefix;

	private readonly bool _allowUnindexedFallback;

	private readonly SemaphoreSlim _loadLock = new(1, 1);

	private IReadOnlyList<KnowledgeDocument>? _documents;

	private IReadOnlyList<KnowledgeRoute> _routes = Array.Empty<KnowledgeRoute>();

	private KnowledgeIndexState _indexState;

	public KnowledgePackService(string? rootDirectory = null, bool allowUnindexedFallback = false)
	{
		_rootDirectory = Path.GetFullPath(rootDirectory ?? Path.Combine(AppContext.BaseDirectory, "AgentKnowledge"));
		_rootPrefix = Path.TrimEndingDirectorySeparator(_rootDirectory) + Path.DirectorySeparatorChar;
		_allowUnindexedFallback = allowUnindexedFallback;
	}

	public bool IsAvailable => Directory.Exists(_rootDirectory);

	public async Task<IReadOnlyList<KnowledgeSearchResult>> SearchAsync(
		string query,
		int limit = 3,
		CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(query))
		{
			throw new ArgumentException("知识检索词不能为空。", nameof(query));
		}
		limit = Math.Clamp(limit, 1, 5);
		IReadOnlyList<KnowledgeDocument> documents = await GetDocumentsAsync(cancellationToken).ConfigureAwait(false);
		string normalizedQuery = Normalize(query);
		string[] terms = BuildTerms(normalizedQuery);

		IEnumerable<KnowledgeDocument> candidates = SelectRoutedDocuments(documents, normalizedQuery, terms);
		var ranked = candidates
			.Select(document => new { Document = document, Score = Score(document, normalizedQuery, terms) })
			.Where(item => item.Score > 0)
			.OrderByDescending(item => item.Score)
			.ThenBy(item => item.Document.RelativePath, StringComparer.Ordinal)
			.Take(limit)
			.ToArray();
		List<KnowledgeSearchResult> results = new();
		int remainingChars = 30000;
		foreach (var item in ranked)
		{
			if (remainingChars <= 0)
			{
				break;
			}
			int maximumChars = Math.Min(MaximumDocumentChars, remainingChars);
			bool truncated = item.Document.Content.Length > maximumChars;
			string content = truncated ? item.Document.Content[..maximumChars] : item.Document.Content;
			results.Add(new KnowledgeSearchResult(item.Document.RelativePath, content, item.Score, truncated));
			remainingChars -= content.Length;
		}
		return results;
	}

	private async Task<IReadOnlyList<KnowledgeDocument>> GetDocumentsAsync(CancellationToken cancellationToken)
	{
		if (_documents != null)
		{
			return _documents;
		}

		await _loadLock.WaitAsync(cancellationToken).ConfigureAwait(false);
		try
		{
			if (_documents != null)
			{
				return _documents;
			}
			if (!Directory.Exists(_rootDirectory))
			{
				_indexState = KnowledgeIndexState.Missing;
				_documents = Array.Empty<KnowledgeDocument>();
				return _documents;
			}

			List<KnowledgeDocument> loaded = new();
			foreach (string path in EnumerateKnowledgeFiles(cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();
				string extension = Path.GetExtension(path);
				if (!extension.Equals(".md", StringComparison.OrdinalIgnoreCase) &&
					!extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				string content = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
				string relativePath = Path.GetRelativePath(_rootDirectory, path).Replace('\\', '/');
				loaded.Add(new KnowledgeDocument(relativePath, content, Normalize(relativePath), Normalize(content)));
			}
			_documents = loaded;
			LoadRoutes(loaded);
			return _documents;
		}
		finally
		{
			_loadLock.Release();
		}
	}

	private IEnumerable<string> EnumerateKnowledgeFiles(CancellationToken cancellationToken)
	{
		DirectoryInfo root = new(_rootDirectory);
		if ((root.Attributes & FileAttributes.ReparsePoint) != 0)
		{
			yield break;
		}

		Stack<DirectoryInfo> pending = new();
		pending.Push(root);
		while (pending.Count > 0)
		{
			cancellationToken.ThrowIfCancellationRequested();
			DirectoryInfo directory = pending.Pop();
			foreach (FileSystemInfo entry in directory.EnumerateFileSystemInfos())
			{
				cancellationToken.ThrowIfCancellationRequested();
				if ((entry.Attributes & FileAttributes.ReparsePoint) != 0)
				{
					continue;
				}
				if (entry is DirectoryInfo childDirectory)
				{
					pending.Push(childDirectory);
					continue;
				}
				if (entry is FileInfo file && IsWithinRoot(file.FullName))
				{
					yield return file.FullName;
				}
			}
		}
	}

	private bool IsWithinRoot(string path)
	{
		return Path.GetFullPath(path).StartsWith(_rootPrefix, StringComparison.OrdinalIgnoreCase);
	}

	private IEnumerable<KnowledgeDocument> SelectRoutedDocuments(
		IReadOnlyList<KnowledgeDocument> documents,
		string query,
		IReadOnlyList<string> terms)
	{
		if (_routes.Count == 0)
		{
			if (_allowUnindexedFallback && _indexState == KnowledgeIndexState.Missing)
			{
				return documents;
			}
			return documents.Where(document =>
				document.RelativePath.Equals("safety/README.zh-CN.md", StringComparison.OrdinalIgnoreCase));
		}

		Dictionary<string, KnowledgeDocument> byPath = documents.ToDictionary(
			document => document.RelativePath,
			StringComparer.OrdinalIgnoreCase);
		var rankedRoutes = _routes
			.Where(route => !route.Topic.Equals("all-legacy-topics", StringComparison.OrdinalIgnoreCase))
			.Select(route => new
			{
				Route = route,
				Score = ScoreRoute(route, byPath, query, terms)
			})
			.Where(item => item.Score > 0)
			.OrderByDescending(item => item.Score)
			.ThenBy(item => item.Route.Topic, StringComparer.Ordinal)
			.Take(2)
			.ToArray();

		HashSet<string> paths = new(StringComparer.OrdinalIgnoreCase)
		{
			"safety/README.zh-CN.md"
		};
		if (rankedRoutes.Length == 0)
		{
			paths.Add("indexes/knowledge-index.json");
		}
		else
		{
			foreach (var item in rankedRoutes)
			{
				paths.UnionWith(item.Route.Entries);
			}
		}
		return paths.Select(path => byPath.TryGetValue(path, out KnowledgeDocument? document) ? document : null)
			.Where(document => document != null)!;
	}

	private static int ScoreRoute(
		KnowledgeRoute route,
		IReadOnlyDictionary<string, KnowledgeDocument> documents,
		string query,
		IReadOnlyList<string> terms)
	{
		string normalizedTopic = Normalize(route.Topic);
		int score = normalizedTopic.Contains(query, StringComparison.Ordinal) ? 160 : 0;
		foreach (string term in terms)
		{
			if (normalizedTopic.Contains(term, StringComparison.Ordinal))
			{
				score += 36;
			}
		}
		score += route.Entries
			.Where(documents.ContainsKey)
			.Select(path => Score(documents[path], query, terms))
			.OrderByDescending(value => value)
			.Take(3)
			.Sum();
		return score;
	}

	private void LoadRoutes(IReadOnlyList<KnowledgeDocument> documents)
	{
		KnowledgeDocument? index = documents.FirstOrDefault(document =>
			document.RelativePath.Equals("indexes/knowledge-index.json", StringComparison.OrdinalIgnoreCase));
		if (index == null)
		{
			_indexState = KnowledgeIndexState.Missing;
			_routes = Array.Empty<KnowledgeRoute>();
			return;
		}
		try
		{
			using JsonDocument json = JsonDocument.Parse(index.Content);
			if (!json.RootElement.TryGetProperty("topics", out JsonElement topics) || topics.ValueKind != JsonValueKind.Object)
			{
				_indexState = KnowledgeIndexState.Invalid;
				_routes = Array.Empty<KnowledgeRoute>();
				return;
			}
			List<KnowledgeRoute> routes = new();
			foreach (JsonProperty topic in topics.EnumerateObject())
			{
				if (!topic.Value.TryGetProperty("entries", out JsonElement entries) || entries.ValueKind != JsonValueKind.Array)
				{
					continue;
				}
				string[] paths = entries.EnumerateArray()
					.Where(entry => entry.ValueKind == JsonValueKind.String)
					.Select(entry => entry.GetString())
					.Where(path => !string.IsNullOrWhiteSpace(path))
					.Cast<string>()
					.ToArray();
				routes.Add(new KnowledgeRoute(topic.Name, paths));
			}
			_routes = routes;
			_indexState = routes.Count == 0 ? KnowledgeIndexState.Invalid : KnowledgeIndexState.Valid;
		}
		catch (Exception exception) when (exception is JsonException or InvalidOperationException)
		{
			_indexState = KnowledgeIndexState.Invalid;
			_routes = Array.Empty<KnowledgeRoute>();
		}
	}

	private static int Score(KnowledgeDocument document, string query, IReadOnlyList<string> terms)
	{
		int score = 0;
		if (document.NormalizedPath.Contains(query, StringComparison.Ordinal))
		{
			score += 120;
		}
		if (document.NormalizedContent.Contains(query, StringComparison.Ordinal))
		{
			score += 60;
		}
		foreach (string term in terms)
		{
			if (document.NormalizedPath.Contains(term, StringComparison.Ordinal))
			{
				score += 24;
			}
			if (document.NormalizedContent.Contains(term, StringComparison.Ordinal))
			{
				score += 5;
			}
		}
		if (document.RelativePath.Equals("safety/README.zh-CN.md", StringComparison.OrdinalIgnoreCase))
		{
			score += 1;
		}
		return score;
	}

	private static string[] BuildTerms(string query)
	{
		HashSet<string> terms = new(StringComparer.Ordinal);
		foreach (string token in query.Split(new[] { ' ', '\t', '\r', '\n', '/', '\\', '-', '_', '.', ',', '，', '。', '：', ':' }, StringSplitOptions.RemoveEmptyEntries))
		{
			if (token.Length >= 2)
			{
				terms.Add(token);
			}
			if (ContainsCjk(token) && token.Length > 2)
			{
				for (int index = 0; index < token.Length - 1; index++)
				{
					terms.Add(token.Substring(index, 2));
				}
			}
		}
		return terms.ToArray();
	}

	private static bool ContainsCjk(string value)
	{
		return value.Any(character => character is >= '\u4e00' and <= '\u9fff');
	}

	private static string Normalize(string value)
	{
		StringBuilder builder = new(value.Length);
		foreach (char character in value)
		{
			builder.Append(char.ToLowerInvariant(character));
		}
		return builder.ToString();
	}

	private sealed record KnowledgeDocument(
		string RelativePath,
		string Content,
		string NormalizedPath,
		string NormalizedContent);

	private sealed record KnowledgeRoute(string Topic, IReadOnlyList<string> Entries);

	private enum KnowledgeIndexState
	{
		Unknown,
		Missing,
		Invalid,
		Valid
	}
}
