using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using Nito.AsyncEx;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Models.Options.Enums;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services;
using PvfCode.Services.PreviewPvfFileFolder;
using PvfCode.Services.PvfParsingNew;
using Pvf110.Core;
using Utools;
using pvfUtility.WebApi.Dto;

namespace PvfCode;

public class PvfGroup : PvfPack
{
	private readonly Ilogger logger;

	private readonly byte[] footerSignature;

	private readonly AsyncLock saveLock;

	/// <summary>Pvf110 模式下持有逻辑流读取器（含 name 池/group/body）。</summary>
	private Pvf110Reader? _pvf110;

	/// <summary>NKPI / ProtectedNKPI 模式下持有读取器（懒加载 body）。</summary>
	private NkpiReader? _nkpi;

	/// <summary>懒加载模式下 entry 索引：路径 → 读取器 entry 索引。</summary>
	private Dictionary<string, int> _entryIndex = new();

	/// <summary>统一管线适配层：110/NKPI 名称池偏移 → 虚拟串表 ID。</summary>
	private Dictionary<int, int> _poolOffsetToVirtualId = new();

	/// <summary>统一管线适配层：当前打开包的名称池引用（打开时缓存，供偏移兜底解析）。</summary>
	private byte[] _classicViewUtf8Pool;
	private byte[] _classicViewUtf16Pool;

	/// <summary>当前打开的是 Pvf110 Builder 保护链的 Script.pvf。</summary>
	public bool IsPvf110 => _pvf110 != null;

	public PvfGroup()
	{
		footerSignature = new byte[41]
		{
			0, 84, 104, 105, 115, 32, 112, 118, 102, 32,
			80, 97, 99, 107, 32, 119, 97, 115, 32, 99,
			114, 101, 97, 116, 101, 100, 32, 98, 121, 32,
			112, 118, 102, 85, 116, 105, 108, 105, 116, 121,
			46
		};
		saveLock = new AsyncLock();
		logger = AppSetting.Instance.GetService<Ilogger>();
	}

	public void Clear()
	{
		if (base.FileList != null)
		{
			base.FileList = null;
		}
		_pvf110 = null;
		_nkpi?.Dispose();
		_nkpi = null;
		_entryIndex.Clear();
		_poolOffsetToVirtualId.Clear();
		_classicViewUtf8Pool = null;
		_classicViewUtf16Pool = null;
		base.ListFileTable.Clear();
		base.Strtable.Clear();
		base.Strview.Clear();
		base.PvfPackFilePath = null;
		base.PvfIsOpen = false;
		base.EquipmentPartSetTable.Clear();
		base.HasUnsavedChanges = false;
	}

