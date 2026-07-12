using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.AiAssistant;

public static class PvfAssistantToolCatalog
{
	private const int MaximumKnowledgeQueryChars = 1000;

	private const int MaximumPvfPathChars = 1024;

	private const int MaximumSearchQueryChars = 512;

	private const int MaximumPvfTextSourceBytes = 16 * 1024 * 1024;

	private const long MaximumContentSearchSourceBytes = 64L * 1024 * 1024;

	public static IReadOnlyList<IAiAssistantTool> Create(
		PvfGroup pvf,
		KnowledgePackService knowledgePack,
		string? currentDocumentPath,
		bool allowPvfRead)
	{
		ArgumentNullException.ThrowIfNull(pvf);
		ArgumentNullException.ThrowIfNull(knowledgePack);
		PvfReadSession? session = allowPvfRead ? PvfReadSession.TryCreate(pvf) : null;

		List<IAiAssistantTool> tools = new()
		{
			CreateKnowledgeSearchTool(knowledgePack)
		};
		if (session == null)
		{
			return tools;
		}

		tools.Add(CreateCurrentContextTool(session, currentDocumentPath));
		tools.Add(CreateListFilesTool(session));
		tools.Add(CreateSearchTool(session));
		tools.Add(CreateReadFileTool(session));
		tools.Add(CreateListRegistriesTool(session));
		tools.Add(CreateResolveLstTool(session));
		return tools;
	}

	private static IAiAssistantTool CreateKnowledgeSearchTool(KnowledgePackService knowledgePack)
	{
		return new AiAssistantTool(
			"knowledge_search",
			"Search the bundled, curated DNF PVF knowledge pack. Use this before making domain-specific claims.",
			"""
			{"type":"object","additionalProperties":false,"required":["query"],"properties":{"query":{"type":"string","maxLength":1000},"limit":{"type":"integer","minimum":1,"maximum":5}}}
			""",
			async (arguments, cancellationToken) =>
			{
				string query = GetRequiredString(arguments, "query", MaximumKnowledgeQueryChars);
				int limit = GetOptionalInt(arguments, "limit", 3, 1, 5);
				IReadOnlyList<KnowledgeSearchResult> results = await knowledgePack
					.SearchAsync(query, limit, cancellationToken)
					.ConfigureAwait(false);
				return new { available = knowledgePack.IsAvailable, query, results };
			});
	}

	private static IAiAssistantTool CreateCurrentContextTool(PvfReadSession session, string? currentDocumentPath)
	{
		return new AiAssistantTool(
			"pvf_current_context",
			"Return read-only metadata for the PVF currently open in pvfUtility and the active logical document path.",
			"{\"type\":\"object\",\"additionalProperties\":false,\"properties\":{}}",
			(_, _) =>
			{
				session.EnsureCurrent();
				return Task.FromResult<object?>(new
				{
					isOpen = true,
					pvfName = session.PvfName,
					fileCount = session.Files.Count,
					activeDocument = string.Equals(currentDocumentPath, "chatGPT", StringComparison.OrdinalIgnoreCase) ? null : currentDocumentPath
				});
			});
	}

	private static IAiAssistantTool CreateListFilesTool(PvfReadSession session)
	{
		return new AiAssistantTool(
			"pvf_list_files",
			"List logical paths from the currently open PVF. Results are read-only and bounded.",
			"""
			{"type":"object","additionalProperties":false,"properties":{"prefix":{"type":"string","maxLength":1024},"contains":{"type":"string","maxLength":1024},"limit":{"type":"integer","minimum":1,"maximum":200}}}
			""",
			(arguments, cancellationToken) => Task.Run<object?>(() =>
			{
				session.EnsureCurrent();
				IReadOnlyDictionary<string, PvfFile> files = session.Files;
				string prefix = GetOptionalString(arguments, "prefix", MaximumPvfPathChars);
				string contains = GetOptionalString(arguments, "contains", MaximumPvfPathChars);
				int limit = GetOptionalInt(arguments, "limit", 100, 1, 200);
				IEnumerable<string> query = files.Keys;
				if (!string.IsNullOrWhiteSpace(prefix))
				{
					query = query.Where(path => path.StartsWith(NormalizePvfPath(prefix), StringComparison.OrdinalIgnoreCase));
				}
				if (!string.IsNullOrWhiteSpace(contains))
				{
					query = query.Where(path => path.Contains(contains, StringComparison.OrdinalIgnoreCase));
				}
				string[] matches = query.OrderBy(path => path, StringComparer.OrdinalIgnoreCase).Take(limit + 1).ToArray();
				cancellationToken.ThrowIfCancellationRequested();
				return new { paths = matches.Take(limit).ToArray(), truncated = matches.Length > limit, limit };
			}, cancellationToken));
	}

