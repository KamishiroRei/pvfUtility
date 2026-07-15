using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Models.Pvf.ImportModels;

[JsonObject(MemberSerialization.OptOut)]
public class ImportConfig : ModelBase
{
	private bool compileChinaPvfScriptFile;

	private bool compileChinaAni;

	private bool addImportedFilesToSearchPanel;

	private string targetPath;

	private bool compileScript;

	private bool compileBinaryAni;

	private bool convertToTraditionalChinese;

	private FileOperation? operation;

	private RemoveOrKeepFileType? removeOrKeepFileType;

	private List<string> fileTypes;

	public bool CompileChinaPvfScriptFile
	{
		get
		{
			return compileChinaPvfScriptFile;
		}
		set
		{
			compileChinaPvfScriptFile = value;
			DoNotify(nameof(CompileChinaPvfScriptFile));
		}
	}

	public bool CompileChinaAni
	{
		get
		{
			return compileChinaAni;
		}
		set
		{
			compileChinaAni = value;
			DoNotify(nameof(CompileChinaAni));
		}
	}

	public bool ImportSuccessFilePathListAddToSearchPanel
	{
		get
		{
			return addImportedFilesToSearchPanel;
		}
		set
		{
			addImportedFilesToSearchPanel = value;
			DoNotify(nameof(ImportSuccessFilePathListAddToSearchPanel));
		}
	}

	[JsonIgnore]
	public HashSet<ImportFileItem> SourceFiles { get; set; }

	public string TargetPath
	{
		get
		{
			if (targetPath == null)
			{
				targetPath = string.Empty;
			}
			return targetPath;
		}
		set
		{
			targetPath = value;
			DoNotify(nameof(TargetPath));
		}
	}

	public bool CompileScript
	{
		get
		{
			return compileScript;
		}
		set
		{
			compileScript = value;
			DoNotify(nameof(CompileScript));
		}
	}

	public bool CompileBinaryAni
	{
		get
		{
			return compileBinaryAni;
		}
		set
		{
			compileBinaryAni = value;
			DoNotify(nameof(CompileBinaryAni));
		}
	}

	public bool ConvertToTraditionalChinese
	{
		get
		{
			return convertToTraditionalChinese;
		}
		set
		{
			convertToTraditionalChinese = value;
			DoNotify(nameof(ConvertToTraditionalChinese));
		}
	}

	public FileOperation Operation
	{
		get
		{
			if (!operation.HasValue)
			{
				operation = FileOperation.Cover;
			}
			return operation.Value;
		}
		set
		{
			operation = value;
			DoNotify(nameof(Operation));
		}
	}

	public RemoveOrKeepFileType RemoveOrKeepFileType
	{
		get
		{
			if (!removeOrKeepFileType.HasValue)
			{
				removeOrKeepFileType = RemoveOrKeepFileType.保留;
			}
			return removeOrKeepFileType.Value;
		}
		set
		{
			removeOrKeepFileType = value;
			DoNotify(nameof(RemoveOrKeepFileType));
		}
	}

	public List<string> FileTypes
	{
		get
		{
			return fileTypes;
		}
		set
		{
			fileTypes = value;
			DoNotify(nameof(FileTypes));
		}
	}

	public ImportConfig()
	{
		ResSet();
	}

	public void ResSet()
	{
		Operation = FileOperation.Cover;
		TargetPath = null;
		CompileScript = true;
		CompileBinaryAni = true;
		ConvertToTraditionalChinese = false;
		FileTypes = null;
		RemoveOrKeepFileType = RemoveOrKeepFileType.排除;
		SourceFiles = null;
	}

	public async Task DiskFileListToImportItems(List<string> diskFiles)
	{
		string rootPath = diskFiles[0].Remove(diskFiles[0].LastIndexOf('\\'));
		ConcurrentDictionary<string, ImportFileItem> importItems = new ConcurrentDictionary<string, ImportFileItem>();
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 200
		};
		await Parallel.ForEachAsync(diskFiles, parallelOptions, async (path, cancellationToken) =>
		{
			if (Directory.Exists(path))
			{
				await Parallel.ForEachAsync(new DirectoryInfo(path).GetFiles("*.*", SearchOption.AllDirectories), parallelOptions, (file, nestedCancellationToken) =>
				{
					AddImportItem(file.FullName, rootPath, importItems);
					return ValueTask.CompletedTask;
				});
			}
			else if (File.Exists(path))
			{
				AddImportItem(path, rootPath, importItems);
			}
		});
		SourceFiles = importItems.Values.ToHashSet();
	}

	private void AddImportItem(string filePath, string rootPath, ConcurrentDictionary<string, ImportFileItem> importItems)
	{
		if (importItems.ContainsKey(filePath))
		{
			return;
		}
		ImportFileItem importFileItem = new ImportFileItem(filePath, rootPath);
		if (!string.IsNullOrEmpty(TargetPath))
		{
			importFileItem.TreeFullPath = TargetPath + importFileItem.FilePath.Replace('\\', '/').ToLower();
		}
		else
		{
			importFileItem.TreeFullPath = importFileItem.FilePath.Replace('\\', '/').ToLower();
			if (importFileItem.TreeFullPath[0] == '/')
			{
				importFileItem.TreeFullPath = importFileItem.TreeFullPath.Remove(0, 1);
			}
		}
		importItems.TryAdd(filePath, importFileItem);
	}
}