	public async Task<ResultData> SavePvfPack(string filePath, bool isFastMode, IProgress<double> progress, bool notButtonClick = true)
	{
		Window loadingWin = null;
		if (AppSetting.Instance.PvfConfig.SavePvfLoadingDisableMainWindow && !notButtonClick)
		{
			AppSetting.Instance.MainWindowIsEnabled = false;
			loadingWin = logger.CreateLoadingWindow(AppSetting.Instance.GetIlogger()?.GetStr("mess_SavingPackage"));
			logger.ShowLoadingWindow(loadingWin);
		}
		ResultData resultData = await SavePvfPackCore(filePath, progress, notButtonClick);
		if (resultData.IsError)
		{
			string directoryName = Path.GetDirectoryName(filePath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
			int suffix = 1;
			string fallbackPath = Path.Combine(directoryName, $"{fileNameWithoutExtension}({suffix}).pvf");
			while (File.Exists(fallbackPath))
			{
				suffix++;
				fallbackPath = Path.Combine(directoryName, $"{fileNameWithoutExtension}({suffix}).pvf");
			}
			logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SavePvfError"), resultData.Msg));
			string msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SavePvfError2"), fallbackPath);
			if (!notButtonClick)
			{
				logger.ShowMsg(msg);
			}
			logger.Warning(msg);
			resultData = await SavePvfPackCore(fallbackPath, progress, notButtonClick);
		}
		AppSetting.Instance.MainWindowIsEnabled = true;
		logger.CloseLoadingWindow(loadingWin);
		return resultData;
	}

	private async Task<ResultData> SavePvfPackCore(string filePath, IProgress<double> progress, bool notButtonClick)
	{
		ResultData result = new ResultData();
		try
		{
			using (await saveLock.LockAsync())
			{
				// Pvf110：保留 name/hash 表，重建 body/group/file table（未修改组复用原始密文）
				if (IsPvf110)
				{
					result = await SavePvfPack110Core(filePath, progress);
				}
				// NKPI / ProtectedNKPI：未修改组复用原始密文增量流式重建；有新增文件时全量重建
				else if (_nkpi != null)
				{
					SavePvfPackNkpiCore(filePath, progress);
					progress?.Report(100.0);
				}
				else
				{
				using Stream output = File.Create(filePath);
				if (base.Strtable.IsStringTableUpdated)
				{
					GetFile("stringtable.bin")?.WriteFileData(base.Strtable.CreateStringTable());
				}
				List<PvfFile> files = base.FileList.Values.OrderBy(file => file.FileNameBytesChecksum).ToList();
				int fileCount = files.Count;
				using BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write(BitConverter.GetBytes(base._guidLen), 0, 4);
				binaryWriter.Write(base.Guid, 0, base._guidLen);
				binaryWriter.Write(BitConverter.GetBytes(base.FileVersion), 0, 4);
				byte[] fileTreeData = CreateFileTreeData(files, progress);
				base._fileTreeChecksum = PvfAlgorithmHelper.CreateBuffKey(fileTreeData, base._fileTreeLength, (uint)base.FileList.Count);
				binaryWriter.Write(BitConverter.GetBytes(base._fileTreeLength), 0, 4);
				binaryWriter.Write(BitConverter.GetBytes(base._fileTreeChecksum), 0, 4);
				binaryWriter.Write(BitConverter.GetBytes(base.FileList.Count), 0, 4);
				binaryWriter.Write(PvfAlgorithmHelper.EncryptionPvf(fileTreeData, base._fileTreeLength, base._fileTreeChecksum), 0, base._fileTreeLength);
				int processedFileCount = 0;
				foreach (PvfFile file in files)
				{
					int blockLength = file.GetBlockLength();
					if (blockLength > 0)
					{
						binaryWriter.Write(PvfAlgorithmHelper.EncryptionPvf(file.Data, blockLength, file.Checksum), 0, blockLength);
					}
					if (processedFileCount % 512 == 0)
					{
						progress?.Report(ProgressHelper.GetProgressNum(fileCount + processedFileCount, fileCount * 2));
					}
					processedFileCount++;
				}
				binaryWriter.Write(footerSignature);
				binaryWriter.Flush();
				progress?.Report(100.0);
				}
			}
			if (!result.IsError)
			{
				base.HasUnsavedChanges = false;
			}
		}
		catch (Exception ex)
		{
			result.Msg = ex.Message;
		}
		return result;
	}

	/// <summary>Pvf110 保存：未修改文件复用原始内容（零重编译），修改文件重编译 → 增量重建 → 临时文件原子替换。</summary>
	private async Task<ResultData> SavePvfPack110Core(string filePath, IProgress<double> progress)
	{
		ResultData result = new ResultData();
		try
		{
			Pvf110Reader reader = _pvf110!;
			Pvf110Compiled compiled = new Pvf110Compiled(reader);
			// 名称池写入器：修改内容引入池外新字符串时追加到池尾，保存时同步重建 name 表段；
			// 既有字符串一律复用原 magic，不移动池内任何既有条目。
			Pvf110NamePool namePool = Pvf110NamePool.FromReader(reader);

			// 未修改（映射缺失按修改处理，由 GetContent 抛出原有错误信息）
			bool IsEntryModified(Pvf110Entry e)
				=> !base.FileList.TryGetValue(reader.FilePath(e), out PvfFile? f) || f.IsContentModified;

			byte[] GetContent(Pvf110Entry e)
			{
				string p = reader.FilePath(e);
				if (!base.FileList.TryGetValue(p, out PvfFile? file) || file.Data == null)
					throw new InvalidOperationException("Pvf110 保存要求文件集合与原包一致，缺少文件: " + p);
				// 未修改文件：直接使用原始内容，跳过懒加载与重编译
				if (!file.IsContentModified)
				{
					return reader.ReadEntry(e);
				}
				// 懒加载：保存前确保未访问过的文件内容已加载
				if (file.Data.Length == 0)
				{
					EnsureFileData(p);
				}
				if (e.DataType == 1)
				{
					try
					{
						// 统一管线：修改内容为经典视图 → 适配回 110 token；
						// 池外新字符串经 Pvf110NamePool 追加到 utf16 池尾
						return CompileModifiedContentForSave(file.Data, compiled, namePool.GetOrAdd);
					}
					catch (InvalidDataException)
					{
						throw;
					}
					catch (Exception ex)
					{
						throw new InvalidOperationException($"Pvf110 重编译失败 {p}: {ex.Message}", ex);
					}
				}
				return file.Data; // type-3 = UTF-16LE
			}

			// 流式重建：直接写临时文件再原子替换。峰值内存与归档大小无关
			// （旧实现需同时驻留整包明文逻辑流与整包密文输出，760 MB 归档上约 1.5 GB 瞬时占用）。
			string tempPath = CreateTempSiblingPath(filePath);
			byte[] sk;
			try
			{
				using (FileStream rebuildStream = new FileStream(tempPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 1 << 20))
				{
					(_, sk) = Pvf110Rebuilder.RebuildToStream(reader, rebuildStream, GetContent, progress, IsEntryModified,
						additions: null, options: Pvf110RebuildOptions.Default, namePool: namePool);
				}
				MoveOrReplace(tempPath, filePath);
			}
			catch
			{
				TempFileDelete(tempPath);
				throw;
			}
			string skPath = Path.Combine(Path.GetDirectoryName(filePath) ?? ".", "sk.dat");
			// sk.dat 与目标位置现有内容一致时跳过写入：chunk key 未变（复用客户端密钥槽位），
			// 客户端自身那份 sk.dat 照旧可用，不必每次保存都重写这个 2.5 KB 的配套件。
			if (!File.Exists(skPath) || !File.ReadAllBytes(skPath).AsSpan().SequenceEqual(sk))
			{
				AtomicWriteAllBytes(skPath, sk);
			}
			progress?.Report(100.0);
			return result;
		}
		catch (Exception ex)
		{
			result.Msg = ex.Message;
			return result;
		}
	}

	/// <summary>
	/// NKPI / ProtectedNKPI 保存：未修改组复用原始密文增量流式重建（临时文件 + 原子替换）；
	/// 新增文件（路径不在读取器映射中）或修改内容引入名称池外新字符串时走全量重建路径。
	/// </summary>
	private void SavePvfPackNkpiCore(string filePath, IProgress<double> progress)
	{
		NkpiReader reader = _nkpi!;
		Pvf110Compiled compiled = new Pvf110Compiled(reader);
		NkpiNamePoolBuilder namePool = new NkpiNamePoolBuilder(reader);
		bool allowNewStrings = false;

		byte[] GetContent(int entryIndex)
		{
			NkpiEntry e = reader.Entries[entryIndex];
			string p = reader.FilePath(e);
			if (!base.FileList.TryGetValue(p, out PvfFile? file) || file.Data == null)
				throw new InvalidOperationException("NKPI 保存时找不到既有文件: " + p);
			// 未修改文件：直接使用原始内容，跳过懒加载与重编译
			if (!file.IsContentModified)
				return reader.ReadEntry(e);
			// 懒加载：保存前确保未访问过的文件内容已加载
			if (file.Data.Length == 0)
			{
				EnsureFileData(p);
			}
			if (e.DataType == 1)
			{
				try
				{
					// 统一管线：修改内容为经典视图 → 适配回 110 token；
					// 虚拟 ID → 名称池偏移，池外新串经 NkpiNamePoolBuilder 追加
					return CompileModifiedContentForSave(file.Data, compiled, allowNewStrings ? namePool.GetOrAdd : null);
				}
				catch (InvalidDataException)
				{
					// "string not in name pool" 等格式错误原样上抛：增量保存据此回退全量重建
					throw;
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"NKPI 重编译失败 {p}: {ex.Message}", ex);
				}
			}
			return file.Data; // type-3 = UTF-16LE
		}

		bool IsEntryModified(int entryIndex)
		{
			NkpiEntry e = reader.Entries[entryIndex];
			return !base.FileList.TryGetValue(reader.FilePath(e), out PvfFile? f) || f.IsContentModified;
		}

		try
		{
			// 新增文件：以读取器 entry 映射为准（修复旧 IsNewFile 过滤把所有既有文件误判为新增的问题）
			List<NkpiNewFile> additions = base.FileList.Values
				.Where(file => !_entryIndex.ContainsKey(file.FileName))
				.Select(file => CreateNkpiNewFile(file, compiled, namePool))
				.ToList();

			if (additions.Count > 0)
			{
				allowNewStrings = true;
				byte[] rebuilt = NkpiRepacker.RebuildWithAdditions(reader, GetContent, additions, namePool, progress);
				AtomicWriteAllBytes(filePath, rebuilt);
				return;
			}

			string tempPath = CreateTempSiblingPath(filePath);
			bool samePath = IsSamePath(filePath, base.PvfPackFilePath);
			try
			{
				NkpiRepacker.RebuildIncrementalToFile(reader, IsEntryModified, GetContent, tempPath, progress);
			}
			catch (InvalidDataException ex) when (ex.Message.StartsWith("string not in name pool", StringComparison.Ordinal))
			{
				// 修改内容引入了名称池外的新字符串：名称/HASH 表必须重建，回退全量重建
				TempFileDelete(tempPath);
				allowNewStrings = true;
				byte[] rebuilt = NkpiRepacker.RebuildWithAdditions(reader, GetContent, additions, namePool, progress);
				AtomicWriteAllBytes(filePath, rebuilt);
				return;
			}
			catch
			{
				TempFileDelete(tempPath);
				throw;
			}

			if (samePath)
			{
				_nkpi = null;
				reader.Dispose(); // 释放句柄以允许原子替换
			}
			try
			{
				MoveOrReplace(tempPath, filePath);
			}
			catch
			{
				if (samePath)
				{
					ReopenNkpiReader(base.PvfPackFilePath!);
				}
				TempFileDelete(tempPath);
				throw;
			}
			if (samePath)
			{
				ReopenNkpiReader(filePath);
			}
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"NKPI 保存失败: {ex.Message}", ex);
		}
	}

	/// <summary>
	/// 保存链路适配（统一管线，写侧）：把用户修改后的内容（经典视图 token 流）适配回
	/// 110 token：虚拟串表 ID → 名称池偏移；池外新串经 <paramref name="namePool"/> 追加
	/// （为 null 时抛 "string not in name pool"，与增量保存的全量重建回退约定一致）。
	/// 兼容导入/历史会话遗留的可读文本内容（#PVF_File 文本 → Pvf110Compiled.FromText）。
	/// </summary>
	private byte[] CompileModifiedContentForSave(byte[] data, Pvf110Compiled compiled, Func<string, int>? resolver)
	{
		if (data.Length >= 2 && BitConverter.ToUInt16(data, 0) == ClassicViewAdapter.ClassicMagic)
		{
			return ClassicViewAdapter.FromClassicView(data, id => base.Strtable.GetStringItem(id), resolver);
		}
		return compiled.FromText(System.Text.Encoding.UTF8.GetString(data), resolver);
	}

	private static bool IsSamePath(string? a, string? b)
	{
		if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
		try
		{
			return string.Equals(Path.GetFullPath(a), Path.GetFullPath(b), StringComparison.OrdinalIgnoreCase);
		}
		catch
		{
			return false;
		}
	}

	private static string CreateTempSiblingPath(string path)
	{
		string dir = Path.GetDirectoryName(path) ?? ".";
		return Path.Combine(dir, Path.GetFileName(path) + ".tmp-" + System.Guid.NewGuid().ToString("N").Substring(0, 8));
	}

	private static void MoveOrReplace(string tempPath, string targetPath)
	{
		if (File.Exists(targetPath))
			File.Replace(tempPath, targetPath, null);
		else
			File.Move(tempPath, targetPath);
	}

	private static void TempFileDelete(string tempPath)
	{
		try { File.Delete(tempPath); } catch { /* 清理失败不影响主流程 */ }
	}

	private static void AtomicWriteAllBytes(string path, byte[] bytes)
	{
		string tempPath = CreateTempSiblingPath(path);
		try
		{
			File.WriteAllBytes(tempPath, bytes);
			MoveOrReplace(tempPath, path);
		}
		catch
		{
			TempFileDelete(tempPath);
			throw;
		}
	}

	private void ReopenNkpiReader(string path)
	{
		try
		{
			_nkpi = NkpiReader.OpenFile(path);
			if (_nkpi != null && base.FileList != null)
			{
				// 重建路径映射（增量保存不改变条目集合，正常应一一对应）
				_entryIndex.Clear();
				for (int i = 0; i < _nkpi.Entries.Count; i++)
				{
					_entryIndex[_nkpi.FilePath(_nkpi.Entries[i])] = i;
				}
			}
		}
		catch (Exception ex)
		{
			logger.Error($"NKPI reopen after save failed: {ex.Message}");
			_nkpi = null;
		}
	}

	private NkpiNewFile CreateNkpiNewFile(PvfFile file, Pvf110Compiled compiled, NkpiNamePoolBuilder namePool)
	{
		byte[] data = file.Data ?? Array.Empty<byte>();
		if (file.DataLen >= 0 && file.DataLen < data.Length)
			data = data.AsSpan(0, file.DataLen).ToArray();

		if (file.Pvf110DataType == 1)
		{
			if (data.Length >= 2 && BitConverter.ToUInt16(data, 0) == ClassicViewAdapter.ClassicMagic)
			{
				// 统一管线：新增文件携带经典视图（导入/复制），适配为 110 token 并补齐名称池
				data = ClassicViewAdapter.FromClassicView(data,
					id => base.Strtable.GetStringItem(id),
					namePool.GetOrAdd);
			}
			else
			{
				string text = Encoding.UTF8.GetString(data);
				if (text.StartsWith("#PVF_File", StringComparison.OrdinalIgnoreCase))
				{
					int newline = text.IndexOf('\n');
					text = newline >= 0 ? text[(newline + 1)..] : string.Empty;
					if (text.StartsWith("\n", StringComparison.Ordinal))
						text = text[1..];
				}
				data = compiled.FromText(text, namePool.GetOrAdd);
			}
		}

		return new NkpiNewFile(file.FileName, data, file.Pvf110DataType is 1 or 3 ? file.Pvf110DataType : 2);
	}

	private byte[] CreateFileTreeData(IEnumerable<PvfFile> files, IProgress<double> progress)
	{
		base._fileTreeLength = (base.FileList.Aggregate<KeyValuePair<string, PvfFile>, int>(0, (length, fileEntry) => length + fileEntry.Value.FileNameLen + 20) + 3) & -4;
		int processedFileCount = 0;
		int dataOffset = 0;
		using MemoryStream memoryStream = new MemoryStream(base._fileTreeLength);
		memoryStream.SetLength(base._fileTreeLength);
		foreach (PvfFile file in files)
		{
			int fileNameLen = file.FileNameLen;
			int blockLength = file.GetBlockLength();
			memoryStream.Write(BitConverter.GetBytes(file.FileNameBytesChecksum), 0, 4);
			memoryStream.Write(BitConverter.GetBytes((uint)fileNameLen), 0, 4);
			memoryStream.Write(file.FileNameBytes, 0, fileNameLen);
			memoryStream.Write(BitConverter.GetBytes((uint)file.DataLen), 0, 4);
			memoryStream.Write(BitConverter.GetBytes(file.Checksum), 0, 4);
			memoryStream.Write(BitConverter.GetBytes((uint)dataOffset), 0, 4);
			dataOffset += blockLength;
			if (processedFileCount % 1024 == 0)
			{
				progress?.Report(ProgressHelper.GetProgressNum(processedFileCount, base.FileList.Count * 2));
			}
			processedFileCount++;
		}
		return memoryStream.ToArray();
	}

	private bool HasFooterSignature(Stream stream)
	{
		int signatureLength = footerSignature.Length;
		stream.Seek(stream.Length - signatureLength, SeekOrigin.Begin);
		for (int i = 0; i < signatureLength; i++)
		{
			if (footerSignature[i] != stream.ReadByte())
			{
				stream.Seek(0L, SeekOrigin.Begin);
				return false;
			}
		}
		stream.Seek(0L, SeekOrigin.Begin);
		return true;
	}

	public Task<bool> OpenPvfPack(string path, IProgress<double> progress)
	{
		lock (this)
		{
			Exception? pvf110Failure = null;
			try
			{
				base.PvfPackFilePath = path;

				// 1. Pvf110 Builder 保护链检测：sk.dat 三级回退（PVF_SKDAT → 同目录/上溯 → 内置 sk.dat），
				//    外层包装密钥优先用**内置客户端密钥集**（115 固定版单机客户端已内置）。
				//    因此打开该客户端的 Script.pvf 既不需要 sk.dat 文件，也不需要客户端 EXE。
				try
				{
					var probe = Pvf110Reader.OpenPvf(path);
					return OpenPvfPack110(probe, path, progress);
				}
				catch (Exception ex)
				{
					// 非 Pvf110 容器（90CN ProtectedNKPI 等）在此必然走到这里，属正常探测失败，故只记 Debug。
					pvf110Failure = ex;
					logger.Debug($"Pvf110 probe: {ex.Message}");
				}

				// 2. NKPI / ProtectedNKPI 检测：流式打开，只驻留结构区，body 按需读取
				if (NkpiReader.IsNkpiFile(path))
				{
					try
					{
						var reader = NkpiReader.OpenFile(path);
						return OpenPvfPackNkpi(reader, path, progress);
					}
					catch (Exception ex)
					{
						logger.Warning($"NKPI open failed, fallback to classic: {ex.Message}");
					}
				}

				// 3. 经典 pvfUtility 格式回退（整包读入）。先按格式特征判定：经典格式头是 1..64 的 guid 长度，
				//    不满足就绝不做整包读入（115 归档 760 MB，读进来只会 OOM 并抛出与本因无关的错误）。
				int guidLenProbe;
				using (FileStream headStream = File.OpenRead(path))
				{
					Span<byte> head = stackalloc byte[4];
					guidLenProbe = headStream.Read(head) == 4 ? BitConverter.ToInt32(head) : -1;
				}
				if (guidLenProbe is < 1 or > 64)
				{
					throw new InvalidDataException(
						$"未能识别的 PVF 容器（{Path.GetFileName(path)}）：" +
						(pvf110Failure == null
							? $"NKPI/ProtectedNKPI 与经典格式均不匹配（guidLen={guidLenProbe}）"
							: $"Pvf110 探测失败：{pvf110Failure.Message}；NKPI/ProtectedNKPI 与经典格式也不匹配。内置 sk.dat/密钥集只对应固定版 115 客户端，换版本时请把该客户端的 sk.dat 与 DFO.exe 放到 PVF 同目录（或设 PVF_SKDAT / PVF_CLIENT_EXE）"),
						pvf110Failure);
				}

				byte[] fileBytes = File.ReadAllBytes(path);
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(fileBytes)))
				{
					HashSet<PvfFile> duplicateFiles = new HashSet<PvfFile>();
					bool hasFooterSignature = HasFooterSignature(binaryReader.BaseStream);
					base._guidLen = binaryReader.ReadInt32();
					base.Guid = binaryReader.ReadBytes(base._guidLen);
					base.FileVersion = binaryReader.ReadInt32();
					base._fileTreeLength = binaryReader.ReadInt32();
					base._fileTreeChecksum = binaryReader.ReadUInt32();
					int fileCount = binaryReader.ReadInt32();
					base.FileList = new Dictionary<string, PvfFile>();
					BinaryReader fileTreeReader = new BinaryReader(new MemoryStream(PvfAlgorithmHelper.DecryptionPvf(binaryReader.ReadBytes(base._fileTreeLength), base._fileTreeLength, base._fileTreeChecksum)));
					for (int i = 0; i < fileCount; i++)
					{
						uint fileNameChecksum = fileTreeReader.ReadUInt32();
						byte[] fileNameBytes = fileTreeReader.ReadBytes(fileTreeReader.ReadInt32());
						int dataLen = fileTreeReader.ReadInt32();
						uint checksum = fileTreeReader.ReadUInt32();
						int offset = fileTreeReader.ReadInt32();
						PvfFile pvfFile = new PvfFile(fileNameChecksum, fileNameBytes, dataLen, checksum, offset);
						if (pvfFile.FileName.Split(new char[2] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Contains("506807329_"))
						{
							continue;
						}
						if (hasFooterSignature || !base.FileList.ContainsKey(pvfFile.FileName))
						{
							base.FileList.Add(pvfFile.FileName, pvfFile);
						}
						else
						{
							string text = pvfFile.FileName;
							while (base.FileList.ContainsKey(text))
							{
								text += "(diff)";
							}
							base.FileList.Add(text, pvfFile);
							duplicateFiles.Add(pvfFile);
						}
						if (i % 512 == 0)
						{
							progress?.Report(ProgressHelper.GetProgressNum(i, fileCount));
						}
					}
					long dataStartOffset = binaryReader.BaseStream.Position;
					foreach (KeyValuePair<string, PvfFile> item in base.FileList.Where<KeyValuePair<string, PvfFile>>((KeyValuePair<string, PvfFile> item) => item.Value.DataLen > 0))
					{
						item.Value.Offset += dataStartOffset;
						binaryReader.BaseStream.Seek(item.Value.Offset, SeekOrigin.Begin);
						item.Value.InitFile(binaryReader.ReadBytes(item.Value.GetBlockLength()));
						if (!hasFooterSignature && duplicateFiles.Contains(item.Value))
						{
							item.Value.Rename(item.Key);
						}
					}
					progress?.Report(100.0);
				}
				Task.Run(delegate
				{
					if (GetFile("stringtable.bin") != null)
					{
						base.Strtable.Loadstringtable(GetFile("stringtable.bin").Data, base.OverAllEncodingType, this);
					}
					else
					{
						base.Strtable.InitDefault();
							logger.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringTableNotFound"));
					}
					if (FileAny(AppSetting.Instance.PvfConfig.StringLstFileName))
					{
						base.Strview.InitStringData(GetFile(AppSetting.Instance.PvfConfig.StringLstFileName), this, base.OverAllEncodingType);
					}
					else
					{
						base.Strview.InitDefault();
							logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLstNotFound"), AppSetting.Instance.PvfConfig.StringLstFileName));
					}
					Task.Run(delegate
					{
						GetEquipmentpartsetInfo(this, out Dictionary<int, Dictionary<string, EquipmentPartSet>> dic);
						base.EquipmentPartSetTable.Init(this, dic);
					});
					Task.Run(delegate
					{
						this.Init();
					});
				});
				return Task.FromResult(result: true);
			}
			catch (Exception ex)
			{
				logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_OpenPackageError"), ex.Message));
				return Task.FromResult(result: false);
			}
		}
	}

	/// <summary>Pvf110 Builder 保护链打开：Pvf110Reader → FileList（懒加载，body 按需读取）。</summary>
	private Task<bool> OpenPvfPack110(Pvf110Reader reader, string path, IProgress<double> progress)
	{
		try
		{
			_pvf110 = reader;
			// 本次会话必然要为全部条目解析路径（建 FileList 字典键），开启记忆化：
			// 打开/保存/名称扫描都复用同一批字符串，省掉百万级条目的重复路径解析。
			reader.PathMemoEnabled = true;
			int count = reader.Entries.Count;
			_entryIndex = new Dictionary<string, int>(count);

			Dictionary<string, PvfFile> list = new Dictionary<string, PvfFile>(count);
			for (int i = 0; i < count; i++)
			{
				Pvf110Entry e = reader.Entries[i];
				string p = reader.FilePath(e);
				PvfFile file = new PvfFile
				{
					FileNameEncoding = System.Text.Encoding.UTF8,
					Pvf110DataType = e.DataType,
					IsNewFile = false, // 归档既有条目：非新建文件（区别于导入/新建）
				};
				file.SetFileNameInitial(p);
				file.SetLoadedContent(Array.Empty<byte>());
				list[file.FileName] = file;
				_entryIndex[file.FileName] = i;
				if ((i & 1023) == 0) progress?.Report(ProgressHelper.GetProgressNum(i, count));
			}

			base.FileList = list;
			// 统一管线：名称池 → 虚拟串表；之后所有解析/编辑/名称功能走经典管线
			AppSetting.Instance.PvfConfig.DefaultEncoding = EncodingType.UTF8;
			InitClassicViewInfrastructure(reader);
			base.PvfIsOpen = true;
			base.HasUnsavedChanges = false;
			RunClassicPostOpenInit();
			progress?.Report(100.0);
			return Task.FromResult(true);
		}
		catch (Exception ex)
		{
			logger.Error("Pvf110 open error: " + ex.Message);
			return Task.FromResult(false);
		}
	}

	/// <summary>NKPI / ProtectedNKPI 打开：NkpiReader → FileList（懒加载，body 按需读取）。</summary>
	private Task<bool> OpenPvfPackNkpi(NkpiReader reader, string path, IProgress<double> progress)
	{
		try
		{
			_nkpi = reader;
			int count = reader.Entries.Count;
			_entryIndex = new Dictionary<string, int>(count);

			// 只建路径索引，不读 body（懒加载）
			Dictionary<string, PvfFile> list = new Dictionary<string, PvfFile>(count);
			for (int i = 0; i < count; i++)
			{
				NkpiEntry e = reader.Entries[i];
				string p = reader.FilePath(e);
				PvfFile file = new PvfFile
				{
					FileNameEncoding = System.Text.Encoding.UTF8,
					Pvf110DataType = e.DataType,
					IsNewFile = false, // 归档既有条目：非新建文件（区别于导入/新建）
				};
				file.SetFileNameInitial(p);
				// Data 留空，后续按需加载
				file.SetLoadedContent(Array.Empty<byte>());
				list[file.FileName] = file;
				_entryIndex[file.FileName] = i;
				if ((i & 1023) == 0) progress?.Report(ProgressHelper.GetProgressNum(i, count));
			}

			base.FileList = list;
			// 统一管线：名称池 → 虚拟串表；之后所有解析/编辑/名称功能走经典管线
			// （NKPI/ProtectedNKPI 使用 UTF-8，非 TW/Big5；直接设 DefaultEncoding 避免 DoNotify 跨线程问题）
			AppSetting.Instance.PvfConfig.DefaultEncoding = EncodingType.UTF8;
			InitClassicViewInfrastructure(reader);
			base.PvfIsOpen = true;
			base.HasUnsavedChanges = false;
			RunClassicPostOpenInit();
			progress?.Report(100.0);
			return Task.FromResult(true);
		}
		catch (Exception ex)
		{
			logger.Error("NKPI open error: " + ex.Message);
			return Task.FromResult(false);
		}
	}

	/// <summary>
	/// 统一管线适配层（读侧）：用名称池构建虚拟串表与"池偏移 → 虚拟 ID"映射。
	/// 串表文本不做传统/简体转换，与名称池原始字节一致；遍历顺序与
	/// Pvf110Compiled.BuildPoolIndex 一致（UTF-16 池优先，含空串）。
	/// 90CN 无 StringLink token（全量普查 0x08/0x09 零出现），Strview 保持空表。
	/// </summary>
	private void InitClassicViewInfrastructure(IPvfNamePool reader)
	{
		_classicViewUtf8Pool = reader.Utf8Pool;
		_classicViewUtf16Pool = reader.Utf16Pool;
		base.Strtable.LoadFromNamePool(EnumeratePoolStrings(reader));
		Dictionary<int, int> map = new Dictionary<int, int>();
		MapPoolOffsets(map, reader.Utf16Pool, isUtf16: true);
		MapPoolOffsets(map, reader.Utf8Pool, isUtf16: false);
		_poolOffsetToVirtualId = map;
		base.Strview.InitDefault();
	}

	private static IEnumerable<string> EnumeratePoolStrings(IPvfNamePool reader)
	{
		byte[] utf16 = reader.Utf16Pool;
		int pos = 0;
		while (pos + 1 < utf16.Length)
		{
			int end = pos;
			while (end + 1 < utf16.Length && !(utf16[end] == 0 && utf16[end + 1] == 0)) end += 2;
			yield return end == pos ? string.Empty : Encoding.Unicode.GetString(utf16, pos, end - pos);
			pos = end + 2;
		}
		byte[] utf8 = reader.Utf8Pool;
		pos = 0;
		while (pos < utf8.Length)
		{
			int end = Array.IndexOf(utf8, (byte)0, pos);
			if (end < 0) end = utf8.Length;
			if (end > pos) yield return Encoding.UTF8.GetString(utf8, pos, end - pos);
			pos = end + 1;
		}
	}

	private void MapPoolOffsets(Dictionary<int, int> map, byte[] pool, bool isUtf16)
	{
		int pos = 0;
		while (pos < pool.Length && (!isUtf16 || pos + 1 < pool.Length))
		{
			int end;
			if (isUtf16)
			{
				end = pos;
				while (end + 1 < pool.Length && !(pool[end] == 0 && pool[end + 1] == 0)) end += 2;
				if (end == pos)
				{
					// 空串（两个连续 0 字节）：magic = (byteOffset/2)*2 | 1
					int emptyId = base.Strtable.GetStringTableId(string.Empty);
					if (emptyId != -1) map[(pos / 2) * 2 | 1] = emptyId;
					pos += 2;
					continue;
				}
			}
			else
			{
				end = Array.IndexOf(pool, (byte)0, pos);
				if (end < 0) end = pool.Length;
				if (end == pos)
				{
					pos = end + 1;
					continue;
				}
			}
			string text = isUtf16 ? Encoding.Unicode.GetString(pool, pos, end - pos) : Encoding.UTF8.GetString(pool, pos, end - pos);
			int id = base.Strtable.GetStringTableId(text);
			if (id != -1)
			{
				map[isUtf16 ? ((pos / 2) * 2 | 1) : (pos * 2)] = id;
			}
			pos = end + (isUtf16 ? 2 : 1);
		}
	}

	/// <summary>名称池偏移 → 文本（与 Pvf110Compiled.Resolve 相同的 magic 偏移语义）。</summary>
	private string ResolvePoolText(int magicOffset)
	{
		uint u = unchecked((uint)magicOffset);
		byte[] utf8 = _classicViewUtf8Pool ?? Array.Empty<byte>();
		byte[] utf16 = _classicViewUtf16Pool ?? Array.Empty<byte>();
		if ((u & 1) == 0)
		{
			int pos = (int)(u >> 1);
			if (pos < 0 || pos >= utf8.Length) return string.Empty;
			int end = Array.IndexOf(utf8, (byte)0, pos);
			if (end < 0) end = utf8.Length;
			return Encoding.UTF8.GetString(utf8, pos, end - pos);
		}
		int pos16 = (int)((u >> 1) * 2);
		if (pos16 < 0 || pos16 + 1 >= utf16.Length) return string.Empty;
		int end16 = pos16;
		while (end16 + 1 < utf16.Length && !(utf16[end16] == 0 && utf16[end16 + 1] == 0)) end16 += 2;
		return Encoding.Unicode.GetString(utf16, pos16, end16 - pos16);
	}

	/// <summary>名称池偏移 → 虚拟串表 ID；未映射偏移（打开早期/池外）按文本解析兜底。</summary>
	private int AcquireVirtualIdByOffset(int poolOffset)
	{
		if (_poolOffsetToVirtualId.TryGetValue(poolOffset, out int id)) return id;
		string text = ResolvePoolText(poolOffset);
		int virtualId = base.Strtable.GetStringTableId(text);
		if (virtualId == -1) virtualId = base.Strtable.AddStringItem(text);
		return virtualId;
	}

	/// <summary>与经典格式打开一致的后台初始化：套装表 + LST 代码表（名称/LST 工具依赖）。</summary>
	private void RunClassicPostOpenInit()
	{
		Task.Run(delegate
		{
			GetEquipmentpartsetInfo(this, out Dictionary<int, Dictionary<string, EquipmentPartSet>> dic);
			base.EquipmentPartSetTable.Init(this, dic);
		});
		Task.Run(delegate
		{
			this.Init();
		});
	}

	/// <summary>
	/// 懒加载：按路径从 Pvf110/NKPI 读取器取回文件内容并写入 PvfFile.Data。
	/// 统一管线：type-1 条目存"经典视图"token 流（0xD0B0 魔数 + 经典 ScriptType + 虚拟串表 ID），
	/// IsScriptFile 成立，富格式反编译/名称扫描/注释/IMG 链接/LST 工具全部走旧版管线；
	/// type-3 保持 UTF-16LE 原文。已在读取器里缓存 group（容量受限于 LRU），不会重复解压。
	/// </summary>
	public bool EnsureFileData(string path)
	{
		if (base.FileList == null) return false;
		if (!base.FileList.TryGetValue(path, out PvfFile? file)) return false;
		// 已有数据（空文件或已加载）则跳过
		if (file.Data != null && file.Data.Length > 0) return true;
		if (!_entryIndex.TryGetValue(path, out int idx)) return false;

		try
		{
			if (_nkpi != null && idx < _nkpi.Entries.Count)
			{
				NkpiEntry e = _nkpi.Entries[idx];
				byte[] content = _nkpi.ReadEntry(e);
				byte[] data = (e.DataType == 1)
					? ClassicViewAdapter.ToClassicView(content, off => AcquireVirtualIdByOffset(off))
					: content; // type-3 = UTF-16LE
				file.SetLoadedContent(data);
				return true;
			}
			if (_pvf110 != null && idx < _pvf110.Entries.Count)
			{
				Pvf110Entry e = _pvf110.Entries[idx];
				byte[] content = _pvf110.ReadEntry(e);
				byte[] data = (e.DataType == 1)
					? ClassicViewAdapter.ToClassicView(content, off => AcquireVirtualIdByOffset(off))
					: content; // type-3 = UTF-16LE
				file.SetLoadedContent(data);
				return true;
			}
		}
		catch (Exception ex)
		{
			logger.Error($"EnsureFileData failed for {path}: {ex.Message}");
		}
		return false;
	}

	/// <summary>
	/// 扫描/名称解析用内容（统一管线）：110/NKPI type-1 返回经典视图 token 流
	/// （未修改文件即时转换、不落地 Data，全量名称扫描不占常驻内存；已修改文件的
	/// Data 本身就是经典视图）；经典格式与未映射文件回退到已加载的 Data。
	/// </summary>
	public byte[] GetBinaryForScan(PvfFile file)
	{
		if (file == null) return Array.Empty<byte>();
		if (!Is110Format || file.Pvf110DataType == 0)
			return file.Data ?? Array.Empty<byte>();
		if (!_entryIndex.TryGetValue(file.FileName, out int idx))
			return file.Data ?? Array.Empty<byte>();
		try
		{
			if (_nkpi != null && idx < _nkpi.Entries.Count)
				return GetScanBinary(_nkpi, _nkpi.Entries[idx], file);
			if (_pvf110 != null && idx < _pvf110.Entries.Count)
				return GetScanBinary(_pvf110, _pvf110.Entries[idx], file);
		}
		catch (Exception ex)
		{
			logger.Error($"GetBinaryForScan failed for {file.FileName}: {ex.Message}");
		}
		return file.Data ?? Array.Empty<byte>();
	}

	private byte[] GetScanBinary(NkpiReader reader, NkpiEntry e, PvfFile file)
	{
		if (e.DataType != 1)
		{
			if (file.IsContentModified && file.Data is { Length: > 0 }) return file.Data;
			return reader.ReadEntry(e);
		}
		if (file.IsContentModified)
		{
			// 已修改文件：Data 即经典视图（编辑器经 ScriptFileCompilerOl 写入）
			if (file.Data is not { Length: > 0 }) EnsureFileData(file.FileName);
			return file.Data ?? reader.ReadEntry(e);
		}
		// 未修改：即时转换经典视图（不落地 Data）
		return ClassicViewAdapter.ToClassicView(reader.ReadEntry(e), off => AcquireVirtualIdByOffset(off));
	}

	private byte[] GetScanBinary(Pvf110Reader reader, Pvf110Entry e, PvfFile file)
	{
		if (e.DataType != 1)
		{
			if (file.IsContentModified && file.Data is { Length: > 0 }) return file.Data;
			return reader.ReadEntry(e);
		}
		if (file.IsContentModified)
		{
			// 已修改文件：Data 即经典视图（编辑器经 ScriptFileCompilerOl 写入）
			if (file.Data is not { Length: > 0 }) EnsureFileData(file.FileName);
			return file.Data ?? reader.ReadEntry(e);
		}
		// 未修改：即时转换经典视图（不落地 Data）
		return ClassicViewAdapter.ToClassicView(reader.ReadEntry(e), off => AcquireVirtualIdByOffset(off));
	}

	/// <summary>检查文件内容是否已加载（懒加载标记）。</summary>
	public bool IsFileDataLoaded(string path)
	{
		return base.FileList != null
			&& base.FileList.TryGetValue(path, out PvfFile? file)
			&& file.Data != null
			&& file.Data.Length > 0;
	}

	public bool GetEquipmentpartsetInfo(PvfPack pvf, out Dictionary<int, Dictionary<string, EquipmentPartSet>> dic)
	{
		dic = new Dictionary<int, Dictionary<string, EquipmentPartSet>>();
		// 懒加载：确保 equipmentpartset.etc 已加载
		EnsureFileData("etc/equipmentpartset.etc");
		PvfFile file = GetFile("etc/equipmentpartset.etc");
		if (file == null || file.Data == null || file.Data.Length == 0)
		{
			return false;
		}
		string equipmentDirectory = "equipment/";
		ScriptFileParserNew parser = new ScriptFileParserNew(file, this);
		parser.PraseStructureMain();
		if (parser.Sections == null || parser.Sections.Count == 0)
		{
			return false;
		}
		StringBuilder errors = new StringBuilder();
		foreach (PvfSection partSetSection in parser.Sections.Where((SectionBase section) => section is PvfSection && section.GetSectionName() == "[equipment part set]"))
		{
			int childCount = partSetSection.Children.Count;
			if (partSetSection.HasEndSection())
			{
				childCount--;
			}
			List<SectionBase> children = partSetSection.Children;
			if (childCount <= 3)
			{
				continue;
			}
			if (children[1].Item.Type != ScriptType.Int)
			{
				ScriptItem linkedItem = children[1].Item.Type == ScriptType.StringLinkIndex ? partSetSection.Children[2].Item : null;
				errors.AppendLine($"套装文件错误 [equipment part set]内应以数字编号开头 错误类型为：{children[1].Item.Type} 值：{children[1].Item.GetItemText(this, linkedItem)}");
				continue;
			}
			int setIndex = children[1].Item.Data;
			if (children[2].Item.Type != ScriptType.String)
			{
				ScriptItem linkedItem = children[2].Item.Type == ScriptType.StringLinkIndex ? partSetSection.Children[3].Item : null;
				errors.AppendLine($"套装文件错误 [equipment part set]内第二个参数应为String型 错误类型：{children[2].Item.Type} 值：{children[2].Item.GetItemText(this, linkedItem)}");
				continue;
			}
			string partSetFilePath = equipmentDirectory + pvf.Strtable.GetStringItem(children[2].Item.Data);
			PvfFile partSetFile = pvf.GetFile(partSetFilePath);
			if (partSetFile == null)
			{
				errors.AppendLine("套装文件错误 找不到套装信息文件：" + partSetFilePath);
				continue;
			}
			int fieldIndex = 0;
			Dictionary<string, EquipmentPartSet> partsByEquipmentType = new Dictionary<string, EquipmentPartSet>();
			EquipmentPartSet partSet = new EquipmentPartSet();
			// 版式自适应（专项解析，多版本兼容）：
			// 经典版式   children[3] 起每 4 token 一行 (部件名, [类型], int, int)；
			// 90CN 扩展  children[3] 为套装名，行从 children[4] 起（同样 4 token 一行）。
			// 行首名称可能是 `[活动]…` 等方括号文本甚至空串，无法用 [ 前缀判别，
			// 因此对两个候选起点做整段行版式走查，取违例更少的起点（平手取经典起点 3）。
			int rowStart = 3;
			int violationsAt3 = MeasurePartSetRowLayout(children, 3, childCount);
			int violationsAt4 = MeasurePartSetRowLayout(children, 4, childCount);
			if (violationsAt4 < violationsAt3)
			{
				rowStart = 4;
			}
			for (int childIndex = rowStart; childIndex < childCount; childIndex++)
			{
				SectionBase child = children[childIndex];
				if (child == null || child is PvfSection)
				{
					continue;
				}
				fieldIndex++;
				switch (fieldIndex)
				{
				case 1:
				{
					if (children[childIndex].Item.Type != ScriptType.String && children[childIndex].Item.Type != ScriptType.StringLinkIndex)
					{
						ScriptItem linkedItem = ScriptLinkText.TryGetLinkedLiteral(children, childIndex, childCount);
						errors.AppendLine($"套装文件错误 套装信息从第二行开始 第一个参数为 String型 当前类型：{children[childIndex].Item.Type} 值：{children[childIndex].Item.GetItemText(this, linkedItem)}");
						break;
					}
					// 链接元数自适应：经典 2 token（链接+名称字面量）／110 适配 1 token（链接自带 <ns::key>）。
					// 旧实现一律取后一项，115 数据会把类型字段当成名称并跳过它，导致整段行错位。
					ScriptItem? linkedNameItem = ScriptLinkText.TryGetLinkedLiteral(children, childIndex, childCount);
					partSet = new EquipmentPartSet
					{
						Name = ScriptLinkText.Resolve(this, child.Item, linkedNameItem),
						ParFile = partSetFile
					};
					if (linkedNameItem != null)
					{
						childIndex++;
					}
					continue;
				}
				case 2:
				{
					if (children[childIndex].Item.Type != ScriptType.String)
					{
						ScriptItem linkedItem = ScriptLinkText.TryGetLinkedLiteral(children, childIndex, childCount);
						errors.AppendLine($"套装文件错误 套装信息从第二行开始 第二个参数为 String型 当前类型：{children[childIndex].Item.Type} 值：{children[childIndex].Item.GetItemText(this, linkedItem)}");
						break;
					}
					partSet.EquType = children[childIndex].Item.GetItemTextNotChar(this);
					if (string.IsNullOrEmpty(partSet.EquType))
					{
						errors.AppendLine($"套装文件错误 套装文件的装备类型不能为空！ 套装索引：{setIndex}");
						break;
					}
					// 90CN(partset2 家族等) 允许同一套装内出现多个同 [类型] 部件（如两套项链/戒指/手镯变体），
					// 类型重复是合法数据，不再报错；表引用按类型归并到首个条目。
					continue;
				}
				case 3:
				{
					if (children[childIndex].Item.Type == ScriptType.Int)
					{
						continue;
					}
					ScriptItem linkedItem = ScriptLinkText.TryGetLinkedLiteral(children, childIndex, childCount);
					errors.AppendLine($"套装文件错误 套装信息从第二行开始 第3个参数为 int型 当前类型：{children[childIndex].Item.Type} 值：{children[childIndex].Item.GetItemText(this, linkedItem)} 套装索引：{setIndex}");
					break;
				}
				case 4:
					if (children[childIndex].Item.Type != ScriptType.Int)
					{
						ScriptItem linkedItem = ScriptLinkText.TryGetLinkedLiteral(children, childIndex, childCount);
						errors.AppendLine($"套装文件错误 套装信息从第二行开始 第4个参数为 int型 当前类型：{children[childIndex].Item.Type} 值：{children[childIndex].Item.GetItemText(this, linkedItem)}");
						break;
					}
					fieldIndex = 0;
					if (!partsByEquipmentType.ContainsKey(partSet.EquType))
					{
						partsByEquipmentType.Add(partSet.EquType, partSet);
					}
					continue;
				default:
					continue;
				}
				break;
			}
			if (partsByEquipmentType.Count > 0)
			{
				if (dic.ContainsKey(setIndex))
				{
					errors.AppendLine($"套装文件错误 套装索引编号重复：{setIndex}");
				}
				else
				{
					dic.Add(setIndex, partsByEquipmentType);
				}
			}
		}
		if (errors.Length > 0)
		{
			AppSetting.Instance.GetIlogger()?.Error(errors.ToString());
		}
		return dic.Count > 0;
	}

	/// <summary>
	/// 套装行版式走查（专项解析的版式判别）：从 <paramref name="start"/> 起按
	/// (String, String, Int, Int) 四字段一行模拟消费 children（跳过嵌套段），
	/// 返回类型违例数；悬挂的未收口字段额外计 1。违例越少说明该起点越可能是
	/// 真实行起点（经典=3，90CN 含套装名=4）。
	/// </summary>
	private static int MeasurePartSetRowLayout(List<SectionBase> children, int start, int count)
	{
		int violations = 0;
		int fieldIndex = 0;
		for (int i = start; i < count; i++)
		{
			SectionBase child = children[i];
			if (child == null || child is PvfSection)
			{
				continue;
			}
			ScriptItem? item = child.Item;
			if (item == null)
			{
				violations++;
				continue;
			}
			fieldIndex++;
			switch (fieldIndex)
			{
			case 1:
			case 2:
				if (item.Type != ScriptType.String && item.Type != ScriptType.StringLinkIndex)
				{
					violations++;
				}
				// 与主解析循环保持一致：2 token 版式的名称字面量随链接项一并消费，不占字段位
				if (fieldIndex == 1 && item.Type == ScriptType.StringLinkIndex)
				{
					i += ScriptLinkText.ExtraLinkedLiteralCount(children, i, count);
				}
				break;
			case 3:
			case 4:
				if (item.Type != ScriptType.Int)
				{
					violations++;
				}
				break;
			}
			if (fieldIndex == 4)
			{
				fieldIndex = 0;
			}
		}
		if (fieldIndex != 0)
		{
			violations++;
		}
		return violations;
	}

	public bool Getavatar_select_abilityItems(PvfFile file, out List<avatar_select_ability_Base> items)
	{
		items = new List<avatar_select_ability_Base>();
		byte[] data = file.Data;
		int dataLen = file.DataLen;
		int stringTableId = base.Strtable.GetStringTableId("[avatar select ability]");
		if (stringTableId == -1)
		{
			return false;
		}
		if (data == null || dataLen < 7)
		{
			return false;
		}
		for (int i = 2; i < dataLen - 4; i += 5)
		{
			if (data[i] != 5)
			{
				continue;
			}
			int startIndex = i + 1;
			if (BitConverter.ToInt32(data, startIndex) != stringTableId)
			{
				continue;
			}
			for (int j = i + 5; j < dataLen - 4; j += 5)
			{
				if (data[j] == 5)
				{
					return items.Count > 0;
				}
				string command = base.Strtable.GetStringItem(BitConverter.ToInt32(data, j + 1));
				if (command == "[SKILL_LEVEL]")
				{
					if (j + 15 < dataLen)
					{
						if (data[j + 5] != 7)
						{
							return items.Count > 0;
						}
						if (data[j + 10] != 2)
						{
							return items.Count > 0;
						}
						if (data[j + 15] != 2)
						{
							return items.Count > 0;
						}
						string jobType = base.Strtable.GetStringItem(BitConverter.ToInt32(data, j + 6));
						int skillId = BitConverter.ToInt32(data, j + 11);
						int level = BitConverter.ToInt32(data, j + 16);
						avatar_select_ability_Skill skillAbility = new avatar_select_ability_Skill(command, jobType, skillId, level);
						GetJobSkillName(jobType, skillId, out string skillName);
						skillAbility.SkillName = skillName;
						items.Add(skillAbility);
						j += 15;
					}
				}
				else if (j + 10 < dataLen)
				{
					if (data[j + 5] != 7)
					{
						return items.Count > 0;
					}
					if (data[j + 10] != 2)
					{
						return items.Count > 0;
					}
					string addType = base.Strtable.GetStringItem(BitConverter.ToInt32(data, j + 6));
					int value = BitConverter.ToInt32(data, j + 11);
					items.Add(new avatar_select_ability(command, addType, value));
					j += 10;
				}
			}
			return items.Count > 0;
		}
		return items.Count > 0;
	}

	public override string GetItemName(string filePath)
	{
		base.FileList.TryGetValue(filePath, out PvfFile file);
		return GetItemName(file);
	}

	/// <summary>当前包为 Pvf110 / NKPI / ProtectedNKPI 新格式（统一管线下 Data 为经典视图）。</summary>
	public bool Is110Format => IsPvf110 || _nkpi != null;

	public override string GetItemName(PvfFile? file)
	{
		if (file == null)
		{
			return null;
		}
		if (base.Strtable == null)
		{
			return null;
		}
		if (!base.Strtable.IsAny())
		{
			return null;
		}
		// 懒加载模式：扫描用原始编译内容（不把文本落地到 Data）
		byte[] scan = GetBinaryForScan(file);
		if (scan.Length < 2 || BitConverter.ToUInt16(scan, 0) != 53424)
		{
			return null;
		}
		PvfFileType fileType = file.FileType;
		string itemName;
		switch (fileType)
		{
		case PvfFileType.shp:
			itemName = GetShopName(file, scan);
			break;
		case PvfFileType.equ:
			itemName = GetNameByLabels(base.Strtable.NameLableOrSetNameLable, scan);
			break;
		default:
		{
			int nameLable = base.Strtable.GetNameLable(fileType);
			itemName = GetNameByLabel(nameLable, scan);
			break;
		}
		}
		if (!string.IsNullOrEmpty(itemName))
		{
			return AppSetting.Instance.PvfConfig.FileTextTraditionalConvertSimplifiedAutoMethods(itemName);
		}
		return null;
	}

	private string GetNameByLabel(int nameLabel, byte[] data)
	{
		int dataLen = data.Length;
		for (int offset = 2; offset < dataLen - 9; offset += 5)
		{
			if (data[offset] == 5 && BitConverter.ToInt32(data, offset + 1) == nameLabel)
			{
				if (offset < dataLen - 14 && data[offset + 5] == 9 && data[offset + 10] == 10)
				{
					return base.Strview.GetStrText(data[offset + 6], base.Strtable.GetStringItem(BitConverter.ToInt32(data, offset + 11)));
				}
				if (data[offset + 5] == 7)
				{
					return base.Strtable.GetStringItem(BitConverter.ToInt32(data, offset + 6));
				}
			}
		}
		return null;
	}

	private string GetNameByLabels(HashSet<int> nameLabels, byte[] data)
	{
		int dataLen = data.Length;
		for (int offset = 2; offset < dataLen - 9; offset += 5)
		{
			if (data[offset] == 5 && nameLabels.Contains(BitConverter.ToInt32(data, offset + 1)))
			{
				if (offset < dataLen - 14 && data[offset + 5] == 9 && data[offset + 10] == 10)
				{
					return base.Strview.GetStrText(data[offset + 6], base.Strtable.GetStringItem(BitConverter.ToInt32(data, offset + 11)));
				}
				if (data[offset + 5] == 7)
				{
					return base.Strtable.GetStringItem(BitConverter.ToInt32(data, offset + 6));
				}
			}
		}
		return null;
	}

	private string GetShopName(PvfFile? file, byte[] scan)
	{
		if (file.GetNpcId(this, scan, out var npcId) && base.ListFileTable.CodeDic.TryGetValue("npc", out Dictionary<int, LstItem> npcFiles) && npcFiles.TryGetValue(npcId, out var npcFile))
		{
			string shopName = GetItemName(GetFile(npcFile.FullPath));
			if (shopName != null)
			{
				shopName = AppSetting.Instance.GetIlogger()?.GetStr("mess_Shop") + "-" + shopName;
			}
			return shopName;
		}
		return null;
	}

	public void DeleteFile(string filePath)
	{
		if (base.FileList.ContainsKey(filePath))
		{
			base.FileList.Remove(filePath);
			base.HasUnsavedChanges = true;
		}
	}

	public DeleteFilesResultDto WebApiDeleteFiles(IEnumerable<string> files)
	{
		DeleteFilesResultDto deleteFilesResultDto = new DeleteFilesResultDto();
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (string file in files)
		{
			if (base.FileList.ContainsKey(file))
			{
				base.FileList.Remove(file);
				list.Add(file);
				base.HasUnsavedChanges = true;
			}
			else
			{
				list2.Add(file);
			}
		}
		deleteFilesResultDto.SuccessFiles = list;
		deleteFilesResultDto.ErrorFiles = list2;
		return deleteFilesResultDto;
	}

	public void DeleteFiles(IEnumerable<string> filePaths)
	{
		foreach (string filePath in filePaths)
		{
			DeleteFile(filePath);
		}
	}

	public void RenameFile(string olFilPath, string newFilePath)
	{
		PvfFile file = GetFile(olFilPath);
		if (file == null)
		{
			logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_RenameError"), olFilPath));
			return;
		}
		base.FileList.Remove(olFilPath);
		file.Rename(newFilePath);
		base.FileList.Add(file.FileName, file);
		base.HasUnsavedChanges = true;
	}

	public void RenameFolder(string olPath, string olFolderName, string newName)
	{
		int num = olPath.LastIndexOf("/");
		string path = newName;
		if (num != -1)
		{
			path = olPath.Remove(num, olFolderName.Length + 1) + "/" + newName;
		}
		path = PathsHelper.PathFix(path);
		string text = PathsHelper.PathFix(olPath);
		PvfFile[] fileObjs = GetFileObjs(text);
		int length = text.Length;
		PvfFile[] array = fileObjs;
		foreach (PvfFile pvfFile in array)
		{
			string fileName = pvfFile.FileName;
			base.FileList.Remove(fileName);
			string newFileName = path + fileName.Remove(0, length);
			pvfFile.Rename(newFileName);
			base.FileList.Add(pvfFile.FileName, pvfFile);
		}
		base.HasUnsavedChanges = true;
	}

	public bool ExtractFile(Stream stream, PvfFile file, bool decompileBinaryAni, bool decompileScript, bool convertConvertSimplifiedChinese, bool isOlWebApi = false, bool? useCompatibleDecompiler = null)
	{
		if (file == null)
		{
			return false;
		}
		// 懒加载：未访问过的文件按需取回内容；只读导出不把内容留在 Data 中
		bool loadedHere = false;
		if ((file.Data == null || file.Data.Length == 0) && file.Pvf110DataType > 0)
		{
			loadedHere = EnsureFileData(file.FileName);
		}
		try
		{
			return ExtractFileCore(stream, file, decompileBinaryAni, decompileScript, convertConvertSimplifiedChinese, isOlWebApi, useCompatibleDecompiler);
		}
		finally
		{
			if (loadedHere)
			{
				file.ReleaseLoadedContent();
			}
		}
	}

	private bool ExtractFileCore(Stream stream, PvfFile file, bool decompileBinaryAni, bool decompileScript, bool convertConvertSimplifiedChinese, bool isOlWebApi = false, bool? useCompatibleDecompiler = null)
	{
		if (file.DataLen <= 0)
		{
			return true;
		}
		// 统一管线：type-3（UTF-16 文本）原样导出；type-1 的 Data 是经典视图，
		// 落入下方 IsScriptFile 反编译路径，导出与经典格式一致的脚本文本。
		if (Is110Format && file.Pvf110DataType == 3)
		{
			byte[] raw = file.Data ?? Array.Empty<byte>();
			stream.Write(raw, 0, raw.Length);
			stream.Seek(0L, SeekOrigin.Begin);
			return true;
		}
		if (file.IsBinaryAniFile && decompileBinaryAni)
		{
			var (flag, s) = BinaryAniCompiler.DecompileBinaryAni(file);
			if (flag)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
				stream.Seek(0L, SeekOrigin.Begin);
			}
			return flag;
		}
		if (file.IsScriptFile && decompileScript)
		{
			if (!useCompatibleDecompiler.HasValue)
			{
				useCompatibleDecompiler = ((!isOlWebApi) ? new bool?(AppSetting.Instance.PvfConfig.UseCompatibleDecompiler) : ((!AppSetting.Instance.ClientApiOptions.UseCompatibleDecompiler) ? new bool?(AppSetting.Instance.PvfConfig.UseCompatibleDecompiler) : new bool?(true)));
			}
			string text = (useCompatibleDecompiler.Value ? new ScriptFileCompilerOl(this).Decompile(file) : new ScriptFileParserNew(file, this).PraseText());
			if (convertConvertSimplifiedChinese)
			{
				text = ChineseHelper.ToSimplified(text);
			}
			byte[] bytes2 = Encoding.UTF8.GetBytes(text);
			stream.Write(bytes2, 0, bytes2.Length);
			stream.Seek(0L, SeekOrigin.Begin);
			return true;
		}
		if (AppSetting.Instance.PvfConfig.DefaultEncoding == EncodingType.TW && convertConvertSimplifiedChinese)
		{
			string source = Encoding.GetEncoding(950).GetString(file.Data);
			source = ChineseHelper.ToSimplified(source);
			byte[] bytes3 = Encoding.UTF8.GetBytes(source);
			stream.Write(bytes3, 0, bytes3.Length);
			stream.Seek(0L, SeekOrigin.Begin);
		}
		else
		{
			stream.Write(file.Data, 0, file.DataLen);
			stream.Seek(0L, SeekOrigin.Begin);
		}
		return true;
	}

	public override bool SaveFileText(PvfFile file, string fileText, EncodingType? encoding = null)
	{
		if (!encoding.HasValue)
		{
			encoding = base.OverAllEncodingType;
		}
		if (file.FileType == PvfFileType.str)
		{
			fileText = AppSetting.Instance.PvfConfig.StrTableAndStrViewConvertStrContent(fileText);
		}
		if (file.IsScriptFile)
		{
			if (fileText.Length >= 9 && fileText.Substring(0, 9) == "#PVF_File")
			{
				return SaveFileAsScript(file, fileText);
			}
			if (logger.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_SaveAsScript")) == MessageResult.Yes)
			{
				return SaveFileAsScript(file, fileText);
			}
			return SaveFileAsTextFile(file, fileText, encoding.Value);
		}
		if (file.IsBinaryAniFile)
		{
			return SaveFileAsBinaryAni(file, fileText);
		}
		if (fileText.Length > 10 && fileText.Substring(0, 9) == "#PVF_File")
		{
			return SaveFileAsScript(file, fileText);
		}
		return SaveFileAsTextFile(file, fileText, encoding.Value);
	}

	public override bool SaveFileText(string filePath, string fileText, EncodingType? encoding = null)
	{
		PvfFile file = GetFile(filePath);
		if (file == null)
		{
			logger?.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SaveFileError"), filePath));
			return false;
		}
		return SaveFileText(file, fileText, encoding);
	}

	public bool SaveFileAsScript(PvfFile file, string fileText)
	{
		// 统一管线：编辑文本经经典编译器生成经典视图 token 存入 Data；
		// 110/NKPI 包保存时再由 CompileModifiedContentForSave 适配回 110 token。
		byte[] array = new ScriptFileCompilerOl(this).Compile(file, fileText);
		if (array != null)
		{
			file.WriteFileData(array);
			base.HasUnsavedChanges = true;
			if (file.FileType == PvfFileType.lst)
			{
				if (file.FileName == "n_string.lst")
				{
					base.Strview.InitStringData(file, this, base.OverAllEncodingType);
					return true;
				}
				if (file.FileName.IndexOf('/') < 0)
				{
					return true;
				}
				ServiceItemCodeTable.LoadLstFile(file, this);
				logger?.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ReloadLstSuccess"), file.FileName));
			}
			return true;
		}
		return false;
	}

	public bool SaveFileAsBinaryAni(PvfFile file, string fileText)
	{
		var (flag, fileData, item) = BinaryAniCompiler.CompileBinaryAni(fileText, file.FileName);
		if (!flag)
		{
			logger.Error(new List<ErrorItem> { item });
			logger.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError10"));
		}
		else
		{
			file.WriteFileData(fileData);
			base.HasUnsavedChanges = true;
		}
		return true;
	}

	public bool SaveFileAsTextFile(PvfFile file, string fileText, EncodingType encoding)
	{
		switch (encoding)
		{
		case EncodingType.CN:
			fileText = ChineseHelper.ToSimplified(fileText);
			break;
		case EncodingType.UTF8:
		{
			byte[] bytes = Encoding.UTF8.GetBytes(fileText);
			file.WriteFileData(bytes);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("encoding", encoding, null);
		case EncodingType.JP:
		case EncodingType.KR:
		case EncodingType.TW:
		case EncodingType.Unicode:
			break;
		}
		byte[] bytes2 = Encoding.GetEncoding((int)encoding).GetBytes(fileText);
		file.WriteFileData(bytes2);
		base.HasUnsavedChanges = true;
		if (file.FileName.IndexOf(".str", StringComparison.OrdinalIgnoreCase) > 0)
		{
			base.Strview.ReloadstrFile(file.FileName, fileText, this);
		}
		return true;
	}

	public bool ImportUpdateFile(PvfFile file, Stream stream, string fileName, bool compileScript, bool compileBinaryAni, bool convertChinese, bool compileChinaScriptFile = false, bool compile70PlusAni = false)
	{
		PrepareNkpiFileForImport(file, stream);
		if (UpdateFile(file, stream, compileScript, compileBinaryAni, convertChinese, compileChinaScriptFile, compile70PlusAni))
		{
			return true;
		}
		logger.Warning(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportError"), fileName));
		return false;
	}

	public bool ImportNewFile(string filePath, Stream stream, bool compileScript, bool compileBinaryAni, bool convertChinese)
	{
		PvfFile pvfFile = new PvfFile(filePath);
		PrepareNkpiFileForImport(pvfFile, stream);
		bool flag = ImportUpdateFile(pvfFile, stream, filePath, compileScript, compileBinaryAni, convertChinese);
		if (flag)
		{
			filePath = pvfFile.FileName;
			lock (this)
			{
				base.FileList.TryAdd(filePath, pvfFile);
			}
			base.HasUnsavedChanges = true;
		}
		return flag;
	}

	private void PrepareNkpiFileForImport(PvfFile file, Stream stream)
	{
		if (_nkpi == null || !file.IsNewFile)
			return;

		// ProtectedNKPI 的路径池使用 UTF-8；新增项保留解编译文本，
		// 由保存阶段统一交给 Pvf110Compiled 编译并补齐名称池。
		string fileName = file.FileName;
		file.FileNameEncoding = Encoding.UTF8;
		file.FileName = fileName;
		if (file.Pvf110DataType == 0 && IsPvfScriptStream(stream))
			file.Pvf110DataType = 1;
	}

	public bool UpdateFile(PvfFile file, Stream stream, bool compileScript, bool compileBinaryAni, bool convertChinese, bool compileChinaScriptFile = false, bool compile70PlusAni = false)
	{
		if (stream == null || stream.Length <= 0)
		{
			return true;
		}
		// Pvf110/NKPI：type-3 导入内容转 UTF-16 原样保存（包保存时原样写入）；
		// type-1 掉入经典编译路径（文本 → 经典视图 token），包保存时再适配回 110 token。
		if (Is110Format && file.Pvf110DataType == 3)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			byte[] raw = new byte[stream.Length];
			stream.Read(raw, 0, raw.Length);
			raw = Encoding.Unicode.GetBytes(Encoding.UTF8.GetString(raw));
			file.WriteRawData(raw);
			base.HasUnsavedChanges = true;
			return true;
		}
		byte[] array = new byte[stream.Length];
		stream.Seek(0L, SeekOrigin.Begin);
		stream.Read(array, 0, array.Length);
		string text = Encoding.UTF8.GetString(array).TrimEnd(new char[1]);
		if (convertChinese)
		{
			text = ChineseHelper.ToTraditional(text);
			array = Encoding.UTF8.GetBytes(text);
		}
		if (compileBinaryAni && file.IsBinaryAniFile && text.Length > 10 && text.Substring(0, 9).ToLower() == "#pvf_file")
		{
			var (flag, fileData, item) = BinaryAniCompiler.CompileBinaryAni(text, file.FileName, compile70PlusAni);
			if (!flag)
			{
				logger.Error(new List<ErrorItem> { item });
				return false;
			}
			file.WriteFileData(fileData);
			return true;
		}
		if (compileScript && text.Length > 10 && text.Substring(0, 9).ToLower() == "#pvf_file")
		{
			byte[] array2 = new ScriptFileCompilerOl(this).Compile(file, text, compileChinaScriptFile);
			if (array2 == null)
			{
				return false;
			}
			file.WriteFileData(array2);
			base.HasUnsavedChanges = true;
			return true;
		}
		file.WriteFileData(array);
		base.HasUnsavedChanges = true;
		return true;
	}

	private static bool IsPvfScriptStream(Stream stream)
	{
		if (stream == null || !stream.CanSeek || stream.Length <= 0)
			return false;
		long position = stream.Position;
		try
		{
			stream.Seek(0L, SeekOrigin.Begin);
			int length = (int)Math.Min(stream.Length, 128L);
			byte[] head = new byte[length];
			int read = stream.Read(head, 0, head.Length);
			string text = Encoding.UTF8.GetString(head, 0, read);
			return text.StartsWith("#PVF_File", StringComparison.OrdinalIgnoreCase);
		}
		finally
		{
			stream.Seek(position, SeekOrigin.Begin);
		}
	}

	public IEnumerable<string> MoveFile(string path, string newPath, bool cut)
	{
		lock (base.FileList)
		{
			if (base.FileList.ContainsKey(path))
			{
				newPath = PathsHelper.PathFix(newPath);
				string item;
				if (cut)
				{
					PvfFile file = GetFile(path);
					if (file == null)
					{
						return null;
					}
					string text = newPath + file.ShortName;
					while (GetFile(text) != null)
					{
						text += "(copy)";
					}
					file.Rename(text);
					base.FileList.Remove(path);
					base.FileList.Add(text, file);
					item = text;
				}
				else
				{
					PvfFile sourceFile = GetFile(path);
					if (sourceFile == null)
					{
						return null;
					}
					string destinationPath = newPath + sourceFile.ShortName;
					PvfFile copiedFile = new PvfFile();
					while (GetFile(destinationPath) != null)
					{
						destinationPath += "(copy)";
					}
					copiedFile.InitNewCopyFile(sourceFile.Data, destinationPath);
					base.FileList.Add(destinationPath, copiedFile);
					item = destinationPath;
				}
				base.HasUnsavedChanges = true;
				return new List<string> { item };
			}
			newPath = PathsHelper.PathFix(newPath);
			int count = (path.Contains("/") ? (path.LastIndexOf('/') + 1) : 0);
			path = PathsHelper.PathFix(path);
			List<string> movedFilePaths = new List<string>();
			if (cut)
			{
				PvfFile[] sourceFiles = GetFileObjs(path);
				foreach (PvfFile sourceFile in sourceFiles)
				{
					string destinationPath = newPath + sourceFile.FileName.Remove(0, count);
					while (GetFile(destinationPath) != null)
					{
						destinationPath += "(copy)";
					}
					base.FileList.Remove(sourceFile.FileName);
					sourceFile.Rename(destinationPath);
					movedFilePaths.Add(destinationPath);
					base.FileList.Add(destinationPath, sourceFile);
				}
			}
			else
			{
				PvfFile[] sourceFiles = GetFileObjs(path);
				foreach (PvfFile sourceFile in sourceFiles)
				{
					string destinationPath = newPath + sourceFile.FileName.Remove(0, count);
					while (GetFile(destinationPath) != null)
					{
						destinationPath += "(copy)";
					}
					PvfFile copiedFile = new PvfFile();
					copiedFile.InitNewCopyFile(sourceFile.Data, destinationPath);
					movedFilePaths.Add(copiedFile.FileName);
					base.FileList.Add(destinationPath, copiedFile);
				}
			}
			base.HasUnsavedChanges = true;
			return movedFilePaths;
		}
	}

	public ConcurrentDictionary<string, ConcurrentDictionary<int, string>> FilesToLstDic(IEnumerable<PvfFile> files)
	{
		ConcurrentDictionary<string, ConcurrentDictionary<int, string>> concurrentDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<int, string>>();
		foreach (PvfFile file in files)
		{
			if (!file.IsScriptFile)
			{
				continue;
			}
			KeyValuePair<int, string>? keyValuePair = file.ToLstItem();
			if (keyValuePair.HasValue)
			{
				string key = file.GetLstPathHeader();
				if (base.ListFileTable.LstFilePaths.ContainsKey(key))
				{
					key = base.ListFileTable.LstFilePaths[key];
				}
				if (!concurrentDictionary.ContainsKey(key))
				{
					concurrentDictionary.TryAdd(key, new ConcurrentDictionary<int, string>(new List<KeyValuePair<int, string>> { keyValuePair.Value }));
				}
				else if (!concurrentDictionary[key].ContainsKey(keyValuePair.Value.Key))
				{
					concurrentDictionary[key].TryAdd(keyValuePair.Value.Key, keyValuePair.Value.Value);
				}
			}
		}
		return concurrentDictionary;
	}

	public ConcurrentDictionary<string, ConcurrentDictionary<int, string>> FilesToLstDic(IEnumerable<string> fileList)
	{
		List<PvfFile> list = new List<PvfFile>();
		foreach (string file in fileList)
		{
			if (base.FileList.TryGetValue(file, out PvfFile value))
			{
				list.Add(value);
			}
		}
		return FilesToLstDic(list);
	}

	public string GetItemCodeAndItemNames(IEnumerable<string> fileList, out int outCount)
	{
		int num = 0;
		List<string> list = new List<string>();
		foreach (string file in fileList)
		{
			if (!base.FileList.TryGetValue(file, out PvfFile value))
			{
				continue;
			}
			int? itemCode = value.ItemCode;
			string itemName = GetItemName(value);
			if (itemCode.HasValue || itemName != null)
			{
				num++;
				if (AppSetting.Instance.TreeSetting.TreeListGetItemNameAndItemCodeFormat == TreeListGetItemNameAndItemCodeFormat.名称在前_代码在后)
				{
					list.Add($"{itemName}{AppSetting.Instance.TreeSetting.GetItemNameAndItemCodeSplitChar}{itemCode}");
				}
				else
				{
					list.Add($"{itemCode}{AppSetting.Instance.TreeSetting.GetItemNameAndItemCodeSplitChar}{itemName}");
				}
			}
		}
		if (AppSetting.Instance.TreeSetting.GetItemNameAndItemCodeIsSort)
		{
			list.Sort((string a, string b) => a.CompareTo(b));
		}
		outCount = num;
		return string.Join("\r\n", list);
	}

	public void AddNewEmptyPvfFile()
	{
		base.FileList = new Dictionary<string, PvfFile>();
		base.Guid = Encoding.UTF8.GetBytes("fa08bf71-4395-6a4b-a3e3-261789fee129");
		base.FileVersion = int.Parse("66282");
		base._guidLen = 36;
		base.Strtable.InitDefault();
		base.Strview.InitDefault();
		base.PvfIsOpen = true;
		string text = "test.txt";
		base.FileList.Add(text, new PvfFile(text));
		SaveFileText(text, "//测试文件，请删除\r\n//Test file, please delete\r\n//테스트 파일, 삭제해 주세요\r\n//テストファイル、削除してください");
		string text2 = "stringtable.bin";
		base.FileList.Add(text2, new PvfFile(text2));
	}

	public bool GetJobSkillName(string jobType, int skillId, out string? skillName)
	{
		skillName = null;
		if (jobType == null)
		{
			goto IL_064e;
		}
		switch (jobType.Length)
		{
		case 8:
			break;
		case 9:
			goto IL_0094;
		case 11:
			goto IL_00b1;
		case 12:
			goto IL_00ce;
		case 10:
			goto IL_01a9;
		case 6:
			goto IL_0236;
		case 18:
			goto IL_0314;
		case 14:
			goto IL_033f;
		case 13:
			goto IL_036a;
		case 7:
			goto IL_039b;
		case 16:
			goto IL_03c6;
		default:
			goto IL_064e;
		}
		char c = jobType[1];
		string text;
		if ((uint)c <= 103u)
		{
			if (c != 'c')
			{
				if (c != 'g' || !(jobType == "[gunner]"))
				{
					goto IL_064e;
				}
				text = "skill/gunner";
			}
			else
			{
				if (!(jobType == "[common]"))
				{
					goto IL_064e;
				}
				text = "skill/swordman";
			}
		}
		else if (c != 'k')
		{
			if (c != 'p' || !(jobType == "[priest]"))
			{
				goto IL_064e;
			}
			text = "skill/priest";
		}
		else
		{
			if (!(jobType == "[knight]"))
			{
				goto IL_064e;
			}
			text = "skill/knight";
		}
		goto IL_0650;
		IL_0650:
		if (text == null)
		{
			return false;
		}
		return TryGetSkillName(text, skillId, out skillName);
		IL_0236:
		if (!(jobType == "[mage]"))
		{
			goto IL_064e;
		}
		text = "skill/mage";
		goto IL_0650;
		IL_036a:
		if (!(jobType == "[at swordman]"))
		{
			goto IL_064e;
		}
		text = "skill/atswordman";
		goto IL_0650;
		IL_039b:
		if (!(jobType == "[thief]"))
		{
			goto IL_064e;
		}
		text = "skill/thief";
		goto IL_0650;
		IL_03c6:
		if (!(jobType == "[demonic lancer]"))
		{
			goto IL_064e;
		}
		text = "skill/demoniclancer";
		goto IL_0650;
		IL_0314:
		if (!(jobType == "[demonic swordman]"))
		{
			goto IL_064e;
		}
		text = "skill/demonicswordman";
		goto IL_0650;
		IL_0094:
		c = jobType[1];
		if (c != 'a')
		{
			if (c != 'f' || !(jobType == "[fighter]"))
			{
				goto IL_064e;
			}
			text = "skill/fighter";
		}
		else
		{
			if (!(jobType == "[at mage]"))
			{
				goto IL_064e;
			}
			text = "skill/atmage";
		}
		goto IL_0650;
		IL_033f:
		if (!(jobType == "[creator mage]"))
		{
			goto IL_064e;
		}
		text = "skill/creatormage";
		goto IL_0650;
		IL_00ce:
		c = jobType[1];
		if (c != 'a')
		{
			if (c != 'g' || !(jobType == "[gun blader]"))
			{
				goto IL_064e;
			}
			text = "skill/gunblader";
		}
		else
		{
			if (!(jobType == "[at fighter]"))
			{
				goto IL_064e;
			}
			text = "skill/atfighter";
		}
		goto IL_0650;
		IL_00b1:
		c = jobType[4];
		if (c != 'g')
		{
			if (c != 'p' || !(jobType == "[at priest]"))
			{
				goto IL_064e;
			}
			text = "skill/atpriest";
		}
		else
		{
			if (!(jobType == "[at gunner]"))
			{
				goto IL_064e;
			}
			text = "skill/atgunner";
		}
		goto IL_0650;
		IL_064e:
		text = null;
		goto IL_0650;
		IL_01a9:
		if (!(jobType == "[swordman]"))
		{
			goto IL_064e;
		}
		text = "skill/swordman";
		goto IL_0650;
	}

	private bool TryGetSkillName(string skillDirectory, int skillId, out string skillName)
	{
		string skillFilePath = base.ListFileTable.ItemCodeConvertFilePath(skillDirectory, skillId);
		if (skillFilePath == null)
		{
			skillName = $"在：{skillDirectory}找不到 技能ID：{skillId}";
			return false;
		}
		skillName = GetItemName(skillFilePath);
		if (string.IsNullOrEmpty(skillName))
		{
			skillName = "未知技能";
		}
		return true;
	}

	public async void Tr()
	{
		if (Is110Format)
		{
			// 虚拟串表由名称池生成，无独立 stringtable.bin 可裁剪；裁剪还会破坏池偏移→虚拟 ID 映射
			logger.ShowMsg("该功能不支持 90CN(NKPI/Pvf110) 格式：虚拟串表由名称池生成，无独立 stringtable.bin 可裁剪。");
			return;
		}
		PvfFile[] equipmentFiles = GetFileObjs("equipment");
		HashSet<string> retainedFilePaths = new HashSet<string>
		{
			"equipment/equipment.lst",
			"equipment/equipment.kor.str"
		};
		foreach (PvfFile equipmentFile in equipmentFiles)
		{
			if (equipmentFile.ItemCode.HasValue)
			{
				retainedFilePaths.Add(equipmentFile.FileName);
			}
		}
		PvfFile[] stackableFiles = GetFileObjs("stackable");
		retainedFilePaths.Add("stackable/stackable.kor.str");
		retainedFilePaths.Add("stackable/stackable.lst");
		foreach (PvfFile stackableFile in stackableFiles)
		{
			if (stackableFile.ItemCode.HasValue)
			{
				retainedFilePaths.Add(stackableFile.FileName);
			}
		}
		PvfFile[] skillFiles = GetFileObjs("skill");
		retainedFilePaths.Add("skill/skill.kor.str");
		foreach (PvfFile skillFile in skillFiles)
		{
			if (skillFile.ItemCode.HasValue || skillFile.FileType == PvfFileType.lst)
			{
				retainedFilePaths.Add(skillFile.FileName);
			}
		}
		retainedFilePaths.Add("n_string.lst");
		retainedFilePaths.Add("stringtable.bin");
		retainedFilePaths.Add("etc/equipmentpartset.etc");
		KeyValuePair<string, PvfFile>[] fileEntries = base.FileList.ToArray();
		for (int i = 0; i < fileEntries.Length; i++)
		{
			KeyValuePair<string, PvfFile> fileEntry = fileEntries[i];
			if (!retainedFilePaths.Contains(fileEntry.Key))
			{
				base.FileList.Remove(fileEntry.Key);
			}
		}
		await base.Strtable.DeletingInvalidReferences(this);
		logger.ShowMsg("SUCCESS");
	}
}
