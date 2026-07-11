using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Models.Images;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.ExtractFilesModel;
using SevenZip;
using Utools;

namespace PvfCode.Services;

public class ServiceExtractFiles
{
	private readonly ExtractConfig _config;

	private readonly PvfGroup _pvfGroup;

	private Ilogger _logger;

	private int _extractedFileCount;

	private int _completedFileCount;

	private CancellationTokenSource _progressCancellation;

	private Ilogger Logger => _logger ??= AppSetting.Instance.GetService<Ilogger>();

	public ServiceExtractFiles(PvfGroup pvf, ExtractConfig config)
	{
		_pvfGroup = pvf;
		_config = config;
	}

	public async Task<ResultData> Extract()
	{
		List<PvfFile> files = _pvfGroup.GetFiles(_config.SourceFiles);
		IRes resources = AppSetting.Instance.GetService<IRes>();
		NotificationViewModel notification = null;
		Logger.TaskTokenStart();

		ResultData status;
		if (_config.ExtractTo7zip)
		{
			status = await Task.Run(
				() => ExtractToArchiveAsync(files),
				Logger.TaskCancellationTokenSource.Token);
			if (!status.IsError)
			{
				string message = string.Format(
					AppSetting.Instance.GetIlogger()?.GetStr("mess_Extract7zComplete"),
					files.Count);
				notification = new NotificationViewModel(
					AppSetting.Instance.AppName,
					message,
					resources.VisualStudioBlendLogo2015Pre_16x,
					AppSetting.Instance.GetIlogger()?.GetStr("mess_OpenFolder"),
					OpenTargetPath);
			}
		}
		else
		{
			status = await Task.Run(
				() => ExtractToDirectoryAsync(files),
				Logger.TaskCancellationTokenSource.Token);
			if (!status.IsError)
			{
				notification = new NotificationViewModel(
					AppSetting.Instance.AppName,
					string.Format(
						AppSetting.Instance.GetIlogger()?.GetStr("mess_ExtractLocalComplete"),
						_extractedFileCount),
					resources.VisualStudioBlendLogo2015Pre_16x,
					AppSetting.Instance.GetIlogger()?.GetStr("mess_OpenFolder"),
					OpenTargetPath);
			}
		}

		_config.SourceFiles = null;
		if (!status.IsError)
		{
			if (_config.ExtractSuccessOpenFolder)
			{
				OpenTargetPath();
			}
			if (notification != null)
			{
				await Logger.ShowNotification(notification);
			}
		}

		Logger.TaskTokenStop();
		return status;
	}

