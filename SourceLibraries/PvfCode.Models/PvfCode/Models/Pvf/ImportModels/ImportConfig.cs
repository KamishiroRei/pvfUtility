using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PvfCode.Models.Pvf.Enums;

namespace PvfCode.Models.Pvf.ImportModels;

[JsonObject(MemberSerialization.OptOut)]
public class ImportConfig : ModelBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass46_0
	{
		public ParallelOptions Buj5DPOw7F;

		public ImportConfig oc353BZ4Rl;

		public string mDp5H4R4Ro;

		public ConcurrentDictionary<string, ImportFileItem> Wg057xA6Hi;

		public Func<FileInfo, CancellationToken, ValueTask> eKO5cKpOWR;

		public _003C_003Ec__DisplayClass46_0()
		{
		}

		internal async ValueTask O0W55NrWTu(string path, CancellationToken ct)
		{
			if (Directory.Exists(path))
			{
				await Parallel.ForEachAsync(new DirectoryInfo(path).GetFiles("*.*", SearchOption.AllDirectories), Buj5DPOw7F, async delegate(FileInfo t, CancellationToken cancellationToken)
				{
					oc353BZ4Rl.wOGLBIlVUB(t.FullName, mDp5H4R4Ro, Wg057xA6Hi);
				});
			}
			else if (File.Exists(path))
			{
				oc353BZ4Rl.wOGLBIlVUB(path, mDp5H4R4Ro, Wg057xA6Hi);
			}
		}

		internal async ValueTask ue65poQJsL(FileInfo t, CancellationToken ct2)
		{
			oc353BZ4Rl.wOGLBIlVUB(t.FullName, mDp5H4R4Ro, Wg057xA6Hi);
		}
	}

	private bool XCkL4O5Pf8;

	private bool sjYLCPx2mq;

	private bool xNdLvbimnH;

	[CompilerGenerated]
	private HashSet<ImportFileItem> nADLbpppcv;

	private string ElELVyJrnk;

	private bool FlGLPenHXB;

	private bool xBGLF6aAWr;

	private bool uyPLXrTfp5;

	private FileOperation? lt6LNEXeVw;

	private RemoveOrKeepFileType? hRSLiQaOTg;

	private List<string> Wg6LMJlTTn;

	public bool CompileChinaPvfScriptFile
	{
		get
		{
			return XCkL4O5Pf8;
		}
		set
		{
			XCkL4O5Pf8 = value;
			DoNotify("CompileChinaPvfScriptFile");
		}
	}

	public bool CompileChinaAni
	{
		get
		{
			return sjYLCPx2mq;
		}
		set
		{
			sjYLCPx2mq = value;
			DoNotify("CompileChinaAni");
		}
	}

	public bool ImportSuccessFilePathListAddToSearchPanel
	{
		get
		{
			return xNdLvbimnH;
		}
		set
		{
			xNdLvbimnH = value;
			DoNotify("ImportSuccessFilePathListAddToSearchPanel");
		}
	}

	[JsonIgnore]
	public HashSet<ImportFileItem> SourceFiles
	{
		[CompilerGenerated]
		get
		{
			return nADLbpppcv;
		}
		[CompilerGenerated]
		set
		{
			nADLbpppcv = value;
		}
	}

	public string TargetPath
	{
		get
		{
			if (ElELVyJrnk == null)
			{
				ElELVyJrnk = string.Empty;
			}
			return ElELVyJrnk;
		}
		set
		{
			ElELVyJrnk = value;
			DoNotify("TargetPath");
		}
	}

	public bool CompileScript
	{
		get
		{
			return FlGLPenHXB;
		}
		set
		{
			FlGLPenHXB = value;
			DoNotify("CompileScript");
		}
	}

	public bool CompileBinaryAni
	{
		get
		{
			return xBGLF6aAWr;
		}
		set
		{
			xBGLF6aAWr = value;
			DoNotify("CompileBinaryAni");
		}
	}

	public bool ConvertToTraditionalChinese
	{
		get
		{
			return uyPLXrTfp5;
		}
		set
		{
			uyPLXrTfp5 = value;
			DoNotify("ConvertToTraditionalChinese");
		}
	}

	public FileOperation Operation
	{
		get
		{
			if (!lt6LNEXeVw.HasValue)
			{
				lt6LNEXeVw = FileOperation.Cover;
			}
			return lt6LNEXeVw.Value;
		}
		set
		{
			lt6LNEXeVw = value;
			DoNotify("Operation");
		}
	}

	public RemoveOrKeepFileType RemoveOrKeepFileType
	{
		get
		{
			if (!hRSLiQaOTg.HasValue)
			{
				hRSLiQaOTg = RemoveOrKeepFileType.保留;
			}
			return hRSLiQaOTg.Value;
		}
		set
		{
			hRSLiQaOTg = value;
			DoNotify("RemoveOrKeepFileType");
		}
	}

	public List<string> FileTypes
	{
		get
		{
			return Wg6LMJlTTn;
		}
		set
		{
			Wg6LMJlTTn = value;
			DoNotify("FileTypes");
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
		_003C_003Ec__DisplayClass46_0 CS_0024_003C_003E8__locals13 = new _003C_003Ec__DisplayClass46_0();
		CS_0024_003C_003E8__locals13.oc353BZ4Rl = this;
		CS_0024_003C_003E8__locals13.mDp5H4R4Ro = diskFiles[0].Remove(diskFiles[0].LastIndexOf('\\'));
		CS_0024_003C_003E8__locals13.Wg057xA6Hi = new ConcurrentDictionary<string, ImportFileItem>();
		CS_0024_003C_003E8__locals13.Buj5DPOw7F = new ParallelOptions
		{
			MaxDegreeOfParallelism = 200
		};
		await Parallel.ForEachAsync(diskFiles, CS_0024_003C_003E8__locals13.Buj5DPOw7F, async delegate(string path, CancellationToken ct)
		{
			if (Directory.Exists(path))
			{
				await Parallel.ForEachAsync(new DirectoryInfo(path).GetFiles("*.*", SearchOption.AllDirectories), CS_0024_003C_003E8__locals13.Buj5DPOw7F, async delegate(FileInfo t, CancellationToken cancellationToken)
				{
					CS_0024_003C_003E8__locals13.oc353BZ4Rl.wOGLBIlVUB(t.FullName, CS_0024_003C_003E8__locals13.mDp5H4R4Ro, CS_0024_003C_003E8__locals13.Wg057xA6Hi);
				});
			}
			else if (File.Exists(path))
			{
				CS_0024_003C_003E8__locals13.oc353BZ4Rl.wOGLBIlVUB(path, CS_0024_003C_003E8__locals13.mDp5H4R4Ro, CS_0024_003C_003E8__locals13.Wg057xA6Hi);
			}
		});
		SourceFiles = CS_0024_003C_003E8__locals13.Wg057xA6Hi.Values.ToHashSet();
	}

	private void wOGLBIlVUB(string P_0, string P_1, ConcurrentDictionary<string, ImportFileItem> P_2)
	{
		if (P_2.ContainsKey(P_0))
		{
			return;
		}
		ImportFileItem importFileItem = new ImportFileItem(P_0, P_1);
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
		P_2.TryAdd(P_0, importFileItem);
	}
}
