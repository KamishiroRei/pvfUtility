using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Collections.Pooled;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Models.Pvf.ImportModels;
using SevenZip;
using Utools;

namespace PvfCode.Services;

public class ServiceImportFiles
{
	private readonly PvfGroup pvf;

	private readonly ImportConfig config;

	private Ilogger logger;

	private ConcurrentHashSet<string> importedFilePaths;

	private Ilogger? Logger
	{
		get
		{
			if (logger == null)
			{
				logger = AppSetting.Instance.GetService<Ilogger>();
			}
			return logger;
		}
	}

	public ServiceImportFiles(PvfGroup pvf, ImportConfig config)
	{
		importedFilePaths = new ConcurrentHashSet<string>();
		this.pvf = pvf;
		this.config = config;
	}

	public async Task<ResultData> Import(bool is7z = false, string filePath7z = null)
	{
		ResultData<int> status = await Task.Run(() => ImportCore(is7z, filePath7z));
		Logger.ProgressUpdate(100.0);
		if (config.SourceFiles.Count > 0)
		{
			IEnumerable<string> fileList = config.SourceFiles.Select((ImportFileItem it) => it.TreeFullPath);
			await Logger.TreeListAddFiles(new PooledList<string>(fileList));
			if (config.ImportSuccessFilePathListAddToSearchPanel)
			{
				Logger.AddFileListToSearchPanel(new PooledList<string>(fileList));
			}
			Logger.GoToTreeListNode(importedFilePaths.FirstOrDefault());
		}
		config.SourceFiles = null;
		if (status.IsError)
		{
			status.Msg += string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportComplete"), status.Data);
			Logger.Error(status.Msg);
			await Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, status.Msg, AppSetting.Instance.GetRes()?.ErrorIcon));
		}
		else
		{
			string arg = (string.IsNullOrEmpty(config.TargetPath) ? AppSetting.Instance.GetIlogger().GetStr("mess_RootFolder") : config.TargetPath);
			string text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportComplete2"), status.Data, arg);
			Logger.Success(text);
			await Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, text, AppSetting.Instance.GetRes()?.VisualStudioBlendLogo2015Pre_16x));
		}
		return status;
	}

	private async Task<ResultData<int>> ImportCore(bool isArchive, string archivePath = null)
	{
		Logger?.TaskTokenStart();
		if (config.FileTypes != null && config.FileTypes.Count > 0)
		{
			List<ImportFileItem> list = new List<ImportFileItem>();
			if (config.RemoveOrKeepFileType == RemoveOrKeepFileType.保留)
			{
				foreach (ImportFileItem sourceFile in config.SourceFiles)
				{
					if (config.FileTypes.Contains(sourceFile.Extension))
					{
						list.Add(sourceFile);
					}
				}
			}
			else
			{
				foreach (ImportFileItem sourceFile2 in config.SourceFiles)
				{
					if (!config.FileTypes.Contains(sourceFile2.Extension))
					{
						list.Add(sourceFile2);
					}
				}
			}
			config.SourceFiles = list.ToHashSet();
		}
		ResultData<int> result = isArchive
			? await Task.Run(() => ImportFromArchive(archivePath), Logger.TaskCancellationTokenSource.Token)
			: await Task.Run((Func<ResultData<int>>)ImportFromFiles, Logger.TaskCancellationTokenSource.Token);
		Logger?.TaskTokenStop();
		return result;
	}

	private async Task<ResultData<int>> ImportFromArchive(string archivePath)
	{
		await Task.Delay(1);
		Logger.Warning(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportFrom7z"));
		int num = 0;
		ResultData<int> resultData = new ResultData<int>();
		string arg = "";
		try
		{
			SevenZipBase.SetLibraryPath(Environment.Is64BitProcess ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z64.dll") : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));
			int resolve = DataHelper.GetResolve(config.SourceFiles.Count);
			int count = config.SourceFiles.Count;
			int num2 = 0;
			using SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(archivePath);
			foreach (ImportFileItem sourceFile in config.SourceFiles)
			{
				if (ImportArchiveItem(sourceFile, sevenZipExtractor))
				{
					num++;
				}
				if (num2 % resolve == 0)
				{
					Logger.ProgressUpdate((float)num2 / (float)count);
				}
				num2++;
			}
		}
		catch (Exception ex)
		{
			Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_UnzipError"), arg));
			resultData.Msg = ex.Message;
		}
		resultData.Data = num;
		return resultData;
	}

	private bool ImportArchiveItem(ImportFileItem item, SevenZipExtractor extractor)
	{
		string treeFullPath = item.TreeFullPath;
		PvfFile file = pvf.GetFile(treeFullPath);
		using MemoryStream memoryStream = new MemoryStream();
		if (file != null)
		{
			switch (config.Operation)
			{
			case FileOperation.Rename:
			{
				extractor.ExtractFile(item.IndexForm7zip.Value, memoryStream);
				int num = 0;
				string text2 = $"{treeFullPath}({num})";
				while (pvf.FileAny(text2))
				{
					num++;
					text2 = $"{treeFullPath}({num})";
				}
				treeFullPath = text2;
				return AddNewFile(treeFullPath, memoryStream);
			}
			case FileOperation.Skip:
				return false;
			case FileOperation.Cancel:
				throw new Exception(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileExists2"), treeFullPath));
			default:
				extractor.ExtractFile(item.IndexForm7zip.Value, memoryStream);
				return pvf.ImportUpdateFile(file, memoryStream, treeFullPath, config.CompileScript, config.CompileBinaryAni, config.ConvertToTraditionalChinese, config.CompileChinaPvfScriptFile, config.CompileChinaAni);
			}
		}
		extractor.ExtractFile(item.IndexForm7zip.Value, memoryStream);
		return AddNewFile(treeFullPath, memoryStream);
	}

	private ResultData<int> ImportFromFiles()
	{
		ResultData<int> resultData = new ResultData<int>();
		int num = 0;
		try
		{
			int num2 = 0;
			int count = config.SourceFiles.Count;
			int resolve = DataHelper.GetResolve(count);
			ImportFileItem[] array = config.SourceFiles.ToArray();
			foreach (ImportFileItem importFileItem in array)
			{
				if (ImportFile(importFileItem.FullPath, importFileItem.TreeFullPath))
				{
					num++;
				}
				if (num2 % resolve == 0)
				{
					Logger.ProgressUpdate((float)num2 / (float)count);
				}
				num2++;
			}
		}
		catch (Exception ex)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportError2"), ex.Message);
		}
		resultData.Data = num;
		return resultData;
	}

	private bool ImportFile(string sourcePath, string targetPath)
	{
		PvfFile file = pvf.GetFile(targetPath);
		using FileStream fileStream = File.OpenRead(sourcePath);
		if (file != null)
		{
			switch (config.Operation)
			{
			case FileOperation.Rename:
			{
				int num = 0;
				string text2 = $"{targetPath}({num})";
				while (pvf.FileAny(text2))
				{
					num++;
					text2 = $"{targetPath}({num})";
				}
				targetPath = text2;
				return AddNewFile(targetPath, fileStream);
			}
			case FileOperation.Skip:
				return false;
			case FileOperation.Cancel:
				throw new Exception(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileExists2"), targetPath));
			default:
				return pvf.ImportUpdateFile(file, fileStream, targetPath, config.CompileScript, config.CompileBinaryAni, config.ConvertToTraditionalChinese, config.CompileChinaPvfScriptFile, config.CompileChinaAni);
			}
		}
		return AddNewFile(targetPath, fileStream);
	}

	private bool AddNewFile(string targetPath, Stream data)
	{
		PvfFile pvfFile = new PvfFile(targetPath);
		bool flag = pvf.ImportUpdateFile(pvfFile, data, targetPath, config.CompileScript, config.CompileBinaryAni, config.ConvertToTraditionalChinese, config.CompileChinaPvfScriptFile, config.CompileChinaAni);
		if (flag)
		{
			targetPath = pvfFile.FileName;
			lock (this)
			{
				pvf.FileList.TryAdd(targetPath, pvfFile);
				importedFilePaths.Add(targetPath);
			}
		}
		return flag;
	}
}
