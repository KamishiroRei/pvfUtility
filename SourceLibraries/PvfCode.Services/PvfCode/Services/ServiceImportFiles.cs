using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public ServiceImportFiles fGD3pYlSZ4;

		public bool TqZ3LZripD;

		public string OLE3EmWxw2;

		public _003C_003Ec__DisplayClass7_0()
		{
		}

		internal Task<ResultData<int>>? mDd3GpD8u7()
		{
			return fGD3pYlSZ4.BWGeF7kTLw(TqZ3LZripD, OLE3EmWxw2);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_0
	{
		public ServiceImportFiles Gjk364h2II;

		public string R0n32THe4A;

		public _003C_003Ec__DisplayClass8_0()
		{
		}

		internal Task<ResultData<int>>? Vf53wbopyV()
		{
			return Gjk364h2II.rlneDBSRRF(R0n32THe4A);
		}
	}

	private readonly PvfGroup RsYeSQkwUk;

	private readonly ImportConfig W7nebvOpd8;

	private Ilogger rnYeVfbUiD;

	private ConcurrentHashSet<string> WZPeks0hR8;

	private Ilogger? ri7eRklf95
	{
		get
		{
			if (rnYeVfbUiD == null)
			{
				rnYeVfbUiD = AppSetting.Instance.GetService<Ilogger>();
			}
			return rnYeVfbUiD;
		}
	}

	public ServiceImportFiles(PvfGroup pvf, ImportConfig config)
	{
		WZPeks0hR8 = new ConcurrentHashSet<string>();
		RsYeSQkwUk = pvf;
		W7nebvOpd8 = config;
	}

	public async Task<ResultData> Import(bool is7z = false, string filePath7z = null)
	{
		_003C_003Ec__DisplayClass7_0 obj = new _003C_003Ec__DisplayClass7_0();
		obj.fGD3pYlSZ4 = this;
		obj.TqZ3LZripD = is7z;
		obj.OLE3EmWxw2 = filePath7z;
		ResultData<int> status = await Task.Run(() => obj.fGD3pYlSZ4.BWGeF7kTLw(obj.TqZ3LZripD, obj.OLE3EmWxw2));
		ri7eRklf95.ProgressUpdate(100.0);
		if (W7nebvOpd8.SourceFiles.Count > 0)
		{
			IEnumerable<string> fileList = W7nebvOpd8.SourceFiles.Select((ImportFileItem it) => it.TreeFullPath);
			await ri7eRklf95.TreeListAddFiles(new PooledList<string>(fileList));
			if (W7nebvOpd8.ImportSuccessFilePathListAddToSearchPanel)
			{
				ri7eRklf95.AddFileListToSearchPanel(new PooledList<string>(fileList));
			}
			ri7eRklf95.GoToTreeListNode(WZPeks0hR8.FirstOrDefault());
		}
		W7nebvOpd8.SourceFiles = null;
		if (status.IsError)
		{
			status.Msg += string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportComplete"), status.Data);
			ri7eRklf95.Error(status.Msg);
			await ri7eRklf95.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, status.Msg, AppSetting.Instance.GetRes()?.ErrorIcon));
		}
		else
		{
			string arg = (string.IsNullOrEmpty(W7nebvOpd8.TargetPath) ? AppSetting.Instance.GetIlogger().GetStr("mess_RootFolder") : W7nebvOpd8.TargetPath);
			string text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportComplete2"), status.Data, arg);
			ri7eRklf95.Success(text);
			await ri7eRklf95.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, text, AppSetting.Instance.GetRes()?.VisualStudioBlendLogo2015Pre_16x));
		}
		return status;
	}

	private async Task<ResultData<int>> BWGeF7kTLw(bool P_0, string P_1 = null)
	{
		_003C_003Ec__DisplayClass8_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass8_0();
		CS_0024_003C_003E8__locals4.Gjk364h2II = this;
		CS_0024_003C_003E8__locals4.R0n32THe4A = P_1;
		ri7eRklf95?.TaskTokenStart();
		if (W7nebvOpd8.FileTypes != null && W7nebvOpd8.FileTypes.Count > 0)
		{
			List<ImportFileItem> list = new List<ImportFileItem>();
			if (W7nebvOpd8.RemoveOrKeepFileType == RemoveOrKeepFileType.保留)
			{
				foreach (ImportFileItem sourceFile in W7nebvOpd8.SourceFiles)
				{
					if (W7nebvOpd8.FileTypes.Contains(sourceFile.Extension))
					{
						list.Add(sourceFile);
					}
				}
			}
			else
			{
				foreach (ImportFileItem sourceFile2 in W7nebvOpd8.SourceFiles)
				{
					if (!W7nebvOpd8.FileTypes.Contains(sourceFile2.Extension))
					{
						list.Add(sourceFile2);
					}
				}
			}
			W7nebvOpd8.SourceFiles = list.ToHashSet();
		}
		ResultData<int> result = ((!P_0) ? (await Task.Run((Func<ResultData<int>>)ALueQBjlw6, ri7eRklf95.TaskCancellationTokenSource.Token)) : (await Task.Run(() => CS_0024_003C_003E8__locals4.Gjk364h2II.rlneDBSRRF(CS_0024_003C_003E8__locals4.R0n32THe4A), ri7eRklf95.TaskCancellationTokenSource.Token)));
		ri7eRklf95?.TaskTokenStop();
		return result;
	}

	private async Task<ResultData<int>> rlneDBSRRF(string P_0)
	{
		await Task.Delay(1);
		ri7eRklf95.Warning(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportFrom7z"));
		int num = 0;
		ResultData<int> resultData = new ResultData<int>();
		string arg = "";
		try
		{
			SevenZipBase.SetLibraryPath(Environment.Is64BitProcess ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z64.dll") : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));
			int resolve = DataHelper.GetResolve(W7nebvOpd8.SourceFiles.Count);
			int count = W7nebvOpd8.SourceFiles.Count;
			int num2 = 0;
			using SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(P_0);
			foreach (ImportFileItem sourceFile in W7nebvOpd8.SourceFiles)
			{
				if (c6remtt4r7(sourceFile, sevenZipExtractor))
				{
					num++;
				}
				if (num2 % resolve == 0)
				{
					ri7eRklf95.ProgressUpdate((float)num2 / (float)count);
				}
				num2++;
			}
		}
		catch (Exception ex)
		{
			ri7eRklf95.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_UnzipError"), arg));
			resultData.Msg = ex.Message;
		}
		resultData.Data = num;
		return resultData;
	}

	private bool c6remtt4r7(ImportFileItem P_0, SevenZipExtractor P_1)
	{
		string treeFullPath = P_0.TreeFullPath;
		PvfFile file = RsYeSQkwUk.GetFile(treeFullPath);
		using MemoryStream memoryStream = new MemoryStream();
		if (file != null)
		{
			switch (W7nebvOpd8.Operation)
			{
			case FileOperation.Rename:
			{
				P_1.ExtractFile(P_0.IndexForm7zip.Value, memoryStream);
				int num = 0;
				string text = treeFullPath;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
				defaultInterpolatedStringHandler.AppendLiteral("(");
				defaultInterpolatedStringHandler.AppendFormatted(num);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				string text2 = text + defaultInterpolatedStringHandler.ToStringAndClear();
				while (RsYeSQkwUk.FileAny(text2))
				{
					num++;
					string text3 = treeFullPath;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(2, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("(");
					defaultInterpolatedStringHandler2.AppendFormatted(num);
					defaultInterpolatedStringHandler2.AppendLiteral(")");
					text2 = text3 + defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				treeFullPath = text2;
				return DknerAuSDM(treeFullPath, memoryStream);
			}
			case FileOperation.Skip:
				return false;
			case FileOperation.Cancel:
				throw new Exception(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileExists2"), treeFullPath));
			default:
				P_1.ExtractFile(P_0.IndexForm7zip.Value, memoryStream);
				return RsYeSQkwUk.ImportUpdateFile(file, memoryStream, treeFullPath, W7nebvOpd8.CompileScript, W7nebvOpd8.CompileBinaryAni, W7nebvOpd8.ConvertToTraditionalChinese, W7nebvOpd8.CompileChinaPvfScriptFile, W7nebvOpd8.CompileChinaAni);
			}
		}
		P_1.ExtractFile(P_0.IndexForm7zip.Value, memoryStream);
		return DknerAuSDM(treeFullPath, memoryStream);
	}

	private Task<bool> wg5e4lLCP9(ImportFileItem P_0, SevenZipExtractor P_1)
	{
		string treeFullPath = P_0.TreeFullPath;
		PvfFile file = RsYeSQkwUk.GetFile(treeFullPath);
		using MemoryStream memoryStream = new MemoryStream();
		if (file != null)
		{
			switch (W7nebvOpd8.Operation)
			{
			case FileOperation.Rename:
			{
				P_1.ExtractFile(P_0.IndexForm7zip.Value, memoryStream);
				int num = 0;
				string text = treeFullPath;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
				defaultInterpolatedStringHandler.AppendLiteral("(");
				defaultInterpolatedStringHandler.AppendFormatted(num);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				string text2 = text + defaultInterpolatedStringHandler.ToStringAndClear();
				while (RsYeSQkwUk.FileAny(text2))
				{
					num++;
					string text3 = treeFullPath;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(2, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("(");
					defaultInterpolatedStringHandler2.AppendFormatted(num);
					defaultInterpolatedStringHandler2.AppendLiteral(")");
					text2 = text3 + defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				treeFullPath = text2;
				return Task.FromResult(DknerAuSDM(treeFullPath, memoryStream));
			}
			case FileOperation.Skip:
				return Task.FromResult(result: false);
			case FileOperation.Cancel:
				throw new Exception(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileExists2"), treeFullPath));
			default:
				P_1.ExtractFile(P_0.IndexForm7zip.Value, memoryStream);
				return Task.FromResult(RsYeSQkwUk.ImportUpdateFile(file, memoryStream, treeFullPath, W7nebvOpd8.CompileScript, W7nebvOpd8.CompileBinaryAni, W7nebvOpd8.ConvertToTraditionalChinese));
			}
		}
		P_1.ExtractFile(P_0.IndexForm7zip.Value, memoryStream);
		return Task.FromResult(DknerAuSDM(treeFullPath, memoryStream));
	}

	private ResultData<int> ALueQBjlw6()
	{
		ResultData<int> resultData = new ResultData<int>();
		int num = 0;
		try
		{
			int num2 = 0;
			int count = W7nebvOpd8.SourceFiles.Count;
			int resolve = DataHelper.GetResolve(count);
			ImportFileItem[] array = W7nebvOpd8.SourceFiles.ToArray();
			foreach (ImportFileItem importFileItem in array)
			{
				if (AFPeJPTB0L(importFileItem.FullPath, importFileItem.TreeFullPath))
				{
					num++;
				}
				if (num2 % resolve == 0)
				{
					ri7eRklf95.ProgressUpdate((float)num2 / (float)count);
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

	private ResultData<int> WyReNlOwhQ()
	{
		ResultData<int> resultData = new ResultData<int>();
		int num = 0;
		try
		{
			int num2 = 0;
			int count = W7nebvOpd8.SourceFiles.Count;
			int resolve = DataHelper.GetResolve(count);
			foreach (ImportFileItem sourceFile in W7nebvOpd8.SourceFiles)
			{
				if (AFPeJPTB0L(sourceFile.FullPath, sourceFile.TreeFullPath))
				{
					num++;
				}
				if (num2 % resolve == 0)
				{
					ri7eRklf95.ProgressUpdate(num2 * 100 / count);
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

	private bool AFPeJPTB0L(string P_0, string P_1)
	{
		PvfFile file = RsYeSQkwUk.GetFile(P_1);
		using FileStream fileStream = File.OpenRead(P_0);
		if (file != null)
		{
			switch (W7nebvOpd8.Operation)
			{
			case FileOperation.Rename:
			{
				int num = 0;
				string text = P_1;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
				defaultInterpolatedStringHandler.AppendLiteral("(");
				defaultInterpolatedStringHandler.AppendFormatted(num);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				string text2 = text + defaultInterpolatedStringHandler.ToStringAndClear();
				while (RsYeSQkwUk.FileAny(text2))
				{
					num++;
					string text3 = P_1;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(2, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("(");
					defaultInterpolatedStringHandler2.AppendFormatted(num);
					defaultInterpolatedStringHandler2.AppendLiteral(")");
					text2 = text3 + defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				P_1 = text2;
				return DknerAuSDM(P_1, fileStream);
			}
			case FileOperation.Skip:
				return false;
			case FileOperation.Cancel:
				throw new Exception(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileExists2"), P_1));
			default:
				return RsYeSQkwUk.ImportUpdateFile(file, fileStream, P_1, W7nebvOpd8.CompileScript, W7nebvOpd8.CompileBinaryAni, W7nebvOpd8.ConvertToTraditionalChinese, W7nebvOpd8.CompileChinaPvfScriptFile, W7nebvOpd8.CompileChinaAni);
			}
		}
		return DknerAuSDM(P_1, fileStream);
	}

	private bool DknerAuSDM(string P_0, Stream P_1)
	{
		PvfFile pvfFile = new PvfFile(P_0);
		bool flag = RsYeSQkwUk.ImportUpdateFile(pvfFile, P_1, P_0, W7nebvOpd8.CompileScript, W7nebvOpd8.CompileBinaryAni, W7nebvOpd8.ConvertToTraditionalChinese, W7nebvOpd8.CompileChinaPvfScriptFile, W7nebvOpd8.CompileChinaAni);
		if (flag)
		{
			P_0 = pvfFile.FileName;
			lock (this)
			{
				RsYeSQkwUk.FileList.TryAdd(P_0, pvfFile);
				WZPeks0hR8.Add(P_0);
			}
		}
		return flag;
	}
}