	private async Task<ResultData> ExtractToArchiveAsync(List<PvfFile> files)
	{
		ResultData result = new ResultData();
		try
		{
			SevenZipCompressor compressor = new SevenZipCompressor
			{
				ArchiveFormat = OutArchiveFormat.SevenZip,
				CompressionMethod = CompressionMethod.BZip2,
				VolumeSize = 0L,
				CompressionLevel = CompressionLevel.Fast
			};
			SevenZipBase.SetLibraryPath(
				Environment.Is64BitProcess
					? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z64.dll")
					: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));

			ConcurrentDictionary<string, Stream> streams = new ConcurrentDictionary<string, Stream>();
			Parallel.ForEach(files, file =>
			{
				MemoryStream stream = new MemoryStream();
				if (!_pvfGroup.ExtractFile(
					stream,
					file,
					_config.DecompileBinaryAni,
					_config.DecompileScript,
					_config.ConvertConvertSimplifiedChinese,
					isOlWebApi: false,
					_config.UseCompatibleDecompiler))
				{
					Logger.Error(string.Format(
						AppSetting.Instance.GetIlogger()?.GetStr("mess_Extract7zError"),
						file.FileName));
				}
				else
				{
					streams.TryAdd(file.FileName, stream);
				}
			});

			if (_config.ExtractCorrespondenceFileLst)
			{
				List<KeyValuePair<string, Stream>> correspondenceFiles = BuildCorrespondenceFiles(files);
				if (correspondenceFiles.Count > 0)
				{
					streams.AddRange(correspondenceFiles.ToArray());
				}
			}

			float compressionProgress = 0f;
			compressor.Compressing += delegate(object? sender, ProgressEventArgs args)
			{
				compressionProgress += (int)args.PercentDelta;
				Logger.ProgressUpdate(compressionProgress / 100f);
			};
			compressor.CompressionFinished += delegate
			{
				Logger.Success(string.Format(
					AppSetting.Instance.GetIlogger()?.GetStr("mess_Extract7zComplete2"),
					_config.TargetPath));
				Logger.ProgressUpdate(100.0);
				GC.Collect();
			};

			await Task.Run(() => compressor.CompressStreamDictionary(streams, _config.TargetPath));
		}
		catch (Exception ex)
		{
			result.Msg = string.Format(
				AppSetting.Instance.GetIlogger()?.GetStr("mess_Extract7zError2"),
				ex.Message);
			Logger.Error(result.Msg);
		}
		return result;
	}

	private List<KeyValuePair<string, Stream>> BuildCorrespondenceFiles(IEnumerable<PvfFile> files)
	{
		List<KeyValuePair<string, Stream>> result = new List<KeyValuePair<string, Stream>>();
		if (!_config.ExtractCorrespondenceFileLst)
		{
			return result;
		}

		ConcurrentDictionary<string, ConcurrentDictionary<int, string>> mappings = _pvfGroup.FilesToLstDic(files);
		if (mappings.Count == 0)
		{
			return result;
		}

		StringBuilder text = new StringBuilder();
		foreach (KeyValuePair<string, ConcurrentDictionary<int, string>> mapping in mappings)
		{
			text.AppendLine(string.Format(
				AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_PasteTo"),
				mapping.Key));
			foreach (KeyValuePair<int, string> entry in mapping.Value.OrderBy(entry => entry.Key))
			{
				text.AppendLine($"{entry.Key}\t`{entry.Value}`");
			}
			text.AppendLine();
		}

		result.Add(new KeyValuePair<string, Stream>(
			AppSetting.Instance.GetIlogger().GetStr("mess_ExtraFile"),
			BytesHelper.StringToStream(text.ToString())));
		result.Add(new KeyValuePair<string, Stream>(
			AppSetting.Instance.GetIlogger()?.GetStr("mess_ExtraFileJson"),
			BytesHelper.StringToStream(mappings.ToJson())));
		return result;
	}

	private async Task ReportProgressAsync(int totalFileCount)
	{
		while (!_progressCancellation.IsCancellationRequested)
		{
			await Task.Delay(500);
			Logger.ProgressUpdate((float)_completedFileCount / totalFileCount);
		}
	}

	private async Task<ResultData> ExtractToDirectoryAsync(List<PvfFile> files)
	{
		await Task.Delay(0);
		ResultData status = new ResultData();
		DataHelper.GetResolve(files.Count);
		int totalFileCount = files.Count;
		_extractedFileCount = 0;
		IProgress<double> progress = AppSetting.Instance.GetService<IProgress<double>>();
		_progressCancellation = new CancellationTokenSource();
		_ = Task.Run(() => ReportProgressAsync(totalFileCount), _progressCancellation.Token);

		try
		{
			List<KeyValuePair<string, Stream>> correspondenceFiles = BuildCorrespondenceFiles(files);
			if (correspondenceFiles != null && correspondenceFiles.Count > 0)
			{
				foreach (KeyValuePair<string, Stream> item in correspondenceFiles)
				{
					string outputPath = Path.Combine(_config.TargetPath, item.Key);
					EnsureOutputDirectory(outputPath);
					await File.WriteAllBytesAsync(outputPath, BytesHelper.StreamToBytes(item.Value));
					item.Value.Dispose();
				}
			}

			Parallel.ForEach(
				files,
				new ParallelOptions
				{
					MaxDegreeOfParallelism = totalFileCount < 10 ? 1 : 10,
					CancellationToken = Logger.TaskCancellationTokenSource.Token
				},
				file =>
				{
					lock (this)
					{
						ExtractFile(file);
						_extractedFileCount++;
						_completedFileCount++;
					}
				});

			Logger.Success(string.Format(
				AppSetting.Instance.GetIlogger()?.GetStr("mess_ExtractComplete2"),
				_extractedFileCount,
				_config.TargetPath));
		}
		catch (Exception ex)
		{
			progress.Report(100.0);
			status.Msg = ex.Message;
			Logger.Error(string.Format(
				AppSetting.Instance.GetIlogger()?.GetStr("mess_ExtractError"),
				ex.Message));
		}

		_progressCancellation.Cancel();
		_config.SourceFiles = null;
		GC.Collect();
		progress.Report(100.0);
		return status;
	}

	private bool ExtractFile(PvfFile file)
	{
		string outputPath = Path.Combine(_config.TargetPath, file.FileName);
		EnsureOutputDirectory(outputPath);
		if (File.Exists(outputPath))
		{
			switch (_config.Operation)
			{
			case FileOperation.Rename:
				int suffix = 0;
				string renamedPath = $"{outputPath}({suffix})";
				while (File.Exists(renamedPath))
				{
					suffix++;
					renamedPath = $"{outputPath}({suffix})";
				}
				outputPath = renamedPath;
				break;
			case FileOperation.Skip:
				return false;
			case FileOperation.Cancel:
				throw new Exception(string.Format(
					AppSetting.Instance.GetIlogger()?.GetStr("mess_FileExists"),
					outputPath));
			}
		}

		try
		{
			using FileStream stream = File.Create(outputPath);
			_pvfGroup.ExtractFile(
				stream,
				file,
				_config.DecompileBinaryAni,
				_config.DecompileScript,
				_config.ConvertConvertSimplifiedChinese,
				isOlWebApi: false,
				_config.UseCompatibleDecompiler);
		}
		catch (Exception ex)
		{
			Logger.Error(string.Format(
				AppSetting.Instance.GetIlogger()?.GetStr("mess_ExportError"),
				file.FileName,
				ex.Message));
		}
		return true;
	}

	private static void EnsureOutputDirectory(string outputPath)
	{
		string directory = Path.GetDirectoryName(outputPath);
		if (directory != null && !Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
	}

	private void OpenTargetPath()
	{
		try
		{
			FileHelper.OpenFolderAndSelectFile(_config.TargetPath);
		}
		catch (Exception ex)
		{
			Logger.ShowMsg(
				string.Format(
					AppSetting.Instance.GetIlogger()?.GetStr("mess_OpenFolderError"),
					ex.Message),
				isError: true,
				null,
				loggerError: true);
		}
	}
}