	private static IAiAssistantTool CreateSearchTool(PvfReadSession session)
	{
		return new AiAssistantTool(
			"pvf_search",
			"Search logical paths or decompiled text in the currently open PVF. Content search is bounded and zero results are not proof of absence.",
			"""
			{"type":"object","additionalProperties":false,"required":["query"],"properties":{"query":{"type":"string","maxLength":512},"mode":{"type":"string","enum":["path","content"]},"prefix":{"type":"string","maxLength":1024},"limit":{"type":"integer","minimum":1,"maximum":50}}}
			""",
			(arguments, cancellationToken) => Task.Run<object?>(() =>
			{
				session.EnsureCurrent();
				IReadOnlyDictionary<string, PvfFile> files = session.Files;
				string searchText = GetRequiredString(arguments, "query", MaximumSearchQueryChars);
				string mode = GetOptionalString(arguments, "mode", 16);
				if (string.IsNullOrEmpty(mode))
				{
					mode = "path";
				}
				string prefix = NormalizePvfPath(GetOptionalString(arguments, "prefix", MaximumPvfPathChars));
				int limit = GetOptionalInt(arguments, "limit", 30, 1, 50);
				IEnumerable<KeyValuePair<string, PvfFile>> candidates = files
					.Where(item => string.IsNullOrEmpty(prefix) || item.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
					.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase);

				if (mode.Equals("path", StringComparison.OrdinalIgnoreCase))
				{
					string[] pathMatches = candidates
						.Where(item => item.Key.Contains(searchText, StringComparison.OrdinalIgnoreCase))
						.Select(item => item.Key)
						.Take(limit + 1)
						.ToArray();
					return new { mode = "path", paths = pathMatches.Take(limit).ToArray(), truncated = pathMatches.Length > limit };
				}
				if (!mode.Equals("content", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException("mode 只能是 path 或 content。");
				}

				const int maximumCandidates = 2000;
				List<string> matches = new();
				int scanned = 0;
				bool candidateLimitReached = false;
				long sourceBytesScanned = 0;
				bool sourceByteLimitReached = false;
				int oversizedCandidatesSkipped = 0;
				session.EnsureDecompilerCurrent();
				try
				{
					foreach (KeyValuePair<string, PvfFile> candidate in candidates)
					{
						cancellationToken.ThrowIfCancellationRequested();
						if (!IsTextSearchCandidate(candidate.Value))
						{
							continue;
						}
						if (scanned >= maximumCandidates)
						{
							candidateLimitReached = true;
							break;
						}
						int sourceBytes = Math.Max(candidate.Value.DataLen, 0);
						if (sourceBytes > MaximumPvfTextSourceBytes)
						{
							sourceByteLimitReached = true;
							oversizedCandidatesSkipped++;
							continue;
						}
						if (sourceBytesScanned + sourceBytes > MaximumContentSearchSourceBytes)
						{
							sourceByteLimitReached = true;
							break;
						}
						scanned++;
						sourceBytesScanned += sourceBytes;
						try
						{
							string content = session.GetFileText(candidate.Key, candidate.Value, validateDecompilerState: false);
							if (content.Contains(searchText, StringComparison.OrdinalIgnoreCase))
							{
								matches.Add(candidate.Key);
								if (matches.Count > limit)
								{
									break;
								}
							}
						}
						catch
						{
							session.EnsureFileCurrent(candidate.Key);
						}
					}
				}
				finally
				{
					session.EnsureDecompilerCurrent();
				}
				return new
				{
					mode = "content",
					paths = matches.Take(limit).ToArray(),
					truncated = matches.Count > limit || candidateLimitReached || sourceByteLimitReached,
					scanned,
					candidateLimit = maximumCandidates,
					oversizedCandidatesSkipped,
					sourceBytesScanned,
					sourceByteLimit = MaximumContentSearchSourceBytes
				};
			}, cancellationToken));
	}

	private static IAiAssistantTool CreateReadFileTool(PvfReadSession session)
	{
		return new AiAssistantTool(
			"pvf_read_file",
			"Read and decompile one exact logical file path from the currently open PVF. This never reads the host file system.",
			"""
			{"type":"object","additionalProperties":false,"required":["path"],"properties":{"path":{"type":"string","maxLength":1024},"startLine":{"type":"integer","minimum":1},"endLine":{"type":"integer","minimum":1},"maxChars":{"type":"integer","minimum":1,"maximum":30000}}}
			""",
			(arguments, cancellationToken) => Task.Run<object?>(() =>
			{
				session.EnsureCurrent();
				IReadOnlyDictionary<string, PvfFile> files = session.Files;
				string path = NormalizePvfPath(GetRequiredString(arguments, "path", MaximumPvfPathChars));
				if (!files.TryGetValue(path, out PvfFile? file))
				{
					throw new ArgumentException("当前 PVF 中不存在该逻辑路径。");
				}
				int startLine = GetOptionalInt(arguments, "startLine", 1, 1, int.MaxValue);
				int endLine = GetOptionalInt(arguments, "endLine", int.MaxValue, startLine, int.MaxValue);
				int maxChars = GetOptionalInt(arguments, "maxChars", 12000, 1, 30000);
				cancellationToken.ThrowIfCancellationRequested();
				session.EnsureReadableSize(file);
				string fullText = session.GetFileText(path, file);
				string[] lines = fullText.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n');
				int actualStart = Math.Min(startLine, lines.Length + 1);
				int take = actualStart > lines.Length ? 0 : Math.Min(endLine - actualStart + 1, lines.Length - actualStart + 1);
				string selected = take == 0 ? string.Empty : string.Join("\n", lines.Skip(actualStart - 1).Take(take));
				bool truncated = selected.Length > maxChars;
				if (truncated)
				{
					selected = selected[..maxChars];
				}
				return new { path, startLine = actualStart, lineCount = take, content = selected, truncated };
			}, cancellationToken));
	}

	private static IAiAssistantTool CreateListRegistriesTool(PvfReadSession session)
	{
		return new AiAssistantTool(
			"pvf_list_registries",
			"List known .lst registry logical paths and loaded registry groups for the current PVF.",
			"{\"type\":\"object\",\"additionalProperties\":false,\"properties\":{\"limit\":{\"type\":\"integer\",\"minimum\":1,\"maximum\":200}}}",
			(arguments, cancellationToken) => Task.Run<object?>(() =>
			{
				session.EnsureCurrent();
				int limit = GetOptionalInt(arguments, "limit", 100, 1, 200);
				string[] paths = session.RegistryPaths
					.Where(path => !string.IsNullOrWhiteSpace(path))
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
					.Take(limit + 1)
					.ToArray();
				var groupMatches = session.RegistryGroups
					.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
					.Take(limit + 1)
					.ToArray();
				var groups = groupMatches
					.Take(limit)
					.Select(item => new { name = item.Key, count = item.Value })
					.ToArray();
				cancellationToken.ThrowIfCancellationRequested();
				return new { paths = paths.Take(limit).ToArray(), groups, truncated = paths.Length > limit || groupMatches.Length > limit };
			}, cancellationToken));
	}

	private static IAiAssistantTool CreateResolveLstTool(PvfReadSession session)
	{
		return new AiAssistantTool(
			"pvf_resolve_lst_id",
			"Resolve a numeric ID through one exact .lst registry in the current PVF. Always use this before treating a bare ID as a fact.",
			"""
			{"type":"object","additionalProperties":false,"required":["lstPath","id"],"properties":{"lstPath":{"type":"string","maxLength":1024},"id":{"type":"integer"}}}
			""",
			(arguments, cancellationToken) => Task.Run<object?>(() =>
			{
				session.EnsureCurrent();
				IReadOnlyDictionary<string, PvfFile> files = session.Files;
				string lstPath = NormalizePvfPath(GetRequiredString(arguments, "lstPath", MaximumPvfPathChars));
				int id = GetRequiredInt(arguments, "id");
				if (!files.TryGetValue(lstPath, out PvfFile? lstFile) || lstFile.FileType != PvfFileType.lst)
				{
					throw new ArgumentException("指定路径不是当前 PVF 中的 .lst 文件。");
				}
				cancellationToken.ThrowIfCancellationRequested();
				session.EnsureReadableSize(lstFile);
				session.EnsureFileCurrent(lstPath);
				session.EnsureDecompilerCurrent();
				var result = session.Pvf.GetLstDicTable(lstFile);
				session.EnsureFileCurrent(lstPath);
				session.EnsureDecompilerCurrent();
				if (result.Data == null)
				{
					throw new InvalidOperationException("无法解析该 .lst 文件。");
				}
				if (!result.Data.TryGetValue(id, out LstItem? item))
				{
					return new { lstPath, id, found = false };
				}
				return new
				{
					lstPath,
					id,
					found = true,
					itemPath = item.ItemPath,
					fullPath = item.FullPath,
					targetExists = files.ContainsKey(item.FullPath)
				};
			}, cancellationToken));
	}

	private static bool IsTextSearchCandidate(PvfFile file)
	{
		if (file.IsScriptFile)
		{
			return true;
		}
		string extension = Path.GetExtension(file.FileName);
		return extension.Equals(".str", StringComparison.OrdinalIgnoreCase) ||
			extension.Equals(".txt", StringComparison.OrdinalIgnoreCase) ||
			extension.Equals(".nut", StringComparison.OrdinalIgnoreCase) ||
			extension.Equals(".sqr", StringComparison.OrdinalIgnoreCase) ||
			extension.Equals(".xml", StringComparison.OrdinalIgnoreCase) ||
			extension.Equals(".json", StringComparison.OrdinalIgnoreCase) ||
			extension.Equals(".cfg", StringComparison.OrdinalIgnoreCase);
	}

	private static string NormalizePvfPath(string? path)
	{
		return (path ?? string.Empty).Trim().TrimStart('/', '\\').Replace('\\', '/').ToLowerInvariant();
	}

	private static string GetRequiredString(JsonElement arguments, string name, int maximumLength)
	{
		if (!arguments.TryGetProperty(name, out JsonElement value) || value.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(value.GetString()))
		{
			throw new ArgumentException($"缺少必需的字符串参数 {name}。");
		}
		string result = value.GetString()!.Trim();
		if (result.Length > maximumLength)
		{
			throw new ArgumentException($"参数 {name} 不能超过 {maximumLength} 个字符。");
		}
		return result;
	}

	private static string GetOptionalString(JsonElement arguments, string name, int maximumLength)
	{
		string result = arguments.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.String
			? value.GetString()?.Trim() ?? string.Empty
			: string.Empty;
		if (result.Length > maximumLength)
		{
			throw new ArgumentException($"参数 {name} 不能超过 {maximumLength} 个字符。");
		}
		return result;
	}

	private static int GetRequiredInt(JsonElement arguments, string name)
	{
		if (!arguments.TryGetProperty(name, out JsonElement value) || !value.TryGetInt32(out int result))
		{
			throw new ArgumentException($"缺少必需的整数参数 {name}。");
		}
		return result;
	}

	private static int GetOptionalInt(JsonElement arguments, string name, int defaultValue, int minimum, int maximum)
	{
		if (!arguments.TryGetProperty(name, out JsonElement value))
		{
			return defaultValue;
		}
		if (!value.TryGetInt32(out int result) || result < minimum || result > maximum)
		{
			throw new ArgumentException($"参数 {name} 必须在 {minimum} 到 {maximum} 之间。");
		}
		return result;
	}

	private sealed class PvfReadSession
	{
		private readonly Dictionary<string, PvfFile> _sourceFiles;

		private readonly IReadOnlyDictionary<string, PvfFileState> _fileStates;

		private readonly IReadOnlyList<string> _decompilerDependencyPaths;

		private readonly string? _sourcePath;

		private readonly bool _sourceStringTableUpdated;

		private readonly object? _sourceStringViewFiles;

		public PvfGroup Pvf { get; }

		public IReadOnlyDictionary<string, PvfFile> Files { get; }

		public string? PvfName { get; }

		public IReadOnlyList<string> RegistryPaths { get; }

		public IReadOnlyList<KeyValuePair<string, int>> RegistryGroups { get; }

		public bool IsCurrent =>
			Pvf.PvfIsOpen &&
			ReferenceEquals(Pvf.FileList, _sourceFiles) &&
			string.Equals(Pvf.PvfPackFilePath, _sourcePath, StringComparison.Ordinal) &&
			Pvf.Strtable.IsStringTableUpdated == _sourceStringTableUpdated &&
			ReferenceEquals(Pvf.Strview.Get_pvfstrlist(), _sourceStringViewFiles);

		private PvfReadSession(PvfGroup pvf, Dictionary<string, PvfFile> sourceFiles)
		{
			Pvf = pvf;
			_sourceFiles = sourceFiles;
			_sourcePath = pvf.PvfPackFilePath;
			_sourceStringTableUpdated = pvf.Strtable.IsStringTableUpdated;
			_sourceStringViewFiles = pvf.Strview.Get_pvfstrlist();
			PvfName = string.IsNullOrWhiteSpace(_sourcePath) ? null : Path.GetFileName(_sourcePath);
			try
			{
				// Clone file metadata so one model request sees a stable directory even while editors replace file data.
				Dictionary<string, PvfFile> files = new(StringComparer.OrdinalIgnoreCase);
				Dictionary<string, PvfFileState> fileStates = new(StringComparer.OrdinalIgnoreCase);
				long updatedDataBytesHashed = 0;
				foreach (KeyValuePair<string, PvfFile> item in sourceFiles)
				{
					PvfFile source = item.Value ?? throw new InvalidOperationException("PVF 文件目录包含空条目。");
					bool hashUpdatedData = source.IsUpdated && source.Data is { Length: <= MaximumPvfTextSourceBytes };
					if (hashUpdatedData)
					{
						updatedDataBytesHashed += source.Data!.Length;
						if (updatedDataBytesHashed > MaximumContentSearchSourceBytes)
						{
							throw new InvalidOperationException("PVF 未保存文件的快照超过安全上限。");
						}
					}
					files.Add(item.Key, source.CloneData());
					fileStates.Add(item.Key, PvfFileState.Capture(source, hashUpdatedData));
				}
				Files = files;
				_fileStates = fileStates;
				_decompilerDependencyPaths = files.Keys
					.Where(IsDecompilerDependency)
					.ToArray();
				RegistryPaths = (pvf.ListFileTable.LstFilePaths?.Values.AsEnumerable() ?? Enumerable.Empty<string>())
					.Where(path => !string.IsNullOrWhiteSpace(path))
					.ToArray();
				RegistryGroups = (pvf.ListFileTable.CodeDic ?? new Dictionary<string, Dictionary<int, LstItem>>())
					.Select(item => new KeyValuePair<string, int>(item.Key, item.Value?.Count ?? 0))
					.ToArray();
			}
			catch (Exception exception) when (exception is InvalidOperationException or ArgumentException or NullReferenceException)
			{
				throw new InvalidOperationException("当前 PVF 正在变化，请稍后重试。", exception);
			}
		}

		public static PvfReadSession? TryCreate(PvfGroup pvf)
		{
			if (!pvf.PvfIsOpen || pvf.FileList == null)
			{
				return null;
			}
			try
			{
				return new PvfReadSession(pvf, pvf.FileList);
			}
			catch (InvalidOperationException)
			{
				return null;
			}
		}

		public void EnsureCurrent()
		{
			if (!IsCurrent)
			{
				throw new InvalidOperationException("当前 PVF 已关闭或切换，请重新发送请求。");
			}
		}

		public void EnsureFileCurrent(string path)
		{
			EnsureCurrent();
			if (!_fileStates.TryGetValue(path, out PvfFileState? state) ||
				!_sourceFiles.TryGetValue(path, out PvfFile? source) ||
				!state.Matches(source))
			{
				throw new InvalidOperationException("当前 PVF 文件已变化，请重新发送请求。");
			}
		}

		public void EnsureDecompilerCurrent()
		{
			EnsureCurrent();
			if (_sourceStringTableUpdated)
			{
				throw new InvalidOperationException("当前 PVF 的字符串表存在未保存变更，请保存后重新发送请求。");
			}
			foreach (string path in _decompilerDependencyPaths)
			{
				if (!_fileStates.TryGetValue(path, out PvfFileState? state) ||
					!_sourceFiles.TryGetValue(path, out PvfFile? source) ||
					!state.Matches(source))
				{
					throw new InvalidOperationException("当前 PVF 的字符串表已变化，请重新发送请求。");
				}
			}
		}

		public void EnsureReadableSize(PvfFile file)
		{
			if (file.DataLen is < 0 or > MaximumPvfTextSourceBytes)
			{
				throw new InvalidOperationException("PVF 文件超过只读解析的安全上限。");
			}
		}

		public string GetFileText(string path, PvfFile file, bool validateDecompilerState = true)
		{
			EnsureFileCurrent(path);
			if (validateDecompilerState)
			{
				EnsureDecompilerCurrent();
			}
			EnsureReadableSize(file);
			try
			{
				return Pvf.GetFileText(file, showAniError: false) ?? string.Empty;
			}
			finally
			{
				EnsureFileCurrent(path);
				if (validateDecompilerState)
				{
					EnsureDecompilerCurrent();
				}
			}
		}

		private static bool IsDecompilerDependency(string path)
		{
			return path.Equals("stringtable.bin", StringComparison.OrdinalIgnoreCase) ||
				path.Equals("n_string.lst", StringComparison.OrdinalIgnoreCase) ||
				Path.GetExtension(path).Equals(".str", StringComparison.OrdinalIgnoreCase);
		}

		private sealed record PvfFileState(
			PvfFile Source,
			byte[]? Data,
			byte[]? FileNameBytes,
			int DataLength,
			uint Checksum,
			bool IsUpdated,
			byte[]? UpdatedDataHash)
		{
			public static PvfFileState Capture(PvfFile file, bool hashUpdatedData)
			{
				return new PvfFileState(
					file,
					file.Data,
					file.FileNameBytes,
					file.DataLen,
					file.Checksum,
					file.IsUpdated,
					hashUpdatedData
						? System.Security.Cryptography.SHA256.HashData(file.Data!)
						: null);
			}

			public bool Matches(PvfFile file)
			{
				return ReferenceEquals(file, Source) &&
					ReferenceEquals(file.Data, Data) &&
					ReferenceEquals(file.FileNameBytes, FileNameBytes) &&
					file.DataLen == DataLength &&
					file.Checksum == Checksum &&
					file.IsUpdated == IsUpdated &&
					(UpdatedDataHash == null ||
						System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
							UpdatedDataHash,
							System.Security.Cryptography.SHA256.HashData(file.Data!)));
			}
		}
	}
}
