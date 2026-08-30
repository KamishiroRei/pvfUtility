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
		_nkpi = null;
		_entryIndex.Clear();
		base.ListFileTable.Clear();
		base.Strtable.Clear();
		base.Strview.Clear();
		base.PvfPackFilePath = null;
		base.PvfIsOpen = false;
		base.EquipmentPartSetTable.Clear();
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
				// Pvf110：保留 name/hash 表，重建 body/group/file table
				if (IsPvf110)
				{
					return await SavePvfPack110Core(filePath, progress);
				}
				// NKPI / ProtectedNKPI：使用 NkpiRepacker 重建
				if (_nkpi != null)
				{
					SavePvfPackNkpiCore(filePath, progress);
					progress?.Report(100.0);
					return result;
				}
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
		catch (Exception ex)
		{
			result.Msg = ex.Message;
		}
		return result;
	}

	/// <summary>Pvf110 保存：FileList（type-1 解编译文本/type-3 UTF-16）→ 重编译 → 重建逻辑流 → 加密 Script.pvf + sk.dat。</summary>
	private async Task<ResultData> SavePvfPack110Core(string filePath, IProgress<double> progress)
	{
		ResultData result = new ResultData();
		try
		{
			Pvf110Reader reader = _pvf110!;
			Pvf110Compiled compiled = new Pvf110Compiled(reader);

			byte[] GetContent(Pvf110Entry e)
			{
				string p = reader.FilePath(e);
				if (!base.FileList.TryGetValue(p, out PvfFile? file) || file.Data == null)
					throw new InvalidOperationException("Pvf110 保存要求文件集合与原包一致，缺少文件: " + p);
				// 懒加载：保存前确保未访问过的文件内容已加载
				if (file.Data.Length == 0)
				{
					EnsureFileData(p);
				}
				if (e.DataType == 1)
				{
					try
					{
						return compiled.FromText(System.Text.Encoding.UTF8.GetString(file.Data));
					}
					catch (Exception ex)
					{
						throw new InvalidOperationException($"Pvf110 重编译失败 {p}: {ex.Message}", ex);
					}
				}
				return file.Data; // type-3 = UTF-16LE
			}

			var (enc, sk) = Pvf110Rebuilder.Rebuild(reader, GetContent, progress);
			string dir = Path.GetDirectoryName(filePath) ?? ".";
			File.WriteAllBytes(filePath, enc);
			File.WriteAllBytes(Path.Combine(dir, "sk.dat"), sk);
			progress.Report(100.0);
			return result;
		}
		catch (Exception ex)
		{
			result.Msg = ex.Message;
			return result;
		}
	}

	/// <summary>NKPI / ProtectedNKPI 保存：FileList（type-1 解编译文本）→ 重编译 → NkpiRepacker 重建。</summary>
	private void SavePvfPackNkpiCore(string filePath, IProgress<double> progress)
	{
		try
		{
			NkpiReader reader = _nkpi!;
			Pvf110Compiled compiled = new Pvf110Compiled(reader);

			byte[] GetContent(int entryIndex)
			{
				NkpiEntry e = reader.Entries[entryIndex];
				string p = reader.FilePath(e);
				if (!base.FileList.TryGetValue(p, out PvfFile? file) || file.Data == null)
					throw new InvalidOperationException("NKPI 保存要求文件集合与原包一致，缺少文件: " + p);
				// 懒加载：保存前确保未访问过的文件内容已加载
				if (file.Data.Length == 0)
				{
					EnsureFileData(p);
				}
				if (e.DataType == 1)
				{
					try
					{
						return compiled.FromText(System.Text.Encoding.UTF8.GetString(file.Data));
					}
					catch (Exception ex)
					{
						throw new InvalidOperationException($"NKPI 重编译失败 {p}: {ex.Message}", ex);
					}
				}
				return file.Data; // type-3 = UTF-16LE
			}

			byte[] rebuilt = NkpiRepacker.Rebuild(reader, GetContent, progress);
			File.WriteAllBytes(filePath, rebuilt);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"NKPI 保存失败: {ex.Message}", ex);
		}
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
			try
			{
				base.PvfPackFilePath = path;
				byte[] fileBytes = File.ReadAllBytes(path);

				// 1. Pvf110 Builder 保护链检测：同目录存在 sk.dat 且可用 Pvf110Reader 打开
				string? skdatPath = Pvf110Support.FindSkDat(path);
				if (skdatPath != null)
				{
					try
					{
						var probe = Pvf110Reader.Open(File.ReadAllBytes(skdatPath), fileBytes);
						return OpenPvfPack110(probe, path, progress);
					}
					catch (Exception ex)
					{
						logger.Warning($"Pvf110 probe failed, fallback to NKPI/ProtectedNKPI: {ex.Message}");
					}
				}

				// 2. NKPI / ProtectedNKPI 检测
				if (NkpiReader.IsNkpi(fileBytes))
				{
					try
					{
						var reader = NkpiReader.Open(fileBytes);
						return OpenPvfPackNkpi(reader, path, progress);
					}
					catch (Exception ex)
					{
						logger.Warning($"NKPI open failed, fallback to classic: {ex.Message}");
					}
				}

				// 3. 经典 pvfUtility 格式回退
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
							progress.Report(ProgressHelper.GetProgressNum(i, fileCount));
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
					progress.Report(100.0);
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
				};
				file.FileName = p;
				file.WriteRawData(Array.Empty<byte>());
				list[file.FileName] = file;
				_entryIndex[file.FileName] = i;
				if ((i & 1023) == 0) progress.Report(ProgressHelper.GetProgressNum(i, count));
			}

			base.FileList = list;
			base.Strtable.InitDefault();
			base.Strview.InitDefault();
			// Pvf110 也使用 UTF-8 编码
			AppSetting.Instance.PvfConfig.DefaultEncoding = EncodingType.UTF8;
			base.PvfIsOpen = true;
			progress.Report(100.0);
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
				};
				file.FileName = p;
				// Data 留空，后续按需加载
				file.WriteRawData(Array.Empty<byte>());
				list[file.FileName] = file;
				_entryIndex[file.FileName] = i;
				if ((i & 1023) == 0) progress.Report(ProgressHelper.GetProgressNum(i, count));
			}

			base.FileList = list;
			base.Strtable.InitDefault();
			base.Strview.InitDefault();
			// NKPI/ProtectedNKPI 使用 UTF-8，非 TW/Big5；直接设 DefaultEncoding 避免 DoNotify 跨线程问题
			AppSetting.Instance.PvfConfig.DefaultEncoding = EncodingType.UTF8;
			base.PvfIsOpen = true;
			progress.Report(100.0);
			return Task.FromResult(true);
		}
		catch (Exception ex)
		{
			logger.Error("NKPI open error: " + ex.Message);
			return Task.FromResult(false);
		}
	}

	/// <summary>
	/// 懒加载：按路径从 Pvf110/NKPI 读取器取回文件内容并写入 PvfFile.Data。
	/// 打开时 Data 为空，只有用户实际访问该文件时才解压+解编译。
	/// 已在读取器里缓存 group，多次访问同一文件不会重复解压。
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
					? System.Text.Encoding.UTF8.GetBytes(new Pvf110Compiled(_nkpi).ToText(content))
					: content; // type-3 = UTF-16LE
				file.WriteRawData(data);
				return true;
			}
			if (_pvf110 != null && idx < _pvf110.Entries.Count)
			{
				Pvf110Entry e = _pvf110.Entries[idx];
				byte[] content = _pvf110.ReadEntry(e);
				byte[] data = (e.DataType == 1)
					? System.Text.Encoding.UTF8.GetBytes(new Pvf110Compiled(_pvf110).ToText(content))
					: content; // type-3 = UTF-16LE
				file.WriteRawData(data);
				return true;
			}
		}
		catch (Exception ex)
		{
			logger.Error($"EnsureFileData failed for {path}: {ex.Message}");
		}
		return false;
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
			for (int childIndex = 3; childIndex < childCount; childIndex++)
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
						ScriptItem linkedItem = child.Item.Type == ScriptType.StringLinkIndex ? partSetSection.Children[childIndex + 1].Item : null;
						errors.AppendLine($"套装文件错误 套装信息从第二行开始 第一个参数为 String型 当前类型：{children[childIndex].Item.Type} 值：{children[childIndex].Item.GetItemText(this, linkedItem)}");
						break;
					}
					ScriptItem linkedNameItem = child.Item.Type == ScriptType.StringLinkIndex ? partSetSection.Children[childIndex + 1].Item : null;
					partSet = new EquipmentPartSet
					{
						Name = children[childIndex].Item.GetItemTextNotChar(this, linkedNameItem),
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
						ScriptItem linkedItem = child.Item.Type == ScriptType.StringLinkIndex ? partSetSection.Children[childIndex + 1].Item : null;
						errors.AppendLine($"套装文件错误 套装信息从第二行开始 第二个参数为 String型 当前类型：{children[childIndex].Item.Type} 值：{children[childIndex].Item.GetItemText(this, linkedItem)}");
						break;
					}
					partSet.EquType = children[childIndex].Item.GetItemTextNotChar(this);
					if (string.IsNullOrEmpty(partSet.EquType))
					{
						errors.AppendLine($"套装文件错误 套装文件的装备类型不能为空！ 套装索引：{setIndex}");
						break;
					}
					if (!partsByEquipmentType.ContainsKey(partSet.EquType))
					{
						continue;
					}
					errors.AppendLine($"套装文件错误 套装文件的套装类型不能重复！ 套装索引：{setIndex} 重复索引：{partSet.EquType}");
					break;
				}
				case 3:
				{
					if (children[childIndex].Item.Type == ScriptType.Int)
					{
						continue;
					}
					ScriptItem linkedItem = children[childIndex].Item.Type == ScriptType.StringLinkIndex ? partSetSection.Children[childIndex + 1].Item : null;
					errors.AppendLine($"套装文件错误 套装信息从第二行开始 第3个参数为 int型 当前类型：{children[childIndex].Item.Type} 值：{children[childIndex].Item.GetItemText(this, linkedItem)}");
					break;
				}
				case 4:
					if (children[childIndex].Item.Type != ScriptType.Int)
					{
						ScriptItem linkedItem = children[childIndex].Item.Type == ScriptType.StringLinkIndex ? partSetSection.Children[childIndex + 1].Item : null;
						errors.AppendLine($"套装文件错误 套装信息从第二行开始 第4个参数为 int型 当前类型：{children[childIndex].Item.Type} 值：{children[childIndex].Item.GetItemText(this, linkedItem)}");
						break;
					}
					fieldIndex = 0;
					partsByEquipmentType.Add(partSet.EquType, partSet);
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
		if (!file.IsScriptFile)
		{
			return null;
		}
		PvfFileType fileType = file.FileType;
		string itemName;
		switch (fileType)
		{
		case PvfFileType.shp:
			itemName = GetShopName(file);
			break;
		case PvfFileType.equ:
			itemName = GetNameByLabels(base.Strtable.NameLableOrSetNameLable, file);
			break;
		default:
		{
			int nameLable = base.Strtable.GetNameLable(fileType);
			itemName = GetNameByLabel(nameLable, file);
			break;
		}
		}
		if (!string.IsNullOrEmpty(itemName))
		{
			return AppSetting.Instance.PvfConfig.FileTextTraditionalConvertSimplifiedAutoMethods(itemName);
		}
		return null;
	}

	private string GetNameByLabel(int nameLabel, PvfFile file)
	{
		for (int offset = 2; offset < file.DataLen - 9; offset += 5)
		{
			if (file.Data[offset] == 5 && BitConverter.ToInt32(file.Data, offset + 1) == nameLabel)
			{
				if (offset < file.DataLen - 14 && file.Data[offset + 5] == 9 && file.Data[offset + 10] == 10)
				{
					return base.Strview.GetStrText(file.Data[offset + 6], base.Strtable.GetStringItem(BitConverter.ToInt32(file.Data, offset + 11)));
				}
				if (file.Data[offset + 5] == 7)
				{
					return base.Strtable.GetStringItem(BitConverter.ToInt32(file.Data, offset + 6));
				}
			}
		}
		return null;
	}

	private string GetNameByLabels(HashSet<int> nameLabels, PvfFile file)
	{
		for (int offset = 2; offset < file.DataLen - 9; offset += 5)
		{
			if (file.Data[offset] == 5 && nameLabels.Contains(BitConverter.ToInt32(file.Data, offset + 1)))
			{
				if (offset < file.DataLen - 14 && file.Data[offset + 5] == 9 && file.Data[offset + 10] == 10)
				{
					return base.Strview.GetStrText(file.Data[offset + 6], base.Strtable.GetStringItem(BitConverter.ToInt32(file.Data, offset + 11)));
				}
				if (file.Data[offset + 5] == 7)
				{
					return base.Strtable.GetStringItem(BitConverter.ToInt32(file.Data, offset + 6));
				}
			}
		}
		return null;
	}

	private string GetShopName(PvfFile? file)
	{
		if (file.GetNpcId(this, out var npcId) && base.ListFileTable.CodeDic.TryGetValue("npc", out Dictionary<int, LstItem> npcFiles) && npcFiles.TryGetValue(npcId, out var npcFile))
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
	}

	public bool ExtractFile(Stream stream, PvfFile file, bool decompileBinaryAni, bool decompileScript, bool convertConvertSimplifiedChinese, bool isOlWebApi = false, bool? useCompatibleDecompiler = null)
	{
		if (file == null)
		{
			return false;
		}
		if (file.DataLen <= 0)
		{
			return true;
		}
		// Pvf110：Data 已是可读内容（type-1 解编译文本 UTF-8 / type-3 UTF-16），直接写出
		if (IsPvf110 && file.Pvf110DataType != 0)
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
		// Pvf110：Data 统一保存解编译文本，包保存时（SavePvfPack110Core）再重编译
		if (IsPvf110 && file.Pvf110DataType == 1)
		{
			file.WriteRawData(System.Text.Encoding.UTF8.GetBytes(fileText));
			return true;
		}
		byte[] array = new ScriptFileCompilerOl(this).Compile(file, fileText);
		if (array != null)
		{
			file.WriteFileData(array);
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
		if (file.FileName.IndexOf(".str", StringComparison.OrdinalIgnoreCase) > 0)
		{
			base.Strview.ReloadstrFile(file.FileName, fileText, this);
		}
		return true;
	}

	public bool ImportUpdateFile(PvfFile file, Stream stream, string fileName, bool compileScript, bool compileBinaryAni, bool convertChinese, bool compileChinaScriptFile = false, bool compile70PlusAni = false)
	{
		_ = stream?.Length;
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
		bool flag = ImportUpdateFile(pvfFile, stream, filePath, compileScript, compileBinaryAni, convertChinese);
		if (flag)
		{
			filePath = pvfFile.FileName;
			lock (this)
			{
				base.FileList.TryAdd(filePath, pvfFile);
			}
		}
		return flag;
	}

	public bool UpdateFile(PvfFile file, Stream stream, bool compileScript, bool compileBinaryAni, bool convertChinese, bool compileChinaScriptFile = false, bool compile70PlusAni = false)
	{
		if (stream == null || stream.Length <= 0)
		{
			return true;
		}
		// Pvf110：导入内容为可读文本（type-1 解编译文本存 UTF-8；type-3 转 UTF-16），包保存时再重编译
		if (IsPvf110 && file.Pvf110DataType != 0)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			byte[] raw = new byte[stream.Length];
			stream.Read(raw, 0, raw.Length);
			if (file.Pvf110DataType == 3)
				raw = Encoding.Unicode.GetBytes(Encoding.UTF8.GetString(raw));
			file.WriteRawData(raw);
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
			return true;
		}
		file.WriteFileData(array);
		return true;
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
